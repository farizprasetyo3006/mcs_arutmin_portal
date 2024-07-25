using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Entity;
using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;
using Common;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;
using Npoi.Mapper;
using Npoi.Mapper.Attributes;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MCSWebApp.Areas.General.Controllers
{
    [Area("General")]
    public class BankAccountController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public BankAccountController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.BankAccount];
            ViewBag.BreadcrumbCode = WebAppMenu.BankAccount;

            return View();
        }

        public async Task<IActionResult> Detail(string Id)
        {
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.BankAccount];
            ViewBag.BreadcrumbCode = WebAppMenu.BankAccount;

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var svc = new BankAccount(CurrentUserContext);
                    var record = await svc.GetByIdAsync(Id);
                    if (record != null)
                    {
                        ViewBag.Id = Id;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "BankAccount.xlsx";
            sFileName = sFileName.Insert(sFileName.LastIndexOf("."), string.Format("_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff")));

            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);

            FileInfo file = new FileInfo(Path.Combine(FilePath, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(FilePath, sFileName), FileMode.Create, FileAccess.Write))
            {
                int RowCount = 1;
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("BankAccount");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Bank Code");
                row.CreateCell(1).SetCellValue("Account Number");
                row.CreateCell(2).SetCellValue("Account Holder");
                row.CreateCell(3).SetCellValue("Swift Code");
                row.CreateCell(4).SetCellValue("Branch Information");
                row.CreateCell(5).SetCellValue("Currency");
                row.CreateCell(6).SetCellValue("Is Active");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.bank_account.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var bank_code = "";
                    var bank = dbFind.bank.Where(o => o.id == Value.bank_id).FirstOrDefault();
                    if (bank != null) bank_code = bank.bank_code.ToString();

                    var currency_name = "";
                    var currency = dbFind.currency.Where(o => o.id == Value.currency_id).FirstOrDefault();
                    if (currency != null) currency_name = currency.currency_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(bank_code);
                    row.CreateCell(1).SetCellValue(Value.account_number);
                    row.CreateCell(2).SetCellValue(Value.account_holder);
                    row.CreateCell(3).SetCellValue(Value.swift_code);
                    row.CreateCell(4).SetCellValue(Value.branch_information);
                    row.CreateCell(5).SetCellValue(currency_name);
                    row.CreateCell(6).SetCellValue(Convert.ToBoolean(Value.is_active));
                    RowCount++;
                }
                workbook.Write(fs);
                using (var stream = new FileStream(Path.Combine(FilePath, sFileName), FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                //Throws Generated file to Browser
                try
                {
                    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
                }
                // Deletes the generated file from /wwwroot folder
                finally
                {
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }

    }
}

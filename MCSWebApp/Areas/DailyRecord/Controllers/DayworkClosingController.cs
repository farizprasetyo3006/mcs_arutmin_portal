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

namespace MCSWebApp.Areas.DailyRecord.Controllers
{
    [Area("DailyRecord")]
    public class DayworkClosingController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public DayworkClosingController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DailyRecord];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DayworkClosing];
            ViewBag.BreadcrumbCode = WebAppMenu.DayworkClosing;

            return View();
        }

        public IActionResult Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DailyRecord];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DayworkClosing];
            ViewBag.BreadcrumbCode = WebAppMenu.DayworkClosing;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "DayworkClosing.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("DayworkClosing");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("Transaction Date");
                row.CreateCell(2).SetCellValue("Accounting Period");
                row.CreateCell(3).SetCellValue("Advance Contract");
                row.CreateCell(4).SetCellValue("Advance Contract Reference");
                row.CreateCell(5).SetCellValue("Customer");
                row.CreateCell(6).SetCellValue("Reference Number");
                row.CreateCell(7).SetCellValue("From Date");
                row.CreateCell(8).SetCellValue("To Date");
                row.CreateCell(9).SetCellValue("Total HM");
                row.CreateCell(10).SetCellValue("Total Value");
                row.CreateCell(11).SetCellValue("Note");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.daywork_closing.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    var accounting_period_name = "";
                    var accounting_period = dbFind.accounting_period.Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                        o.id == baris.accounting_period_id).FirstOrDefault();
                    if (accounting_period != null) accounting_period_name = accounting_period.accounting_period_name.ToString();

                    var advance_contract_number = "";
                    var advance_contract = dbFind.advance_contract
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.id == baris.advance_contract_id).FirstOrDefault();
                    if (advance_contract != null) advance_contract_number = advance_contract.advance_contract_number.ToString();

                    var advance_contract_reference_name = "";
                    var advance_contract_reference = dbFind.advance_contract_reference
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.id == baris.advance_contract_reference_id).FirstOrDefault();
                    if (advance_contract_reference != null) advance_contract_reference_name = advance_contract_reference.name.ToString();

                    var customer_code = "";
                    var customer = dbFind.customer
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.id == baris.customer_id).FirstOrDefault();
                    if (customer != null) customer_code = customer.business_partner_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.transaction_number);
                    row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(baris.transaction_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(2).SetCellValue(accounting_period_name);
                    row.CreateCell(3).SetCellValue(advance_contract_number);
                    row.CreateCell(4).SetCellValue(advance_contract_reference_name);
                    row.CreateCell(5).SetCellValue(customer_code);
                    row.CreateCell(6).SetCellValue(baris.reference_number);
                    row.CreateCell(7).SetCellValue(" " + Convert.ToDateTime(baris.from_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(8).SetCellValue(" " + Convert.ToDateTime(baris.to_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(9).SetCellValue(PublicFunctions.Pecahan(baris.total_hm));
                    row.CreateCell(10).SetCellValue(PublicFunctions.Pecahan(baris.total_value));
                    row.CreateCell(11).SetCellValue(baris.note);

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

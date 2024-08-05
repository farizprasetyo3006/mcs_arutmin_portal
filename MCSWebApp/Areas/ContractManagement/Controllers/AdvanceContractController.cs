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
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.ContractManagement.Controllers
{
    [Area("ContractManagement")]
    public class AdvanceContractController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public AdvanceContractController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ContractManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.AdvanceContract];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.AdvanceContract];
            ViewBag.BreadcrumbCode = WebAppMenu.AdvanceContract;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ItemDetail(String Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ContractManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.AdvanceContractIndex];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.AdvanceContractIndex];
            ViewBag.BreadcrumbCode = WebAppMenu.AdvanceContractItemDetail;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var advanceContractItemRecord = await dbContext.vw_advance_contract_item
                    .Where(o => o.id == Id).FirstOrDefaultAsync();

                    if (advanceContractItemRecord != null)
                    {
                        ViewBag.AdvanceContractItem = advanceContractItemRecord;
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
            string sFileName = "AdvanceContract.xlsx";
            sFileName = sFileName.Insert(sFileName.LastIndexOf("."), string.Format("_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff")));

            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);

            FileInfo file = new FileInfo(Path.Combine(FilePath, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(FilePath, sFileName), FileMode.Create, FileAccess.Write))
            {
                int RowCount = 1;
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("AdvanceContract");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Contract Number");
                row.CreateCell(1).SetCellValue("Version");
                row.CreateCell(2).SetCellValue("Description");
                row.CreateCell(3).SetCellValue("Reference Number");
                row.CreateCell(4).SetCellValue("Contract Type");
                row.CreateCell(5).SetCellValue("Contract Target");
                row.CreateCell(6).SetCellValue("Start Date");
                row.CreateCell(7).SetCellValue("End Date");
                row.CreateCell(8).SetCellValue("Contract Value");
                row.CreateCell(9).SetCellValue("Contract Currency");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.advance_contract.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.start_date);
                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    var contractor_code = "";
                    if (baris.contract_type == "AR")
                    {
                        var customer = dbFind.customer.Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.id == baris.contractor_id).FirstOrDefault();
                        if (customer != null) contractor_code = customer.business_partner_code.ToString();
                    }
                    else
                    {
                        var contractor = dbFind.contractor.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.id == baris.contractor_id).FirstOrDefault();
                        if (contractor != null) contractor_code = contractor.business_partner_code.ToString();
                    }

                    var currency_code = "";
                    var currency = dbFind.currency.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.id == baris.contract_currency_id).FirstOrDefault();
                    if (currency != null) currency_code = currency.currency_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.advance_contract_number);
                    row.CreateCell(1).SetCellValue(Convert.ToDouble(baris.version));
                    row.CreateCell(2).SetCellValue(baris.note);
                    row.CreateCell(3).SetCellValue(baris.reference_number);
                    row.CreateCell(4).SetCellValue(baris.contract_type);
                    row.CreateCell(5).SetCellValue(contractor_code);
                    row.CreateCell(6).SetCellValue(" " + Convert.ToDateTime(baris.start_date).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(7).SetCellValue(" " + Convert.ToDateTime(baris.end_date).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(baris.contract_value));
                    row.CreateCell(9).SetCellValue(currency_code);

                    RowCount++;
                    if (RowCount > 50) break;
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

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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.ContractManagement.Controllers
{
    [Area("ContractManagement")]
    public class AdvanceContractValuationController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public AdvanceContractValuationController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Contractor];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ContractManagement];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.AdvanceContractValuation];
            ViewBag.BreadcrumbCode = WebAppMenu.AdvanceContractValuation;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "AdvanceContractReference.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("AdvanceContractReference");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Advance Contract");
                row.CreateCell(1).SetCellValue("Progress Claim Name");
                row.CreateCell(2).SetCellValue("Accounting Period");
                row.CreateCell(3).SetCellValue("Start Date");
                row.CreateCell(4).SetCellValue("End Date");
                row.CreateCell(5).SetCellValue("Target Qty");
                row.CreateCell(6).SetCellValue("Target Unit");
                row.CreateCell(7).SetCellValue("Actual Qty");
                row.CreateCell(8).SetCellValue("Actual Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.advance_contract_reference.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.start_date);
                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    var advance_contract_number = "";
                    var advance_contract = dbFind.advance_contract.Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                        o.id == baris.advance_contract_id).FirstOrDefault();
                    if (advance_contract != null) advance_contract_number = advance_contract.advance_contract_number.ToString();

                    var accounting_period_name = "";
                    var accounting_period = dbFind.accounting_period.Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                        o.id == baris.accounting_period_id).FirstOrDefault();
                    if (accounting_period != null) accounting_period_name = accounting_period.accounting_period_name.ToString();

                    var target_uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                        o.id == baris.target_uom_id).FirstOrDefault();
                    if (uom != null) target_uom_symbol = uom.uom_symbol.ToString();

                    var actual_uom_symbol = "";
                    uom = dbFind.uom.Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                        o.id == baris.actual_uom_id).FirstOrDefault();
                    if (uom != null) actual_uom_symbol = uom.uom_symbol.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(advance_contract_number);
                    row.CreateCell(1).SetCellValue(baris.progress_claim_name);
                    row.CreateCell(2).SetCellValue(accounting_period_name);
                    row.CreateCell(3).SetCellValue(" " + Convert.ToDateTime(baris.start_date).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(4).SetCellValue(" " + Convert.ToDateTime(baris.end_date).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(baris.target_quantity));
                    row.CreateCell(6).SetCellValue(target_uom_symbol);
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(baris.actual_quantity));
                    row.CreateCell(8).SetCellValue(actual_uom_symbol);

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

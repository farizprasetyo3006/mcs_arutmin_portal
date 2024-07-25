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
using Npoi.Mapper;
using Npoi.Mapper.Attributes;

namespace MCSWebApp.Areas.Planning.Controllers
{
    [Area("Planning")]
    public class ProductionPlanBackupController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ProductionPlanBackupController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionPlan];
            ViewBag.BreadcrumbCode = WebAppMenu.ProductionPlan;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "ProductionPlan.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("ProductionPlan");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Production Plan Number");
                row.CreateCell(1).SetCellValue("Start Date");
                row.CreateCell(2).SetCellValue("End Date");
                row.CreateCell(3).SetCellValue("Quantity");
                row.CreateCell(4).SetCellValue("Prev. Qty");
                row.CreateCell(5).SetCellValue("Unit");
                row.CreateCell(6).SetCellValue("Budget");
                row.CreateCell(7).SetCellValue("Currency");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var header = dbContext.production_plan.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var Value in header)
                {
                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == Value.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    var currency_code = "";
                    var currency = dbFind.currency.Where(o => o.id == Value.budget_currency_id).FirstOrDefault();
                    if (currency != null) currency_code = currency.currency_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(Value.production_plan_number);
                    row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(Value.start_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(2).SetCellValue(" " + Convert.ToDateTime(Value.end_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(Value.quantity));
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(Value.previous_quantity));
                    row.CreateCell(5).SetCellValue(uom_symbol);
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(Value.budget_amount));
                    row.CreateCell(7).SetCellValue(currency_code);
                    RowCount++;
                }

                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Production Plan Number");
                row.CreateCell(1).SetCellValue("Plan Date");
                row.CreateCell(2).SetCellValue("Percentage");
                row.CreateCell(3).SetCellValue("Quantity");
                row.CreateCell(4).SetCellValue("Previous Quantity");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.production_plan_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var Value in detail)
                {
                    var production_plan_number = "";
                    var production_plan = dbFind.production_plan.Where(o => o.id == Value.production_plan_id).FirstOrDefault();
                    if (production_plan != null) production_plan_number = production_plan.production_plan_number.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(production_plan_number);
                    row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(Value.plan_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(Value.percentage));
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(Value.quantity));
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(Value.previous_quantity));
                    RowCount++;
                }
                //****************

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

                finally
                {
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }

    }
}

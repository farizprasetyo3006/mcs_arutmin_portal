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
using Microsoft.EntityFrameworkCore;

namespace MCSWebApp.Areas.Planning.Controllers
{
    [Area("Planning")]
    public class SalesPlanSnapshotController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SalesPlanSnapshotController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesPlanSnapshot];
            ViewBag.BreadcrumbCode = WebAppMenu.SalesPlanSnapshot;

            return View();
        }

        public async Task<IActionResult> Detail(String Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesPlanSnapshot];
            ViewBag.BreadcrumbCode = WebAppMenu.SalesPlanSnapshot;

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var salesPlanSnapshot = await dbContext.sales_plan_snapshot
                    .Where(o => o.id == Id).FirstOrDefaultAsync();

                    if(salesPlanSnapshot != null)
                    {
                        ViewBag.SalesPlanSnapshot = salesPlanSnapshot;
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
            string sFileName = "SalesPlan.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("SalesPlan");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sales Plan Number");
                row.CreateCell(1).SetCellValue("Start Date");
                row.CreateCell(2).SetCellValue("End Date");
                row.CreateCell(3).SetCellValue("Quantity");
                row.CreateCell(4).SetCellValue("Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var header = dbContext.sales_plan.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var isi in header)
                {
                    var plan_name = "";
                    var master_list = dbFind.master_list.Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                        && o.id == isi.plan_year_id).FirstOrDefault();
                    if (master_list != null) plan_name = master_list.item_name.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                        && o.id == isi.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(plan_name);
                    row.CreateCell(1).SetCellValue(isi.created_on.ToString());
                    row.CreateCell(2).SetCellValue(isi.created_by.ToString());
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(isi.quantity));
                    row.CreateCell(4).SetCellValue(uom_symbol);
                    RowCount++;
                }

                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Sales Plan Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sales Plan Name");
                row.CreateCell(1).SetCellValue("Month");
                row.CreateCell(2).SetCellValue("Quantity");
                row.CreateCell(3).SetCellValue("Percentage");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.sales_plan_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var isi in detail)
                {
                    var plan_name = "";
                    var sales_plan = dbFind.vw_sales_plan.Where(o => o.id == isi.sales_plan_id).FirstOrDefault();
                    if (sales_plan != null) plan_name = sales_plan.plan_year.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(plan_name);
                    row.CreateCell(1).SetCellValue(Convert.ToInt32(isi.month_id));  //(isi.month_index));
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(isi.quantity));
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(1));  //(isi.percentage));
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

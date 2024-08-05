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
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Planning.Controllers
{
    [Area("Planning")]
    public class HaulingPlanController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public HaulingPlanController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.HaulingPlan];
            ViewBag.BreadcrumbCode = WebAppMenu.HaulingPlan;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public IActionResult Report()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.HaulingPlan];
            ViewBag.BreadcrumbCode = WebAppMenu.HaulingPlan;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

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
                ISheet excelSheet = workbook.CreateSheet("Sales Plan");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sales Plan Name");
                row.CreateCell(1).SetCellValue("Site");
                row.CreateCell(2).SetCellValue("Revision Number");
                row.CreateCell(3).SetCellValue("Created Date");
                row.CreateCell(4).SetCellValue("Last Modified Date");
                row.CreateCell(5).SetCellValue("Quantity");
                row.CreateCell(6).SetCellValue("Unit");
                row.CreateCell(7).SetCellValue("Is Baseline");
                row.CreateCell(8).SetCellValue("Is Locked");
                row.CreateCell(9).SetCellValue("Remark");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var header = dbContext.sales_plan.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var baris in header)
                {
                    var plan_name = "";
                    var master_list = dbFind.master_list.Where(o => o.id == baris.plan_year_id).FirstOrDefault();
                    if (master_list != null) plan_name = master_list.item_name.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == baris.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    var site_name = "";
                    var master_list2 = dbFind.master_list.Where(o => o.id == baris.site_id).FirstOrDefault();
                    if (master_list2 != null) site_name = master_list2.item_name.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(plan_name);
                    row.CreateCell(1).SetCellValue(site_name);
                    row.CreateCell(2).SetCellValue(baris.revision_number.ToString());
                    row.CreateCell(3).SetCellValue(" " + Convert.ToDateTime(baris.created_on).ToString("yyyy-MM-dd"));
                    row.CreateCell(4).SetCellValue(" " + Convert.ToDateTime(baris.modified_on).ToString("yyyy-MM-dd"));
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(baris.quantity));
                    row.CreateCell(6).SetCellValue(uom_symbol);
                    row.CreateCell(7).SetCellValue(Convert.ToBoolean(baris.is_baseline));
                    row.CreateCell(8).SetCellValue(Convert.ToBoolean(baris.is_locked));
                    row.CreateCell(9).SetCellValue(baris.notes);

                    RowCount++;
                    if (RowCount > 50) break;
                }

                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Sales Plan Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sales Plan Number");
                row.CreateCell(1).SetCellValue("Month");
                row.CreateCell(2).SetCellValue("Quantity");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.sales_plan_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var baris in detail)
                {
                    var plan_name = "";
                    var sales_plan = dbFind.vw_sales_plan.Where(o => o.id == baris.sales_plan_id).FirstOrDefault();
                    if (sales_plan != null) plan_name = sales_plan.plan_year.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(plan_name);
                    row.CreateCell(1).SetCellValue(Convert.ToInt32(baris.month_id));  //(isi.month_index));
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(baris.quantity));

                    RowCount++;
                    if (RowCount > 50) break;
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

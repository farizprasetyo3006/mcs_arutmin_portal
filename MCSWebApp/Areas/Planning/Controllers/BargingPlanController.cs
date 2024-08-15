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
using Microsoft.EntityFrameworkCore;

namespace MCSWebApp.Areas.Planning.Controllers
{
    [Area("Planning")]
    public class BargingPlanController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public BargingPlanController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.BargingPlan];
            ViewBag.BreadcrumbCode = WebAppMenu.BargingPlan;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public IActionResult Report()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.BargingPlan];
            ViewBag.BreadcrumbCode = WebAppMenu.BargingPlan;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "BargingPlan.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Barging Plan");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Planning Number");
                row.CreateCell(1).SetCellValue("Location");
                row.CreateCell(2).SetCellValue("Year");
                row.CreateCell(3).SetCellValue("Plan Type");
                row.CreateCell(4).SetCellValue("Product");
                row.CreateCell(5).SetCellValue("Product Category");
                row.CreateCell(6).SetCellValue("Contractor");
                row.CreateCell(7).SetCellValue("Business Unit");
                row.CreateCell(8).SetCellValue("Month");
                row.CreateCell(9).SetCellValue("Quantity");
                row.CreateCell(10).SetCellValue("Activity Plan");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                mcsContext dbFind2 = new mcsContext(DbOptionBuilder.Options);

                //  var business_unit1 = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var header = dbContext.barging_plan.Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.created_on);
                    dynamic detail;
                    // Inserting values to table
                    foreach (var baris in header)
                    {
                        detail = dbFind2.vw_barging_plan_monthly.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.barging_plan_id == baris.id)
                        .OrderByDescending(o => o.created_on);
                        foreach (var bar in detail)
                        {
                            var business_area_code = "";
                            var business_area = dbFind.business_area
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.id == baris.location_id).FirstOrDefault();
                            if (business_area != null) business_area_code = business_area.business_area_code.ToString();

                            var year = "";
                            var master = dbFind.master_list.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == baris.master_list_id).FirstOrDefault();
                            if (master != null) year = master.item_name.ToString();

                            var seam = "";
                            var seam_id = dbFind.mine_location.Where(o => o.id == baris.mine_location_id).FirstOrDefault();
                            if (seam_id != null) seam = seam_id.mine_location_code.ToString();

                            var plan_type = "";
                            var type = dbFind.master_list.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == baris.plan_type).FirstOrDefault();
                            if (type != null) plan_type = type.item_name.ToString();

                            /* var activity_plan = "";
                             var activity = dbFind.master_list.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == baris.activity_plan).FirstOrDefault();
                             if (activity != null) activity_plan = activity.item_name.ToString();*/

                            var product_name = "";
                            var product = dbFind.product.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == baris.product_id).FirstOrDefault();
                            if (product != null) product_name = product.product_code;

                            var business_unit = "";
                            var BU = dbFind.business_unit.Where(o => o.id == baris.business_unit_id).FirstOrDefault();
                            if (BU != null) business_unit = BU.business_unit_code.ToString();

                            var contractor = "";
                            var c = await dbFind.contractor.Where(o => o.id == baris.contractor_id).FirstOrDefaultAsync();
                            if (c != null) contractor = c.business_partner_code.ToString();

                            var productCategory = "";
                            var pc = await dbFind.product_category.Where(o => o.id == baris.product_category_id).FirstOrDefaultAsync();
                            if (pc != null) productCategory = pc.product_category_code.ToString();

                            var activityPlan = "";
                            var ap = dbFind.master_list.Where(o => o.item_group == "activity-plan-type" && o.id == baris.activity_plan).FirstOrDefault();
                            if (ap != null) activityPlan = ap.item_name.ToString();

                            row = excelSheet.CreateRow(RowCount);
                            row.CreateCell(0).SetCellValue(baris.barging_plan_number);
                            row.CreateCell(1).SetCellValue(business_area_code);
                            row.CreateCell(2).SetCellValue(year);
                            row.CreateCell(3).SetCellValue(plan_type);
                            row.CreateCell(4).SetCellValue(product_name);
                            row.CreateCell(5).SetCellValue(productCategory);
                            row.CreateCell(6).SetCellValue(contractor);
                            row.CreateCell(7).SetCellValue(business_unit);
                            row.CreateCell(8).SetCellValue(Convert.ToDouble(bar.month_id));
                            row.CreateCell(9).SetCellValue(Convert.ToDouble(bar.quantity));
                            row.CreateCell(10).SetCellValue(activityPlan);

                            RowCount++;
                           // if (RowCount > 50) break;
                        }
                    }
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

                finally
                {
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }

    }
}

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
using NPOI.Util;
using BusinessLogic;
using Microsoft.EntityFrameworkCore;
using DataAccess.Repository;

namespace MCSWebApp.Areas.Planning.Controllers
{
    [Area("Planning")]
    public class EightWeekForecastController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public EightWeekForecastController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.EightWeekForecast];
            ViewBag.BreadcrumbCode = WebAppMenu.EightWeekForecast;

            return View();
        }

        public IActionResult Report()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.EightWeekForecast];
            ViewBag.BreadcrumbCode = WebAppMenu.EightWeekForecast;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            var result = new StandardResult();

            string sFileName = "EightWeekForecast.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Eight Week Forecast");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Business Unit");
                row.CreateCell(1).SetCellValue("Planning Number");
                row.CreateCell(2).SetCellValue("Year");
                row.CreateCell(3).SetCellValue("Activity Plan");
                row.CreateCell(4).SetCellValue("Location");
                row.CreateCell(5).SetCellValue("Pit");
                row.CreateCell(6).SetCellValue("Product Category");
                row.CreateCell(7).SetCellValue("Contractor");
                row.CreateCell(8).SetCellValue("UOM");
                row.CreateCell(9).SetCellValue("Week");
                row.CreateCell(10).SetCellValue("From Date");
                row.CreateCell(11).SetCellValue("To Date");
                row.CreateCell(12).SetCellValue("Product");
                row.CreateCell(13).SetCellValue("Quantity");
                row.CreateCell(14).SetCellValue("Ash (adb)");
                row.CreateCell(15).SetCellValue("TS (adb)");
                row.CreateCell(16).SetCellValue("IM (adb)");
                row.CreateCell(17).SetCellValue("TM (arb)");
                row.CreateCell(18).SetCellValue("CV gad");
                row.CreateCell(19).SetCellValue("CV gar");
                row.CreateCell(20).SetCellValue("Is Using");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                
                List<eight_week_forecast> headers = null;
                if (CurrentUserContext.IsSysAdmin) headers = await dbContext.eight_week_forecast
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on)
                    .ToListAsync();

                else headers = await dbContext.eight_week_forecast
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == business_unit_id)
                    .OrderByDescending(o => o.created_on)
                    .ToListAsync();

                foreach (var header in headers)
                {
                    var items = await dbContext.eight_week_forecast_item
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.header_id == header.id)
                        .OrderByDescending(o => o.created_on)
                        .ToListAsync();

                    var business_unit_name = string.Empty;
                    var business_unit = await dbContext.business_unit
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.id == header.business_unit_id)
                        .FirstOrDefaultAsync();
                    if (business_unit != null) business_unit_name = business_unit.business_unit_name;
                    
                    var year_number = string.Empty;
                    var year = await dbContext.master_list
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.id == header.year_id)
                        .FirstOrDefaultAsync();
                    if (year != null) year_number = year.item_name;

                    foreach (var item in items)
                    {
                        var details = await dbContext.eight_week_forecast_item_detail
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.item_id == item.id)
                            .OrderByDescending(o => o.created_on)
                            .ToListAsync();

                        var activity_name = string.Empty;
                        var activity = await dbContext.master_list
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.id == item.activity_plan_id)
                            .FirstOrDefaultAsync();
                        if (activity != null) activity_name = activity.item_name;

                        var location_name = string.Empty;
                        var location = await dbContext.vw_business_area_breakdown_structure
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.child_2 != null)
                            .Where(o => o.child_3 == null)
                            .Where(o => o.id == item.location_id)
                            .FirstOrDefaultAsync();
                        if (location != null) location_name = location.business_area_name;

                        var pit_name = string.Empty;
                        var pit = await dbContext.vw_business_area_breakdown_structure
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.child_4 != null)
                            .Where(o => o.child_5 == null)
                            .Where(o => o.id == item.business_area_pit_id)
                            .FirstOrDefaultAsync();
                        if (pit != null) pit_name = pit.business_area_name;

                        var product_category_name = string.Empty;
                        var product_category = await dbContext.product_category
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.id == item.product_category_id)
                            .FirstOrDefaultAsync();
                        if (product_category != null) product_category_name = product_category.product_category_name;

                        var contractor_name = string.Empty;
                        var contractor = await dbContext.contractor
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.id == item.contractor_id)
                            .FirstOrDefaultAsync();
                        if (contractor != null) contractor_name = contractor.business_partner_name;

                        var uom_symbol = string.Empty;
                        var uom = await dbContext.uom
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.id == item.uom_id)
                            .FirstOrDefaultAsync();
                        if (uom != null) uom_symbol = uom.uom_symbol;

                        foreach (var detail in details)
                        {
                            var product_name = string.Empty;
                            var product = await dbContext.product
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.id == detail.product_id)
                                .FirstOrDefaultAsync();
                            if (product != null) product_name = product.product_name;

                            row = excelSheet.CreateRow(RowCount);
                            row.CreateCell(0).SetCellValue(business_unit_name);
                            row.CreateCell(1).SetCellValue(header.planning_number);
                            row.CreateCell(2).SetCellValue(year_number);
                            row.CreateCell(3).SetCellValue(activity_name);
                            row.CreateCell(4).SetCellValue(location_name);
                            row.CreateCell(5).SetCellValue(pit_name);
                            row.CreateCell(6).SetCellValue(product_category_name);
                            row.CreateCell(7).SetCellValue(contractor_name);
                            row.CreateCell(8).SetCellValue(uom_symbol);
                            row.CreateCell(9).SetCellValue(Convert.ToDouble(detail.week_id));
                            row.CreateCell(10).SetCellValue(" " + detail.from_date.Value.ToString("yyyy-MM-dd"));
                            row.CreateCell(11).SetCellValue(" " + detail.to_date.Value.ToString("yyyy-MM-dd"));
                            row.CreateCell(12).SetCellValue(product_name);
                            row.CreateCell(13).SetCellValue(Convert.ToDouble(detail.quantity));
                            row.CreateCell(14).SetCellValue(Convert.ToDouble(detail.ash_adb));
                            row.CreateCell(15).SetCellValue(Convert.ToDouble(detail.ts_adb));
                            row.CreateCell(16).SetCellValue(Convert.ToDouble(detail.im_adb));
                            row.CreateCell(17).SetCellValue(Convert.ToDouble(detail.tm_arb));
                            row.CreateCell(18).SetCellValue(Convert.ToDouble(detail.gcv_gad));
                            row.CreateCell(19).SetCellValue(Convert.ToDouble(detail.gcv_gar));
                            row.CreateCell(20).SetCellValue(PublicFunctions.BenarSalah(detail.is_using));

                            RowCount++;
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

        public async Task<IActionResult> ExcelExportSelected([FromBody] dynamic Data)
        {
            var result = new StandardResult();

            string sFileName = "EightWeekForecast.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Eight Week Forecast");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Business Unit");
                row.CreateCell(1).SetCellValue("Planning Number");
                row.CreateCell(2).SetCellValue("Year");
                row.CreateCell(3).SetCellValue("Activity Plan");
                row.CreateCell(4).SetCellValue("Location");
                row.CreateCell(5).SetCellValue("Pit");
                row.CreateCell(6).SetCellValue("Product Category");
                row.CreateCell(7).SetCellValue("Contractor");
                row.CreateCell(8).SetCellValue("UOM");
                row.CreateCell(9).SetCellValue("Week");
                row.CreateCell(10).SetCellValue("From Date");
                row.CreateCell(11).SetCellValue("To Date");
                row.CreateCell(12).SetCellValue("Product");
                row.CreateCell(13).SetCellValue("Quantity");
                row.CreateCell(14).SetCellValue("Ash (adb)");
                row.CreateCell(15).SetCellValue("TS (adb)");
                row.CreateCell(16).SetCellValue("IM (adb)");
                row.CreateCell(17).SetCellValue("TM (arb)");
                row.CreateCell(18).SetCellValue("CV gad");
                row.CreateCell(19).SetCellValue("CV gar");
                row.CreateCell(20).SetCellValue("Is Using");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                List<eight_week_forecast> headers = null;

                var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                headers = await dbContext.eight_week_forecast
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.created_on)
                    .ToListAsync();

                foreach (var header in headers)
                {
                    var items = await dbContext.eight_week_forecast_item
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.header_id == header.id)
                        .OrderByDescending(o => o.created_on)
                        .ToListAsync();

                    var business_unit_name = string.Empty;
                    var business_unit = await dbContext.business_unit
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.id == header.business_unit_id)
                        .FirstOrDefaultAsync();
                    if (business_unit != null) business_unit_name = business_unit.business_unit_name;

                    var year_number = string.Empty;
                    var year = await dbContext.master_list
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.id == header.year_id)
                        .FirstOrDefaultAsync();
                    if (year != null) year_number = year.item_name;

                    foreach (var item in items)
                    {
                        var details = await dbContext.eight_week_forecast_item_detail
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.item_id == item.id)
                            .OrderByDescending(o => o.created_on)
                            .ToListAsync();

                        var activity_name = string.Empty;
                        var activity = await dbContext.master_list
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.id == item.activity_plan_id)
                            .FirstOrDefaultAsync();
                        if (activity != null) activity_name = activity.item_name;

                        var location_name = string.Empty;
                        var location = await dbContext.vw_business_area_breakdown_structure
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.child_2 != null)
                            .Where(o => o.child_3 == null)
                            .Where(o => o.id == item.location_id)
                            .FirstOrDefaultAsync();
                        if (location != null) location_name = location.business_area_name;

                        var pit_name = string.Empty;
                        var pit = await dbContext.vw_business_area_breakdown_structure
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.child_4 != null)
                            .Where(o => o.child_5 == null)
                            .Where(o => o.id == item.business_area_pit_id)
                            .FirstOrDefaultAsync();
                        if (pit != null) pit_name = pit.business_area_name;

                        var product_category_name = string.Empty;
                        var product_category = await dbContext.product_category
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.id == item.product_category_id)
                            .FirstOrDefaultAsync();
                        if (product_category != null) product_category_name = product_category.product_category_name;

                        var contractor_name = string.Empty;
                        var contractor = await dbContext.contractor
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.id == item.contractor_id)
                            .FirstOrDefaultAsync();
                        if (contractor != null) contractor_name = contractor.business_partner_name;

                        var uom_symbol = string.Empty;
                        var uom = await dbContext.uom
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.id == item.uom_id)
                            .FirstOrDefaultAsync();
                        if (uom != null) uom_symbol = uom.uom_symbol;

                        foreach (var detail in details)
                        {
                            var product_name = string.Empty;
                            var product = await dbContext.product
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.id == detail.product_id)
                                .FirstOrDefaultAsync();
                            if (product != null) product_name = product.product_name;

                            row = excelSheet.CreateRow(RowCount);
                            row.CreateCell(0).SetCellValue(business_unit_name);
                            row.CreateCell(1).SetCellValue(header.planning_number);
                            row.CreateCell(2).SetCellValue(year_number);
                            row.CreateCell(3).SetCellValue(activity_name);
                            row.CreateCell(4).SetCellValue(location_name);
                            row.CreateCell(5).SetCellValue(pit_name);
                            row.CreateCell(6).SetCellValue(product_category_name);
                            row.CreateCell(7).SetCellValue(contractor_name);
                            row.CreateCell(8).SetCellValue(uom_symbol);
                            row.CreateCell(9).SetCellValue(Convert.ToDouble(detail.week_id));
                            var fromDateCell = row.CreateCell(10);
                            fromDateCell.SetCellValue(detail.from_date ?? DateTime.MinValue); // Handle null values
                            var fromDateCellStyle = workbook.CreateCellStyle();
                            fromDateCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("m/d/yyyy");
                            fromDateCell.CellStyle = fromDateCellStyle;

                            // Set date format for to_date field
                            var toDateCell = row.CreateCell(11);
                            toDateCell.SetCellValue(detail.to_date ?? DateTime.MinValue); // Handle null values
                            var toDateCellStyle = workbook.CreateCellStyle();
                            toDateCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("m/d/yyyy");
                            toDateCell.CellStyle = toDateCellStyle;
                            row.CreateCell(12).SetCellValue(product_name);
                            row.CreateCell(13).SetCellValue(Convert.ToDouble(detail.quantity));
                            row.CreateCell(14).SetCellValue(Convert.ToDouble(detail.ash_adb));
                            row.CreateCell(15).SetCellValue(Convert.ToDouble(detail.ts_adb));
                            row.CreateCell(16).SetCellValue(Convert.ToDouble(detail.im_adb));
                            row.CreateCell(17).SetCellValue(Convert.ToDouble(detail.tm_arb));
                            row.CreateCell(18).SetCellValue(Convert.ToDouble(detail.gcv_gad));
                            row.CreateCell(19).SetCellValue(Convert.ToDouble(detail.gcv_gar));
                            row.CreateCell(20).SetCellValue(PublicFunctions.BenarSalah(detail.is_using));

                            RowCount++;
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

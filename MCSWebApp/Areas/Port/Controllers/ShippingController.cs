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
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Port.Controllers
{
    [Area("Port")]
    public class ShippingController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ShippingController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Port];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.PortShipping];
            ViewBag.BreadcrumbCode = WebAppMenu.PortShipping;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public IActionResult Loading()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Port];
            ViewBag.SubAreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.PortShipping];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ShipLoading];
            ViewBag.BreadcrumbCode = WebAppMenu.ShipLoading;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public IActionResult Unloading()
        {
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Port];
            ViewBag.SubAreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.PortShipping];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ShipUnloading];
            ViewBag.BreadcrumbCode = WebAppMenu.ShipUnloading;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> LoadingExcelExport([FromBody] dynamic Data)
        {
            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);
            string sFileName = "ShippingLoading.xlsx";
            sFileName = sFileName.Insert(sFileName.LastIndexOf("."), string.Format("_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff")));

            FileInfo file = new FileInfo(Path.Combine(FilePath, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(FilePath, sFileName), FileMode.Create, FileAccess.Write))
            {
                int RowCount = 1;
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("ShippingLoading");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("Shipping Order");
                row.CreateCell(2).SetCellValue("Process Flow");
                row.CreateCell(3).SetCellValue("Source Location");
                row.CreateCell(4).SetCellValue("Vessel");
                row.CreateCell(5).SetCellValue("Product");
                row.CreateCell(6).SetCellValue("Equipment");
                row.CreateCell(7).SetCellValue("Draft Survey Quantity");
                row.CreateCell(8).SetCellValue("Unit"); 
                row.CreateCell(9).SetCellValue("Initial Draft Survey"); 
                row.CreateCell(10).SetCellValue("Final Draft Survey"); 
                row.CreateCell(11).SetCellValue("Note");
                row.CreateCell(12).SetCellValue("Arrival DateTime");
                row.CreateCell(13).SetCellValue("Alongside DateTime");
                row.CreateCell(14).SetCellValue("Commenced Loading DateTime");
                row.CreateCell(15).SetCellValue("Completed Loading DateTime");
                row.CreateCell(16).SetCellValue("Cast Off DateTime");
                row.CreateCell(17).SetCellValue("Departure DateTime");
                row.CreateCell(18).SetCellValue("Distance");
                row.CreateCell(19).SetCellValue("Ref.Work Order");
                row.CreateCell(20).SetCellValue("Draft Survey Number");
                row.CreateCell(21).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var header = dbContext.shipping_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.is_loading == true && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.start_datetime);
                    // Inserting values to table
                    foreach (var baris in header)
                    {
                        var accounting_period_name = "";
                        var accounting_period = dbFind.accounting_period.Where(o => o.id == baris.accounting_period_id).FirstOrDefault();
                        if (accounting_period != null) accounting_period_name = accounting_period.accounting_period_name.ToString();

                        var process_flow_code = "";
                        var process_flow = dbFind.process_flow.Where(o => o.id == baris.process_flow_id).FirstOrDefault();
                        if (process_flow != null) process_flow_code = process_flow.process_flow_code.ToString();

                        var vehicle_name = "";
                        var barge = dbFind.barge.Where(o => o.id == baris.source_location_id).FirstOrDefault();
                        if (barge != null) vehicle_name = barge.vehicle_id.ToString();

                        var vessel_name = "";
                        var vessel = dbFind.vessel.Where(o => o.id == baris.ship_location_id).FirstOrDefault();
                        if (vessel != null) vessel_name = vessel.vehicle_name.ToString();

                        var product_code = "";
                        var product = dbFind.product.Where(o => o.id == baris.product_id).FirstOrDefault();
                        if (product != null) product_code = product.product_code.ToString();

                        var uom_symbol = "";
                        var uom = dbFind.uom.Where(o => o.id == baris.uom_id).FirstOrDefault();
                        if (uom != null) uom_symbol = uom.uom_name.ToString();

                        var equipment_code = "";
                        var equipment = dbFind.equipment.Where(o => o.id == baris.equipment_id).FirstOrDefault();
                        if (equipment != null) equipment_code = equipment.equipment_code.ToString();

                        var survey_number = "";
                        var survey = dbFind.survey.Where(o => o.id == baris.survey_id && o.is_draft_survey == true).FirstOrDefault();
                        if (survey != null) survey_number = survey.survey_number.ToString();

                        var despatch_order_number = "";
                        var despatch_order = dbFind.despatch_order.Where(o => o.id == baris.despatch_order_id).FirstOrDefault();
                        if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                        var business_unit = "";
                        var bu = dbFind.business_unit.Where(o => o.id == baris.business_unit_id).FirstOrDefault();
                        if (bu != null) business_unit = bu.business_unit_code.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(baris.transaction_number);
                        row.CreateCell(1).SetCellValue(despatch_order_number);//(accounting_period_name);
                        row.CreateCell(2).SetCellValue(process_flow_code);
                        row.CreateCell(3).SetCellValue(vehicle_name);
                        row.CreateCell(4).SetCellValue(vessel_name);
                        row.CreateCell(5).SetCellValue(product_code);
                        row.CreateCell(6).SetCellValue(equipment_code);
                        row.CreateCell(7).SetCellValue(Convert.ToDouble(baris.quantity));
                        row.CreateCell(8).SetCellValue(uom_symbol);
                        row.CreateCell(9).SetCellValue(" " + (baris.initial_draft_survey != null ? Convert.ToDateTime(baris.initial_draft_survey).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(10).SetCellValue(" " + (baris.final_draft_survey != null ? Convert.ToDateTime(baris.final_draft_survey).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(11).SetCellValue(baris.note);
                        row.CreateCell(12).SetCellValue(" " + (baris.arrival_datetime != null ? Convert.ToDateTime(baris.arrival_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(13).SetCellValue(" " + (baris.berth_datetime != null ? Convert.ToDateTime(baris.berth_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(14).SetCellValue(" " + (baris.start_datetime != null ? Convert.ToDateTime(baris.start_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(15).SetCellValue(" " + (baris.end_datetime != null ? Convert.ToDateTime(baris.end_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(16).SetCellValue(" " + (baris.unberth_datetime != null ? Convert.ToDateTime(baris.unberth_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(17).SetCellValue(" " + (baris.departure_datetime != null ? Convert.ToDateTime(baris.departure_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(18).SetCellValue(Convert.ToDouble(baris.distance));
                        row.CreateCell(19).SetCellValue(baris.ref_work_order);
                        row.CreateCell(20).SetCellValue(baris.draft_survey_number);
                        row.CreateCell(21).SetCellValue(business_unit);

                        RowCount++;
                     //   if (RowCount > 50) break;
                    }
                }
                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Header Transaction Number");
                row.CreateCell(1).SetCellValue("Detail Transaction Number");
                row.CreateCell(2).SetCellValue("Source");
                row.CreateCell(3).SetCellValue("Reference Number");
                row.CreateCell(4).SetCellValue("Start DateTime");
                row.CreateCell(5).SetCellValue("End Datetime");
                row.CreateCell(6).SetCellValue("Quantity");
                row.CreateCell(7).SetCellValue("Unit");
                row.CreateCell(8).SetCellValue("Equipment");
                row.CreateCell(9).SetCellValue("Hour Usage");
                row.CreateCell(10).SetCellValue("Survey");
                row.CreateCell(11).SetCellValue("Final Quantity");
                row.CreateCell(12).SetCellValue("Note");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.vw_shipping_transaction_detail
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.is_loading == true)
                    .OrderByDescending(o => o.start_datetime);
                // Inserting values to table
                foreach (var baris in detail)
                {
                    var shipping_transaction_number = "";
                    var shipping_transaction = dbFind.shipping_transaction.Where(o => o.id == baris.shipping_transaction_id).FirstOrDefault();
                    if (shipping_transaction != null) shipping_transaction_number = shipping_transaction.transaction_number.ToString();

                    var source_name = "";
                    var barge = dbFind.barge.Where(o => o.id == baris.detail_location_id).FirstOrDefault();
                    if (barge != null) source_name = barge.stock_location_name?.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == baris.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_name.ToString();

                    var equipment_code = "";
                    var equipment = dbFind.equipment.Where(o => o.id == baris.equipment_id).FirstOrDefault();
                    if (equipment != null) equipment_code = equipment.equipment_code.ToString();

                    var survey_number = "";
                    var survey = dbFind.survey.Where(o => o.id == baris.survey_id && o.is_draft_survey == true).FirstOrDefault();
                    if (survey != null) survey_number = survey.survey_number.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(shipping_transaction_number);
                    row.CreateCell(1).SetCellValue(baris.transaction_number);
                    row.CreateCell(2).SetCellValue(source_name);
                    row.CreateCell(3).SetCellValue(baris.reference_number);
                    row.CreateCell(4).SetCellValue(" " + (baris.start_datetime != null ? Convert.ToDateTime(baris.start_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                    row.CreateCell(5).SetCellValue(" " + (baris.end_datetime != null ? Convert.ToDateTime(baris.end_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(baris.quantity));
                    row.CreateCell(7).SetCellValue(uom_symbol);
                    row.CreateCell(8).SetCellValue(equipment_code);
                    row.CreateCell(9).SetCellValue(Convert.ToDouble(baris.hour_usage));
                    row.CreateCell(10).SetCellValue(survey_number);
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(baris.final_quantity));
                    row.CreateCell(12).SetCellValue(baris.note);

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

        public async Task<IActionResult> UnloadingExcelExport([FromBody] dynamic Data)
        {
            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);
            string sFileName = "ShippingUnloading.xlsx";
            sFileName = sFileName.Insert(sFileName.LastIndexOf("."), string.Format("_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff")));

            FileInfo file = new FileInfo(Path.Combine(FilePath, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(FilePath, sFileName), FileMode.Create, FileAccess.Write))
            {
                int RowCount = 1;
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("ShippingUnloading");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("Shipping Order");//("Accounting Period");
                row.CreateCell(2).SetCellValue("Process Flow");
                row.CreateCell(3).SetCellValue("Vessel");
                row.CreateCell(4).SetCellValue("Product");
                row.CreateCell(5).SetCellValue("Equipment");
                row.CreateCell(6).SetCellValue("Quantity");
                row.CreateCell(7).SetCellValue("Unit");
                row.CreateCell(8).SetCellValue("Initial Draft Survey");
                row.CreateCell(9).SetCellValue("Final Draft Survey");
                row.CreateCell(10).SetCellValue("Note");
                row.CreateCell(11).SetCellValue("Arrival DateTime");
                row.CreateCell(12).SetCellValue("Alongside DateTime");
                row.CreateCell(13).SetCellValue("Commenced Loading DateTime");
                row.CreateCell(14).SetCellValue("Completed Loading DateTime");// ("Quality Survey");
                row.CreateCell(15).SetCellValue("Cast Off DateTime");
                row.CreateCell(16).SetCellValue("Departure DateTime");
                row.CreateCell(17).SetCellValue("Destination Location");
                row.CreateCell(18).SetCellValue("Distance");
                row.CreateCell(19).SetCellValue("Ref.Work Order");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    var header = dbContext.shipping_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.is_loading == false && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.start_datetime);
                    // Inserting values to table
                    foreach (var baris in header)
                    {
                        var accounting_period_name = "";
                        var accounting_period = dbFind.accounting_period.Where(o => o.id == baris.accounting_period_id).FirstOrDefault();
                        if (accounting_period != null) accounting_period_name = accounting_period.accounting_period_name.ToString();

                        var process_flow_code = "";
                        var process_flow = dbFind.process_flow.Where(o => o.id == baris.process_flow_id).FirstOrDefault();
                        if (process_flow != null) process_flow_code = process_flow.process_flow_code.ToString();

                        var vessel_name = "";
                        var vessel = dbFind.vessel.Where(o => o.id == baris.ship_location_id).FirstOrDefault();
                        if (vessel != null) vessel_name = vessel.vehicle_name.ToString();

                        var product_code = "";
                        var product = dbFind.product.Where(o => o.id == baris.product_id).FirstOrDefault();
                        if (product != null) product_code = product.product_code.ToString();

                        var uom_symbol = "";
                        var uom = dbFind.uom.Where(o => o.id == baris.uom_id).FirstOrDefault();
                        if (uom != null) uom_symbol = uom.uom_name.ToString();

                        var equipment_code = "";
                        var equipment = dbFind.equipment.Where(o => o.id == baris.equipment_id).FirstOrDefault();
                        if (equipment != null) equipment_code = equipment.equipment_code.ToString();

                        var survey_number = "";
                        var survey = dbFind.survey.Where(o => o.id == baris.survey_id && o.is_draft_survey == true).FirstOrDefault();
                        if (survey != null) survey_number = survey.survey_number.ToString();

                        var despatch_order_number = "";
                        var despatch_order = dbFind.despatch_order.Where(o => o.id == baris.despatch_order_id).FirstOrDefault();
                        if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                        var stock_location_name = "";
                        var port_location = dbFind.port_location.Where(o => o.id == baris.destination_location_id).FirstOrDefault();
                        if (port_location != null) stock_location_name = port_location.stock_location_name.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(baris.transaction_number);
                        row.CreateCell(1).SetCellValue(despatch_order_number);//(accounting_period_name);
                        row.CreateCell(2).SetCellValue(process_flow_code);
                        row.CreateCell(3).SetCellValue(vessel_name);
                        row.CreateCell(4).SetCellValue(product_code);
                        row.CreateCell(5).SetCellValue(equipment_code);
                        row.CreateCell(6).SetCellValue(Convert.ToDouble(baris.quantity));
                        row.CreateCell(7).SetCellValue(uom_symbol);
                        row.CreateCell(8).SetCellValue(" " + (baris.initial_draft_survey != null ? Convert.ToDateTime(baris.initial_draft_survey).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(9).SetCellValue(" " + (baris.final_draft_survey != null ? Convert.ToDateTime(baris.final_draft_survey).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(10).SetCellValue(baris.note);
                        row.CreateCell(11).SetCellValue(" " + (baris.arrival_datetime != null ? Convert.ToDateTime(baris.arrival_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(12).SetCellValue(" " + (baris.berth_datetime != null ? Convert.ToDateTime(baris.berth_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(13).SetCellValue(" " + (baris.start_datetime != null ? Convert.ToDateTime(baris.start_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(14).SetCellValue(" " + (baris.end_datetime != null ? Convert.ToDateTime(baris.end_datetime).ToString("yyyy-MM-dd HH:mm") : ""));//(survey_number);
                        row.CreateCell(15).SetCellValue(" " + (baris.unberth_datetime != null ? Convert.ToDateTime(baris.unberth_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(16).SetCellValue(" " + (baris.departure_datetime != null ? Convert.ToDateTime(baris.departure_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(17).SetCellValue(stock_location_name);
                        row.CreateCell(18).SetCellValue(Convert.ToDouble(baris.distance));
                        row.CreateCell(19).SetCellValue(baris.ref_work_order);

                        RowCount++;
                    }
                }
                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("Source");
                row.CreateCell(2).SetCellValue("Reference Number");
                row.CreateCell(3).SetCellValue("Start DateTime");
                row.CreateCell(4).SetCellValue("End Datetime");
                row.CreateCell(5).SetCellValue("Quantity");
                row.CreateCell(6).SetCellValue("Unit");
                row.CreateCell(7).SetCellValue("Equipment");
                row.CreateCell(8).SetCellValue("Hour Usage");
                row.CreateCell(9).SetCellValue("Survey");
                row.CreateCell(10).SetCellValue("Final Quantity");
                row.CreateCell(11).SetCellValue("Note");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.vw_shipping_transaction_detail
                    .Where(o => o.is_loading == false && o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var baris in detail)
                {
                    var shipping_transaction_number = "";
                    var shipping_transaction = dbFind.shipping_transaction.Where(o => o.id == baris.shipping_transaction_id).FirstOrDefault();
                    if (shipping_transaction != null) shipping_transaction_number = shipping_transaction.transaction_number.ToString();

                    var source_name = "";
                    var port_location = dbFind.port_location.Where(o => o.id == baris.detail_location_id).FirstOrDefault();
                    if (port_location != null) source_name = port_location.port_location_code.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == baris.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_name.ToString();

                    var equipment_code = "";
                    var equipment = dbFind.equipment.Where(o => o.id == baris.equipment_id).FirstOrDefault();
                    if (equipment != null) equipment_code = equipment.equipment_code.ToString();

                    var survey_number = "";
                    var survey = dbFind.survey.Where(o => o.id == baris.survey_id && o.is_draft_survey == true).FirstOrDefault();
                    if (survey != null) survey_number = survey.survey_number.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(shipping_transaction_number);
                    row.CreateCell(1).SetCellValue(source_name);
                    row.CreateCell(2).SetCellValue(baris.reference_number);
                    row.CreateCell(3).SetCellValue(" " + (baris.start_datetime != null ? Convert.ToDateTime(baris.start_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                    row.CreateCell(4).SetCellValue(" " + (baris.end_datetime != null ? Convert.ToDateTime(baris.end_datetime).ToString("yyyy-MM-dd HH:mm") : ""));
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(baris.quantity));
                    row.CreateCell(6).SetCellValue(uom_symbol);
                    row.CreateCell(7).SetCellValue(equipment_code);
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(baris.hour_usage));
                    row.CreateCell(9).SetCellValue(survey_number);
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(baris.final_quantity));
                    row.CreateCell(11).SetCellValue(baris.note);
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

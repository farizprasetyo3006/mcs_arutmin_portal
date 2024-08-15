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
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Mining.Controllers
{
    [Area("Mining")]
    public class TimesheetPlanController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public TimesheetPlanController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Mining];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.TimesheetPlan];
            ViewBag.BreadcrumbCode = WebAppMenu.TimesheetPlan;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> Detail(String Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Mining];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.TimesheetPlan];
            ViewBag.BreadcrumbCode = WebAppMenu.TimesheetPlan;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var timesheet_plan = await dbContext.vw_timesheet_plan
                    .Where(o => o.id == Id).FirstOrDefaultAsync();

                    if (timesheet_plan != null)
                    {
                        ViewBag.TimesheetPlan = timesheet_plan;

                        var shift = await dbContext.vw_shift
                            .Where(o => o.id == timesheet_plan.shift_id).FirstOrDefaultAsync();

                        if(shift != null)
                        {
                            ViewBag.Shift = shift;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return View();
        }

        /*public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "Timesheet.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Timesheet");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("CN Unit");
                row.CreateCell(1).SetCellValue("Location");
                row.CreateCell(2).SetCellValue("Hour Meter Awal");
                row.CreateCell(3).SetCellValue("Operator NIK");
                row.CreateCell(4).SetCellValue("Hour Meter Akhir");
                row.CreateCell(5).SetCellValue("Quantity");
                row.CreateCell(6).SetCellValue("Shift");
                row.CreateCell(7).SetCellValue("UoM");
                row.CreateCell(8).SetCellValue("Date");
                row.CreateCell(9).SetCellValue("Material Code");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.timesheet;
                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    var equipment_name = "";
                    var equipments = dbFind.equipment.Where(o => o.id == baris.cn_unit_id).FirstOrDefault();
                    if (equipments != null) 
                        equipment_name = equipments.equipment_name.ToString();
                    else
                    {
                        var trucks = dbFind.truck.Where(o => o.id == baris.cn_unit_id).FirstOrDefault();
                        if (trucks != null) equipment_name = trucks.vehicle_name.ToString();
                    }

                    var location_code = "";
                    var location = dbFind.mine_location.Where(o => o.id == baris.mine_location_id).FirstOrDefault();
                    if (location != null) location_code = location.mine_location_code.ToString();

                    var operator_nik = "";
                    var _operator = dbFind._operator.Where(o => o.id == baris.operator_id).FirstOrDefault();
                    if (_operator != null) operator_nik = _operator.nik.ToString();

                    var shift_name = "";
                    var shift = dbFind.shift.Where(o => o.id == baris.shift_id).FirstOrDefault();
                    if (shift != null) shift_name = shift.shift_name.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == baris.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    var productRecord = dbFind.product.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .Select(o => new { Id = o.id, Text = o.product_code });
                    var wasteRecord = dbFind.waste.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .Select(o => new { Id = o.id, Text = o.waste_code });
                    var pw = productRecord.Union(wasteRecord);
                    var material_name = "";
                    var material = pw.Where(o => o.Id == baris.material_id).FirstOrDefault();
                    if (material != null) material_name = material.Text.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(equipment_name);
                    row.CreateCell(1).SetCellValue(location_code);
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(baris.hour_start));
                    row.CreateCell(3).SetCellValue(operator_nik);
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(baris.hour_end));
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(baris.quantity));
                    row.CreateCell(6).SetCellValue(shift_name);
                    row.CreateCell(7).SetCellValue(uom_symbol);
                    row.CreateCell(8).SetCellValue(" " + Convert.ToDateTime(baris.timesheet_date).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(9).SetCellValue(material_name);
                    RowCount++;
                }

                ///**************************************** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Timesheet ID");
                row.CreateCell(1).SetCellValue("Activity");
                row.CreateCell(2).SetCellValue("Source Location");
                row.CreateCell(3).SetCellValue("Destination Location");
                row.CreateCell(4).SetCellValue("Hour");
                row.CreateCell(5).SetCellValue("Minute");
                row.CreateCell(6).SetCellValue("Distance");
                row.CreateCell(7).SetCellValue("Ritase");
                row.CreateCell(8).SetCellValue("Quantity");
                row.CreateCell(9).SetCellValue("Refuelling Quantity");
                row.CreateCell(10).SetCellValue("UoM");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                string[] HourList = { "00:00 - 01.00", "01:00 - 02.00", "02:00 - 03.00", "03:00 - 04.00", "04:00 - 05.00", "05:00 - 06.00", "06:00 - 07.00", "07:00 - 08.00", "08:00 - 09.00", "09:00 - 10.00", "10:00 - 11.00", "11:00 - 12.00", "12:00 - 13.00", "13:00 - 14.00", "14:00 - 15.00", "15:00 - 16.00", "16:00 - 17.00", "17:00 - 18.00", "18:00 - 19.00", "19:00 - 20.00", "20:00 - 21.00", "21:00 - 22.00", "22:00 - 23.00", "23:00 - 24.00" };

                var detail = dbContext.timesheet_detail;
                // Inserting values to table
                foreach (var isi in detail)
                {
                    var cn_unit_id = "";
                    var timesheet = dbFind.timesheet.Where(o => o.id == isi.timesheet_id).FirstOrDefault();
                    if (timesheet != null) cn_unit_id = timesheet.cn_unit_id.ToString();

                    var equipments = dbFind.equipment
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .Select(o => new { Id = o.id, Text = o.equipment_name });
                    var trucks = dbFind.truck
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .Select(o => new { Id = o.id, Text = o.vehicle_name });
                    var et = equipments.Union(trucks);
                    var equipment_name = "";
                    var equiptruck = et.Where(o => o.Id == cn_unit_id).FirstOrDefault();
                    if (equiptruck != null) equipment_name = equiptruck.Text.ToString();

                    var event_category_name = "";
                    var event_category = dbFind.event_category.Where(o => o.id == isi.event_category_id).FirstOrDefault();
                    if (event_category != null) event_category_name = event_category.event_category_name.ToString();

                    var source_location_code = "";
                    var location = dbFind.mine_location.Where(o => o.id == isi.mine_location_id).FirstOrDefault();
                    if (location != null) source_location_code = location.mine_location_code.ToString();

                    var stockpileRecord = dbFind.stockpile_location
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .Select(o => new { Id = o.id, Text = o.stock_location_name });
                    var wasteLocationRecord = dbFind.waste_location
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .Select(o => new { Id = o.id, Text = o.stock_location_name });
                    var sw = stockpileRecord.Union(wasteLocationRecord);
                    var destination_location_name = "";
                    var destination_location = sw.Where(o => o.Id == isi.destination_id).FirstOrDefault();
                    if (destination_location != null) destination_location_name = destination_location.Text.ToString();

                    var hour_desc = HourList[Convert.ToInt32(isi.periode)];

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == isi.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(equipment_name);
                    row.CreateCell(1).SetCellValue(event_category_name);
                    row.CreateCell(2).SetCellValue(source_location_code);
                    row.CreateCell(3).SetCellValue(destination_location_name);
                    row.CreateCell(4).SetCellValue(hour_desc);
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(isi.duration));
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(isi.distance));
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(isi.ritase));
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(isi.quantity));
                    row.CreateCell(9).SetCellValue(Convert.ToDouble(isi.refuelling_quantity));
                    row.CreateCell(10).SetCellValue(uom_symbol);
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
                // Deletes the generated file from /wwwroot folder
                finally
                {
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }*/

    }
}

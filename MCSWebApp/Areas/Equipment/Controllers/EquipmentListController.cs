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
using Microsoft.EntityFrameworkCore;

namespace MCSWebApp.Areas.Equipment.Controllers
{
    [Area("Equipment")]
    public class EquipmentListController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public EquipmentListController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Equipment];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.EquipmentList];
            ViewBag.BreadcrumbCode = WebAppMenu.EquipmentList;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "EquipmentList.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("EquipmentList");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Equipment Code");
                row.CreateCell(1).SetCellValue("Equipment Name");
                row.CreateCell(2).SetCellValue("Equipment Type");
                row.CreateCell(3).SetCellValue("Owner");
                row.CreateCell(4).SetCellValue("Capacity");
                row.CreateCell(5).SetCellValue("Capacity Unit");
                row.CreateCell(6).SetCellValue("Is Active");
                row.CreateCell(7).SetCellValue("Class");
                row.CreateCell(8).SetCellValue("Model Year");
                row.CreateCell(9).SetCellValue("Manufactured");
                row.CreateCell(10).SetCellValue("Plan");
                row.CreateCell(11).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var header = await dbContext.equipment.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.created_on).ToArrayAsync();
                    // Inserting values to table
                    foreach (var Value in header)
                    {
                        var equipment_type_name = "";
                        var equipment_type = dbFind.equipment_type.Where(o => o.id == Value.equipment_type_id).FirstOrDefault();
                        if (equipment_type != null) equipment_type_name = equipment_type.equipment_type_code.ToString();

                        var capacity_unit = "";
                        var uom = dbFind.uom.Where(o => o.id == Value.capacity_uom_id).FirstOrDefault();
                        if (uom != null) capacity_unit = uom.uom_symbol.ToString();

                        var business_unit = "";
                        var BU = dbFind.business_unit.Where(o => o.id == Value.business_unit_id).FirstOrDefault();
                        if (BU != null) business_unit = BU.business_unit_code.ToString();

                        var _class = "";
                        var C = dbFind.master_list.Where(o => o.id == Value.vehicle_model).FirstOrDefault();
                        if (C != null) _class = C.item_name.ToString();

                        var vendor_code = "";
                        var business_partner = dbFind.business_partner.Where(o => o.id == Value.vendor_id).FirstOrDefault();
                        if (business_partner != null) vendor_code = business_partner.business_partner_code.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(Value.equipment_code);
                        row.CreateCell(1).SetCellValue(Value.equipment_name);
                        row.CreateCell(2).SetCellValue(equipment_type_name);
                        row.CreateCell(3).SetCellValue(vendor_code);
                        row.CreateCell(4).SetCellValue(Convert.ToDouble(Value.capacity));
                        row.CreateCell(5).SetCellValue(capacity_unit);
                        row.CreateCell(6).SetCellValue(Convert.ToBoolean(Value.status));
                        row.CreateCell(7).SetCellValue(_class);
                        row.CreateCell(8).SetCellValue(Convert.ToDouble(Value.vehicle_model_year));
                        row.CreateCell(9).SetCellValue(Value.vehicle_manufactured_year);
                        row.CreateCell(10).SetCellValue(Convert.ToDouble(Value.plan));
                        row.CreateCell(11).SetCellValue(business_unit);

                        RowCount++;
                        if (RowCount > 50) break;
                    }
                }
                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Equipment Code");
                row.CreateCell(1).SetCellValue("Code");
                row.CreateCell(2).SetCellValue("Name");
                row.CreateCell(3).SetCellValue("Accounting Period");
                row.CreateCell(4).SetCellValue("Currency");
                row.CreateCell(5).SetCellValue("Hourly Rate");
                row.CreateCell(6).SetCellValue("Trip Rate");
                row.CreateCell(7).SetCellValue("Monthly Rate");
                row.CreateCell(8).SetCellValue("Fuel per Hour");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.equipment_cost_rate.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var Value in detail)
                {
                    var equipment_code = "";
                    var equipment = dbFind.equipment.Where(o => o.id == Value.equipment_id).FirstOrDefault();
                    if (equipment != null) equipment_code = equipment.equipment_code.ToString();

                    var acc_period = "";
                    var accounting_period = dbFind.accounting_period.Where(o => o.id == Value.accounting_period_id).FirstOrDefault();
                    if (accounting_period != null) acc_period = accounting_period.accounting_period_name.ToString();

                    var currency_code = "";
                    var currency = dbFind.currency.Where(o => o.id == Value.currency_id).FirstOrDefault();
                    if (currency != null) currency_code = currency.currency_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(equipment_code);
                    row.CreateCell(1).SetCellValue(Value.name);
                    row.CreateCell(2).SetCellValue(Value.code);
                    row.CreateCell(3).SetCellValue(acc_period);
                    row.CreateCell(4).SetCellValue(currency_code);
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(Value.hourly_rate));
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(Value.trip_rate));
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(Value.monthly_rate));
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(Value.fuel_per_hour));

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

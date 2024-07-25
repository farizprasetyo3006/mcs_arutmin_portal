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

namespace MCSWebApp.Areas.Transport
{
    [Area("Transport")]
    public class TruckController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public TruckController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Transport];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Truck];
            ViewBag.BreadcrumbCode = WebAppMenu.Truck;

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "Truck.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Truck");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Truck Name");
                row.CreateCell(1).SetCellValue("Truck Id");
                row.CreateCell(2).SetCellValue("Capacity");
                row.CreateCell(3).SetCellValue("Capacity Unit");
                row.CreateCell(4).SetCellValue("Owner");
                row.CreateCell(5).SetCellValue("Make");
                row.CreateCell(6).SetCellValue("Model");
                row.CreateCell(7).SetCellValue("Model Year");
                row.CreateCell(8).SetCellValue("Manufactured Year");
                row.CreateCell(9).SetCellValue("Typical Tonnage");
                row.CreateCell(10).SetCellValue("Typical Volume");
                row.CreateCell(11).SetCellValue("Tare");
                row.CreateCell(12).SetCellValue("Average Scale");
                row.CreateCell(13).SetCellValue("Is Active");
                row.CreateCell(14).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var business_unit1 = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                IOrderedQueryable<truck> tabledata;
                var selectedIds = ((string)Data.selectedIds)
                    .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                tabledata = dbContext.truck
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                && selectedIds.Contains(o.id))
                .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var isi in tabledata)
                {
                    var capacity_unit = "";
                    var uom = dbFind.uom.Where(o => o.id == isi.capacity_uom_id).FirstOrDefault();
                    if (uom != null) capacity_unit = uom.uom_symbol.ToString();

                    var owner = "";
                    var business_partner = dbFind.contractor.Where(o => o.id == isi.vendor_id).FirstOrDefault();
                    if (business_partner != null) owner = business_partner.business_partner_code.ToString();

                    var business_unit = "";
                    var BU = dbFind.business_unit.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == isi.business_unit_id).FirstOrDefault();
                    if (BU != null) business_unit = BU.business_unit_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(isi.vehicle_name);
                    row.CreateCell(1).SetCellValue(isi.vehicle_id);
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(isi.capacity));
                    row.CreateCell(3).SetCellValue(capacity_unit);
                    row.CreateCell(4).SetCellValue(owner);
                    row.CreateCell(5).SetCellValue(isi.vehicle_make);
                    row.CreateCell(6).SetCellValue(isi.vehicle_model);
                    row.CreateCell(7).SetCellValue(Convert.ToInt32(isi.vehicle_model_year));
                    row.CreateCell(8).SetCellValue(Convert.ToInt32(isi.vehicle_manufactured_year));
                    row.CreateCell(9).SetCellValue(Convert.ToDouble(isi.typical_tonnage));
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(isi.typical_volume));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(isi.tare));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(isi.average_scale));
                    row.CreateCell(13).SetCellValue(Convert.ToBoolean(isi.status));
                    row.CreateCell(14).SetCellValue(business_unit);

                    RowCount++;
                    if (RowCount > 50) break;
                }

                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Truck Cost Rate");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Truck Id");
                row.CreateCell(1).SetCellValue("Code");
                row.CreateCell(2).SetCellValue("Name");
                row.CreateCell(3).SetCellValue("Accounting Period");
                row.CreateCell(4).SetCellValue("Currency");
                row.CreateCell(5).SetCellValue("Hourly Rate");
                row.CreateCell(6).SetCellValue("Trip Rate");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.truck_cost_rate.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var isi in detail)
                {
                    var vehicle_id = "";
                    var truck = dbFind.truck.Where(o => o.id == isi.truck_id).FirstOrDefault();
                    if (truck != null) vehicle_id = truck.vehicle_id.ToString();

                    var accounting_period_name = "";
                    var accounting_period = dbFind.accounting_period.Where(o => o.id == isi.accounting_period_id).FirstOrDefault();
                    if (accounting_period != null) accounting_period_name = accounting_period.accounting_period_name.ToString();

                    var currency_code = "";
                    var currency = dbFind.currency.Where(o => o.id == isi.currency_id).FirstOrDefault();
                    if (currency != null) currency_code = currency.currency_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(vehicle_id);
                    row.CreateCell(1).SetCellValue(isi.code);
                    row.CreateCell(2).SetCellValue(isi.name);
                    row.CreateCell(3).SetCellValue(accounting_period_name);
                    row.CreateCell(4).SetCellValue(currency_code);
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(isi.hourly_rate));
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(isi.trip_rate));

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

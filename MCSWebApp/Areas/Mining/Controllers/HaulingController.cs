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
using Microsoft.AspNetCore.SignalR;

namespace MCSWebApp.Areas.Mining.Controllers
{
    [Area("Mining")]
    public class HaulingController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly IHubContext<ProgressHub> _hubContext;

        public HaulingController(IConfiguration Configuration, IHubContext<ProgressHub> hubContext)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Mining];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Hauling];
            ViewBag.BreadcrumbCode = WebAppMenu.Hauling;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        //public async Task<IActionResult> ExcelExport1()
        //{
        //    string sFileName = "Hauling.xlsx";
        //    string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
        //    if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);
        //    FilePath = Path.Combine(FilePath, sFileName);

        //    var mapper = new Npoi.Mapper.Mapper();
        //    mapper.Ignore<hauling_transaction>(o => o.organization_);
        //    mapper.Put(dbContext.hauling_transaction, "Hauling", true);
        //    mapper.Save(FilePath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(FilePath, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;
        //    //Throws Generated file to Browser
        //    try
        //    {
        //        return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        //    }
        //    // Deletes the generated file
        //    finally
        //    {
        //        var path = Path.Combine(FilePath, sFileName);
        //        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        //    }
        //}

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string operationId = Data.operationId;

            string sFileName = "CoalHauling.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Hauling");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("DateTime In");
                row.CreateCell(2).SetCellValue("DateTime Out");
                row.CreateCell(3).SetCellValue("Shift"); 
                row.CreateCell(4).SetCellValue("Process Flow");
                row.CreateCell(5).SetCellValue("Transport");
                row.CreateCell(6).SetCellValue("Source");
                row.CreateCell(7).SetCellValue("Destination");
                row.CreateCell(8).SetCellValue("Gross (MT)"); 
                row.CreateCell(9).SetCellValue("Tare (MT)");
                row.CreateCell(10).SetCellValue("Loading Quantity (MT)");
                row.CreateCell(11).SetCellValue("Product");
                row.CreateCell(12).SetCellValue("Distance (meter)");
                row.CreateCell(13).SetCellValue("Quality Sampling");
                row.CreateCell(14).SetCellValue("Contract Reference");
                row.CreateCell(15).SetCellValue("PIC");
                row.CreateCell(16).SetCellValue("Note");
                row.CreateCell(17).SetCellValue("Contractor");
                row.CreateCell(18).SetCellValue("Equipment");
                row.CreateCell(19).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var business_unit1 = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                IOrderedQueryable<hauling_transaction> tabledata;
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    tabledata = dbContext.hauling_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.loading_datetime);
                }
                else
                {
                    tabledata = dbContext.hauling_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId )
                    .OrderByDescending(o => o.loading_datetime);
                }
                int totalData = tabledata.Count();
                int count = 1;
                await _hubContext.Clients.Group(operationId).SendAsync("ReceiveDownloadProgress", count, totalData);

                var process_flow = await dbContext.process_flow.ToListAsync();
                var truck = await dbContext.truck.ToListAsync();
                var contractor = await dbContext.contractor.ToListAsync();
                var shift = await dbContext.shift.ToListAsync();
                var product = await dbContext.product.ToListAsync();
                var uom = await dbContext.uom.ToListAsync();
                var vw_stockpile_location = await dbContext.vw_stockpile_location.ToListAsync();
                var equipment = await dbContext.equipment.ToListAsync();
                var despatch_order = await dbContext.despatch_order.ToListAsync();
                var quality_sampling = await dbContext.quality_sampling.ToListAsync();
                var employee = await dbContext.employee.ToListAsync();
                var advance_contract = await dbContext.advance_contract.ToListAsync();
                var business_unit = await dbContext.business_unit.ToListAsync();

                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    await _hubContext.Clients.Group(operationId).SendAsync("ReceiveDownloadProgress", count, totalData);
                    count++;

                    var process_flow_name = "";
                    var a = process_flow.Where(o => o.id == baris.process_flow_id).FirstOrDefault();
                    if (a != null) process_flow_name = a.process_flow_code.ToString();

                    var transport_id = "";
                    var b =  truck.Where(o => o.id == baris.transport_id).FirstOrDefault();
                    if (b != null) transport_id = b.vehicle_id.ToString();

                    var contractor_code = "";
                    var c = contractor.Where(o => o.id == baris.contractor_id).FirstOrDefault();
                    if (c != null) contractor_code = c.business_partner_code.ToString();

                    var source_location_code = "";
                    var d = vw_stockpile_location.Where(o => o.id == baris.source_location_id).FirstOrDefault();
                    if (d != null) source_location_code = d.stockpile_location_code.ToString();

                    var source_shift_name = "";
                    var e = shift.Where(o => o.id == baris.source_shift_id).FirstOrDefault();
                    if (e != null) source_shift_name = e.shift_code.ToString();

                    var product_name = "";
                    var f = product.Where(o => o.id == baris.product_id).FirstOrDefault();
                    if (f != null) product_name = f.product_code.ToString();

                    var uom_symbol = "";
                    var g = uom.Where(o => o.id == baris.uom_id).FirstOrDefault();
                    if (g != null) uom_symbol = g.uom_symbol.ToString();

                    var destination_location_code = "";
                    //var destination_location = dbFind.vw_stock_location.Where(o => o.id == baris.destination_location_id).FirstOrDefault();
                    var destination_location = vw_stockpile_location.Where(o => o.id == baris.destination_location_id).FirstOrDefault();
                    if (destination_location != null) destination_location_code = destination_location.stockpile_location_code.ToString();

                    var equipment_code = "";
                    var h = equipment.Where(o => o.id == baris.equipment_id).FirstOrDefault();
                    if (h != null) equipment_code = h.equipment_code.ToString();

                    //var survey_number = "";
                    // var survey = dbFind.survey.Where(o => o.id == baris.survey_id).FirstOrDefault();
                    //if (survey != null) survey_number = survey.survey_number.ToString();

                    var despatch_order_number = "";
                    var i = despatch_order.Where(o => o.id == baris.despatch_order_id).FirstOrDefault();
                    if (i != null) despatch_order_number = i.despatch_order_number.ToString();

                    var quality_sampling_number = "";
                    var j = quality_sampling.Where(o => o.id == baris.quality_sampling_id).FirstOrDefault();
                    if (j != null) quality_sampling_number = j.sampling_number.ToString();

                    //var progress_claim_name = "";
                    //var progress_claim = dbFind.progress_claim.Where(o => o.id == baris.progress_claim_id).FirstOrDefault();
                    //if (progress_claim != null) progress_claim_name = progress_claim.progress_claim_name.ToString();

                    var employee_number = "";
                    var k = employee.Where(o => o.id == baris.pic).FirstOrDefault();
                    if (k != null) employee_number = k.employee_number.ToString();

                    var advance_contract_number = "";
                    var l = advance_contract.Where(o => o.id == baris.advance_contract_id).FirstOrDefault();
                    if (l != null) advance_contract_number = l.advance_contract_number.ToString();

                    var business_unit_code = "";
                    var m = business_unit.Where(o => o.id == baris.business_unit_id).FirstOrDefault();
                    if (m != null) business_unit_code = m.business_unit_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.transaction_number);
                    row.CreateCell(1).SetCellValue(Convert.ToDateTime(baris.loading_datetime).ToString("yyyy-MM-dd HH:mm:ss")); 
                    row.CreateCell(2).SetCellValue(Convert.ToDateTime(baris.unloading_datetime).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(3).SetCellValue(source_shift_name);
                    row.CreateCell(4).SetCellValue(process_flow_name);
                    row.CreateCell(5).SetCellValue(transport_id);
                    row.CreateCell(6).SetCellValue(source_location_code);
                    row.CreateCell(7).SetCellValue(destination_location_code); 
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(baris.gross));
                    row.CreateCell(9).SetCellValue(Convert.ToDouble(baris.tare)); 
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(baris.loading_quantity));
                    row.CreateCell(11).SetCellValue(product_name);
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(baris.distance));
                    row.CreateCell(13).SetCellValue(quality_sampling_number);
                    row.CreateCell(14).SetCellValue(advance_contract_number);
                    row.CreateCell(15).SetCellValue(employee_number);
                    row.CreateCell(16).SetCellValue(baris.note);
                    row.CreateCell(17).SetCellValue(contractor_code);
                    row.CreateCell(18).SetCellValue(equipment_code);
                    row.CreateCell(19).SetCellValue(business_unit_code);

                    RowCount++;
                    //if (RowCount > 50) break;
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
                    await _hubContext.Clients.Group(operationId).SendAsync("ReceiveDownloadProgress", "complete", null);
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }

    }
}

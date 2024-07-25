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
using Microsoft.AspNetCore.SignalR;

namespace MCSWebApp.Areas.Mining.Controllers
{
    [Area("Mining")]
    public class WasteRemovalController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly IHubContext<ProgressHub> _hubContext;

        public WasteRemovalController(IConfiguration Configuration, IHubContext<ProgressHub> hubContext)
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
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.WasteRemoval];
            ViewBag.BreadcrumbCode = WebAppMenu.WasteRemoval;

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string operationId = Data.operationId; 

            string sFileName = "WasteRemoval.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("WasteRemoval");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("Date Time");
                row.CreateCell(2).SetCellValue("Contractor");
                row.CreateCell(3).SetCellValue("Process Flow");
                row.CreateCell(4).SetCellValue("Shift");
                row.CreateCell(5).SetCellValue("Source Location");
                row.CreateCell(6).SetCellValue("Equipment");
                row.CreateCell(7).SetCellValue("Quantity");
                row.CreateCell(8).SetCellValue("Distance");
                row.CreateCell(9).SetCellValue("Waste");
                row.CreateCell(10).SetCellValue("Destination Location");
                row.CreateCell(11).SetCellValue("Elevation");
                row.CreateCell(12).SetCellValue("Unit");
                row.CreateCell(13).SetCellValue("Transport");
                row.CreateCell(14).SetCellValue("Note");
                row.CreateCell(15).SetCellValue("PIC");
                row.CreateCell(16).SetCellValue("Contract Reference");
                row.CreateCell(17).SetCellValue("Business Unit");
                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;
                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var business_unit1 = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                IOrderedQueryable<waste_removal> tabledata;
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    tabledata = dbContext.waste_removal
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.loading_datetime);
                }
                else
                {
                    tabledata = dbContext.waste_removal
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.loading_datetime);
                }
                int totalData = tabledata.Count();
                int count = 1;
                await _hubContext.Clients.Group(operationId).SendAsync("ReceiveDownloadProgress", count, totalData);
                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    await _hubContext.Clients.Group(operationId).SendAsync("ReceiveDownloadProgress", count, totalData);
                    count++;
                    
                    var process_flow_code = "";
                    var process_flow = dbFind.process_flow.Where(o => o.id == baris.process_flow_id).FirstOrDefault();
                    if (process_flow != null) process_flow_code = process_flow.process_flow_code.ToString();

                    var shift_code = "";
                    var shift = dbFind.shift.Where(o => o.id == baris.source_shift_id).FirstOrDefault();
                    if (shift != null) shift_code = shift.shift_code.ToString();

                    var source_location_code = "";
                    var source_location = dbFind.mine_location.Where(o => o.id == baris.source_location_id).FirstOrDefault();
                    if (source_location != null) source_location_code = source_location.mine_location_code.ToString();

                    var destination_location_code = "";
                    var destination_location = dbFind.waste_location.Where(o => o.id == baris.destination_location_id).FirstOrDefault();
                    if (destination_location != null) destination_location_code = destination_location.waste_location_code.ToString();

                    var waste_name = "";
                    var waste = dbFind.waste.Where(o => o.id == baris.waste_id).FirstOrDefault();
                    if (waste != null) waste_name = waste.waste_code.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == baris.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_name.ToString();

                    var transport_name = "";
                    var truck = dbFind.truck.Where(o => o.id == baris.transport_id).FirstOrDefault();
                    if (truck != null) transport_name = truck.vehicle_id.ToString();
                    else
                    {
                        var equip = dbFind.vw_equipment.Where(o => o.id == baris.transport_id).FirstOrDefault();
                        if (equip != null) transport_name = equip.equipment_code.ToString();
                    }

                    var equipment_code = "";
                    var equipment = dbFind.equipment.Where(o => o.id == baris.equipment_id).FirstOrDefault();
                    if (equipment != null) equipment_code = equipment.equipment_code.ToString();

                    /*var despatch_order_number = "";
                    var despatch_order = dbFind.despatch_order.Where(o => o.id == baris.despatch_order_id).FirstOrDefault();
                    if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                    var progress_claim_name = "";
                    var progress_claim = dbFind.progress_claim.Where(o => o.id == baris.progress_claim_id).FirstOrDefault();
                    if (progress_claim != null) progress_claim_name = progress_claim.progress_claim_name.ToString();*/

                    var advance_contract_number = "";
                    var advance_contract = dbFind.advance_contract.Where(o => o.id == baris.advance_contract_id).FirstOrDefault();
                    if (advance_contract != null) advance_contract_number = advance_contract.advance_contract_number.ToString();

                    var contractor_name = "";
                    var contractor = dbFind.contractor.Where(o => o.id == baris.contractor_id).FirstOrDefault();
                    if (contractor != null) contractor_name = contractor.business_partner_code.ToString();

                    var employee_number = "";
                    var employee = dbFind.employee.Where(o => o.id == baris.pic).FirstOrDefault();
                    if (employee != null) employee_number = employee.employee_number.ToString();

                    var business_unit_code = "";
                    var business_unit = dbFind.business_unit.Where(o => o.id == baris.business_unit_id).FirstOrDefault();
                    if (business_unit != null) business_unit_code = business_unit.business_unit_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.transaction_number);
                    row.CreateCell(1).SetCellValue(Convert.ToDateTime(baris.unloading_datetime).ToString("yyyy-MM-dd HH:mm"));
                    row.CreateCell(2).SetCellValue(contractor_name);
                    row.CreateCell(3).SetCellValue(process_flow_code);
                    row.CreateCell(4).SetCellValue(shift_code);
                    row.CreateCell(5).SetCellValue(source_location_code);
                    row.CreateCell(6).SetCellValue(equipment_code);
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(baris.loading_quantity));
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(baris.distance));
                    row.CreateCell(9).SetCellValue(waste_name);
                    row.CreateCell(10).SetCellValue(destination_location_code);
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(baris.elevation));
                    row.CreateCell(12).SetCellValue(uom_symbol);
                    row.CreateCell(13).SetCellValue(transport_name);
                    row.CreateCell(14).SetCellValue(baris.note);
                    row.CreateCell(15).SetCellValue(employee_number);
                    row.CreateCell(16).SetCellValue(advance_contract_number);
                    row.CreateCell(17).SetCellValue(business_unit_code);

                    RowCount++;
                   // if (RowCount > 50) break;
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

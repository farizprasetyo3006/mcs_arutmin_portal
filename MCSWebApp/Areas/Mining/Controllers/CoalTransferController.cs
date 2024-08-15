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
    public class CoalTransferController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly IHubContext<ProgressHub> _hubContext;

        public CoalTransferController(IConfiguration Configuration, IHubContext<ProgressHub> hubContext)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ReconcileNumber];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.CoalTransfer];
            ViewBag.BreadcrumbCode = WebAppMenu.CoalTransfer;

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
            
            string sFileName = "Coal Transfer.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Coal Transfer");
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
                row.CreateCell(8).SetCellValue("Loading Quantity");
                row.CreateCell(9).SetCellValue("Unit");
                row.CreateCell(10).SetCellValue("Product");
                row.CreateCell(11).SetCellValue("Distance (meter)");
                row.CreateCell(12).SetCellValue("Quality Sampling");
                row.CreateCell(13).SetCellValue("Contract Reference");
                row.CreateCell(14).SetCellValue("PIC");
                row.CreateCell(15).SetCellValue("Note");
                row.CreateCell(16).SetCellValue("Contractor");
                row.CreateCell(17).SetCellValue("Equipment");
                row.CreateCell(18).SetCellValue("Gross");
                row.CreateCell(19).SetCellValue("Tare");
                row.CreateCell(20).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var business_unit1 = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                IOrderedQueryable<coal_transfer> tabledata;
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    tabledata = dbContext.coal_transfer
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.loading_datetime);
                }
                else
                {
                    tabledata = dbContext.coal_transfer
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

                    var process_flow_name = "";
                    var process_flow = dbFind.process_flow.Where(o => o.id == baris.process_flow_id).FirstOrDefault();
                    if (process_flow != null) process_flow_name = process_flow.process_flow_code.ToString();

                    var transport_id = "";
                    var truck = dbFind.truck.Where(o => o.id == baris.transport_id).FirstOrDefault();
                    if (truck != null) transport_id = truck.vehicle_id.ToString();

                    var contractor_code = "";
                    var contractor = dbFind.contractor.Where(o => o.id == baris.contractor_id).FirstOrDefault();
                    if (contractor != null) contractor_code = contractor.business_partner_code.ToString();

                    var source_location_name = "";
                    var source_location = dbFind.vw_stockpile_location.Where(o => o.id == baris.source_location_id).FirstOrDefault();
                    if (source_location != null) source_location_name = source_location.stockpile_location_code.ToString();

                    var source_shift_name = "";
                    var shift = dbFind.shift.Where(o => o.id == baris.source_shift_id).FirstOrDefault();
                    if (shift != null) source_shift_name = shift.shift_code.ToString();

                    var product_code = "";
                    var product = dbFind.product.Where(o => o.id == baris.product_id).FirstOrDefault();
                    if (product != null) product_code = product.product_code.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == baris.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_name.ToString();

                    var destination_location_name = "";
                    //var destination_location = dbFind.vw_stock_location.Where(o => o.id == baris.destination_location_id).FirstOrDefault();
                    var destination_location = dbFind.vw_stockpile_location
                        .FromSqlRaw(" SELECT l1.id, l1.stockpile_location_code FROM vw_stockpile_location l1 "
                            + " WHERE l1.organization_id = {0} ",
                                CurrentUserContext.OrganizationId
                            )
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.stockpile_location_code
                            })
                        .Where(o => o.value == baris.destination_location_id).FirstOrDefault();

                    if (destination_location != null) destination_location_name = destination_location.text.ToString();

                    var equipment_code = "";
                    var equipment = dbFind.equipment.Where(o => o.id == baris.equipment_id).FirstOrDefault();
                    if (equipment != null) equipment_code = equipment.equipment_code.ToString();

                    //var survey_number = "";
                    // var survey = dbFind.survey.Where(o => o.id == baris.survey_id).FirstOrDefault();
                    //if (survey != null) survey_number = survey.survey_number.ToString();

                    /*  var despatch_order_number = "";
                      var despatch_order = dbFind.despatch_order.Where(o => o.id == baris.despatch_order_id).FirstOrDefault();
                      if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();*/

                    var quality_sampling_number = "";
                    var quality_sampling = dbFind.quality_sampling.Where(o => o.id == baris.quality_sampling_id).FirstOrDefault();
                    if (quality_sampling != null) quality_sampling_number = quality_sampling.sampling_number.ToString();

                    //var progress_claim_name = "";
                    //var progress_claim = dbFind.progress_claim.Where(o => o.id == baris.progress_claim_id).FirstOrDefault();
                    //if (progress_claim != null) progress_claim_name = progress_claim.progress_claim_name.ToString();

                    var employee_number = "";
                    var employee = dbFind.employee.Where(o => o.id == baris.pic).FirstOrDefault();
                    if (employee != null) employee_number = employee.employee_number.ToString();

                    var advance_contract_number = "";
                    var advance_contract1 = dbFind.advance_contract.Where(o => o.id == baris.advance_contract_id).FirstOrDefault();
                    if (advance_contract1 != null) advance_contract_number = advance_contract1.advance_contract_number.ToString();

                    var business_unit_code = "";
                    var business_unit = dbFind.business_unit.Where(o =>
                            o.id == baris.business_unit_id).FirstOrDefault();
                    if (business_unit != null) business_unit_code = business_unit.business_unit_code.ToString();


                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.transaction_number);
                    row.CreateCell(1).SetCellValue(Convert.ToDateTime(baris.loading_datetime).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(2).SetCellValue(Convert.ToDateTime(baris.unloading_datetime).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(3).SetCellValue(source_shift_name);
                    row.CreateCell(4).SetCellValue(process_flow_name);
                    row.CreateCell(5).SetCellValue(transport_id);
                    row.CreateCell(6).SetCellValue(source_location_name);
                    row.CreateCell(7).SetCellValue(destination_location_name);
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(baris.loading_quantity));
                    row.CreateCell(9).SetCellValue(uom_symbol);
                    row.CreateCell(10).SetCellValue(product_code);
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(baris.distance));
                    row.CreateCell(12).SetCellValue(quality_sampling_number);
                    row.CreateCell(13).SetCellValue(advance_contract_number);
                    row.CreateCell(14).SetCellValue(employee_number);
                    row.CreateCell(15).SetCellValue(baris.note);
                    row.CreateCell(16).SetCellValue(contractor_code);
                    row.CreateCell(17).SetCellValue(equipment_code);
                    row.CreateCell(18).SetCellValue(Convert.ToDouble(baris.gross));
                    row.CreateCell(19).SetCellValue(Convert.ToDouble(baris.tare));
                    row.CreateCell(20).SetCellValue(business_unit_code);

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

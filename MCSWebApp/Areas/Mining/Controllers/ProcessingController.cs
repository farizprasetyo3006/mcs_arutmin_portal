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
    public class ProcessingController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly IHubContext<ProgressHub> _hubContext;

        public ProcessingController(IConfiguration Configuration, IHubContext<ProgressHub> hubContext)
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
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.CoalProduce];
            ViewBag.BreadcrumbCode = WebAppMenu.CoalProduce;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string operationId = Data.operationId;
            
            string sFileName = "Coal Produce.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Coal Produce");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("DateTime In");
                row.CreateCell(2).SetCellValue("Shift");
                row.CreateCell(3).SetCellValue("Process Flow");
                row.CreateCell(4).SetCellValue("Equipment");
                row.CreateCell(5).SetCellValue("Source");
                row.CreateCell(6).SetCellValue("Destination");
                row.CreateCell(7).SetCellValue("In Quantity");
                row.CreateCell(8).SetCellValue("Out Quantity");
                row.CreateCell(9).SetCellValue("Product");
                row.CreateCell(10).SetCellValue("Quality Sampling");
                row.CreateCell(11).SetCellValue("Contract Reference");
                row.CreateCell(12).SetCellValue("PIC");
                row.CreateCell(13).SetCellValue("Note");
                row.CreateCell(14).SetCellValue("Business Unit");
                row.CreateCell(15).SetCellValue("Contractor");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;
                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var business_unit1 = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                IOrderedQueryable<processing_transaction> tabledata;
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    tabledata = dbContext.processing_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.loading_datetime);
                }
                else
                {
                    tabledata = dbContext.processing_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId )
                    .OrderByDescending(o => o.loading_datetime);
                }
                int totalData = tabledata.Count();
                int count = 1;
                await _hubContext.Clients.Group(operationId).SendAsync("ReceiveDownloadProgress", count, totalData);

                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    await _hubContext.Clients.Group(operationId).SendAsync("ReceiveDownloadProgress", count, totalData);
                    count++; 
                    
                    var process_flow_code = "";
                    var process_flow = dbFind.process_flow.Where(o => o.id == Value.process_flow_id).FirstOrDefault();
                    if (process_flow != null) process_flow_code = process_flow.process_flow_code.ToString();

                    var equipment_code = "";
                    var equipment = dbFind.equipment.Where(o => o.id == Value.equipment_id).FirstOrDefault();
                    if (equipment != null) equipment_code = equipment.equipment_code.ToString();

                    /*var processing_category_name = "";
                    var processing_category = dbFind.processing_category.Where(o => o.id == Value.processing_category_id).FirstOrDefault();
                    if (processing_category != null) processing_category_name = processing_category.processing_category_name.ToString();

                    var transport_name = "";
                    var truck = dbFind.transport.Where(o => o.id == Value.transport_id).FirstOrDefault();
                    if (truck != null) transport_name = truck.vehicle_name.ToString();

                    var accounting_period_name = "";
                    var accounting_period = dbFind.accounting_period.Where(o => o.id == Value.accounting_period_id).FirstOrDefault();
                    if (accounting_period != null) accounting_period_name = accounting_period.accounting_period_name.ToString();*/

                    var advance_contract_number = "";
                    var advance_contract = dbFind.advance_contract.Where(o => o.id == Value.advance_contract_id1).FirstOrDefault();
                    if (advance_contract != null) advance_contract_number = advance_contract.advance_contract_number.ToString();

                    var source_location_code = "";
                    var source_location = dbFind.stockpile_location.Where(o => o.id == Value.source_location_id).FirstOrDefault();
                    if (source_location != null) source_location_code = source_location.stockpile_location_code.ToString();

                    var destination_location_code = "";
                    var destination_location = dbFind.stockpile_location.Where(o => o.id == Value.destination_location_id).FirstOrDefault();
                    if (destination_location != null) destination_location_code = destination_location.stockpile_location_code.ToString();

                    var source_shift_code = "";
                    var source_shift = dbFind.shift.Where(o => o.id == Value.source_shift_id).FirstOrDefault();
                    if (source_shift != null) source_shift_code = source_shift.shift_code.ToString();

                    var product_code = "";
                    var source_product = dbFind.product.Where(o => o.id == Value.source_product_id).FirstOrDefault();
                    if (source_product != null) product_code = source_product.product_code.ToString();

                    var source_unit_symbol = "";
                    var source_unit = dbFind.uom.Where(o => o.id == Value.source_uom_id).FirstOrDefault();
                    if (source_unit != null) source_unit_symbol = source_unit.uom_name.ToString();

                    //var destination_shift_name = "";
                    //var destination_shift = dbFind.shift.Where(o => o.id == Value.destination_shift_id).FirstOrDefault();
                    //if (destination_shift != null) destination_shift_name = destination_shift.shift_name.ToString();

                    //var survey_number = "";
                    //var survey = dbFind.survey.Where(o => o.id == Value.survey_id).FirstOrDefault();
                    //if (survey != null) survey_number = survey.survey_number.ToString();

                    var despatch_order_number = "";
                    var despatch_order = dbFind.despatch_order.Where(o => o.id == Value.despatch_order_id).FirstOrDefault();
                    if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                    var quality_sampling_number = "";
                    var quality_sampling = dbFind.quality_sampling.Where(o => o.id == Value.quality_sampling_id).FirstOrDefault();
                    if (quality_sampling != null) quality_sampling_number = quality_sampling.sampling_number.ToString();

                    var employee_number = "";
                    var employee = dbFind.employee.Where(o => o.id == Value.pic).FirstOrDefault();
                    if (employee != null) employee_number = employee.employee_number.ToString();

                    var business_unit_name = "";
                    var business_unit = dbFind.business_unit.Where(o => o.id == Value.business_unit_id).FirstOrDefault();
                    if (business_unit != null) business_unit_name = business_unit.business_unit_code.ToString();

                    var contractor = "";
                    var c = dbFind.contractor.Where(o => o.id == Value.contractor_id).FirstOrDefault();
                    if (c != null)contractor = c.business_partner_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(Value.transaction_number);
                    row.CreateCell(1).SetCellValue(Convert.ToDateTime(Value.loading_datetime).ToString("yyyy-MM-dd HH:mm"));
                    row.CreateCell(2).SetCellValue(source_shift_code);
                    row.CreateCell(3).SetCellValue(process_flow_code);
                    row.CreateCell(4).SetCellValue(equipment_code);
                    row.CreateCell(5).SetCellValue(source_location_code);
                    row.CreateCell(6).SetCellValue(destination_location_code);
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(Value.loading_quantity));
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(Value.unloading_quantity));
                    row.CreateCell(9).SetCellValue(product_code);
                    row.CreateCell(10).SetCellValue(quality_sampling_number);
                    row.CreateCell(11).SetCellValue(advance_contract_number);
                    row.CreateCell(12).SetCellValue(employee_number);
                    row.CreateCell(13).SetCellValue(Value.note);
                    row.CreateCell(14).SetCellValue(business_unit_name);
                    row.CreateCell(15).SetCellValue(contractor);

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

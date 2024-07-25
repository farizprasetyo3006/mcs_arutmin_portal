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

namespace MCSWebApp.Areas.Port.Controllers
{
    [Area("Port")]
    public class CoalMovementController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public CoalMovementController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Port];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.CoalMovement];
            ViewBag.BreadcrumbCode = WebAppMenu.CoalMovement;

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "CoalMovement.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("CoalMovement");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("DateTime");
                row.CreateCell(2).SetCellValue("Shift");
                row.CreateCell(3).SetCellValue("Process Flow"); //("Accounting Period");
                row.CreateCell(4).SetCellValue("Shipping Order");
                row.CreateCell(5).SetCellValue("Source");
                row.CreateCell(6).SetCellValue("Destination");
                row.CreateCell(7).SetCellValue("Net Quantity");
                row.CreateCell(8).SetCellValue("Product");
                row.CreateCell(9).SetCellValue("Quality Sampling");
                row.CreateCell(10).SetCellValue("Equipment");
                row.CreateCell(11).SetCellValue("PIC");// ("Quality Survey");
                row.CreateCell(12).SetCellValue("Business Unit");
                row.CreateCell(13).SetCellValue("Note");
                row.CreateCell(14).SetCellValue("Contractor");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var business_unit1 = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var tabledata = dbContext.coal_movement
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.loading_datetime);
                    // Inserting values to table
                    foreach (var Value in tabledata)
                    {
                        var process_flow_code = "";
                        var process_flow = dbFind.process_flow.Where(o => o.id == Value.process_flow_id).FirstOrDefault();
                        if (process_flow != null) process_flow_code = process_flow.process_flow_code.ToString();

                        var source_location_code = "";
                        dynamic source_location = dbFind.stockpile_location.Where(o => o.id == Value.source_location_id).FirstOrDefault();
                        if (source_location != null)
                            source_location_code = source_location.stockpile_location_code.ToString();
                        else
                        {
                            source_location = dbFind.barge.Where(o => o.id == Value.source_location_id).FirstOrDefault();
                            if (source_location != null)
                                source_location_code = source_location.vehicle_id.ToString();
                        }

                        var destination_location_code = "";
                        dynamic destination_location = dbFind.stockpile_location.Where(o => o.id == Value.destination_location_id).FirstOrDefault();
                        if (destination_location != null)
                            destination_location_code = destination_location.stockpile_location_code.ToString();
                        else
                        {
                            destination_location = dbFind.vessel.Where(o => o.id == Value.destination_location_id).FirstOrDefault();
                            if (destination_location != null)
                                destination_location_code = destination_location.vehicle_name.ToString();
                        }

                        var source_shift_code = "";
                        var source_shift = dbFind.shift.Where(o => o.id == Value.source_shift_id).FirstOrDefault();
                        if (source_shift != null) source_shift_code = source_shift.shift_code.ToString();

                        var quality_sampling = "";
                        var qs = dbFind.quality_sampling.Where(o => o.id == Value.quality_sampling_id).FirstOrDefault();
                        if (qs != null) quality_sampling = qs.sampling_number.ToString();

                        var product_code = "";
                        var product = dbFind.product.Where(o => o.id == Value.product_id).FirstOrDefault();
                        if (product != null) product_code = product.product_code.ToString();

                        var so_number = "";
                        dynamic transport = dbFind.despatch_order.Where(o => o.id == Value.shipping_order_id).FirstOrDefault();
                        if (transport != null)
                            so_number = transport.despatch_order_number.ToString();
                        /*else
                        {
                            transport = dbFind.equipment.Where(o => o.id == Value.transport_id).FirstOrDefault();
                            if (transport != null)
                                transport_code = transport.equipment_code.ToString();
                        }*/

                        var equipment_code = "";
                        var equipment = dbFind.equipment.Where(o => o.id == Value.equipment_id).FirstOrDefault();
                        if (equipment != null) equipment_code = equipment.equipment_code.ToString();

                        var pic = "";
                        var p = dbFind.employee.Where(o => o.id == Value.pic).FirstOrDefault();
                        if (p != null) pic = p.employee_number.ToString();

                        var contractor = "";
                        var cc = await dbFind.contractor.Where(o=>o.id ==  Value.contractor_id).FirstOrDefaultAsync();
                        if(cc != null) contractor = cc.business_partner_code.ToString();

                        var business_unit = "";
                        var BU = dbFind.business_unit.Where(o => o.id == Value.business_unit_id).FirstOrDefault();
                        if (BU != null) business_unit = BU.business_unit_code.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(Value.transaction_number);
                        row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(Value.loading_datetime).ToString("yyyy-MM-dd HH:mm"));
                        row.CreateCell(2).SetCellValue(source_shift_code);
                        row.CreateCell(3).SetCellValue(process_flow_code);
                        row.CreateCell(4).SetCellValue(so_number);
                        row.CreateCell(5).SetCellValue(source_location_code);
                        row.CreateCell(6).SetCellValue(destination_location_code);
                        row.CreateCell(7).SetCellValue(Convert.ToDouble(Value.loading_quantity));
                        row.CreateCell(8).SetCellValue(product_code);
                        row.CreateCell(9).SetCellValue(quality_sampling);
                        row.CreateCell(10).SetCellValue(equipment_code);
                        row.CreateCell(11).SetCellValue(pic);
                        row.CreateCell(12).SetCellValue(business_unit);
                        row.CreateCell(13).SetCellValue(Value.note);
                        row.CreateCell(14).SetCellValue(contractor);

                        RowCount++;
                        //if (RowCount > 50) break;
                    }
                }
               /* //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Coal Movement Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Coal Movement Transaction Number");
                row.CreateCell(1).SetCellValue("Business Unit");
                row.CreateCell(2).SetCellValue("Product");
                row.CreateCell(3).SetCellValue("Contractor");
                row.CreateCell(4).SetCellValue("Quantity");
                row.CreateCell(5).SetCellValue("Presentage");
                row.CreateCell(6).SetCellValue("Adjustment");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.lq_proportion.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.module == "Coal-Movement")
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var baris in detail)
                {
                    var header = dbContext.coal_movement.Where(o=>o.id == baris.header_id).FirstOrDefault();

                    var business_unit = "";
                    var BU = dbFind.business_unit.Where(o => o.id == baris.business_unit_id).FirstOrDefault();
                    if (BU != null) business_unit = BU.business_unit_code.ToString();

                    var product = "";
                    var p = dbFind.product.Where(o => o.id == baris.product_id).FirstOrDefault();
                    if (p != null) product = p.product_code.ToString();

                    var contractor = "";
                    var c = dbFind.contractor.Where(o => o.id == baris.contractor_id).FirstOrDefault();
                    if (c != null) contractor = c.business_partner_code.ToString();


                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(header.transaction_number);
                    row.CreateCell(1).SetCellValue(business_unit); //(isi.month_index));
                    row.CreateCell(2).SetCellValue(product);
                    row.CreateCell(3).SetCellValue(contractor);
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(baris.quantity));
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(baris.presentage));
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(baris.adjustment));

                    RowCount++;
                    if (RowCount > 20) break;
                }
                //*****************/

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
        }

    }
}

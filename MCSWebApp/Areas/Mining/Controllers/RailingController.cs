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

namespace MCSWebApp.Areas.Mining.Controllers
{
    [Area("Mining")]
    public class RailingController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public RailingController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Mining];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Railing];
            ViewBag.BreadcrumbCode = WebAppMenu.Railing;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "Railing.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Railing");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("Process Flow");
                row.CreateCell(2).SetCellValue("Transport");
                row.CreateCell(3).SetCellValue("Accounting Period");
                row.CreateCell(4).SetCellValue("Source Location");
                row.CreateCell(5).SetCellValue("Loading DateTime");
                row.CreateCell(6).SetCellValue("Source Shift");
                row.CreateCell(7).SetCellValue("Product");
                row.CreateCell(8).SetCellValue("Loading Qty");
                row.CreateCell(9).SetCellValue("Unit");
                row.CreateCell(10).SetCellValue("Unloading DateTime");
                row.CreateCell(11).SetCellValue("Destination Location");
                row.CreateCell(12).SetCellValue("Destination Shift");
                row.CreateCell(13).SetCellValue("Unloading Qty");
                row.CreateCell(14).SetCellValue("Quality Survey");
                row.CreateCell(15).SetCellValue("Shipping Order");
                row.CreateCell(16).SetCellValue("Progress Claim");
                row.CreateCell(17).SetCellValue("Note");
                row.CreateCell(18).SetCellValue("PIC");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var header = dbContext.railing_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.loading_datetime);
                // Inserting values to table
                foreach (var isi in header)
                {
                    var process_flow_name = "";
                    var process_flow = dbFind.process_flow.Where(o => o.id == isi.process_flow_id).FirstOrDefault();
                    if (process_flow != null) process_flow_name = process_flow.process_flow_name.ToString();

                    var transport_name = "";
                    var transport = dbFind.transport.Where(o => o.id == isi.transport_id).FirstOrDefault();
                    if (transport != null) transport_name = transport.vehicle_name.ToString();

                    var accounting_period_name = "";
                    var accounting_period = dbFind.accounting_period.Where(o => o.id == isi.accounting_period_id).FirstOrDefault();
                    if (accounting_period != null) accounting_period_name = accounting_period.accounting_period_name.ToString();

                    var source_location_code = "";
                    var source_location = dbFind.stockpile_location.Where(o => o.id == isi.source_location_id).FirstOrDefault();
                    if (source_location != null) source_location_code = source_location.stockpile_location_code.ToString();

                    var source_shift_name = "";
                    var shift = dbFind.shift.Where(o => o.id == isi.source_shift_id).FirstOrDefault();
                    if (shift != null) source_shift_name = shift.shift_name.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == isi.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    var product_name = "";
                    var product = dbFind.product.Where(o => o.id == isi.product_id).FirstOrDefault();
                    if (product != null) product_name = product.product_name.ToString();

                    var destination_location_code = "";
                    var destination_location = dbFind.stockpile_location.Where(o => o.id == isi.destination_location_id).FirstOrDefault();
                    if (destination_location != null) destination_location_code = destination_location.stockpile_location_code.ToString();

                    var destination_shift_name = "";
                    var destination_shift = dbFind.shift.Where(o => o.id == isi.destination_shift_id).FirstOrDefault();
                    if (destination_shift != null) destination_shift_name = destination_shift.shift_name.ToString();

                    var survey_number = "";
                    var survey = dbFind.survey.Where(o => o.id == isi.survey_id).FirstOrDefault();
                    if (survey != null) survey_number = survey.survey_number.ToString();

                    var despatch_order_number = "";
                    var despatch_order = dbFind.despatch_order.Where(o => o.id == isi.despatch_order_id).FirstOrDefault();
                    if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                    var progress_claim_name = "";
                    var progress_claim = dbFind.progress_claim.Where(o => o.id == isi.progress_claim_id).FirstOrDefault();
                    if (progress_claim != null) progress_claim_name = progress_claim.progress_claim_name.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(isi.transaction_number);
                    row.CreateCell(1).SetCellValue(process_flow_name);
                    row.CreateCell(2).SetCellValue(transport_name);
                    row.CreateCell(3).SetCellValue(accounting_period_name);
                    row.CreateCell(4).SetCellValue(source_location_code);
                    row.CreateCell(5).SetCellValue(" " + Convert.ToDateTime(isi.loading_datetime).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(6).SetCellValue(source_shift_name);
                    row.CreateCell(7).SetCellValue(product_name);
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(isi.loading_quantity));
                    row.CreateCell(9).SetCellValue(uom_symbol);
                    row.CreateCell(10).SetCellValue(" " + Convert.ToDateTime(isi.unloading_datetime).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(11).SetCellValue(destination_location_code);
                    row.CreateCell(12).SetCellValue(destination_shift_name);
                    row.CreateCell(13).SetCellValue(Convert.ToDouble(isi.unloading_quantity));
                    row.CreateCell(14).SetCellValue(survey_number);
                    row.CreateCell(15).SetCellValue(despatch_order_number);
                    row.CreateCell(16).SetCellValue(progress_claim_name);
                    row.CreateCell(17).SetCellValue(isi.note);
                    row.CreateCell(18).SetCellValue(isi.pic);

                    RowCount++;
                    if (RowCount > 50) break;
                }

                ///**************************************** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("Container");
                row.CreateCell(2).SetCellValue("Loading DateTime");
                row.CreateCell(3).SetCellValue("Loading Quantity");
                row.CreateCell(4).SetCellValue("Unloading DateTime");
                row.CreateCell(5).SetCellValue("Unloading Quantity");
                row.CreateCell(6).SetCellValue("Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.railing_transaction_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.loading_datetime);
                // Inserting values to table
                foreach (var isi in detail)
                {
                    var railing_transaction_number = "";
                    var railing_transaction = dbFind.railing_transaction.Where(o => o.id == isi.railing_transaction_id).FirstOrDefault();
                    if (railing_transaction != null) railing_transaction_number = railing_transaction.transaction_number.ToString();

                    var vehicle_name = "";
                    var wagon = dbFind.wagon.Where(o => o.id == isi.wagon_id).FirstOrDefault();
                    if (wagon != null) vehicle_name = wagon.vehicle_name.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == isi.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(railing_transaction_number);
                    row.CreateCell(1).SetCellValue(vehicle_name);
                    row.CreateCell(2).SetCellValue(isi.loading_datetime.ToString());
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(isi.loading_quantity));
                    row.CreateCell(4).SetCellValue(isi.unloading_datetime.ToString());
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(isi.unloading_quantity));
                    row.CreateCell(6).SetCellValue(uom_symbol);
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

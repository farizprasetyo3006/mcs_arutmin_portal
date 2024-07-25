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

namespace MCSWebApp.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class DespatchOrderController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public DespatchOrderController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesMarketing];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesContract];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DespatchOrder];
            ViewBag.BreadcrumbCode = WebAppMenu.DespatchOrder;

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "Shipping Order.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Shipping Order");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Shipping Order Number");
                row.CreateCell(1).SetCellValue("Shipping Order Date");
                row.CreateCell(2).SetCellValue("Shipment Plan");
                row.CreateCell(3).SetCellValue("Delivery Term");
                row.CreateCell(4).SetCellValue("Laycan Start");
                row.CreateCell(5).SetCellValue("Laycan End");
                row.CreateCell(6).SetCellValue("Vessel/Barge Name");
                row.CreateCell(7).SetCellValue("Contract Term");
                row.CreateCell(8).SetCellValue("Cargo Qty");
                row.CreateCell(9).SetCellValue("Unit");
                row.CreateCell(10).SetCellValue("Turn Time");
                row.CreateCell(11).SetCellValue("Loading Rate");
                row.CreateCell(12).SetCellValue("Despatch Demurrage Rate");
                row.CreateCell(13).SetCellValue("Despatch (%)");
                row.CreateCell(14).SetCellValue("Seller");
                row.CreateCell(15).SetCellValue("Buyer");
                row.CreateCell(16).SetCellValue("Loading Port");
                row.CreateCell(17).SetCellValue("Discharge Port");
                row.CreateCell(18).SetCellValue("LC Number");
                row.CreateCell(19).SetCellValue("Surveyor");
                row.CreateCell(20).SetCellValue("Shipping Agent");
                row.CreateCell(21).SetCellValue("Notes");
                row.CreateCell(22).SetCellValue("Royalty Number");
                row.CreateCell(23).SetCellValue("Invoice Number");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                var tabledata = await dbContext.despatch_order
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                .OrderByDescending(o => o.despatch_order_date).ToListAsync();
                // Inserting values to table
                foreach (var isi in tabledata)
                {
                    var shipping_order_number = string.Empty;
                    shipping_order_number = isi.despatch_order_number;

                    DateTime? shipping_order_date = DateTime.MinValue;
                    shipping_order_date = isi.despatch_order_date;

                    var shipment_plan_name = string.Empty;
                    var shipment_plan = await dbContext.shipment_plan
                        .Where(x => x.id == isi.despatch_plan_id && x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();
                    if (shipment_plan != null) shipment_plan_name = shipment_plan.lineup_number;

                    var delivery_term_name = string.Empty;
                    var delivery_term = await dbContext.master_list
                        .Where(x => x.id == isi.delivery_term_id && x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();
                    if (delivery_term != null) delivery_term_name = delivery_term.item_name;

                    DateTime? laycan_start = DateTime.MinValue;
                    laycan_start = isi.laycan_start;

                    DateTime? laycan_end = DateTime.MinValue;
                    laycan_end = isi.laycan_end;

                    var vessel_barge_name = string.Empty;
                    var vessel = await dbContext.vessel
                        .Where(x => x.id == isi.vessel_id && x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();
                    if (vessel != null) vessel_barge_name = vessel.vehicle_name;
                    else
                    {
                        var barge = await dbContext.barge
                            .Where(x => x.id == isi.vessel_id && x.organization_id == CurrentUserContext.OrganizationId)
                            .FirstOrDefaultAsync();
                        if (barge != null) vessel_barge_name = barge.vehicle_name;
                    }

                    var contract_term_name = string.Empty;
                    var contract_term = await dbContext.sales_contract_term
                        .Where(x => x.id == isi.contract_term_id && x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();
                    if (contract_term != null) contract_term_name = contract_term.contract_term_name;

                    decimal? cargo_qty = 0;
                    cargo_qty = isi.required_quantity;

                    var uom_name = string.Empty;
                    var uom = await dbContext.uom
                        .Where(x => x.id == isi.uom_id && x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();
                    if (uom != null) uom_name = uom.uom_name;

                    decimal? turn_time = 0;
                    turn_time = isi.turn_time;

                    decimal? loading_rate = 0;
                    loading_rate = isi.loading_rate;

                    decimal? despatch_demurrage_rate = 0;
                    despatch_demurrage_rate = isi.despatch_demurrage_rate;

                    decimal? despatch_percentage = 0;
                    despatch_percentage = isi.despatch_percentage;

                    var seller_name = string.Empty;
                    var seller = await dbContext.organization
                        .Where(x => x.id == isi.seller_id && x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();
                    if (seller != null) seller_name = seller.organization_name;

                    var buyer_name = string.Empty;
                    var buyer = await dbContext.customer
                        .Where(x => x.id == isi.customer_id && x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();
                    if (buyer != null) buyer_name = buyer.business_partner_name;

                    var loadin_port_name = string.Empty;
                    var loading_port = await dbContext.port_location
                        .Where(x => x.id == isi.loading_port && x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();
                    if (loading_port != null) loadin_port_name = loading_port.stock_location_name;

                    var discharge_port_name = string.Empty;
                    discharge_port_name = isi.discharge_port;

                    var lc_number = string.Empty;
                    lc_number = isi.letter_of_credit;

                    var surveyor_name = string.Empty;
                    var surveyor = await dbContext.contractor
                        .Where(x => x.id == isi.surveyor_id && x.is_surveyor == true && x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();
                    if (surveyor != null) surveyor_name = surveyor.business_partner_name;

                    var shipping_agent = string.Empty;
                    shipping_agent = isi.shipping_agent;

                    var notes = string.Empty;
                    notes = isi.notes;

                    var royalty_number = string.Empty;
                    royalty_number = isi.royalty_number;

                    var invoice_number = string.Empty;
                    invoice_number = isi.invoice_number;

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(shipping_order_number);
                    row.CreateCell(1).SetCellValue(" " + shipping_order_date);
                    row.CreateCell(2).SetCellValue(shipment_plan_name);
                    row.CreateCell(3).SetCellValue(delivery_term_name);
                    row.CreateCell(4).SetCellValue(" " + laycan_start);
                    row.CreateCell(5).SetCellValue(" " + laycan_end);
                    row.CreateCell(6).SetCellValue(vessel_barge_name);
                    row.CreateCell(7).SetCellValue(contract_term_name);
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(cargo_qty));
                    row.CreateCell(9).SetCellValue(uom_name);
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(turn_time));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(loading_rate));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(despatch_demurrage_rate));
                    row.CreateCell(13).SetCellValue(Convert.ToDouble(despatch_percentage));
                    row.CreateCell(14).SetCellValue(seller_name);
                    row.CreateCell(15).SetCellValue(buyer_name);
                    row.CreateCell(16).SetCellValue(loadin_port_name);
                    row.CreateCell(17).SetCellValue(discharge_port_name);
                    row.CreateCell(18).SetCellValue(lc_number);
                    row.CreateCell(19).SetCellValue(surveyor_name);
                    row.CreateCell(20).SetCellValue(shipping_agent);
                    row.CreateCell(21).SetCellValue(notes);
                    row.CreateCell(22).SetCellValue(royalty_number);
                    row.CreateCell(23).SetCellValue(invoice_number);

                    RowCount++;
                    if (RowCount > 50) break;
                }

                ////***** detail
                //RowCount = 1;
                //excelSheet = workbook.CreateSheet("Shipping Order Delay");
                //row = excelSheet.CreateRow(0);
                //// Setting Cell Heading
                //row.CreateCell(0).SetCellValue("Shipping Order Number");
                //row.CreateCell(1).SetCellValue("Delay Category");
                //row.CreateCell(2).SetCellValue("Demurrage %");
                //row.CreateCell(3).SetCellValue("Despatch %");

                //excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                //var detail = dbContext.despatch_order_delay.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                //    .OrderByDescending(o => o.created_on);
                //// Inserting values to table
                //foreach (var isi in detail)
                //{
                //    var despatch_order_number = "";
                //    var despatch_order = dbFind.despatch_order.Where(o => o.id == isi.despatch_order_id).FirstOrDefault();
                //    if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                //    var delay_category_name = "";
                //    var delay_category = dbFind.delay_category.Where(o => o.id == isi.delay_category_id).FirstOrDefault();
                //    if (delay_category != null) delay_category_name = delay_category.delay_category_name.ToString();

                //    row = excelSheet.CreateRow(RowCount);
                //    row.CreateCell(0).SetCellValue(despatch_order_number);
                //    row.CreateCell(1).SetCellValue(delay_category_name);
                //    row.CreateCell(2).SetCellValue(Convert.ToDouble(isi.demurrage_percent));
                //    row.CreateCell(3).SetCellValue(Convert.ToDouble(isi.despatch_percent));

                //    RowCount++;
                //    if (RowCount > 50) break;
                //}
                ////****************

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

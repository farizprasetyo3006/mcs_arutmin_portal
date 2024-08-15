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
using System.Net;
using DocumentFormat.OpenXml.InkML;
using NPOI.HSSF.Record;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Planning.Controllers
{
    [Area("Planning")]
    public class ShipmentPlanController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ShipmentPlanController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ShipmentPlan];
            ViewBag.BreadcrumbCode = WebAppMenu.ShipmentPlan;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public IActionResult Detail()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ShipmentPlan];
            ViewBag.BreadcrumbCode = WebAppMenu.ShipmentPlan;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "ShipmentPlan.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("ShipmentPlan");
                IRow row = excelSheet.CreateRow(0);

                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Lineup Code");
                row.CreateCell(1).SetCellValue("Customer");
                row.CreateCell(2).SetCellValue("Contract Term Name");
                row.CreateCell(3).SetCellValue("Year");
                row.CreateCell(4).SetCellValue("Month");
                row.CreateCell(5).SetCellValue("Product");
                row.CreateCell(6).SetCellValue("Port of Discharge");
                row.CreateCell(7).SetCellValue("Shipment Number");
                row.CreateCell(8).SetCellValue("Incoterm");
                row.CreateCell(9).SetCellValue("Transport");
                row.CreateCell(10).SetCellValue("Laycan Start");
                row.CreateCell(11).SetCellValue("Laycan End");
                row.CreateCell(12).SetCellValue("Laycan Status");

                row.CreateCell(13).SetCellValue("Vessel Type");
                row.CreateCell(14).SetCellValue("Invoice No");
                row.CreateCell(15).SetCellValue("Royalty");
                row.CreateCell(16).SetCellValue("ETA");
                row.CreateCell(17).SetCellValue("ETA Status");

                row.CreateCell(18).SetCellValue("Contracted Tonage");
                row.CreateCell(19).SetCellValue("Business Unit");
                row.CreateCell(20).SetCellValue("Remarks");
                row.CreateCell(21).SetCellValue("Loading Contracted");
                row.CreateCell(22).SetCellValue("Loading Standard");
                row.CreateCell(23).SetCellValue("Dem USD/Day");
                row.CreateCell(24).SetCellValue("PLN Declared Month");
                row.CreateCell(25).SetCellValue("Certain");
                row.CreateCell(26).SetCellValue("Nora");
                row.CreateCell(27).SetCellValue("ETB");
                row.CreateCell(28).SetCellValue("ETC");
                row.CreateCell(29).SetCellValue("PLN Schedule");
                row.CreateCell(30).SetCellValue("Original Schedule");
                row.CreateCell(31).SetCellValue("Loading Port");
                row.CreateCell(32).SetCellValue("ETA Disc");
                row.CreateCell(33).SetCellValue("ETB Disc");
                row.CreateCell(34).SetCellValue("ETC Commence Disc");
                row.CreateCell(35).SetCellValue("ETC Completed Disc");
                row.CreateCell(36).SetCellValue("Stow Plan");
                row.CreateCell(37).SetCellValue("Transport Type");
                row.CreateCell(38).SetCellValue("Fc Provider");
                row.CreateCell(39).SetCellValue("Transport Provider");
                row.CreateCell(40).SetCellValue("Loadport Agent");
                row.CreateCell(41).SetCellValue("HPB Forecast");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                mcsContext dbFind2 = new mcsContext(DbOptionBuilder.Options);

                //---[Field Isian]---

                var yearToday = DateTime.Now.Year;
                var masterListYear = await dbContext.master_list
                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                    .Where(x => x.item_name == yearToday.ToString())
                    .FirstOrDefaultAsync();
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var header = await dbContext.vw_shipment_plan
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o=>selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.created_on).ToListAsync();

                    List<string> checkedId = new List<string>();
                    List<contractor> contractor = await dbFind.contractor.ToListAsync();
                        
                    foreach (var isi in header)
                    {
                        if (checkedId.Contains(isi.id)) continue;
                        else checkedId.Add(isi.id);

                        #region Variables

                        var szLineupCode = string.Empty;
                        var szCustomerName = string.Empty;
                        var szContractTermName = string.Empty;
                        var szYear = string.Empty;
                        var szMonth = string.Empty;
                        var szProductName = string.Empty;
                        var szDestination = string.Empty;
                        var szShipmentNumber = string.Empty;
                        var szIncoterm = string.Empty;
                        var szVesselBarge = string.Empty;
                        var szLaycanStart = string.Empty;
                        var szLaycanEnd = string.Empty;
                        var szVesselStatus = string.Empty;
                        var szInvoice = string.Empty;
                        var szRoyalty = string.Empty;
                        var szETA = string.Empty;
                        var szContractedTon = string.Empty;
                        var szBusinessUnit = string.Empty;
                        var szRemark = string.Empty;
                        var szLoadrateContract = string.Empty;
                        var szLoadrateStandard = string.Empty;
                        var szDemUSD = string.Empty;
                        var szPLNDeclaredMonth = string.Empty;
                        var szCertain = string.Empty;
                        var szNora = string.Empty;
                        var szETB = string.Empty;
                        var szETC = string.Empty;
                        var szPLNSchedule = string.Empty;
                        var szOriginSchedule = string.Empty;
                        var szLoadingPort = string.Empty;
                        var szETADisc = string.Empty;
                        var szETBDisc = string.Empty;
                        var szETCCommenceDisc = string.Empty;
                        var szETCCompletedDisc = string.Empty;
                        var szStowPlan = string.Empty;
                        var szTransportType = string.Empty;
                        var szLoadportAgent = string.Empty;
                        var szHPBForecast = string.Empty;

                        #endregion
                        #region Lineup Code

                        szLineupCode = string.IsNullOrEmpty(isi.lineup_number) ? string.Empty : isi.lineup_number;

                        #endregion
                        #region Customer

                        var resultCustomer = await dbContext.customer
                            .Where(x => x.id == isi.customer_id
                            && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                        if (resultCustomer != null) szCustomerName = string.IsNullOrEmpty(resultCustomer.business_partner_name) ? string.Empty : resultCustomer.business_partner_name;

                        #endregion
                        #region Contract Name

                        /*  var resultContract = await dbContext.sales_contract
                              .Where(x => x.customer_id == isi.customer_id).FirstOrDefaultAsync();
                          if (resultContract != null) szContractTermName = resultContract.id;
                          var resultContractTerm = await dbContext.sales_contract_term
                              .Where(x => x.sales_contract_id == szContractTermName).FirstOrDefaultAsync();
                          if (resultContractTerm != null) szContractTermName = resultContractTerm.contract_term_name ?? string.Empty;*/
                        var term = await dbContext.sales_contract_term.Where(o => o.id == isi.sales_contract_id).FirstOrDefaultAsync();
                        if (term != null) szContractTermName = term.contract_term_name.ToString();
                        #endregion
                        #region Year

                        var resultYear = await dbContext.master_list
                            .Where(x => x.id == isi.shipment_year).FirstOrDefaultAsync();
                        if (resultYear != null) szYear = string.IsNullOrEmpty(resultYear.item_name) ? string.Empty : resultYear.item_name;

                        #endregion
                        #region Month

                        szMonth = string.IsNullOrEmpty(isi.month_name) ? string.Empty : isi.month_name;

                        #endregion
                        #region Product

                        var resultProduct = await dbContext.product
                            .Where(x => x.id == isi.product_id
                            && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                        if (resultProduct != null) szProductName = resultProduct.product_name;

                        #endregion
                        #region Port of Discharge

                        szDestination = string.IsNullOrEmpty(isi.destination) ? string.Empty : isi.destination;

                        #endregion
                        #region Shipment Number

                        szShipmentNumber = string.IsNullOrEmpty(isi.shipment_number) ? string.Empty : isi.shipment_number;

                        #endregion
                        #region Incoterm

                        var resultIncoterm = await dbContext.master_list
                            .Where(x => x.id == isi.incoterm).SingleOrDefaultAsync();
                        if (resultIncoterm != null)
                            szIncoterm = string.IsNullOrEmpty(resultIncoterm.item_name) ? string.Empty : resultIncoterm.item_name;

                        #endregion
                        #region Transport Barge / Vessel

                        var resultBarge = await dbContext.barge
                            .Where(x => x.id == isi.transport_id).FirstOrDefaultAsync();
                        if (resultBarge != null)
                            szVesselBarge = string.IsNullOrEmpty(resultBarge.vehicle_name) ? string.Empty : resultBarge.vehicle_name;
                        else
                        {
                            var resultVessel = await dbContext.vessel
                            .Where(x => x.id == isi.transport_id).FirstOrDefaultAsync();
                            if (resultVessel != null)
                                szVesselBarge = string.IsNullOrEmpty(resultVessel.vehicle_name) ? string.Empty : resultVessel.vehicle_name;
                        }

                        #endregion
                        #region Laycan Start

                        szLaycanStart = string.IsNullOrEmpty(isi.laycan_start.ToString()) ? string.Empty : isi.laycan_start.ToString();

                        #endregion
                        #region Laycan End

                        szLaycanEnd = string.IsNullOrEmpty(isi.laycan_end.ToString()) ? string.Empty : isi.laycan_end.ToString();

                        #endregion
                        #region Vessel Type

                        var isIsGeared = isi.is_geared.HasValue;
                        if (isIsGeared)
                        {
                            var resultIsGeared = isi.is_geared.Value;
                            if (resultIsGeared) szVesselStatus = "G&G";
                            else szVesselStatus = "Gearless";
                        }

                        #endregion
                        #region Invoice No

                        szInvoice = string.IsNullOrEmpty(isi.invoice_no) ? string.Empty : isi.invoice_no.ToString();

                        #endregion
                        #region Royaltiy

                        szRoyalty = string.IsNullOrEmpty(isi.royalti) ? string.Empty : isi.royalti;

                        #endregion
                        #region ETA

                        szETA = string.IsNullOrEmpty(isi.eta.ToString()) ? string.Empty : isi.eta.ToString();

                        #endregion
                        #region Contracted Tonnage

                        if (isi.qty_sp.HasValue) szContractedTon = isi.qty_sp.Value.ToString("n2");

                        #endregion
                        #region Business Unit

                        szBusinessUnit = string.IsNullOrEmpty(isi.business_unit_name) ? string.Empty : isi.business_unit_name;

                        #endregion
                        #region Remarks

                        szRemark = string.IsNullOrEmpty(isi.remarks) ? string.Empty : isi.remarks;

                        #endregion
                        #region Loadrate Contracted

                        if (isi.loading_rate.HasValue)
                            szLoadrateContract = isi.loading_rate.Value.ToString("n2");

                        #endregion
                        #region Loadrate Standard

                        if (isi.loading_standart.HasValue)
                            szLoadrateStandard = isi.loading_standart.Value.ToString("n2");

                        #endregion
                        #region Desm USD/Day

                        if (isi.despatch_demurrage_rate.HasValue)
                            szDemUSD = isi.despatch_demurrage_rate.Value.ToString("n2");

                        #endregion
                        #region PLN Declared Month

                        if (!string.IsNullOrEmpty(isi.declared_month_id))
                            szPLNDeclaredMonth = isi.declared_month_id.ToString();

                        #endregion
                        #region Certain
                        
                        if (isi.certain != null)
                            szCertain = Convert.ToBoolean(isi.certain).ToString();

                        #endregion
                        #region Nora

                        if (isi.nora.HasValue) szNora = isi.nora.Value.ToString();

                        #endregion
                        #region ETB

                        if (isi.etb.HasValue) szETB = isi.etb.Value.ToString();

                        #endregion
                        #region ETC

                        if (isi.etc.HasValue) szETC = isi.etc.Value.ToString();

                        #endregion
                        #region PLN Schedule

                        szPLNSchedule = string.IsNullOrEmpty(isi.pln_schedule) ? string.Empty : isi.pln_schedule;

                        #endregion
                        #region Original Schedule

                        szOriginSchedule = string.IsNullOrEmpty(isi.original_schedule) ? string.Empty : isi.original_schedule;

                        #endregion
                        #region Loading Port

                        var resultLoadingPort = await dbContext.port_location
                            .Where(x => x.id == isi.loading_port
                            && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                        if (resultLoadingPort != null) szLoadingPort = resultLoadingPort.stock_location_name;

                        #endregion
                        #region ETA Disc

                        if (isi.eta_disc.HasValue) szETADisc = isi.eta_disc.Value.ToString();

                        #endregion
                        #region ETB Disc

                        if (isi.etb_disc.HasValue) szETBDisc = isi.etb_disc.Value.ToString();

                        #endregion
                        #region ETC Commence Disc

                        if (isi.etcommence_disc.HasValue) szETCCommenceDisc = isi.etcommence_disc.ToString();

                        #endregion
                        #region ETC Completed Disc

                        if (isi.etcompleted_disc.HasValue) szETCCompletedDisc = isi.etcompleted_disc.ToString();

                        #endregion
                        #region Stow Plan

                        if (isi.stow_plan.HasValue) szStowPlan = isi.stow_plan.Value.ToString();

                        #endregion
                        #region Transport Type

                        var transportType = await dbContext.master_list
                            .Where(x => x.id == isi.vessel_id).FirstOrDefaultAsync();
                        if (transportType != null) szTransportType = transportType.item_name ?? string.Empty;

                        #endregion
                        #region fc provider
                        dynamic c;
                        var fc = "";
                        c = contractor.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == isi.fc_provider_id).Select(o => o.business_partner_code).FirstOrDefault();
                        if (c != null) fc = c.ToString();
                        #endregion
                        #region transport provider

                        var transport_provider = "";
                        c = contractor.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == isi.transport_provider_id).Select(o => o.business_partner_code).FirstOrDefault();
                        if (c != null) transport_provider = c.ToString();

                        #endregion
                        #region Loadport Agent
                        
                        szLoadportAgent = isi.loadport_agent ?? string.Empty;

                        #endregion
                        #region HPB forecast
                        if (isi.hpb_forecast.HasValue) szHPBForecast = isi.hpb_forecast.Value.ToString("n2");
                        #endregion
                        //Laycan Status
                        var laycanStatusName = "";
                        var laycanStatus = await dbContext.master_list
                            .Where(x => x.id == isi.laycan_status).FirstOrDefaultAsync();
                        if (laycanStatus != null) laycanStatusName = laycanStatus.item_name ?? string.Empty;

                        //ETA Status
                        var ETAStatusName = "";
                        var ETAStatus = await dbContext.master_list
                            .Where(x => x.id == isi.eta_status).FirstOrDefaultAsync();
                        if (ETAStatus != null) ETAStatusName = ETAStatus.item_name ?? string.Empty;

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(szLineupCode);
                        row.CreateCell(1).SetCellValue(szCustomerName);
                        row.CreateCell(2).SetCellValue(szContractTermName);
                        row.CreateCell(3).SetCellValue(szYear);
                        row.CreateCell(4).SetCellValue(szMonth);
                        row.CreateCell(5).SetCellValue(szProductName);
                        row.CreateCell(6).SetCellValue(szDestination);
                        row.CreateCell(7).SetCellValue(szShipmentNumber);
                        row.CreateCell(8).SetCellValue(szIncoterm);
                        row.CreateCell(9).SetCellValue(szVesselBarge);
                        row.CreateCell(10).SetCellValue(szLaycanStart);
                        row.CreateCell(11).SetCellValue(szLaycanEnd);
                        row.CreateCell(12).SetCellValue(laycanStatusName);

                        row.CreateCell(13).SetCellValue(szVesselStatus);
                        row.CreateCell(14).SetCellValue(szInvoice);
                        row.CreateCell(15).SetCellValue(szRoyalty);
                        row.CreateCell(16).SetCellValue(szETA);
                        row.CreateCell(17).SetCellValue(ETAStatusName);

                        row.CreateCell(18).SetCellValue(szContractedTon);
                        row.CreateCell(19).SetCellValue(szBusinessUnit);
                        row.CreateCell(20).SetCellValue(szRemark);
                        row.CreateCell(21).SetCellValue(szLoadrateContract);
                        row.CreateCell(22).SetCellValue(szLoadrateStandard);
                        row.CreateCell(23).SetCellValue(szDemUSD);
                        row.CreateCell(24).SetCellValue(szPLNDeclaredMonth);
                        row.CreateCell(25).SetCellValue(szCertain);
                        row.CreateCell(26).SetCellValue(szNora);
                        row.CreateCell(27).SetCellValue(szETB);
                        row.CreateCell(28).SetCellValue(szETC);
                        row.CreateCell(29).SetCellValue(szPLNSchedule);
                        row.CreateCell(30).SetCellValue(szOriginSchedule);
                        row.CreateCell(31).SetCellValue(szLoadingPort);
                        row.CreateCell(32).SetCellValue(szETADisc);
                        row.CreateCell(33).SetCellValue(szETBDisc);
                        row.CreateCell(34).SetCellValue(szETCCommenceDisc);
                        row.CreateCell(35).SetCellValue(szETCCompletedDisc);
                        row.CreateCell(36).SetCellValue(szStowPlan);
                        row.CreateCell(37).SetCellValue(szTransportType);
                        row.CreateCell(38).SetCellValue(fc);
                        row.CreateCell(39).SetCellValue(transport_provider);
                        row.CreateCell(40).SetCellValue(szLoadportAgent); 
                        row.CreateCell(41).SetCellValue(szHPBForecast); 

                        RowCount++;
                        //if (RowCount > 50) break;
                    }
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

                finally
                {
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }

        public async Task<IActionResult> ExcelExportLineupRawData([FromBody] dynamic Data)
        {
            string sFileName = "LineupRawData.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("ShipmentPlan");
                IRow row = excelSheet.CreateRow(0);

                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Tipe Penjualan");
                row.CreateCell(1).SetCellValue("Year");
                row.CreateCell(2).SetCellValue("Month Loading");
                row.CreateCell(3).SetCellValue("PLN Declared Month");
                row.CreateCell(4).SetCellValue("Parcel");
                row.CreateCell(5).SetCellValue("Contract Term");
                row.CreateCell(6).SetCellValue("Commitment");
                row.CreateCell(7).SetCellValue("Buyer");
                row.CreateCell(8).SetCellValue("End Buyer");
                row.CreateCell(9).SetCellValue("Destination Country");
                row.CreateCell(10).SetCellValue("DMO");
                row.CreateCell(11).SetCellValue("Sales Agent");
                row.CreateCell(12).SetCellValue("Confirmation");
                row.CreateCell(13).SetCellValue("Shipment Number");
                row.CreateCell(14).SetCellValue("Shipping Term");
                row.CreateCell(15).SetCellValue("Vessel");
                row.CreateCell(16).SetCellValue("Laycan Start");
                row.CreateCell(17).SetCellValue("Laycan End");
                row.CreateCell(18).SetCellValue("Laycan Status");
                row.CreateCell(19).SetCellValue("Transport Type");
                row.CreateCell(20).SetCellValue("Invoice Number");
                row.CreateCell(21).SetCellValue("Royalty Number");
                row.CreateCell(22).SetCellValue("Barge/Vessel Provider");
                row.CreateCell(23).SetCellValue("FC Provider");
                row.CreateCell(24).SetCellValue("Remarks");
                row.CreateCell(25).SetCellValue("ETA");
                row.CreateCell(26).SetCellValue("ETA Status");
                row.CreateCell(27).SetCellValue("NORA");
                row.CreateCell(28).SetCellValue("ETB");
                row.CreateCell(29).SetCellValue("ETC");
                row.CreateCell(30).SetCellValue("Loading Port");
                row.CreateCell(31).SetCellValue("Loading Status");
                row.CreateCell(32).SetCellValue("Loadrate Actual (tpd)");
                row.CreateCell(33).SetCellValue("Dem USD/Day");
                row.CreateCell(34).SetCellValue("Dem Status");
                row.CreateCell(35).SetCellValue("Dem Hours");
                row.CreateCell(36).SetCellValue("Dem Days");
                row.CreateCell(37).SetCellValue("Dem USD");
                row.CreateCell(38).SetCellValue("Contracted Tonnage");
                row.CreateCell(39).SetCellValue("Stow Plan");
                row.CreateCell(40).SetCellValue("COB");
                row.CreateCell(41).SetCellValue("Loading Rate");
                row.CreateCell(42).SetCellValue("Loadrate Standart (tpd)");
                row.CreateCell(43).SetCellValue("Brand");
                row.CreateCell(44).SetCellValue("Loadport Agent");
                row.CreateCell(45).SetCellValue("Superintending Company");
                row.CreateCell(46).SetCellValue("SEN 0.8");
                row.CreateCell(47).SetCellValue("SEN 1.0");
                row.CreateCell(48).SetCellValue("SEN 1.5");
                row.CreateCell(49).SetCellValue("SEN 2.0");
                row.CreateCell(50).SetCellValue("Sat 10");
                row.CreateCell(51).SetCellValue("Sat 13");
                row.CreateCell(52).SetCellValue("Sat 15");
                row.CreateCell(53).SetCellValue("Sarongga");
                row.CreateCell(54).SetCellValue("Eco Mulia");
                row.CreateCell(55).SetCellValue("Kintap");
                row.CreateCell(56).SetCellValue("Eco Jumbang");
                row.CreateCell(57).SetCellValue("Eco Asam-asam");
                row.CreateCell(58).SetCellValue("Total Loading Tons");
                row.CreateCell(59).SetCellValue("Disc Tonnage");
                row.CreateCell(60).SetCellValue("ETA Disc");
                row.CreateCell(61).SetCellValue("ETB Disc");
                row.CreateCell(62).SetCellValue("ETCommence Disc");
                row.CreateCell(63).SetCellValue("ETCompleted Disc");
                row.CreateCell(64).SetCellValue("Index Link");
                row.CreateCell(65).SetCellValue("Contracted Price");
                row.CreateCell(66).SetCellValue("FOB Price");
                row.CreateCell(67).SetCellValue("Freight");
                row.CreateCell(68).SetCellValue("FOB Gross Revenue");
                row.CreateCell(69).SetCellValue("Total Gross Revenue");
                row.CreateCell(70).SetCellValue("Total Freight");
                row.CreateCell(71).SetCellValue("GCV(arb)");
                row.CreateCell(72).SetCellValue("ASH(arb)");
                row.CreateCell(73).SetCellValue("TS(arb)");
                row.CreateCell(74).SetCellValue("TM(arb)");
                row.CreateCell(75).SetCellValue("HBA");
                row.CreateCell(76).SetCellValue("HPB");
                row.CreateCell(77).SetCellValue("GCNEWC");
                row.CreateCell(78).SetCellValue("ICI-1");
                row.CreateCell(79).SetCellValue("ICI-4");
                row.CreateCell(80).SetCellValue("ICI-5");
                row.CreateCell(81).SetCellValue("PLATTS 5900");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;
                var header = await dbContext.vw_raw_data_lineup_transhipment
                    .FromSqlRaw(
                    "select * from vw_raw_data_lineup_transhipment r " +
                    "union all " +
                    "select * from vw_raw_data_lineup_nplct s " +
                    "union all " +
                    "select * from vw_raw_data_lineup_db t").ToListAsync();
                var count = 0;
                foreach (var isi in header)
                {
                    count++;
                    var sen8 = Convert.ToDouble(isi.sen08);
                    var sen10 = Convert.ToDouble(isi.sen10);
                    var sen15 = Convert.ToDouble(isi.sen15);
                    var sen20 = Convert.ToDouble(isi.sen20);
                    var sat10 = Convert.ToDouble(isi.sat10);
                    var sat13 = Convert.ToDouble(isi.sat13);
                    var sat15 = Convert.ToDouble(isi.sat15);
                    var sarongga = Convert.ToDouble(isi.eco_sarongga);
                    var eco_mul = Convert.ToDouble(isi.eco_mulia);
                    var eco_ktp = Convert.ToDouble(isi.eco_kintap);
                    var eco_jum = Convert.ToDouble(isi.eco_jumbang);
                    var eco_asm = Convert.ToDouble(isi.eco_asamasam);
                    var totalProduct = sen8 + sen10 + sen15 + sat10 + sat10 + sat13 + sat15 + sarongga + eco_mul + eco_ktp + eco_jum + eco_asm;

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(isi.tipe_penjualan);
                    row.CreateCell(1).SetCellValue(isi.year);
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(isi.month_sales));
                    row.CreateCell(3).SetCellValue(isi.pln_declare_month);
                    row.CreateCell(4).SetCellValue(isi.parcel);
                    row.CreateCell(5).SetCellValue(isi.contract_term_name);
                    row.CreateCell(6).SetCellValue(isi.commitment);
                    row.CreateCell(7).SetCellValue(isi.buyer);
                    row.CreateCell(8).SetCellValue(isi.end_buyer);
                    row.CreateCell(9).SetCellValue(isi.destination_country);
                    row.CreateCell(10).SetCellValue(isi.dmo);
                    row.CreateCell(11).SetCellValue(isi.sales_agent);
                    row.CreateCell(12).SetCellValue(isi.confirmation);
                    row.CreateCell(13).SetCellValue(isi.shipment_number);
                    row.CreateCell(14).SetCellValue(isi.shipping_term);
                    row.CreateCell(15).SetCellValue(isi.vessel);
                    row.CreateCell(16).SetCellValue(isi.laycan_start);
                    row.CreateCell(17).SetCellValue(isi.laycan_end);
                    row.CreateCell(18).SetCellValue(isi.laycan_status);
                    row.CreateCell(19).SetCellValue(isi.transport_type);
                    row.CreateCell(20).SetCellValue(isi.invoice_number);
                    row.CreateCell(21).SetCellValue(isi.royalty_number);
                    row.CreateCell(22).SetCellValue(isi.barge_or_vessel_provider);
                    row.CreateCell(23).SetCellValue(isi.fc_provider);
                    row.CreateCell(24).SetCellValue(isi.remarks);
                    row.CreateCell(25).SetCellValue(isi.eta);
                    row.CreateCell(26).SetCellValue(isi.eta_status);
                    row.CreateCell(27).SetCellValue(isi.nora);
                    row.CreateCell(28).SetCellValue(isi.etb);
                    row.CreateCell(29).SetCellValue(isi.etc);
                    row.CreateCell(30).SetCellValue(isi.loading_port);
                    row.CreateCell(31).SetCellValue(isi.loading_status);
                    row.CreateCell(32).SetCellValue(Convert.ToDouble(isi.loadrate_actual_tpd));
                    row.CreateCell(33).SetCellValue(Convert.ToDouble(isi.dem_usd_per_day));
                    row.CreateCell(34).SetCellValue(isi.usd_status);
                    row.CreateCell(35).SetCellValue(Convert.ToDouble(isi.dem_hours));
                    row.CreateCell(36).SetCellValue(Convert.ToDouble(isi.dem_days));
                    row.CreateCell(37).SetCellValue(Convert.ToDouble(isi.dem_usd));
                    row.CreateCell(38).SetCellValue(Convert.ToDouble(isi.contracted_tonnage)); 
                    row.CreateCell(39).SetCellValue(Convert.ToDouble(isi.stow_plan));
                    row.CreateCell(40).SetCellValue(Convert.ToDouble(isi.cob));
                    row.CreateCell(41).SetCellValue(Convert.ToDouble(isi.loading_rate));
                    row.CreateCell(42).SetCellValue(Convert.ToDouble(isi.loading_standart_tpd));
                    row.CreateCell(43).SetCellValue(isi.brand);
                    row.CreateCell(44).SetCellValue(isi.loadport_agent);
                    row.CreateCell(45).SetCellValue(isi.superintending_company);
                    row.CreateCell(46).SetCellValue(sen8);
                    row.CreateCell(47).SetCellValue(sen10);
                    row.CreateCell(48).SetCellValue(sen15);
                    row.CreateCell(49).SetCellValue(sen20);
                    row.CreateCell(50).SetCellValue(sat10);
                    row.CreateCell(51).SetCellValue(sat13);
                    row.CreateCell(52).SetCellValue(sat15);
                    row.CreateCell(53).SetCellValue(sarongga);
                    row.CreateCell(54).SetCellValue(eco_mul);
                    row.CreateCell(55).SetCellValue(eco_ktp);
                    row.CreateCell(56).SetCellValue(eco_jum);
                    row.CreateCell(57).SetCellValue(eco_asm);
                    row.CreateCell(58).SetCellValue(totalProduct);
                    row.CreateCell(59).SetCellValue(Convert.ToDouble(isi.disc_tonnage));
                    row.CreateCell(60).SetCellValue(isi.eta_disc);
                    row.CreateCell(61).SetCellValue(isi.etb_disc);
                    row.CreateCell(62).SetCellValue(isi.etcommence_disc);
                    row.CreateCell(63).SetCellValue(isi.pricing_status);
                    row.CreateCell(64).SetCellValue(isi.index_link);
                    row.CreateCell(65).SetCellValue(Convert.ToDouble(isi.contracted_price));
                    row.CreateCell(66).SetCellValue(Convert.ToDouble(isi.fob_price));
                    row.CreateCell(67).SetCellValue(Convert.ToDouble(isi.freight));
                    row.CreateCell(68).SetCellValue(Convert.ToDouble(isi.fob_gross_revenue));
                    row.CreateCell(69).SetCellValue(Convert.ToDouble(isi.total_gross_revenue));
                    row.CreateCell(70).SetCellValue(Convert.ToDouble(isi.total_freight));
                    row.CreateCell(71).SetCellValue(Convert.ToDouble(isi.gar));
                    row.CreateCell(72).SetCellValue(Convert.ToDouble(isi.ash));
                    row.CreateCell(73).SetCellValue(Convert.ToDouble(isi.ts));
                    row.CreateCell(74).SetCellValue(Convert.ToDouble(isi.tm));
                    row.CreateCell(75).SetCellValue(Convert.ToDouble(isi.hba));
                    row.CreateCell(76).SetCellValue(Convert.ToDouble(isi.hpb));
                    row.CreateCell(77).SetCellValue(Convert.ToDouble(isi.gcnewc));
                    row.CreateCell(78).SetCellValue(Convert.ToDouble(isi.ici1));
                    row.CreateCell(79).SetCellValue(Convert.ToDouble(isi.ici4));
                    row.CreateCell(80).SetCellValue(Convert.ToDouble(isi.ici5));
                    row.CreateCell(81).SetCellValue(Convert.ToDouble(isi.plastts_5900));

                    RowCount++;
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

                finally
                {
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }

    }
}

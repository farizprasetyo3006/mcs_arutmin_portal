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

namespace MCSWebApp.Areas.StockpileManagement.Controllers
{
    [Area("StockpileManagement")]
    public class QualitySamplingController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public QualitySamplingController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.StockpileManagement];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.QualitySampling];
            ViewBag.BreadcrumbCode = WebAppMenu.QualitySampling;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExportIC([FromBody] dynamic Data)
        {
            string sFileName = "QualitySamplingIC.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Quality Sampling");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sampling Number");
                row.CreateCell(1).SetCellValue("Sampling DateTime");
                row.CreateCell(2).SetCellValue("Surveyor");
                row.CreateCell(3).SetCellValue("Sampling Location");
                row.CreateCell(4).SetCellValue("Product");
                row.CreateCell(5).SetCellValue("Sampling Template");
                row.CreateCell(6).SetCellValue("Shipping Order");
                row.CreateCell(7).SetCellValue("Non Commercial");
                row.CreateCell(8).SetCellValue("Sampling Type");
                row.CreateCell(9).SetCellValue("Shift");
                row.CreateCell(10).SetCellValue("Is Draft");
                row.CreateCell(11).SetCellValue("Barging Number");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var HeaderIdList = new List<string>();
                var SamplingTemplateCodeList = new List<string>();
                var SamplingDateTimeList = new List<string>();
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var tabledata = dbContext.quality_sampling
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.sampling_datetime);

                    //.Take(1);   //*** limit to only 1 row
                    // Inserting values to table
                    foreach (var Value in tabledata)
                    {
                        HeaderIdList.Add(Value.id);
                        SamplingDateTimeList.Add(Convert.ToDateTime(Value.sampling_datetime).ToString("yyyy-MM-dd HH:mm"));

                        var surveyor_code = "";
                        var contractor = dbFind.contractor.Where(o => o.id == Value.surveyor_id).FirstOrDefault();
                        if (contractor != null) surveyor_code = contractor.business_partner_code.ToString();

                        var stockpile_location_code = "";
                        var stockpile = dbFind.stockpile_location
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Value.stock_location_id)
                            .Select(o => new { id = o.id, code = o.stockpile_location_code });

                        var ports = dbFind.port_location
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Value.stock_location_id)
                            .Select(o => new { id = o.id, code = o.port_location_code });

                        var mines = dbFind.mine_location
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Value.stock_location_id)
                            .Select(o => new { id = o.id, code = o.mine_location_code });

                        var barges = dbFind.barge
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Select(o => new { id = o.id, code = o.vehicle_name });

                        var vessels = dbFind.vessel
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Select(o => new { id = o.id, code = o.vehicle_name });

                        var lookup = stockpile.Union(barges).Union(vessels).Union(ports).Union(mines)
                            .Where(o => o.id == Value.stock_location_id)
                            .FirstOrDefault();
                        if (lookup != null) stockpile_location_code = lookup.code ?? "";

                        var product_code = "";
                        var product = dbFind.product.Where(o => o.id == Value.product_id).FirstOrDefault();
                        if (product != null) product_code = product.product_code.ToString();

                        var sampling_template_code = "";
                        var sampling_template = dbFind.sampling_template.Where(o => o.id == Value.sampling_template_id).FirstOrDefault();
                        if (sampling_template != null) sampling_template_code = sampling_template.sampling_template_code.ToString();

                        SamplingTemplateCodeList.Add(sampling_template_code);

                        var despatch_order_number = "";
                        var despatch_order = dbFind.despatch_order.Where(o => o.id == Value.despatch_order_id).FirstOrDefault();
                        if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                        var sampling_type_code = "";
                        //var master_list = dbFind.master_list.Where(o => o.id == Value.sampling_type_id).FirstOrDefault();
                        //if (master_list != null) sampling_type_code = master_list.item_in_coding.ToString();

                        var sampling_type = dbFind.sampling_type.Where(o => o.id == Value.sampling_type_id).FirstOrDefault();
                        if (sampling_type != null) sampling_type_code = sampling_type.sampling_type_code.ToString();

                        var shift_code = "";
                        var shift = dbFind.shift.Where(o => o.id == Value.shift_id).FirstOrDefault();
                        if (shift != null) shift_code = shift.shift_code.ToString();

                        var barging_number = "";
                        var barging_transaction = dbFind.barging_transaction.Where(o => o.id == Value.barging_transaction_id)
                            .FirstOrDefault();
                        if (barging_transaction != null) barging_number = barging_transaction.transaction_number.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(" " + Value.sampling_number);
                        row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(Value.sampling_datetime).ToString("yyyy-MM-dd HH:mm"));
                        row.CreateCell(2).SetCellValue(surveyor_code);
                        row.CreateCell(3).SetCellValue(stockpile_location_code);
                        row.CreateCell(4).SetCellValue(product_code);
                        row.CreateCell(5).SetCellValue(sampling_template_code);
                        row.CreateCell(6).SetCellValue(despatch_order_number);
                        row.CreateCell(7).SetCellValue(Convert.ToBoolean(Value.non_commercial));
                        row.CreateCell(8).SetCellValue(sampling_type_code);
                        row.CreateCell(9).SetCellValue(shift_code);
                        row.CreateCell(10).SetCellValue(Convert.ToBoolean(Value.is_draft));
                        row.CreateCell(11).SetCellValue(barging_number);

                        RowCount++;
                    }
                }
                ///**************************************** detail
                excelSheet = workbook.CreateSheet("Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sampling Number");
                row.CreateCell(1).SetCellValue("Sampling DateTime");
                row.CreateCell(2).SetCellValue("Sampling Template");
                row.CreateCell(3).SetCellValue("TM (arb)");
                row.CreateCell(4).SetCellValue("IM (adb)");
                row.CreateCell(5).SetCellValue("Ash (adb)");
                row.CreateCell(6).SetCellValue("VM (adb)");
                row.CreateCell(7).SetCellValue("FC (adb)");
                row.CreateCell(8).SetCellValue("TS (adb)");
                row.CreateCell(9).SetCellValue("GCV (adb)");
                row.CreateCell(10).SetCellValue("GCV (arb)");
                row.CreateCell(11).SetCellValue("TS (arb)");
                row.CreateCell(12).SetCellValue("Ash (arb)");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                string[] analyteSymbolList = { "tm(arb)", "im(adb)", "ash(adb)", "vm(adb)", "fc(adb)", "ts(adb)", "gcv(adb)", "gcv(arb)", "ts(arb)", "ash(arb)" };

                RowCount = 1;
                int listIndex = 0;
                foreach (var idHeader in HeaderIdList)
                {
                    var judul = dbContext.vw_quality_sampling_analyte
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.quality_sampling_id == idHeader)
                        .FirstOrDefault();

                    if (judul != null)
                    {

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(judul.sampling_number);
                        row.CreateCell(1).SetCellValue(SamplingDateTimeList[listIndex]);
                        row.CreateCell(2).SetCellValue(SamplingTemplateCodeList[listIndex]);

                        foreach (var analyteSymbol in analyteSymbolList)
                        {
                            var detail = dbContext.vw_quality_sampling_analyte
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.quality_sampling_id == idHeader &&
                                            o.analyte_symbol.Replace(" ", "").ToLower() == analyteSymbol)
                                .FirstOrDefault();

                            if (detail != null)
                            {
                                switch (analyteSymbol)
                                {
                                    case "tm(arb)":
                                        row.CreateCell(3).SetCellValue(Convert.ToDouble(detail.analyte_value));
                                        break;
                                    case "im(adb)":
                                        row.CreateCell(4).SetCellValue(Convert.ToDouble(detail.analyte_value));
                                        break;
                                    case "ash(adb)":
                                        row.CreateCell(5).SetCellValue(Convert.ToDouble(detail.analyte_value));
                                        break;
                                    case "vm(adb)":
                                        row.CreateCell(6).SetCellValue(Convert.ToDouble(detail.analyte_value));
                                        break;
                                    case "fc(adb)":
                                        row.CreateCell(7).SetCellValue(Convert.ToDouble(detail.analyte_value));
                                        break;
                                    case "ts(adb)":
                                        row.CreateCell(8).SetCellValue(Convert.ToDouble(detail.analyte_value));
                                        break;
                                    case "gcv(adb)":
                                        row.CreateCell(9).SetCellValue(Convert.ToDouble(detail.analyte_value));
                                        break;
                                    case "gcv(arb)":
                                        row.CreateCell(10).SetCellValue(Convert.ToDouble(detail.analyte_value));
                                        break;
                                    case "ts(arb)":
                                        row.CreateCell(11).SetCellValue(Convert.ToDouble(detail.analyte_value));
                                        break;
                                    case "ash(arb)":
                                        row.CreateCell(12).SetCellValue(Convert.ToDouble(detail.analyte_value));
                                        break;
                                }
                            }
                        }

                        RowCount++;
                        if (RowCount > 50) break;
                    }

                    listIndex++;
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
                // Deletes the generated file from /wwwroot folder
                finally
                {
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }

        public async Task<IActionResult> ExcelExportAI([FromBody] dynamic Data)
        {
            string sFileName = "QualitySamplingIC.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Quality Sampling");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sampling Number");
                row.CreateCell(1).SetCellValue("Sampling DateTime");
                row.CreateCell(2).SetCellValue("Surveyor");
                row.CreateCell(3).SetCellValue("Sampling Location");
                row.CreateCell(4).SetCellValue("Product");
                row.CreateCell(5).SetCellValue("Sampling Template");
                row.CreateCell(6).SetCellValue("Shipping Order");
                row.CreateCell(7).SetCellValue("Non Commercial");
                row.CreateCell(8).SetCellValue("Sampling Type");
                row.CreateCell(9).SetCellValue("Shift");
                row.CreateCell(10).SetCellValue("Is Draft");
                row.CreateCell(11).SetCellValue("Barging Number");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var HeaderIdList = new List<string>();
                var SamplingTemplateCodeList = new List<string>();
                var SamplingDateTimeList = new List<string>();

                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var tabledata = dbContext.quality_sampling
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.sampling_datetime);

                    //.Take(1);   //*** limit to only 1 row
                    // Inserting values to table
                    foreach (var Value in tabledata)
                    {
                        HeaderIdList.Add(Value.id);
                        SamplingDateTimeList.Add(Convert.ToDateTime(Value.sampling_datetime).ToString("yyyy-MM-dd HH:mm"));

                        var surveyor_code = "";
                        var contractor = dbFind.contractor.Where(o => o.id == Value.surveyor_id).FirstOrDefault();
                        if (contractor != null) surveyor_code = contractor.business_partner_code.ToString();

                        var stockpile_location_code = "";
                        var stockpile = dbFind.stockpile_location
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Value.stock_location_id)
                            .Select(o => new { id = o.id, code = o.stockpile_location_code });

                        var ports = dbFind.port_location
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Value.stock_location_id)
                            .Select(o => new { id = o.id, code = o.port_location_code });

                        var mines = dbFind.mine_location
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Value.stock_location_id)
                            .Select(o => new { id = o.id, code = o.mine_location_code });

                        var barges = dbFind.barge
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Select(o => new { id = o.id, code = o.vehicle_name });

                        var vessels = dbFind.vessel
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Select(o => new { id = o.id, code = o.vehicle_name });

                        var lookup = stockpile.Union(barges).Union(vessels).Union(ports).Union(mines)
                            .Where(o => o.id == Value.stock_location_id)
                            .FirstOrDefault();
                        if (lookup != null) stockpile_location_code = lookup.code ?? "";

                        var product_code = "";
                        var product = dbFind.product.Where(o => o.id == Value.product_id).FirstOrDefault();
                        if (product != null) product_code = product.product_code.ToString();

                        var sampling_template_code = "";
                        var sampling_template = dbFind.sampling_template.Where(o => o.id == Value.sampling_template_id).FirstOrDefault();
                        if (sampling_template != null) sampling_template_code = sampling_template.sampling_template_code.ToString();

                        SamplingTemplateCodeList.Add(sampling_template_code);

                        var despatch_order_number = "";
                        var despatch_order = dbFind.despatch_order.Where(o => o.id == Value.despatch_order_id).FirstOrDefault();
                        if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                        var sampling_type_code = "";
                        //var master_list = dbFind.master_list.Where(o => o.id == Value.sampling_type_id).FirstOrDefault();
                        //if (master_list != null) sampling_type_code = master_list.item_in_coding.ToString();

                        var sampling_type = dbFind.sampling_type.Where(o => o.id == Value.sampling_type_id).FirstOrDefault();
                        if (sampling_type != null) sampling_type_code = sampling_type.sampling_type_code.ToString();

                        var shift_code = "";
                        var shift = dbFind.shift.Where(o => o.id == Value.shift_id).FirstOrDefault();
                        if (shift != null) shift_code = shift.shift_code.ToString();

                        var barging_number = "";
                        var barging_transaction = dbFind.barging_transaction.Where(o => o.id == Value.barging_transaction_id)
                            .FirstOrDefault();
                        if (barging_transaction != null) barging_number = barging_transaction.transaction_number.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(" " + Value.sampling_number);
                        row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(Value.sampling_datetime).ToString("yyyy-MM-dd HH:mm"));
                        row.CreateCell(2).SetCellValue(surveyor_code);
                        row.CreateCell(3).SetCellValue(stockpile_location_code);
                        row.CreateCell(4).SetCellValue(product_code);
                        row.CreateCell(5).SetCellValue(sampling_template_code);
                        row.CreateCell(6).SetCellValue(despatch_order_number);
                        row.CreateCell(7).SetCellValue(Convert.ToBoolean(Value.non_commercial));
                        row.CreateCell(8).SetCellValue(sampling_type_code);
                        row.CreateCell(9).SetCellValue(shift_code);
                        row.CreateCell(10).SetCellValue(Convert.ToBoolean(Value.is_draft));
                        row.CreateCell(11).SetCellValue(barging_number);

                        RowCount++;
                        if (RowCount > 10) break;
                    }
                }
                ///**************************************** detail
                excelSheet = workbook.CreateSheet("Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sampling Number");
                row.CreateCell(1).SetCellValue("Sampling DateTime");
                row.CreateCell(2).SetCellValue("Sampling Template");
                row.CreateCell(3).SetCellValue("Analyte");
                row.CreateCell(4).SetCellValue("Value");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                string[] analyteSymbolList = { "tm(arb)", "im(adb)", "ash(adb)", "vm(adb)", "fc(adb)", "ts(adb)", "gcv(adb)", "gcv(arb)", "ts(arb)", "ash(arb)" };

                RowCount = 1;
                int listIndex = 0;
                foreach (var idHeader in HeaderIdList)
                {
                    var judul = dbContext.vw_quality_sampling_analyte
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.quality_sampling_id == idHeader)
                        .ToList();

                    if (judul != null)
                    {
                        foreach (var item in judul)
                        {
                            var analyte = "";
                            var a = dbFind.analyte.Where(o => o.id == item.analyte_id).FirstOrDefault();
                            if (a != null) analyte = a.analyte_name;

                            var unit = "";
                            var u = dbFind.uom.Where(o => o.id == item.uom_id).FirstOrDefault();
                            if (u != null) unit = u.uom_name;

                            row = excelSheet.CreateRow(RowCount);
                            row.CreateCell(0).SetCellValue(item.sampling_number);
                            row.CreateCell(1).SetCellValue(SamplingDateTimeList[listIndex]);
                            row.CreateCell(2).SetCellValue(SamplingTemplateCodeList[listIndex]);
                            row.CreateCell(3).SetCellValue(analyte);
                            row.CreateCell(4).SetCellValue(Convert.ToDouble(item.analyte_value));

                            RowCount++;
                            if (RowCount > 50) break;
                        }
                    }

                    listIndex++;
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
                // Deletes the generated file from /wwwroot folder
                finally
                {
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }

        //public async Task<IActionResult> ExcelExportKMIA()
        //{
        //    string sFileName = "QualitySamplingKMIA.xlsx";
        //    sFileName = sFileName.Insert(sFileName.LastIndexOf("."), string.Format("_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff")));

        //    string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
        //    if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);

        //    FileInfo file = new FileInfo(Path.Combine(FilePath, sFileName));
        //    var memory = new MemoryStream();
        //    using (var fs = new FileStream(Path.Combine(FilePath, sFileName), FileMode.Create, FileAccess.Write))
        //    {
        //        int RowCount = 1;
        //        IWorkbook workbook;
        //        workbook = new XSSFWorkbook();
        //        ISheet excelSheet = workbook.CreateSheet("Quality Sampling");
        //        IRow row = excelSheet.CreateRow(0);
        //        // Setting Cell Heading
        //        row.CreateCell(0).SetCellValue("Sampling Number");
        //        row.CreateCell(1).SetCellValue("Sampling DateTime");
        //        row.CreateCell(2).SetCellValue("Surveyor");
        //        row.CreateCell(3).SetCellValue("Sampling Location");
        //        row.CreateCell(4).SetCellValue("Product");
        //        row.CreateCell(5).SetCellValue("Sampling Template");
        //        row.CreateCell(6).SetCellValue("Shipping Order");
        //        row.CreateCell(7).SetCellValue("Non Commercial");
        //        row.CreateCell(8).SetCellValue("Sampling Type");
        //        row.CreateCell(9).SetCellValue("Shift");
        //        row.CreateCell(10).SetCellValue("Is Draft");
        //        row.CreateCell(11).SetCellValue("Barging Number");

        //        excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

        //        mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

        //        var HeaderIdList = new List<string>();
        //        var SamplingTemplateCodeList = new List<string>();
        //        var SamplingDateTimeList = new List<string>();

        //        //var tabledata = dbContext.quality_sampling
        //        //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //        //    .OrderByDescending(o => o.sampling_datetime);

        //        var tabledata = dbContext.quality_sampling
        //            .Where(o => o.organization_.organization_code == "KM01")
        //            .OrderByDescending(o => o.sampling_datetime);
        //        foreach (var Value in tabledata)
        //        {
        //            HeaderIdList.Add(Value.id);
        //            SamplingDateTimeList.Add(Convert.ToDateTime(Value.sampling_datetime).ToString("yyyy-MM-dd HH:mm"));

        //            var surveyor_code = "";
        //            var contractor = dbFind.contractor.Where(o => o.id == Value.surveyor_id).FirstOrDefault();
        //            if (contractor != null) surveyor_code = contractor.business_partner_code.ToString();

        //            var stockpile_location_code = "";
        //            var stockpile = dbFind.stockpile_location
        //                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Value.stock_location_id)
        //                .Select(o => new { id = o.id, code = o.stockpile_location_code });

        //            var ports = dbFind.port_location
        //                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Value.stock_location_id)
        //                .Select(o => new { id = o.id, code = o.port_location_code });

        //            var mines = dbFind.mine_location
        //                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Value.stock_location_id)
        //                .Select(o => new { id = o.id, code = o.mine_location_code });

        //            var barges = dbFind.barge
        //                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //                .Select(o => new { id = o.id, code = o.vehicle_name });

        //            var vessels = dbFind.vessel
        //                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //                .Select(o => new { id = o.id, code = o.vehicle_name });

        //            var lookup = stockpile.Union(ports).Union(mines).Union(barges).Union(vessels)
        //                .Where(o => o.id == Value.stock_location_id)
        //                .FirstOrDefault();
        //            if (lookup != null) stockpile_location_code = lookup.code ?? "";

        //            var product_code = "";
        //            var product = dbFind.product.Where(o => o.id == Value.product_id).FirstOrDefault();
        //            if (product != null) product_code = product.product_code.ToString();

        //            var sampling_template_code = "";
        //            var sampling_template = dbFind.sampling_template.Where(o => o.id == Value.sampling_template_id).FirstOrDefault();
        //            if (sampling_template != null) sampling_template_code = sampling_template.sampling_template_code.ToString();

        //            SamplingTemplateCodeList.Add(sampling_template_code);

        //            var despatch_order_number = "";
        //            var despatch_order = dbFind.despatch_order.Where(o => o.id == Value.despatch_order_id).FirstOrDefault();
        //            if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

        //            var sampling_type_code = "";
        //            //var master_list = dbFind.master_list.Where(o => o.id == Value.sampling_type_id).FirstOrDefault();
        //            //if (master_list != null) sampling_type_code = master_list.item_in_coding.ToString();

        //            var sampling_type = dbFind.sampling_type.Where(o => o.id == Value.sampling_type_id).FirstOrDefault();
        //            if (sampling_type != null) sampling_type_code = sampling_type.sampling_type_code.ToString();

        //            var shift_code = "";
        //            var shift = dbFind.shift.Where(o => o.id == Value.shift_id).FirstOrDefault();
        //            if (shift != null) shift_code = shift.shift_code.ToString();

        //            var barging_number = "";
        //            var barging_transaction = dbFind.barging_transaction.Where(o => o.id == Value.barging_transaction_id)
        //                .FirstOrDefault();
        //            if (barging_transaction != null) barging_number = barging_transaction.transaction_number.ToString();

        //            row = excelSheet.CreateRow(RowCount);
        //            row.CreateCell(0).SetCellValue(" " + Value.sampling_number);
        //            row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(Value.sampling_datetime).ToString("yyyy-MM-dd HH:mm"));
        //            row.CreateCell(2).SetCellValue(surveyor_code);
        //            row.CreateCell(3).SetCellValue(stockpile_location_code);
        //            row.CreateCell(4).SetCellValue(product_code);
        //            row.CreateCell(5).SetCellValue(sampling_template_code);
        //            row.CreateCell(6).SetCellValue(despatch_order_number);
        //            row.CreateCell(7).SetCellValue(Convert.ToBoolean(Value.non_commercial));
        //            row.CreateCell(8).SetCellValue(sampling_type_code);
        //            row.CreateCell(9).SetCellValue(shift_code);
        //            row.CreateCell(10).SetCellValue(Convert.ToBoolean(Value.is_draft));
        //            row.CreateCell(11).SetCellValue(barging_number);

        //            RowCount++;
        //            if (RowCount > 10) break;
        //        }

        //        ///**************************************** detail
        //        excelSheet = workbook.CreateSheet("Detail");
        //        row = excelSheet.CreateRow(0);
        //        // Setting Cell Heading
        //        row.CreateCell(0).SetCellValue("Sampling Number");
        //        row.CreateCell(1).SetCellValue("Sampling DateTime");
        //        row.CreateCell(2).SetCellValue("Sampling Template");
        //        row.CreateCell(3).SetCellValue("TM (arb)");
        //        row.CreateCell(4).SetCellValue("IM (adb)");
        //        row.CreateCell(5).SetCellValue("Ash (arb)");
        //        row.CreateCell(6).SetCellValue("VM (adb)");
        //        row.CreateCell(7).SetCellValue("FC (adb)");
        //        row.CreateCell(8).SetCellValue("TS (adb)");
        //        row.CreateCell(9).SetCellValue("CV (adb)");
        //        row.CreateCell(10).SetCellValue("CV (daf)");
        //        row.CreateCell(11).SetCellValue("CV (arb)");

        //        excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

        //        string[] analyteSymbolList = { "tm(arb)", "im(adb)", "ash(arb)", "vm(adb)", "fc(adb)", "ts(adb)", "cv(adb)", "cv(daf)", "cv(arb)" };

        //        RowCount = 1;
        //        int listIndex = 0;
        //        foreach (var idHeader in HeaderIdList)
        //        {
        //            var judul = dbContext.vw_quality_sampling_analyte
        //                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                            o.quality_sampling_id == idHeader &&
        //                            SamplingTemplateCodeList[listIndex].ToLower() == "channel sampling standard")
        //                .FirstOrDefault();
        //            if (judul != null)
        //            {
        //                row = excelSheet.CreateRow(RowCount);
        //                row.CreateCell(0).SetCellValue(judul.sampling_number);
        //                row.CreateCell(1).SetCellValue(SamplingDateTimeList[listIndex]);
        //                row.CreateCell(2).SetCellValue(SamplingTemplateCodeList[listIndex]);

        //                foreach (var analyteSymbol in analyteSymbolList)
        //                {
        //                    var detail = dbContext.vw_quality_sampling_analyte
        //                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                                    o.quality_sampling_id == idHeader &&
        //                                    o.analyte_symbol.Replace(" ", "").ToLower() == analyteSymbol)
        //                        .FirstOrDefault();

        //                    if (detail != null)
        //                    {
        //                        switch (analyteSymbol)
        //                        {
        //                            case "tm(arb)":
        //                                row.CreateCell(3).SetCellValue(Convert.ToDouble(detail.analyte_value));
        //                                break;
        //                            case "im(adb)":
        //                                row.CreateCell(4).SetCellValue(Convert.ToDouble(detail.analyte_value));
        //                                break;
        //                            case "ash(arb)":
        //                                row.CreateCell(5).SetCellValue(Convert.ToDouble(detail.analyte_value));
        //                                break;
        //                            case "vm(adb)":
        //                                row.CreateCell(6).SetCellValue(Convert.ToDouble(detail.analyte_value));
        //                                break;
        //                            case "fc(adb)":
        //                                row.CreateCell(7).SetCellValue(Convert.ToDouble(detail.analyte_value));
        //                                break;
        //                            case "ts(adb)":
        //                                row.CreateCell(8).SetCellValue(Convert.ToDouble(detail.analyte_value));
        //                                break;
        //                            case "cv(adb)":
        //                                row.CreateCell(9).SetCellValue(Convert.ToDouble(detail.analyte_value));
        //                                break;
        //                            case "cv(daf)":
        //                                row.CreateCell(10).SetCellValue(Convert.ToDouble(detail.analyte_value));
        //                                break;
        //                            case "cv(arb)":
        //                                row.CreateCell(11).SetCellValue(Convert.ToDouble(detail.analyte_value));
        //                                break;
        //                        }
        //                    }
        //                }

        //                RowCount++;
        //                if (RowCount > 50) break;
        //            }

        //            listIndex++;
        //        }
        //        //****************
        //        workbook.Write(fs);
        //        using (var stream = new FileStream(Path.Combine(FilePath, sFileName), FileMode.Open))
        //        {
        //            await stream.CopyToAsync(memory);
        //        }
        //        memory.Position = 0;
        //        //Throws Generated file to Browser
        //        try
        //        {
        //            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        //        }
        //        // Deletes the generated file from /wwwroot folder
        //        finally
        //        {
        //            var path = Path.Combine(FilePath, sFileName);
        //            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        //        }
        //    }
        //}
    }
}

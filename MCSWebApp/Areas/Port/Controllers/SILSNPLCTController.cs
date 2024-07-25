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

namespace MCSWebApp.Areas.Port.Controllers
{
    [Area("Port")]
    public class SILSNPLCTController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SILSNPLCTController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Port];
            ViewBag.SubAreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SILS];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SILSNPLCT];
            ViewBag.BreadcrumbCode = WebAppMenu.SILSNPLCT;

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "SILSLoadingVessel.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("SilsLoadingVessel");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("Business Unit");
                row.CreateCell(2).SetCellValue("Shipping Order");
                row.CreateCell(3).SetCellValue("Process Flow");
                row.CreateCell(4).SetCellValue("Source Location");
                row.CreateCell(5).SetCellValue("Vessel");
                row.CreateCell(6).SetCellValue("Product Brand");
                row.CreateCell(7).SetCellValue("Draft Survey Number");
                row.CreateCell(8).SetCellValue("Date Arrived");
                row.CreateCell(9).SetCellValue("Date Berthed");
                row.CreateCell(10).SetCellValue("Start Loading");
                row.CreateCell(11).SetCellValue("Finish Loading");
                row.CreateCell(12).SetCellValue("Unberthed Time");
                row.CreateCell(13).SetCellValue("Departed");
                row.CreateCell(14).SetCellValue("Total BW CV2 End");
                row.CreateCell(15).SetCellValue("Tonnage Scale");
                row.CreateCell(16).SetCellValue("Tonnage Draft");
                row.CreateCell(17).SetCellValue("Total on Board");
                row.CreateCell(18).SetCellValue("TS");
                row.CreateCell(19).SetCellValue("Description");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var tabledata = dbContext.sils_nplct
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.date_berthed);
                    // Inserting values to table
                    foreach (var baris in tabledata)
                    {
                        var process_flow_code = "";
                        var process_flow = dbFind.process_flow.Where(o => o.id == baris.process_flow_id).FirstOrDefault();
                        if (process_flow != null) process_flow_code = process_flow.process_flow_code.ToString();

                        var source_location_code = "";
                        var source_location = dbFind.barge.Where(o => o.id == baris.source_location_id).FirstOrDefault();
                        if (source_location != null) source_location_code = source_location.vehicle_id.ToString();

                        var product_code = "";
                        var product = await dbFind.product.Where(o => o.id == baris.product_brand).FirstOrDefaultAsync();
                        if (product != null) product_code = product.product_code.ToString();

                        //var uom_symbol = "";
                        //var uom = dbFind.uom.Where(o => o.id == baris.uom_id).FirstOrDefault();
                        //if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                        var vessel = "";
                        var v = await dbFind.vessel.Where(o => o.id == baris.vessel_id).FirstOrDefaultAsync();
                        if (v != null) vessel = v.vehicle_name.ToString();

                        //var survey_number = "";
                        //var survey = dbFind.survey.Where(o => o.id == baris.survey_id).FirstOrDefault();
                        //if (survey != null) survey_number = survey.survey_number.ToString();

                        var despatch_order_number = "";
                        var despatch_order = dbFind.despatch_order.Where(o => o.id == baris.despatch_order_id).FirstOrDefault();
                        if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                        var business_unit = "";
                        var bu = await dbFind.business_unit.Where(o => o.id == baris.business_unit_id).FirstOrDefaultAsync();
                        if (bu != null) business_unit = bu.business_unit_code.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(baris.transaction_number);
                        row.CreateCell(1).SetCellValue(business_unit);
                        row.CreateCell(2).SetCellValue(despatch_order_number);
                        row.CreateCell(3).SetCellValue(process_flow_code);
                        row.CreateCell(4).SetCellValue(source_location_code);
                        row.CreateCell(5).SetCellValue(vessel);
                        row.CreateCell(6).SetCellValue(product_code);
                        row.CreateCell(7).SetCellValue(baris.draft_survey_number);
                        row.CreateCell(8).SetCellValue(" " + (baris.date_arrived != null ? Convert.ToDateTime(baris.date_arrived).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(9).SetCellValue(" " + (baris.date_berthed != null ? Convert.ToDateTime(baris.date_berthed).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(10).SetCellValue(" " + (baris.start_loading != null ? Convert.ToDateTime(baris.start_loading).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(11).SetCellValue(" " + (baris.finish_loading != null ? Convert.ToDateTime(baris.finish_loading).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(12).SetCellValue(" " + (baris.unberthed_time != null ? Convert.ToDateTime(baris.unberthed_time).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(13).SetCellValue(" " + (baris.departed_time != null ? Convert.ToDateTime(baris.departed_time).ToString("yyyy-MM-dd HH:mm") : ""));
                        row.CreateCell(14).SetCellValue(Convert.ToDouble(baris.bw_end));
                        row.CreateCell(15).SetCellValue(Convert.ToDouble(baris.tonnage_scale));
                        row.CreateCell(16).SetCellValue(Convert.ToDouble(baris.tonnage_draft));
                        row.CreateCell(17).SetCellValue(Convert.ToDouble(baris.total_on_board));
                        row.CreateCell(18).SetCellValue(Convert.ToDouble(baris.supervisor));
                        row.CreateCell(19).SetCellValue(baris.description);

                        RowCount++;
                        if (RowCount > 50) break;
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

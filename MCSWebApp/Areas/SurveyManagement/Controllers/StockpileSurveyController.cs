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
using Microsoft.AspNetCore.Http;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Npoi.Mapper;
using Npoi.Mapper.Attributes;
using NPOI.Util;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.SurveyManagement.Controllers
{
    [Area("SurveyManagement")]
    public class StockpileSurveyController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public StockpileSurveyController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SurveyManagement];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.StockpileSurvey];
            ViewBag.BreadcrumbCode = WebAppMenu.StockpileSurvey;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "StockpileSurvey.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("StockpileSurvey");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Survey Number");
                row.CreateCell(1).SetCellValue("Draft Survey");
                row.CreateCell(2).SetCellValue("Survey Date");
                row.CreateCell(3).SetCellValue("Surveyor");
                row.CreateCell(4).SetCellValue("Stock Location");
                row.CreateCell(5).SetCellValue("Product");
                row.CreateCell(6).SetCellValue("Quantity");
                row.CreateCell(7).SetCellValue("Unit");
                row.CreateCell(8).SetCellValue("Sampling Template");
                row.CreateCell(9).SetCellValue("Accounting Periode");
                row.CreateCell(10).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var business_unit = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var header = dbContext.survey
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.survey_date);
                    // Inserting values to table
                    foreach (var isi in header)
                    {
                        var surveyor_code = "";
                        var surveyor = dbFind.business_partner.Where(o => o.id == isi.surveyor_id).FirstOrDefault();
                        if (surveyor != null) surveyor_code = surveyor.business_partner_code.ToString();

                        var stock_location_name = "";
                        var stock_location = dbFind.stockpile_location.Where(o => o.id == isi.stock_location_id).FirstOrDefault();
                        if (stock_location != null) stock_location_name = stock_location.stockpile_location_code.ToString();

                        var uom_symbol = "";
                        var uom = dbFind.uom.Where(o => o.id == isi.uom_id).FirstOrDefault();
                        if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                        var product_code = "";
                        var product = dbFind.product.Where(o => o.id == isi.product_id).FirstOrDefault();
                        if (product != null) product_code = product.product_code.ToString();

                        var sampling_template_code = "";
                        var sampling_template = dbFind.sampling_template.Where(o => o.id == isi.sampling_template_id).FirstOrDefault();
                        if (sampling_template != null) sampling_template_code = sampling_template.sampling_template_code.ToString();

                        var businessunit = "";
                        var bu = dbFind.business_unit.Where(o => o.id == isi.business_unit_id).FirstOrDefault();
                        if (bu != null) businessunit = bu.business_unit_code.ToString();

                        var accountingPeriod = "";
                        var ap = dbFind.accounting_period.Where(o => o.id == isi.accounting_period_id).FirstOrDefault();
                        if (ap != null) accountingPeriod = ap.accounting_period_name.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(isi.survey_number);
                        row.CreateCell(1).SetCellValue(isi.is_draft_survey.ToString());
                        //row.CreateCell(2).SetCellValue(isi.survey_date.ToString());
                        row.CreateCell(2).SetCellValue(" " + Convert.ToDateTime(isi.survey_date).ToString("yyyy-MM-dd HH:mm"));
                        row.CreateCell(3).SetCellValue(surveyor_code);
                        row.CreateCell(4).SetCellValue(stock_location_name);
                        row.CreateCell(5).SetCellValue(product_code);
                        row.CreateCell(6).SetCellValue(Convert.ToDouble(isi.quantity));
                        row.CreateCell(7).SetCellValue(uom_symbol);
                        row.CreateCell(8).SetCellValue(sampling_template_code);
                        row.CreateCell(9).SetCellValue(accountingPeriod);
                        row.CreateCell(10).SetCellValue(businessunit);

                        RowCount++;
                      //  if (RowCount > 50) break;
                    }
                }
                //***** detail 1
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Survey Analyte");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Survey Number");
                row.CreateCell(1).SetCellValue("Analyte");
                row.CreateCell(2).SetCellValue("Unit");
                row.CreateCell(3).SetCellValue("Value");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.survey_analyte.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var isi in detail)
                {
                    var survey_number = "";
                    var survey = dbFind.survey.Where(o => o.id == isi.survey_id).FirstOrDefault();
                    if (survey != null) survey_number = survey.survey_number.ToString();

                    var analyte_name = "";
                    var analyte = dbFind.analyte.Where(o => o.id == isi.analyte_id).FirstOrDefault();
                    if (analyte != null) analyte_name = analyte.analyte_name.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == isi.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(survey_number);
                    row.CreateCell(1).SetCellValue(analyte_name);
                    row.CreateCell(2).SetCellValue(uom_symbol);
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(isi.analyte_value));

                    RowCount++;
                    if (RowCount > 50) break;
                }
                //****************

                //***** detail 2
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Survey Number");
                row.CreateCell(1).SetCellValue("Product");
                row.CreateCell(2).SetCellValue("Contractor");
                row.CreateCell(3).SetCellValue("Quantity");
                row.CreateCell(4).SetCellValue("Percentage");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail2 = dbContext.survey_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var isi in detail2)
                {
                    var survey_number = "";
                    var survey = dbFind.survey.Where(o => o.id == isi.survey_id).FirstOrDefault();
                    if (survey != null) survey_number = survey.survey_number.ToString();

                    var product_code = "";
                    var product = dbFind.product.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == isi.product_id).FirstOrDefault();
                    if (product != null) product_code = product.product_code.ToString();

                    var contractor_code = "";
                    var contractor = dbFind.contractor.Where(o => o.id == isi.contractor_id).FirstOrDefault();
                    if (contractor != null) contractor_code = contractor.business_partner_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(survey_number);
                    row.CreateCell(1).SetCellValue(product_code);
                    row.CreateCell(2).SetCellValue(contractor_code);
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(isi.quantity));
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(isi.percentage));

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

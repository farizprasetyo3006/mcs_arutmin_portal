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

namespace MCSWebApp.Areas.SurveyManagement.Controllers
{
    [Area("SurveyManagement")]
    public class ShippingDocsController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ShippingDocsController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SurveyManagement];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ShippingDocs];
            ViewBag.BreadcrumbCode = WebAppMenu.ShippingDocs;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "DraftSurvey.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("DraftSurvey");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Survey Number");
                row.CreateCell(1).SetCellValue("Survey Date");
                row.CreateCell(2).SetCellValue("Surveyor");
                row.CreateCell(3).SetCellValue("Stock Location");
                row.CreateCell(4).SetCellValue("Product");
                row.CreateCell(5).SetCellValue("Quantity");
                row.CreateCell(6).SetCellValue("Unit");
                row.CreateCell(7).SetCellValue("Sampling Template");
                row.CreateCell(8).SetCellValue("Shipping Order");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var header = dbContext.draft_survey
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.survey_date);
                // Inserting values to table
                foreach (var isi in header)
                {
                    var surveyor_code = "";
                    var surveyor = dbFind.business_partner.Where(o => o.id == isi.surveyor_id).FirstOrDefault();
                    if (surveyor != null) surveyor_code = surveyor.business_partner_code.ToString();

                    var stock_location_name = "";
                    var stock_location = dbFind.stock_location.Where(o => o.id == isi.stock_location_id).FirstOrDefault();
                    if (stock_location != null) stock_location_name = stock_location.stock_location_name.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == isi.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    var product_code = "";
                    var product = dbFind.product.Where(o => o.id == isi.product_id).FirstOrDefault();
                    if (product != null) product_code = product.product_code.ToString();

                    var sampling_template_code = "";
                    var sampling_template = dbFind.sampling_template.Where(o => o.id == isi.sampling_template_id).FirstOrDefault();
                    if (sampling_template != null) sampling_template_code = sampling_template.sampling_template_code.ToString();

                    var despatch_order_number = "";
                    var despatch_order = dbFind.despatch_order.Where(o => o.id == isi.despatch_order_id).FirstOrDefault();
                    if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(isi.survey_number);
                    row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(isi.survey_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(2).SetCellValue(surveyor_code);
                    row.CreateCell(3).SetCellValue(stock_location_name);
                    row.CreateCell(4).SetCellValue(product_code);
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(isi.quantity));
                    row.CreateCell(6).SetCellValue(uom_symbol);
                    row.CreateCell(7).SetCellValue(sampling_template_code);
                    row.CreateCell(8).SetCellValue(despatch_order_number);

                    RowCount++;
                    if (RowCount > 50) break;
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
                    var draft_survey = dbFind.draft_survey.Where(o => o.id == isi.survey_id).FirstOrDefault();
                    if (draft_survey != null) survey_number = draft_survey.survey_number.ToString();

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
                excelSheet = workbook.CreateSheet("Survey Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Survey Number");
                row.CreateCell(1).SetCellValue("Quantity");
                row.CreateCell(2).SetCellValue("Distance");
                row.CreateCell(3).SetCellValue("Elevation");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                //var detail2 = dbContext.survey_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                //    .OrderByDescending(o => o.created_on);
                //// Inserting values to table
                //foreach (var isi in detail2)
                //{
                //    var survey_number = "";
                //    var draft_survey = dbFind.draft_survey.Where(o => o.id == isi.survey_id).FirstOrDefault();
                //    if (draft_survey != null) survey_number = draft_survey.survey_number.ToString();

                //    row = excelSheet.CreateRow(RowCount);
                //    row.CreateCell(0).SetCellValue(survey_number);
                //    row.CreateCell(1).SetCellValue(Convert.ToDouble(isi.quantity));
                //    row.CreateCell(2).SetCellValue(Convert.ToDouble(isi.distance));
                //    row.CreateCell(3).SetCellValue(Convert.ToDouble(isi.elevation));

                //    RowCount++;
                //    if (RowCount > 50) break;
                //}
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

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

namespace MCSWebApp.Areas.Quality.Controllers
{
    [Area("Quality")]
    public class SamplingTemplateController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SamplingTemplateController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Quality];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SamplingTemplate];
            ViewBag.BreadcrumbCode = WebAppMenu.SamplingTemplate;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "SamplingTemplate.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Sampling Template");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sampling Template Code");
                row.CreateCell(1).SetCellValue("Sampling Template Name");
                row.CreateCell(2).SetCellValue("Is Active");
                row.CreateCell(3).SetCellValue("Is Shipping Order Required?");
                row.CreateCell(4).SetCellValue("Is Stock State");
                row.CreateCell(5).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var header = dbContext.sampling_template
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var baris in header)
                {

                    var business_unit = "";
                    var bu = dbFind.business_unit.Where(o => o.id == baris.business_unit_id).FirstOrDefault();
                    if (bu!= null) business_unit = bu.business_unit_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.sampling_template_code);
                    row.CreateCell(1).SetCellValue(baris.sampling_template_name);
                    row.CreateCell(2).SetCellValue(Convert.ToBoolean(baris.is_active));
                    row.CreateCell(3).SetCellValue(Convert.ToBoolean(baris.is_despatch_order_required));
                    row.CreateCell(4).SetCellValue(Convert.ToBoolean(baris.is_stock_state));
                    row.CreateCell(5).SetCellValue(business_unit);

                    RowCount++;
                    if (RowCount > 50) break;
                }

                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sampling Template");
                row.CreateCell(1).SetCellValue("Analyte");
                row.CreateCell(2).SetCellValue("Unit");
                row.CreateCell(3).SetCellValue("Remark");
                row.CreateCell(4).SetCellValue("Sort Order");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.sampling_template_detail
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var baris in detail)
                {
                    var sampling_template_code = "";
                    var sampling_template = dbFind.sampling_template.Where(o => o.id == baris.sampling_template_id 
                        && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                    if (sampling_template != null) sampling_template_code = sampling_template.sampling_template_code.ToString();

                    var analyte_name = "";
                    var analyte = dbFind.analyte.Where(o => o.id == baris.analyte_id
                        && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                    if (analyte != null) analyte_name = analyte.analyte_name.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == baris.uom_id
                        && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(sampling_template_code);
                    row.CreateCell(1).SetCellValue(analyte_name);
                    row.CreateCell(2).SetCellValue(uom_symbol);
                    row.CreateCell(3).SetCellValue(baris.remark);
                    row.CreateCell(4).SetCellValue(Convert.ToInt32(baris.order));

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

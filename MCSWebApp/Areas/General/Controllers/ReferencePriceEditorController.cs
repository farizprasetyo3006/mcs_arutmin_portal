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
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.General.Controllers
{
    [Area("General")]
    public class ReferencePriceEditorController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ReferencePriceEditorController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ReferencePriceEditor];
            ViewBag.BreadcrumbCode = WebAppMenu.ReferencePriceEditor;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }


        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "Reference_Price_Editor.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Province");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Price Name");
                row.CreateCell(1).SetCellValue("Start Date");
                row.CreateCell(2).SetCellValue("End Date");
                row.CreateCell(3).SetCellValue("Price");
                row.CreateCell(4).SetCellValue("Currency");
                row.CreateCell(5).SetCellValue("Currency Unit");
                row.CreateCell(6).SetCellValue("Calorie");
                row.CreateCell(7).SetCellValue("Calorie Unit");
                row.CreateCell(8).SetCellValue("Total Moisture");
                row.CreateCell(9).SetCellValue("Moisture Unit");
                row.CreateCell(10).SetCellValue("Total Sulphur");
                row.CreateCell(11).SetCellValue("Sulphur Unit");
                row.CreateCell(12).SetCellValue("Ash");
                row.CreateCell(13).SetCellValue("Ash Unit");
                row.CreateCell(14).SetCellValue("Note");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.reference_price_series.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    var currency_code = "";
                    var currrency = dbFind.currency.Where(o => o.id == baris.currency_uom_id).FirstOrDefault();
                    if (currrency != null) currency_code = currrency.currency_code.ToString();

                    var currency_uom = "";
                    var uom = dbFind.uom.Where(o => o.id == baris.currency_uom_id).FirstOrDefault();
                    if (uom != null) currency_uom = uom.uom_symbol.ToString();

                    var calori_uom = "";
                    var calori = dbFind.uom.Where(o => o.id == baris.calori_uom_id).FirstOrDefault();
                    if (calori != null) calori_uom = calori.uom_symbol.ToString();

                    var moisture_uom = "";
                    var moisture = dbFind.uom.Where(o => o.id == baris.total_moisture_uom_id).FirstOrDefault();
                    if (moisture != null) moisture_uom = moisture.uom_symbol.ToString();

                    var sulphur_uom = "";
                    var sulphur = dbFind.uom.Where(o => o.id == baris.total_sulphur_uom_id).FirstOrDefault();
                    if (sulphur != null) sulphur_uom = sulphur.uom_symbol.ToString();

                    var ash_uom = "";
                    var ash = dbFind.uom.Where(o => o.id == baris.ash_uom_id).FirstOrDefault();
                    if (ash != null) ash_uom = ash.uom_symbol.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.price_name);
                    row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(baris.start_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(2).SetCellValue(" " + Convert.ToDateTime(baris.end_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(baris.price));
                    row.CreateCell(4).SetCellValue(currency_code);
                    row.CreateCell(5).SetCellValue(currency_uom);
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(baris.calori));
                    row.CreateCell(7).SetCellValue(calori_uom);
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(baris.total_moisture));
                    row.CreateCell(9).SetCellValue(moisture_uom);
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(baris.total_sulphur));
                    row.CreateCell(11).SetCellValue(sulphur_uom);
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(baris.ash));
                    row.CreateCell(13).SetCellValue(ash_uom);
                    row.CreateCell(14).SetCellValue(baris.notes);
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

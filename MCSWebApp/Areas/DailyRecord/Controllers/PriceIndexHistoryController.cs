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

namespace MCSWebApp.Areas.DailyRecord.Controllers
{
    [Area("DailyRecord")]
    public class PriceIndexHistoryController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public PriceIndexHistoryController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Contractor];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DailyRecord];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.PriceIndexHistory];
            ViewBag.BreadcrumbCode = WebAppMenu.PriceIndexHistory;

            return View();
        }

        public IActionResult Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Contractor];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DailyRecord];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.PriceIndexHistory];
            ViewBag.BreadcrumbCode = WebAppMenu.PriceIndexHistory;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "PriceIndexHistory.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("PriceIndexHistory");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Price Index");
                row.CreateCell(1).SetCellValue("Date");
                row.CreateCell(2).SetCellValue("Index Value");
                row.CreateCell(3).SetCellValue("Currency");
                row.CreateCell(4).SetCellValue("Note");
                row.CreateCell(5).SetCellValue("Is Forecast");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.price_index_history.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.index_date);
                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    var price_index_code = "";
                    var price_index = dbFind.price_index
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == baris.price_index_id)
                        .FirstOrDefault();
                    if (price_index != null) price_index_code = price_index.price_index_code.ToString();

                    var currency_code = "";
                    var currency = dbFind.currency
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .Where(x => x.id == baris.master_list_id)
                        .FirstOrDefault();
                    if (currency != null) currency_code = currency.currency_code;

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(price_index_code);
                    row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(baris.index_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(2).SetCellValue(PublicFunctions.Pecahan(baris.index_value));
                    row.CreateCell(3).SetCellValue(currency_code);
                    row.CreateCell(4).SetCellValue(baris.note);
                    row.CreateCell(5).SetCellValue(PublicFunctions.BenarSalah(baris.is_forecast));

                    RowCount++;
                    if (RowCount > 50) break;
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

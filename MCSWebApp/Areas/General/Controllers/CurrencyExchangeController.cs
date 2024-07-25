using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Entity;
using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NLog;
using Common;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;
using Npoi.Mapper;
using Npoi.Mapper.Attributes;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MCSWebApp.Areas.General.Controllers
{
    [Area("General")]
    public class CurrencyExchangeController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public CurrencyExchangeController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.CurrencyExchange];
            ViewBag.BreadcrumbCode = WebAppMenu.CurrencyExchange;

            return View();
        }

        public async Task<IActionResult> Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.CurrencyExchange];
            ViewBag.BreadcrumbCode = WebAppMenu.CurrencyExchange;

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var svc = new City(CurrentUserContext);
                    var record = await svc.GetByIdAsync(Id);
                    if (record != null)
                    {
                        ViewBag.Id = Id;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); 
            }

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "CurrencyExchange.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("CurrencyExchange");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Source Currency");
                row.CreateCell(1).SetCellValue("Target Currency");
                row.CreateCell(2).SetCellValue("Start Date");
                row.CreateCell(3).SetCellValue("End Date");
                row.CreateCell(4).SetCellValue("Exchange Rate");
                row.CreateCell(5).SetCellValue("Selling Rate");
                row.CreateCell(6).SetCellValue("Buying Rate");
                row.CreateCell(7).SetCellValue("Exchange Type");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.currency_exchange.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var source_currency_code = "";
                    var source_currency = dbFind.currency.Where(o => o.id == Value.source_currency_id).FirstOrDefault();
                    if (source_currency != null) source_currency_code = source_currency.currency_code.ToString();

                    var target_currency_code = "";
                    var target_currency = dbFind.currency.Where(o => o.id == Value.target_currency_id).FirstOrDefault();
                    if (target_currency != null) target_currency_code = target_currency.currency_code.ToString();

                    var exchangeType = "";
                    var ET = dbFind.master_list.Where(o => o.id == Value.exchange_type_id).FirstOrDefault();
                    if (ET != null) exchangeType = ET.item_in_coding.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(source_currency_code);
                    row.CreateCell(1).SetCellValue(target_currency_code);
                    row.CreateCell(2).SetCellValue(" " + Convert.ToDateTime(Value.start_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(3).SetCellValue(" " + Convert.ToDateTime(Value.end_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(Value.exchange_rate));
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(Value.selling_rate));
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(Value.buying_rate));
                    row.CreateCell(7).SetCellValue(exchangeType);

                    RowCount++;
                    if (RowCount == 30) break;
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

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

namespace MCSWebApp.Areas.StockpileManagement.Controllers
{
    [Area("StockpileManagement")]
    public class StockpileStateController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public StockpileStateController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.StockpileManagement];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.StockpileState];
            ViewBag.BreadcrumbCode = WebAppMenu.StockpileState;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "StockpileState.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Stockpile State");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Stockpile");
                row.CreateCell(1).SetCellValue("Transaction Datetime");
                row.CreateCell(2).SetCellValue("Opening");
                row.CreateCell(3).SetCellValue("In");
                row.CreateCell(4).SetCellValue("Out");
                row.CreateCell(5).SetCellValue("Adjustment");
                row.CreateCell(6).SetCellValue("Closing");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.stockpile_state
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.transaction_datetime);
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var stock_location_name = "";
                    var stock_location = dbFind.stock_location.Where(o => o.id == Value.stockpile_location_id).FirstOrDefault();
                    if (stock_location != null) stock_location_name = stock_location.stock_location_name.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(stock_location_name);
                    row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(Value.transaction_datetime).ToString("yyyy-MM-dd HH:mm"));
                    row.CreateCell(2).SetCellValue(Convert.ToInt32(Value.qty_opening));
                    row.CreateCell(3).SetCellValue(Convert.ToInt32(Value.qty_in));
                    row.CreateCell(4).SetCellValue(Convert.ToInt32(Value.qty_out));
                    row.CreateCell(5).SetCellValue(Convert.ToInt32(Value.qty_adjustment));
                    row.CreateCell(6).SetCellValue(Convert.ToInt32(Value.qty_closing));

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

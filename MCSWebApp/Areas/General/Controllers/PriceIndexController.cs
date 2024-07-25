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
using Npoi.Mapper;
using Npoi.Mapper.Attributes;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.EntityFrameworkCore;

namespace MCSWebApp.Areas.General.Controllers
{
    [Area("General")]
    public class PriceIndexController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public PriceIndexController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.PriceIndex];
            ViewBag.BreadcrumbCode = WebAppMenu.PriceIndex;

            return View();
        }

        public async Task<IActionResult> Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.PriceIndex];
            ViewBag.BreadcrumbCode = WebAppMenu.PriceIndex;

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var record = await dbContext.vw_price_index
                    .Where(o => o.id == Id).FirstOrDefaultAsync();

                    if (record != null)
                    {
                        ViewBag.record = record;
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
            string sFileName = "PriceIndex.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("PriceIndex");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Price Index Code");
                row.CreateCell(1).SetCellValue("Price Index Name");
                row.CreateCell(2).SetCellValue("Is Active");
                row.CreateCell(3).SetCellValue("Base Index");
                row.CreateCell(4).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.price_index.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var business_unit_name = "";
                    var business_unit = dbFind.business_unit.Where(o => o.id == Value.business_unit_id).FirstOrDefault();
                    if (business_unit != null) business_unit_name = business_unit.business_unit_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(Value.price_index_code);
                    row.CreateCell(1).SetCellValue(Value.price_index_name);
                    row.CreateCell(2).SetCellValue(Convert.ToBoolean(Value.is_active));
                    row.CreateCell(3).SetCellValue(Convert.ToBoolean(Value.is_base_index));
                    row.CreateCell(4).SetCellValue(business_unit_name);
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

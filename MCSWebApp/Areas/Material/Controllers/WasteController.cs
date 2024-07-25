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
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Material.Controllers
{
    [Area("Material")]
    public class WasteController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public WasteController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Material];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Waste];
            ViewBag.BreadcrumbCode = WebAppMenu.Waste;
            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "Waste.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Waste");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Waste Code");
                row.CreateCell(1).SetCellValue("Waste Category");
                row.CreateCell(2).SetCellValue("Waste Name");
                row.CreateCell(3).SetCellValue("Is Active");
                row.CreateCell(4).SetCellValue("Density");
                row.CreateCell(5).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var business_unit1 = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                IOrderedQueryable<waste> tabledata;
                var selectedIds = ((string)Data.selectedIds)
                    .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                tabledata = dbContext.waste
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                && selectedIds.Contains(o.id))
                .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    var waste_category_code = "";
                    var waste_category = dbFind.waste_category.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == baris.waste_category_id).FirstOrDefault();
                    if (waste_category != null) waste_category_code = waste_category.waste_category_code.ToString();
                    var business_unit = "";
                    var BU = dbFind.business_unit.Where(o=>o.id == baris.business_unit_id).FirstOrDefault();
                    if (BU != null) business_unit = BU.business_unit_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.waste_code);
                    row.CreateCell(1).SetCellValue(waste_category_code);
                    row.CreateCell(2).SetCellValue(baris.waste_name);
                    row.CreateCell(3).SetCellValue(Convert.ToBoolean(baris.is_active));
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(baris.density));
                    row.CreateCell(5).SetCellValue(business_unit);

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

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

namespace MCSWebApp.Areas.Organisation.Controllers
{
    [Area("Shift")]
    public class ShiftController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ShiftController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Shift];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Shift];
            ViewBag.BreadcrumbCode = WebAppMenu.Shift;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "Shift.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Shift");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Shift Category Code");
                row.CreateCell(1).SetCellValue("Shift Name");
                row.CreateCell(2).SetCellValue("Shift Code");
                row.CreateCell(3).SetCellValue("Start Time");
                row.CreateCell(4).SetCellValue("End Time");
                row.CreateCell(5).SetCellValue("Is Active");
                row.CreateCell(6).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.shift.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var shift_category_code = "";
                    var shift_category = dbFind.shift_category.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Value.shift_category_id).FirstOrDefault();
                    if (shift_category != null) shift_category_code = shift_category.shift_category_code == null ? "" : shift_category.shift_category_code.ToString();

                    var business_unit_name = "";
                    var business_unit = dbFind.business_unit.Where(o => o.id == Value.business_unit_id).FirstOrDefault();
                    if (business_unit != null) business_unit_name = business_unit.business_unit_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(shift_category_code);
                    row.CreateCell(1).SetCellValue(Value.shift_name);
                    row.CreateCell(2).SetCellValue(Value.shift_code);
                    row.CreateCell(3).SetCellValue(PublicFunctions.Waktu(Value.start_time).ToString("HH:mm"));
                    row.CreateCell(4).SetCellValue(PublicFunctions.Waktu(Value.end_time).ToString("HH:mm"));
                    row.CreateCell(5).SetCellValue(Convert.ToBoolean(Value.is_active));
                    row.CreateCell(6).SetCellValue(business_unit_name);

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

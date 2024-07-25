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

namespace MCSWebApp.Areas.Delay.Controllers
{
    [Area("Delay")]
    public class DelayController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public DelayController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DailyRecord];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Delay];
            ViewBag.BreadcrumbCode = WebAppMenu.Delay;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "Delay_Event.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Delay_Event");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Date");
                row.CreateCell(1).SetCellValue("Business Area");
                row.CreateCell(2).SetCellValue("Equipment Type");
                row.CreateCell(3).SetCellValue("Equipment");
                row.CreateCell(4).SetCellValue("Duration");
                row.CreateCell(5).SetCellValue("Remark");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.delay
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.delay_date);
                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    var business_area_code = "";
                    var business_area = dbFind.business_area.Where(o => o.id == baris.business_area_id).FirstOrDefault();
                    if (business_area != null) business_area_code = business_area.business_area_code.ToString();
                   
                    var equipment_type_code = "";
                    var equipment_type = dbFind.equipment_type.Where(o => o.id == baris.equipment_type_id).FirstOrDefault();
                    if (equipment_type != null) equipment_type_code = equipment_type.equipment_type_code.ToString();

                    var equipment_code = "";
                    var equipment = dbFind.equipment.Where(o => o.id == baris.equipment_id).FirstOrDefault();
                    if (equipment != null) equipment_code = equipment.equipment_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(" " + Convert.ToDateTime(baris.delay_date).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(1).SetCellValue(business_area_code);
                    row.CreateCell(2).SetCellValue(equipment_type_code);
                    row.CreateCell(3).SetCellValue(equipment_code);
                    row.CreateCell(4).SetCellValue(Convert.ToInt32(baris.duration));
                    row.CreateCell(5).SetCellValue(baris.remark);
                    
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

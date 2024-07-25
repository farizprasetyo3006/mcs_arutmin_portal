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

namespace MCSWebApp.Areas.Equipment.Controllers
{
    [Area("Equipment")]
    public class EquipmentUsageTransactionController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public EquipmentUsageTransactionController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Equipment];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.EquipmentUsageTransaction];
            ViewBag.BreadcrumbCode = WebAppMenu.EquipmentUsageTransaction;

            return View();
        }

        public async Task<IActionResult> Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Equipment];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.EquipmentUsageTransaction];
            ViewBag.BreadcrumbCode = WebAppMenu.EquipmentUsageTransaction;

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var record = await dbContext.vw_equipment_usage_transaction
                     .Where(o => o.id == Id).FirstOrDefaultAsync();

                    if (record != null)
                    {
                        ViewBag.EquipmentUsageTransaction = record;
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
            string sFileName = "EquipmentUsage.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("EquipmentUsage");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Equipment");
                row.CreateCell(1).SetCellValue("Accounting Period");
                row.CreateCell(2).SetCellValue("Hour Usage");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.equipment_usage_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.start_datetime);
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var equipment_code = "";
                    //var equipment = dbFind.equipment.Where(o => o.id == Value.equipment_id).FirstOrDefault();
                    //if (equipment != null) equipment_code = equipment.equipment_code.ToString();

                    var accounting_period_name = "";
                    var accounting_period = dbFind.accounting_period.Where(o => o.id == Value.accounting_period_id).FirstOrDefault();
                    if (accounting_period != null) accounting_period_name = accounting_period.accounting_period_name.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(equipment_code);
                    row.CreateCell(1).SetCellValue(accounting_period_name);
                    //row.CreateCell(2).SetCellValue(PublicFunctions.Pecahan(Value.hour_usage));

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

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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Port.Controllers
{
    [Area("Port")]
    public class SILSUnloadingController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SILSUnloadingController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Port];
            ViewBag.SubAreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SILS];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SILSUnloading];
            ViewBag.BreadcrumbCode = WebAppMenu.SILSUnloading;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "SILSUnloadingBarge.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("SilsUnloadingBarge");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Barge Name");
                row.CreateCell(1).SetCellValue("Voyage Number");
                row.CreateCell(2).SetCellValue("Simultan With");
                row.CreateCell(3).SetCellValue("Mine Of Origin");
                row.CreateCell(4).SetCellValue("Destination");
                row.CreateCell(5).SetCellValue("Date Arrived");
                row.CreateCell(6).SetCellValue("Date Berthed");
                row.CreateCell(7).SetCellValue("Start Loading");
                row.CreateCell(8).SetCellValue("Finish Loading");
                row.CreateCell(9).SetCellValue("Unberthed Time");
                row.CreateCell(10).SetCellValue("Departed");
                row.CreateCell(11).SetCellValue("Business Unit");
                row.CreateCell(12).SetCellValue("Product Name");
                row.CreateCell(13).SetCellValue("Ash");
                row.CreateCell(14).SetCellValue("TS");
                row.CreateCell(15).SetCellValue("GCV");
                row.CreateCell(16).SetCellValue("Site Scale");
                row.CreateCell(17).SetCellValue("Draft Scale");
                row.CreateCell(18).SetCellValue("Belt Scale");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var tabledata = dbContext.sils_unloading
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.finish_loading);
                    // Inserting values to table
                    foreach (var baris in tabledata)
                    {
                        var barge = "";
                        var b = await dbFind.barge.Where(o => o.id == baris.barge_id).FirstOrDefaultAsync();
                        if (b != null) { barge = b.vehicle_id; }

                        var voyage_number = "";
                        var vn = await dbFind.barge_rotation.Where(o => o.id == baris.barge_rotation_id).FirstOrDefaultAsync();
                        if (vn != null) voyage_number = vn.voyage_number.ToString();

                        var simultan_with = "";
                        var sw = await dbFind.barge.Where(o => o.id == baris.simultan_with).FirstOrDefaultAsync();
                        if (sw != null) simultan_with = sw.vehicle_id.ToString();

                        var destination = "";
                        var d = await dbFind.business_unit.Where(o => o.id == baris.barge_destination).FirstOrDefaultAsync();
                        if (d != null) destination = d.business_unit_code.ToString();

                        var business_unit = "";
                        var bu = await dbFind.business_unit.Where(o => o.id == baris.business_unit_id).FirstOrDefaultAsync();
                        if (bu != null) business_unit = bu.business_unit_code.ToString();

                        var product = "";
                        var p = await dbFind.product.Where(o => o.id == baris.product_id).FirstOrDefaultAsync();
                        if (p != null) product = p.product_code.ToString();


                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(barge);
                        row.CreateCell(1).SetCellValue(voyage_number);
                        row.CreateCell(2).SetCellValue(simultan_with);
                        row.CreateCell(3).SetCellValue(baris.mine_of_origin);
                        row.CreateCell(4).SetCellValue(destination);
                        row.CreateCell(5).SetCellValue(" " + Convert.ToDateTime(baris.date_arrived).ToString("yyyy-MM-dd HH:mm"));
                        row.CreateCell(6).SetCellValue(" " + Convert.ToDateTime(baris.date_berthed).ToString("yyyy-MM-dd HH:mm"));
                        row.CreateCell(7).SetCellValue(" " + Convert.ToDateTime(baris.start_loading).ToString("yyyy-MM-dd HH:mm"));
                        row.CreateCell(8).SetCellValue(" " + Convert.ToDateTime(baris.finish_loading).ToString("yyyy-MM-dd HH:mm"));
                        row.CreateCell(9).SetCellValue(" " + Convert.ToDateTime(baris.unberthed_time).ToString("yyyy-MM-dd HH:mm"));
                        row.CreateCell(10).SetCellValue(" " + Convert.ToDateTime(baris.departed_time).ToString("yyyy-MM-dd HH:mm"));
                        row.CreateCell(11).SetCellValue(business_unit);
                        row.CreateCell(12).SetCellValue(product);
                        row.CreateCell(13).SetCellValue(Convert.ToDouble(baris.analyte_1));
                        row.CreateCell(14).SetCellValue(Convert.ToDouble(baris.analyte_2));
                        row.CreateCell(15).SetCellValue(Convert.ToDouble(baris.analyte_3));
                        row.CreateCell(16).SetCellValue(Convert.ToDouble(baris.site_scale));
                        row.CreateCell(17).SetCellValue(Convert.ToDouble(baris.draft_scale));
                        row.CreateCell(18).SetCellValue(Convert.ToDouble(baris.belt_scale));

                        RowCount++;
                        if (RowCount > 50) break;
                    }
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

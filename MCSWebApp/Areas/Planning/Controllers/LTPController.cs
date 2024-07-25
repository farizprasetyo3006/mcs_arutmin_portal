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
using Microsoft.AspNetCore.SignalR;
using NPOI.SS.Formula.Functions;

namespace MCSWebApp.Areas.Planning.Controllers
{
    [Area("Planning")]
    public class LTPController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly IHubContext<ProgressHub> _hubContext;

        public LTPController(IConfiguration Configuration, IHubContext<ProgressHub> hubContext)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
            _hubContext = hubContext;
        }

        //[Route("/Planning/MinePlan/LTP/Index")]
        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;/*
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];*/
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MinePlanLTP];
            ViewBag.BreadcrumbCode = WebAppMenu.MinePlanLTP;

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string operationId = Data;

            string sFileName = "MinePlanLtp.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Mine Plan LTP");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Mine");
                row.CreateCell(1).SetCellValue("Sub Mine");
                row.CreateCell(2).SetCellValue("Pit");
                row.CreateCell(3).SetCellValue("Sub Pit");
                row.CreateCell(4).SetCellValue("Contractor");
                row.CreateCell(5).SetCellValue("SEAM");
                row.CreateCell(6).SetCellValue("Blok");
                row.CreateCell(7).SetCellValue("Material Type");
                row.CreateCell(8).SetCellValue("Reserve Type");
                row.CreateCell(9).SetCellValue("Waste (bcm)");
                row.CreateCell(10).SetCellValue("Coal (Tonnage)");
                row.CreateCell(11).SetCellValue("TM% (ar)");
                row.CreateCell(12).SetCellValue("IM% (adb)");
                row.CreateCell(13).SetCellValue("Ash% (adb)");
                row.CreateCell(14).SetCellValue("VM% (adb)");
                row.CreateCell(15).SetCellValue("FC% (adb)");
                row.CreateCell(16).SetCellValue("TS% (adb)");
                row.CreateCell(17).SetCellValue("CV Kcal/Kg (adb)");
                row.CreateCell(18).SetCellValue("CV Kcal/Kg (arb)");
                row.CreateCell(19).SetCellValue("RD (gr/cc)");
                row.CreateCell(20).SetCellValue("RDI (gr/cc)");
                row.CreateCell(21).SetCellValue("HGI");
                row.CreateCell(22).SetCellValue("Business Unit");
                row.CreateCell(23).SetCellValue("Model Date");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var header = await dbContext.mine_plan_ltp.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on).ToArrayAsync();
                int totalData = header.Count();
                int count = 1;
                var master_list = await dbContext.master_list.ToListAsync();

                var business_unit = await dbContext.business_unit.ToListAsync();
                await _hubContext.Clients.Group(operationId).SendAsync("ReceiveDownloadProgress", count, totalData);

                // Inserting values to table
                foreach (var baris in header)
                {
                    await _hubContext.Clients.Group(operationId).SendAsync("ReceiveDownloadProgress", count, totalData);
                    count++;
                    var material_name = "";
                    var a =  master_list.Where(o => o.id == baris.material_type_id).FirstOrDefault();
                    if (a != null) material_name = a.item_name.ToString();

                    var reverse_type = "";
                    var b =  master_list.Where(o => o.id == baris.reserve_type_id).FirstOrDefault();
                    if (b != null) reverse_type = b.item_name.ToString();

                    var business_units = "";
                    var BU =  business_unit.Where(o => o.id == baris.business_unit_id).FirstOrDefault();
                    if (BU != null) business_units = BU.business_unit_name.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.mine_code != null ? baris.mine_code.ToString() : "");
                    row.CreateCell(1).SetCellValue(baris.submine_code != null ? baris.submine_code.ToString() : "");
                    row.CreateCell(2).SetCellValue(baris.pit_code != null ? baris.pit_code.ToString() : "");
                    row.CreateCell(3).SetCellValue(baris.subpit_code != null ? baris.subpit_code.ToString() : "");
                    row.CreateCell(4).SetCellValue(baris.contractor_code != null ? baris.contractor_code.ToString() : "");
                    row.CreateCell(5).SetCellValue(baris.seam_code != null ? baris.seam_code.ToString() : "");
                    row.CreateCell(6).SetCellValue(baris.blok_code != null ? baris.blok_code.ToString() : "");
                    row.CreateCell(7).SetCellValue(material_name != null ? material_name : "");
                    row.CreateCell(8).SetCellValue(reverse_type != null ? reverse_type : "");
                    row.CreateCell(9).SetCellValue(baris.waste_bcm != null ? Convert.ToDouble(baris.waste_bcm) : 0);
                    row.CreateCell(10).SetCellValue(baris.coal_tonnage != null ? Convert.ToDouble(baris.coal_tonnage) : 0);
                    row.CreateCell(11).SetCellValue(baris.tm_ar != null ? Convert.ToDouble(baris.tm_ar) : 0);
                    row.CreateCell(12).SetCellValue(baris.im_ar != null ? Convert.ToDouble(baris.im_ar) : 0);
                    row.CreateCell(13).SetCellValue(baris.ash_ar != null ? Convert.ToDouble(baris.ash_ar) : 0); 
                    row.CreateCell(14).SetCellValue(baris.vm_ar != null ? Convert.ToDouble(baris.vm_ar) : 0);
                    row.CreateCell(15).SetCellValue(baris.fc_ar != null ? Convert.ToDouble(baris.fc_ar) : 0);
                    row.CreateCell(16).SetCellValue(baris.ts_ar != null ? Convert.ToDouble(baris.ts_ar) : 0);
                    row.CreateCell(17).SetCellValue(baris.gcv_adb_ar != null ? Convert.ToDouble(baris.gcv_adb_ar) : 0);
                    row.CreateCell(18).SetCellValue(baris.gcv_ar_ar != null ? Convert.ToDouble(baris.gcv_ar_ar) : 0);
                    row.CreateCell(19).SetCellValue(baris.rd_ar != null ? Convert.ToDouble(baris.rd_ar) : 0);
                    row.CreateCell(20).SetCellValue(baris.rdi_ar != null ? Convert.ToDouble(baris.rdi_ar) : 0);
                    row.CreateCell(21).SetCellValue(baris.hgi_ar != null ? Convert.ToDouble(baris.hgi_ar) : 0);
                    row.CreateCell(22).SetCellValue(business_units);
                    row.CreateCell(23).SetCellValue(Convert.ToDateTime(baris.model_date).ToString("yyyy-MM-dd HH:mm"));
                     RowCount++;
                  //  if (RowCount > 2) break;
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

                finally
                {
                    await _hubContext.Clients.Group(operationId).SendAsync("ReceiveDownloadProgress", "complete", null);
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }
    }
}

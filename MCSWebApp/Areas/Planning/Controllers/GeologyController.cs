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

namespace MCSWebApp.Areas.Planning.Controllers
{
    [Area("Planning")]
    public class GeologyController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public GeologyController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        //[Route("/Planning/MinePlan/Geology/Index")]
        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MinePlanGeology];
            ViewBag.BreadcrumbCode = WebAppMenu.MinePlanGeology;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "MinePlanGeology.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Mine Plan Geology");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Mine");
                row.CreateCell(1).SetCellValue("Sub Mine");
                row.CreateCell(2).SetCellValue("SEAM");
                row.CreateCell(3).SetCellValue("Truethick");
                row.CreateCell(4).SetCellValue("Mass Tonnage");
                row.CreateCell(5).SetCellValue("TM% (ar)");
                row.CreateCell(6).SetCellValue("IM% (adb)");
                row.CreateCell(7).SetCellValue("Ash% (adb)");
                row.CreateCell(8).SetCellValue("VM% (adb)");
                row.CreateCell(9).SetCellValue("FC% (adb)");
                row.CreateCell(10).SetCellValue("TS% (adb)");
                row.CreateCell(11).SetCellValue("CV Kcal/Kg (adb)");
                row.CreateCell(12).SetCellValue("CV Kcal/Kg (arb)");
                row.CreateCell(13).SetCellValue("RD (gr/cc)");
                row.CreateCell(14).SetCellValue("RDI (gr/cc)");
                row.CreateCell(15).SetCellValue("HGI");
                row.CreateCell(16).SetCellValue("Resource");
                row.CreateCell(17).SetCellValue("Coal Type");
                row.CreateCell(18).SetCellValue("Model Data");
                row.CreateCell(19).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var header = dbContext.mine_plan_geology.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var baris in header)
                {
                    var resource_name = "";
                    var master_list = dbFind.master_list.Where(o => o.id == baris.resource_type_id).FirstOrDefault();
                    if (master_list != null) resource_name = master_list.item_name.ToString();

                    var coal_type = "";
                    var master_list2 = dbFind.master_list.Where(o => o.id == baris.coal_type_id).FirstOrDefault();
                    if (master_list2 != null) coal_type = master_list2.item_name.ToString();

                    var business_unit = "";
                    var BU = dbFind.business_unit.Where(o => o.id == baris.business_unit_id).FirstOrDefault();
                    if (BU != null) business_unit = BU.business_unit_name.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.mine_code != null ? baris.mine_code.ToString() : "");
                    row.CreateCell(1).SetCellValue(baris.submine_code != null ? baris.submine_code.ToString() : "");
                    row.CreateCell(2).SetCellValue(baris.seam_code != null ? baris.seam_code.ToString() : "");
                    row.CreateCell(3).SetCellValue(baris.truethick != null ? Convert.ToDouble(baris.truethick) : 0);
                    row.CreateCell(4).SetCellValue(baris.mass_tonnage != null ? Convert.ToDouble(baris.mass_tonnage) : 0);
                    row.CreateCell(5).SetCellValue(baris.tm_ar != null ? Convert.ToDouble(baris.tm_ar) : 0);
                    row.CreateCell(6).SetCellValue(baris.im_adb != null ? Convert.ToDouble(baris.tm_ar) : 0);
                    row.CreateCell(7).SetCellValue(baris.ash_adb != null ? Convert.ToDouble(baris.ash_adb) : 0);
                    row.CreateCell(8).SetCellValue(baris.vm_adb != null ? Convert.ToDouble(baris.vm_adb) : 0);
                    row.CreateCell(9).SetCellValue(baris.fc_adb != null ? Convert.ToDouble(baris.fc_adb) : 0);
                    row.CreateCell(10).SetCellValue(baris.ts_adb != null ? Convert.ToDouble(baris.ts_adb) : 0);
                    row.CreateCell(11).SetCellValue(baris.cv_adb != null ? Convert.ToDouble(baris.cv_adb) : 0);
                    row.CreateCell(12).SetCellValue(baris.cv_arb != null ? Convert.ToDouble(baris.cv_arb) : 0);
                    row.CreateCell(13).SetCellValue(baris.rd != null ? Convert.ToDouble(baris.rd) : 0);
                    row.CreateCell(14).SetCellValue(baris.rdi != null ? Convert.ToDouble(baris.rdi) : 0);
                    row.CreateCell(15).SetCellValue(baris.hgi != null ? Convert.ToDouble(baris.hgi) : 0);
                    row.CreateCell(16).SetCellValue(resource_name != null ? resource_name : "");
                    row.CreateCell(17).SetCellValue(coal_type != null ? coal_type : "");
                    row.CreateCell(18).SetCellValue(baris.model_data != null ? baris.model_data.ToString() : "");
                    row.CreateCell(19).SetCellValue(business_unit);
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
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }
    }
}

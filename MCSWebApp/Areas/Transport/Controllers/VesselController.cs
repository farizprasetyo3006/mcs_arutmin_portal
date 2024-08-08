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

namespace MCSWebApp.Areas.Transport
{
    [Area("Transport")]
    public class VesselController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public VesselController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Transport];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Vessel];
            ViewBag.BreadcrumbCode = WebAppMenu.Vessel;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "Vessel.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Vessel");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Business Area");
                row.CreateCell(1).SetCellValue("Vessel Name");
                row.CreateCell(2).SetCellValue("Capacity");
                row.CreateCell(3).SetCellValue("Capacity Unit");
                row.CreateCell(4).SetCellValue("IMO Number");
                row.CreateCell(5).SetCellValue("Type");
                row.CreateCell(6).SetCellValue("Flag");
                row.CreateCell(7).SetCellValue("Is Geared");
                row.CreateCell(8).SetCellValue("Is Active");
                row.CreateCell(9).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var business_unit1 = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                IOrderedQueryable<vessel> tabledata;
                var selectedIds = ((string)Data.selectedIds)
                    .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                tabledata = dbContext.vessel
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                && selectedIds.Contains(o.id))
                .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var business_area_name = "";
                    var business_area = dbFind.vw_business_area_structure.Where(o => o.id == Value.business_area_id).FirstOrDefault();
                    if (business_area != null) business_area_name = business_area.business_area_code.ToString();

                    var capacity_unit = "";
                    var uom = dbFind.uom.Where(o => o.id == Value.capacity_uom_id).FirstOrDefault();
                    if (uom != null) capacity_unit = uom.uom_symbol.ToString();

                    var owner_name = "";
                    var contractor = dbFind.contractor.Where(o => o.id == Value.vendor_id).FirstOrDefault();
                    if (contractor != null) owner_name = contractor.business_partner_code.ToString();

                    var business_unit_name = "";
                    var business_unit = dbFind.business_unit.Where(o => o.id == Value.business_unit_id).FirstOrDefault();
                    if (business_unit != null) business_unit_name = business_unit.business_unit_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(business_area_name);
                    row.CreateCell(1).SetCellValue(Value.vehicle_name);
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(Value.capacity));
                    row.CreateCell(3).SetCellValue(capacity_unit);
                    row.CreateCell(4).SetCellValue(Value.imo_number);
                    row.CreateCell(5).SetCellValue(Value.type);
                    row.CreateCell(6).SetCellValue(Value.flag);
                    row.CreateCell(7).SetCellValue(Convert.ToBoolean(Value.is_geared));
                    row.CreateCell(8).SetCellValue(Convert.ToBoolean(Value.is_active));
                    row.CreateCell(9).SetCellValue(business_unit_name);

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

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
    public class TrainController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public TrainController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Transport];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Train];
            ViewBag.BreadcrumbCode = WebAppMenu.Train;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport1()
        {
            string sFileName = "Train.xlsx";
            sFileName = sFileName.Insert(sFileName.LastIndexOf("."), string.Format("_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff")));

            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);
            FilePath = Path.Combine(FilePath, sFileName);

            var mapper = new Npoi.Mapper.Mapper();
            mapper.Ignore<train>(o => o.organization_);
            mapper.Put(dbContext.train, "Train", true);
            mapper.Save(FilePath);

            var memory = new MemoryStream();
            using (var stream = new FileStream(FilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            //Throws Generated file to Browser
            try
            {
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
            }
            // Deletes the generated file
            finally
            {
                var path = Path.Combine(FilePath, sFileName);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "Train.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Train");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Train Name");
                row.CreateCell(1).SetCellValue("Train Id");
                row.CreateCell(2).SetCellValue("Capacity");
                row.CreateCell(3).SetCellValue("Capacity Unit");
                row.CreateCell(4).SetCellValue("Owner");
                row.CreateCell(5).SetCellValue("Make");
                row.CreateCell(6).SetCellValue("Model");
                row.CreateCell(7).SetCellValue("Model Year");
                row.CreateCell(8).SetCellValue("Manufactured Year");
                row.CreateCell(9).SetCellValue("Is Active");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.train.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var capacity_unit = "";
                    var uom = dbFind.uom.Where(o => o.id == Value.capacity_uom_id).FirstOrDefault();
                    if (uom != null) capacity_unit = uom.uom_symbol.ToString();

                    var owner_name = "";
                    var contractor = dbFind.contractor.Where(o => o.id == Value.vendor_id).FirstOrDefault();
                    if (contractor != null) owner_name = contractor.business_partner_name.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(Value.vehicle_name);
                    row.CreateCell(1).SetCellValue(Value.vehicle_id);
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(Value.capacity));
                    row.CreateCell(3).SetCellValue(capacity_unit);
                    row.CreateCell(4).SetCellValue(owner_name);
                    row.CreateCell(5).SetCellValue(Value.vehicle_make);
                    row.CreateCell(6).SetCellValue(Value.vehicle_model);
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(Value.vehicle_model_year));
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(Value.vehicle_manufactured_year));
                    row.CreateCell(9).SetCellValue(Convert.ToBoolean(Value.is_active));

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

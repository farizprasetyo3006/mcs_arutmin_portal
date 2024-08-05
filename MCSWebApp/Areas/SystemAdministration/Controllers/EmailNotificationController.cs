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

namespace MCSWebApp.Areas.SystemAdministration.Controllers
{
    [Area("SystemAdministration")]
    public class EmailNotificationController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public EmailNotificationController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.UserSecurityManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SystemAdministration];
            ViewBag.Breadcrumb = "Email Notification";
            ViewBag.BreadcrumbCode = "Email Notification";

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public IActionResult Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.UserSecurityManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SystemAdministration];
            ViewBag.Breadcrumb = "Email Notification";
            ViewBag.BreadcrumbCode = "Email Notification";

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            if (!string.IsNullOrEmpty(Id)) ViewBag.Id = Id;

            var record = dbContext.vw_email_notification.Where(o => o.id == Id).FirstOrDefault();
            if (record != null)
            {
                ViewBag.email_subject = record.email_subject;
                ViewBag.email_content = record.email_content;
                ViewBag.delivery_schedule = Convert.ToDateTime(record.delivery_schedule).ToString("yyyy-MM-dd HH:mm:ss");
                ViewBag.table_name = record.table_name;
                ViewBag.fields = record.fields;
                ViewBag.criteria = record.criteria;
            }
            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "EmailNotification.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("EmailNotification");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Business Area");
                row.CreateCell(1).SetCellValue("Stockpile Location Code");
                row.CreateCell(2).SetCellValue("Stockpile Location Name");
                row.CreateCell(3).SetCellValue("Product");
                row.CreateCell(4).SetCellValue("Unit");
                row.CreateCell(5).SetCellValue("Current Stock");
                row.CreateCell(6).SetCellValue("Opening Date");
                row.CreateCell(7).SetCellValue("Closing Date");
                row.CreateCell(8).SetCellValue("Quality Sampling");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.stockpile_location.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var business_area_name = "";
                    var business_area = dbFind.vw_business_area_structure.Where(o => o.id == Value.business_area_id).FirstOrDefault();
                    if (business_area != null) business_area_name = business_area.name_path.ToString();

                    var product_name = "";
                    var product = dbFind.product.Where(o => o.id == Value.product_id).FirstOrDefault();
                    if (product != null) product_name = product.product_name.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == Value.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    var sampling_number = "";
                    var quality_sampling = dbFind.quality_sampling.Where(o => o.id == Value.quality_sampling_id).FirstOrDefault();
                    if (quality_sampling != null) sampling_number = quality_sampling.sampling_number.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(business_area_name);
                    row.CreateCell(1).SetCellValue(Value.stockpile_location_code);
                    row.CreateCell(2).SetCellValue(Value.stock_location_name);
                    row.CreateCell(3).SetCellValue(product_name);
                    row.CreateCell(4).SetCellValue(uom_symbol);
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(Value.current_stock));
                    row.CreateCell(6).SetCellValue(" " + Convert.ToDateTime(Value.opening_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(7).SetCellValue(" " + Convert.ToDateTime(Value.closing_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(8).SetCellValue(sampling_number);
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

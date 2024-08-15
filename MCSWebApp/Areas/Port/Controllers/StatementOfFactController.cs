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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.DespatchDemurrage.Controllers
{
    [Area("Port")]
    public class StatementOfFactController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public StatementOfFactController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Port];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.StatementOfFact];
            ViewBag.BreadcrumbCode = WebAppMenu.StatementOfFact;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "LaytimeCalculation.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Laytime Calculation");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Laytime Calculation Number");
                row.CreateCell(1).SetCellValue("DesDem Term");
                row.CreateCell(2).SetCellValue("Shipping Order");
                row.CreateCell(3).SetCellValue("NOR Tendered");
                row.CreateCell(4).SetCellValue("NOR Accepted");
                row.CreateCell(5).SetCellValue("Laytime Commenced");
                row.CreateCell(6).SetCellValue("Commenced Loading");
                row.CreateCell(7).SetCellValue("Completed Loading");
                row.CreateCell(8).SetCellValue("Laytime Completed");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var header = dbContext.sof.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var isi in header)
                {
                    var term_name = "";
                    var despatch_demurrage_detail = dbFind.despatch_demurrage_detail.Where(o => o.id == isi.desdem_term_id).FirstOrDefault();
                    if (despatch_demurrage_detail != null) term_name = despatch_demurrage_detail.term_name.ToString();

                    var despatch_order_number = "";
                    var despatch_order = dbFind.despatch_order.Where(o => o.id == isi.despatch_order_id).FirstOrDefault();
                    if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(isi.sof_number);
                    row.CreateCell(1).SetCellValue(term_name);
                    row.CreateCell(2).SetCellValue(despatch_order_number);
                    row.CreateCell(3).SetCellValue(" " + Convert.ToDateTime(isi.nor_tendered).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(4).SetCellValue(" " + Convert.ToDateTime(isi.nor_accepted).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(5).SetCellValue(" " + Convert.ToDateTime(isi.laytime_commenced).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(6).SetCellValue(" " + Convert.ToDateTime(isi.commenced_loading).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(7).SetCellValue(" " + Convert.ToDateTime(isi.completed_loading).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(8).SetCellValue(" " + Convert.ToDateTime(isi.laytime_completed).ToString("yyyy-MM-dd HH:mm:ss"));

                    RowCount++;
                    if (RowCount > 50) break;
                }

                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("SOF Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Laytime Calculation Number");
                row.CreateCell(1).SetCellValue("Start Date");
                row.CreateCell(2).SetCellValue("End Date");
                row.CreateCell(3).SetCellValue("Code");
                row.CreateCell(4).SetCellValue("Percentage");
                row.CreateCell(5).SetCellValue("Remark");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.vw_sof_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.start_datetime);
                // Inserting values to table
                foreach (var isi in detail)
                {
                    var event_category_code = "";
                    var event_category = dbFind.vw_event_category.Where(o => o.id == isi.event_category_id).FirstOrDefault();
                    if (event_category != null) event_category_code = event_category.event_category_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(isi.sof_number);
                    row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(isi.start_datetime).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(2).SetCellValue(" " + Convert.ToDateTime(isi.end_datetime).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(3).SetCellValue(event_category_code);
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(isi.percentage));
                    row.CreateCell(5).SetCellValue(isi.remark);

                    RowCount++;
                    if (RowCount > 50) break;
                }
                //****************

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

        public async Task<IActionResult> DetailExcelExport(string Id)
        {
            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);
            string sFileName = "StatementOfFactDetail.xlsx";

            FileInfo file = new FileInfo(Path.Combine(FilePath, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(FilePath, sFileName), FileMode.Create, FileAccess.Write))
            {
                int RowCount = 1;
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Laytime Calculation Detail");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Start Date");
                row.CreateCell(1).SetCellValue("End Date");
                row.CreateCell(2).SetCellValue("Code");
                row.CreateCell(3).SetCellValue("Percentage");
                row.CreateCell(4).SetCellValue("Remark");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var header = dbContext.vw_sof_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.sof_id == Id);
                // Inserting values to table
                foreach (var isi in header)
                {
                    //var sof_number = "";
                    //var sof = dbFind.sof.Where(o => o.id == isi.sof_id).FirstOrDefault();
                    //if (sof != null) sof_number = sof.sof_number.ToString();

                    var event_category_code = "";
                    var event_category = dbFind.vw_event_category.Where(o => o.id == isi.event_category_id).FirstOrDefault();
                    if (event_category != null) event_category_code = event_category.event_category_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    //row.CreateCell(0).SetCellValue(sof_number);
                    row.CreateCell(0).SetCellValue(" " + Convert.ToDateTime(isi.start_datetime).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(isi.end_datetime).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(2).SetCellValue(event_category_code);
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(isi.percentage));
                    row.CreateCell(4).SetCellValue(isi.remark);

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

                finally
                {
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }

        public async Task<IActionResult> Detail(String Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Port];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.PortDespatchDemurrage];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.StatementOfFact];
            ViewBag.BreadcrumbCode = WebAppMenu.PortDespatchDemurrage;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var record = await dbContext.vw_sof
                    .Where(o => o.id == Id).FirstOrDefaultAsync();

                    if (record != null)
                    {
                        ViewBag.StatementOfFact = record;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return View();
        }
    }
}

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

namespace MCSWebApp.Areas.DailyRecord.Controllers
{
    [Area("DailyRecord")]
    public class DayWorkController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public DayWorkController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DailyRecord];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Daywork];
            ViewBag.BreadcrumbCode = WebAppMenu.Daywork;

            return View();
        }

        public IActionResult Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DailyRecord];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Daywork];
            ViewBag.BreadcrumbCode = WebAppMenu.Daywork;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "Daywork.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Daywork");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Daywork Number");
                row.CreateCell(1).SetCellValue("Transaction Date");
                row.CreateCell(2).SetCellValue("Customer");
                row.CreateCell(3).SetCellValue("Equipment");
                row.CreateCell(4).SetCellValue("Daywork Type");
                row.CreateCell(5).SetCellValue("Accounting Period");
                row.CreateCell(6).SetCellValue("Reference Number");
                row.CreateCell(7).SetCellValue("HM Start");
                row.CreateCell(8).SetCellValue("HM End");
                row.CreateCell(9).SetCellValue("HM Duration");
                row.CreateCell(10).SetCellValue("Operator NIK");
                row.CreateCell(11).SetCellValue("Supervisor NIK");
                row.CreateCell(12).SetCellValue("Note");
                row.CreateCell(13).SetCellValue("Is Active");
                row.CreateCell(14).SetCellValue("Shift");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.daywork
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.transaction_date);
                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    var business_partner_code = "";
                    //var customer = dbFind.customer.Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                    //    o.id == baris.customer_id).FirstOrDefault();
                    //if (customer != null) business_partner_code = customer.business_partner_code.ToString();

                    var customer = dbFind.customer
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .OrderBy(o => o.business_partner_code)
                                    .Select(o => new { o.id, code = o.business_partner_code });
                    var contractor = dbFind.contractor
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .OrderBy(o => o.business_partner_code)
                                    .Select(o => new { o.id, code = o.business_partner_code });
                    var business_partner = customer.Union(contractor)
                        .Where(o => o.id == baris.customer_id).FirstOrDefault();
                    if (business_partner != null) business_partner_code = business_partner.code.ToString();

                    var equipment_code = "";
                    var equipments = dbFind.equipment
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .Select(o => new { id = o.id, Text = o.equipment_code, Type = o.equipment_name });
                    var trucks = dbFind.truck
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .Select(o => new { id = o.id, Text = o.vehicle_id, Type = o.vehicle_name });
                    var equipment = equipments.Union(trucks).Where(o => o.id == baris.equipment_id).FirstOrDefault();
                    if (equipment != null) equipment_code = equipment.Text.ToString();

                    var accounting_period_name = "";
                    var accounting_period = dbFind.accounting_period.Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                        o.id == baris.accounting_period_id).FirstOrDefault();
                    if (accounting_period != null) accounting_period_name = accounting_period.accounting_period_name.ToString();

                    var employee_operator_number = "";
                    var employee_operator = dbFind.employee.Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.id == baris.operator_id && o.is_operator == true)
                        .Select(o => new { employee_number = o.employee_number + (o.is_active == true ? "" : " ## Not Active") })
                        .FirstOrDefault();
                    if (employee_operator != null) employee_operator_number = employee_operator.employee_number.ToString();

                    var employee_supervisor_number = "";
                    var employee_supervisor = dbFind.employee.Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.id == baris.supervisor_id && o.is_supervisor == true)
                        .Select(o => new { employee_number = o.employee_number + (o.is_active == true ? "" : " ## Not Active") })
                        .FirstOrDefault();
                    if (employee_supervisor != null) employee_supervisor_number = employee_supervisor.employee_number.ToString();

                    var shift_code = "";
                    var shift = dbFind.shift.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.id == baris.shift_id)
                        .FirstOrDefault();
                    if (shift != null) shift_code = shift.shift_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.daywork_number);
                    row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(baris.transaction_date).ToString("yyyy-MM-dd HH:mm"));
                    row.CreateCell(2).SetCellValue(business_partner_code);
                    row.CreateCell(3).SetCellValue(equipment_code);
                    row.CreateCell(4).SetCellValue(baris.daywork_type);
                    row.CreateCell(5).SetCellValue(accounting_period_name);
                    row.CreateCell(6).SetCellValue(baris.reference_number);
                    row.CreateCell(7).SetCellValue(PublicFunctions.Pecahan(baris.hm_start));
                    row.CreateCell(8).SetCellValue(PublicFunctions.Pecahan(baris.hm_end));
                    row.CreateCell(9).SetCellValue(PublicFunctions.Pecahan(baris.hm_duration));
                    row.CreateCell(10).SetCellValue(employee_operator_number);
                    row.CreateCell(11).SetCellValue(employee_supervisor_number);
                    row.CreateCell(12).SetCellValue(baris.note);
                    row.CreateCell(13).SetCellValue(Convert.ToBoolean(baris.is_active));
                    row.CreateCell(14).SetCellValue(shift_code);

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

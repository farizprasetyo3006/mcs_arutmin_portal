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

namespace MCSWebApp.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class RoyaltyController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public RoyaltyController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ContractManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Invoice];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Royalty];
            ViewBag.BreadcrumbCode = WebAppMenu.Royalty;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "Royalty.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Royalty");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sales Order Number");
                row.CreateCell(1).SetCellValue("Sales Plan");
                row.CreateCell(2).SetCellValue("Sales Date");
                row.CreateCell(3).SetCellValue("Accounting Period");
                row.CreateCell(4).SetCellValue("Customer");
                row.CreateCell(5).SetCellValue("Product");
                row.CreateCell(6).SetCellValue("Quantity");
                row.CreateCell(7).SetCellValue("Unit");
                row.CreateCell(8).SetCellValue("Currency");
                row.CreateCell(9).SetCellValue("Reference Number");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var header = dbContext.sales_order.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.sales_date);
                // Inserting values to table
                foreach (var isi in header)
                {
                    var sales_plan_name = "";
                    var sales_plan = dbFind.vw_sales_plan.Where(o => o.id == isi.sales_plan_id).FirstOrDefault();
                    if (sales_plan != null) sales_plan_name = sales_plan.plan_year.ToString();

                    var accounting_period_name = "";
                    var accounting_period = dbFind.accounting_period.Where(o => o.id == isi.accounting_period_id && o.is_closed == false).FirstOrDefault();
                    if (accounting_period != null) accounting_period_name = accounting_period.accounting_period_name.ToString();

                    var customer = "";
                    var business_partner = dbFind.business_partner.Where(o => o.id == isi.business_partner_id && o.is_customer == true).FirstOrDefault();
                    if (business_partner != null) customer = business_partner.business_partner_name.ToString();

                    var product_name = "";
                    var product = dbFind.product.Where(o => o.id == isi.product_id).FirstOrDefault();
                    if (product != null) product_name = product.product_name.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == isi.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    var currency_code = "";
                    var currency = dbFind.currency.Where(o => o.id == isi.currency_id).FirstOrDefault();
                    if (currency != null) currency_code = currency.currency_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(isi.sales_order_number);
                    row.CreateCell(1).SetCellValue(sales_plan_name);
                    row.CreateCell(2).SetCellValue(isi.sales_date.ToString());
                    row.CreateCell(3).SetCellValue(accounting_period_name);
                    row.CreateCell(4).SetCellValue(customer);
                    row.CreateCell(5).SetCellValue(product_name);
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(isi.quantity));
                    row.CreateCell(7).SetCellValue(uom_symbol);
                    row.CreateCell(8).SetCellValue(currency_code);
                    row.CreateCell(9).SetCellValue(isi.reference_number);

                    RowCount++;
                    if (RowCount > 50) break;
                }

                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Sales Order Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sales Order Number");
                row.CreateCell(1).SetCellValue("Analyte");
                row.CreateCell(2).SetCellValue("Minimum");
                row.CreateCell(3).SetCellValue("Maximum");
                row.CreateCell(4).SetCellValue("Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.sales_order_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var isi in detail)
                {
                    var sales_order_number = "";
                    var sales_order = dbFind.sales_order.Where(o => o.id == isi.sales_order_id).FirstOrDefault();
                    if (sales_order != null) sales_order_number = sales_order.sales_order_number.ToString();

                    var analyte_symbol = "";
                    var analyte = dbFind.analyte.Where(o => o.id == isi.analyte_id).FirstOrDefault();
                    if (analyte != null) analyte_symbol = analyte.analyte_symbol.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == isi.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(sales_order_number);
                    row.CreateCell(1).SetCellValue(analyte_symbol);
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(isi.minimum_value));
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(isi.maximum_value));
                    row.CreateCell(4).SetCellValue(uom_symbol);

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

        public async Task<IActionResult> Detail(String Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ContractManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Invoice];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Royalty];
            ViewBag.BreadcrumbCode = WebAppMenu.Royalty;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var record = await dbContext.vw_royalty
                        .Where(o => o.id == Id).FirstOrDefaultAsync();

                    if (record != null)
                    {
                        ViewBag.Royalty = record;

                        var recQQ = await dbContext.vw_royalty_quantity_quality
                            .Where(o => o.id == Id).FirstOrDefaultAsync();
                        if (recQQ != null)
                        {
                            ViewBag.RoyaltyQQ = recQQ;
                        }
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

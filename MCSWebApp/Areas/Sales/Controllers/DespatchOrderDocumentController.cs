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
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class DespatchOrderDocumentController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public DespatchOrderDocumentController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesMarketing];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesContract];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.DespatchOrder];
            ViewBag.BreadcrumbCode = WebAppMenu.DespatchOrder;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "DespatchOrder.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Shipping Order");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Shipping Order");
                row.CreateCell(1).SetCellValue("Shipping Order Date");
                row.CreateCell(2).SetCellValue("Planned Despatch Date");
                row.CreateCell(3).SetCellValue("Sales Contract");
                row.CreateCell(4).SetCellValue("Contract Term");
                row.CreateCell(5).SetCellValue("Shipping Plan");
                row.CreateCell(6).SetCellValue("Seller");
                row.CreateCell(7).SetCellValue("Buyer");
                row.CreateCell(8).SetCellValue("Ship To");
                row.CreateCell(9).SetCellValue("Contract Product");
                row.CreateCell(10).SetCellValue("Required Quantity");
                row.CreateCell(11).SetCellValue("Final Quantity");
                row.CreateCell(12).SetCellValue("Quantity");
                row.CreateCell(13).SetCellValue("Unit");
                row.CreateCell(14).SetCellValue("Despatch Fulfilment Type");
                row.CreateCell(15).SetCellValue("Delivery Term");
                row.CreateCell(16).SetCellValue("Notes");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var header = dbContext.despatch_order.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var isi in header)
                {
                    var sales_contract_name = "";
                    var sales_contract = dbFind.sales_contract.Where(o => o.id == isi.sales_order_id).FirstOrDefault();
                    if (sales_contract != null) sales_contract_name = sales_contract.sales_contract_name.ToString();

                    var contract_term_name = "";
                    var sales_contract_term = dbFind.sales_contract_term.Where(o => o.id == isi.contract_term_id).FirstOrDefault();
                    if (sales_contract_term != null) contract_term_name = sales_contract_term.contract_term_name.ToString();

                    var despatch_plan_name = "";
                    var sales_contract_despatch_plan = dbFind.sales_contract_despatch_plan.Where(o => o.id == isi.despatch_plan_id).FirstOrDefault();
                    if (sales_contract_despatch_plan != null) despatch_plan_name = sales_contract_despatch_plan.despatch_plan_name.ToString();

                    var organization_name = "";
                    var organization = dbFind.organization.Where(o => o.id == isi.organization_id).FirstOrDefault();
                    if (organization != null) organization_name = organization.organization_name.ToString();

                    var buyer_name = "";
                    var customer = dbFind.customer.Where(o => o.id == isi.customer_id).FirstOrDefault();
                    if (customer != null) buyer_name = customer.business_partner_name.ToString();

                    var shipto = "";
                    var customer2 = dbFind.customer.Where(o => o.id == isi.ship_to).FirstOrDefault();
                    if (customer2 != null) shipto = customer2.business_partner_name.ToString();

                    var contract_product_name = "";
                    var contract_product = dbFind.sales_contract_product.Where(o => o.id == isi.contract_product_id).FirstOrDefault();
                    if (contract_product != null) contract_product_name = contract_product.contract_product_name.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.id == isi.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    var item_name = "";
                    var master_list = dbFind.master_list.Where(o => o.id == isi.fulfilment_type_id).FirstOrDefault();
                    if (master_list != null) item_name = master_list.item_name.ToString();

                    var del_term = "";
                    var master_list2 = dbFind.master_list.Where(o => o.id == isi.delivery_term_id).FirstOrDefault();
                    if (master_list2 != null) del_term = master_list2.item_name.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(isi.despatch_order_number);
                    row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(isi.despatch_order_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(2).SetCellValue(" " + Convert.ToDateTime(isi.planned_despatch_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(3).SetCellValue(sales_contract_name);
                    row.CreateCell(4).SetCellValue(contract_term_name);
                    row.CreateCell(5).SetCellValue(despatch_plan_name);
                    row.CreateCell(6).SetCellValue(organization_name);
                    row.CreateCell(7).SetCellValue(buyer_name);
                    row.CreateCell(8).SetCellValue(shipto);
                    row.CreateCell(9).SetCellValue(contract_product_name);
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(isi.required_quantity));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(isi.final_quantity));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(isi.quantity));
                    row.CreateCell(13).SetCellValue(uom_symbol);
                    row.CreateCell(14).SetCellValue(item_name);
                    row.CreateCell(15).SetCellValue(del_term);
                    row.CreateCell(16).SetCellValue(isi.notes);
                    RowCount++;
                }

                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Shipping Order Delay");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Shipping Order Number");
                row.CreateCell(1).SetCellValue("Delay Category");
                row.CreateCell(2).SetCellValue("Demurrage %");
                row.CreateCell(3).SetCellValue("Despatch %");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.despatch_order_delay.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var isi in detail)
                {
                    var despatch_order_number = "";
                    var despatch_order = dbFind.despatch_order.Where(o => o.id == isi.despatch_order_id).FirstOrDefault();
                    if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                    var delay_category_name = "";
                    var delay_category = dbFind.delay_category.Where(o => o.id == isi.delay_category_id).FirstOrDefault();
                    if (delay_category != null) delay_category_name = delay_category.delay_category_name.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(despatch_order_number);
                    row.CreateCell(1).SetCellValue(delay_category_name);
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(isi.demurrage_percent));
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(isi.despatch_percent));
                    RowCount++;
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

    }
}

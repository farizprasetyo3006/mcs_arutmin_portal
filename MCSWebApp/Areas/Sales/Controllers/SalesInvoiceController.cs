using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Entity;
using MCSWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;
using Microsoft.AspNetCore.Http;
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

namespace MCSWebApp.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class SalesInvoiceController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SalesInvoiceController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ContractManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Invoice];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesInvoice];
            ViewBag.BreadcrumbCode = WebAppMenu.SalesInvoice;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "Invoice_PLN.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Invoice PLN");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Shipping Order");
                row.CreateCell(1).SetCellValue("Invoice Number");
                row.CreateCell(2).SetCellValue("Sales Type");
                row.CreateCell(3).SetCellValue("Quotation Type");
                row.CreateCell(4).SetCellValue("Invoice Type");
                row.CreateCell(5).SetCellValue("Invoice Date");
                row.CreateCell(6).SetCellValue("Consignee");
                row.CreateCell(7).SetCellValue("Desrription of Goods");
                row.CreateCell(8).SetCellValue("Advising Bank");
                row.CreateCell(9).SetCellValue("Bill To");
                row.CreateCell(10).SetCellValue("Payment Date");
                row.CreateCell(11).SetCellValue("Quantity");
                row.CreateCell(12).SetCellValue("Base Price");
                row.CreateCell(13).SetCellValue("Harga Dasar Transportasi Bulanan");
                row.CreateCell(14).SetCellValue("CIF PLN Adjusted");
                row.CreateCell(15).SetCellValue("Inco Term");
                row.CreateCell(16).SetCellValue("Freight Invoice No");
                row.CreateCell(17).SetCellValue("Freight Price/ton");
                row.CreateCell(18).SetCellValue("Freight Amount");
                row.CreateCell(19).SetCellValue("Freight Invoice Date Receive");
                row.CreateCell(20).SetCellValue("Freight Contract No");
                row.CreateCell(21).SetCellValue("FOB Price");
                row.CreateCell(22).SetCellValue("Sub Total");
                row.CreateCell(23).SetCellValue("Total Price");
                row.CreateCell(24).SetCellValue("Late Pinalty");
                row.CreateCell(25).SetCellValue("Currency Exchange");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var tabledata = await dbContext.sales_invoice
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.sales_type_id == "682ac4d955ad4028bfb382b3bdcf549a")
                    .OrderBy(o => o.created_on)
                    .ToListAsync();

                var despatch_order = await dbContext.despatch_order.ToListAsync();

                var master_list = await dbContext.master_list.ToListAsync(); 

                var vw_bank_account = await dbContext.vw_bank_account.ToListAsync();

                var customer = await dbContext.customer.ToListAsync();

                var shipping_cost = await dbContext.shipping_cost.ToListAsync();

                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    var SONumber = "";
                    var SO = despatch_order.Where(o => o.id == baris.despatch_order_id).FirstOrDefault();
                    if (SO != null) SONumber = SO.despatch_order_number.ToString();

                    var salesType = "";
                    var ST = master_list.Where(o => o.id == baris.sales_type_id).FirstOrDefault();
                    if (ST != null) salesType = ST.item_name.ToString();

                    var quotationType = "";
                    var QT = master_list.Where(o => o.id == baris.quotation_type_id).FirstOrDefault();
                    if (QT != null) quotationType = QT.item_name.ToString();

                    var invoiceType = "";
                    var split = baris.invoice_type_id.Split("-");
                    var IT = master_list.Where(o => o.id == split[0]).FirstOrDefault();
                    if (IT != null) invoiceType = IT.item_name.ToString();

                    var AdvBank = "";
                    var AB = vw_bank_account.Where(o => o.id == baris.bank_account_id).FirstOrDefault();
                    if (AB != null) AdvBank = AB.bank_code.ToString() + "-" + AB.account_number.ToString();

                    var billTo = "";
                    var CST = customer.Where(o => o.id == baris.bill_to).FirstOrDefault();
                    if (CST != null) billTo = CST.alias_name.ToString();

                    var incoTerm = "";
                    var incotermDat = master_list
                        .Where(o => o.id == baris.inco_term)
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                    if (incotermDat != null) incoTerm = incotermDat.item_name;

                    decimal? penalty = 0;
                    var shippingCostDat = shipping_cost
                        .Where(o => o.despatch_order_id == baris.despatch_order_id)
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                    if (shippingCostDat != null) penalty = shippingCostDat.late_penalty;

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(SONumber);
                    row.CreateCell(1).SetCellValue(baris.invoice_number);
                    row.CreateCell(2).SetCellValue(salesType);
                    row.CreateCell(3).SetCellValue(quotationType);
                    row.CreateCell(4).SetCellValue(invoiceType);
                    row.CreateCell(5).SetCellValue(" " + Convert.ToDateTime(baris.invoice_date).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(6).SetCellValue(baris.consignee);
                    row.CreateCell(7).SetCellValue(baris.description_of_goods);
                    row.CreateCell(8).SetCellValue(AdvBank);
                    row.CreateCell(9).SetCellValue(billTo);
                    row.CreateCell(10).SetCellValue(" " + Convert.ToDateTime(baris.alias4).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(baris.quantity));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(baris.unit_price));
                    row.CreateCell(13).SetCellValue(Convert.ToDouble(baris.alias7));
                    row.CreateCell(14).SetCellValue(Convert.ToDouble(baris.alias8));
                    row.CreateCell(15).SetCellValue(incoTerm);
                    row.CreateCell(16).SetCellValue(baris.szfreightinvoiceno);
                    row.CreateCell(17).SetCellValue(Convert.ToDouble(baris.decfreightpriceton));
                    row.CreateCell(18).SetCellValue(Convert.ToDouble(baris.decfreightamount)); 
                    row.CreateCell(19).SetCellValue(" " + Convert.ToDateTime(baris.dtmfreightinvoicedatereceive).ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(20).SetCellValue(baris.szfreightcontractno); 
                    row.CreateCell(21).SetCellValue(Convert.ToDouble(baris.fob_price)); 
                    row.CreateCell(22).SetCellValue(Convert.ToDouble(baris.subtotal)); 
                    row.CreateCell(23).SetCellValue(Convert.ToDouble(baris.total_price));
                    row.CreateCell(24).SetCellValue(PublicFunctions.Bulat(penalty));
                    row.CreateCell(25).SetCellValue(PublicFunctions.Bulat(baris.notes));
                    RowCount++;
                    if (RowCount > 100) break;
                    
                }
                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Product Specification");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Invoice Number");
                row.CreateCell(1).SetCellValue("Analyte");
                row.CreateCell(2).SetCellValue("Nilai Penyesuaian");
                row.CreateCell(3).SetCellValue("Nilai Denda Penolakan");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;
                List<vw_sales_invoice_product_specification> vw_sales_invoice_product_specification = await dbContext.vw_sales_invoice_product_specification.ToListAsync();
                List<analyte> analyte = await dbContext.analyte.ToListAsync();
                foreach (var data in tabledata)
                {
                    var details = vw_sales_invoice_product_specification
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.sales_invoice_id == data.id)
                        .Where(o => o.non_commercial == false)
                        .ToList();

                    // Inserting values to table
                    foreach (var Value in details)
                    {
                        //var analyte_name = "";
                        //var A = analyte
                        //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        //    .Where(o =>o.id == Value.analyte_id)
                        //    .FirstOrDefault();

                        //if (A != null) analyte_name = A.analyte_symbol.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(Value.invoice_number);
                        row.CreateCell(1).SetCellValue(Value.analyte_name);
                        row.CreateCell(2).SetCellValue(Value.nilai_penyesuaian);
                        row.CreateCell(3).SetCellValue(Convert.ToDouble(Value.nilai_denda_penolakan));
                        RowCount++;
                    }
                }

                //***** detail analyte
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Analyte List");
                row = excelSheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("Analyte Name");
                row.CreateCell(1).SetCellValue("Analyte Symbol");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;
                var detail = analyte
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .ToList();

                foreach (var Value in detail)  // ini cuma nge-list dari master list bang, gausah di loop berdasarkan data yang ada di sales invoice.
                {
                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(Value.analyte_name);
                    row.CreateCell(1).SetCellValue(Value.analyte_symbol);
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

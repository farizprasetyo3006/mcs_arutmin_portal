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
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Planning.Controllers
{
    [Area("Planning")]
    public class ShippingProgram : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ShippingProgram(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ShippingProgram];
            ViewBag.BreadcrumbCode = WebAppMenu.ShippingProgram;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public IActionResult Report()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.SalesPlan];
            ViewBag.BreadcrumbCode = WebAppMenu.SalesPlan;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "ShippingProgram.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Shipping Program");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Shipping Program Number");
                row.CreateCell(1).SetCellValue("Year");
                row.CreateCell(2).SetCellValue("Month");
                row.CreateCell(3).SetCellValue("PLN Declared Month");
                row.CreateCell(4).SetCellValue("Product");
                row.CreateCell(5).SetCellValue("Commitment");
                row.CreateCell(6).SetCellValue("Buyer");
                row.CreateCell(7).SetCellValue("Sales Contract Term");
                row.CreateCell(8).SetCellValue("Transport");
                row.CreateCell(9).SetCellValue("Loading Port");
                row.CreateCell(10).SetCellValue("Quantity");
                row.CreateCell(11).SetCellValue("End Buyer");
                var numb = 12;
                var numb2 = 12;
                var numb3 = 0;
                IOrderedQueryable<shipping_program> header;
                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var product = await dbFind.master_list
                    .Where(x => x.item_group == "shipping-program-product")
                    .OrderBy(x => x.item_in_coding).Take(35)
                    .ToListAsync();
                foreach (var item in product)
                {
                    row.CreateCell(numb).SetCellValue(item.item_name);
                    numb++;
                }
                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    header = dbContext.shipping_program
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => selectedIds.Contains(o.id))
                        .OrderByDescending(o => o.created_on);
                }
                else
                {
                    header = dbContext.shipping_program
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .OrderByDescending(o => o.created_on);
                }
                // Inserting values to table
                foreach (var baris in header)
                {
                    var year = "";
                    var master_list = await dbFind.master_list.Where(o => o.id == baris.plan_year_id).FirstOrDefaultAsync();
                    if (master_list != null) year = master_list.item_name.ToString(); //2

                    var commitment = "";
                    var ml = await dbFind.master_list.Where(o => o.id == baris.commitment_id).FirstOrDefaultAsync();
                    if (ml != null) commitment = ml.item_name.ToString(); //5

                    var product_name = "";
                    var uom = await dbFind.product.Where(o => o.id == baris.product_category_id).FirstOrDefaultAsync();
                    if (uom != null) product_name = uom.product_name.ToString(); //4

                    var customer_name = "";
                    var cust = await dbFind.customer.Where(o => o.id == baris.customer_id).FirstOrDefaultAsync();
                    if (cust != null && cust.alias_name != null) customer_name = cust.alias_name.ToString(); //6

                    var sales_contract = "";
                    var sales = await dbFind.sales_contract_term.Where(o => o.id == baris.sales_contract_id).FirstOrDefaultAsync(); //7
                    if (sales != null) sales_contract = sales.contract_term_name.ToString();

                    var port_1 = "";
                    var port1 = await dbFind.master_list.Where(o => o.id == baris.tipe_penjualan_id).FirstOrDefaultAsync(); //8
                    if (port1 != null) port_1 = port1.item_name.ToString();

                    var port_2 = "";
                    var port2 = await dbFind.port_location.Where(o => o.id == baris.source_coal_id).FirstOrDefaultAsync(); //9
                    if (port2 != null) port_2 = port2.stock_location_name.ToString();

                    var end_buyer = "";
                    var eb = await dbFind.customer.Where(o => o.id == baris.end_user_id).FirstOrDefaultAsync(); //10
                    if (eb != null) end_buyer = eb.business_partner_name.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.shipping_program_number);
                    row.CreateCell(1).SetCellValue(year);
                    row.CreateCell(2).SetCellValue(baris.month_id.ToString());
                    row.CreateCell(3).SetCellValue(baris.declared_month_id);
                    row.CreateCell(4).SetCellValue(product_name);
                    row.CreateCell(5).SetCellValue(commitment);
                    row.CreateCell(6).SetCellValue(customer_name);
                    row.CreateCell(7).SetCellValue(sales_contract);
                    row.CreateCell(8).SetCellValue(port_1);
                    row.CreateCell(9).SetCellValue(port_2);
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(baris.quantity));
                    row.CreateCell(11).SetCellValue(end_buyer);
                    decimal?[] data = { baris.product_1, baris.product_2, baris.product_3, baris.product_4, baris.product_5, baris.product_6, baris.product_7, baris.product_8, baris.product_9, baris.product_10
                    , baris.product_11, baris.product_12, baris.product_13, baris.product_14, baris.product_15, baris.product_16, baris.product_17, baris.product_18, baris.product_19, baris.product_20
                    , baris.product_21, baris.product_22, baris.product_23, baris.product_24, baris.product_25, baris.product_26, baris.product_27, baris.product_28, baris.product_29, baris.product_30, baris.product_31
                    , baris.product_32, baris.product_33, baris.product_34, baris.product_35};
                    foreach (var item in product)
                    {
                        row.CreateCell(numb2).SetCellValue(Convert.ToDouble(data[numb3] ?? 0));
                        numb2++; numb3++;
                    }
                    numb3 = 0;
                    numb2 = 12;
                    RowCount++;
                  //  if (RowCount > 50) break;
                }

                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Detail Spec");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Shipping Program Number");
                row.CreateCell(1).SetCellValue("Product Category");
                row.CreateCell(2).SetCellValue("Product");
                row.CreateCell(3).SetCellValue("Contractor");
                row.CreateCell(4).SetCellValue("Quantity");
                mcsContext dbFind2 = new mcsContext(DbOptionBuilder.Options);

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;
                foreach (var data in header)
                {
                    var detail = await dbFind.shipping_program_detail.Where(o=>o.shipping_program_id == data.id).ToArrayAsync();
                    foreach(var item in detail)
                    {
                        var productCategory = "";
                        var pc = await dbFind2.product_category.Where(o => o.id == item.product_category_id).FirstOrDefaultAsync();
                        if (pc != null) productCategory = pc.product_category_code.ToString();

                        var product_ = "";
                        var p = await dbFind2.product.Where(o => o.id == item.product_id).FirstOrDefaultAsync();
                        if(p!= null) product_ = p.product_code.ToString();

                        var contractor = "";
                        var c = await dbFind2.contractor.Where(o => o.id == item.contractor_id).FirstOrDefaultAsync();
                        if(c!=null) contractor = c.business_partner_code.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(data.shipping_program_number);
                        row.CreateCell(1).SetCellValue(productCategory);
                        row.CreateCell(2).SetCellValue(product_);
                        row.CreateCell(3).SetCellValue(contractor);
                        row.CreateCell(4).SetCellValue(Convert.ToDouble(item.quantity));
                        RowCount++;

                    }
                }


                workbook.Write(fs);
            }
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


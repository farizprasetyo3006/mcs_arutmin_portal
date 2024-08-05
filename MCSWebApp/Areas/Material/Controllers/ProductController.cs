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
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Material.Controllers
{
    [Area("Material")]
    public class ProductController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ProductController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Material];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Product];
            ViewBag.BreadcrumbCode = WebAppMenu.Product;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MasterData];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Material];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Product];
            ViewBag.BreadcrumbCode = WebAppMenu.Product;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var svc = new BusinessLogic.Entity.Product(CurrentUserContext);
                    var record = await svc.GetByIdAsync(Id);
                    if (record != null)
                    {
                        ViewBag.Id = Id;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "Product.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Product");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Product Code");
                row.CreateCell(1).SetCellValue("Prod. Category Code");
                row.CreateCell(2).SetCellValue("Product Name");
                row.CreateCell(3).SetCellValue("Account Code");
                row.CreateCell(4).SetCellValue("Is Active");
                row.CreateCell(5).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var business_unit1 = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                IOrderedQueryable<product> tabledata;
                var selectedIds = ((string)Data.selectedIds)
                    .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                tabledata = dbContext.product
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                && selectedIds.Contains(o.id))
                .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var baris in tabledata)
                {
                    var product_category_code = "";
                    var product_category = dbFind.product_category.Where(o => o.id == baris.product_category_id).FirstOrDefault();
                    if (product_category != null) product_category_code = product_category.product_category_code.ToString();

                    var account_code = "";
                    var coa = dbFind.coa.Where(o => o.id == baris.coa_id).FirstOrDefault();
                    if (coa != null) account_code = coa.account_code.ToString();

                    var business_unit = "";
                    var BU = dbFind.business_unit.Where(o => o.id == baris.business_unit_id).FirstOrDefault();
                    if (BU != null) business_unit = BU.business_unit_code.ToString();
                    
                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.product_code);
                    row.CreateCell(1).SetCellValue(product_category_code);
                    row.CreateCell(2).SetCellValue(baris.product_name);
                    row.CreateCell(3).SetCellValue(account_code);
                    row.CreateCell(4).SetCellValue(Convert.ToBoolean(baris.is_active));
                    row.CreateCell(5).SetCellValue(business_unit);

                    RowCount++;
                    if (RowCount > 50) break;
                }

                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Detail");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Product");
                row.CreateCell(1).SetCellValue("Analyte");
                row.CreateCell(2).SetCellValue("Unit");
                row.CreateCell(3).SetCellValue("Minimum Value");
                row.CreateCell(4).SetCellValue("Target Value");
                row.CreateCell(5).SetCellValue("Maximum Value");
                row.CreateCell(6).SetCellValue("Applicable Date");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.product_specification.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var Value in detail)
                {
                    var product_code = "";
                    var product = dbFind.product.Where(o => o.id == Value.product_id
                        && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                    if (product != null) product_code = product.product_code.ToString();

                    var analyte_name = "";
                    var analyte = dbFind.analyte.Where(o => o.id == Value.analyte_id
                        && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                    if (analyte != null) analyte_name = analyte.analyte_name.ToString();

                    var unit = "";
                    var uom = dbFind.uom.Where(o => o.id == Value.uom_id
                        && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                    if (uom != null) unit = uom.uom_symbol.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(product_code);
                    row.CreateCell(1).SetCellValue(analyte_name);
                    row.CreateCell(2).SetCellValue(unit);
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(Value.minimum_value));
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(Value.target_value));
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(Value.maximum_value));
                    row.CreateCell(6).SetCellValue(" " + Convert.ToDateTime(Value.applicable_date).ToString("yyyy-MM-dd"));
                    
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

    }
}

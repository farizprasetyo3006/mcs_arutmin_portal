﻿using System;
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
using Microsoft.AspNetCore.Http;
using Npoi.Mapper;
using Npoi.Mapper.Attributes;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MCSWebApp.Areas.Location.Controllers
{
    [Area("Location")]
    public class StockpileLocationController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public StockpileLocationController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Modelling];
            ViewBag.SubAreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Location];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.StockpileLocation];
            ViewBag.BreadcrumbCode = WebAppMenu.StockpileLocation;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "StockpileLocation.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("StockpileLocation");
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

                var business_unit = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                var tabledata = dbContext.stockpile_location.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                (business_unit != null && business_unit != "" ? o.business_unit_id == business_unit : true))
                    .OrderByDescending(o => o.opening_date);
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var business_area_code = "";
                    var business_area = dbFind.business_area
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                            && o.id == Value.business_area_id).FirstOrDefault();
                    if (business_area != null) business_area_code = business_area.business_area_code.ToString();

                    var product_code = "";
                    var product = dbFind.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.id == Value.product_id).FirstOrDefault();
                    if (product != null) product_code = product.product_code.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.id == Value.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    var sampling_number = "";
                    var quality_sampling = dbFind.quality_sampling
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.id == Value.quality_sampling_id).FirstOrDefault();
                    if (quality_sampling != null) sampling_number = quality_sampling.sampling_number.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(business_area_code);
                    row.CreateCell(1).SetCellValue(Value.stockpile_location_code);
                    row.CreateCell(2).SetCellValue(Value.stock_location_name);
                    row.CreateCell(3).SetCellValue(product_code);
                    row.CreateCell(4).SetCellValue(uom_symbol);
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(Value.current_stock));
                    row.CreateCell(6).SetCellValue(" " + Convert.ToDateTime(Value.opening_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(7).SetCellValue(" " + Convert.ToDateTime(Value.closing_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(8).SetCellValue(sampling_number);

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

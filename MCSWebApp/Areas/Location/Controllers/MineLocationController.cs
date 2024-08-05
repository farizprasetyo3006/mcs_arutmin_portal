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
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace MCSWebApp.Areas.Location.Controllers
{
    [Area("Location")]
    public class MineLocationController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public MineLocationController(IConfiguration Configuration)
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
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MineLocation];
            ViewBag.BreadcrumbCode = WebAppMenu.MineLocation;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public IActionResult Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Modelling];
            ViewBag.SubAreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Location];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.MineLocation];
            ViewBag.BreadcrumbCode = WebAppMenu.MineLocation;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "MineLocation.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Mine Location");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Business Area Code");
                row.CreateCell(1).SetCellValue("Mine Location Code");
                row.CreateCell(2).SetCellValue("Mine Location Name");
                row.CreateCell(3).SetCellValue("Product");
                row.CreateCell(4).SetCellValue("Unit");
                row.CreateCell(5).SetCellValue("Opening Date");
                row.CreateCell(6).SetCellValue("Closing Date");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var business_unit = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                var tabledata = await dbContext.mine_location.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                (business_unit != null && business_unit != "" ? o.business_unit_id == business_unit : true))
                    .OrderByDescending(o => o.opening_date).ToListAsync();
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var business_area_code = "";
                    var business_area = dbFind.business_area.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Value.business_area_id).FirstOrDefault();
                    if (business_area != null) business_area_code = business_area.business_area_code.ToString();

                    var product_code = "";
                    var product = dbFind.product.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Value.product_id).FirstOrDefault();
                    if (product != null) product_code = product.product_code.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Value.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(business_area_code);
                    row.CreateCell(1).SetCellValue(Value.mine_location_code);
                    row.CreateCell(2).SetCellValue(Value.stock_location_name);
                    row.CreateCell(3).SetCellValue(product_code);
                    row.CreateCell(4).SetCellValue(uom_symbol);
                    row.CreateCell(5).SetCellValue(" " + Convert.ToDateTime(Value.opening_date).ToString("yyyy-MM-dd"));
                    row.CreateCell(6).SetCellValue(" " + Convert.ToDateTime(Value.closing_date).ToString("yyyy-MM-dd"));

                    RowCount++;
                    if (RowCount > 50) break;
                }

                //***** detail
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Exposed Coal");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Mine Location Code");
                row.CreateCell(1).SetCellValue("Transaction Date");
                row.CreateCell(2).SetCellValue("Quantity");
                row.CreateCell(3).SetCellValue("Unit");
                //row.CreateCell(4).SetCellValue("Survey");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail = dbContext.exposed_coal.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.transaction_date);
                // Inserting values to table
                foreach (var Value in detail)
                {
                    var mine_location_code = "";
                    var mine_location = dbFind.mine_location.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Value.mine_location_id).FirstOrDefault();
                    if (mine_location != null) mine_location_code = mine_location.mine_location_code.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Value.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    //var survey_number = "";
                    //var survey = dbFind.survey.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    //    && o.id == Value.survey_id).FirstOrDefault();
                    //if (survey != null) survey_number = survey.survey_number.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(mine_location_code);
                    row.CreateCell(1).SetCellValue(Value.transaction_date.ToString("yyyy-MM-dd"));
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(Value.quantity));
                    row.CreateCell(3).SetCellValue(uom_symbol);
                    //row.CreateCell(4).SetCellValue(survey_number);
                    RowCount++;
                    if (RowCount > 50) break;
                }

                //***** Ready to Get
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Ready to Get");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Mine Location Code");
                row.CreateCell(1).SetCellValue("Transaction Date");
                row.CreateCell(2).SetCellValue("Quantity");
                row.CreateCell(3).SetCellValue("Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var ready_to_get = dbContext.ready_to_get.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.transaction_date);
                // Inserting values to table
                foreach (var Value in ready_to_get)
                {
                    var mine_location_code = "";
                    var mine_location = dbFind.mine_location.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Value.mine_location_id).FirstOrDefault();
                    if (mine_location != null) mine_location_code = mine_location.mine_location_code.ToString();

                    var uom_symbol = "";
                    var uom = dbFind.uom.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Value.uom_id).FirstOrDefault();
                    if (uom != null) uom_symbol = uom.uom_symbol.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(mine_location_code);
                    row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(Value.transaction_date).ToString("yyyy-MM-dd HH:mm"));
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(Value.quantity));
                    row.CreateCell(3).SetCellValue(uom_symbol);
                    RowCount++;
                    if (RowCount > 50) break;
                }

                //***** Model Geology
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Model Geology");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Mine Location Code");
                row.CreateCell(1).SetCellValue("Month");
                row.CreateCell(2).SetCellValue("Year");
                row.CreateCell(3).SetCellValue("Quantity");
                row.CreateCell(4).SetCellValue("TM");
                row.CreateCell(5).SetCellValue("TS");
                row.CreateCell(6).SetCellValue("Ash");
                row.CreateCell(7).SetCellValue("IM");
                row.CreateCell(8).SetCellValue("VM");
                row.CreateCell(9).SetCellValue("FC");
                row.CreateCell(10).SetCellValue("GCV (ar)");
                row.CreateCell(11).SetCellValue("GCV (adb)");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var model_geology = dbContext.model_geology.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var Value in model_geology)
                {
                    var mine_location_code = "";
                    var mine_location = dbFind.mine_location.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Value.mine_location_id).FirstOrDefault();
                    if (mine_location != null) mine_location_code = mine_location.mine_location_code.ToString();

                    var year_number = "";
                    var years = dbFind.master_list.Where(o => o.id == Value.year_id).FirstOrDefault();
                    if (years != null) year_number = years.item_in_coding.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(mine_location_code);
                    row.CreateCell(1).SetCellValue(Value.month_id);
                    row.CreateCell(2).SetCellValue(year_number);
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(Value.quantity));
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(Value.tm));
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(Value.ts));
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(Value.ash));
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(Value.im));
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(Value.vm));
                    row.CreateCell(9).SetCellValue(Convert.ToDouble(Value.fc));
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(Value.gcv_ar));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(Value.gcv_adb));

                    RowCount++;
                    if (RowCount > 50) break;
                }
                //****************

                ////***** Mine Location Quality (Channel Sampling)
                //RowCount = 1;
                //excelSheet = workbook.CreateSheet("Channel Sampling");
                //row = excelSheet.CreateRow(0);
                //// Setting Cell Heading
                //row.CreateCell(0).SetCellValue("Mine Location Code");
                //row.CreateCell(1).SetCellValue("Date");
                //row.CreateCell(2).SetCellValue("TM");
                //row.CreateCell(3).SetCellValue("TS");
                //row.CreateCell(4).SetCellValue("Ash");
                //row.CreateCell(5).SetCellValue("IM");
                //row.CreateCell(6).SetCellValue("VM");
                //row.CreateCell(7).SetCellValue("FC");
                //row.CreateCell(8).SetCellValue("GCV (ar)");
                //row.CreateCell(9).SetCellValue("GCV (adb)");

                //excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                //var mine_location_quality = dbContext.mine_location_quality.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                //    .OrderByDescending(o => o.created_on);
                //// Inserting values to table
                //foreach (var Value in mine_location_quality)
                //{
                //    var mine_location_code = "";
                //    var mine_location = dbFind.mine_location.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                //        && o.id == Value.mine_location_id).FirstOrDefault();
                //    if (mine_location != null) mine_location_code = mine_location.mine_location_code.ToString();

                //    row = excelSheet.CreateRow(RowCount);
                //    row.CreateCell(0).SetCellValue(mine_location_code);
                //    row.CreateCell(1).SetCellValue(Convert.ToDateTime(Value.sampling_datetime).ToString("yyyy-MM-dd"));
                //    row.CreateCell(2).SetCellValue(Convert.ToDouble(Value.tm));
                //    row.CreateCell(3).SetCellValue(Convert.ToDouble(Value.ts));
                //    row.CreateCell(4).SetCellValue(Convert.ToDouble(Value.ash));
                //    row.CreateCell(5).SetCellValue(Convert.ToDouble(Value.im));
                //    row.CreateCell(6).SetCellValue(Convert.ToDouble(Value.vm));
                //    row.CreateCell(7).SetCellValue(Convert.ToDouble(Value.fc));
                //    row.CreateCell(8).SetCellValue(Convert.ToDouble(Value.gcv_ar));
                //    row.CreateCell(9).SetCellValue(Convert.ToDouble(Value.gcv_adb));
                //    RowCount++;
                //    if (RowCount > 50) break;
                //}
                ////****************

                //***** Quality Pit
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Quality Pit");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Mine Location Code");
                row.CreateCell(1).SetCellValue("Month");
                row.CreateCell(2).SetCellValue("Year");
                row.CreateCell(3).SetCellValue("Quantity");
                row.CreateCell(4).SetCellValue("TM");
                row.CreateCell(5).SetCellValue("TS");
                row.CreateCell(6).SetCellValue("Ash");
                row.CreateCell(7).SetCellValue("IM");
                row.CreateCell(8).SetCellValue("VM");
                row.CreateCell(9).SetCellValue("FC");
                row.CreateCell(10).SetCellValue("GCV (ar)");
                row.CreateCell(11).SetCellValue("GCV (adb)");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var mine_location_quality_pit = dbContext.mine_location_quality_pit
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var Value in mine_location_quality_pit)
                {
                    var mine_location_code = "";
                    var mine_location = dbFind.mine_location.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Value.mine_location_id).FirstOrDefault();
                    if (mine_location != null) mine_location_code = mine_location.mine_location_code.ToString();

                    var year_number = "";
                    var years = dbFind.master_list.Where(o => o.id == Value.year_id).FirstOrDefault();
                    if (years != null) year_number = years.item_in_coding.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(mine_location_code);
                    row.CreateCell(1).SetCellValue(Value.month_id);
                    row.CreateCell(2).SetCellValue(year_number);
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(Value.quantity));
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(Value.tm));
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(Value.ts));
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(Value.ash));
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(Value.im));
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(Value.vm));
                    row.CreateCell(9).SetCellValue(Convert.ToDouble(Value.fc));
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(Value.gcv_ar));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(Value.gcv_adb));

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
                // Deletes the generated file from /wwwroot folder
                finally
                {
                    var path = Path.Combine(FilePath, sFileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
        }

        public async Task<IActionResult> ExcelExportExposedCoal()
        {
            string sFileName = "ExposedCoal.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Exposed Coal");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Sub Pit");
                row.CreateCell(1).SetCellValue("Conctractor");
                row.CreateCell(2).SetCellValue("Transaction Date");
                row.CreateCell(3).SetCellValue("Quantity");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var business_unit = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                
                var detaildata = await dbContext.exposed_coal.Where(o => o.organization_id == CurrentUserContext.OrganizationId )
                .OrderByDescending(o => o.transaction_date).ToListAsync();
                
                // Inserting values to table
                foreach (var item in detaildata)
                {
                    var tabledata = await dbContext.mine_location
                                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    (business_unit != null && business_unit != "" ? o.business_unit_id == business_unit : true))
                                        .Where(o=>o.id == item.mine_location_id)
                                    .OrderByDescending(o => o.opening_date).ToListAsync();
                    foreach (var Value in tabledata)
                    {
                        var child4 = "";
                        var business_area = dbFind.vw_business_area_breakdown_structure.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.id == Value.business_area_id).FirstOrDefault();
                        if (business_area != null) child4 = business_area.child_4.ToString();

                        var contractor_code = "";
                        var contractor = dbFind.contractor.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.id == Value.contractor_id).FirstOrDefault();
                        if (contractor != null) contractor_code = contractor.business_partner_code.ToString();


                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(child4);
                        row.CreateCell(1).SetCellValue(contractor_code);
                        row.CreateCell(2).SetCellValue(item.transaction_date);
                        var cell = row.CreateCell(2);
                        cell.SetCellValue(item.transaction_date); // Handle null values
                        var CellStyle = workbook.CreateCellStyle();
                        CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("m/d/yyyy");
                        cell.CellStyle = CellStyle;
                        row.CreateCell(3).SetCellValue(Convert.ToDouble(item.quantity));

                        RowCount++;
                    }
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

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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Mining.Controllers
{
    [Area("Mining")]
    public class ProcessingClosingController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ProcessingClosingController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ReconcileNumber];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.CoalProduceClosing];
            ViewBag.BreadcrumbCode = WebAppMenu.CoalProduceClosing;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public IActionResult Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ReconcileNumber];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.CoalProduceClosing];
            ViewBag.BreadcrumbCode = WebAppMenu.CoalProduceClosing;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "Coal Produce Closing.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Coal Produce Closing");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("Transaction Date");
                row.CreateCell(2).SetCellValue("Accounting Period");
                row.CreateCell(3).SetCellValue("Advance Contract");
                row.CreateCell(4).SetCellValue("From Date");
                row.CreateCell(5).SetCellValue("To Date");
                row.CreateCell(6).SetCellValue("Volume");
                row.CreateCell(7).SetCellValue("Distance");
                row.CreateCell(8).SetCellValue("Business Unit");
                row.CreateCell(9).SetCellValue("Note");
                row.CreateCell(10).SetCellValue("Item Number");
                row.CreateCell(11).SetCellValue("Loading Date");
                row.CreateCell(12).SetCellValue("Mine Location Code");
                row.CreateCell(13).SetCellValue("Business Area Code");
                row.CreateCell(14).SetCellValue("Quantity");
                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var dataTable = await dbContext.processing_closing
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .ToListAsync();
                // Inserting values to table
                foreach (var data in dataTable)
                {
                    var accounting_period_name = "";
                    var accounting_period = await dbContext.accounting_period
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == data.accounting_period_id).FirstOrDefaultAsync();
                    if (accounting_period != null)
                        accounting_period_name = accounting_period.accounting_period_name;

                    var advance_contract_number = "";
                    var advance_contract = await dbContext.advance_contract
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == data.advance_contract_id).FirstOrDefaultAsync();
                    if (advance_contract != null)
                        advance_contract_number = advance_contract.advance_contract_number;

                    var business_unit_code = string.Empty;
                    var business_unit = await dbContext.business_unit
                        .Where(x => x.id == data.business_unit_id).FirstOrDefaultAsync();
                    if (business_unit != null)
                        business_unit_code = business_unit.business_unit_code;

                    var itemData = await dbContext.processing_closing_item
                        .Where(x => x.processing_closing_id == data.id).ToListAsync();
                    int iCount = 1;
                    if (itemData == null || itemData.Count < 1)
                    {
                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(data.transaction_number);
                        row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(data.transaction_date).ToString("yyyy-MM-dd"));
                        row.CreateCell(2).SetCellValue(accounting_period_name);
                        row.CreateCell(3).SetCellValue(advance_contract_number);
                        row.CreateCell(4).SetCellValue(" " + Convert.ToDateTime(data.from_date).ToString("yyyy-MM-dd"));
                        row.CreateCell(5).SetCellValue(" " + Convert.ToDateTime(data.to_date).ToString("yyyy-MM-dd"));
                        row.CreateCell(6).SetCellValue(PublicFunctions.Pecahan(data.volume));
                        row.CreateCell(7).SetCellValue(PublicFunctions.Pecahan(data.distance));
                        row.CreateCell(8).SetCellValue(business_unit_code);
                        row.CreateCell(9).SetCellValue(data.note);
                        row.CreateCell(10).SetCellValue("-");
                        row.CreateCell(11).SetCellValue("");
                        row.CreateCell(12).SetCellValue("");
                        row.CreateCell(13).SetCellValue("");
                        row.CreateCell(14).SetCellValue("");
                        RowCount++;
                        continue;
                    }
                    foreach (var item in itemData)
                    {
                        var mine_location_code = string.Empty;
                        var mine_location = await dbContext.mine_location
                            .Where(x => x.id == item.mine_location_id)
                            .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            .FirstOrDefaultAsync();
                        if (mine_location != null)
                            mine_location_code = mine_location.mine_location_code;

                        var business_area_code = string.Empty;
                        business_area business_area = new business_area();
                        if (mine_location != null)
                            business_area = await dbContext.business_area
                            .Where(x => x.id == mine_location.business_area_id)
                            .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            .FirstOrDefaultAsync();
                        else
                            business_area = await dbContext.business_area
                            .Where(x => x.id == item.business_area_id)
                            .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            .FirstOrDefaultAsync();
                        if (business_area != null)
                            business_area_code = business_area.business_area_code;

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(data.transaction_number);
                        row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(data.transaction_date).ToString("yyyy-MM-dd"));
                        row.CreateCell(2).SetCellValue(accounting_period_name);
                        row.CreateCell(3).SetCellValue(advance_contract_number);
                        row.CreateCell(4).SetCellValue(" " + Convert.ToDateTime(data.from_date).ToString("yyyy-MM-dd"));
                        row.CreateCell(5).SetCellValue(" " + Convert.ToDateTime(data.to_date).ToString("yyyy-MM-dd"));
                        row.CreateCell(6).SetCellValue(PublicFunctions.Pecahan(data.volume));
                        row.CreateCell(7).SetCellValue(PublicFunctions.Pecahan(data.distance));
                        row.CreateCell(8).SetCellValue(business_unit_code);
                        row.CreateCell(9).SetCellValue(data.note);
                        row.CreateCell(10).SetCellValue(iCount);
                        row.CreateCell(11).SetCellValue(" " + Convert.ToDateTime(item.transaction_item_date).ToString("yyyy-MM-dd"));
                        row.CreateCell(12).SetCellValue(mine_location_code);
                        row.CreateCell(13).SetCellValue(business_area_code);
                        row.CreateCell(14).SetCellValue(Convert.ToString(item.quantity_item));
                        RowCount++;
                        iCount++;
                    }
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

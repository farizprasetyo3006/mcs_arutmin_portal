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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Npoi.Mapper;
using Npoi.Mapper.Attributes;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Location.Controllers
{
    [Area("Location")]
    public class BusinessAreaController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly mcsContext dbContext;

        public BusinessAreaController(IConfiguration Configuration)
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
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.BusinessArea];
            ViewBag.BreadcrumbCode = WebAppMenu.BusinessArea;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Modelling];
            ViewBag.SubAreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Location];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.BusinessArea];
            ViewBag.BreadcrumbCode = WebAppMenu.BusinessArea;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var svc = new BusinessLogic.Entity.BusinessArea(CurrentUserContext);
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

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "BusinessArea.xlsx";
            sFileName = sFileName.Insert(sFileName.LastIndexOf("."), string.Format("_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff")));

            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);

            FileInfo file = new FileInfo(Path.Combine(FilePath, sFileName));
            var memory = new MemoryStream();
            try
            {
                using (var fs = new FileStream(Path.Combine(FilePath, sFileName), FileMode.Create, FileAccess.Write))
                {
                    int RowCount = 1;
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Business Area");
                    IRow row = excelSheet.CreateRow(0);
                    // Setting Cell Heading
                    row.CreateCell(0).SetCellValue("Parent Business Area Code");
                    row.CreateCell(1).SetCellValue("Business Area Code");
                    row.CreateCell(2).SetCellValue("Business Area Name");
                    row.CreateCell(3).SetCellValue("Business Unit");

                    excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                    mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                    var business_unit = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                    var tabledata = dbContext.business_area.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                (business_unit != null && business_unit != "" ? o.business_unit_id == business_unit : true))
                        .OrderByDescending(o => o.created_on);
                    // Inserting values to table
                    foreach (var Value in tabledata)
                    {
                        var parent_business_area_code = "";
                        var business_area = dbFind.business_area
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.id == Value.parent_business_area_id)
                            .FirstOrDefault();
                        if (business_area != null)
                        {
                            parent_business_area_code = business_area.business_area_code.ToString();
                        }

                        var business_unitB = "";
                        var BU = dbFind.business_unit.Where(o => o.id == Value.business_unit_id).FirstOrDefault();
                        if (BU != null) business_unitB = BU.business_unit_code.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(parent_business_area_code);
                        row.CreateCell(1).SetCellValue(Value.business_area_code);
                        row.CreateCell(2).SetCellValue(Value.business_area_name);
                        row.CreateCell(3).SetCellValue(business_unitB);

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
                }
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            // Deletes the generated file from /wwwroot folder
            finally
            {
                var path = Path.Combine(FilePath, sFileName);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            return null;
        }

    }
}

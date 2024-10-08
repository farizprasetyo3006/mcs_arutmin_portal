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
using Npoi.Mapper;
using Npoi.Mapper.Attributes;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Equipment.Controllers
{
    [Area("Equipment")]
    public class EquipmentTypeController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public EquipmentTypeController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Equipment];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.EquipmentType];
            ViewBag.BreadcrumbCode = WebAppMenu.EquipmentType;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            return View();
        }

        public async Task<IActionResult> Detail(string Id)
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Equipment];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.EquipmentType];
            ViewBag.BreadcrumbCode = WebAppMenu.EquipmentType;

            ViewBag.RoleAccessList = HttpContext.Session.GetString("RoleAccessList");

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var svc = new EquipmentType(CurrentUserContext);
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
            string sFileName = "EquipmentType.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Equipment Type");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Equipment Type Code");
                row.CreateCell(1).SetCellValue("Equipment Type Name");
                row.CreateCell(2).SetCellValue("Is Active");
                row.CreateCell(3).SetCellValue("Item Class");
                row.CreateCell(4).SetCellValue("Business Unit");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                var tabledata = dbContext.equipment_type.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var Value in tabledata)
                {
                    var business_unit = "";
                    var BU = dbFind.business_unit.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Value.business_unit_id).FirstOrDefault();
                    if (BU != null) business_unit = BU.business_unit_code.ToString();
                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(Value.equipment_type_code);
                    row.CreateCell(1).SetCellValue(Value.equipment_type_name);
                    row.CreateCell(2).SetCellValue(Convert.ToBoolean(Value.is_active));
                    row.CreateCell(3).SetCellValue(Value.item_class);
                    row.CreateCell(4).SetCellValue(business_unit);

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

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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace MCSWebApp.Areas.Planning.Controllers
{
    [Area("Planning")]
    public class BlendingPlanController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public BlendingPlanController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Planning];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.BlendingPlan];
            ViewBag.BreadcrumbCode = WebAppMenu.BlendingPlan;

            return View();
        }

        public async Task<IActionResult> ExcelExport()
        {
            string sFileName = "BlendingPlan.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Blending Plan");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("Planning Category");
                row.CreateCell(2).SetCellValue("Product");
                row.CreateCell(3).SetCellValue("Plan Date");
                row.CreateCell(4).SetCellValue("Shift");
                row.CreateCell(5).SetCellValue("Shipping Order");
                row.CreateCell(6).SetCellValue("Destination Location");
                row.CreateCell(7).SetCellValue("Business Area");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                mcsContext dbFind2 = new mcsContext(DbOptionBuilder.Options);

                var HeaderIdList = new List<string>();

                var header = dbContext.blending_plan.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var isi in header)
                {
                    HeaderIdList.Add(isi.id);

                    var product_code = "";
                    var product = dbFind.product.Where(o => o.id == isi.product_id).FirstOrDefault();
                    if (product != null) product_code = product.product_code.ToString();

                    var shift_code = "";
                    var shift = dbFind.shift.Where(o => o.id == isi.source_shift_id).FirstOrDefault();
                    if (shift != null) shift_code = shift.shift_code.ToString();

                    var despatch_order_number = "";
                    var despatch_order = dbFind.despatch_order.Where(o => o.id == isi.despatch_order_id).FirstOrDefault();
                    if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                    var destination_location_code = "";
                    var minelocation = dbFind.mine_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Select(o => new { id = o.id, location_code = o.mine_location_code, business_area_id = o.business_area_id });
                    var vessel = dbFind.vessel
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Select(o => new { id = o.id, location_code = o.vehicle_id, business_area_id = o.business_area_id });
                    var stockpileocation = dbFind.stockpile_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Select(o => new { id = o.id, location_code = o.stockpile_location_code, business_area_id = o.business_area_id });
                    var portlocation = dbFind.port_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Select(o => new { id = o.id, location_code = o.port_location_code, business_area_id = o.business_area_id });

                    var business_area_code = "";
                    var location = minelocation.Union(vessel).Union(stockpileocation).Union(portlocation)
                        .Where(o => o.id == isi.destination_location_id)
                        .FirstOrDefault();
                    if (location != null)
                    {
                        destination_location_code = location.location_code.ToString();

                        var business_area = dbFind.business_area.Where(o => o.id == location.business_area_id).FirstOrDefault();
                        business_area_code = business_area?.business_area_code;
                    }

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(isi.transaction_number);
                    row.CreateCell(1).SetCellValue(isi.planning_category);
                    row.CreateCell(2).SetCellValue(product_code);
                    row.CreateCell(3).SetCellValue(" " + (isi.unloading_datetime != null ? Convert.ToDateTime(isi.unloading_datetime).ToString("yyyy-MM-dd") : ""));
                    row.CreateCell(4).SetCellValue(shift_code);
                    row.CreateCell(5).SetCellValue(despatch_order_number);
                    row.CreateCell(6).SetCellValue(destination_location_code);
                    row.CreateCell(7).SetCellValue(business_area_code);

                    RowCount++;
                    if (RowCount > 50) break;
                }

                //***** detail
                excelSheet = workbook.CreateSheet("Detail Source");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("SPEC TS");
                row.CreateCell(2).SetCellValue("Source Location");
                row.CreateCell(3).SetCellValue("Sampling Number");
                row.CreateCell(4).SetCellValue("IKH Pit");
                row.CreateCell(5).SetCellValue("Note");
                row.CreateCell(6).SetCellValue("Volume");
                row.CreateCell(7).SetCellValue("TM");
                row.CreateCell(8).SetCellValue("IM");
                row.CreateCell(9).SetCellValue("AC");
                row.CreateCell(10).SetCellValue("VM");
                row.CreateCell(11).SetCellValue("FC");
                row.CreateCell(12).SetCellValue("TS");
                row.CreateCell(13).SetCellValue("CV(adb)");
                row.CreateCell(14).SetCellValue("CV(ar)");
                row.CreateCell(15).SetCellValue("Equipment");
                row.CreateCell(16).SetCellValue("Transport");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                RowCount = 1;
                foreach (var idHeader in HeaderIdList)
                {
                    var detail = dbContext.blending_plan_source
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.blending_plan_id == idHeader)
                        .OrderByDescending(o => o.created_on);

                    // Inserting values to table
                    foreach (var isi in detail)
                    {
                        var transaction_number = "";
                        var blending_plan = dbFind.blending_plan.Where(o => o.id == isi.blending_plan_id).FirstOrDefault();
                        if (blending_plan != null) transaction_number = blending_plan.transaction_number.ToString();

                        var sampling_number = "";
                        var sn = dbFind.quality_sampling.Where(o => o.id == isi.quality_sampling_id).FirstOrDefault();
                        if (sn != null) sampling_number = sn.sampling_number.ToString();

                        var ikh_pit = "";
                        var ik = dbFind.blending_plan.Where(o => o.id == isi.ikh_pit_id).FirstOrDefault();
                        if (ik != null) ikh_pit = ik.transaction_number.ToString();

                        var equipment = "";
                        var e = dbFind.equipment.Where(o => o.id == isi.equipment_id).FirstOrDefault();
                        if (e != null) equipment = e.equipment_name.ToString();

                        var transport= "";
                        var t = dbFind.vessel.Where(o => o.id == isi.transport_id).FirstOrDefault();
                        if (t != null) transport = t.vehicle_name.ToString();

                        var source_location_code = "";
                        var minelocation = dbFind.mine_location
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Select(o => new { id = o.id, code = o.mine_location_code });
                        var stockpileocation = dbFind.stockpile_location
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Select(o => new { id = o.id, code = o.stockpile_location_code });
                        var portlocation = dbFind.port_location
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Select(o => new { id = o.id, code = o.port_location_code });

                        var location = minelocation.Union(stockpileocation).Union(portlocation)
                            .Where(o => o.id == isi.source_location_id)
                            .FirstOrDefault();
                        if (location != null) source_location_code = location.code.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(transaction_number);
                        row.CreateCell(1).SetCellValue(Convert.ToDouble(isi.spec_ts));
                        row.CreateCell(2).SetCellValue(source_location_code);
                        row.CreateCell(3).SetCellValue(sampling_number);
                        row.CreateCell(4).SetCellValue(ikh_pit);
                        row.CreateCell(5).SetCellValue(isi.note);
                        row.CreateCell(6).SetCellValue(Convert.ToDouble(isi.volume));
                        row.CreateCell(7).SetCellValue(Convert.ToDouble(isi.analyte_1));
                        row.CreateCell(8).SetCellValue(Convert.ToDouble(isi.analyte_2));
                        row.CreateCell(9).SetCellValue(Convert.ToDouble(isi.analyte_3));
                        row.CreateCell(10).SetCellValue(Convert.ToDouble(isi.analyte_4));
                        row.CreateCell(11).SetCellValue(Convert.ToDouble(isi.analyte_5));
                        row.CreateCell(12).SetCellValue(Convert.ToDouble(isi.analyte_6));
                        row.CreateCell(13).SetCellValue(Convert.ToDouble(isi.analyte_7));
                        row.CreateCell(14).SetCellValue(Convert.ToDouble(isi.analyte_8));
                        row.CreateCell(15).SetCellValue(equipment);
                        row.CreateCell(16).SetCellValue(transport);

                        RowCount++;
                        if (RowCount > 50) break;
                    }
                }
                //****************

                //***** detail Product
                excelSheet = workbook.CreateSheet("Detail Product");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("Product Specifiaction");
                row.CreateCell(2).SetCellValue("Volume (MT)");
                row.CreateCell(3).SetCellValue("TM (%ar)");
                row.CreateCell(4).SetCellValue("IM (%adb)");
                row.CreateCell(5).SetCellValue("Ash (%adb)");
                row.CreateCell(6).SetCellValue("VM (%adb)");
                row.CreateCell(7).SetCellValue("FC (%adb)");
                row.CreateCell(8).SetCellValue("TS (%adb)");
                row.CreateCell(9).SetCellValue("CV Kcal/Kg (adb)");
                row.CreateCell(10).SetCellValue("CV Kcal/Kg (ar)");
                row.CreateCell(11).SetCellValue("RD (gr/cc)");
                row.CreateCell(12).SetCellValue("RDI (gr/cc)");
                row.CreateCell(13).SetCellValue("HGI");
                row.CreateCell(14).SetCellValue("Note");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                RowCount = 1;
                foreach (var idHeader2 in HeaderIdList)
                {
                    var detailP = dbFind2.blending_plan_product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.blending_plan_id == idHeader2)
                        .OrderByDescending(o => o.created_on);

                    // Inserting values to table
                    foreach (var isi2 in detailP)
                    {
                        var transaction_number = "";
                        var blending_plan = dbFind.blending_plan.Where(o => o.id == isi2.blending_plan_id).FirstOrDefault();
                        if (blending_plan != null) transaction_number = blending_plan.transaction_number.ToString();

                        var product_name = "";
                        var product = dbFind.product.Where(o => o.id == isi2.product_id).FirstOrDefault();
                        if (product != null) product_name = product.product_name.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(transaction_number);
                        row.CreateCell(1).SetCellValue(product_name);
                        row.CreateCell(2).SetCellValue((double)isi2.volume);
                        row.CreateCell(3).SetCellValue((double)isi2.analyte_1);
                        row.CreateCell(4).SetCellValue((double)isi2.analyte_2);
                        row.CreateCell(5).SetCellValue((double)isi2.analyte_3);
                        row.CreateCell(6).SetCellValue((double)isi2.analyte_4);
                        row.CreateCell(7).SetCellValue((double)isi2.analyte_5);
                        row.CreateCell(8).SetCellValue((double)isi2.analyte_6);
                        row.CreateCell(9).SetCellValue((double)isi2.analyte_7);
                        row.CreateCell(10).SetCellValue((double)isi2.analyte_8);
                        row.CreateCell(11).SetCellValue((double)isi2.analyte_9);
                        row.CreateCell(12).SetCellValue((double)isi2.analyte_10);
                        row.CreateCell(13).SetCellValue((double)isi2.analyte_11);
                        row.CreateCell(14).SetCellValue(isi2.note);

                        RowCount++;
                        if (RowCount > 50) break;
                    }
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

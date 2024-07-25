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
    public class ShippingInstructionController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ShippingInstructionController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ContractManagement];
            ViewBag.AreaBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Port];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ShippingInstruction];
            ViewBag.BreadcrumbCode = WebAppMenu.ShippingInstruction;

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "ShippingInstruction.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("ShippingInstruction");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Transaction Number");
                row.CreateCell(1).SetCellValue("Shipping Instruction Date");
                row.CreateCell(2).SetCellValue("Shipping Order");
                row.CreateCell(3).SetCellValue("Notify Party");
                row.CreateCell(4).SetCellValue("Cargo Description");
                row.CreateCell(5).SetCellValue("Sampling Template");
                row.CreateCell(6).SetCellValue("Marked");
                row.CreateCell(7).SetCellValue("Issued Date");
                row.CreateCell(8).SetCellValue("Placed");
                row.CreateCell(9).SetCellValue("Created By");
                row.CreateCell(10).SetCellValue("Lampiran 1");
                row.CreateCell(11).SetCellValue("Lampiran 2");
                row.CreateCell(12).SetCellValue("Lampiran 3");
                row.CreateCell(13).SetCellValue("Lampiran 4");
                row.CreateCell(14).SetCellValue("Lampiran 5");
                row.CreateCell(15).SetCellValue("SI Number");
                row.CreateCell(16).SetCellValue("Notify Party Address");
                row.CreateCell(17).SetCellValue("HS Code");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var business_unit1 = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                if (Data != null && Data.selectedIds != null)
                {
                    var selectedIds = ((string)Data.selectedIds)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var tabledata = dbContext.shipping_instruction.Where(o => o.organization_id == CurrentUserContext.OrganizationId && selectedIds.Contains(o.id))
                    .OrderByDescending(o => o.shipping_instruction_date);
                    // Inserting values to table
                    foreach (var baris in tabledata)
                    {
                        var despatch_order_number = "";
                        var despatch_order = dbFind.despatch_order.Where(o => o.id == baris.despatch_order_id).FirstOrDefault();
                        if (despatch_order != null) despatch_order_number = despatch_order.despatch_order_number.ToString();

                        //var barge_id = "";
                        //var barge = dbFind.barge.Where(o => o.id == baris.barge_id).FirstOrDefault();
                        //if (barge != null) barge_id = barge.vehicle_name.ToString();

                        //var tug_id = "";
                        //var tug = dbFind.tug.Where(o => o.id == baris.tug_id).FirstOrDefault();
                        //if (tug != null) tug_id = tug.vehicle_name.ToString();

                        //var vendor_id = "";
                        //var vendor = dbFind.contractor.Where(o => o.id == baris.vendor_id).FirstOrDefault();
                        //if (vendor != null) vendor_id = vendor.business_partner_name.ToString();

                        var sampling_template_code = "";
                        var sampling_template = dbFind.sampling_template.Where(o => o.id == baris.sampling_template_id).FirstOrDefault();
                        if (sampling_template != null) sampling_template_code = sampling_template.sampling_template_code.ToString();

                        row = excelSheet.CreateRow(RowCount);
                        row.CreateCell(0).SetCellValue(baris.shipping_instruction_number);
                        row.CreateCell(1).SetCellValue(" " + Convert.ToDateTime(baris.shipping_instruction_date).ToString("yyyy-MM-dd HH:mm:ss"));
                        row.CreateCell(2).SetCellValue(despatch_order_number);
                        row.CreateCell(3).SetCellValue(baris.notify_party);//(baris.to_other);
                        row.CreateCell(4).SetCellValue(baris.cargo_description);
                        row.CreateCell(5).SetCellValue(sampling_template_code);//(baris.cc);
                        row.CreateCell(6).SetCellValue(baris.marked);
                        row.CreateCell(7).SetCellValue(" " + Convert.ToDateTime(baris.issued_date).ToString("yyyy-MM-dd HH:mm:ss"));
                        row.CreateCell(8).SetCellValue(baris.placed);
                        row.CreateCell(9).SetCellValue(baris.shipping_instruction_created_by);
                        row.CreateCell(10).SetCellValue(baris.lampiran1);
                        row.CreateCell(11).SetCellValue(baris.lampiran2);
                        row.CreateCell(12).SetCellValue(baris.lampiran3);
                        row.CreateCell(13).SetCellValue(baris.lampiran4);
                        row.CreateCell(14).SetCellValue(baris.lampiran5);
                        row.CreateCell(15).SetCellValue(baris.si_number);
                        row.CreateCell(16).SetCellValue(baris.notify_party_address);
                        row.CreateCell(17).SetCellValue(baris.hs_code);

                        RowCount++;
                       // if (RowCount > 50) break;
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

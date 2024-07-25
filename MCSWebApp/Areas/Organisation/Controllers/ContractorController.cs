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
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Areas.Organisation.Controllers
{
    [Area("Organisation")]
    public class ContractorController : BaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ContractorController(IConfiguration Configuration)
            : base(Configuration)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        public IActionResult Index()
        {
            ViewBag.WebAppName = WebAppName;
            ViewBag.RootBreadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.ProductionLogistics];
            ViewBag.Breadcrumb = WebAppMenu.BreadcrumbText[WebAppMenu.Contractor];
            ViewBag.BreadcrumbCode = WebAppMenu.Contractor;

            return View();
        }

        public async Task<IActionResult> ExcelExport([FromBody] dynamic Data)
        {
            string sFileName = "Contractor.xlsx";
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
                ISheet excelSheet = workbook.CreateSheet("Contractor");
                IRow row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Contractor Name");
                row.CreateCell(1).SetCellValue("Contractor Code");
                row.CreateCell(2).SetCellValue("Primary Address");
                row.CreateCell(3).SetCellValue("Country");
                row.CreateCell(4).SetCellValue("Primary Contact Name");
                row.CreateCell(5).SetCellValue("Primary Contact Email");
                row.CreateCell(6).SetCellValue("Primary Contact Phone");
                row.CreateCell(7).SetCellValue("Barge Trip Plan");
                row.CreateCell(8).SetCellValue("Bank");
                row.CreateCell(9).SetCellValue("Bank Account");
                row.CreateCell(10).SetCellValue("Currency");
                row.CreateCell(11).SetCellValue("Taxable");
                row.CreateCell(12).SetCellValue("Equipment Owner");
                row.CreateCell(13).SetCellValue("Truck Owner");
                row.CreateCell(14).SetCellValue("Barge Owner");
                row.CreateCell(15).SetCellValue("Tug Owner");
                row.CreateCell(16).SetCellValue("Vessel Owner");
                row.CreateCell(17).SetCellValue("Train Owner");
                row.CreateCell(18).SetCellValue("Surveyor");
                row.CreateCell(19).SetCellValue("Other");
                row.CreateCell(20).SetCellValue("Is Active");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);
                var business_unit1 = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                IOrderedQueryable<contractor> tabledata;
                var selectedIds = ((string)Data.selectedIds)
                    .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                tabledata = dbContext.contractor
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                && selectedIds.Contains(o.id))
                .OrderByDescending(o => o.created_on);
                // Inserting values to table
                foreach (var baris in tabledata)
                {
                   /* var contractor_type_name = "";
                    var contractor_type = dbFind.contractor_type.Where(o => o.id == baris.contractor_type_id).FirstOrDefault();
                    if (contractor_type != null) contractor_type_name = contractor_type.contractor_type_name.ToString();*/

                    var country_code = "";
                    var country = dbFind.country.Where(o => o.id == baris.country_id).FirstOrDefault();
                    if (country != null) country_code = country.country_code.ToString();

                    var bank_code = ""; var account_number = ""; var currency_code = "";
                    var bank_account = dbFind.vw_bank_account.Where(o => o.id == baris.bank_account_id).FirstOrDefault();
                    if (bank_account != null)
                    {
                        account_number = bank_account.account_number.ToString();
                        bank_code = bank_account.bank_code.ToString();
                        currency_code = bank_account.currency_code.ToString();
                    }

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(baris.business_partner_name);
                    row.CreateCell(1).SetCellValue(baris.business_partner_code);
                    row.CreateCell(2).SetCellValue(baris.primary_address);
                    row.CreateCell(3).SetCellValue(country_code);
                    row.CreateCell(4).SetCellValue(baris.primary_contact_name);
                    row.CreateCell(5).SetCellValue(baris.primary_contact_email);
                    row.CreateCell(6).SetCellValue(baris.primary_contact_phone);
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(baris.barge_trip_plan));
                    row.CreateCell(8).SetCellValue(bank_code);
                    row.CreateCell(9).SetCellValue(account_number);
                    row.CreateCell(10).SetCellValue(currency_code);
                    row.CreateCell(11).SetCellValue(Convert.ToBoolean(baris.is_taxable));
                    row.CreateCell(12).SetCellValue(Convert.ToBoolean(baris.is_equipment_owner));
                    row.CreateCell(13).SetCellValue(Convert.ToBoolean(baris.is_truck_owner));
                    row.CreateCell(14).SetCellValue(Convert.ToBoolean(baris.is_barge_owner));
                    row.CreateCell(15).SetCellValue(Convert.ToBoolean(baris.is_tug_owner));
                    row.CreateCell(16).SetCellValue(Convert.ToBoolean(baris.is_vessel_owner));
                    row.CreateCell(17).SetCellValue(Convert.ToBoolean(baris.is_train_owner));
                    row.CreateCell(18).SetCellValue(Convert.ToBoolean(baris.is_surveyor));
                    row.CreateCell(19).SetCellValue(Convert.ToBoolean(baris.is_other));
                    row.CreateCell(20).SetCellValue(Convert.ToBoolean(baris.is_active));

                    RowCount++;
                }

                //*********** #2 Document Attachment Sheet
               /* RowCount = 1;
                excelSheet = workbook.CreateSheet("Attachment");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Contractor ID");
                row.CreateCell(1).SetCellValue("File Name");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail1 = dbContext.contractor_document;
                // Inserting values to table
                foreach (var isi in detail1)
                {
                    var contractor_code = "";
                    var contractor = dbFind.contractor.Where(o => o.id == isi.contractor_id).FirstOrDefault();
                    if (contractor != null) contractor_code = contractor.business_partner_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(contractor_code);
                    row.CreateCell(1).SetCellValue(isi.file_name.ToString());
                    RowCount++;
                }*/
                //*********** #3
                RowCount = 1;
                excelSheet = workbook.CreateSheet("Contact");
                row = excelSheet.CreateRow(0);
                // Setting Cell Heading
                row.CreateCell(0).SetCellValue("Contractor ID");
                row.CreateCell(1).SetCellValue("Name");
                row.CreateCell(2).SetCellValue("Email");
                row.CreateCell(3).SetCellValue("Phone");
                row.CreateCell(4).SetCellValue("Position");

                excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                var detail2 = dbContext.contact.Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                // Inserting values to table
                foreach (var isi in detail2)
                {
                    var contractor_code = "";
                    var contractor = dbFind.contractor.Where(o => o.id == isi.business_partner_id).FirstOrDefault();
                    if (contractor != null) contractor_code = contractor.business_partner_code.ToString();

                    row = excelSheet.CreateRow(RowCount);
                    row.CreateCell(0).SetCellValue(contractor_code);
                    row.CreateCell(1).SetCellValue(isi.contact_name);
                    row.CreateCell(2).SetCellValue(isi.contact_email);
                    row.CreateCell(3).SetCellValue(isi.contact_phone);
                    row.CreateCell(4).SetCellValue(isi.contact_position);
                    RowCount++;
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
    }
}

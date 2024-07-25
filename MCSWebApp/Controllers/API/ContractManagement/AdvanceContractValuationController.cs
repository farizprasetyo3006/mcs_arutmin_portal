using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Http;
using DevExtreme.AspNet.Mvc;
using DataAccess.EFCore;
using Microsoft.EntityFrameworkCore;
using NLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataAccess.DTO;
using Omu.ValueInjecter;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using BusinessLogic;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Common;
using Microsoft.AspNetCore.StaticFiles;
using WebApp.Extensions;

namespace MCSWebApp.Controllers.API.ContractManagement
{
    [Route("api/ContractManagement/[controller]")]
    [ApiController]
    public class AdvanceContractValuationController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public AdvanceContractValuationController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_advance_contract_valuation
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_advance_contract_valuation.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(advance_contract_valuation),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new advance_contract_valuation();
                        JsonConvert.PopulateObject(values, record);

                        var cekemployee = dbContext.employee.Where(o => o.id == record.employee_id)
                            .FirstOrDefault();
                        if (cekemployee.is_active == false)
                            return BadRequest("The Employee is NOT ACTIVE.");

                        record.id = Guid.NewGuid().ToString("N");
                        record.created_by = CurrentUserContext.AppUserId;
                        record.created_on = DateTime.Now;
                        record.modified_by = null;
                        record.modified_on = null;
                        record.is_active = true;
                        record.is_default = null;
                        record.is_locked = null;
                        record.entity_id = null;
                        record.owner_id = CurrentUserContext.AppUserId;
                        record.organization_id = CurrentUserContext.OrganizationId;
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        dbContext.advance_contract_valuation.Add(record);

                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();

                        return Ok(record);
                    }
                    else
                    {
                        return BadRequest("User is not authorized.");
                    }
                }
				catch (Exception ex)
				{
					logger.Error(ex.InnerException ?? ex);
					return BadRequest(ex.InnerException?.Message ?? ex.Message);
				}
            }
        }

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.advance_contract_valuation
                        .Where(o => o.id == key
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            var e = new entity();
                            e.InjectFrom(record);

                            JsonConvert.PopulateObject(values, record);

                            var cekemployee = dbContext.employee.Where(o => o.id == record.employee_id)
                                .FirstOrDefault();
                            if (cekemployee.is_active == false)
                                return BadRequest("The Employee is NOT ACTIVE.");

                            record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();

                            return Ok(record);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else
                    {
                        return BadRequest("Record does not exist.");
                    }
                }
				catch (Exception ex)
				{
					logger.Error(ex.InnerException ?? ex);
					return BadRequest(ex.InnerException?.Message ?? ex.Message);
				}
            }
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.advance_contract_valuation
                        .Where(o => o.id == key
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.advance_contract_valuation.Remove(record);

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    return Ok();
                }
				catch (Exception ex)
				{
					logger.Error(ex.InnerException ?? ex);
					return BadRequest(ex.InnerException?.Message ?? ex.Message);
				}
            }
        }

        [HttpGet("List")]
        public async Task<IActionResult> List([FromQuery] string q)
        {
            try
            {
                var rows = dbContext.vw_advance_contract_valuation.AsNoTracking();
                rows = rows.Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin);
                if (!string.IsNullOrEmpty(q))
                {
                    rows = rows.Where(o => o.progress_claim_name.Contains(q));
                }

                return Ok(await rows.ToListAsync());
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] advance_contract_valuation Record)
        {
            try
            {
                var record = dbContext.advance_contract_valuation
                    .Where(o => o.id == Record.id)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var e = new entity();
                        e.InjectFrom(record);
                        record.InjectFrom(Record);
                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        await dbContext.SaveChangesAsync();
                        return Ok(record);
                    }
                    else
                    {
                        return BadRequest("User is not authorized.");
                    }
                }
                else if (await mcsContext.CanCreate(dbContext, nameof(advance_contract_valuation),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new advance_contract_valuation();
                    record.InjectFrom(Record);

                    record.id = Guid.NewGuid().ToString("N");
                    record.created_by = CurrentUserContext.AppUserId;
                    record.created_on = DateTime.Now;
                    record.modified_by = null;
                    record.modified_on = null;
                    record.is_active = true;
                    record.is_default = null;
                    record.is_locked = null;
                    record.entity_id = null;
                    record.owner_id = CurrentUserContext.AppUserId;
                    record.organization_id = CurrentUserContext.OrganizationId;
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    dbContext.advance_contract_valuation.Add(record);
                    await dbContext.SaveChangesAsync();

                    return Ok(record);
                }
                else
                {
                    return BadRequest("User is not authorized.");
                }
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpPost("UploadDocument")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UploadDocument([FromBody] dynamic FileDocument)
        {
            var result = new StandardResult();
            long size = 0;

            if (FileDocument == null)
            {
                return BadRequest("No file uploaded!");
            }

            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);

            var fileName = (string)FileDocument.filename;
            FilePath += $@"\{fileName}";

            string strfile = (string)FileDocument.data;
            byte[] arrfile = Convert.FromBase64String(strfile);

            await System.IO.File.WriteAllBytesAsync(FilePath, arrfile);

            size = fileName.Length;
            string sFileExt = Path.GetExtension(FilePath).ToLower();

            ISheet sheet;
            dynamic wb;
            if (sFileExt == ".xls")
            {
                FileStream stream = System.IO.File.OpenRead(FilePath);
                wb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats
                sheet = wb.GetSheetAt(0); //get first sheet from workbook
                stream.Close();
            }
            else
            {
                wb = new XSSFWorkbook(FilePath); //This will read 2007 Excel format
                sheet = wb.GetSheetAt(0); //get first sheet from workbook
            }

            string teks = "";
            bool gagal = false; string errormessage = "";

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.Count() < 5) continue;

                    var advance_contract_id = "";
                    var advance_contract = dbContext.advance_contract
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                               o.advance_contract_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()).FirstOrDefault();
                    if (advance_contract != null) advance_contract_id = advance_contract.id.ToString();

                    string accounting_period_id = "";
                    var accounting_period = dbContext.accounting_period
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                               o.accounting_period_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower()).FirstOrDefault();
                    if (accounting_period != null) accounting_period_id = accounting_period.id.ToString();

                    string target_uom_id = "";
                    var uom = dbContext.uom.Where(o => o.uom_symbol == PublicFunctions.IsNullCell(row.GetCell(6))
                            && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                    if (uom != null) target_uom_id = uom.id.ToString();

                    string actual_uom_id = "";
                    uom = dbContext.uom.Where(o => o.uom_symbol == PublicFunctions.IsNullCell(row.GetCell(8))
                            && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                    if (uom != null) actual_uom_id = uom.id.ToString();

                    var record = dbContext.advance_contract_valuation
                        .Where(o => o.advance_contract_valuation_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower()
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new advance_contract_valuation();
                        record.id = Guid.NewGuid().ToString("N");
                        record.created_by = CurrentUserContext.AppUserId;
                        record.created_on = DateTime.Now;
                        record.modified_by = null;
                        record.modified_on = null;
                        record.is_active = true;
                        record.is_default = null;
                        record.is_locked = null;
                        record.entity_id = null;
                        record.owner_id = CurrentUserContext.AppUserId;
                        record.organization_id = CurrentUserContext.OrganizationId;
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        dbContext.advance_contract_valuation.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 1, Line " + (i+1) + " : " + Environment.NewLine;
                    }
                    else errormessage = ex.Message;

                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                    gagal = true;
                    break;
                }
            }
            wb.Close();
            if (gagal)
            {
                await transaction.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "AdvanceContractValuation");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpGet("DownloadDocument/{Id}")]
        public async Task<IActionResult> DownloadDocument(string Id)
        {
            logger.Debug($"string Id = {Id}");

            try
            {
                string sFileName = "temp.xlsx";
                string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
                if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);

                FileInfo file = new FileInfo(Path.Combine(FilePath, sFileName));
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(FilePath, sFileName), FileMode.Create, FileAccess.Write))
                {
                    int RowCount = 1;
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Sheet1");
                    IRow row = excelSheet.CreateRow(0);
                    // Setting Cell Heading
                    row.CreateCell(0).SetCellValue("Invoice Group");
                    row.CreateCell(1).SetCellValue("Customer No");
                    row.CreateCell(2).SetCellValue("Sales Person");
                    row.CreateCell(3).SetCellValue("Position Id");
                    row.CreateCell(4).SetCellValue("Invoice Date (YYYYMMDD)");
                    row.CreateCell(5).SetCellValue("Due Date (YYYYMMDD)");
                    row.CreateCell(6).SetCellValue("Delivery Location");
                    row.CreateCell(7).SetCellValue("Full Period");
                    row.CreateCell(8).SetCellValue("Reference");
                    row.CreateCell(9).SetCellValue("Dunning Code");
                    row.CreateCell(10).SetCellValue("Invoice Class 1");
                    row.CreateCell(11).SetCellValue("Invoice Class 2");
                    row.CreateCell(12).SetCellValue("Invoice Class 3");
                    row.CreateCell(13).SetCellValue("Invoice Class 4");
                    row.CreateCell(14).SetCellValue("Tax No");
                    row.CreateCell(15).SetCellValue("Invoice Description");
                    row.CreateCell(16).SetCellValue("Account Group Code");
                    row.CreateCell(17).SetCellValue("Item Description");
                    row.CreateCell(18).SetCellValue("Revenue Code");
                    row.CreateCell(19).SetCellValue("VAT");
                    row.CreateCell(20).SetCellValue("Price Code");
                    row.CreateCell(21).SetCellValue("UOM");
                    row.CreateCell(22).SetCellValue("Invoice Quantity");
                    row.CreateCell(23).SetCellValue("Unit Price");
                    row.CreateCell(24).SetCellValue("Item Value");
                    row.CreateCell(25).SetCellValue("Account Code");
                    row.CreateCell(26).SetCellValue("Work Order");
                    row.CreateCell(27).SetCellValue("Project");
                    row.CreateCell(28).SetCellValue("UPLOAD STATUS");

                    excelSheet.DefaultColumnWidth = PublicFunctions.ExcelDefaultColumnWidth;

                    var tabledata = dbContext.vw_advance_contract_valuation_download
                        .Where(o => o.advance_contract_valuation_id == Id && o.organization_id == CurrentUserContext.OrganizationId);
                    foreach (var baris in tabledata)
                    {
                        row = excelSheet.CreateRow(RowCount);
                        //row.CreateCell(0).SetCellValue(baris.invoice_group);
                        row.CreateCell(0).SetCellValue("1");
                        //row.CreateCell(1).SetCellValue(baris.customer_no);
                        row.CreateCell(1).SetCellValue("000001");
                        row.CreateCell(2).SetCellValue(baris.sales_person);
                        row.CreateCell(3).SetCellValue(baris.position_id);
                        row.CreateCell(4).SetCellValue(baris.invoice_date);
                        row.CreateCell(5).SetCellValue(baris.due_date);
                        row.CreateCell(6).SetCellValue(baris.delivery_location);
                        row.CreateCell(7).SetCellValue(baris.full_period);
                        row.CreateCell(8).SetCellValue(baris.reference);
                        row.CreateCell(9).SetCellValue(baris.dunning_code);
                        row.CreateCell(10).SetCellValue(baris.invoice_class_1);
                        row.CreateCell(11).SetCellValue(baris.invoice_class_2);
                        row.CreateCell(12).SetCellValue(baris.invoice_class_3);
                        row.CreateCell(13).SetCellValue(baris.invoice_class_4);
                        row.CreateCell(14).SetCellValue(baris.tax_no);
                        row.CreateCell(15).SetCellValue(baris.invoice_description);
                        row.CreateCell(16).SetCellValue(baris.account_group_code);
                        row.CreateCell(17).SetCellValue(baris.item_description);
                        row.CreateCell(18).SetCellValue(baris.revenue_code);
                        row.CreateCell(19).SetCellValue(baris.vat);
                        row.CreateCell(20).SetCellValue(baris.price_code);
                        row.CreateCell(21).SetCellValue(baris.uom);
                        row.CreateCell(22).SetCellValue(baris.invoice_quantity);
                        row.CreateCell(23).SetCellValue(baris.unit_price);
                        row.CreateCell(24).SetCellValue(Convert.ToDouble(baris.final_value));
                        row.CreateCell(25).SetCellValue(baris.account_code);
                        row.CreateCell(26).SetCellValue(baris.work_order);
                        row.CreateCell(27).SetCellValue(baris.project);
                        row.CreateCell(28).SetCellValue(baris.upload_status);
                        RowCount++;
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

                //throw new Exception("File not found");
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}

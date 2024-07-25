using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using DataAccess.EFCore;
using NLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataAccess.DTO;
using Omu.ValueInjecter;
using Microsoft.EntityFrameworkCore;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using BusinessLogic;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Common;
using DocumentFormat.OpenXml.InkML;

namespace MCSWebApp.Controllers.API.General
{
    [Route("api/General/[controller]")]
    [ApiController]
    public class CurrencyExchangeController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public CurrencyExchangeController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.currency_exchange
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
                //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.currency_exchange.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpGet("GetBySource")]
        public async Task<object> GetBySource(string idSource, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.currency_exchange.Where(o => o.source_currency_id == idSource),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(currency_exchange),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new currency_exchange();
                    JsonConvert.PopulateObject(values, record);

                    var cekdata = dbContext.currency_exchange
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.exchange_type_id== record.exchange_type_id
                            && o.start_date == record.start_date)
                        .FirstOrDefault();
                    if (cekdata != null) return BadRequest("Already have the same Exchange Type and Start Date.");

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

                    dbContext.currency_exchange.Add(record);
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

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.currency_exchange
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);

                        var cekdata = dbContext.currency_exchange
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.exchange_type_id == record.exchange_type_id
                                && o.start_date == record.start_date
                                && o.id != record.id)
                            .FirstOrDefault();
                        if (cekdata != null) return BadRequest("Already have the same Exchange Type and Start Date.");

                        var cekSalesInvoice = dbContext.sales_invoice
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.currency_exchange_id == key)
                            .FirstOrDefault();
                        if (cekSalesInvoice != null) return BadRequest("Can't change this, because its already been used in Sales Invoice.");

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
                else
                {
                    return BadRequest("No default organization");
                }
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] currency_exchange Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.currency_exchange
                        .Where(o => o.id == Record.id)
                        .FirstOrDefaultAsync();
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
                            await tx.CommitAsync();
                            return Ok(record);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else if (await mcsContext.CanCreate(dbContext, nameof(currency_exchange),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new currency_exchange();
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

                        dbContext.currency_exchange.Add(record);
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

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Trace($"string key = {key}");

            try
            {
                var cekSalesInvoice = dbContext.sales_invoice
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.currency_exchange_id == key)
                    .FirstOrDefault();
                if (cekSalesInvoice != null) return BadRequest("Can't delete this, because its already been used in Sales Invoice.");

                var record = dbContext.vw_currency_exchange
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                var id_curr = "";
                if( record != null)
                {
                    if(record.item_in_coding == "MD")
                    {
                        id_curr = "USD";
                    }
                    else if(record.item_in_coding == "AVG")
                    {
                        id_curr = "AVG";
                    }
                    else if(record.item_in_coding == "JD")
                    {
                        id_curr = "JIS";
                    }
                }
                var recordApi = await dbContext.currency_exchange_api.Where(o=>o.id_date == record.end_date && o.id_curr == id_curr).FirstOrDefaultAsync();
                var currencyExchange = await dbContext.currency_exchange
                    .Where(o => o.id == key)
                    .FirstOrDefaultAsync();

                if (currencyExchange != null)
                {
                    dbContext.currency_exchange.Remove(currencyExchange);
                    await dbContext.SaveChangesAsync();
                }
                if(recordApi != null)
                {
                    dbContext.currency_exchange_api.Remove(recordApi);
                    await dbContext.SaveChangesAsync();
                }

                return Ok();
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }        

        [HttpGet("CurrencyExchangeIdLookup")]
        public async Task<object> CurrencyExchangeIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.currency_exchange
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
					.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.exchange_rate });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
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
            if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);

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

                    var source_currency_id = "";
                    var source_currency = dbContext.currency.Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                        o.currency_code == PublicFunctions.IsNullCell(row.GetCell(0))).FirstOrDefault();
                    if (source_currency != null) source_currency_id = source_currency.id.ToString();

                    var target_currency_id = "";
                    var target_currency = dbContext.currency.Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                        o.currency_code == PublicFunctions.IsNullCell(row.GetCell(1))).FirstOrDefault();
                    if (target_currency != null) target_currency_id = target_currency.id.ToString();

                    var exchangeType = "";
                    var ET = dbContext.master_list.Where(o => o.item_in_coding.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(7)).Trim().ToLower() && o.item_group == "exchange-type").FirstOrDefault();
                    if (ET != null) exchangeType = ET.id.ToString();

                    var record = dbContext.currency_exchange
                        .Where(o => o.source_currency_id == source_currency_id && o.target_currency_id == target_currency_id
                            && o.start_date == PublicFunctions.Tanggal(row.GetCell(2)) && o.exchange_type_id == exchangeType
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.end_date = PublicFunctions.Tanggal(row.GetCell(3));
                        record.exchange_rate = PublicFunctions.Desimal(row.GetCell(4));
                        record.selling_rate = PublicFunctions.Desimal(row.GetCell(5));
                        record.buying_rate = PublicFunctions.Desimal(row.GetCell(6));
                        record.exchange_type_id = exchangeType;
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new currency_exchange();
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

                        record.source_currency_id = source_currency_id;
                        record.target_currency_id = target_currency_id;
                        record.start_date = PublicFunctions.Tanggal(row.GetCell(2));
                        record.end_date = PublicFunctions.Tanggal(row.GetCell(3));
                        record.exchange_rate = PublicFunctions.Desimal(row.GetCell(4));
                        record.selling_rate = PublicFunctions.Desimal(row.GetCell(5));
                        record.buying_rate = PublicFunctions.Desimal(row.GetCell(6));
                        record.exchange_type_id = exchangeType;

                        dbContext.currency_exchange.Add(record);
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
                HttpContext.Session.SetString("filename", "CurrencyExchange");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

    }
}

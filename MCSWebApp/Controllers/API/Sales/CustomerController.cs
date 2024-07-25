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

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/[controller]")]
    [ApiController]
    public class CustomerController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public CustomerController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var record = dbContext.vw_customer
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if ((await mcsContext.CanRead(dbContext, record.entity_id, CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin) == false)
                        return BadRequest("User is not authorized.");
                }

                return await DataSourceLoader.LoadAsync(dbContext.vw_customer
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DataGridCL")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGridCL(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var datagridCL = dbContext.vw_customer
                                    .Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                                            || CurrentUserContext.IsSysAdmin)
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId);

                var arrayDataGridCL = await datagridCL.ToArrayAsync();
                foreach (vw_customer item in arrayDataGridCL)
                {
                    var remainedcl = RemainedCreditLimit(item.id, loadOptions);
                    var tstring = remainedcl.ToString();
                    item.remained_credit_limit = decimal.Parse(tstring);

                    var temp1 = new vw_customer { id = item.id, remained_credit_limit=item.remained_credit_limit };
                    var entry1 = dbContext.Entry(temp1);
                    entry1.Property(p => p.remained_credit_limit).IsModified = true;
                    dbContext.SaveChanges();

                }
                return await DataSourceLoader.LoadAsync(datagridCL,
                    loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_customer.Where(o => o.id == Id),                    
                loadOptions);
        }

        //[HttpGet("GetCustomerByCode")]
        //public async Task<object> GetCustomerByCode(string Code)
        //{
        //    var record = await dbContext.vw_customer.Where(o => o.business_partner_code == Code).FirstOrDefaultAsync();
        //    if (record != null)
        //        return Ok(record);
        //    else
        //        return BadRequest("Record does not exist.");
        //}

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(customer),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new customer();
                    JsonConvert.PopulateObject(values, record);

                    record.id = Guid.NewGuid().ToString("N");
                    record.created_by = CurrentUserContext.AppUserId;
                    record.created_on = DateTime.Now;
                    record.modified_by = null;
                    record.modified_on = null;
                    //record.is_active = true;
                    record.is_default = null;
                    record.is_locked = null;
                    record.entity_id = null;
                    record.owner_id = CurrentUserContext.AppUserId;
                    record.organization_id = CurrentUserContext.OrganizationId;
					//record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                    record.is_customer = true;
                    record.is_government = null;
                    record.is_vendor = null;

                    #region Get Customer Number
                    var conn = dbContext.Database.GetDbConnection();
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                    }
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            try
                            {
                                cmd.CommandText = $"SELECT nextval('seq_customer_number')";
                                var r = await cmd.ExecuteScalarAsync();
                                record.business_partner_code = Convert.ToInt64(r).ToString("D6");
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.ToString());
                                return BadRequest(ex.Message);
                            }
                        }
                    }
                    #endregion

                    dbContext.customer.Add(record);
                    await dbContext.SaveChangesAsync();

                    var creditLimitActivation = record.credit_limit_activation ?? false;
                    string sql = $"UPDATE sales_contract SET credit_limit_activation = {creditLimitActivation} WHERE " +
                            $"customer_id = '{record.id}' ";
                    await dbContext.Database.ExecuteSqlRawAsync(sql);


                    var recordCreditLimitHistory = new credit_limit_history();
                    JsonConvert.PopulateObject(values, recordCreditLimitHistory);

                    recordCreditLimitHistory.id = Guid.NewGuid().ToString("N");
                    recordCreditLimitHistory.created_by = CurrentUserContext.AppUserId;
                    recordCreditLimitHistory.created_on = System.DateTime.Now;
                    recordCreditLimitHistory.modified_by = null;
                    recordCreditLimitHistory.modified_on = null;
                    recordCreditLimitHistory.is_active = true;
                    recordCreditLimitHistory.is_default = null;
                    recordCreditLimitHistory.is_locked = null;
                    recordCreditLimitHistory.entity_id = null;
                    recordCreditLimitHistory.owner_id = CurrentUserContext.AppUserId;
                    recordCreditLimitHistory.organization_id = CurrentUserContext.OrganizationId;
                    recordCreditLimitHistory.credit_limit_value = record.credit_limit;
                    recordCreditLimitHistory.customer_id = record.id;
                    dbContext.credit_limit_history.Add(recordCreditLimitHistory);

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
                var record = dbContext.customer
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var currentCreditLimit = record.credit_limit;
                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);
                        var is_active = record.is_active;
                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        record.is_active = is_active;
                        record.is_customer = true;
                        record.is_government = null;
                        record.is_vendor = null;

                        if (currentCreditLimit!=record.credit_limit)
                        {
                            var recordCreditLimitHistory = new credit_limit_history();
                            JsonConvert.PopulateObject(values, recordCreditLimitHistory);

                            recordCreditLimitHistory.id = Guid.NewGuid().ToString("N");
                            recordCreditLimitHistory.created_by = CurrentUserContext.AppUserId;
                            recordCreditLimitHistory.created_on = System.DateTime.Now;
                            recordCreditLimitHistory.modified_by = null;
                            recordCreditLimitHistory.modified_on = null;
                            recordCreditLimitHistory.is_active = true;
                            recordCreditLimitHistory.is_default = null;
                            recordCreditLimitHistory.is_locked = null;
                            recordCreditLimitHistory.entity_id = null;
                            recordCreditLimitHistory.owner_id = CurrentUserContext.AppUserId;
                            recordCreditLimitHistory.organization_id = CurrentUserContext.OrganizationId;
                            recordCreditLimitHistory.credit_limit_value = record.credit_limit;
                            recordCreditLimitHistory.customer_id = record.id;
                            dbContext.credit_limit_history.Add(recordCreditLimitHistory);
                        }

                        var creditLimitActivation = record.credit_limit_activation ?? false;
                        string sql = $"UPDATE sales_contract SET credit_limit_activation = {creditLimitActivation} WHERE " +
                                $"customer_id = '{key}' ";
                        await dbContext.Database.ExecuteSqlRawAsync(sql);

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
                    return BadRequest("Record does not exist.");
                }
            }
            catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var record = dbContext.customer
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.customer.Remove(record);
                        await dbContext.SaveChangesAsync();
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
        

        [HttpGet("CustomerIdLookup")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> CustomerIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.customer
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .OrderBy(o=>o.business_partner_name)
                    .Select(o => new { Value = o.id, Text = o.business_partner_name, search = o.business_partner_name.ToLower() + o.business_partner_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("GetCustomerById")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetCustomerById(string Id, DataSourceLoadOptions loadOptions)
        {
            var lookup = dbContext.customer
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Id);
            return await DataSourceLoader.LoadAsync(lookup, loadOptions);
        }

        [HttpGet("CustomerIdDespatchOrderBasedLookup")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> CustomerIdDespatchOrderBasedLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var customerIds = await dbContext.despatch_order
                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                    .Select(x => x.customer_id)
                    .Distinct().ToListAsync();

                var lookup = dbContext.customer
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Select(o => new { Value = o.id, Text = o.business_partner_name, search = o.business_partner_name.ToLower() + o.business_partner_name.ToUpper() })
                            .Where(e => customerIds.Contains(e.Value));

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

                    var customer_type_id = "";
                    var customer_type = dbContext.customer_type
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.customer_type_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower()).FirstOrDefault();
                    if (customer_type != null) customer_type_id = customer_type.id.ToString();

                    var country_id = "";
                    var country = dbContext.country
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.country_code == PublicFunctions.IsNullCell(row.GetCell(4))).FirstOrDefault();
                    if (country != null) country_id = country.id.ToString();

                    var bank_account_id = ""; var currency_id = "";
                    var bank_account = dbContext.bank_account
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.account_number == PublicFunctions.IsNullCell(row.GetCell(13))).FirstOrDefault();
                    if (bank_account != null)
                    {
                        bank_account_id = bank_account.id.ToString();
                        currency_id = bank_account.currency_id.ToString();
                    }

                    var CustomerCode = "";
                    if (PublicFunctions.IsNullCell(row.GetCell(1)) == "")
                    {
                        #region Get customer number
                        var conn = dbContext.Database.GetDbConnection();
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            await conn.OpenAsync();
                        }
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                try
                                {
                                    cmd.CommandText = $"SELECT nextval('seq_customer_number')";
                                    var r = await cmd.ExecuteScalarAsync();
                                    CustomerCode = Convert.ToInt64(r).ToString("D6");
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }
                        #endregion
                    }
                    else
                        CustomerCode = PublicFunctions.IsNullCell(row.GetCell(1));

                    var record = dbContext.customer
                        .Where(o => o.business_partner_code == CustomerCode
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.business_partner_name = PublicFunctions.IsNullCell(row.GetCell(0));
                        
                        record.customer_type_id = customer_type_id;
                        record.primary_address = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.country_id = country_id;
                        record.primary_contact_name = PublicFunctions.IsNullCell(row.GetCell(5));
                        record.primary_contact_email = PublicFunctions.IsNullCell(row.GetCell(6));
                        record.primary_contact_phone = PublicFunctions.IsNullCell(row.GetCell(7));
                        record.secondary_contact_name = PublicFunctions.IsNullCell(row.GetCell(8));
                        record.secondary_contact_email = PublicFunctions.IsNullCell(row.GetCell(9));
                        record.secondary_contact_phone = PublicFunctions.IsNullCell(row.GetCell(10));
                        record.additional_information = PublicFunctions.IsNullCell(row.GetCell(11));
                        record.bank_account_id = bank_account_id;
                        record.currency_id = currency_id;
                        record.is_active = PublicFunctions.BenarSalah(row.GetCell(16));
                        record.credit_limit = PublicFunctions.Desimal(row.GetCell(17));
                        record.credit_limit_activation = PublicFunctions.BenarSalah(row.GetCell(18));
                        record.alias_name = PublicFunctions.IsNullCell(row.GetCell(19));

                        await dbContext.SaveChangesAsync();

                        var creditLimitActivation = record.credit_limit_activation ?? false;
                        string sql = $"UPDATE sales_contract SET credit_limit_activation = {creditLimitActivation} WHERE " +
                                $"customer_id = '{record.id}' ";
                        await dbContext.Database.ExecuteSqlRawAsync(sql);
                    }
                    else
                    {
                        record = new customer();
                        record.id = Guid.NewGuid().ToString("N");
                        record.created_by = CurrentUserContext.AppUserId;
                        record.created_on = DateTime.Now;
                        record.modified_by = null;
                        record.modified_on = null;
                        //record.is_active = true;
                        record.is_default = null;
                        record.is_locked = null;
                        record.entity_id = null;
                        record.owner_id = CurrentUserContext.AppUserId;
                        record.organization_id = CurrentUserContext.OrganizationId;
					    //record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        record.business_partner_name = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.business_partner_code = CustomerCode;
                        record.customer_type_id = customer_type_id;
                        record.primary_address = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.country_id = country_id;
                        record.primary_contact_name = PublicFunctions.IsNullCell(row.GetCell(5));
                        record.primary_contact_email = PublicFunctions.IsNullCell(row.GetCell(6));
                        record.primary_contact_phone = PublicFunctions.IsNullCell(row.GetCell(7));
                        record.secondary_contact_name = PublicFunctions.IsNullCell(row.GetCell(8));
                        record.secondary_contact_email = PublicFunctions.IsNullCell(row.GetCell(9));
                        record.secondary_contact_phone = PublicFunctions.IsNullCell(row.GetCell(10));
                        record.additional_information = PublicFunctions.IsNullCell(row.GetCell(11));
                        record.bank_account_id = bank_account_id;
                        record.currency_id = currency_id;
                        record.is_active = PublicFunctions.BenarSalah(row.GetCell(16));
                        record.credit_limit = PublicFunctions.Desimal(row.GetCell(17));
                        record.credit_limit_activation = PublicFunctions.BenarSalah(row.GetCell(18));

                        dbContext.customer.Add(record);
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

            /*sheet = wb.GetSheetAt(1); //***** sheet 2
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var customer_id = "";
                    var customer = dbContext.customer
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()).FirstOrDefault();
                    if (customer != null) customer_id = customer.id.ToString();

                    var record = dbContext.customer_attachment
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.customer_id == customer_id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.filename = PublicFunctions.IsNullCell(row.GetCell(1));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new customer_attachment();
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
					    //record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        record.customer_id = customer_id;
                        record.filename = PublicFunctions.IsNullCell(row.GetCell(1));

                        dbContext.customer_attachment.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 2, Line " + (i+1) + " : " + Environment.NewLine;
                    }
                    else errormessage = ex.Message;

                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                    gagal = true;
                    break;
                }
            }
*/
            sheet = wb.GetSheetAt(1); //***** sheet 3
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var business_partner_id = "";
                    var customer = dbContext.customer
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()).FirstOrDefault();
                    if (customer != null) business_partner_id = customer.id.ToString();

                    var record = dbContext.contact
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.business_partner_id.ToLower() == business_partner_id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.contact_name = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.contact_email = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.contact_phone = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.contact_position = PublicFunctions.IsNullCell(row.GetCell(4));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new contact();
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
					    //record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        record.business_partner_id = business_partner_id;
                        record.contact_name = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.contact_email = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.contact_phone = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.contact_position = PublicFunctions.IsNullCell(row.GetCell(4));

                        dbContext.contact.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 3, Line " + (i+1) + " : " + Environment.NewLine;
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
                HttpContext.Session.SetString("filename", "Customer");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpPost("UploadCustomerPayment")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UploadCustomerPayment([FromBody] dynamic FileDocument)
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

                    var sales_invoice = dbContext.vw_sales_invoice
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.invoice_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower())
                        .FirstOrDefault();
                    if (sales_invoice == null)
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Invoice Number not found!" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        break;
                    }

                    var currency_id = "";
                    var currency_code = "";
                    var currency = dbContext.currency
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.currency_code == PublicFunctions.IsNullCell(row.GetCell(3))).FirstOrDefault();
                    if (currency != null)
                    {
                        currency_id = currency.id.ToString();
                        currency_code = currency.currency_code.ToString();
                    }

                    var record = dbContext.sales_invoice_payment
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sales_invoice_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower() &&
                            o.payment_date.Value.Date == PublicFunctions.Tanggal(row.GetCell(1)).Date &&
                            o.currency_id == currency_id)
                        .FirstOrDefault();
                    if (record == null)
                    {
                        var newRecord = new sales_invoice_payment();

                        newRecord.id = Guid.NewGuid().ToString("N");
                        newRecord.created_by = CurrentUserContext.AppUserId;
                        newRecord.created_on = DateTime.Now;
                        newRecord.modified_by = null;
                        newRecord.modified_on = null;
                        newRecord.is_active = true;
                        newRecord.is_default = null;
                        newRecord.is_locked = null;
                        newRecord.entity_id = null;
                        newRecord.owner_id = CurrentUserContext.AppUserId;
                        newRecord.organization_id = CurrentUserContext.OrganizationId;
                        newRecord.business_unit_id = sales_invoice.business_unit_id;

                        newRecord.sales_invoice_number = PublicFunctions.IsNullCell(row.GetCell(0));
                        newRecord.payment_date = PublicFunctions.Tanggal(row.GetCell(1));
                        newRecord.payment_value = PublicFunctions.Desimal(row.GetCell(2));
                        newRecord.currency_id = currency_id;
                        newRecord.currency_code = currency_code;

                        dbContext.sales_invoice_payment.Add(newRecord);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        record.payment_value = PublicFunctions.Desimal(row.GetCell(2));

                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 1, Line " + (i + 1) + " : " + Environment.NewLine;
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
                HttpContext.Session.SetString("filename", "Customer");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] customer Record)
        {
            try
            {
                var record = dbContext.customer
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
                else if (await mcsContext.CanCreate(dbContext, nameof(customer),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new customer();
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
					//record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    dbContext.customer.Add(record);
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

        [HttpGet("CreditLimitHistory")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> CreditLimitHistory(string customer_id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_credit_limit_history.Where(o => o.customer_id == customer_id),
                    loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<object> CountCreditLimit(string customer_id)
        {
            List<CreditLimitData> retval = new List<CreditLimitData>();
            decimal remainedCreditLimit = 0;
            decimal customerCreditLimit = 0;
            try
            {
                List<Task> tasks = new List<Task>();
                var customer_data = await dbContext.customer
                    .Where(o => o.id == customer_id)
                    .Select(o => o.credit_limit).FirstOrDefaultAsync();
                //foreach (decimal v in customer_data)
                //{
                //    customerCreditLimit += (decimal)v;
                //}
                customerCreditLimit = customer_data ?? 0;

                // find all sales contract within the same customer
                var vw_sales_invoice_custid = dbContext.vw_sales_invoice
                    .Where(o => o.bill_to == customer_id || o.customer_id == customer_id);
                var array_vw_sales_invoice_custid = await vw_sales_invoice_custid.ToArrayAsync();
                decimal totalPaymentfromAllInvoice = 0;
                List<vw_sales_invoice_payment> vw_sales_invoice_payment = await dbContext.vw_sales_invoice_payment.ToListAsync();

                decimal totalPricefromAllInvoice = 0;
                if (array_vw_sales_invoice_custid.Length > 0)
                {
                    foreach (vw_sales_invoice item1 in array_vw_sales_invoice_custid)
                    {
                        await Task.Run(() =>
                        {
                            if (item1.total_price != null)
                            {
                                var exchangeRate = ((decimal?)item1.exchange_rate == null ? (decimal)1 : (decimal?)item1.exchange_rate);
                                totalPricefromAllInvoice += (decimal)item1.total_price * (decimal)exchangeRate;
                            }
                        });
                        
                        var data = vw_sales_invoice_payment.Where(o => o.sales_invoice_number == item1.invoice_number);
                        var array_vw_sales_invoice_payment_data = data.ToArray();
                        if (array_vw_sales_invoice_payment_data.Length > 0)
                        {
                            tasks.Add(Task.Run(() =>
                            {
                                foreach (vw_sales_invoice_payment itemX in array_vw_sales_invoice_payment_data)
                                {
                                    if (itemX.payment_value != null)
                                    {
                                        totalPaymentfromAllInvoice += (decimal)itemX.payment_value;
                                    }
                                }
                            }));
                            await Task.WhenAll(tasks);
                        }
                    }
                }
                remainedCreditLimit = customerCreditLimit - totalPricefromAllInvoice + totalPaymentfromAllInvoice;
                retval.Add(new CreditLimitData(customerCreditLimit, remainedCreditLimit));
                return retval;

            }
            catch (Exception ex)
            {
                logger.Debug($"exception error message = {ex.Message}");
                return retval;
            }
        }

        [HttpGet("RemainedCreditLimit")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public object RemainedCreditLimit(string customer_id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                decimal remainedCreditLimit = 0;
                List<CreditLimitData> varCreditLimitData = (List<CreditLimitData>)CountCreditLimit(customer_id).Result;
                if (varCreditLimitData.Count > 0)
                    remainedCreditLimit = varCreditLimitData[0].RemainedCreditLimit;
                return Ok(remainedCreditLimit);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CreditLimitAlert")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public object CreditLimitAlert(string customer_id, DataSourceLoadOptions loadOptions)
        {
            List<AlertCL> retval = new List<AlertCL>();
            try
            {
                List<CreditLimitData> varCreditLimitData = (List<CreditLimitData>)CountCreditLimit(customer_id).Result;
                decimal remainedCreditLimit = varCreditLimitData[0].RemainedCreditLimit;
                decimal customerCreditLimit = varCreditLimitData[0].InitialCreditLimit;

                decimal percentage = 1;

                if (customerCreditLimit > 0)
                {
                    percentage = remainedCreditLimit / customerCreditLimit;
                }
                var color = "green";
                var status = "safe";

                if (percentage < (decimal)0.2)
                {
                    color = "red";
                    status = "limit";
                }

                if (percentage < (decimal)0.5)
                {
                    color = "yellow";
                    status = "warning";
                }

                percentage = percentage * 100;

                retval.Add(new AlertCL(status, color, percentage));
                return Ok(retval);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("InvoiceOverdueAlert")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public object InvoiceOverdueAlert(string customer_id, DataSourceLoadOptions loadOptions)
        {
            List<AlertCL> retval = new List<AlertCL>();
            try
            {
                var invoice_overdue = dbContext.vw_invoice_overdue
               .Where(o => o.customer_id == customer_id && 
                   o.sales_invoice_payment == null && 
                   o.overdue_day < 0);

                //List<CreditLimitData> varCreditLimitData = (List<CreditLimitData>)CountCreditLimit(customer_id).Result;
                //decimal remainedCreditLimit = varCreditLimitData[0].RemainedCreditLimit;
                //decimal customerCreditLimit = varCreditLimitData[0].InitialCreditLimit;

                int countInvoice = invoice_overdue.Count();
    
                //decimal countInvoice = 1;

                //if (customerCreditLimit > 0)
                //{
                //    percentage = remainedCreditLimit / customerCreditLimit;
                //}
                var color = "green";
                var status = "safe";

                if (countInvoice > 0)
                {
                    color = "red";
                    status = "limit";
                }

                //if (percentage < (decimal)0.5)
                //{
                //    color = "yellow";
                //    status = "warning";
                //}

                //percentage = percentage * 100;

                retval.Add(new AlertCL(status, color, countInvoice));
                return Ok(retval);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("CustomerTransactionHistory")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> CustomerTransactionHistory(string Id, DataSourceLoadOptions loadOptions)
        //{
        //    List<CustomerInvoicePaymentHistory> retval = new List<CustomerInvoicePaymentHistory>();
        //    try
        //    {
        //        var invoice_data = dbContext.vw_details_customer_invoice_history
        //            .Where(o => o.customer_id == Id )
        //            ;
        //        var array_invoice_data = await invoice_data.ToArrayAsync();
        //        decimal? currentCreditLimit = 0;
        //        if (array_invoice_data.Length > 0)
        //        {
        //            currentCreditLimit = array_invoice_data[0].credit_limit;
        //        }
        //        foreach (vw_details_customer_invoice_history v in array_invoice_data)
        //        {
        //            var temp1 = new CustomerInvoicePaymentHistory(v);
        //            currentCreditLimit = currentCreditLimit - temp1.billing + temp1.receipt;
        //            temp1.outstanding = currentCreditLimit;
        //            retval.Add(temp1);

        //            var payment_data = dbContext.vw_details_customer_payment_history
        //                .Where(o => o.invoice_number == v.invoice_number)
        //                ;
        //            var array_payment_data = await payment_data.ToArrayAsync();

        //            foreach (vw_details_customer_payment_history w in array_payment_data)
        //            {
        //                temp1 = new CustomerInvoicePaymentHistory(w);
        //                currentCreditLimit = currentCreditLimit - temp1.billing + temp1.receipt;
        //                temp1.outstanding = currentCreditLimit;
        //                temp1.paydate = w.tdate;
        //                retval.Add(temp1);
        //            }
        //        }
        //        return retval;

        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Trace(ex.Message);
        //        return retval;
        //    }
        //}

        [HttpGet("CustomerTransactionHistory")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> CustomerTransactionHistory(string Id, DataSourceLoadOptions loadOptions)
        {
            List<CustomerInvoicePaymentHistory> retval = new List<CustomerInvoicePaymentHistory>();
            try
            {
                var invoice_data = dbContext.vw_details_customer_invoice_history
                    .Where(o => o.customer_id == Id).OrderBy(o => o.bill_lading_date); 
                var array_invoice_data = await invoice_data.ToArrayAsync();
                decimal? currentCreditLimit = 0;

                foreach (vw_details_customer_invoice_history v in array_invoice_data)
                {
                    var temp1 = new CustomerInvoicePaymentHistory(v);
                    decimal? lastCreditLimit = currentCreditLimit - temp1.billing + temp1.receipt;
                    temp1.outstanding = lastCreditLimit;

                    var payment_data = dbContext.vw_details_customer_payment_history
                        .Where(o => o.invoice_number == v.invoice_number);

                    var array_payment_data = await payment_data.ToArrayAsync();

                    foreach (vw_details_customer_payment_history w in array_payment_data)
                    {
                        var temp2 = new CustomerInvoicePaymentHistory(w);
                        lastCreditLimit = lastCreditLimit + temp2.receipt;
                        temp1.outstanding = lastCreditLimit;
                        temp1.paydate = w.tdate;
                        temp1.receipt = temp1.receipt + temp2.receipt;
                    }

                    retval.Add(temp1);
                }
                return retval;
            }
            catch (Exception ex)
            {
                logger.Trace(ex.Message);
                return retval;
            }
        }

        [HttpGet("GetCustomerNumber")]
        public async Task<object> GetCustomerNumber()
        {
            var organization = dbContext.organization
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .FirstOrDefault();
            var organizationCode = organization.organization_code;
            var sequenceName = "";
            switch (organizationCode)
            {
                case "IN01":
                    sequenceName = "seq_customer_number_ic";
                    break;
                case "KM01":
                    sequenceName = "seq_customer_number_kmia";
                    break;
                default:
                    sequenceName = "seq_customer_number";
                    break;
            }

            var conn = dbContext.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            if (conn.State == System.Data.ConnectionState.Open)
            {
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = string.Format("SELECT nextval('{0}')", sequenceName);
                        var r = await cmd.ExecuteScalarAsync();
                        return r;
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
            }

            return null;
        }

        [HttpPut("ChangeCustomerNumber")]
        public async Task<object> ChangeCustomerNumber(string SequenceNumber)
        {
            int i = 1;
            bool result = int.TryParse(SequenceNumber, out i);

            try
            {
                var organization = dbContext.organization
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                var organizationCode = organization.organization_code;
                var sequenceName = "";
                switch (organizationCode)
                {
                    case "IN01":
                        sequenceName = "seq_customer_number_ic";
                        break;
                    case "KM01":
                        sequenceName = "seq_customer_number_kmia";
                        break;
                    default:
                        sequenceName = "seq_customer_number";
                        break;
                }

                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = string.Format("ALTER SEQUENCE {0} RESTART WITH {1}", sequenceName, i);
                            await cmd.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                }

                return "Success";

            }
            catch (Exception ex)
            {
                return "Update failed";
            }
        }
    }

    public class AlertCL
    {
        public decimal Persentase { get; set; }
        public string Status { get; set; }
        public string Color { get; set; }
        public AlertCL()
        {
            Status = "name";
            Persentase = 0;
            Color = "code";
        }
        public AlertCL(string vStatus, string vColor, decimal vPercen)
        {
            Status = vStatus;
            Persentase = vPercen;
            Color = vColor;
        }
    }

    public partial class CustomerInvoicePaymentHistory
    {
        public string customer_id { get; set; }
        public string business_partner_name { get; set; }
        public DateTime? tdate { get; set; }
        public string invoice_number { get; set; }
        public decimal? billing { get; set; }
        public decimal? receipt { get; set; }
        public decimal? outstanding { get; set; }
        public string currency { get; set; }
        public string despatch_order_number { get; set; }
        public string sales_contract_name { get; set; }
        public string contract_term_name { get; set; }
        public string despatch_plan_name { get; set; }
        public decimal? credit_limit { get; set; }
        public DateTime? paydate { get; set; }
        public DateTime? bl_date { get; set; }

        public CustomerInvoicePaymentHistory(vw_details_customer_payment_history input)
        {
            customer_id = input.customer_id;
            business_partner_name = input.business_partner_name;
            tdate = input.tdate;
            invoice_number = input.invoice_number;
            billing = (decimal)input.billing;
            receipt = (decimal)input.receipt;
            outstanding = (decimal)input.outstanding;
            currency = input.currency;
            despatch_order_number = input.despatch_order_number;
            sales_contract_name = input.sales_contract_name;
            contract_term_name = input.contract_term_name;
            despatch_plan_name = input.despatch_plan_name;
            credit_limit = input.credit_limit;
            paydate = input.tdate;
            bl_date = null;
        }

        public CustomerInvoicePaymentHistory(vw_details_customer_invoice_history input)
        {
            customer_id = input.customer_id;
            business_partner_name = input.business_partner_name;
            tdate = input.tdate;
            invoice_number = input.invoice_number;
            billing = (decimal)(input.billing ?? 0);
            receipt = (decimal)(input.receipt ?? 0);
            outstanding = (decimal)input.outstanding;
            currency = input.currency;
            despatch_order_number = input.despatch_order_number;
            sales_contract_name = input.sales_contract_name ?? "";
            contract_term_name = input.contract_term_name ?? "";
            despatch_plan_name = input.despatch_plan_name ?? "";
            credit_limit = input.credit_limit;
            paydate = null;
            bl_date = input.bill_lading_date;
        }
    }
}

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
using System.Dynamic;

namespace MCSWebApp.Controllers.API.ContractManagement
{
    [Route("api/ContractManagement/[controller]")]
    [ApiController]
    public class AdvanceContractController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public AdvanceContractController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_advance_contract
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_advance_contract.Where(o => o.id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)),
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
                    if (await mcsContext.CanCreate(dbContext, nameof(advance_contract),
                                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new advance_contract();
                        JsonConvert.PopulateObject(values, record);

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

                        #region Get transaction number

                        if (string.IsNullOrEmpty(record.advance_contract_number))
                        {
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
                                        cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                        var r = await cmd.ExecuteScalarAsync();
                                        record.advance_contract_number = $"AC-{DateTime.Now:yyyyMMdd}-{r}";
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex.ToString());
                                        return BadRequest(ex.Message);
                                    }
                                }
                            }
                        }

                        #endregion

                        if (IsContractNumberExist("", record.advance_contract_number))
                        {
                            return BadRequest("Advance Contract Number already exist");
                        }

                        dbContext.advance_contract.Add(record);

                        var resRef = true;
                        //try
                        //{
                        //    for (var startDate = record.start_date; startDate <= record.end_date; startDate = startDate.AddMonths(1))
                        //    {
                        //        var newRecord = new advance_contract_reference();
                        //        newRecord.id = Guid.NewGuid().ToString("N");
                        //        newRecord.created_by = CurrentUserContext.AppUserId;
                        //        newRecord.created_on = DateTime.Now;
                        //        newRecord.modified_by = null;
                        //        newRecord.modified_on = null;
                        //        newRecord.is_active = true;
                        //        newRecord.is_default = null;
                        //        newRecord.is_locked = null;
                        //        newRecord.entity_id = null;
                        //        newRecord.owner_id = CurrentUserContext.AppUserId;
                        //        newRecord.organization_id = CurrentUserContext.OrganizationId;

                        //        newRecord.advance_contract_id = record.id;
                        //        newRecord.start_date = new DateTime(startDate.Year, startDate.Month, 1);
                        //        newRecord.end_date = LastDayOfMonth(newRecord.start_date.Value);
                        //        var postfix = startDate.Month < 10 ? "0" + startDate.Month.ToString() : startDate.Month.ToString();
                        //        newRecord.progress_claim_name = record.advance_contract_number + "-" + postfix;

                        //        dbContext.advance_contract_reference.Add(newRecord);
                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    logger.Error(ex.ToString());
                        //    resRef = false;
                        //}

                        if (resRef)
                        {
                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                        }
                        else
                        {
                            return BadRequest(resRef);
                        }

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
                    var record = await dbContext.advance_contract
                        .Where(o => o.id == key)
                        .FirstOrDefaultAsync();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            var e = new entity();
                            e.InjectFrom(record);

                            JsonConvert.PopulateObject(values, record);

                            record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            if (IsContractNumberExist(record.id, record.advance_contract_number))
                            {
                                return BadRequest("Advance Contract Number already exist");
                            }

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
                    var record = await dbContext.advance_contract
                        .Where(o => o.id == key)
                        .FirstOrDefaultAsync();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.advance_contract.Remove(record);

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok();
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

        [HttpGet("AdvanceContractIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AdvanceContractIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.advance_contract
                    .OrderBy(o => o.advance_contract_number)
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.advance_contract_number + (" - " + o.note ?? "") });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ContractorLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ContractorLookup(string ContractType, DataSourceLoadOptions loadOptions)
        {
            try
            {
                //var lookup = dbContext.contractor
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                //    .Select(o => new { Value = o.id, Text = o.business_partner_name });
                //return await DataSourceLoader.LoadAsync(lookup, loadOptions);

                if (ContractType == "AR")
                {
                    var lookup = dbContext.customer
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .OrderBy(o => o.business_partner_code)
                        .Select(o => new
                        {
                            Value = o.id,
                            Text = (String.IsNullOrEmpty(o.business_partner_code) ? "" : o.business_partner_code + " - ")
                                + (o.business_partner_name ?? ""),
                            o.business_partner_name
                        });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.contractor
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .OrderBy(o => o.business_partner_code)
                        .Select(o => new
                        {
                            Value = o.id,
                            Text = (String.IsNullOrEmpty(o.business_partner_code) ? "" : o.business_partner_code + " - ")
                                + (o.business_partner_name ?? ""),
                            o.business_partner_name
                        });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ContractorById")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ContractorById(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.contractor.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Id)
                    .Select(o => new { o.id, o.business_partner_name })
                .Union(
                    dbContext.customer.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Id)
                        .Select(o => new { o.id, o.business_partner_name })),
                loadOptions);
        }

        [HttpGet("ContractTypeLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public object ContractTypeLookup()
        {
            var result = new List<dynamic>();
            try
            {
                foreach (var item in Common.Constants.ContractTypes)
                {
                    dynamic obj = new ExpandoObject();
                    obj.value = item;
                    obj.text = item;
                    result.Add(obj);
                }
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }

            return result;
        }

        [HttpGet("List")]
        public async Task<IActionResult> List([FromQuery] string q)
        {
            try
            {
                var rows = dbContext.vw_advance_contract.AsNoTracking();
                rows = rows.Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin);
                if (!string.IsNullOrEmpty(q))
                {
                    rows = rows.Where(o => o.advance_contract_number.Contains(q));
                }

                return Ok(await rows.ToListAsync());
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.vw_advance_contract
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if(record != null)
                {
                    if (await mcsContext.CanRead(dbContext, Id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
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

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] advance_contract Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.advance_contract
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
                            await tx.CommitAsync();
                            return Ok(record);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else if (await mcsContext.CanCreate(dbContext, nameof(advance_contract),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new advance_contract();
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

                        #region Get transaction number

                        if (string.IsNullOrEmpty(record.advance_contract_number))
                        {
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
                                        cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                        var r = await cmd.ExecuteScalarAsync();
                                        record.advance_contract_number = $"AC-{DateTime.Now:yyyyMMdd}-{r}";
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex.ToString());
                                        return BadRequest(ex.Message);
                                    }
                                }
                            }
                        }

                        #endregion

                        dbContext.advance_contract.Add(record);
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

        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Debug($"string Id = {Id}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.advance_contract
                        .Where(o => o.id == Id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, Id, CurrentUserContext.AppUserId)
                                    || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.advance_contract.Remove(record);
                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok();
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

        private DateTime LastDayOfMonth(DateTime inDate)
        {
            var daysInMonth = DateTime.DaysInMonth(inDate.Year, inDate.Month);
            return new DateTime(inDate.Year, inDate.Month, daysInMonth);
        }

        private bool IsContractNumberExist(string id, string contract_number)
        {
            var record = dbContext.vw_advance_contract
                    .Where(r => r.advance_contract_number.Trim().ToLower() == contract_number.Trim().ToLower() &&
                           r.organization_id == CurrentUserContext.OrganizationId &&
                           r.id != id)
                    .FirstOrDefault();
            if (record != null)
                return true;
            else
                return false;
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
                    if (row.Cells.Count() < 8) continue;

                    string contractor_id = "";
                    if (PublicFunctions.IsNullCell(row.GetCell(4)).Trim() == "AR")
                    {
                        var customer = dbContext.customer
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower()).FirstOrDefault();
                        if (customer != null) contractor_id = customer.id.ToString();
                    }
                    else
                    {
                        var contractor = dbContext.contractor
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower()).FirstOrDefault();
                        if (contractor != null) contractor_id = contractor.id.ToString();
                    }

                    string currency_id = "";
                    var currency = dbContext.currency
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.currency_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(9)).ToLower()).FirstOrDefault();
                    if (currency != null) currency_id = currency.id.ToString();

                    var record = dbContext.advance_contract
                        .Where(o => o.advance_contract_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.version = PublicFunctions.Bulat(row.GetCell(1));
                        record.note = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.reference_number = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.contract_type = PublicFunctions.IsNullCell(row.GetCell(4));
                        record.contractor_id = contractor_id;
                        record.start_date = Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(6)));
                        record.end_date = Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(7)));
                        record.contract_value = PublicFunctions.Desimal(row.GetCell(8));
                        record.contract_currency_id = currency_id;

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new advance_contract();
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

                        //#region Get transaction number
                        //var conn = dbContext.Database.GetDbConnection();
                        //if (conn.State != System.Data.ConnectionState.Open)
                        //{
                        //    await conn.OpenAsync();
                        //}
                        //if (conn.State == System.Data.ConnectionState.Open)
                        //{
                        //    using (var cmd = conn.CreateCommand())
                        //    {
                        //        try
                        //        {
                        //            cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                        //            var r = await cmd.ExecuteScalarAsync();
                        //            record.transaction_number = $"PD-{DateTime.Now:yyyyMMdd}-{r}";
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            logger.Error(ex.ToString());
                        //            return BadRequest(ex.Message);
                        //        }
                        //    }
                        //}
                        //#endregion

                        record.advance_contract_number = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.version = PublicFunctions.Bulat(row.GetCell(1));
                        record.note = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.reference_number = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.contract_type = PublicFunctions.IsNullCell(row.GetCell(4));
                        record.contractor_id = contractor_id;
                        record.start_date = Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(6)));
                        record.end_date = Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(7)));
                        record.contract_value = PublicFunctions.Desimal(row.GetCell(8));
                        record.contract_currency_id = currency_id;

                        dbContext.advance_contract.Add(record);
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
                HttpContext.Session.SetString("filename", "AdvanceContract");
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

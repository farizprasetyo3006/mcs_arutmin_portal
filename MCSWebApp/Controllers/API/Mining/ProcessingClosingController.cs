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
using BusinessLogic;
using Omu.ValueInjecter;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Common;

namespace MCSWebApp.Controllers.API.Mining
{
    [Route("api/Mining/[controller]")]
    [ApiController]
    public class ProcessingClosingController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ProcessingClosingController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.processing_closing
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("ProcessingByDate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProcessingByDate(string FromDate, string ToDate, DataSourceLoadOptions loadOptions)
        {
            if (FromDate == "null" || ToDate == "null")
                return null;

            FromDate = FromDate.Replace("T", " ");
            ToDate = ToDate.Replace("T", " ");

            var dt1 = DateTime.Parse(FromDate);
            var dt2 = DateTime.Parse(ToDate);

            return await DataSourceLoader.LoadAsync(dbContext.processing_transaction
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                       o.loading_datetime >= Convert.ToDateTime(dt1) &&
                       o.loading_datetime <= Convert.ToDateTime(dt2)),
                loadOptions);
        }

        //[HttpGet("ProductionTotal")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> ProductionTotal(DataSourceLoadOptions loadOptions, string FromDate, string ToDate)
        //{
        //    logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

        //    try
        //    {
        //        if (FromDate == "null" || ToDate == "null")
        //            return null;

        //        var dt1 = DateTime.Parse(FromDate);
        //        var dt2 = DateTime.Parse(ToDate);

        //        var lookup = dbContext.processing_transaction
        //            .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                   o.unloading_datetime >= Convert.ToDateTime(dt1) &&
        //                   o.unloading_datetime <= Convert.ToDateTime(dt2))
        //            .GroupBy(o => o.organization_id)
        //            .Select(o =>
        //            new
        //            {
        //                unloading_quantity = o.Sum(p => p.unloading_quantity),
        //                distance = o.Average(p => p.distance)
        //            });
        //        return await DataSourceLoader.LoadAsync(lookup, loadOptions);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex.InnerException ?? ex);
        //        return BadRequest(ex.InnerException?.Message ?? ex.Message);
        //    }
        //}

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.processing_closing
                .Where(o => o.id == Id &&
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
                    if (await mcsContext.CanCreate(dbContext, nameof(processing_closing),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new processing_closing();
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
                        //record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        dbContext.processing_closing.Add(record);
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
                    if (ex.InnerException != null)
                    {
                        logger.Error(ex.InnerException.Message);
                        return BadRequest(ex.InnerException.Message);
                    }
                    else
                    {
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
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
                    var record = await dbContext.processing_closing
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

                            //record.InjectFrom(e);
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
                    if (ex.InnerException != null)
                    {
                        logger.Error(ex.InnerException.Message);
                        return BadRequest(ex.InnerException.Message);
                    }
                    else
                    {
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
                }
            }
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Trace($"string key = {key}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.processing_closing
                        .Where(o => o.id == key)
                        .FirstOrDefaultAsync();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.processing_closing.Remove(record);
                            await dbContext.SaveChangesAsync();

                            #region Delete Items

                            var itemList = await dbContext.processing_closing_item
                                .Where(x => x.processing_closing_id == key)
                                .ToListAsync();

                            foreach (var itemRecord in itemList)
                            {
                                dbContext.processing_closing_item.Remove(itemRecord);
                                await dbContext.SaveChangesAsync();
                            }

                            #endregion

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
                    if (ex.InnerException != null)
                    {
                        logger.Error(ex.InnerException.Message);
                        return BadRequest(ex.InnerException.Message);
                    }
                    else
                    {
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
                }
            }
        }

        [HttpGet("SourceLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SourceLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                var lookup = dbContext.vw_processing_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .GroupBy(p => p.source_location_id)
                    .Select(o =>
                        new
                        {
                            value = o.Max(p => p.source_location_id),
                            text = o.Max(p => p.source_location_name)
                        });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DestinationLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DestinationLocationIdLookup(string ProcessFlowId, DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                var lookup = dbContext.vw_processing_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .GroupBy(p => p.destination_location_id)
                    .Select(o =>
                        new
                        {
                            value = o.Max(p => p.destination_location_id),
                            text = o.Max(p => p.destination_location_name)
                        });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("List")]
        public async Task<IActionResult> List([FromQuery] string q)
        {
            try
            {
                var rows = dbContext.processing_closing.AsNoTracking();
                rows = rows.Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin);
                if (!string.IsNullOrEmpty(q))
                {
                    rows = rows.Where(o => o.note.Contains(q));
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
                var record = await dbContext.processing_closing
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if (record != null)
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
        public async Task<IActionResult> SaveData([FromBody] processing_closing Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.processing_closing
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
                    else if (await mcsContext.CanCreate(dbContext, nameof(processing_closing),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new processing_closing();
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

                        dbContext.processing_closing.Add(record);
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
                    if (ex.InnerException != null)
                    {
                        logger.Error(ex.InnerException.Message);
                        return BadRequest(ex.InnerException.Message);
                    }
                    else
                    {
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
                }
            }
        }

        [HttpDelete("DeleteById/{Id}")]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Trace($"string Id = {Id}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.processing_closing
                        .Where(o => o.id == Id)
                        .FirstOrDefaultAsync();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, Id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.processing_closing.Remove(record);
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
                    if (ex.InnerException != null)
                    {
                        logger.Error(ex.InnerException.Message);
                        return BadRequest(ex.InnerException.Message);
                    }
                    else
                    {
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
                }
            }
        }

        [HttpGet("DataGridItem")]
        public async Task<object> DataGridItem(string id)
        {
            var result = await dbContext.vw_processing_closing_item
                .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                .Where(x => x.processing_closing_id == id)
                .ToListAsync();
            return result;
        }

        [HttpPost("InsertItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertItemData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    var record = new processing_closing_item();
                    JsonConvert.PopulateObject(values, record);

                    var header = await dbContext.processing_closing
                    .Where(x => x.id == record.processing_closing_id).FirstOrDefaultAsync();

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
                    record.business_unit_id = header.business_unit_id;

                    dbContext.processing_closing_item.Add(record);
                    await dbContext.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Ok(record);

                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        logger.Error(ex.InnerException.Message);
                        return BadRequest(ex.InnerException.Message);
                    }
                    else
                    {
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
                }
            }
        }

        [HttpPut("UpdateItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateItemData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.processing_closing_item
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
                    if (ex.InnerException != null)
                    {
                        logger.Error(ex.InnerException.Message);
                        return BadRequest(ex.InnerException.Message);
                    }
                    else
                    {
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
                }
            }
        }

        [HttpDelete("DeleteItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteItemData([FromForm] string key)
        {
            logger.Trace($"string key = {key}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.processing_closing_item
                        .Where(o => o.id == key)
                        .FirstOrDefaultAsync();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.processing_closing_item.Remove(record);
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
                    if (ex.InnerException != null)
                    {
                        logger.Error(ex.InnerException.Message);
                        return BadRequest(ex.InnerException.Message);
                    }
                    else
                    {
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
                }
            }
        }

        //Custom Lookups
        [HttpGet("GetMineLocation")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetMineLocation(string businessAreaId, DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (string.IsNullOrEmpty(businessAreaId))
                    return await DataSourceLoader.LoadAsync(
                        dbContext.mine_location
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                        .Select(x => new { Text = x.stock_location_name, Value = x.id }), loadOptions);
                else
                    return await DataSourceLoader.LoadAsync(
                        dbContext.mine_location
                        .Where(x => x.business_area_id == businessAreaId)
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                        .Select(x => new { Text = x.stock_location_name, Value = x.id }), loadOptions);
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
            bool gagal = false;
            string errormessage = "";

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            string lastTxNumber = string.Empty;
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var accounting_period_id = "";
                    var accounting_period = await dbContext.accounting_period
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.accounting_period_name.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).Trim().ToLower())
                        .FirstOrDefaultAsync();
                    if (accounting_period != null)
                        accounting_period_id = accounting_period.id.ToString();

                    var advance_contract_id = "";
                    var advance_contract = await dbContext.advance_contract
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.advance_contract_number.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).Trim().ToLower())
                        .FirstOrDefaultAsync();
                    if (advance_contract != null)
                        advance_contract_id = advance_contract.id.ToString();

                    var business_unit_id = string.Empty;
                    var business_unit = await dbContext.business_unit
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId
                        && x.business_unit_code.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(8)).Trim().ToLower())
                        .FirstOrDefaultAsync();
                    if (business_unit != null)
                        business_unit_id = business_unit.id.ToString();

                    processing_closing record;

                    record = await dbContext.processing_closing
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.transaction_number.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).Trim().ToLower())
                        .FirstOrDefaultAsync();

                    var transaction_number = PublicFunctions.IsNullCell(row.GetCell(0)).Trim();
                    if (transaction_number == null && record == null)
                    {
                        var baseTransaction = string.Empty;
                        var business_unit_code = PublicFunctions.IsNullCell(row.GetCell(8)).Trim();
                        var moduleName = "CP";
                        var monthTransaction = Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(1))).Month;
                        var yearTransaction = Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(1))).Year;
                        baseTransaction = $"RN-{business_unit_code}/{moduleName}/{yearTransaction}/{monthTransaction}";
                        transaction_number = baseTransaction;
                    }

                    if (lastTxNumber != transaction_number)
                        await CreateOrUpdateHeader(record, row, accounting_period_id, advance_contract_id, business_unit_id, transaction_number);
                    await CreateItems(record, row, accounting_period_id, advance_contract_id, business_unit_id, transaction_number);

                    lastTxNumber = record.transaction_number;
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
                HttpContext.Session.SetString("filename", "DayWork");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        private async Task CreateItems(processing_closing record, IRow row, string accounting_period_id, string advance_contract_id, string business_unit_id, string transaction_number)
        {
            var mine_location_id = string.Empty;
            var mine_location_code = PublicFunctions.IsNullCell(row.GetCell(12));
            var mine_location = await dbContext.mine_location
                .Where(x => x.mine_location_code == mine_location_code)
                .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                .FirstOrDefaultAsync();
            if (mine_location != null)
                mine_location_id = mine_location.id;

            var business_area_id = string.Empty;
            var business_area_code = PublicFunctions.IsNullCell(row.GetCell(13));
            var business_area = await dbContext.business_area
                .Where(x => x.business_area_code == business_area_code)
                .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                .FirstOrDefaultAsync();
            if (business_area != null)
                business_area_id = business_area.id;
            else if (mine_location != null)
                business_area_id = mine_location.business_area_id;
            else
                business_area_id = string.Empty;

            processing_closing_item itemDat = new processing_closing_item
            {
                id = Guid.NewGuid().ToString("N"),
                created_by = CurrentUserContext.AppUserId,
                created_on = DateTime.Now,
                is_active = true,
                owner_id = CurrentUserContext.AppUserId,
                organization_id = CurrentUserContext.OrganizationId,
                business_unit_id = business_unit_id,
                processing_closing_id = record.id,
                transaction_item_date = PublicFunctions.Tanggal(row.GetCell(11)),
                mine_location_id = mine_location_id,
                business_area_id = business_area_id,
                quantity_item = PublicFunctions.Desimal(row.GetCell(14))
            };

            dbContext.processing_closing_item.Add(itemDat);
            await dbContext.SaveChangesAsync();
        }

        private async Task CreateOrUpdateHeader(processing_closing record, IRow row,
        string accounting_period_id, string advance_contract_id, string business_unit_id, string transaction_number)
        {
            if (record != null)
            {
                record.modified_by = CurrentUserContext.AppUserId;
                record.modified_on = DateTime.Now;
                record.accounting_period_id = accounting_period_id;
                record.advance_contract_id = advance_contract_id;
                record.from_date = PublicFunctions.Tanggal(row.GetCell(4));
                record.to_date = PublicFunctions.Tanggal(row.GetCell(5));
                record.volume = PublicFunctions.Desimal(row.GetCell(6));
                record.distance = PublicFunctions.Desimal(row.GetCell(7));
                record.business_unit_id = business_unit_id;
                record.note = PublicFunctions.IsNullCell(row.GetCell(9));
                await dbContext.SaveChangesAsync();
            }
            else
            {
                record = new processing_closing();
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
                record.business_unit_id = business_unit_id;

                record.transaction_number = transaction_number;
                record.accounting_period_id = accounting_period_id;
                record.advance_contract_id = advance_contract_id;
                record.from_date = PublicFunctions.Tanggal(row.GetCell(4));
                record.to_date = PublicFunctions.Tanggal(row.GetCell(5));
                record.volume = PublicFunctions.Desimal(row.GetCell(6));
                record.distance = PublicFunctions.Desimal(row.GetCell(7));
                record.business_unit_id = business_unit_id;
                record.note = PublicFunctions.IsNullCell(row.GetCell(9));

                dbContext.processing_closing.Add(record);
                await dbContext.SaveChangesAsync();
            }
        }

    }
}

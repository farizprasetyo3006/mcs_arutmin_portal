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

namespace MCSWebApp.Controllers.API.Location
{
    [Route("api/Location/[controller]")]
    [ApiController]
    public class StockpileLocationController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public StockpileLocationController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.stockpile_location
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.stockpile_location.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(stockpile_location),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new stockpile_location();
                    JsonConvert.PopulateObject(values, record);

                    var cekdata = dbContext.stockpile_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.stockpile_location_code.ToLower().Trim() == record.stockpile_location_code.ToLower().Trim())
                        .FirstOrDefault();
                    if (cekdata != null) return BadRequest("Duplicate Code field.");

                    if (record.opening_date > record.closing_date)
                        return BadRequest("Opening Date tidak boleh melampaui Closing Date");

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
                    record.stockpile_location_code = record.stockpile_location_code.ToUpper();

                    dbContext.stockpile_location.Add(record);
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
                var record = dbContext.stockpile_location
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

                        var cekdata = dbContext.stockpile_location
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.stockpile_location_code.ToLower().Trim() == record.stockpile_location_code.ToLower().Trim()
                                && o.id != record.id)
                            .FirstOrDefault();
                        if (cekdata != null) return BadRequest("Duplicate Code field.");

                        if (record.opening_date > record.closing_date)
                            return BadRequest("Opening Date tidak boleh melampaui Closing Date");

                       // record.InjectFrom(e);
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

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var timesheet_detail = dbContext.timesheet_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.destination_id == key).FirstOrDefault();
                if (timesheet_detail != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var record = dbContext.stockpile_location
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.stockpile_location.Remove(record);
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

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] stockpile_location Record)
        {
            try
            {
                var record = dbContext.stockpile_location
                    .Where(o => o.id == Record.id
                        && o.organization_id == CurrentUserContext.OrganizationId)
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
                else if (await mcsContext.CanCreate(dbContext, nameof(stockpile_location),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new stockpile_location();
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

                    dbContext.stockpile_location.Add(record);
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

        [HttpDelete("DeleteById/{Id}")]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Trace($"string Id = {Id}");

            try
            {
                var record = dbContext.stockpile_location
                    .Where(o => o.id == Id
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.stockpile_location.Remove(record);
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

        [HttpGet("StockpileLocationIdLookup")]
        public async Task<object> StockpileLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_stockpile_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = o.business_area_name + " > " + o.stock_location_name,
                            o.product_id,
                            search = o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() + o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper(),
                        });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("StockpileLocationNoVirtualIdLookup")]
        public async Task<object> StockpileLocationNoVirtualIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_stockpile_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.is_virtual == null)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = o.business_area_name + " > " + o.stock_location_name,
                            o.product_id,
                            search = o.business_area_name.ToLower()  + " > " + o.stock_location_name.ToLower() + o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper(),
                        });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProductIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProductIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.product
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.product_name, Search = o.product_name.ToLower() + o.product_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("QualitySamplingIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> QualitySamplingIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.quality_sampling
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sampling_number, Search = o.sampling_number.ToLower() + o.sampling_number.ToUpper() });

                //var lookup = dbContext.quality_sampling.FromSqlRaw(
                //        "select * from quality_sampling where id not in " +
                //        "(select quality_sampling_id from shipping_transaction where quality_sampling_id is not null)"
                //    )
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                //    .Select(o => new { Value = o.id, Text = o.sampling_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ByStockpileLocationId/{Id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ByStockpileLocationId(string Id, DataSourceLoadOptions loadOptions)
        {
            var record = dbContext.vw_stockpile_location
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Id)
                .FirstOrDefault();
            var quality_sampling_id = "";
            if (record != null) quality_sampling_id = record.quality_sampling_id;

            return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling_analyte
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.quality_sampling_id == quality_sampling_id),
                loadOptions);
        }

        [HttpGet("GetItemsById")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetItemsById(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.stockpile_location_detail
                .Where(o => o.stockpile_location_id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertItemData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            stockpile_location_detail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(stockpile_location_detail),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new stockpile_location_detail();
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
                        var header = dbContext.stockpile_location
                                    .Where(x => x.id == record.stockpile_location_id)
                                    .FirstOrDefault();
                        var detail = dbContext.stockpile_location_detail
                            .Where(x => x.stockpile_location_id == record.stockpile_location_id)
                            .ToList();

                        decimal? sum = 0;
                        foreach (var data in detail)
                        {
                            sum = sum + data.quantity;
                        }
                        sum  = sum + record.quantity;
                        /*if (sum > header.current_stock)
                        {
                            return BadRequest("Quantity in Detail Cannot be Bigger than Current Stock in Header");
                        }*/
                        /*else
                        {*/
                        decimal? stock = 0;
                        if (header.current_stock == 0 || header.current_stock == null) {
                            stock = 1;
                        }
                        else
                        {
                            stock = header.current_stock;
                        }
                        var quantity = sum == 0 ? sum = 1 : sum;
                        var presentage = (record.quantity / stock) * 100;
                        record.presentage = presentage;
                        dbContext.stockpile_location_detail.Add(record);

                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();
                        //}
                        
                    }

                    else
                    {
                        logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            return Ok(record);
        }

        [HttpPut("UpdateItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateItemData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.stockpile_location_detail
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

                        // record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        var header = dbContext.stockpile_location
                                    .Where(x => x.id == record.stockpile_location_id)
                                    .FirstOrDefault();
                        var detail = dbContext.stockpile_location_detail
                            .Where(x => x.stockpile_location_id == record.stockpile_location_id)
                            .ToList();

                        decimal? sum = 0;
                        foreach (var data in detail)
                        {
                            sum = sum + data.quantity;
                        }
                        sum = sum + record.quantity;
                        /*if (sum > header.current_stock)
                        {
                            return BadRequest("Quantity in Detail Cannot be Bigger than Current Stock in Header");
                        }*/
                        /*else
                        {*/
                        decimal? stock = 0;
                        if (header.current_stock == 0 || header.current_stock == null)
                        {
                            stock = 1;
                        }
                        else
                        {
                            stock = header.current_stock;
                        }
                        var presentage = (record.quantity / stock) * 100;
                        record.presentage = presentage;

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

        [HttpDelete("DeleteItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteItemData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {

                var record = dbContext.stockpile_location_detail
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.stockpile_location_detail.Remove(record);
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

        [HttpPut("FetchAVG")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ApiResponse> FetchAVG()
        {
            int count = 0;
            // List<contractor> contractors = await dbContext.contractor.ToListAsync();
            List<currency_exchange> currency_Exchanges = await dbContext.currency_exchange.Where(o=>o.exchange_type_id == "9dbb786b3b244c75ad46620d1596fac7").ToListAsync();
            var result = new ApiResponse();
            result.Status.Success = true;
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach(var item in  currency_Exchanges)
                    {
                        DateTime startMonth = new DateTime(item.start_date.Year, item.start_date.Month, 1);
                        item.start_date = startMonth;
                        await dbContext.SaveChangesAsync();
                    }
                    if (result.Status.Success)
                    {
                        await tx.CommitAsync();
                        result.Status.Message = "Ok";
                    }
                    return result;

                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    result.Status.Success = false;
                    result.Status.Message = ex.InnerException?.Message ?? ex.Message;
                    return result;
                }
            }
        }

        [HttpPut("FetchContractorStockpileFromMineLocation")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ApiResponse> FetchContractorStockpileFromMineLocation()
        {
            int count = 0;
            // List<contractor> contractors = await dbContext.contractor.ToListAsync();
            List<mine_location> mineLocations = await dbContext.mine_location.ToListAsync();
            List<stockpile_location> stockpileLocations = await dbContext.stockpile_location.Where(o=>o.is_virtual == true).ToListAsync();
            var result = new ApiResponse();
            result.Status.Success = true;
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var item in stockpileLocations)
                    {
                        count++;
                        var mineLoc = mineLocations.Where(o => o.mine_location_code == item.stockpile_location_code).FirstOrDefault();
                        if (mineLoc !=null && mineLoc.contractor_id != null)
                        {
                        item.contractor_id = mineLoc.contractor_id;
                        await dbContext.SaveChangesAsync();
                        }
                    }
                    if (result.Status.Success)
                    {
                        await tx.CommitAsync();
                        result.Status.Message = "Ok";
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    result.Status.Success = false;
                    result.Status.Message = ex.InnerException?.Message ?? ex.Message;
                    return result;
                }
            }
        }

        [HttpGet("FetchMineLocationToStockPileLocation")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ApiResponse> FetchMineLocationToStockPileLocation()
        {
            var result = new ApiResponse();
            result.Status.Success = true;
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    int count = 0;
                    var mineLocation = await dbContext.mine_location
                        .Where(o=>o.ready_to_get == true)
                        .ToArrayAsync();
                    foreach (var item in mineLocation)
                    {
                        count++; // to check the current loop for debugging mode only
                        item.ready_to_get = false;
                        var current = await dbContext.stockpile_location
                            .Where(o => o.is_virtual == true && o.stockpile_location_code == item.mine_location_code).FirstOrDefaultAsync();
                        if (current != null)
                        {
                            current.modified_by = CurrentUserContext.AppUserId;
                            current.modified_on = DateTime.Now;
                            current.business_area_id = item.business_area_id;
                            current.stockpile_location_code = item.mine_location_code;
                            current.stock_location_name = item.stock_location_name+" Virtual";
                            current.opening_date = item.opening_date;
                            current.is_virtual = true;
                            current.contractor_id = item.contractor_id;
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            var recordStockpileLocation = new stockpile_location();

                            recordStockpileLocation.id = Guid.NewGuid().ToString("N");
                            recordStockpileLocation.created_by = CurrentUserContext.AppUserId;
                            recordStockpileLocation.created_on = DateTime.Now;
                            recordStockpileLocation.modified_by = null;
                            recordStockpileLocation.modified_on = null;
                            recordStockpileLocation.is_active = true;
                            recordStockpileLocation.is_default = null;
                            recordStockpileLocation.is_locked = null;
                            recordStockpileLocation.entity_id = null;
                            recordStockpileLocation.owner_id = CurrentUserContext.AppUserId;
                            recordStockpileLocation.organization_id = CurrentUserContext.OrganizationId;
                            recordStockpileLocation.business_unit_id = item.business_unit_id;

                            recordStockpileLocation.contractor_id = item.contractor_id;
                            recordStockpileLocation.business_area_id = item.business_area_id;
                            recordStockpileLocation.stockpile_location_code = item.mine_location_code;
                            recordStockpileLocation.stock_location_name = item.stock_location_name + " Virtual";
                            recordStockpileLocation.opening_date = item.opening_date;
                            recordStockpileLocation.is_virtual = true;

                            dbContext.stockpile_location.Add(recordStockpileLocation);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    if (result.Status.Success)
                    {
                        await tx.CommitAsync();
                        result.Status.Message = "Ok";
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    result.Status.Success = false;
                    result.Status.Message = ex.InnerException?.Message ?? ex.Message;
                    return result;
                }
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

                    var business_area_id = "";
                    var business_area = dbContext.business_area
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.business_area_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()).FirstOrDefault();
                    if (business_area != null) business_area_id = business_area.id.ToString();

                    var product_id = "";
                    var product = dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.product_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                    if (product != null) product_id = product.id.ToString();

                    var uom_id = "";
                    var uom = dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.uom_symbol.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower()).FirstOrDefault();
                    if (uom != null) uom_id = uom.id.ToString();

                    var quality_sampling_id = "";
                    var quality_sampling = dbContext.quality_sampling
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.sampling_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(8)).ToLower()).FirstOrDefault();
                    if (quality_sampling != null) quality_sampling_id = quality_sampling.id.ToString();

                    var record = dbContext.stockpile_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.stockpile_location_code == PublicFunctions.IsNullCell(row.GetCell(1)))
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.business_area_id = business_area_id;
                        record.product_id = product_id;
                        record.uom_id = uom_id;
                        record.current_stock = PublicFunctions.Desimal(row.GetCell(5));
                        record.opening_date = PublicFunctions.Tanggal(row.GetCell(6));
                        record.closing_date = PublicFunctions.Tanggal(row.GetCell(7));
                        record.quality_sampling_id = quality_sampling_id;

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new stockpile_location();
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

                        record.stockpile_location_code = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.stock_location_name = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.business_area_id = business_area_id;
                        record.product_id = product_id;
                        record.uom_id = uom_id;
                        record.current_stock = PublicFunctions.Desimal(row.GetCell(5));
                        record.opening_date = PublicFunctions.Tanggal(row.GetCell(6));
                        record.closing_date = PublicFunctions.Tanggal(row.GetCell(7));
                        record.quality_sampling_id = quality_sampling_id;

                        dbContext.stockpile_location.Add(record);
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
                HttpContext.Session.SetString("filename", "StockpileLocation");
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

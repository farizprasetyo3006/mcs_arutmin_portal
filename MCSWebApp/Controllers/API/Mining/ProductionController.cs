using BusinessLogic;
using BusinessLogic.Entity;
using Common;
using DataAccess.DTO;
using DataAccess.EFCore.Repository;
using DataAccess.Select2;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MCSWebApp.Controllers.API.Mining
{
    [Route("api/Mining/[controller]")]
    [ApiController]
    public class ProductionController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly IHubContext<ProgressHub> _hubContext;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public ProductionController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption, IHubContext<ProgressHub> hubContext)
             : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
            _hubContext = hubContext;
        }

        //[HttpGet("DataGrid")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        //{
        //    return await DataSourceLoader.LoadAsync(dbContext.vw_production_transaction
        //        .Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
        //                || CurrentUserContext.IsSysAdmin)
        //        .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
        //        loadOptions);
        //}

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_production_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.vw_production_transaction
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.loading_datetime >= dt1 && o.loading_datetime <= dt2),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_production_transaction
                .Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("FetchItem")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> FetchItem([FromQuery] string id)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(production_transaction_item),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = await dbContext.production_transaction.FirstOrDefaultAsync(x => x.id == id);
                        if (record == null)
                            return Ok(new StandardResult { Success = false, Message = "Record not found" });

                        if (!string.IsNullOrEmpty(record.transport_id))
                            return Ok(new StandardResult { Success = false, Message = "Transport is already exists" });

                        var addTruck = await dbContext.truck
                                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin).ToListAsync();

                        var existingItems = await dbContext.production_transaction_item.Where(x => x.production_transaction_id == id).ToListAsync();
                        var items = addTruck.Select(truck => new production_transaction_item
                        {
                            id = Guid.NewGuid().ToString("N"),
                            created_by = CurrentUserContext.AppUserId,
                            created_on = DateTime.Now,
                            modified_by = null,
                            modified_on = null,
                            is_active = true,
                            is_default = null,
                            is_locked = null,
                            entity_id = null,
                            owner_id = CurrentUserContext.AppUserId,
                            organization_id = CurrentUserContext.OrganizationId,
                            business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID"),
                            production_transaction_id = record.id,
                            truck_id = truck.id,
                            truck_factor = truck.typical_tonnage,
                            ritase = 0,
                            jam01 = 0,
                            jam02 = 0,
                            jam03 = 0,
                            jam04 = 0,
                            jam05 = 0,
                            jam06 = 0,
                            jam07 = 0,
                            jam08 = 0,
                            jam09 = 0,
                            jam10 = 0,
                            jam11 = 0,
                            jam12 = 0,
                        }).ToList();

                        decimal? sumRitase = 0;
                        decimal? sumNetQuantity = 0;
                        var summaries = existingItems.Concat(items)
                              .GroupBy(obj => obj.truck_id)
                              .Select(group => group.First())
                              .ToList();
                        foreach (var item in summaries)
                        {
                            for (int i = 1; i <= 12; i++)
                            {
                                var propertyName = $"jam{i:00}";
                                var propertyValue = (decimal?)item.GetType().GetProperty(propertyName)?.GetValue(item) ?? 0;
                                item.ritase += propertyValue;
                            }

                            sumRitase += item.ritase;
                            var truck = item.truck_factor ?? 0;
                            sumNetQuantity = sumNetQuantity + (truck * item.ritase);
                        }

                        items = items.Where(i => !existingItems.Any(e => e.truck_id == i.truck_id)).ToList();
                        if (items.Count > 0)
                        {
                            await dbContext.production_transaction_item.AddRangeAsync(items);
                            await dbContext.SaveChangesAsync();
                        }

                        #region Header Update

                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            record.loading_quantity = !string.IsNullOrEmpty(record.transport_id) ? record.loading_quantity : sumNetQuantity;
                            record.ritase = sumRitase;

                            record.density ??= 1;
                            record.volume = record.density * sumNetQuantity;
                            await dbContext.SaveChangesAsync();
                        }

                        #endregion
                        await tx.CommitAsync();

                        return Ok(new StandardResult { Success = true, Data = record, Message = "Successfully retrieve items" });
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
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            production_transaction record;

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(production_transaction),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new production_transaction();
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

                        #region Validation

                        // Source location != destination location
                        if (record.source_location_id == record.destination_location_id)
                        {
                            return BadRequest("Source location must be different from destination location");
                        }

                        // Capacity
                        //if (record.transport_id != null)
                        //{
                        //    var tr1 = await dbContext.transport
                        //        .Where(o => o.id == record.transport_id)
                        //        .FirstOrDefaultAsync();
                        //    if (tr1 != null)
                        //    {
                        //        if ((decimal)(tr1?.capacity ?? 0) < record.unloading_quantity)
                        //        {
                        //            return BadRequest("Transport capacity is less than unloading quantity");
                        //        }
                        //    }
                        //}

                        #endregion

                        #region Get transaction number

                        var conn = dbContext.Database.GetDbConnection();
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            await conn.OpenAsync();
                        }
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            await using var cmd = conn.CreateCommand();
                            try
                            {
                                cmd.CommandText = "SELECT nextval('seq_transaction_number')";
                                var r = await cmd.ExecuteScalarAsync();
                                record.transaction_number = $"PD-{DateTime.Now:yyyyMMdd}-{r}";
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.ToString());
                                return BadRequest(ex.Message);
                            }
                        }

                        #endregion

                        dbContext.production_transaction.Add(record);

                        #region Calculate actual progress claim

                        if (!string.IsNullOrEmpty(record.progress_claim_id))
                        {
                            var pc = await dbContext.progress_claim
                                .Where(o => o.id == record.progress_claim_id)
                                .FirstOrDefaultAsync();
                            if (pc != null)
                            {
                                var actualQty = await dbContext.production_transaction
                                    .Where(o => o.progress_claim_id == pc.id)
                                    .SumAsync(o => o.unloading_quantity);
                                pc.actual_quantity = actualQty;
                            }
                        }

                        #endregion

                        await dbContext.SaveChangesAsync();

                        await tx.CommitAsync();
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

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            production_transaction record;

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.production_transaction
                        .FirstOrDefault(o => o.id == key
                                             && o.organization_id == CurrentUserContext.OrganizationId);
                    if (record != null)
                    {
                        if (!await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            && !CurrentUserContext.IsSysAdmin)
                        {
                            logger.Debug("User is not authorized.");
                            return Unauthorized();
                        }
                        else
                        {
                            var e = new entity();
                            e.InjectFrom(record);

                            JsonConvert.PopulateObject(values, record);

                            //record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            #region Validation

                            // Must be in open accounting period
                            var ap1 = await dbContext.accounting_period
                                .Where(o => o.id == record.accounting_period_id)
                                .FirstOrDefaultAsync();
                            if (ap1 is { is_closed: true })
                            {
                                return BadRequest("Data update is not allowed");
                            }

                            // Source location != destination location
                            if (record.source_location_id == record.destination_location_id)
                            {
                                return BadRequest("Source location must be different from destination location");
                            }

                            // Capacity
                            //if (record.transport_id != null)
                            //{
                            //    var tr1 = await dbContext.transport
                            //        .Where(o => o.id == record.transport_id)
                            //        .FirstOrDefaultAsync();
                            //    if (tr1 != null)
                            //    {
                            //        if ((decimal)(tr1?.capacity ?? 0) < record.unloading_quantity)
                            //        {
                            //            return BadRequest("Transport capacity is less than unloading quantity");
                            //        }
                            //    }
                            //}

                            #endregion

                            #region Calculate actual progress claim

                            if (!string.IsNullOrEmpty(record.progress_claim_id))
                            {
                                var pc = await dbContext.progress_claim
                                    .Where(o => o.id == record.progress_claim_id)
                                    .FirstOrDefaultAsync();
                                if (pc != null)
                                {
                                    var actualQty = await dbContext.production_transaction
                                        .Where(o => o.progress_claim_id == pc.id)
                                        .SumAsync(o => o.unloading_quantity);
                                    pc.actual_quantity = actualQty;
                                }
                            }

                            #endregion

                            await dbContext.SaveChangesAsync();
                            //if (record.transport_id == null)
                            //{
                            //    var headerData = await dbContext.production_transaction.FirstOrDefaultAsync(x => x.id == record.id);
                            //    var itemData = await dbContext.production_transaction_item
                            //        .Where(x => x.production_transaction_id == record.id).ToListAsync();

                            //    decimal? sumRitase = 0;
                            //    decimal? sumNetQuantity = 0;

                            //    foreach (var item in itemData)
                            //    {
                            //        if (!await mcsContext.CanUpdate(dbContext, item.id, CurrentUserContext.AppUserId)
                            //            && !CurrentUserContext.IsSysAdmin) continue;
                            //        item.jam01 ??= 0;
                            //        item.jam02 ??= 0;
                            //        item.jam03 ??= 0;
                            //        item.jam04 ??= 0;
                            //        item.jam05 ??= 0;
                            //        item.jam06 ??= 0;
                            //        item.jam07 ??= 0;
                            //        item.jam08 ??= 0;
                            //        item.jam09 ??= 0;
                            //        item.jam10 ??= 0;
                            //        item.jam11 ??= 0;
                            //        item.jam12 ??= 0;

                            //        item.ritase = item.jam01 + item.jam02 + item.jam03 + item.jam04 + item.jam05 +
                            //                      item.jam06 +
                            //                      item.jam07 + item.jam08 + item.jam09 + item.jam10 + item.jam11 +
                            //                      item.jam12;

                            //        sumRitase += item.ritase;
                            //        sumNetQuantity += item.truck_factor * item.ritase;

                            //        await dbContext.SaveChangesAsync();
                            //    }

                            //    #region Header Update

                            //    if (await mcsContext.CanUpdate(dbContext, headerData.id, CurrentUserContext.AppUserId)
                            //        || CurrentUserContext.IsSysAdmin)
                            //    {
                            //        headerData.loading_quantity = !string.IsNullOrEmpty(record.transport_id) ? record.loading_quantity : sumNetQuantity;

                            //        headerData.ritase = sumRitase;
                            //        //headerData.loading_quantity = sumNetQuantity;
                            //        headerData.density ??= 1;
                            //        headerData.volume = headerData.density * sumNetQuantity;
                            //        await dbContext.SaveChangesAsync();
                            //    }

                            //    #endregion
                            //}
                            await tx.CommitAsync();
                        }
                    }
                    else
                    {
                        logger.Debug("Record is not found.");
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            /*
            if (success && record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.production_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ProductionTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ProductionTransaction.UpdateStockStateAnalyte(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok();
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Trace($"string key = {key}");

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.production_transaction
                        .FirstOrDefault(o => o.id == key && o.organization_id == CurrentUserContext.OrganizationId);
                    var hauling = dbContext.hauling_transaction.FirstOrDefault(o => o.id == record.hauling_id);
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.production_transaction.Remove(record);

                            await dbContext.SaveChangesAsync();

                            var itemList = dbContext.production_transaction_item
                                .Where(o => o.production_transaction_id == record.id).ToList();

                            foreach (var item in itemList)
                            {

                                dbContext.production_transaction_item.Remove(item);
                                await dbContext.SaveChangesAsync();

                            }
                        }
                        else
                        {
                            logger.Debug("User is not authorized.");
                            return Unauthorized();
                        }
                        if (hauling != null)
                        {
                            hauling.approved = false;
                            await dbContext.SaveChangesAsync();

                        }
                        await tx.CommitAsync();
                    }
                    else
                    {
                        logger.Debug("Record is not found.");
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            /*
            if (success && record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.production_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ProductionTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ProductionTransaction.UpdateStockStateAnalyte(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok();
        }

        [HttpGet("SourceLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SourceLocationIdLookup(string ProcessFlowId,
            DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"ProcessFlowId = {ProcessFlowId}");
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                if (string.IsNullOrEmpty(ProcessFlowId))
                {
                    var lookup = dbContext.vw_mine_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.business_area_name + " > " + o.stock_location_name,
                                o.product_id,
                                search = o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() + o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper()
                            });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.vw_mine_location.FromSqlRaw(
                        " SELECT l.* FROM vw_mine_location l "
                        + " WHERE l.organization_id = {0} "
                        + " AND l.business_area_id IN ( "
                        + "     SELECT ba.id FROM vw_business_area_structure ba, process_flow pf "
                        + "     WHERE position(pf.source_location_id in ba.id_path) > 0"
                        + "         AND pf.id = {1} "
                        + " ) ",
                        CurrentUserContext.OrganizationId, ProcessFlowId)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.business_area_name + " > " + o.stock_location_name,
                                o.product_id,
                                search = o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() + o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper()
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

        [HttpGet("DestinationLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DestinationLocationIdLookup(string ProcessFlowId,
            DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"ProcessFlowId = {ProcessFlowId}");
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                if (string.IsNullOrEmpty(ProcessFlowId))
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
                                search = o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() + o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper()
                            });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.vw_stockpile_location.FromSqlRaw(
                        " SELECT l.* FROM vw_stockpile_location l "
                        + " WHERE l.organization_id = {0} "
                        + " AND l.business_area_id IN ( "
                        + "     SELECT ba.id FROM vw_business_area_structure ba, process_flow pf "
                        + "     WHERE position(pf.destination_location_id in ba.id_path) > 0"
                        + "         AND pf.id = {1} "
                        + " ) ",
                        CurrentUserContext.OrganizationId, ProcessFlowId)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.business_area_name + " > " + o.stock_location_name,
                                o.product_id,
                                search = o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() + o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper()
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

        [HttpGet("SurveyIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SurveyIdLookup(string SourceLocationId,
            DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"SourceLocationId = {SourceLocationId}");

            try
            {
                if (string.IsNullOrEmpty(SourceLocationId))
                {
                    var lookup = dbContext.survey
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && (o.is_draft_survey == null || o.is_draft_survey == false))
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.survey_number, search = o.survey_number.ToLower() + o.survey_number.ToUpper() });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.survey.FromSqlRaw(
                          " SELECT s.* FROM survey s "
                        + " INNER JOIN stock_location sl ON sl.id = s.stock_location_id "
                        + " AND COALESCE(s.is_draft_survey, FALSE) = FALSE "
                        + " AND s.organization_id = {0} "
                        + " AND sl.id = {1} ",
                           CurrentUserContext.OrganizationId, SourceLocationId)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.survey_number,
                                o.product_id,
                                search = o.survey_number.ToLower() + o.survey_number.ToUpper()
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

        [HttpGet("ProcessFlowIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProcessFlowIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.process_flow
                    .Where(o => o.process_flow_category.ToLower() == ProcessFlowCategory.COAL_MINED.ToLower())
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderByDescending(o => o.is_active).ThenBy(o => o.process_flow_name)
                    .Select(o =>
                    new
                    {
                        Value = o.id,
                        Text = o.process_flow_name + (o.is_active == true ? "" : "( ## Not Active )"),
                        Search = o.process_flow_name.ToLower() + (o.is_active == true ? "" : "( ## not active )") + o.process_flow_name.ToUpper() + (o.is_active == true ? "" : "( ## NOT ACTIVE )")
                    });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DespatchOrderIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DespatchOrderIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number, search = o.despatch_order_number.ToLower() + o.despatch_order_number.ToUpper() });
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
                    .Select(o => new { Value = o.id, Text = o.sampling_number, search = o.sampling_number.ToLower() + o.sampling_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProgressClaimIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProgressClaimIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.progress_claim
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.progress_claim_name, search = o.progress_claim_name.ToLower() + o.progress_claim_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ContractRefIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ContractRefIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.advance_contract
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.advance_contract_number, search = o.advance_contract_number.ToLower() + o.advance_contract_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
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
                var record = await dbContext.vw_production_transaction
                    .Where(o => o.id == Id
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefaultAsync();
                return Ok(record);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] production_transaction Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.production_transaction
                        .Where(o => o.id == Record.id
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            if (string.IsNullOrEmpty(record.transaction_number))
                            {
                                #region Get transaction number

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
                                            record.transaction_number = $"PD-{DateTime.Now:yyyyMMdd}-{r}";
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

                            #region Update record

                            var e = new entity();
                            e.InjectFrom(record);
                            record.InjectFrom(Record);
                            record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            #endregion

                            #region Validation

                            // Source location != destination location
                            if (record.source_location_id == record.destination_location_id)
                            {
                                return BadRequest("Source location must be different from destination location");
                            }

                            #endregion

                            #region Update stockpile state

                            var qtyOut = await dbContext.stockpile_state
                                .Where(o => o.stockpile_location_id == record.source_location_id
                                    && o.transaction_id == record.id)
                                .FirstOrDefaultAsync();
                            if (qtyOut != null)
                            {
                                qtyOut.modified_by = CurrentUserContext.AppUserId;
                                qtyOut.modified_on = DateTime.Now;
                                qtyOut.qty_out = record.loading_quantity ?? record.unloading_quantity;
                                qtyOut.transaction_datetime = record.unloading_datetime;
                            }
                            else
                            {
                                qtyOut = new stockpile_state
                                {
                                    id = Guid.NewGuid().ToString("N"),
                                    created_by = CurrentUserContext.AppUserId,
                                    created_on = DateTime.Now,
                                    is_active = true,
                                    owner_id = CurrentUserContext.AppUserId,
                                    organization_id = CurrentUserContext.OrganizationId,
                                    stockpile_location_id = record.source_location_id,
                                    transaction_id = record.id,
                                    qty_out = record.loading_quantity ?? record.unloading_quantity,
                                    transaction_datetime = record.loading_datetime ?? record.unloading_datetime
                                };

                                dbContext.stockpile_state.Add(qtyOut);
                            }

                            var qtyIn = await dbContext.stockpile_state
                                .Where(o => o.stockpile_location_id == record.destination_location_id
                                    && o.transaction_id == record.id)
                                .FirstOrDefaultAsync();
                            if (qtyIn != null)
                            {
                                qtyIn.modified_by = CurrentUserContext.AppUserId;
                                qtyIn.modified_on = DateTime.Now;
                                qtyIn.qty_in = record.unloading_quantity;
                                qtyIn.transaction_datetime = record.loading_datetime ?? record.unloading_datetime;
                            }
                            else
                            {
                                qtyIn = new stockpile_state
                                {
                                    id = Guid.NewGuid().ToString("N"),
                                    created_by = CurrentUserContext.AppUserId,
                                    created_on = DateTime.Now,
                                    is_active = true,
                                    owner_id = CurrentUserContext.AppUserId,
                                    organization_id = CurrentUserContext.OrganizationId,
                                    stockpile_location_id = record.destination_location_id,
                                    transaction_id = record.id,
                                    qty_in = record.unloading_quantity,
                                    transaction_datetime = record.unloading_datetime
                                };

                                dbContext.stockpile_state.Add(qtyIn);
                            }

                            #endregion

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();

                            Task.Run(() =>
                            {
                                var ss = new BusinessLogic.Entity.StockpileState(CurrentUserContext);
                                ss.Update(record.source_location_id, record.id);
                                ss.Update(record.destination_location_id, record.id);
                            }).Forget();

                            return Ok(record);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else if (await mcsContext.CanCreate(dbContext, nameof(production_transaction),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        #region Add record

                        record = new production_transaction();
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

                        #endregion

                        if (string.IsNullOrEmpty(record.transaction_number))
                        {
                            #region Get transaction number

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
                                        record.transaction_number = $"PD-{DateTime.Now:yyyyMMdd}-{r}";
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

                        dbContext.production_transaction.Add(record);

                        #region Validation

                        // Source location != destination location
                        if (record.source_location_id == record.destination_location_id)
                        {
                            return BadRequest("Source location must be different from destination location");
                        }

                        #endregion

                        #region Add to stockpile state

                        var qtyOut = await dbContext.stockpile_state
                            .Where(o => o.stockpile_location_id == record.source_location_id
                                && o.transaction_id == record.id)
                            .FirstOrDefaultAsync();
                        if (qtyOut == null)
                        {
                            qtyOut = new stockpile_state
                            {
                                id = Guid.NewGuid().ToString("N"),
                                created_by = CurrentUserContext.AppUserId,
                                created_on = DateTime.Now,
                                is_active = true,
                                owner_id = CurrentUserContext.AppUserId,
                                organization_id = CurrentUserContext.OrganizationId,
                                stockpile_location_id = record.source_location_id,
                                transaction_id = record.id,
                                qty_out = record.loading_quantity ?? record.unloading_quantity,
                                transaction_datetime = record.loading_datetime ?? record.unloading_datetime
                            };

                            dbContext.stockpile_state.Add(qtyOut);
                        }

                        var qtyIn = await dbContext.stockpile_state
                            .Where(o => o.stockpile_location_id == record.destination_location_id
                                && o.transaction_id == record.id)
                            .FirstOrDefaultAsync();
                        if (qtyIn == null)
                        {
                            qtyIn = new stockpile_state
                            {
                                id = Guid.NewGuid().ToString("N"),
                                created_by = CurrentUserContext.AppUserId,
                                created_on = DateTime.Now,
                                is_active = true,
                                owner_id = CurrentUserContext.AppUserId,
                                organization_id = CurrentUserContext.OrganizationId,
                                stockpile_location_id = record.destination_location_id,
                                transaction_id = record.id,
                                qty_in = record.unloading_quantity,
                                transaction_datetime = record.unloading_datetime
                            };

                            dbContext.stockpile_state.Add(qtyOut);
                        }

                        #endregion

                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();

                        Task.Run(() =>
                        {
                            var ss = new BusinessLogic.Entity.StockpileState(CurrentUserContext);
                            ss.Update(record.source_location_id, record.id);
                            ss.Update(record.destination_location_id, record.id);
                        }).Forget();

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

        [HttpDelete("DeleteById/{Id}")]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Debug($"string Id = {Id}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.production_transaction
                        .Where(o => o.id == Id
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        #region Empty stockpile state

                        var qtyOut = await dbContext.stockpile_state
                            .Where(o => o.stockpile_location_id == record.source_location_id
                                && o.transaction_id == record.id)
                            .FirstOrDefaultAsync();
                        if (qtyOut != null)
                        {
                            qtyOut.qty_in = null;
                            qtyOut.qty_out = null;
                            qtyOut.qty_adjustment = null;
                        }

                        var qtyIn = await dbContext.stockpile_state
                            .Where(o => o.stockpile_location_id == record.destination_location_id
                                && o.transaction_id == record.id)
                            .FirstOrDefaultAsync();
                        if (qtyIn != null)
                        {
                            qtyIn.qty_in = null;
                            qtyIn.qty_out = null;
                            qtyIn.qty_adjustment = null;
                        }

                        #endregion

                        dbContext.production_transaction.Remove(record);

                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();

                        Task.Run(() =>
                        {
                            var ss = new BusinessLogic.Entity.StockpileState(CurrentUserContext);
                            ss.Update(record.source_location_id, record.id);
                            ss.Update(record.destination_location_id, record.id);
                        }).Forget();
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

        [HttpPost("DeleteSelectedRows")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteSelectedRows([FromBody] dynamic Data)
        {
            var result = new StandardResult();
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (Data != null && Data.selectedIds != null)
                    {
                        var selectedIds = ((string)Data.selectedIds)
                            .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .ToList();

                        foreach (string key in selectedIds)
                        {
                            var record = dbContext.production_transaction
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.production_transaction.Remove(record);
                                await dbContext.SaveChangesAsync();

                                var itemList = dbContext.production_transaction_item
                                .Where(o => o.production_transaction_id == record.id).ToList();

                                foreach (var item in itemList)
                                {

                                    dbContext.production_transaction_item.Remove(item);
                                    await dbContext.SaveChangesAsync();

                                }

                                var hauling = dbContext.hauling_transaction.FirstOrDefault(o => o.id == record.hauling_id);
                                if (hauling != null)
                                {
                                    hauling.approved = false;
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }

                        await tx.CommitAsync();

                        result.Success = true;
                        return Ok(result);
                    }
                    else
                    {
                        result.Message = "Invalid data.";
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    result.Message = ex.Message;
                }
            }

            return new JsonResult(result);
        }

        [HttpGet("select2")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Select2([FromQuery] string q)
        {
            var result = new Select2Response();

            try
            {
                var s2Request = new Select2Request()
                {
                    q = q
                };
                if (s2Request != null)
                {
                    var svc = new QualitySampling(CurrentUserContext);
                    var kunci = new Dictionary<string, object>(){
                        {"sampling_type_name", "Hauling Sampling"}
                    };
                    result = await svc.Select2(s2Request, "sampling_number", null, kunci);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return new JsonResult(result);
        }

        [HttpPost("UploadDocument")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UploadDocument([FromBody] dynamic FileDocument)
        {
            var operationId = (string)FileDocument.operationId;
            using (await GlobalUploadQueue.EnterQueueAsync(operationId, _hubContext))
            {
                await _semaphore.WaitAsync();
                try
                {
                    await _hubContext.Clients.Group(operationId).SendAsync("QueueUpdate", -1);
                    var result = new StandardResult();
                    var records = new List<production_transaction>();
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
                    int totalRows = sheet.LastRowNum - sheet.FirstRowNum;

                    using var transaction = await dbContext.Database.BeginTransactionAsync();
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        try
                        {
                            await _hubContext.Clients.Group(operationId).SendAsync("UpdateUploaderProgress", i - sheet.FirstRowNum, totalRows);
                            IRow row = sheet.GetRow(i);
                            if (row == null) continue;
                            //cek row kosong tapi mengandung format cell, dianggap ada datanya, padahal tdk valid
                            if (row.Cells.Count() < 10) continue;

                            //string accounting_period_id = "";
                            //var accounting_period = dbContext.accounting_period
                            //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            //        o.accounting_period_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower()).FirstOrDefault();
                            //if (accounting_period != null) accounting_period_id = accounting_period.id.ToString();

                            string source_shift_id = "";
                            var shift = dbContext.shift
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.shift_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                            if (shift != null) source_shift_id = shift.id.ToString();

                            string process_flow_id = "";
                            var process_flow = dbContext.process_flow
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.process_flow_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower()).FirstOrDefault();
                            if (process_flow != null) process_flow_id = process_flow.id.ToString();


                            string transport_id = "";
                            var truck = dbContext.truck
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.vehicle_id.ToLower() == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower()).FirstOrDefault();
                            if (truck != null) transport_id = truck.id.ToString();

                            string contractor_id = "";
                            var contractor = dbContext.contractor
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(16)).ToLower()).FirstOrDefault();
                            if (contractor != null)
                                contractor_id = contractor.id.ToString();
                            else if (truck != null && truck.vendor_id != null)
                            {
                                contractor_id = truck.vendor_id;
                            }
                            else
                            {
                                teks += "Error in Line : " + (i + 1) + " ==> Contractor not found. Please ensure that your data includes contractor information. " + Environment.NewLine;
                                teks += errormessage + Environment.NewLine + Environment.NewLine;
                                gagal = true;
                                break;
                            }

                            string source_location_id = "";
                            var mine_location = dbContext.mine_location
                                .Where(o => o.mine_location_code == PublicFunctions.IsNullCell(row.GetCell(6))
                                    && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                            if (mine_location != null) source_location_id = mine_location.id.ToString();

                            string destination_location_id = "";
                            var stockpile_location = dbContext.stockpile_location
                                .Where(o => o.stockpile_location_code == PublicFunctions.IsNullCell(row.GetCell(7))
                                    && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                            if (stockpile_location != null) destination_location_id = stockpile_location.id.ToString();

                            //string uom_id = "";
                            //var uom = dbContext.uom
                            //    .Where(o => o.uom_symbol == PublicFunctions.IsNullCell(row.GetCell(11))
                            //        && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                            //if (uom != null) uom_id = uom.id.ToString();

                            string product_id = "";
                            var product = dbContext.product
                                .Where(o => o.product_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(12)).ToLower()
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (product != null) product_id = product.id.ToString();

                            string equipment_id = "";
                            var equipment = dbContext.equipment
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.equipment_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(11)).ToLower()).FirstOrDefault();
                            if (equipment != null) equipment_id = equipment.id.ToString();

                            //string survey_id = "";
                            //var survey = dbContext.survey
                            //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            //        o.survey_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(14)).ToLower()).FirstOrDefault();
                            //if (survey != null) survey_id = survey.id.ToString();

                            var uom_id = "";
                            var uom = dbContext.uom
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.uom_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(11)).ToLower()).FirstOrDefault();
                            if (uom != null) uom_id = uom.id.ToString();

                            var quality_sampling_id = "";
                            var quality_sampling = dbContext.quality_sampling
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.sampling_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(14)).ToLower()).FirstOrDefault();
                            if (quality_sampling != null) quality_sampling_id = quality_sampling.id.ToString();

                            /* string despatch_order_id = "";
                             var despatch_order = dbContext.despatch_order
                                 .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                     o.despatch_order_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(15)).ToLower()).FirstOrDefault();
                             if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();*/

                            string advance_contract_id = "";
                            var advance_contract1 = dbContext.advance_contract
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.advance_contract_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(16)).ToLower()).FirstOrDefault();
                            if (advance_contract1 != null) advance_contract_id = advance_contract1.id.ToString();

                            string employee_id = "";
                            var employee = dbContext.employee
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.employee_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(18)).ToLower()).FirstOrDefault();
                            if (employee != null) employee_id = employee.id.ToString();

                            var TransactionNumber = "";
                            if (PublicFunctions.IsNullCell(row.GetCell(0)) == "")
                            {
                                #region Get transaction number
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
                                            TransactionNumber = $"PD-{DateTime.Now:yyyyMMdd}-{r}";
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
                                TransactionNumber = PublicFunctions.IsNullCell(row.GetCell(0));

                            var business_unit_id = "";
                            var business_unit = dbContext.business_unit.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.business_unit_code.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(21)).ToUpper()).FirstOrDefault();
                            if (business_unit != null)
                            {
                                business_unit_id = business_unit.id.ToString();
                            }
                            else
                            {
                                teks += "Error in Line : " + (i + 1) + " ==> Business Unit Not Found" + Environment.NewLine;
                                teks += errormessage + Environment.NewLine + Environment.NewLine;
                                gagal = true;
                                break;

                            }
                            var record = dbContext.production_transaction
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.transaction_number.Trim() == TransactionNumber.Trim())
                                .FirstOrDefault();
                            if (record != null)
                            {
                                var e = new entity();
                                e.InjectFrom(record);

                                //record.InjectFrom(e);
                                record.modified_by = CurrentUserContext.AppUserId;
                                record.modified_on = DateTime.Now;

                                record.loading_datetime = Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(1)));
                                record.unloading_datetime = Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(2)));
                                record.source_shift_id = source_shift_id;
                                record.process_flow_id = process_flow_id;
                                record.transport_id = transport_id;
                                record.source_location_id = source_location_id;
                                record.destination_location_id = destination_location_id;
                                record.gross = PublicFunctions.Desimal(row.GetCell(8));
                                record.tare = PublicFunctions.Desimal(row.GetCell(9));
                                record.loading_quantity = PublicFunctions.Desimal(row.GetCell(10));
                                record.uom_id = uom_id;
                                record.product_id = product_id;
                                record.distance = PublicFunctions.Desimal(row.GetCell(13));
                                record.quality_sampling_id = quality_sampling_id;
                                record.equipment_id = equipment_id;
                                record.contractor_id = contractor_id;
                                record.advance_contract_id1 = advance_contract_id;
                                record.pic = employee_id;
                                record.note = PublicFunctions.IsNullCell(row.GetCell(19));
                                record.ritase = PublicFunctions.Desimal(row.GetCell(20));
                                record.business_unit_id = business_unit_id;

                                await dbContext.SaveChangesAsync();
                            }
                            else
                            {
                                record = new production_transaction();
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

                                record.transaction_number = TransactionNumber;
                                record.loading_datetime = Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(1)));
                                record.unloading_datetime = Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(2)));
                                record.source_shift_id = source_shift_id;
                                record.process_flow_id = process_flow_id;
                                record.transport_id = transport_id;
                                record.source_location_id = source_location_id;
                                record.destination_location_id = destination_location_id;
                                record.gross = PublicFunctions.Desimal(row.GetCell(8));
                                record.tare = PublicFunctions.Desimal(row.GetCell(9));
                                record.loading_quantity = PublicFunctions.Desimal(row.GetCell(10));
                                record.uom_id = uom_id;
                                record.product_id = product_id;
                                record.distance = PublicFunctions.Desimal(row.GetCell(13));
                                record.quality_sampling_id = quality_sampling_id;
                                record.equipment_id = equipment_id;
                                record.contractor_id = contractor_id;
                                record.advance_contract_id1 = advance_contract_id;
                                record.pic = employee_id;
                                record.note = PublicFunctions.IsNullCell(row.GetCell(19));
                                record.ritase = PublicFunctions.Desimal(row.GetCell(20));
                                record.business_unit_id = business_unit_id;

                                dbContext.production_transaction.Add(record);
                                await dbContext.SaveChangesAsync();
                            }

                            records.Add(record);
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
                        HttpContext.Session.SetString("filename", "Production");
                        return BadRequest("File gagal di-upload");
                    }
                    else
                    {
                        await transaction.CommitAsync();
                        return "File berhasil di-upload!";
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

            #region Item Data Section

            [HttpGet("GetItemsById")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetItemsById(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.production_transaction_item
                .Where(o => o.production_transaction_id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertItemData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            production_transaction_item record;

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(production_transaction_item),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new production_transaction_item();
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
                        record.ritase = 0;

                        dbContext.production_transaction_item.Add(record);

                        await dbContext.SaveChangesAsync();

                        var headerData = dbContext.production_transaction.FirstOrDefault(x => x.id == record.production_transaction_id);
                        var itemData = dbContext.production_transaction_item
                            .Where(x => x.production_transaction_id == record.production_transaction_id).ToList();

                        decimal? sumRitase = 0;
                        decimal? sumNetQuantity = 0;

                        foreach (var item in itemData)
                        {
                            if (!await mcsContext.CanUpdate(dbContext, item.id, CurrentUserContext.AppUserId)
                                && !CurrentUserContext.IsSysAdmin) continue;
                            item.jam01 ??= 0;
                            item.jam02 ??= 0;
                            item.jam03 ??= 0;
                            item.jam04 ??= 0;
                            item.jam05 ??= 0;
                            item.jam06 ??= 0;
                            item.jam07 ??= 0;
                            item.jam08 ??= 0;
                            item.jam09 ??= 0;
                            item.jam10 ??= 0;
                            item.jam11 ??= 0;
                            item.jam12 ??= 0;

                            item.ritase = item.jam01 + item.jam02 + item.jam03 + item.jam04 + item.jam05 + item.jam06 +
                                          item.jam07 + item.jam08 + item.jam09 + item.jam10 + item.jam11 + item.jam12;

                            sumRitase += item.ritase;
                            sumNetQuantity += item.truck_factor * item.ritase;

                            await dbContext.SaveChangesAsync();
                        }

                        #region Header Update

                        if (await mcsContext.CanUpdate(dbContext, headerData.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            if (!string.IsNullOrEmpty(headerData.transport_id))
                            {
                                headerData.loading_quantity = headerData.loading_quantity;
                            }
                            else
                            {
                                headerData.loading_quantity = sumNetQuantity;
                            }
                            headerData.ritase = sumRitase;
                            //headerData.loading_quantity = sumNetQuantity;
                            headerData.density ??= 1;
                            headerData.volume = headerData.density * sumNetQuantity;
                            await dbContext.SaveChangesAsync();
                        }

                        #endregion

                        await tx.CommitAsync();
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

            production_transaction_item record;

            await using var tx = await dbContext.Database.BeginTransactionAsync();
            try
            {
                record = dbContext.production_transaction_item
                    .FirstOrDefault(o => o.id == key
                                         && o.organization_id == CurrentUserContext.OrganizationId);
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
                        record.ritase = 0;
                        for (int i = 1; i <= 12; i++)
                        {
                            var propertyName = $"jam{i:00}";
                            record.ritase += (decimal?)record.GetType().GetProperty(propertyName)?.GetValue(record) ?? 0;
                        }
                        await dbContext.SaveChangesAsync();

                        var headerData = dbContext.production_transaction.FirstOrDefault(x => x.id == record.production_transaction_id);
                        var itemData = dbContext.production_transaction_item
                            .Where(x => x.production_transaction_id == record.production_transaction_id).ToList();

                        decimal? sumRitase = 0;
                        decimal? sumNetQuantity = 0;

                        sumRitase = itemData.Sum(item =>
                        {
                            decimal ritase = 0;
                            for (int i = 1; i <= 12; i++)
                            {
                                var propertyName = $"jam{i:00}";
                                ritase += (decimal?)item.GetType().GetProperty(propertyName)?.GetValue(item) ?? 0;
                            }
                            return ritase;
                        });

                        sumNetQuantity = itemData.Sum(item => (item.truck_factor ?? 0) * item.ritase);

                        #region Header Update

                        if (await mcsContext.CanUpdate(dbContext, headerData.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            if (!string.IsNullOrEmpty(headerData.transport_id))
                            {
                                headerData.loading_quantity = headerData.loading_quantity;
                            }
                            else
                            {
                                headerData.loading_quantity = sumNetQuantity;
                            }
                            headerData.ritase = sumRitase;
                            // headerData.loading_quantity = sumNetQuantity;
                            headerData.density ??= 1;
                            headerData.volume = headerData.density * (sumNetQuantity ?? 0);
                            await dbContext.SaveChangesAsync();
                        }

                        #endregion

                        await tx.CommitAsync();
                        return Ok(record);

                    }
                    else
                    {
                        logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                else
                {
                    logger.Debug("Record is not found.");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("DeleteItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteItemData([FromForm] string key)
        {
            logger.Trace($"string key = {key}");

            production_transaction_item record;

            await using var tx = await dbContext.Database.BeginTransactionAsync();
            try
            {
                record = dbContext.production_transaction_item
                    .FirstOrDefault(o => o.id == key
                                         && o.organization_id == CurrentUserContext.OrganizationId);
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.production_transaction_item.Remove(record);

                        await dbContext.SaveChangesAsync();

                        var headerData = dbContext.production_transaction.FirstOrDefault(x => x.id == record.production_transaction_id);
                        var itemData = dbContext.production_transaction_item
                            .Where(x => x.production_transaction_id == record.production_transaction_id).ToList();

                        decimal? sumRitase = 0;
                        decimal? sumNetQuantity = 0;

                        foreach (var item in itemData)
                        {
                            if (!await mcsContext.CanUpdate(dbContext, item.id, CurrentUserContext.AppUserId)
                                && !CurrentUserContext.IsSysAdmin) continue;
                            item.jam01 ??= 0;
                            item.jam02 ??= 0;
                            item.jam03 ??= 0;
                            item.jam04 ??= 0;
                            item.jam05 ??= 0;
                            item.jam06 ??= 0;
                            item.jam07 ??= 0;
                            item.jam08 ??= 0;
                            item.jam09 ??= 0;
                            item.jam10 ??= 0;
                            item.jam11 ??= 0;
                            item.jam12 ??= 0;

                            item.ritase = item.jam01 + item.jam02 + item.jam03 + item.jam04 + item.jam05 + item.jam06 +
                                          item.jam07 + item.jam08 + item.jam09 + item.jam10 + item.jam11 + item.jam12;

                            sumRitase += item.ritase;
                            sumNetQuantity += item.truck_factor * item.ritase;
                            await dbContext.SaveChangesAsync();
                        }
                        #region Header Update
                        if (await mcsContext.CanUpdate(dbContext, headerData.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            headerData.ritase = sumRitase;
                            headerData.loading_quantity = sumNetQuantity;
                            headerData.density ??= 1;
                            headerData.volume = headerData.density * sumNetQuantity;
                            await dbContext.SaveChangesAsync();
                        }
                        #endregion
                        await tx.CommitAsync();
                        return Ok(record);
                    }
                    else
                    {
                        logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                else
                {
                    logger.Debug("Record is not found.");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        #endregion

        #region Additional Lookup

        // ---> This Lookup Based on Business Unit or SysAdmin
        [HttpGet("ShiftIdLookup")]
        public async Task<object> ShiftIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.shift
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.business_unit_id == CurrentUserContext.BusinessUnitId || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.shift_name, search = o.shift_name.ToLower() + o.shift_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }

            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("EquipmentIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> EquipmentIdLookup(DataSourceLoadOptions loadOptions, string contractorId)
        {
            try
            {
                if (!string.IsNullOrEmpty(contractorId))
                {
                    var lookup = dbContext.equipment
                        //.Where(o => o.vendor_id == contractorId)
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.equipment_code + " - " + o.equipment_name, search = o.equipment_code.ToLower() + " - " + o.equipment_name.ToLower() + o.equipment_code.ToUpper() + " - " + o.equipment_name.ToUpper() });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.equipment
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.equipment_code + " - " + o.equipment_name, search = o.equipment_code.ToLower() + " - " + o.equipment_name.ToLower() + o.equipment_code.ToUpper() + " - " + o.equipment_name.ToUpper() });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        #endregion

    }
}

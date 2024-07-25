using BusinessLogic;
using DataAccess.DTO;
using DataAccess.EFCore.Repository;
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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MCSWebApp.Controllers.API.Mining
{
    [Route("api/Mining/[controller]")]
    [ApiController]
    public class WasteRemovalController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly IHubContext<ProgressHub> _hubContext;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public WasteRemovalController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption, IHubContext<ProgressHub> hubContext)
             : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
            _hubContext = hubContext;
        }

        //[HttpGet("DataGrid")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        //{
        //    return await DataSourceLoader.LoadAsync(dbContext.waste_removal
        //        //.Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
        //        //        || CurrentUserContext.IsSysAdmin)
        //        .Where(o => o.organization_id == CurrentUserContext.OrganizationId), 
        //        loadOptions);
        //}

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_waste_removal
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.vw_waste_removal
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.unloading_datetime >= dt1 && o.unloading_datetime <= dt2),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.waste_removal
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
                    if (await mcsContext.CanCreate(dbContext, nameof(waste_removal_item),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = await dbContext.waste_removal
                            .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                            .Where(x => x.id == id).FirstOrDefaultAsync();

                        if (record == null)
                            return Ok(new StandardResult { Success = false, Message = "Record not found" });

                        if (!string.IsNullOrEmpty(record.transport_id))
                            return Ok(new StandardResult { Success = false, Message = "Transport is already exists" });

                        #region Add Item Truck List
                        var addTruck = await dbContext.truck
                            .Where(x => x.business_unit_id == "" ||
                                        x.business_unit_id == null ||
                                        x.business_unit_id == record.business_unit_id).ToListAsync();

                        var existingItems = await dbContext.waste_removal_item.Where(x => x.waste_removal_id == record.id).ToListAsync();
                        var wasteRemovalItems = addTruck.Select(truck => new waste_removal_item
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
                            waste_removal_id = record.id,
                            truck_id = truck.id,
                            truck_factor = truck.typical_volume,
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
                        var updateItems = existingItems.Concat(wasteRemovalItems)
                              .GroupBy(obj => obj.truck_id)
                              .Select(group => group.First())
                              .ToList();
                        foreach (var item in updateItems)
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

                        // Save changes after all items are added
                        wasteRemovalItems = wasteRemovalItems.Where(x => !existingItems.Any(w => w.truck_id == x.truck_id)).ToList();
                        if (wasteRemovalItems.Count > 0)
                        {
                            await dbContext.waste_removal_item.AddRangeAsync(wasteRemovalItems);
                            await dbContext.SaveChangesAsync();
                        }

                        #endregion

                        #region Recalculate Item Total Ritase | Header | Ritase | Tonnase | Quantity

                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            if (!string.IsNullOrEmpty(record.transport_id))
                            {
                                record.loading_quantity = record.loading_quantity;
                            }
                            else
                            {
                                record.loading_quantity = sumNetQuantity;
                            }
                            record.ritase = sumRitase;
                            //headerData.loading_quantity = sumNetQuantity;
                            if (record.density == null)
                                record.density = 1;
                            record.unloading_quantity = record.density.Value * sumNetQuantity.Value;
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }

                        #endregion

                        await tx.CommitAsync();
                        return Ok(new StandardResult { Success = true, Data = record, Message = "Successfully retrieve items" });
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

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(waste_removal),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new waste_removal();
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

                        #endregion

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
                                    string organizationCode = dbContext.organization
                                        .Where(o => o.id == CurrentUserContext.OrganizationId)
                                        .Select(o => o.organization_code)
                                        .FirstOrDefault() ?? "";
                                    switch (organizationCode.ToUpper())
                                    {
                                        case "IN01":
                                            cmd.CommandText = $"SELECT nextval('seq_waste_removal_number_ic')";
                                            break;
                                        case "KM01":
                                            cmd.CommandText = $"SELECT nextval('seq_waste_removal_number_kmia')";
                                            break;
                                        default:
                                            cmd.CommandText = $"SELECT nextval('seq_waste_removal_number')";
                                            break;
                                    }

                                    var r = await cmd.ExecuteScalarAsync();
                                    r = Convert.ToInt32(r).ToString("D5");
                                    record.transaction_number = $"WR-{DateTime.Now:yyyyMMdd}-{r}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        dbContext.waste_removal.Add(record);

                        #region Calculate actual progress claim

                        if (!string.IsNullOrEmpty(record.progress_claim_id))
                        {
                            var pc = await dbContext.progress_claim
                                .Where(o => o.id == record.progress_claim_id)
                                .FirstOrDefaultAsync();
                            if (pc != null)
                            {
                                var actualQty = await dbContext.waste_removal
                                    .Where(o => o.progress_claim_id == pc.id)
                                    .SumAsync(o => o.unloading_quantity);
                                pc.actual_quantity = actualQty;
                            }
                        }

                        #endregion

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
                    var record = dbContext.waste_removal
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

                            //record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            #region Validation

                            // Must be in open accounting period
                            var ap1 = await dbContext.accounting_period
                                .Where(o => o.id == record.accounting_period_id)
                                .FirstOrDefaultAsync();
                            if (ap1 != null && (ap1?.is_closed ?? false))
                            {
                                return BadRequest("Data update is not allowed");
                            }

                            // Source location != destination location
                            if (record.source_location_id == record.destination_location_id)
                            {
                                return BadRequest("Source location must be different from destination location");
                            }

                            #endregion

                            #region Calculate actual progress claim

                            if (!string.IsNullOrEmpty(record.progress_claim_id))
                            {
                                var pc = await dbContext.progress_claim
                                    .Where(o => o.id == record.progress_claim_id)
                                    .FirstOrDefaultAsync();
                                if (pc != null)
                                {
                                    var actualQty = await dbContext.waste_removal
                                        .Where(o => o.progress_claim_id == pc.id)
                                        .SumAsync(o => o.unloading_quantity);
                                    pc.actual_quantity = actualQty;
                                }
                            }

                            #endregion

                            await dbContext.SaveChangesAsync();
                            //if (record.transport_id == null)
                            //{
                            //    #region Recalculate Item Total Ritase | Header | Ritase | Tonnase | Quantity

                            //    var headerData = await dbContext.waste_removal
                            //        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            //        .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                            //        .Where(x => x.id == record.id).FirstOrDefaultAsync();
                            //    var itemData = await dbContext.waste_removal_item
                            //        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            //        .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                            //        .Where(x => x.waste_removal_id == record.id).ToListAsync();

                            //    decimal? sumRitase = 0;
                            //    decimal? sumNetQuantity = 0;

                            //    foreach (var item in itemData)
                            //    {
                            //        if (await mcsContext.CanUpdate(dbContext, item.id, CurrentUserContext.AppUserId)
                            //        || CurrentUserContext.IsSysAdmin)
                            //        {
                            //            if (item.jam01 == null)
                            //                item.jam01 = 0;
                            //            if (item.jam02 == null)
                            //                item.jam02 = 0;
                            //            if (item.jam03 == null)
                            //                item.jam03 = 0;
                            //            if (item.jam04 == null)
                            //                item.jam04 = 0;
                            //            if (item.jam05 == null)
                            //                item.jam05 = 0;
                            //            if (item.jam06 == null)
                            //                item.jam06 = 0;
                            //            if (item.jam07 == null)
                            //                item.jam07 = 0;
                            //            if (item.jam08 == null)
                            //                item.jam08 = 0;
                            //            if (item.jam09 == null)
                            //                item.jam09 = 0;
                            //            if (item.jam10 == null)
                            //                item.jam10 = 0;
                            //            if (item.jam11 == null)
                            //                item.jam11 = 0;
                            //            if (item.jam12 == null)
                            //                item.jam12 = 0;

                            //            item.ritase = item.jam01 + item.jam02 + item.jam03 + item.jam04 + item.jam05 + item.jam06 +
                            //                item.jam07 + item.jam08 + item.jam09 + item.jam10 + item.jam11 + item.jam12;

                            //            sumRitase = sumRitase + item.ritase;
                            //            sumNetQuantity = sumNetQuantity + (item.truck_factor * item.ritase);

                            //            await dbContext.SaveChangesAsync();
                            //        }
                            //        else
                            //        {
                            //            return BadRequest("User is not authorized.");
                            //        }
                            //    }

                            //    if (await mcsContext.CanUpdate(dbContext, headerData.id, CurrentUserContext.AppUserId)
                            //        || CurrentUserContext.IsSysAdmin)
                            //    {
                            //        if (!string.IsNullOrEmpty(record.transport_id))
                            //        {
                            //            headerData.loading_quantity = record.loading_quantity;
                            //        }
                            //        else
                            //        {
                            //            headerData.loading_quantity = sumNetQuantity;
                            //        }
                            //        headerData.ritase = sumRitase;
                            //        //headerData.loading_quantity = sumNetQuantity;
                            //        if (headerData.density == null)
                            //            headerData.density = 1;
                            //        headerData.unloading_quantity = headerData.density.Value * headerData.loading_quantity.Value;
                            //        await dbContext.SaveChangesAsync();
                            //    }
                            //    else
                            //    {
                            //        return BadRequest("User is not authorized.");
                            //    }

                            //    #endregion
                            //}
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
                        return BadRequest("Data update is not allowed");
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
                var record = dbContext.waste_removal
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.waste_removal.Remove(record);
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
                            var record = dbContext.waste_removal
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.waste_removal.Remove(record);
                                await dbContext.SaveChangesAsync();
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

        [HttpGet("WasteIdLookup")]
        public async Task<object> WasteIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.waste
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.waste_name, Search = o.waste_name.ToLower() + o.waste_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        //     [HttpGet("SourceLocationIdLookup")]
        //     [ApiExplorerSettings(IgnoreApi = true)]
        //     public async Task<object> SourceLocationIdLookup(DataSourceLoadOptions loadOptions)
        //     {
        //         logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

        //         try
        //         {
        //             var mines = dbContext.mine_location
        //                 .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //                 .Select(o => 
        //                     new 
        //                     { 
        //                         value = o.id, 
        //                         text = o.stock_location_name,
        //                         o.product_id
        //                     });
        //             return await DataSourceLoader.LoadAsync(mines, loadOptions);
        //         }
        //catch (Exception ex)
        //{
        //	logger.Error(ex.InnerException ?? ex);
        //             return BadRequest(ex.InnerException?.Message ?? ex.Message);
        //}
        //     }

        [HttpGet("SourceLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SourceLocationIdLookup(string ProcessFlowId, DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"ProcessFlowId = {ProcessFlowId}");
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                if (string.IsNullOrEmpty(ProcessFlowId))
                {
                    //var lookup = dbContext.vw_mine_location
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //    .Select(o =>
                    //        new
                    //        {
                    //            value = o.id,
                    //            text = o.business_area_name + " > " + o.stock_location_name,
                    //            o.product_id
                    //        })
                    //    .OrderBy(o => o.text);

                    var ml1 = dbContext.vw_business_area
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.parent_business_area_name == "PIT")
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.business_area_name,
                                norut = 1,
                                search = o.business_area_name.ToLower() + o.business_area_name.ToUpper()
                            }); ;
                    var ml2 = dbContext.vw_mine_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.business_area_name + " > " + o.stock_location_name,
                                norut = 2,
                                search = o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() + o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper(),
                            });

                    var lookup = ml1.Union(ml2).OrderBy(o => o.norut).ThenBy(o => o.text);

                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    //var lookup = dbContext.vw_mine_location.FromSqlRaw(
                    //    " SELECT l.* FROM vw_mine_location l "
                    //    + " WHERE l.organization_id = {0} "
                    //    + " AND l.business_area_id IN ( "
                    //    + "     SELECT ba.id FROM vw_business_area_structure ba, process_flow pf "
                    //    + "     WHERE position(pf.source_location_id in ba.id_path) > 0"
                    //    + "         AND pf.id = {1} "
                    //    + " ) ",
                    //    CurrentUserContext.OrganizationId, ProcessFlowId)
                    //    .Select(o =>
                    //        new
                    //        {
                    //            value = o.id,
                    //            text = o.business_area_name + " > " + o.stock_location_name,
                    //            o.product_id
                    //        })
                    //    .OrderBy(o => o.text);

                    var ml1 = dbContext.vw_business_area
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.parent_business_area_name == "PIT")
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.business_area_name,
                                norut = 1,
                                search = o.business_area_name.ToLower() + o.business_area_name.ToUpper(),
                            });
                    var ml2 = dbContext.vw_mine_location.FromSqlRaw(
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
                                norut = 2,
                                search = o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() + o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper(),
                            })
                        .OrderBy(o => o.text);

                    var lookup = ml1.Union(ml2).OrderBy(o => o.norut).ThenBy(o => o.text);

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
        public async Task<object> DestinationLocationIdLookup(string ProcessFlowId, DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                var lookup = dbContext.waste_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                    new
                    {
                        value = o.id,
                        text = o.stock_location_name,
                        o.product_id,
                        search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper()
                    }); ;
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
        public async Task<object> EquipmentIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.equipment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.equipment_code + " - " + o.equipment_name, Search = o.equipment_code.ToLower() + " - " + o.equipment_name.ToLower() + o.equipment_code.ToUpper() + " - " + o.equipment_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("EquipmentIdContractorLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> EquipmentIdContractorLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var contractor = dbContext.contractor
                    .Where(o => o.id == Id)
                    .Select(o => o.owner_id)
                    .FirstOrDefault();
                var lookup = dbContext.equipment
                    .Where(o => o.owner_id == contractor)
                    .Select(o => new { Value = o.id, Text = o.equipment_code + " - " + o.equipment_name, search = o.equipment_code.ToLower() + " - " + o.equipment_name.ToLower() + o.equipment_code.ToUpper() + " - " + o.equipment_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
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
                    .Where(o => o.process_flow_category == Common.ProcessFlowCategory.WASTE_REMOVAL
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                    new
                    {
                        Value = o.id,
                        Text = o.process_flow_name,
                        search = o.process_flow_name.ToLower() + o.process_flow_name.ToUpper()
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
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number, Search = o.despatch_order_number.ToLower() + o.despatch_order_number.ToUpper() });
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
                    .Select(o => new { Value = o.id, Text = o.progress_claim_name, Search = o.progress_claim_name.ToLower() + o.progress_claim_name.ToUpper() });
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
                var record = await dbContext.vw_waste_removal
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
        public async Task<IActionResult> SaveData([FromBody] waste_removal Record)
        {
            try
            {
                var record = dbContext.waste_removal
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
                else if (await mcsContext.CanCreate(dbContext, nameof(waste_removal),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new waste_removal();
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

                    dbContext.waste_removal.Add(record);
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
            logger.Debug($"string Id = {Id}");

            try
            {
                var record = dbContext.waste_removal
                    .Where(o => o.id == Id
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.waste_removal.Remove(record);
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

        [HttpGet("SurveyIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SurveyIdLookup(string SourceLocationId, DataSourceLoadOptions loadOptions)
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
                        .Select(o => new { Value = o.id, Text = o.survey_number, Search = o.survey_number.ToLower() + o.survey_number.ToUpper() });
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

        [HttpGet("ShiftIdLookup")]
        public async Task<object> ShiftIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.shift
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.shift_name, Search = o.shift_name.ToLower() + o.shift_name.ToUpper() });
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
            var operationId = (string)FileDocument.operationId;
            using (await GlobalUploadQueue.EnterQueueAsync(operationId, _hubContext))
            {
                await _semaphore.WaitAsync();
                try
                {
                    await _hubContext.Clients.Group(operationId).SendAsync("QueueUpdate", -1);
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

                            var process_flow_id = "";
                            var process_flow = dbContext.process_flow
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.process_flow_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                            if (process_flow != null) process_flow_id = process_flow.id.ToString();

                            var source_shift_id = "";
                            var shift = dbContext.shift
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.shift_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower()).FirstOrDefault();
                            if (shift != null) source_shift_id = shift.id.ToString();

                            var source_location_id = "";
                            var mine_location = dbContext.mine_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.mine_location_code == PublicFunctions.IsNullCell(row.GetCell(5))).FirstOrDefault();
                            if (mine_location != null)
                                source_location_id = mine_location.id.ToString();
                            else
                            {
                                teks += "==>Error Sheet 1, Line " + (i + 1) + ", 'Source' is empty or not found!" + Environment.NewLine;
                                gagal = true;
                                break;
                            }

                            var destination_location_id = "";
                            var waste_location = dbContext.waste_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.waste_location_code == PublicFunctions.IsNullCell(row.GetCell(10))).FirstOrDefault();
                            if (waste_location != null)
                                destination_location_id = waste_location.id.ToString();
                            else
                            {
                                teks += "==>Error Sheet 1, Line " + (i + 1) + ", 'Destination' is empty or not found!" + Environment.NewLine;
                                gagal = true;
                                break;
                            }

                            var waste_id = "";
                            var waste = dbContext.waste
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.waste_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(9)).ToLower()).FirstOrDefault();
                            if (waste != null) waste_id = waste.id.ToString();

                            var uom_id = "";
                            var uom = dbContext.uom
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.uom_name == PublicFunctions.IsNullCell(row.GetCell(12))).FirstOrDefault();
                            if (uom != null) uom_id = uom.id.ToString();

                            var transport_id = "";
                            var truck = dbContext.truck
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.vehicle_id.ToLower() == PublicFunctions.IsNullCell(row.GetCell(13)).ToLower()).FirstOrDefault();
                            if (truck != null) transport_id = truck.id.ToString();
                            else
                            {
                                var equip = dbContext.vw_equipment.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.equipment_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(13)).ToLower()).FirstOrDefault();
                                if (equip != null) transport_id = equip.id.ToString();
                            }
                            var equipment_id = "";
                            var equipment = dbContext.equipment
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.equipment_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(6)).ToLower()).FirstOrDefault();
                            if (equipment != null) equipment_id = equipment.id.ToString();

                            /* var despatch_order_id = "";
                             var despatch_order = dbContext.despatch_order
                                 .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                     o.despatch_order_number == PublicFunctions.IsNullCell(row.GetCell(14))).FirstOrDefault();
                             if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();

                             var progress_claim_id = "";
                             var progress_claim = dbContext.progress_claim
                                 .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                     o.progress_claim_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(15)).ToLower()).FirstOrDefault();
                             if (progress_claim != null) progress_claim_id = progress_claim.id.ToString();*/

                            var advance_contract_id = "";
                            var advance_contract = dbContext.advance_contract
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.advance_contract_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(16)).ToLower()).FirstOrDefault();
                            if (advance_contract != null) advance_contract_id = advance_contract.id.ToString();

                            var contractor_id = "";
                            var contractor = dbContext.contractor
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.business_partner_code.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).Trim().ToLower())
                                .FirstOrDefault();
                            if (contractor != null) contractor_id = contractor.id.ToString();

                            var employee_id = "";
                            var employee = dbContext.employee.Where(o => o.employee_number == PublicFunctions.IsNullCell(row.GetCell(15))).FirstOrDefault();
                            if (employee != null) employee_id = employee.id.ToString();

                            var business_unit_id = "";
                            var business_unit = dbContext.business_unit
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.business_unit_code == PublicFunctions.IsNullCell(row.GetCell(17)).ToUpper())
                                .FirstOrDefault();
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
                                            string organizationCode = dbContext.organization
                                                .Where(o => o.id == CurrentUserContext.OrganizationId)
                                                .Select(o => o.organization_code)
                                                .FirstOrDefault() ?? "";
                                            switch (organizationCode.ToUpper())
                                            {
                                                case "IN01":
                                                    cmd.CommandText = $"SELECT nextval('seq_waste_removal_number_ic')";
                                                    break;
                                                case "KM01":
                                                    cmd.CommandText = $"SELECT nextval('seq_waste_removal_number_kmia')";
                                                    break;
                                                default:
                                                    cmd.CommandText = $"SELECT nextval('seq_waste_removal_number')";
                                                    break;
                                            }

                                            var r = await cmd.ExecuteScalarAsync();
                                            r = Convert.ToInt32(r).ToString("D5");
                                            TransactionNumber = $"WR-{DateTime.Now:yyyyMMdd}-{r}";
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

                            var record = dbContext.waste_removal
                                .Where(o => o.transaction_number.ToLower() == TransactionNumber.ToLower()
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                var e = new entity();
                                e.InjectFrom(record);

                                // record.InjectFrom(e);
                                record.modified_by = CurrentUserContext.AppUserId;
                                record.modified_on = DateTime.Now;

                                record.unloading_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                                record.contractor_id = contractor_id;
                                record.process_flow_id = process_flow_id;
                                record.source_shift_id = source_shift_id;
                                record.source_location_id = source_location_id;
                                record.equipment_id = equipment_id;
                                record.loading_quantity = PublicFunctions.Desimal(row.GetCell(7));
                                record.distance = PublicFunctions.Desimal(row.GetCell(8));
                                record.waste_id = waste_id;
                                record.destination_location_id = destination_location_id;
                                record.elevation = PublicFunctions.Desimal(row.GetCell(11));
                                record.uom_id = uom_id;
                                record.transport_id = transport_id;
                                record.note = PublicFunctions.IsNullCell(row.GetCell(14));
                                record.pic = employee_id;
                                record.advance_contract_id = advance_contract_id;
                                record.business_unit_id = business_unit_id;

                                await dbContext.SaveChangesAsync();
                            }
                            else
                            {
                                record = new waste_removal();
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
                                record.unloading_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                                record.contractor_id = contractor_id;
                                record.process_flow_id = process_flow_id;
                                record.source_shift_id = source_shift_id;
                                record.source_location_id = source_location_id;
                                record.equipment_id = equipment_id;
                                record.loading_quantity = PublicFunctions.Desimal(row.GetCell(7));
                                record.distance = PublicFunctions.Desimal(row.GetCell(8));
                                record.waste_id = waste_id;
                                record.destination_location_id = destination_location_id;
                                record.elevation = PublicFunctions.Desimal(row.GetCell(11));
                                record.uom_id = uom_id;
                                record.transport_id = transport_id;
                                record.note = PublicFunctions.IsNullCell(row.GetCell(14));
                                record.pic = employee_id;
                                record.advance_contract_id = advance_contract_id;
                                record.business_unit_id = business_unit_id;

                                dbContext.waste_removal.Add(record);
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
                        HttpContext.Session.SetString("filename", "WasteRemoval");
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

        [HttpGet("DetailsByIdDay")]
        public async Task<object> DetailsByIdDay(string Id, string Shift, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.waste_removal_item
            .Where(o => o.waste_removal_id == Id
                //&& o.shift == Shift
                && o.organization_id == CurrentUserContext.OrganizationId),
            loadOptions);
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
                    var viewData = new vw_waste_removal_item();
                    JsonConvert.PopulateObject(values, viewData);

                    if (await mcsContext.CanCreate(dbContext, nameof(waste_removal_item),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {

                        var record = new waste_removal_item();
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
                        dbContext.waste_removal_item.Add(record);
                        await dbContext.SaveChangesAsync();


                        #region Validasi

                        //Jika ada Truck yang sama Maka Gagal
                        var existTruck = dbContext.waste_removal_item
                            .Where(x => x.waste_removal_id == record.waste_removal_id)
                            .Where(x => x.truck_id == record.truck_id)
                            .Where(x => x.id != record.id)
                            .FirstOrDefaultAsync();

                        if (existTruck != null)
                        {
                            return BadRequest("Truck lebih dari 1 buah.");
                        }

                        #endregion

                        #region Recalculate Item Total Ritase | Header | Ritase | Tonnase | Quantity | Milik Item

                        var headerData = dbContext.waste_removal
                            //.Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            //.Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                            .Where(x => x.id == record.waste_removal_id).FirstOrDefault();
                        var itemData = dbContext.waste_removal_item
                            //.Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            //.Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                            .Where(x => x.waste_removal_id == record.waste_removal_id).ToList();

                        decimal? sumRitase = 0;
                        decimal? sumNetQuantity = 0;

                        foreach (var item in itemData)
                        {
                            if (await mcsContext.CanUpdate(dbContext, item.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                            {
                                if (item.jam01 == null)
                                    item.jam01 = 0;
                                if (item.jam02 == null)
                                    item.jam02 = 0;
                                if (item.jam03 == null)
                                    item.jam03 = 0;
                                if (item.jam04 == null)
                                    item.jam04 = 0;
                                if (item.jam05 == null)
                                    item.jam05 = 0;
                                if (item.jam06 == null)
                                    item.jam06 = 0;
                                if (item.jam07 == null)
                                    item.jam07 = 0;
                                if (item.jam08 == null)
                                    item.jam08 = 0;
                                if (item.jam09 == null)
                                    item.jam09 = 0;
                                if (item.jam10 == null)
                                    item.jam10 = 0;
                                if (item.jam11 == null)
                                    item.jam11 = 0;
                                if (item.jam12 == null)
                                    item.jam12 = 0;

                                item.ritase = item.jam01 + item.jam02 + item.jam03 + item.jam04 + item.jam05 + item.jam06 +
                                    item.jam07 + item.jam08 + item.jam09 + item.jam10 + item.jam11 + item.jam12;

                                sumRitase = sumRitase + item.ritase;
                                sumNetQuantity = sumNetQuantity + (item.truck_factor * item.ritase);

                                await dbContext.SaveChangesAsync();
                            }
                            else
                            {
                                return BadRequest("User is not authorized.");
                            }
                        }

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
                            if (headerData.density == null)
                                headerData.density = 1;
                            headerData.unloading_quantity = headerData.density.Value * sumNetQuantity.Value;
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
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
                catch (Exception ex)
                {
                    logger.Error(ex.InnerException ?? ex);
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
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
                    var record = dbContext.waste_removal_item
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

                            #region Recalculate Item Total Ritase | Header | Ritase | Tonnase | Quantity | Milik Item

                            var headerData = dbContext.waste_removal
                                //.Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                                //.Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Where(x => x.id == record.waste_removal_id).FirstOrDefault();
                            var itemData = dbContext.waste_removal_item
                                //.Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                                //.Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Where(x => x.waste_removal_id == record.waste_removal_id).ToList();

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
                                if (headerData.density == null)
                                    headerData.density = 1;
                                headerData.unloading_quantity = (headerData.density ?? 0) * (sumNetQuantity ?? 0);
                                //headerData.capacity = 
                                await dbContext.SaveChangesAsync();
                            }
                            else
                            {
                                return BadRequest("User is not authorized.");
                            }

                            #endregion

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
                        return BadRequest("Data update is not allowed");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.InnerException ?? ex);
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
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
                    var record = dbContext.waste_removal_item
                        .Where(o => o.id == key
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.waste_removal_item.Remove(record);
                            await dbContext.SaveChangesAsync();

                            #region Recalculate Item Total Ritase | Header | Ritase | Tonnase | Quantity | Milik Item

                            var headerData = dbContext.waste_removal
                                .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                                .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Where(x => x.id == record.waste_removal_id).FirstOrDefault();
                            var itemData = dbContext.waste_removal_item
                                .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                                .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Where(x => x.waste_removal_id == record.waste_removal_id).ToList();

                            decimal? sumRitase = 0;
                            decimal? sumNetQuantity = 0;

                            foreach (var item in itemData)
                            {
                                if (await mcsContext.CanUpdate(dbContext, item.id, CurrentUserContext.AppUserId)
                                || CurrentUserContext.IsSysAdmin)
                                {
                                    if (item.jam01 == null)
                                        item.jam01 = 0;
                                    if (item.jam02 == null)
                                        item.jam02 = 0;
                                    if (item.jam03 == null)
                                        item.jam03 = 0;
                                    if (item.jam04 == null)
                                        item.jam04 = 0;
                                    if (item.jam05 == null)
                                        item.jam05 = 0;
                                    if (item.jam06 == null)
                                        item.jam06 = 0;
                                    if (item.jam07 == null)
                                        item.jam07 = 0;
                                    if (item.jam08 == null)
                                        item.jam08 = 0;
                                    if (item.jam09 == null)
                                        item.jam09 = 0;
                                    if (item.jam10 == null)
                                        item.jam10 = 0;
                                    if (item.jam11 == null)
                                        item.jam11 = 0;
                                    if (item.jam12 == null)
                                        item.jam12 = 0;

                                    item.ritase = item.jam01 + item.jam02 + item.jam03 + item.jam04 + item.jam05 + item.jam06 +
                                        item.jam07 + item.jam08 + item.jam09 + item.jam10 + item.jam11 + item.jam12;

                                    sumRitase = sumRitase + item.ritase;
                                    sumNetQuantity = sumNetQuantity + (item.truck_factor * item.ritase);

                                    await dbContext.SaveChangesAsync();
                                }
                                else
                                {
                                    return BadRequest("User is not authorized.");
                                }
                            }

                            if (await mcsContext.CanUpdate(dbContext, headerData.id, CurrentUserContext.AppUserId)
                                || CurrentUserContext.IsSysAdmin)
                            {
                                headerData.ritase = sumRitase;
                                headerData.loading_quantity = sumNetQuantity;
                                if (headerData.density == null)
                                    headerData.density = 1;
                                headerData.unloading_quantity = headerData.density.Value * sumNetQuantity.Value;
                                await dbContext.SaveChangesAsync();
                            }
                            else
                            {
                                return BadRequest("User is not authorized.");
                            }

                            #endregion

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
    }
}

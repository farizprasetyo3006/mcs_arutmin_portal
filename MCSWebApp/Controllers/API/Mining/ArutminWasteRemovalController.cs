using DataAccess.DTO;
using DataAccess.EFCore.Repository;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NLog;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Common;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Office2010.Excel;
using Omu.ValueInjecter;
using Microsoft.EntityFrameworkCore.Storage;

namespace MCSWebApp.Controllers.API.Mining
{
    [Route("api/Mining/[controller]")]
    [ApiController]
    public class ArutminWasteRemovalController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ArutminWasteRemovalController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.arutmin_waste_removal
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.arutmin_waste_removal
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.start_time >= dt1 && o.start_time <= dt2),
                    loadOptions);
        }

        [HttpGet("DetailsById/{Id}")]
        public async Task<object> Exposed(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.arutmin_waste_removal_item
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.transaction_id == Id),
                loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.arutmin_waste_removal
                .Where(o => o.id == Id
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
                    if (await mcsContext.CanCreate(dbContext, nameof(arutmin_waste_removal),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new arutmin_waste_removal();
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
                        record.transaction_id = "WR-" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                        dbContext.arutmin_waste_removal.Add(record);

                        await dbContext.SaveChangesAsync();

                        #region Add ItemData

                        var equipment_group_item_data = dbContext.equipment_group_item
                        .Where(o => o.fleet_id == record.fleet_id && o.organization_id == CurrentUserContext.OrganizationId)
                        .ToList();

                        foreach (var truck in equipment_group_item_data)
                        {
                            CreateItemData(tx,record, truck);
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
                catch (Exception ex)
                {
                    logger.Error(ex.InnerException ?? ex);
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        private async void CreateItemData(IDbContextTransaction tx, arutmin_waste_removal wr_data, equipment_group_item truck)
        {
            try
            {
                var record = new arutmin_waste_removal_item();
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
                record.transaction_id = wr_data.transaction_id;
                record.transport_id = truck.transport_id;
                record.ritase = 0;
                record.truck_factor = Convert.ToDecimal(truck.truck_factor);
                record.tonnage = record.ritase * record.truck_factor;
                dbContext.arutmin_waste_removal_item.Add(record);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                BadRequest(ex.InnerException?.Message ?? ex.Message);
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
                    var record = dbContext.arutmin_waste_removal
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
                var record = dbContext.arutmin_waste_removal
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.arutmin_waste_removal.Remove(record);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        return BadRequest("User is not authorized.");
                    }
                }

                var itemRecord = dbContext.arutmin_waste_removal_item
                    .Where(o => o.transaction_id == record.transaction_id).ToList();

                foreach (var item in itemRecord)
                {
                    if (item != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, record.transaction_id, CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.arutmin_waste_removal_item.Remove(item);
                            await dbContext.SaveChangesAsync();
                        }
                        else { return BadRequest("User is not authorized."); }
                    }
                    Ok();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("InsertItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertItemData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var record = new arutmin_waste_removal_item();

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(equipment_group_item),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        #region Add record

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
                        record.business_unit_id = CurrentUserContext.BusinessUnitId;

                        #endregion

                        dbContext.arutmin_waste_removal_item.Add(record);

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

        [HttpPut("UpdateItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateItemData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            arutmin_waste_removal_item record;
            var tx_id = string.Empty;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.arutmin_waste_removal_item
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

                            record.InjectFrom(e);
                            record.tonnage = record.ritase * record.truck_factor;
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;
                            record.is_active = true;

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();

                            tx_id = record.transaction_id;
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

            UpdateFromItem(record, tx_id);

            return Ok(record);
        }

        [HttpDelete("DeleteItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteItemData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var record = dbContext.arutmin_waste_removal_item
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.arutmin_waste_removal_item.Remove(record);
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

        //-------------------[Other API Call]------------------------------------

        [HttpGet("WasteDetailsById")]
        public async Task<object> WasteDetailsById(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var record = await dbContext.waste
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                return Ok(record);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("WasteIdLookup")]
        public async Task<object> WasteIdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.waste
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.waste_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        //-------------------[Other API Call]------------------------------------

        //-------------------[Other Function]------------------------------------
        private async void UpdateFromItem(arutmin_waste_removal_item itemRecord, string tx_id)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                var listItem = dbContext.arutmin_waste_removal_item
                    .Where(x => x.transaction_id == itemRecord.transaction_id).ToListAsync().Result;

                decimal? totalRitase = 0;
                decimal? totalTonnage = 0;

                foreach (var item in listItem)
                {
                    if (item != null)
                    {
                        totalRitase += item.ritase;
                        totalTonnage += item.tonnage;
                    }
                }

                var header = dbContext.arutmin_waste_removal
                    .Where(x => x.transaction_id == itemRecord.transaction_id).FirstOrDefaultAsync().Result;
                header.ritase = totalRitase.Value;
                header.tonnage = totalTonnage.Value;
                header.volume_bcm = header.ritase * header.tonnage;
                header.modified_by = CurrentUserContext.AppUserId;
                header.modified_on = DateTime.Now;
                header.is_active = true;
                await dbContext.SaveChangesAsync();
                await tx.CommitAsync();
                Ok(header);
            }
        }
        //-------------------[Other Function]------------------------------------

    }
};
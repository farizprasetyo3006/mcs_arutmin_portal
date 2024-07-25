using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using System.Globalization;
using Common;

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class ProductionPlanDetailBackupController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ProductionPlanDetailBackupController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("ByProductionPlanId/{Id}")]
        public async Task<object> ByProductionPlanId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_production_plan_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.production_plan_id == Id)
                    .OrderBy(o => o.month_index),
                    loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ByProductionPlanIdMonth/{Id}/{Month}")]
        public async Task<object> ByProductionPlanId(string Id, int month, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.production_plan_history.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.production_plan_id == Id && o.month_index == month),
                    loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.production_plan_detail
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
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
                dbContext.production_plan_detail.Where(o => o.id == Id),
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
                    if (await mcsContext.CanCreate(dbContext, nameof(production_plan_detail),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new production_plan_detail();
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
                        record.previous_quantity = record.quantity;

                        dbContext.production_plan_detail.Add(record);
                        await dbContext.SaveChangesAsync();

                        decimal subtotal = 0;

                        // Quantity
                        var totalQty = dbContext.production_plan_detail
                            .Where(o => o.production_plan_id == record.production_plan_id)
                            .Sum(s => s.quantity);
                        if (totalQty != null)
                        {
                            subtotal = (decimal)totalQty.GetValueOrDefault();

                            var record3 = dbContext.production_plan
                                .Where(o => o.id == record.production_plan_id)
                                .FirstOrDefault();
                            if (record3 != null)
                            {
                                record3.quantity = subtotal;

                                await dbContext.SaveChangesAsync();
                            }
                        }

                        // Prev Quantity
                        var totalPrevQty = dbContext.production_plan_detail
                            .Where(o => o.production_plan_id == record.production_plan_id)
                            .Sum(s => s.previous_quantity);
                        if (totalQty != null)
                        {
                            subtotal = (decimal)totalPrevQty.GetValueOrDefault();

                            var record3 = dbContext.production_plan
                                .Where(o => o.id == record.production_plan_id)
                                .FirstOrDefault();
                            if (record3 != null)
                            {
                                record3.previous_quantity = subtotal;

                                await dbContext.SaveChangesAsync();
                            }
                        }

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

        [HttpPut("UpdateData_old")]
        public async Task<IActionResult> UpdateData_old([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.production_plan_detail
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        var prevQty = record.quantity;
                        JsonConvert.PopulateObject(values, record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        record.previous_quantity = prevQty;

                        await dbContext.SaveChangesAsync();

                        decimal subtotal = 0;

                        // Quantity
                        var totalQty = dbContext.production_plan_detail
                            .Where(o => o.production_plan_id == record.production_plan_id)
                            .Sum(s => s.quantity);
                        if (totalQty != null)
                        {
                            subtotal = (decimal)totalQty.GetValueOrDefault();

                            var record3 = dbContext.production_plan
                                .Where(o => o.id == record.production_plan_id)
                                .FirstOrDefault();
                            if (record3 != null)
                            {
                                record3.quantity = subtotal;

                                await dbContext.SaveChangesAsync();
                            }
                        }

                        var details = await dbContext.production_plan_detail
                            .Where(o => o.production_plan_id == record.production_plan_id
                                && o.id != record.id)
                            .ToListAsync();
                        foreach(var detail in details)
                        {
                            detail.previous_quantity = detail.quantity;
                        }
                        await dbContext.SaveChangesAsync();

                        // Prev Quantity
                        var totalPrevQty = dbContext.production_plan_detail
                            .Where(o => o.production_plan_id == record.production_plan_id)
                            .Sum(s => s.previous_quantity);
                        if (totalQty != null)
                        {
                            subtotal = (decimal)totalPrevQty.GetValueOrDefault();

                            var record3 = dbContext.production_plan
                                .Where(o => o.id == record.production_plan_id)
                                .FirstOrDefault();
                            if (record3 != null)
                            {
                                record3.previous_quantity = subtotal;

                                await dbContext.SaveChangesAsync();
                            }
                        }

                        await tx.CommitAsync();

                        return Ok(record);
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
        }

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            decimal selisih = 0;
            decimal subtotal = 0;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.production_plan_detail
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            //--- save to detail history
                            var recHistory = new production_plan_history();
                            recHistory.id = Guid.NewGuid().ToString("N");
                            recHistory.created_by = CurrentUserContext.AppUserId;
                            recHistory.created_on = DateTime.Now;
                            recHistory.modified_by = null;
                            recHistory.modified_on = null;
                            recHistory.is_active = true;
                            recHistory.is_default = null;
                            recHistory.is_locked = null;
                            recHistory.entity_id = null;
                            recHistory.owner_id = CurrentUserContext.AppUserId;
                            recHistory.organization_id = CurrentUserContext.OrganizationId;

                            recHistory.production_plan_id = record.production_plan_id;
                            recHistory.month_index = record.month_index;
                            recHistory.quantity = record.quantity;
                            recHistory.previous_quantity = record.previous_quantity;

                            dbContext.production_plan_history.Add(recHistory);

                            var prevQty = record.quantity;

                            //--- update detail
                            var e = new entity();
                            e.InjectFrom(record);

                            JsonConvert.PopulateObject(values, record);

                            record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;
                            record.previous_quantity = prevQty;
                            
                            selisih = (decimal)record.quantity - (decimal)prevQty;

                            var recordB = dbContext.production_plan
                                    .Where(o => o.id == record.production_plan_id)
                                    .FirstOrDefault();
                            if (recordB != null)
                            {
                                subtotal = (decimal)recordB.quantity + selisih;
                                recordB.previous_quantity = recordB.quantity;
                                recordB.quantity = subtotal;
                            }

                            record.percentage = record.quantity * 100 / subtotal;

                            await dbContext.SaveChangesAsync();
                            //--------------------------------------------------------------

                            //// Quantity
                            //var totalQty = dbContext.production_plan_detail
                            //    .Where(o => o.production_plan_id == record.production_plan_id)
                            //    .Sum(s => s.quantity);
                            //if (totalQty != null)
                            //{
                            //    subtotal = (decimal)totalQty.GetValueOrDefault();

                            //    var record3 = dbContext.production_plan
                            //        .Where(o => o.id == record.production_plan_id)
                            //        .FirstOrDefault();
                            //    if (record3 != null)
                            //    {
                            //        record3.quantity = subtotal;

                            //        await dbContext.SaveChangesAsync();
                            //    }
                            //}

                            //var details = await dbContext.production_plan_detail
                            //    .Where(o => o.production_plan_id == record.production_plan_id
                            //        && o.id != record.id)
                            //    .ToListAsync();
                            //foreach (var detail in details)
                            //{
                            //    detail.previous_quantity = detail.quantity;
                            //}
                            //await dbContext.SaveChangesAsync();

                            //// Prev Quantity
                            //var totalPrevQty = dbContext.production_plan_detail
                            //    .Where(o => o.production_plan_id == record.production_plan_id)
                            //    .Sum(s => s.previous_quantity);
                            //if (totalQty != null)
                            //{
                            //    subtotal = (decimal)totalPrevQty.GetValueOrDefault();

                            //    var record3 = dbContext.production_plan
                            //        .Where(o => o.id == record.production_plan_id)
                            //        .FirstOrDefault();
                            //    if (record3 != null)
                            //    {
                            //        record3.previous_quantity = subtotal;

                            //        await dbContext.SaveChangesAsync();
                            //    }
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
                        return BadRequest("No default organization");
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

            try
            {
                var record = dbContext.production_plan_detail
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.production_plan_detail.Remove(record);
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

        [HttpGet("MonthIndexLookup")]
        public object MonthIndexLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var months = new Dictionary<int, string>();
                for(var i = 1; i <= 12; i++)
                {
                    months.Add(i, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i));
                }
                var lookup = months
                    .Select(o => new { Value = o.Key, Text = o.Value });
                return DataSourceLoader.Load(lookup, loadOptions);
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
                var record = await dbContext.production_plan_detail
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                return Ok(record);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] production_plan_detail Record)
        {
            try
            {
                var record = dbContext.production_plan_detail
                    .Where(o => o.id == Record.id)
                    .FirstOrDefault();
                if (record != null)
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
                    record = new production_plan_detail();
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

                    dbContext.production_plan_detail.Add(record);
                    await dbContext.SaveChangesAsync();

                    return Ok(record);
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
                var record = dbContext.production_plan_detail
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.production_plan_detail.Remove(record);
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

    }
}

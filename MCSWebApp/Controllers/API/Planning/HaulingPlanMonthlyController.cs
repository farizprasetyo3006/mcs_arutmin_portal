using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataAccess.DTO;
using NLog;
using DataAccess.EFCore.Repository;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Common;
using System.Dynamic;
using PetaPoco;
using BusinessLogic.Entity;

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class HaulingPlanMonthlyController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public HaulingPlanMonthlyController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("ByHaulingPlanId/{Id}")]
        public async Task<object> ByHaulingPlanId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_hauling_plan_monthly.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.hauling_plan_id == Id)
                    .OrderBy(o => o.month_id),
                    loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("History/ByHaulingPlanMonthlyId/{Id}")]
        public async Task<object> HistoryByHaulingPlanMonthlyId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_hauling_plan_monthly_history.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.hauling_plan_monthly_id == Id)
                    .OrderBy(o => o.month_id),
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
                return await DataSourceLoader.LoadAsync(dbContext.vw_hauling_plan_monthly
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
                dbContext.hauling_plan_monthly.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpGet("MonthIndexLookup")]
        public object MonthIndexLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var months = new Dictionary<int, string>();
                for (var i = 1; i <= 12; i++)
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

        private bool CheckQuantity(int new_spc_qty, string spd_id)
        {
            bool retval = true;

            var record_spd = dbContext.sales_plan_detail
            .Where(o => o.id == spd_id)
            .FirstOrDefault();

            if (record_spd != null)
            {

                var records_spc = dbContext.sales_plan_customer
                .Where(o => o.sales_plan_detail_id == spd_id)
                ;

                int total_qty_customers = 0;
                foreach (var record_spc in records_spc)
                {
                    total_qty_customers += (int)record_spc.quantity;
                }

                if (new_spc_qty > (record_spd.quantity - total_qty_customers))
                {
                    retval = false;
                }
                else
                {
                    retval = true;
                }

            }
            else
            {
                return false;
            }

            return retval;
        }

        [HttpPut("UpdateData")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                using (var tx = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var record = dbContext.hauling_plan_monthly
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                        if (record != null)
                        {
                            if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                                || CurrentUserContext.IsSysAdmin)
                            {
                                #region History
                                var history = new hauling_plan_monthly_history();
                                history.InjectFrom(record);
                                history.id = Guid.NewGuid().ToString("N");
                                history.hauling_plan_monthly_id = record.id;
                                history.created_by = CurrentUserContext.AppUserId;
                                history.created_on = DateTime.Now; 
                                dbContext.hauling_plan_monthly_history.Add(history);
                                #endregion

                                var e = new entity();
                                e.InjectFrom(record);

                                JsonConvert.PopulateObject(values, record);

                                record.InjectFrom(e);
                                record.modified_by = CurrentUserContext.AppUserId;
                                record.modified_on = DateTime.Now;

                                await dbContext.SaveChangesAsync();


                                var haulingRecord = await dbContext.vw_hauling_plan.FirstOrDefaultAsync(o => o.id == record.hauling_plan_id);
                                if (haulingRecord == null)
                                    return BadRequest("No Hauling Plan Record.");

                                #region Daily all daily records
                                var haulingDailyRecords = await dbContext.hauling_plan_daily.Where(o => o.hauling_plan_monthly_id == record.id).ToListAsync();
                                foreach(var item in haulingDailyRecords)
                                    dbContext.hauling_plan_daily.Remove(item);
                                #endregion

                                int days = DateTime.DaysInMonth(int.Parse(haulingRecord.plan_year), (int)record.month_id);

                                decimal avgQuantity = 0;
                                if (record.quantity > 0)
                                    avgQuantity = (decimal)(record.quantity / days);

                                for (var i = 1; i <= days; i++)
                                {
                                    var dailyPlanRecord = new hauling_plan_daily();
                                    dailyPlanRecord.id = Guid.NewGuid().ToString("N");
                                    dailyPlanRecord.created_by = CurrentUserContext.AppUserId;
                                    dailyPlanRecord.created_on = DateTime.Now;
                                    dailyPlanRecord.modified_by = null;
                                    dailyPlanRecord.modified_on = null;
                                    dailyPlanRecord.is_active = true;
                                    dailyPlanRecord.is_default = null;
                                    dailyPlanRecord.is_locked = null;
                                    dailyPlanRecord.entity_id = null;
                                    dailyPlanRecord.owner_id = CurrentUserContext.AppUserId;
                                    dailyPlanRecord.organization_id = CurrentUserContext.OrganizationId;

                                    dailyPlanRecord.hauling_plan_monthly_id = record.id;
                                    dailyPlanRecord.daily_date = new DateTime(int.Parse(haulingRecord.plan_year), (int)record.month_id, i);
                                    dailyPlanRecord.quantity = avgQuantity;

                                    dbContext.hauling_plan_daily.Add(dailyPlanRecord);
                                    await dbContext.SaveChangesAsync();
                                }

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
                        await tx.RollbackAsync();
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
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
                var record = dbContext.hauling_plan_monthly
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.hauling_plan_monthly.Remove(record);
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


        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.hauling_plan_monthly
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
        public async Task<IActionResult> SaveData([FromBody] hauling_plan_monthly Record)
        {
            try
            {
                var record = dbContext.hauling_plan_monthly
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
                else if (await mcsContext.CanCreate(dbContext, nameof(hauling_plan_monthly),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new hauling_plan_monthly();
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

                    dbContext.hauling_plan_monthly.Add(record);
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
                var record = dbContext.hauling_plan_monthly
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.hauling_plan_monthly.Remove(record);
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

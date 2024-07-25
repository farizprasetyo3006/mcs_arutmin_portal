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
using Newtonsoft.Json;
using Omu.ValueInjecter;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Common;
using System.Dynamic;
using PetaPoco;
using BusinessLogic.Entity;
using BusinessLogic;
using PetaPoco.Providers;

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class BargingPlanMonthlyController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public BargingPlanMonthlyController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                    dbContext.vw_barging_plan_monthly.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.barging_plan_id == Id)
                    .OrderBy(o => o.month_id),
                    loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("History/ByHaulingPlanMonthlyId")]
        public async Task<object> HistoryByHaulingPlanMonthlyId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_barging_plan_monthly_history.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.barging_plan_monthly_id == Id)
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
                return await DataSourceLoader.LoadAsync(dbContext.vw_barging_plan_monthly
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
                dbContext.barging_plan_monthly.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpGet("GetResultById/{Id}")]
        public async Task<StandardResult> GetResultById(string Id)
        {
            var result = new StandardResult();
            try
            {
                var monthlyRecord = dbContext.barging_plan_monthly.FirstOrDefault(o => o.id == Id);
                if (monthlyRecord == null)
                {
                    result.Message = "Monthy Plan Record is not exist.";
                    result.Success = false;
                    return result;
                }

                var total_quantity_updated = await dbContext.barging_plan_monthly
                    .Where(o => o.barging_plan_id == monthlyRecord.barging_plan_id).SumAsync(r => r.quantity ?? 0);

                result.Data = new Dictionary<string, decimal?>
                    {
                        { "monthly_quantity_updated", monthlyRecord.quantity ?? 0 },
                        { "total_quantity_updated", total_quantity_updated }
                    };
                result.Success = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                result.Success = false;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpGet("FetchById/{Id}")]
        public async Task<StandardResult> FetchById(string Id)
        {
            var result = new StandardResult();
            try
            {
                var monthlyRecord = dbContext.barging_plan_monthly.FirstOrDefault(o => o.id == Id);
                if (monthlyRecord == null)
                {
                    result.Message = "Monthy Plan Record is not exist.";
                    result.Success = false;
                    return result;
                }

                #region Get Barging Plan Record
                var bargingPlanRecord = await dbContext.barging_plan
                                .FirstOrDefaultAsync(r => r.id == monthlyRecord.barging_plan_id);
                #endregion

                #region Get Shipment Record
                decimal totalShipmentQty = dbContext.shipment_plan
                                .Where(r => r.shipment_year == bargingPlanRecord.master_list_id && r.month_id == monthlyRecord.month_id)
                                .Select(t => t.qty_sp ?? 0).Sum();
                #endregion

                var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                var db = new Database(connectionString, new PostgreSQLDatabaseProvider());
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        var sql = Sql.Builder.Append("DELETE FROM barging_plan_daily");
                        sql.Append("WHERE barging_plan_monthly_id = @0", Id);
                        await db.ExecuteAsync(sql);
                        logger.Debug(db.LastCommand);

                        var yearRecord = dbContext.master_list
                            .FirstOrDefault(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == bargingPlanRecord.master_list_id);

                        #region Barging Daily
                        int days = DateTime.DaysInMonth(int.Parse(yearRecord.item_in_coding), (int)monthlyRecord.month_id);
                        for (var j = 1; j <= days; j++)
                        {
                            var dailyPlanRecord = new barging_plan_daily();
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

                            dailyPlanRecord.barging_plan_monthly_id = monthlyRecord.id;
                            dailyPlanRecord.daily_date = new DateTime(int.Parse(yearRecord.item_in_coding), (int)monthlyRecord.month_id, j);
                            dailyPlanRecord.quantity = totalShipmentQty > 0 ? (totalShipmentQty / days) : 0;
                            dailyPlanRecord.operational_hours = 20;
                            dailyPlanRecord.loading_rate = dailyPlanRecord.quantity > 0 ? (dailyPlanRecord.quantity / dailyPlanRecord.operational_hours) : 0;

                            dbContext.barging_plan_daily.Add(dailyPlanRecord);
                            await dbContext.SaveChangesAsync();
                        }
                        #endregion

                        monthlyRecord.quantity = totalShipmentQty;
                        await dbContext.SaveChangesAsync();

                        result.Success = true;
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(db.LastCommand);
                        logger.Error(ex);
                        result.Success = false;
                        result.Message = ex.InnerException?.Message ?? ex.Message;
                    }

                    if (result.Success)
                        tx.Complete();

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                result.Success = false;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
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
                        var record = dbContext.barging_plan_monthly
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                        if (record != null)
                        {
                            if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                                || CurrentUserContext.IsSysAdmin)
                            {
                                #region History
                                var history = new barging_plan_monthly_history();
                                history.InjectFrom(record);
                                history.id = Guid.NewGuid().ToString("N");
                                history.barging_plan_monthly_id = record.id;
                                history.created_by = CurrentUserContext.AppUserId;
                                history.created_on = DateTime.Now; 
                                dbContext.barging_plan_monthly_history.Add(history);
                                #endregion


                                var e = new entity();
                                e.InjectFrom(record);

                                JsonConvert.PopulateObject(values, record);

                                record.InjectFrom(e);
                                record.modified_by = CurrentUserContext.AppUserId;
                                record.modified_on = DateTime.Now;

                                await dbContext.SaveChangesAsync();


                                var planRecord = await dbContext.vw_barging_plan.FirstOrDefaultAsync(o => o.id == record.barging_plan_id);
                                if (planRecord == null)
                                    return BadRequest("No Barging Plan Record.");

                                //#region Daily all daily records
                                //var planDailyRecords = await dbContext.barging_plan_daily.Where(o => o.barging_plan_monthly_id == record.id).ToListAsync();
                                //foreach(var item in planDailyRecords)
                                //    dbContext.barging_plan_daily.Remove(item);
                                //#endregion

                                int days = DateTime.DaysInMonth(int.Parse(planRecord.plan_year), (int)record.month_id);

                                #region Barging Daily

                                var recDaily = dbContext.barging_plan_daily
                                   .Where(o => o.barging_plan_monthly_id == key);
                                if (recDaily.Count() > 0)
                                    dbContext.barging_plan_daily.RemoveRange(recDaily);

                                decimal avgQuantity = (decimal)(record.quantity??0) > 0 ? ((decimal)(record.quantity??0) / days) : 0;

                                for (var j = 1; j <= days; j++)
                                {
                                    var dailyPlanRecord = new barging_plan_daily();

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

                                    dailyPlanRecord.barging_plan_monthly_id = record.id;
                                    dailyPlanRecord.daily_date = new DateTime(int.Parse(planRecord.plan_year), (int)record.month_id, j);
                                    //dailyPlanRecord.quantity = record.quantity > 0 ? (record.quantity / days) : 0;
                                    dailyPlanRecord.quantity = avgQuantity;
                                    dailyPlanRecord.operational_hours = 20;
                                    dailyPlanRecord.loading_rate = dailyPlanRecord.quantity > 0 ? (dailyPlanRecord.quantity / dailyPlanRecord.operational_hours) : 0;

                                    dbContext.barging_plan_daily.Add(dailyPlanRecord);
                                    await dbContext.SaveChangesAsync();
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
                var record = dbContext.barging_plan_monthly
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.barging_plan_monthly.Remove(record);
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
                var record = await dbContext.barging_plan_monthly
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
        public async Task<IActionResult> SaveData([FromBody] barging_plan_monthly Record)
        {
            try
            {
                var record = dbContext.barging_plan_monthly
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
                else if (await mcsContext.CanCreate(dbContext, nameof(barging_plan_monthly),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new barging_plan_monthly();
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

                    dbContext.barging_plan_monthly.Add(record);
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
                var record = dbContext.barging_plan_monthly
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.barging_plan_monthly.Remove(record);
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

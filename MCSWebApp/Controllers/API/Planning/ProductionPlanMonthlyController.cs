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
using BusinessLogic;

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class ProductionPlanMonthlyController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ProductionPlanMonthlyController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                    dbContext.vw_production_plan_monthly.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.production_plan_id == Id)
                    .OrderBy(o => o.month_id),
                    loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("History/ByProductionPlanMonthlyId/{Id}")]
        public async Task<object> HistoryByProductionPlanMonthlyId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_production_plan_monthly_history.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.production_plan_monthly_id == Id)
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
                return await DataSourceLoader.LoadAsync(dbContext.vw_production_plan_monthly
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
                dbContext.production_plan_monthly.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpGet("GetTotalQuantityByPlanId/{Id}")]
        public async Task<IActionResult> GetTotalQuantityByPlanId(string Id)
        {
            try
            {
                var record = await dbContext.production_plan_monthly
                    .Where(o => o.production_plan_id == Id).SumAsync(r => r.quantity ?? 0);
                return Ok(record);
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
                        var record = dbContext.production_plan_monthly
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                        if (record != null)
                        {
                            if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                                || CurrentUserContext.IsSysAdmin)
                            {
                                #region History
                                var history = new production_plan_monthly_history();
                                history.InjectFrom(record);
                                history.id = Guid.NewGuid().ToString("N");
                                history.production_plan_monthly_id = record.id;
                                history.created_by = CurrentUserContext.AppUserId;
                                history.created_on = DateTime.Now; 
                                dbContext.production_plan_monthly_history.Add(history);
                                #endregion


                                var e = new entity();
                                e.InjectFrom(record);

                                JsonConvert.PopulateObject(values, record);

                                record.InjectFrom(e);
                                record.modified_by = CurrentUserContext.AppUserId;
                                record.modified_on = DateTime.Now;

                                await dbContext.SaveChangesAsync();


                                var planRecord = await dbContext.vw_production_plan.FirstOrDefaultAsync(o => o.id == record.production_plan_id);
                                if (planRecord == null)
                                    return BadRequest("No Production Plan Record.");

                                #region Daily all daily records
                                var planDailyRecords = await dbContext.production_plan_daily.Where(o => o.production_plan_monthly_id == record.id).ToListAsync();
                                foreach(var item in planDailyRecords)
                                    dbContext.production_plan_daily.Remove(item);
                                #endregion

                                int days = DateTime.DaysInMonth(int.Parse(planRecord.plan_year), (int)record.month_id);

                                decimal avgQuantity = 0;
                                if (record.quantity > 0)
                                    avgQuantity = (decimal)(record.quantity / days);

                                for (var i = 1; i <= days; i++)
                                {
                                    var dailyPlanRecord = new production_plan_daily();
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

                                    dailyPlanRecord.production_plan_monthly_id = record.id;
                                    dailyPlanRecord.daily_date = new DateTime(int.Parse(planRecord.plan_year), (int)record.month_id, i);
                                    dailyPlanRecord.quantity = avgQuantity;

                                    dbContext.production_plan_daily.Add(dailyPlanRecord);
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
                var record = dbContext.production_plan_monthly
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.production_plan_monthly.Remove(record);
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
                var record = await dbContext.production_plan_monthly
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                return Ok(record);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("GetResultById/{Id}")]
        public async Task<StandardResult> GetResultById(string Id)
        {
            var result = new StandardResult();
            try
            {
                var monthlyRecord = dbContext.production_plan_monthly.FirstOrDefault(o => o.id == Id);
                if (monthlyRecord == null)
                {
                    result.Message = "Monthy Plan Record is not exist.";
                    result.Success = false;
                    return result;
                }

                var total_quantity_updated = await dbContext.production_plan_monthly
                    .Where(o => o.production_plan_id == monthlyRecord.production_plan_id).SumAsync(r => r.quantity ?? 0);

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

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] production_plan_monthly Record)
        {
            try
            {
                var record = dbContext.production_plan_monthly
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
                    record = new production_plan_monthly();
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

                    dbContext.production_plan_monthly.Add(record);
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
                var record = dbContext.production_plan_monthly
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.production_plan_monthly.Remove(record);
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

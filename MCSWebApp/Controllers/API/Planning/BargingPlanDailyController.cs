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
using Microsoft.AspNetCore.Http;
using Omu.ValueInjecter;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Common;
using System.Dynamic;
using PetaPoco;
using BusinessLogic.Entity;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class BargingPlanDailyController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public BargingPlanDailyController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("ByHaulingMonthlyId/{Id}")]
        public async Task<object> BySalesPlanDetailId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_barging_plan_daily.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.barging_plan_monthly_id == Id)
                    .OrderBy(o => o.created_on),
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
                return await DataSourceLoader.LoadAsync(dbContext.vw_barging_plan_daily
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
                dbContext.barging_plan_daily.Where(o => o.id == Id),
                loadOptions);
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
                        var record = dbContext.barging_plan_daily
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
                                record.modified_by = CurrentUserContext.AppUserId;
                                record.modified_on = DateTime.Now;
                                record.loading_rate = 0;
                                if (record.quantity > 0 && record.operational_hours > 0)
                                    record.loading_rate = record.quantity / record.operational_hours;
                                await dbContext.SaveChangesAsync();

                                var barging_plan_id = "";

                                #region Monthly Record
                                    var temp = record.daily_date.Value.Month;

                                var monthlyRecord = dbContext.barging_plan_monthly
                                .FirstOrDefault(o => o.id == record.barging_plan_monthly_id && o.month_id == record.daily_date.Value.Month);

                                if (monthlyRecord != null)
                                {
                                    barging_plan_id = monthlyRecord.barging_plan_id;

                                    var bargingPlanDailyRecords = await dbContext.barging_plan_daily
                                        .Where(o => o.barging_plan_monthly_id == record.barging_plan_monthly_id).ToListAsync();

                                    if (bargingPlanDailyRecords.Count > 0)
                                        monthlyRecord.quantity = decimal.Parse(bargingPlanDailyRecords.Select(r => r.quantity ?? 0).Sum().ToString("0.#####"));
                                    await dbContext.SaveChangesAsync();
                                }
                                #endregion
                                #region History
                                var history = new barging_plan_monthly_history();
                                    history.InjectFrom(monthlyRecord);
                                    history.id = Guid.NewGuid().ToString("N");
                                    history.barging_plan_monthly_id = monthlyRecord.id;
                                    history.created_by = CurrentUserContext.AppUserId;
                                    history.created_on = DateTime.Now;
                                    dbContext.barging_plan_monthly_history.Add(history);
                                    #endregion

                                #region Update Total Quantity
                                //if (string.IsNullOrEmpty(barging_plan_id))
                                //{
                                //    var total = dbContext.barging_plan_monthly
                                //             .Where(o => o.barging_plan_id == barging_plan_id).Sum(r => r.quantity ?? 0);

                                //    var bargingPlanRecord = dbContext.barging_plan.FirstOrDefault(r => r.id == barging_plan_id);

                                //    bargingPlanRecord.
                                //    //monthlyRecord.quantity = totalQuantityDaily;
                                //    await dbContext.SaveChangesAsync();
                                //}
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
                var record = dbContext.barging_plan_daily
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.barging_plan_daily.Remove(record);
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
                var record = await dbContext.barging_plan_daily
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
        public async Task<IActionResult> SaveData([FromBody] sales_plan_customer Record)
        {
            try
            {
                var record = dbContext.barging_plan_daily
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
                else if (await mcsContext.CanCreate(dbContext, nameof(barging_plan_daily),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new barging_plan_daily();
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

                    dbContext.barging_plan_daily.Add(record);
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
                var record = dbContext.barging_plan_daily
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.barging_plan_daily.Remove(record);
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

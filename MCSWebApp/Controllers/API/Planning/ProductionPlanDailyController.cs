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
using Common;
using System.Dynamic;
using PetaPoco;
using BusinessLogic.Entity;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class ProductionPlanDailyController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ProductionPlanDailyController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("ByProductionPlanDailyId/{Id}")]
        public async Task<object> ByProductionPlanDailyId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_production_plan_daily.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.production_plan_monthly_id == Id)
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
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_production_plan_daily.Where(o => o.organization_id == CurrentUserContext.OrganizationId),
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
                dbContext.production_plan_daily.Where(o => o.id == Id),
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
                        var record = dbContext.production_plan_daily
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
                                /*var newQuantity =  record.quantity - old;
                                var newQuantity1 = newQuantity < 0 ? -newQuantity : newQuantity;*/
                                await dbContext.SaveChangesAsync();
                                var Daily = dbContext.production_plan_daily.Where (o => o.production_plan_monthly_id == record.production_plan_monthly_id).ToList();
                                decimal total = 0;
                                foreach (var item in Daily)
                                {
                                    total += (decimal)item.quantity;
                                }
                                var monthly = dbContext.production_plan_monthly.Where(o => o.id == record.production_plan_monthly_id).FirstOrDefault();
                                var lastQuantity = monthly.quantity;
                                
                                monthly.quantity = Math.Round(total, 3);
                                await dbContext.SaveChangesAsync();

                                var history = new production_plan_monthly_history();
                                history.InjectFrom(monthly);
                                history.id = Guid.NewGuid().ToString("N");
                                history.quantity = lastQuantity;
                                history.production_plan_id = monthly.production_plan_id;
                                history.production_plan_monthly_id = monthly.id;
                                history.created_by = CurrentUserContext.AppUserId;
                                history.created_on = DateTime.Now;
                                dbContext.production_plan_monthly_history.Add(history);
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
                var record = dbContext.production_plan_daily
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.production_plan_daily.Remove(record);
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
                var record = await dbContext.production_plan_daily
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
                var record = dbContext.production_plan_daily
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
                else if (await mcsContext.CanCreate(dbContext, nameof(production_plan_daily),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new production_plan_daily();
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

                    dbContext.production_plan_daily.Add(record);
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
                var record = dbContext.production_plan_daily
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.production_plan_daily.Remove(record);
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

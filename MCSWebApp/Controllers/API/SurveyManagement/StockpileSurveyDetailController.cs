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
using Common;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Controllers.API.SurveyManagement
{
    [Route("api/SurveyManagement/[controller]")]
    [ApiController]
    public class StockpileSurveyDetailController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public StockpileSurveyDetailController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("BySurveyId/{Id}")]
        public async Task<object> DataGrid(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.survey_detail
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.survey_id == Id),
                loadOptions);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.survey_detail
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.survey_detail.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            survey_detail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(survey_detail),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new survey_detail();
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

                        dbContext.survey_detail.Add(record);
                        await dbContext.SaveChangesAsync();

                        var survey = await dbContext.survey.Where(o => o.id == record.survey_id)
                            .FirstOrDefaultAsync();
                        if (survey != null)
                        {
                            var sum = await dbContext.survey_detail.Where(o => o.survey_id == survey.id)
                                .SumAsync(o => o.quantity);
                            survey.quantity = sum;
                            await dbContext.SaveChangesAsync();

                            var recSLD = new stockpile_location_detail();
                            recSLD.id = Guid.NewGuid().ToString("N");
                            recSLD.created_by = CurrentUserContext.AppUserId;
                            recSLD.created_on = DateTime.Now;
                            recSLD.modified_by = null;
                            recSLD.modified_on = null;
                            recSLD.is_active = true;
                            recSLD.is_default = null;
                            recSLD.is_locked = null;
                            recSLD.entity_id = null;
                            recSLD.owner_id = CurrentUserContext.AppUserId;
                            recSLD.organization_id = CurrentUserContext.OrganizationId;
                            recSLD.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                            recSLD.stockpile_location_id = survey.stock_location_id;
                            recSLD.product_id = record.product_id;
                            recSLD.contractor_id = record.contractor_id;
                            recSLD.quantity = record.quantity;
                            recSLD.presentage = record.percentage;
                            recSLD.survey_date = survey.survey_date;

                            dbContext.stockpile_location_detail.Add(recSLD);
                            await dbContext.SaveChangesAsync();
                        }

                        await tx.CommitAsync();
                    }
                    else
                    {
                        return BadRequest("User is not authorized.");
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

            survey_detail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.survey_detail.Where(o => o.id == key).FirstOrDefault();
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

                            var survey = await dbContext.survey.Where(o => o.id == record.survey_id)
                                .FirstOrDefaultAsync();
                            if (survey != null)
                            {
                                var sum = await dbContext.survey_detail.Where(o => o.survey_id == survey.id)
                                    .SumAsync(o => o.quantity);
                                survey.quantity = sum;
                                await dbContext.SaveChangesAsync();

                                var recSLD = await dbContext.stockpile_location_detail
                                    .Where(o => o.stockpile_location_id == survey.stock_location_id).FirstOrDefaultAsync();
                                if (recSLD != null)
                                {
                                    recSLD.modified_by = CurrentUserContext.AppUserId;
                                    recSLD.modified_on = DateTime.Now;

                                    recSLD.product_id = record.product_id;
                                    recSLD.contractor_id = record.contractor_id;
                                    recSLD.quantity = record.quantity;
                                    recSLD.presentage = record.percentage;
                                    recSLD.survey_date = survey.survey_date;

                                    await dbContext.SaveChangesAsync();
                                }
                            }

                            await tx.CommitAsync();
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else
                    {
                        return BadRequest("Record is not found.");
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }

            if (record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.survey_detail();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }

            return Ok(record);
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            survey_detail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.survey_detail.Where(o => o.id == key).FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.survey_detail.Remove(record);
                            await dbContext.SaveChangesAsync();

                            var survey = await dbContext.survey.Where(o => o.id == key)
                                .FirstOrDefaultAsync();
                            if (survey != null)
                            {
                                var sum = await dbContext.survey_detail.Where(o => o.survey_id == survey.id)
                                    .SumAsync(o => o.quantity);
                                survey.quantity = sum;
                                await dbContext.SaveChangesAsync();
                            }

                            await tx.CommitAsync();
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else
                    {
                        return BadRequest("Record is not found.");
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }


            if (/*success && */record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.survey_detail();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }

            return Ok();
        }

        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.vw_survey_detail
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
        public async Task<IActionResult> SaveData([FromBody] survey Record)
        {
            try
            {
                var record = dbContext.survey_detail.Where(o => o.id == Record.id).FirstOrDefault();
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
                    record = new survey_detail();
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

                    dbContext.survey_detail.Add(record);
                    await dbContext.SaveChangesAsync();

                    return Ok(record);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("DeleteById/{Id}")]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Debug($"string Id = {Id}");

            try
            {
                var record = dbContext.survey_detail.Where(o => o.id == Id).FirstOrDefault();
                if (record != null)
                {
                    dbContext.survey_detail.Remove(record);
                    await dbContext.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}

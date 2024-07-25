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

namespace MCSWebApp.Controllers.API.SystemAdministration
{
    [Route("api/SystemAdministration/[controller]")]
    [ApiController]
    public class BusinessUnitController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public BusinessUnitController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("ByOrganizationId/{Id}")]
        public async Task<object> DataGrid(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_business_unit
                    .Where(o => o.organization_id == Id
                        && (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)),
                    loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DataGrid")]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_business_unit
                    //.Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                    //        || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
                    loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DataDetail")]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_business_unit
                .Where(o => o.id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)),
                loadOptions);
        }

        [HttpPost("InsertData")]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
					if (await mcsContext.CanCreate(dbContext, nameof(business_unit),
						CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
					{
                        var record = new business_unit();
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

                        record.business_unit_code = record.business_unit_code.Trim().Replace("  ", " ").ToUpper();

                        //var newTeam = new team()
                        //{
                        //    id = Guid.NewGuid().ToString("N"),
                        //    created_by = CurrentUserContext.AppUserId,
                        //    created_on = DateTime.Now,
                        //    is_active = true,
                        //    owner_id = CurrentUserContext.AppUserId,
                        //    organization_id = CurrentUserContext.OrganizationId,
                        //    team_name = record.business_unit_name,
                        //    team_code = record.business_unit_code
                        //};

                        //record.default_team_id = newTeam.id;

                        //dbContext.team.Add(newTeam);

                        dbContext.business_unit.Add(record);

                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();
                        return Ok(record);
					}
					else
					{
                        logger.Debug("User is not authorized.");
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
        }

        [HttpPut("UpdateData")]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.business_unit
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

                            record.business_unit_code = record.business_unit_code.Trim().Replace("  ", " ").ToUpper();

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok(record);
                        }
                        else
                        {
                            logger.Debug("User is not authorized.");
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else
                    {
                        logger.Debug("Record does not exist.");
                        return BadRequest("Record does not exist.");
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

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.business_unit
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            var recordTeam = await dbContext.team
                                .Where(o => o.id == record.default_team_id)
                                .FirstOrDefaultAsync();
                            if (recordTeam != null)
                            {
                                dbContext.team.Remove(recordTeam);
                            }

                            dbContext.business_unit.Remove(record);

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok();
                        }
                        else
                        {
                            logger.Debug("User is not authorized.");
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else
                    {
                        logger.Debug("Record does not exist.");
                        return BadRequest("Record does not exist.");
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

        [HttpGet("List")]
        public async Task<IActionResult> List([FromQuery] string q)
        {
            try
            {
                var rows = dbContext.vw_business_unit.AsNoTracking();
                rows = rows.Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin);
                if (!string.IsNullOrEmpty(q))
                {
                    rows = rows.Where(o => o.business_unit_name.Contains(q)
                        || o.business_unit_code.Contains(q));
                }

                return Ok(await rows.ToListAsync());
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.vw_business_unit
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    if (await mcsContext.CanRead(dbContext, Id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        return Ok(record);
                    }
                    else
                    {
                        logger.Debug("User is not authorized.");
                        return BadRequest("User is not authorized.");
                    }
                }
                else
                {
                    logger.Debug("Record does not exist.");
                    return BadRequest("Record does not exist.");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] business_unit Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    business_unit record = null;

                    if (!string.IsNullOrEmpty(Record?.id))
                    {
                        record = await dbContext.business_unit
                            .Where(o => o.id == Record.id)
                            .FirstOrDefaultAsync();
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
                                await tx.CommitAsync();
                                return Ok(record);
                            }
                            else
                            {
                                logger.Debug("User is not authorized.");
                                return BadRequest("User is not authorized.");
                            }
                        }
                        else
                        {
                            logger.Debug("Record does not exist.");
                            return BadRequest("Record does not exist.");
                        }
                    }
                    else if (await mcsContext.CanCreate(dbContext, nameof(business_unit),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new business_unit();
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

                        dbContext.business_unit.Add(record);
                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();
                        return Ok(record);
                    }
                    else
                    {
                        logger.Debug("User is not authorized.");
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
        }

        [HttpGet("BusinessUnitIdLookup")]
        public async Task<object> BusinessUnitIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_unit
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.business_unit_name, Search = o.business_unit_name.ToLower() + o.business_unit_name.ToUpper() })
                    .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("BusinessUnitIdLookupNoFilter")]
        public async Task<object> BusinessUnitIdLookupNoFilter(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_unit
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.business_unit_name, Search = o.business_unit_name.ToLower() + o.business_unit_name.ToUpper() })
                    .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
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
            logger.Trace($"string Id = {Id}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.business_unit
                        .Where(o => o.id == Id)
                        .FirstOrDefaultAsync();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, Id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.business_unit.Remove(record);
                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok();
                        }
                        else
                        {
                            logger.Debug("User is not authorized.");
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else
                    {
                        logger.Debug("Record does not exist.");
                        return BadRequest("Record does not exist.");
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
    }
}
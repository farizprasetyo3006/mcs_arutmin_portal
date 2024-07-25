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
using BusinessLogic;
using BusinessLogic.Entity;
using DataAccess.Select2;
using PetaPoco;
using WebApp.Extensions;
using Common;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Controllers.API.SystemAdministration
{
    [Route("api/SystemAdministration/[controller]")]
    [ApiController]
    public class ApplicationUserController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        //private readonly LdapConfiguration ldapConfiguration;

        public ApplicationUserController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("ByOrganizationId/{Id}")]
        public async Task<object> ByOrganizationId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_application_user
                    .Where(o => o.organization_id == Id
                        //&& (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        //|| CurrentUserContext.IsSysAdmin)
                        ),
                    loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        [HttpGet("lookup")]
        public async Task<object> lookup(string Id,DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.application_user
                    .Where (o => o.id == Id)
                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                   .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                   .Select(o => new { Value = o.id, Text = o.fullname });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
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
                //return await DataSourceLoader.LoadAsync(dbContext.vw_application_user
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                //        //&& (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                //        //|| CurrentUserContext.IsSysAdmin)
                //        ),
                //    loadOptions);

                var record = dbContext.vw_application_user
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if ((await mcsContext.CanRead(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin) == false)
                        return BadRequest("User is not authorized.");
                }

                return await DataSourceLoader.LoadAsync(dbContext.vw_application_user
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin),
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
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_application_user.Where(o => o.id == Id && (
                        CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)),
                    loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("InsertData")]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(application_user),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new application_user();
                        JsonConvert.PopulateObject(values, record);

                        var checkExisting = dbContext.application_user
                            .Where(o => o.application_username.Trim().ToLower() == record.application_username.Trim().ToLower())
                            .FirstOrDefault();
                        if (checkExisting != null)
                        {
                            if (!record.application_username.Contains('@'))
                                return BadRequest("User Name already exists. You can add '@[organization code]' after User Name to differentiate between organization.");
                            else
                                return BadRequest("User Name already exists. Use another name.");
                        }

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
                        record.api_key = Guid.NewGuid().ToString();
                        record.expired_date = DateTime.Now.AddDays(90);

                        record.application_password =
                            BusinessLogic.Utilities.StringHash.CreateHash(record.application_username);

                        dbContext.application_user.Add(record);
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
                    var record = dbContext.application_user
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

                            var is_active = record.is_active;

                            record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            record.is_active = is_active;

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
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.application_user
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.application_user.Remove(record);
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
                var rows = dbContext.vw_application_user.AsNoTracking();
                rows = rows.Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin);
                if (!string.IsNullOrEmpty(q))
                {
                    rows = rows.Where(o => o.application_username.Contains(q));
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
                var record = await dbContext.vw_application_user
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
        public async Task<IActionResult> SaveData([FromBody] application_user Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    application_user record = null;

                    if (!string.IsNullOrEmpty(Record.id))
                    {
                        record = await dbContext.application_user
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
                    else if (await mcsContext.CanCreate(dbContext, nameof(application_user),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new application_user();
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

                        dbContext.application_user.Add(record);
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
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
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
                    var record = await dbContext.application_user
                        .Where(o => o.id == Id)
                        .FirstOrDefaultAsync();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, Id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.application_user.Remove(record);
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
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        [HttpPut("RefreshKey")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> RefreshKey([FromForm] string Id)
        {
            logger.Trace($"string values = {Id}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.application_user
                        .Where(o => o.id == Id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, Id, CurrentUserContext.AppUserId)
                                                    || CurrentUserContext.IsSysAdmin)
                        {
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;
                            if (string.IsNullOrEmpty(record.api_key))
                            {
                                record.api_key = Guid.NewGuid().ToString();
                            }
                            record.expired_date = DateTime.Now.AddDays(90);

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
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        [HttpGet("PrimaryTeamIdLookup")]
        public async Task<object> PrimaryTeamIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.business_unit
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.business_unit_name })
                    .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }

			catch (Exception ex)
			{
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("select2")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Select2([FromQuery] string q)
        {
            var result = new Select2Response();

            try
            {
                var s2Request = new Select2Request()
                {
                    q = q
                };
                if (s2Request != null)
                {
                    var svc = new ApplicationUser(CurrentUserContext);
                    result = await svc.Select2(s2Request, "fullname");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return new JsonResult(result);
        }

        [HttpGet("getbyid/{Id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetById(string Id)
        {
            var result = new StandardResult();

            try
            {
                var svc = new ApplicationUser(CurrentUserContext);
                result.Data = await svc.GetViewByIdAsync(Id);
                result.Success = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet("getall")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetAll()
        {
            var result = new StandardResult();

            try
            {
                var svc = new ApplicationUser(CurrentUserContext);
                result.Data = await svc.GetViewAllAsync();
                result.Success = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpPost("save")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Save()
        {
            var result = new StandardResult();

            try
            {
                var svc = new ApplicationUser(CurrentUserContext);
                var values = await HttpContext.Request.GetRawBodyStringAsync();
                logger.Trace($"values = {values}");

                var data = await svc.SaveValues(values, (isNew, success) =>
                {
                    result.Success = success;
                });

                if (result.Success)
                {
                    result.Data = data;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet("ResetPassword")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ResetPassword([FromQuery]string Id, [FromQuery] string NewPassword)
        {
            var result = new StandardResult();

            try
            {
                var svc = new ApplicationUser(CurrentUserContext);
                result.Data = await svc.ResetPassword(Id, NewPassword);
                result.Success = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet("DisableUser")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DisableUser([FromQuery] string Id)
        {
            var result = new StandardResult();

            try
            {
                var svc = new ApplicationUser(CurrentUserContext);
                result.Data = await svc.DisableUser(Id);
                result.Success = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet("EnableUser")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> EnableUser([FromQuery] string Id)
        {
            var result = new StandardResult();

            try
            {
                var svc = new ApplicationUser(CurrentUserContext);
                result.Data = await svc.EnableUser(Id);
                result.Success = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet("GetApiKey")]
        public async Task<IActionResult> GetApiKey([FromQuery] string Id)
        {
            var result = new StandardResult();

            try
            {
                var svc = new ApplicationUser(CurrentUserContext);
                result.Data = await svc.GetApiKey(Id);
                result.Success = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return new JsonResult(result);
        }
    }
}
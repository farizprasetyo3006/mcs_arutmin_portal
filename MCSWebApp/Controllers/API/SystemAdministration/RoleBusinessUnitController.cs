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
    public class RoleBusinessUnitController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public RoleBusinessUnitController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("ByApplicationRoleId/{Id}")]
        public async Task<object> ByApplicationRoleId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_role_business_unit.Where(o => o.application_role_id == Id),
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
                return await DataSourceLoader.LoadAsync(dbContext.vw_role_business_unit
                    //.Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                    //    || CurrentUserContext.IsSysAdmin)
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
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_role_business_unit
                    .Where(o => o.id == Id
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

        [HttpPost("InsertData")]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
					if (await mcsContext.CanCreate(dbContext, nameof(role_business_unit),
						CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
					{
                        var record = new role_business_unit();
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

                        dbContext.role_business_unit.Add(record);
                        await dbContext.SaveChangesAsync();
                        
                        mcsContext dbFind = new mcsContext(DbOptionBuilder.Options);

                        var recAppEntity = dbFind.application_entity
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                        foreach (var rowData in recAppEntity)
                        {
                            var recAEBU = dbContext.application_business_unit
                                .Where(o => o.role_business_unit_id == record.id 
                                    && o.application_entity_id == rowData.entity_id)
                                .FirstOrDefault();
                            if (recAEBU == null)
                            {
                                var newRec = new application_business_unit();
                                JsonConvert.PopulateObject(values, newRec);

                                newRec.id = Guid.NewGuid().ToString("N");
                                newRec.created_by = CurrentUserContext.AppUserId;
                                newRec.created_on = DateTime.Now;
                                newRec.modified_by = null;
                                newRec.modified_on = null;
                                newRec.is_active = true;
                                newRec.is_default = null;
                                newRec.is_locked = null;
                                newRec.entity_id = null;
                                newRec.owner_id = CurrentUserContext.AppUserId;
                                newRec.organization_id = CurrentUserContext.OrganizationId;

                                newRec.role_business_unit_id = record.id;
                                newRec.application_entity_id = rowData.entity_id;

                                dbContext.application_business_unit.Add(newRec);
                                await dbContext.SaveChangesAsync();
                            }
                        }

                        await tx.CommitAsync();

                        return Ok(record);
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
        }

        [HttpPut("UpdateData")]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.role_business_unit
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

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok(record);
                        }
                        else
                        {
                            logger.Debug("User is not authorized.");
                            return Unauthorized();
                        }
                    }
                    else
                    {
                        logger.Debug("Record does not exist.");
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
        }

        [HttpDelete("DeleteData")]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.role_business_unit
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            var recAEBU = dbContext.application_business_unit
                                .Where(o => o.role_business_unit_id == key);
                            if (recAEBU != null)
                            {
                                dbContext.application_business_unit.RemoveRange(recAEBU);
                                await dbContext.SaveChangesAsync();
                            }

                            dbContext.role_business_unit.Remove(record);
                            await dbContext.SaveChangesAsync();

                            await tx.CommitAsync();
                            return Ok();
                        }
                        else
                        {
                            logger.Debug("User is not authorized.");
                            return Unauthorized();
                        }
                    }
                    else
                    {
                        logger.Debug("Record does not exist.");
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
        }

        [HttpGet("BusinessUnitLookupByApplicationRoleId/{Id}")]
        public async Task<object> BusinessUnitLookupByApplicationRoleId(string Id, DataSourceLoadOptions loadOptions)
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

        //     [HttpGet("RoleList")]
        //     public async Task<object> RoleList(DataSourceLoadOptions loadOptions)
        //     {
        //         try
        //         {
        //             var lookup = dbContext.application_role
        //                 .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //                 .Select(o => new { o.id, o.role_name })
        //                 .OrderBy(o => o.role_name);
        //             return await DataSourceLoader.LoadAsync(lookup, loadOptions);
        //         }
        //catch (Exception ex)
        //{
        //             logger.Error(ex.ToString());
        //             return BadRequest(ex.InnerException?.Message ?? ex.Message);
        //         }
        //     }

        [HttpGet("GetList")]
        public async Task<object> GetList(DataSourceLoadOptions loadOptions)
        {
            Dictionary<string, dynamic> myData = new Dictionary<string, dynamic>();

            try
            {
                //var RList = await dbContext.application_role
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                //    .Select(o => new { o.id, o.role_name })
                //    .OrderBy(o => o.role_name).ToListAsync();
                //var records = DataSourceLoader.Load(RList, loadOptions);

                string sql = string.Format("select ar.id, ar.role_name from application_user au join user_role ur on " +
                    "au.id = ur.application_user_id join application_role ar on ar.id = ur.application_role_id " +
                    "where au.application_username = '{0}' order by ar.role_name", CurrentUserContext.AppUsername);

                var RList = await dbContext.application_role.FromSqlRaw(sql)
                    .Select(o => new { o.id, o.role_name })
                    .OrderBy(o => o.role_name).ToListAsync();
                var records = DataSourceLoader.Load(RList, loadOptions);

                myData.Add("RoleList", records);
                myData.Add("defaultRoleId", GlobalVars.ROLE_ID);

                var BUList = await dbContext.vw_role_business_unit
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.application_role_id == GlobalVars.ROLE_ID)
                    .Select(o => new { id = o.business_unit_id, o.business_unit_name })
                    .OrderBy(o => o.business_unit_name).ToListAsync();
                var recBU = DataSourceLoader.Load(BUList, loadOptions);

                myData.Add("BusinessUnitList", recBU);
                myData.Add("defaultBUId", HttpContext.Session.GetString("BUSINESS_UNIT_ID"));
                //myData.Add("defaultBUId", HttpContext.Session.GetString("BUSINESS_UNIT_ID"));

                return myData;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpGet("BusinessUnitList/{RoleId}")]
        public async Task<object> BusinessUnitList(string RoleId, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_role_business_unit
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.application_role_id == RoleId)
                    .Select(o => new { id = o.business_unit_id, o.business_unit_name })
                    .OrderBy(o => o.business_unit_name);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("ChangeRoleBusinessUnit/{RoleId}/{BusinessUnitId}")]
        public IActionResult ChangeRoleBusinessUnit(string RoleId, string BusinessUnitId)
        {
            try
            {
                GlobalVars.ROLE_ID = RoleId;
                CurrentUserContext.RoleId = RoleId;

                if (!string.IsNullOrEmpty(BusinessUnitId) && BusinessUnitId != "xxx")
                {
                    HttpContext.Session.SetString("BUSINESS_UNIT_ID", BusinessUnitId);
                    CurrentUserContext.BusinessUnitId = BusinessUnitId;
                }
                return Ok();
            }
            catch (Exception ex)
            {
                logger.Error("ChangeRoleBusinessUnit: "+ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
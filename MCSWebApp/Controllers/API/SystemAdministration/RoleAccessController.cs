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
using DataAccess.DataTables;
using MCSWebApp.Extensions;
using PetaPoco;
using BusinessLogic;
using Common;

namespace MCSWebApp.Controllers.API.SystemAdministration
{
    [Route("api/SystemAdministration/[controller]")]
    [ApiController]
    public class RoleAccessController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public RoleAccessController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpPost("DataTables/{Id}")]
        public async Task<IActionResult> DataTables(string Id)
        {
            var result = new DTResponse();

            try
            {
                logger.Debug($"Id = {Id}");

                var request = HttpContext.Request;
                logger.Trace($"ContentType = {request.ContentType}");
                logger.Trace($"ContentLength = {request.ContentLength}");

                DTRequest dtRequest = null;
                if ((request.ContentType == "x-www-url-form-encoded" ||
                    request.ContentType == "form-data")
                    && request.ContentLength > 0)
                {
                    var nv = request.Form;
                    if (nv != null)
                    {
                        dtRequest = nv.ToDataTablesRequest();
                    }
                }
                else
                {
                    dtRequest = new DTRequest()
                    {
                        Draw = 0
                    };
                }

                if (dtRequest != null)
                {
                    var svc = new BusinessLogic.Entity.RoleAccess(CurrentUserContext);
                    var filters = Sql.Builder.Append("AND organization_id = @0", CurrentUserContext.OrganizationId);
                    filters.Append("AND application_role_id = @0", Id);
                    result = await svc.GetDataTablesResponseAsync(dtRequest, filters);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());                
            }

            return new JsonResult(result);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] dynamic CurrentRoleAccess)
        {
            var result = new StandardResult();

            try
            {
                logger.Debug(JsonConvert.SerializeObject(CurrentRoleAccess));

                var id = (string)CurrentRoleAccess.id;
                var roleAccess = await dbContext.role_access.Where(o => o.id == id)
                    .FirstOrDefaultAsync();
                if(roleAccess != null)
                {
                    logger.Debug(JsonConvert.SerializeObject(roleAccess));

                    var field = (string)CurrentRoleAccess.field;
                    var value = (long)CurrentRoleAccess.value;
                    if (!string.IsNullOrEmpty(field))
                    {
                        bool modified = false;
                        long nextValue = -1;

                        if (value <= Constants.ACCESS_NO_ACCESS)
                            nextValue = Constants.ACCESS_OWN_RECORD;
                        else if (value <= Constants.ACCESS_OWN_RECORD)
                            nextValue = Constants.ACCESS_BUSINESS_UNIT;
                        else if (value <= Constants.ACCESS_BUSINESS_UNIT)
                            nextValue = Constants.ACCESS_DEEP_BUSINESS_UNIT;
                        else if (value <= Constants.ACCESS_DEEP_BUSINESS_UNIT)
                            nextValue = Constants.ACCESS_ORGANIZATION;
                        else nextValue = Constants.ACCESS_NO_ACCESS;

                        //if (value <= Constants.ACCESS_NO_ACCESS)
                        //    nextValue = Constants.ACCESS_ORGANIZATION;
                        //else nextValue = Constants.ACCESS_NO_ACCESS;

                        switch (field.ToLower())
                        {
                            case "access_create":
                                if (roleAccess.access_create == value)
                                {
                                    roleAccess.access_create = nextValue;
                                    modified = true;
                                }
                                break;
                            case "access_read":
                                if (roleAccess.access_read == value)
                                {
                                    roleAccess.access_read = nextValue;
                                    modified = true;
                                }
                                break;
                            case "access_update":
                                if (roleAccess.access_update == value)
                                {
                                    roleAccess.access_update = nextValue;
                                    modified = true;
                                }
                                break;
                            case "access_delete":
                                if (roleAccess.access_delete == value)
                                {
                                    roleAccess.access_delete = nextValue;
                                    modified = true;
                                }
                                break;
                            case "access_append":
                                if (roleAccess.access_append == value)
                                {
                                    roleAccess.access_append = nextValue;
                                    modified = true;
                                }
                                break;
                            default:
                                result.Message = "Undefined field role access";
                                logger.Debug(result.Message);
                                break;
                        }

                        if (modified)
                        {
                            roleAccess.modified_by = CurrentUserContext.AppUserId;
                            roleAccess.modified_on = DateTime.Now;
                            await dbContext.SaveChangesAsync();

                            result.Data = new
                            {
                                id,
                                field,
                                value = nextValue
                            };
                            result.Success = true;
                        }
                    }
                    else
                    {
                        result.Message = "Role access field is empty";
                        logger.Debug(result.Message);
                    }
                }
                else
                {
                    result.Message = "Role access does not exist";
                    logger.Debug(result.Message);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return new JsonResult(result);
        }

        [HttpGet("List")]
        public async Task<IActionResult> List([FromQuery] string q)
        {
            try
            {
                var rows = dbContext.vw_role_access.AsNoTracking();
                rows = rows.Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin);
                if (!string.IsNullOrEmpty(q))
                {
                    rows = rows.Where(o => o.role_name.Contains(q)
                        || o.entity_name.Contains(q));
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
                var record = await dbContext.vw_role_access
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
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}

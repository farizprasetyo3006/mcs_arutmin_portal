using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using DataAccess.EFCore;
using NLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataAccess.DTO;
using Omu.ValueInjecter;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using BusinessLogic;
using System.Text;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using DataAccess.Select2;
using Common;

namespace MCSWebApp.Controllers.API.Report
{
    [Route("api/Report/[controller]")]
    [ApiController]
    public class ReportTemplateController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ReportTemplateController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_report_template
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_report_template.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(report_template),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var record = new report_template();
                    JsonConvert.PopulateObject(values, record);

                    record.id = Guid.NewGuid().ToString("N");
                    record.created_by = CurrentUserContext.AppUserId;
                    record.created_on = DateTime.Now;
                    record.modified_by = null;
                    record.modified_on = null;
                    //record.is_active = true;
                    record.is_default = null;
                    record.is_locked = null;
                    record.entity_id = null;
                    record.owner_id = CurrentUserContext.AppUserId;
                    record.organization_id = CurrentUserContext.OrganizationId;
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    dbContext.report_template.Add(record);
                    await dbContext.SaveChangesAsync();

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
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = await dbContext.report_template
                    .Where(o => o.id == key)
                    .FirstOrDefaultAsync();
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
                    logger.Debug("Record is not found.");
                    return NotFound();
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
            logger.Trace($"string key = {key}");

            try
            {
                var record = await dbContext.report_template
                    .Where(o => o.id == key)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.report_template.Remove(record);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("UploadDocument")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<StandardResult> UploadDocument([FromBody] dynamic FileDocument)
        {
            var result = new StandardResult();
            string key = null;

            if (FileDocument == null)
            {
                result.Message = "No file uploaded.";
                logger.Debug(result.Message);
                return result;
            }

            key = (string)FileDocument.recordkey;
            if (string.IsNullOrEmpty(key))
            {
                result.Message = "Record id is empty.";
                logger.Debug(result.Message);
                return result;
            }

            string strFile = (string)FileDocument.data;
            if (string.IsNullOrEmpty(strFile))
            {
                result.Message = "Report definition is empty.";
                logger.Debug(result.Message);
                return result;
            }

            try
            {
                string reportDefinition = Encoding.UTF8.GetString(Convert.FromBase64String(strFile));
                var record = await dbContext.report_template
                    .Where(o => o.id == key)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    record.report_definition = reportDefinition;
                    record.modified_by = CurrentUserContext.AppUserId;
                    record.modified_on = DateTime.Now;

                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        await dbContext.SaveChangesAsync();
                        result.Success = true;                        
                    }
                    else
                    {
                        result.Message = "User is not authorized.";
                        logger.Debug(result.Message);
                    }
                }
                else
                {
                    result.Message = "Record is not found.";
                    logger.Debug(result.Message);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        [HttpGet("DownloadDocument")]
        public async Task<object> DownloadDocument([FromQuery] string Id)
        {
            try
            {
                var record = await dbContext.report_template
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    if (await mcsContext.CanRead(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        logger.Debug($"Returning file {record.report_name}.frx");
                        var stream = new MemoryStream(Encoding.UTF8.GetBytes(record.report_definition));
                        return new FileStreamResult(stream, new MediaTypeHeaderValue("application/octetstream"))
                        {
                            FileDownloadName = $"{record.report_name}.frx"
                        };
                    }
                    else
                    {
                        logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                else
                {
                    logger.Debug("Record is not found.");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ApplicationEntityIdLookup")]
        public async Task<object> ApplicationEntityIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.application_entity
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin))
                    .Select(o => new { Value = o.id, Text = o.display_name })
                    .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("PrintOutListSelect2/{ApplicationEntityId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> PrintOutListSelect2([FromQuery] string q, string ApplicationEntityId)
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
                    result.results = new List<Select2Item>();
                    var records = await dbContext.report_template.Where(o => o.menu_id == ApplicationEntityId)
                        .ToListAsync();
                    if(records?.Count > 0)
                    {
                        foreach(var record in records)
                        {
                            if (!string.IsNullOrEmpty(q))
                            {
                                if (!record.report_name.Contains(q))
                                    continue;
                            }

                            result.results.Add(new Select2Item()
                            {
                                id = record.id,
                                text = record.report_name
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return new JsonResult(result);
        }

    }
}

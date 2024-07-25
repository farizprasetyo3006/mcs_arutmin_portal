
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
using Microsoft.EntityFrameworkCore;
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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using BusinessLogic.Formula;
using Common;

namespace MCSWebApp.Controllers.API.Mining
{
    [Route("api/Timesheet/[controller]")]
    [ApiController]
    public class TimesheetDetailEventPlanController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public TimesheetDetailEventPlanController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(string timesheetDetailId, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_timesheet_detail_event_plan
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.timesheet_detail_id == timesheetDetailId),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_timesheet_detail_event_plan.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }


        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(timesheet_detail_event_plan),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new timesheet_detail_event_plan();
                    JsonConvert.PopulateObject(values, record);

                    if(record.minute == null || record.minute == 0)
                    {
                        return BadRequest("minute cannot be empty.");
                    }

                    //get detail
                    var detail = dbContext.timesheet_detail_plan.Where(x => x.id == record.timesheet_detail_id).FirstOrDefault();
                    if(detail == null)
                    {
                        return BadRequest("detail cannot be empty.");
                    }
                    var timesheet = dbContext.timesheet_plan.Where(x => x.id == detail.timesheet_id).FirstOrDefault();
                    if(timesheet == null)
                    {
                        return BadRequest("timesheet cannot be empty.");
                    }

                    var eventObj = dbContext.timesheet_detail_event_plan.Where(x => x.event_category_id == record.event_category_id 
                    && x.event_definition_category_id == record.event_definition_category_id
                    && x.timesheet_detail_id == record.timesheet_detail_id).FirstOrDefault();

                    if (eventObj != null)
                    {
                        return BadRequest("Already have same event.");
                    }

                    //get total minute
                    var totalMinute = dbContext.timesheet_detail_event_plan.Where(x => x.timesheet_detail_id == record.timesheet_detail_id).Sum(x => x.minute);

                    if (!string.IsNullOrEmpty(timesheet.cn_unit_id))
                    {
                        var shift = dbContext.shift.Where(x => x.id == timesheet.shift_id).FirstOrDefault();
                        if(shift == null)
                        {
                            return BadRequest("shift cannot be empty.");
                        }
                        var dura = ((TimeSpan)shift.duration).TotalMinutes;
                        
                        if ((double)totalMinute + (double)record.minute > dura)
                        {
                            return BadRequest("minute cannot be over shift duration.");
                        }
                    }
                    else
                    {
                        if (totalMinute + record.minute > 60)
                        {
                            return BadRequest("minute cannot be over 60.");
                        }
                    }
                    record.id = Guid.NewGuid().ToString("N");
                    #region Base Record
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
                    #endregion

                    dbContext.timesheet_detail_event_plan.Add(record);
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

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.timesheet_detail_event_plan
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var recordMin = record.minute;
                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);

                        if (record.minute == null || record.minute == 0)
                        {
                            return BadRequest("minute cannot be empty.");
                        }

                        //get detail
                        var detail = dbContext.timesheet_detail_plan.Where(x => x.id == record.timesheet_detail_id).FirstOrDefault();
                        if (detail == null)
                        {
                            return BadRequest("detail cannot be empty.");
                        }
                        var timesheet = dbContext.timesheet_plan.Where(x => x.id == detail.timesheet_id).FirstOrDefault();
                        if (timesheet == null)
                        {
                            return BadRequest("timesheet cannot be empty.");
                        }

                        var eventObj = dbContext.timesheet_detail_event_plan.Where(x => x.event_category_id == record.event_category_id 
                        && x.event_definition_category_id == record.event_definition_category_id
                        && x.timesheet_detail_id == record.timesheet_detail_id).FirstOrDefault();

                        if (eventObj != null)
                        {
                            if(eventObj.id != key)
                                return BadRequest("Already have same event.");
                        }

                        //get total minute
                        var totalMinute = dbContext.timesheet_detail_event_plan.Where(x => x.timesheet_detail_id == record.timesheet_detail_id).Sum(x => x.minute);

                        if (!string.IsNullOrEmpty(timesheet.cn_unit_id))
                        {
                            var shift = dbContext.shift.Where(x => x.id == timesheet.shift_id).FirstOrDefault();
                            if (shift == null)
                            {
                                return BadRequest("shift cannot be empty.");
                            }
                            var dura = ((TimeSpan)shift.duration).TotalMinutes;

                            if (((double)totalMinute - (double)recordMin) + (double)record.minute > dura)
                            {
                                return BadRequest("minute cannot be over shift duration.");
                            }
                        }
                        else
                        {
                            if ((totalMinute - recordMin) + record.minute > 60)
                            {
                                return BadRequest("minute cannot be over 60.");
                            }
                        }                    

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
                else
                {
                    return BadRequest("No default organization");
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
                var record = dbContext.timesheet_detail_event_plan
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.timesheet_detail_event_plan.Remove(record);
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

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] timesheet Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.timesheet_detail_event_plan
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
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else if (await mcsContext.CanCreate(dbContext, nameof(timesheet_detail_event_plan),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new timesheet_detail_event_plan();
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

                        dbContext.timesheet_detail_event_plan.Add(record);
                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();
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
        }
    }
}

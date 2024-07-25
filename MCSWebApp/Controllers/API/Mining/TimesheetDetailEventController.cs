
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
    public class TimesheetDetailEventController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public TimesheetDetailEventController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(string timesheetDetailId, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_timesheet_detail_event
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
                dbContext.vw_timesheet_detail_event.Where(o => o.id == Id
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
                if (await mcsContext.CanCreate(dbContext, nameof(timesheet_detail_event),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var record = new timesheet_detail_event();
                    JsonConvert.PopulateObject(values, record);

                    if (record.minute == null || record.minute == 0)
                    {
                        return BadRequest("minute cannot be empty.");
                    }


                    //get total minute
                    var totalMinute = dbContext.timesheet_detail_event
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId
                            && x.timesheet_detail_id == record.timesheet_detail_id)
                        .Sum(x => x.minute);

                    if (totalMinute + record.minute > 60)
                    {
                        return BadRequest("minute cannot be over 60.");
                    }

                    var eventObj = dbContext.timesheet_detail_event
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId
                            && x.event_category_id == record.event_category_id
                            && x.event_definition_category_id == record.event_definition_category_id
                            && x.timesheet_detail_id == record.timesheet_detail_id).FirstOrDefault();

                    if (eventObj != null)
                    {
                        return BadRequest("Already have same event.");
                    }

                    string newid = Guid.NewGuid().ToString("N");
                    record.id = newid;
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

                    //***** sync to ellipse
                    var rec2 = new ell_sync();
                    rec2.id = newid;
                    rec2.organization_id = CurrentUserContext.OrganizationId;
                    rec2.data = newid + "|" + record.organization_id + "|" + record.event_category_id + "|" + record.event_definition_category_id +
                        "|" + record.minute + "|" + record.timesheet_detail_id;
                    rec2.new_sync_status = false;
                    rec2.module = "BREAKDOWN";
                    dbContext.ell_sync.Add(rec2);
                    //******************

                    dbContext.timesheet_detail_event.Add(record);
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
                var record = dbContext.timesheet_detail_event
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

                        var eventObj = dbContext.timesheet_detail_event.Where(x => x.event_category_id == record.event_category_id
                        && x.event_definition_category_id == record.event_definition_category_id
                        && x.timesheet_detail_id == record.timesheet_detail_id).FirstOrDefault();
                        if (eventObj != null)
                        {
                            if (eventObj.id != key)
                                return BadRequest("Already have same event.");
                        }

                        //get total minute
                        var totalMinute = dbContext.timesheet_detail_event.Where(x => x.timesheet_detail_id == record.timesheet_detail_id).Sum(x => x.minute);

                        if ((totalMinute - recordMin) + record.minute > 60)
                        {
                            return BadRequest("minute cannot be over 60.");
                        }


                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        //***** sync to ellipse
                        var rec2 = dbContext.ell_sync
                            .Where(o => o.id == key && o.module == "BREAKDOWN"
                                && o.organization_id == CurrentUserContext.OrganizationId)
                            .FirstOrDefault();
                        if (rec2 != null)
                        {
                            rec2.data = rec2.id + "|" + record.organization_id + "|" + record.event_category_id + "|" + record.event_definition_category_id +
                                "|" + record.minute + "|" + record.timesheet_detail_id;
                            if (rec2.new_sync_status == true) rec2.update_sync_status = false;
                        }
                        //******************

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
                var record = dbContext.timesheet_detail_event
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.timesheet_detail_event.Remove(record);

                    //***** sync to ellipse
                    var rec2 = dbContext.ell_sync
                        .Where(o => o.id == key && o.module == "BREAKDOWN"
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (rec2 != null)
                    {
                        rec2.data = rec2.id + "|" + record.organization_id + "|" + record.event_category_id + "|" + record.event_definition_category_id +
                            "|" + record.minute + "|" + record.timesheet_detail_id;
                        rec2.delete_sync_status = false;
                    }
                    //******************

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
                    var record = await dbContext.timesheet_detail_event
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

                            //***** sync to ellipse
                            var rec2 = dbContext.ell_sync
                                .Where(o => o.id == Record.id && o.module == "BREAKDOWN"
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (rec2 != null)
                                dbContext.ell_sync.Remove(rec2);
                            //******************

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok(record);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else if (await mcsContext.CanCreate(dbContext, nameof(timesheet_detail_event),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new timesheet_detail_event();
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

                        dbContext.timesheet_detail_event.Add(record);

                        //***** sync to ellipse
                        var rec2 = new ell_sync();
                        rec2.id = record.id;
                        rec2.organization_id = CurrentUserContext.OrganizationId;
                        rec2.data = record.id + "|" + record.organization_id + "|" + record.event_category_id + "|" + record.event_definition_category_id +
                            "|" + record.minute + "|" + record.timesheet_detail_id;
                        rec2.new_sync_status = false;
                        rec2.module = "BREAKDOWN";
                        dbContext.ell_sync.Add(rec2);
                        //******************

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
                    if (ex.InnerException != null)
                    {
                        logger.Error(ex.InnerException.Message);
                        return BadRequest(ex.InnerException.Message);
                    }
                    else
                    {
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
                }
            }
        }

        [HttpGet("GetActivityOBCoal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public object GetActivityOBCoal(string timesheetDetailId)
        {
            var temp = dbContext.vw_timesheet_detail_event.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.timesheet_detail_id == timesheetDetailId && (o.activity_code.ToUpper() == "A1" || o.activity_code.ToUpper() == "A2")).FirstOrDefault();

            return temp;
        }
    }
}

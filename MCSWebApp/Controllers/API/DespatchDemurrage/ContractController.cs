
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

namespace MCSWebApp.Controllers.API.DespatchDemurrage
{
    [Route("api/DespatchDemurrage/[controller]")]
    [ApiController]
    public class ContractController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ContractController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_despatch_demurrage
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_despatch_demurrage.Where(o => o.id == Id
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
				if (await mcsContext.CanCreate(dbContext, nameof(despatch_demurrage),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new despatch_demurrage();
                    JsonConvert.PopulateObject(values, record);

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

                    var despatchOrderRecord = dbContext.despatch_order
                        .Where(o => o.id == record.despatch_order_id && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();

                    if (despatchOrderRecord != null)
                        record.vessel_id = despatchOrderRecord.vessel_id;

                    dbContext.despatch_demurrage.Add(record);
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
                var record = dbContext.despatch_demurrage
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
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
                var record = dbContext.despatch_demurrage
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.despatch_demurrage.Remove(record);
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
        public async Task<IActionResult> SaveData([FromBody] despatch_demurrage Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.despatch_demurrage
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
                    else if (await mcsContext.CanCreate(dbContext, nameof(despatch_demurrage),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new despatch_demurrage();
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

                        dbContext.despatch_demurrage.Add(record);
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

        [HttpGet("DesDemContractIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DesDemContractIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.despatch_demurrage
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
					.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.contract_name});
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("Calculation")]
        public async Task<ApiResponse> Calculation(string desdemId)
        {
            var response = await Task.Run<ApiResponse>(() =>
            {
                var result = new ApiResponse();
                result.Status.Success = true;
                try
                {
                    var obj = new BusinessLogic.Formula.DespatchDemurrage();

                    var desDemRecord = dbContext.despatch_demurrage
                        .FirstOrDefault(r => r.organization_id == CurrentUserContext.OrganizationId && r.id == desdemId);

                    var desdemDetails = dbContext.vw_despatch_demurrage_detail
                        .FirstOrDefault(r => r.organization_id == CurrentUserContext.OrganizationId && r.despatch_demurrage_id == desdemId);

                    #region DesDem Details Validation
                    if (desdemDetails == null)
                    {
                        result.Status.Success = false;
                        result.Status.Message = "Laytime Calculation is blank. Please check your DesDem Contract Detail.";
                        return result;
                    }
                    #endregion

                    var despatchOrderRecord = dbContext.despatch_order
                        .FirstOrDefault(r => r.id == desDemRecord.despatch_order_id);

                    #region Shipping Order Validation
                    if (despatchOrderRecord == null)
                    {
                        result.Status.Success = false;
                        result.Status.Message = "There is no selected Shipping Order on DesDem Details. Please check your DesDem Detail.";
                        return result;
                    }
                    else
                    {
                        if (despatchOrderRecord.quantity == null)
                        {
                            result.Status.Success = false;
                            result.Status.Message = "There is no Quantity on Shipping Order Record. Please check your Shipping Order.";
                            return result;
                        }
                    }
                    #endregion

                    var desdemDelays = dbContext.despatch_demurrage_delay
                       .Where(r => r.despatch_demurrage_id == desdemId && r.organization_id == CurrentUserContext.OrganizationId)
                       .ToList();

                    double total_allowed_time = ((double)despatchOrderRecord.quantity / (double)desdemDetails.loading_rate) + (double)desdemDetails.turn_time;
                    double total_actual_time = 0;
                    if (desdemDelays.Count > 0)
                    {
                        foreach (var item in desdemDelays)
                        {
                            double actual_time = 0;
                            var sof_details = dbContext.sof_detail
                                .Where(r => r.sof_id == desdemDetails.sof_id && r.event_category_id == item.event_category_id)
                                .ToList();
                            if (sof_details.Count > 0)
                            {
                                foreach (var detail in sof_details)
                                {
                                    actual_time += (detail.end_datetime.Subtract(detail.start_datetime).TotalMinutes / 60);
                                }
                            }
                            total_actual_time += actual_time;
                        }
                    }
                    else
                    {
                        result.Status.Success = false;
                        result.Status.Message = "There is no DesDem Delay record. Please input first.";
                        return result;
                    }

                    var record = new BusinessLogic.Formula.DespatchDemurrage();
                    record.AllowedTime = total_allowed_time;
                    record.ActualTime = total_actual_time;
                    record.Rate = (double)desdemDetails.rate;
                    record.Difference = Math.Abs(total_allowed_time - total_actual_time);
                    record.IsDespatch = total_allowed_time < total_actual_time ? false : true;
                    record.Amount = record.Rate * record.Difference;
                    record.StatementofFactNumber = desdemDetails.sof_number;

                    result.Data = record;
                    result.Status.Message = "Calculation Successfully.";

                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    result.Status.Success = false;
                    result.Status.Message = ex.Message;
                }
                return result;
            });
            return response;
        }
    }
}

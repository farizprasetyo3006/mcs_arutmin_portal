
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
using System.Dynamic;

namespace MCSWebApp.Controllers.API.DespatchDemurrage
{
    [Route("api/DespatchDemurrage/[controller]")]
    [ApiController]
    public class InvoiceController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public InvoiceController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_despatch_demurrage_invoice
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_despatch_demurrage_invoice.Where(o => o.id == Id
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
				if (await mcsContext.CanCreate(dbContext, nameof(despatch_demurrage_invoice),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new despatch_demurrage_invoice();
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

                    dbContext.despatch_demurrage_invoice.Add(record);
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
                var record = dbContext.despatch_demurrage_invoice
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
                var record = dbContext.despatch_demurrage_invoice
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.despatch_demurrage_invoice.Remove(record);
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
                    var record = await dbContext.despatch_demurrage_invoice
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
                    else if (await mcsContext.CanCreate(dbContext, nameof(despatch_demurrage_invoice),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new despatch_demurrage_invoice();
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

                        dbContext.despatch_demurrage_invoice.Add(record);
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

        [HttpGet("DesDemInvoiceIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DesDemInvoiceIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.despatch_demurrage_invoice
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
					.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.invoice_number});
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("InvoiceTargetIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> InvoiceTargetIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_despatch_demurrage
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
					            .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { Value = o.contractor_id, Text = o.contractor_name })
                                .Union
                                (dbContext.vw_despatch_order
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
					            .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { Value = o.customer_id, Text = o.customer_name }))
                                .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("InvoiceTargetById")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> InvoiceTargetById(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_despatch_demurrage.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { id = o.contractor_id, invoice_target_type = "Supplier", desdem_type = "Despatch" })
                .Union
                    (dbContext.vw_despatch_order.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Select(o => new { id = o.customer_id, invoice_target_type = "Buyer", desdem_type = "Demurrage" }))
                .Where(o => o.id == Id),
                loadOptions);
        }

        [HttpGet("PrintStatusLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public object PrintStatusLookup()
        {
            var result = new List<dynamic>();
            try
            {
                foreach (var item in Common.Constants.PrintStatus)
                {
                    dynamic obj = new ExpandoObject();
                    obj.value = item;
                    obj.text = item;
                    result.Add(obj);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }

            return result;
        }

        [HttpGet("DesDemInvoiceStatusLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DesDemInvoiceStatusLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.master_list
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                        && o.item_group == "desdem-invoice-status")
					.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.item_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("CountLaytimeAllowed")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<StandardResult> CountLaytimeAllowed(string Id, DataSourceLoadOptions loadOptions)
        {
            var result = new StandardResult();
            Dictionary<string, dynamic> myData = new Dictionary<string, dynamic>();

            try
            {
                decimal draftSurveyQuantity = 0;

                dynamic record = await dbContext.shipping_transaction
                    .Where(o => o.despatch_order_id == Id).FirstOrDefaultAsync();
                if (record != null)
                {
                    draftSurveyQuantity = Convert.ToDecimal(record.original_quantity) ?? 0;
                }

                decimal loadingRate = 0;
                record = await dbContext.despatch_order
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                if (record != null)
                {
                    loadingRate = Convert.ToDecimal(record.loading_rate) ?? 0;
                }

                if (loadingRate != 0)
                {
                    decimal totalSeconds = (draftSurveyQuantity / loadingRate) * 24 * 3600;

                    var laytime_text = secondsToDhms((decimal)totalSeconds);

                    myData.Add("laytime_text", laytime_text);
                    myData.Add("laytime_duration", totalSeconds);

                    result.Data = myData;
                    result.Success = record != null ? true : false;
                    result.Message = result.Success ? "Ok" : "Record not found";
                }
                else
                {
                    result.Data = null;
                    result.Success = false;
                    result.Message = "DO Loading Rate is empty or zero.";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string secondsToDhms(decimal seconds)
        {
            var d = Math.Floor(seconds / (3600 * 24));
            var h = Math.Floor(seconds % (3600 * 24) / 3600);
            var m = Math.Floor(seconds % 3600 / 60);
            var s = Math.Floor(seconds % 60);

            var dDisplay = d > 0 ? d + (d == 1 ? " Day " : " Days ") : "";
            var hDisplay = h > 0 ? h + (h == 1 ? " Hour " : " Hours ") : "";
            var mDisplay = m > 0 ? m + (m == 1 ? " Minute " : " Minutes ") : "";
            return dDisplay + hDisplay + mDisplay;
        }

    }
}

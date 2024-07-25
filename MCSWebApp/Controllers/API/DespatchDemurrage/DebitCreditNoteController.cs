
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
using System.Dynamic;
using Common;

namespace MCSWebApp.Controllers.API.DespatchDemurrage
{
    [Route("api/DespatchDemurrage/[controller]")]
    [ApiController]
    public class DebitCreditNoteController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public DebitCreditNoteController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_despatch_demurrage_debit_credit_note
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_despatch_demurrage_debit_credit_note.Where(o => o.id == Id
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
				if (await mcsContext.CanCreate(dbContext, nameof(despatch_demurrage_debit_credit_note),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new despatch_demurrage_debit_credit_note();
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

                    dbContext.despatch_demurrage_debit_credit_note.Add(record);
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
                var record = dbContext.despatch_demurrage_debit_credit_note
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
                var record = dbContext.despatch_demurrage_debit_credit_note
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.despatch_demurrage_debit_credit_note.Remove(record);
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
                    var record = await dbContext.despatch_demurrage_debit_credit_note
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
                    else if (await mcsContext.CanCreate(dbContext, nameof(despatch_demurrage_debit_credit_note),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new despatch_demurrage_debit_credit_note();
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

                        dbContext.despatch_demurrage_debit_credit_note.Add(record);
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

        [HttpGet("ValuationTargetLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DayworkTargetLookup(string TargetType, DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (string.IsNullOrEmpty(TargetType))
                {
                    return null;
                }

                if (TargetType.ToLower() == "seller")
                {
                    var lookup = dbContext.customer
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .OrderBy(o => o.business_partner_code)
                        .Select(o => new
                        {
                            Value = o.id,
                            Text = (String.IsNullOrEmpty(o.business_partner_code) ? "" : o.business_partner_code + " - ")
                                + (o.business_partner_name ?? ""),
                            o.business_partner_name
                        });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                    //var lookup = dbContext.contractor
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    //    .OrderBy(o => o.business_partner_code)
                    //    .Select(o => new
                    //    {
                    //        Value = o.id,
                    //        Text = (String.IsNullOrEmpty(o.business_partner_code) ? "" : o.business_partner_code + " - ")
                    //            + (o.business_partner_name ?? ""),
                    //        o.business_partner_name
                    //    });
                    //return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.customer
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .OrderBy(o => o.business_partner_code)
                        .Select(o => new
                        {
                            Value = o.id,
                            Text = (String.IsNullOrEmpty(o.business_partner_code) ? "" : o.business_partner_code + " - ")
                                + (o.business_partner_name ?? ""),
                            o.business_partner_name
                        });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("CurrencyExchangeIdLookupByBLDate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> InvoiceCurrencyExchangeIdLookupByDo(DataSourceLoadOptions loadOptions, string source_currency_id, string invoice_date)
        {
            try
            {
                //var exchDate = dbContext.vw_do_inv_currency_exchange
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == despatch_order_id)
                //    .Select(o => new { Value = o.exchange_date })
                //    .FirstOrDefault();

                //if (exchDate != null)
                //{
                //    //var _exchDate = Convert.ToString(exchDate.Value);
                //    //if (_exchDate != "{ Value =  }")
                //    if (exchDate.Value != null)
                //    {
                //        invoice_date = exchDate.Value.ToString().Replace(" 00:00:00", "");
                //    }
                //}

                logger.Debug($"func InvoiceCurrencyExchangeIdLookup()");
                //var lookup = dbContext.vw_currency_exchange
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.start_date == Convert.ToDateTime(invoice_date))// && o.end_date >= Convert.ToDateTime(invoice_date))
                //    .OrderBy(o => o.end_date)
                //    .Select(o => new { Value = o.id, Text = o.source_currency_code + "-" + o.target_currency_code, o.source_currency_id, o.start_date, o.end_date, Xchange = o.exchange_rate });

                var tgl = Convert.ToDateTime(invoice_date);
                if (invoice_date == null) tgl = System.DateTime.Now;
                else tgl = Convert.ToDateTime(invoice_date);

                var daritgl = Convert.ToDateTime(tgl.AddDays(-1));
                var hinggatgl = Convert.ToDateTime(tgl.AddDays(1));

                var lookup = dbContext.vw_currency_exchange
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.end_date >= daritgl
                        && o.end_date <= hinggatgl
                        && o.source_currency_id == source_currency_id)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderBy(o => o.end_date)
                    .Select(o => new {
                        Value = o.id,
                        Text = o.source_currency_code + " - " + o.target_currency_code + " - " + o.end_date,
                        o.source_currency_id,
                        o.start_date,
                        o.end_date,
                        Xchange = o.exchange_rate
                    });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        //     [HttpGet("DesDemInvoiceIdLookup")]
        //     [ApiExplorerSettings(IgnoreApi = true)]
        //     public async Task<object> DesDemInvoiceIdLookup(DataSourceLoadOptions loadOptions)
        //     {
        //         try
        //         {
        //             var lookup = dbContext.despatch_demurrage_debit_credit_note
        //                 .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //                 .Select(o => new { Value = o.id, Text = o.invoice_number});
        //             return await DataSourceLoader.LoadAsync(lookup, loadOptions);
        //         }
        //catch (Exception ex)
        //{
        //	logger.Error(ex.InnerException ?? ex);
        //             return BadRequest(ex.InnerException?.Message ?? ex.Message);
        //}
        //     }

        //    [HttpGet("InvoiceTargetIdLookup")]
        //    [ApiExplorerSettings(IgnoreApi = true)]
        //    public async Task<object> InvoiceTargetIdLookup(DataSourceLoadOptions loadOptions)
        //    {
        //        try
        //        {
        //            var lookup = dbContext.vw_despatch_demurrage
        //                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //                            .Select(o => new { Value = o.contractor_id, Text = o.contractor_name })
        //                            .Union
        //                            (dbContext.vw_despatch_order
        //                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //                            .Select(o => new { Value = o.customer_id, Text = o.customer_name }))
        //                            .OrderBy(o => o.Text);
        //            return await DataSourceLoader.LoadAsync(lookup, loadOptions);
        //        }
        //        catch (Exception ex)
        //        {
        //logger.Error(ex.InnerException ?? ex);
        //return BadRequest(ex.InnerException?.Message ?? ex.Message);
        //        }
        //    }

        //[HttpGet("InvoiceTargetById")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> InvoiceTargetById(string Id, DataSourceLoadOptions loadOptions)
        //{
        //    return await DataSourceLoader.LoadAsync(
        //        dbContext.vw_despatch_demurrage.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //            .Select(o => new { id = o.contractor_id, invoice_target_type = "Supplier", desdem_type = "Despatch" })
        //        .Union
        //            (dbContext.vw_despatch_order.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //                        .Select(o => new { id = o.customer_id, invoice_target_type = "Buyer", desdem_type = "Demurrage" }))
        //        .Where(o => o.id == Id),
        //        loadOptions);
        //}

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
    }
}

using DataAccess.DTO;
using DataAccess.EFCore.Repository;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using Omu.ValueInjecter;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using DataAccess.EFCore;
using Microsoft.EntityFrameworkCore;
using HiSystems.Interpreter;
using Common;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/[controller]")]
    [ApiController]
    public class CreditLimitController : ApiBaseController
    {
        private const string stringQuotationPeriod_4LIBL = "4LIBL";
        private const string stringQuotationPeriod_4LI1LD = "4LI1LD";
        private const string stringQuotationPeriod_QBQBL = "QBQBL";
        private const string stringQuotationPeriod_QBQLD = "QBQLD";
        private const string stringPricingMethod_Calculated = "Calculated";
        private const string stringPricingMethod_Fixed = "Fixed";
        private const string stringsalesChargeType_premium= "premium";
        private const string stringsalesChargeType_adjustment = "adjustment";
        private const string stringQuotationPrice = "currentinvoiceunitprice()";

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public CreditLimitController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_sales_invoice_payment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_sales_invoice_payment.Where(o => o.customer_id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(sales_invoice_payment),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new sales_invoice_payment();
                    JsonConvert.PopulateObject(values, record);

                    record.id = Guid.NewGuid().ToString("N");
                    record.created_by = CurrentUserContext.AppUserId;
                    record.created_on = System.DateTime.Now;
                    record.modified_by = null;
                    record.modified_on = null;
                    record.is_active = true;
                    record.is_default = null;
                    record.is_locked = null;
                    record.entity_id = null;
                    record.owner_id = CurrentUserContext.AppUserId;
                    record.organization_id = CurrentUserContext.OrganizationId;
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                 
                    dbContext.sales_invoice_payment.Add(record);
                    await dbContext.SaveChangesAsync();

                    return Ok(record.payment_value);
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
                var record = dbContext.sales_invoice_payment
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
                        record.modified_on = System.DateTime.Now;

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

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] sales_invoice_payment Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.sales_invoice_payment
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
                            record.modified_on = System.DateTime.Now;

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok(record);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else if (await mcsContext.CanCreate(dbContext, nameof(sales_invoice_payment),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new sales_invoice_payment();
                        record.InjectFrom(Record);

                        record.id = Guid.NewGuid().ToString("N");
                        record.created_by = CurrentUserContext.AppUserId;
                        record.created_on = System.DateTime.Now;
                        record.modified_by = null;
                        record.modified_on = null;
                        record.is_active = true;
                        record.is_default = null;
                        record.is_locked = null;
                        record.entity_id = null;
                        record.owner_id = CurrentUserContext.AppUserId;
                        record.organization_id = CurrentUserContext.OrganizationId;
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        dbContext.sales_invoice_payment.Add(record);
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

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var record = dbContext.sales_invoice_payment
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.sales_invoice_payment.Remove(record);
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

        [HttpGet("SalesOrderIdLookup")]
        public async Task<object> SalesOrderIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.sales_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sales_order_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("DespatchOrderIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DespatchOrderIdLookup([FromQuery] string SalesOrderId, DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (string.IsNullOrEmpty(SalesOrderId))
                {
                    var lookup = dbContext.despatch_order
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.despatch_order_number, o.sales_order_id });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.despatch_order
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.sales_order_id == SalesOrderId)
                        .Select(o => new { Value = o.id, Text = o.despatch_order_number, o.sales_order_id });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("LookupQuotationTypeOnDespatchOrder")]
        public async Task<object> LookupQuotationTypeOnDespatchOrder(string despatch_order_id, DataSourceLoadOptions loadOptions)
        {
            if (despatch_order_id is null)
            {
                return BadRequest("null input parameter");
                //return 9.9999;
            }

            var quotationTypeArray = dbContext.vw_lookup_despatch_order_for_quotation
                .Where(o => o.despatch_order_id == despatch_order_id)
                .Select(o => new { Value = o.quotation_master_id, Text = o.quotation_name}); ;

            return await DataSourceLoader.LoadAsync(quotationTypeArray, loadOptions);

        }

        [HttpGet("LookupInvoiceTypeOnDespatchOrder")]
        public async Task<object> LookupInvoiceTypeOnDespatchOrder(string despatch_order_id, DataSourceLoadOptions loadOptions)
        {
            if (despatch_order_id is null)
            {
                return BadRequest("null input parameter");
                //return 9.9999;
            }

            var invoiceTypeArray = dbContext.vw_lookup_despatch_order_for_invoice
                .Where(o => o.despatch_order_id == despatch_order_id)
                .Select(o => new { Value = o.invoice_master_id, Text = o.invoice_name});

            return await DataSourceLoader.LoadAsync(invoiceTypeArray, loadOptions);

        }

        [HttpGet("LookupBothOnDespatchOrder")]
        public async Task<object> LookupBothOnDespatchOrder(string despatch_order_id, DataSourceLoadOptions loadOptions)
        {
            if (despatch_order_id is null )
            {
                return BadRequest("null input parameter");
                //return 9.9999;
            }

            var invoiceTypeList = dbContext.vw_lookup_despatch_order_for_invoice.Where(o => o.despatch_order_id == despatch_order_id)
                .Select(o => new { Value = o.invoice_master_id, Text = o.invoice_name });
            var invoiceTypeArray =  await invoiceTypeList.ToArrayAsync();
            var quotationTypeList = dbContext.vw_lookup_despatch_order_for_quotation.Where(o => o.despatch_order_id == despatch_order_id)
                .Select(o => new { Value = o.quotation_master_id, Text = o.quotation_name});
            var quotationTypeArray = await quotationTypeList.ToArrayAsync();

            var retVal2 = new
            {
                quotationType = quotationTypeArray,
                invoiceType = invoiceTypeArray
            };

            return retVal2;
            
        }
        
        
        //[HttpGet("CountPrice")]
        //public async Task<object> CountPrice(string despatch_order_id, string sales_contract_id, string sales_contract_term_id, string invoice_type_id, string quotation_type_id, DataSourceLoadOptions loadOptions)
        //{
        //    if (despatch_order_id is null && invoice_type_id is null && sales_contract_term_id is null)
        //    {
        //        return BadRequest("required input parameter is null");
        //        //return 9.9999;
        //    }

        //    logger.Debug($"string key = {despatch_order_id} dan  {invoice_type_id}");

        //    List<SalesPrice> arrayRetVal = new List<SalesPrice>();
        //    return arrayRetVal;


        //}

        [HttpGet("CheckSyntaxFormula")]
        public async Task<object> CheckSyntaxFormula(string tsyntax, DataSourceLoadOptions loadOptions)
        {
            string tstatus = "";
            string tmessage = "";
            string xsyntax = tsyntax.ToLower();
            var lookupMasterList = await dbContext.master_list.Where(o => o.item_group == "reserved-words").ToArrayAsync() ;
            foreach (master_list lookup in lookupMasterList)
            {
                xsyntax = xsyntax.Replace(lookup.item_name.ToLower(), "5");
            }
            var lookupAnalyteSymbol = dbContext.analyte.
                Where(o => o.analyte_symbol.Length > 0);
            foreach (analyte lookup in lookupAnalyteSymbol)
            {
                xsyntax = xsyntax.Replace((lookup.analyte_symbol.Replace(" ","")+".value()").ToLower(), "4");
                xsyntax = xsyntax.Replace((lookup.analyte_symbol.Replace(" ","")+".target()").ToLower(), "3");
            }
            try
            {
                var engine1 = new Engine();
                var expression1 = engine1.Parse(xsyntax);
                var resultText = expression1.Execute().ToString();
                tstatus = "OK";
                tmessage = "OK";
            }
            catch (Exception ex)
            {
                tstatus = "Error in syntax";
                tmessage = ex.Message;
            }
            var retVal = new
            {
                status = tstatus,
                message = tmessage
            };
            return retVal;
        }

        [HttpGet("GetReservedFunctions")]
        public async Task<object> GetReservedFunctions(DataSourceLoadOptions loadOptions)
        {
            ReservedWords rw;
            List<ReservedWords> listRW = new List<ReservedWords>();
            var lookupMasterList = await dbContext.master_list.
                Where(o => o.item_group == "reserved-functions").ToArrayAsync();
            foreach (master_list lookup in lookupMasterList)
            {
                rw = new ReservedWords(lookup.item_name, lookup.notes);
                listRW.Add(rw);
            }
            return listRW;
        }

    }

}
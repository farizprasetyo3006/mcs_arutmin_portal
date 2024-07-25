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
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using DataAccess.EFCore;
using Microsoft.EntityFrameworkCore;
using HiSystems.Interpreter;
using BusinessLogic;
using System.IO;
using Common;
using System.Dynamic;
using DataAccess.Select2;
using BusinessLogic.Entity;
using Microsoft.AspNetCore.Http.Extensions;
using SelectPdf;
using Common;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/[controller]")]
    [ApiController]
    public class SalesInvoiceApprovalController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SalesInvoiceApprovalController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                return await DataSourceLoader.LoadAsync(dbContext.vw_sales_invoice
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
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
                dbContext.vw_sales_invoice.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpGet("GetSalesInvoiceApproval/{Id}")]
        public object GetSalesInvoiceApproval(string Id)
        {
            try
            {
                var result = new sales_invoice_approval();

                result = dbContext.sales_invoice_approval.Where(x => x.sales_invoice_id == Id).FirstOrDefault();

                if (result == null)
                {
                    result = new sales_invoice_approval();
                    result.sales_invoice_id = Id;
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        //[HttpPost("ApproveUnapprove")]    //**** dengan trigger trf_sales_invoice_approval_sync di table sales_invoice_approval
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<IActionResult> ApproveUnapprove([FromForm] string key, [FromForm] string values)
        //{
        //    try
        //    {
        //        var record = dbContext.sales_invoice_approval
        //            .Where(o => (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
        //                || CurrentUserContext.IsSysAdmin) && o.sales_invoice_id == key)
        //            .FirstOrDefault();
        //        if (record != null)
        //        {
        //            var e = new entity();
        //            e.InjectFrom(record);

        //            JsonConvert.PopulateObject(values, record);

        //            record.InjectFrom(e);

        //            record.modified_by = CurrentUserContext.AppUserId;
        //            record.modified_on = System.DateTime.Now;

        //            if (record.approve_status == "APPROVED")
        //            {
        //                record.approve_status = "UNAPPROVED";
        //                record.disapprove_by_id = CurrentUserContext.AppUserId;
        //            }
        //            else
        //            {
        //                record.approve_status = "APPROVED";
        //                record.approve_by_id = CurrentUserContext.AppUserId;
        //            }

        //            await dbContext.SaveChangesAsync();
        //            return Ok(record);
        //        }
        //        else
        //        if (await mcsContext.CanCreate(dbContext, nameof(sales_invoice_approval),
        //            CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
        //        {
        //            var newRec = new sales_invoice_approval();
        //            JsonConvert.PopulateObject(values, newRec);

        //            newRec.id = Guid.NewGuid().ToString("N");
        //            newRec.created_by = CurrentUserContext.AppUserId;
        //            newRec.created_on = System.DateTime.Now;
        //            newRec.modified_by = null;
        //            newRec.modified_on = null;
        //            newRec.is_active = true;
        //            newRec.is_default = null;
        //            newRec.is_locked = null;
        //            newRec.entity_id = null;
        //            newRec.owner_id = CurrentUserContext.AppUserId;
        //            newRec.organization_id = CurrentUserContext.OrganizationId;

        //            newRec.approve_status = "APPROVED";
        //            newRec.approve_by_id = CurrentUserContext.AppUserId;

        //            dbContext.sales_invoice_approval.Add(newRec);
        //            await dbContext.SaveChangesAsync();
        //            return Ok(newRec);
        //        }
        //        else
        //        {
        //            return BadRequest("User is not authorized.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex.InnerException ?? ex);
        //        return BadRequest(ex.InnerException?.Message ?? ex.Message);
        //    }
        //}

        [HttpPost("ApproveUnapprove")]    //**** tanpa trigger trf_sales_invoice_approval_sync di table sales_invoice_approval
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ApproveUnapprove([FromForm] string key, [FromForm] string values)
        {
            dynamic result;
            bool? isApproved = null;

            var recSalesInvoice = dbContext.sales_invoice
                .Where(o => o.id == key)
                .FirstOrDefault();
            if (recSalesInvoice == null) return BadRequest("Data not found!.");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.sales_invoice_approval
                        .Where(o => o.sales_invoice_id == key)
                        .FirstOrDefault();
                    if (record == null)
                    {
                        logger.Debug($"ApproveUnapprove; sales_invoice_approval; new record; sales_invoice_id = {key}");

                        var newRec = new sales_invoice_approval();
                        JsonConvert.PopulateObject(values, newRec);

                        newRec.id = Guid.NewGuid().ToString("N");
                        newRec.created_by = CurrentUserContext.AppUserId;
                        newRec.created_on = System.DateTime.Now;
                        newRec.modified_by = null;
                        newRec.modified_on = null;
                        newRec.is_active = true;
                        newRec.is_default = null;
                        newRec.is_locked = null;
                        newRec.entity_id = null;
                        newRec.owner_id = CurrentUserContext.AppUserId;
                        newRec.organization_id = CurrentUserContext.OrganizationId;

                        newRec.approve_status = "APPROVED";
                        newRec.approve_by_id = CurrentUserContext.AppUserId;

                        dbContext.sales_invoice_approval.Add(newRec);
                        await dbContext.SaveChangesAsync();

                        result = newRec;
                        isApproved = true;
                    }
                    else
                    {
                        #region *** Three minutes validation ***
                        if (record.modified_on != null)
                        {
                            if (Convert.ToDateTime(record.modified_on) > System.DateTime.Now.AddMinutes(-3))
                            {
                                logger.Debug($"ApproveUnapprove; sales_invoice_approval; Three minutes validation-modified_on; sales_invoice_id = {key}");

                                return BadRequest("Please wait for about 3 minutes after last edit.");
                            }
                        }
                        else if (Convert.ToDateTime(record.created_on) > System.DateTime.Now.AddMinutes(-3))
                        {
                            logger.Debug($"ApproveUnapprove; sales_invoice_approval; Three minutes validation-created_on; sales_invoice_id = {key}");

                            return BadRequest("Please wait for about 3 minutes after last edit.");
                        }
                        #endregion

                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);
                        record.InjectFrom(e);

                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = System.DateTime.Now;

                        if (record.approve_status == "APPROVED")
                        {
                            record.approve_status = "UNAPPROVED";
                            record.disapprove_by_id = CurrentUserContext.AppUserId;
                            isApproved = false;
                        }
                        else
                        {
                            record.approve_status = "APPROVED";
                            record.approve_by_id = CurrentUserContext.AppUserId;
                            isApproved = true;
                        }

                        logger.Debug($"ApproveUnapprove; sales_invoice_approval; sales_invoice_id = {key}; approve_status = {record.approve_status};" +
                            " isApproved = {isApproved}");

                        await dbContext.SaveChangesAsync();
                        result = record;
                    }

                    string responseCode = "";
                    string responseText = "";

                    if (isApproved == false)    //*** Cancel invoice
                    {
                        //var recSIE = dbContext.sales_invoice_ell
                        //   .Where(o => o.id == key && o.sync_type == "UPDATE" && o.sync_status == "FAILED");
                        //if (recSIE.Count() > 0)
                        //    dbContext.sales_invoice_ell.RemoveRange(recSIE);

                        var SIERespCode = dbContext.sales_invoice_ell
                           .Where(o => o.id == key && o.sync_status == "SUCCESS" && o.response_code != null)
                           .OrderByDescending(o => o.created_on).FirstOrDefault();
                        if (SIERespCode != null)
                        {
                            responseCode = SIERespCode.response_code;
                            responseText = SIERespCode.response_text;
                        }
                        else
                        {
                            logger.Debug($"ApproveUnapprove; sales_invoice_ell; sales_invoice_id = {key}; isApproved = {isApproved}; Cancel invoice-CommitAsync");

                            await tx.CommitAsync();
                            return Ok(result);
                        }

                        var recEll = new sales_invoice_ell()
                        {
                            id = recSalesInvoice.id,
                            created_by = CurrentUserContext.AppUserId,
                            created_on = System.DateTime.Now,
                            modified_by = null,
                            modified_on = null,
                            is_active = recSalesInvoice.is_active,
                            is_locked = recSalesInvoice.is_locked,
                            is_default = recSalesInvoice.is_default,
                            owner_id = recSalesInvoice.owner_id,
                            organization_id = recSalesInvoice.organization_id,
                            entity_id = recSalesInvoice.entity_id,

                            despatch_order_id = recSalesInvoice.despatch_order_id,
                            quantity = recSalesInvoice.quantity,
                            uom_id = recSalesInvoice.uom_id,
                            unit_price = recSalesInvoice.unit_price,
                            currency_id = recSalesInvoice.currency_id,
                            invoice_date = recSalesInvoice.invoice_date,
                            accounting_period_id = recSalesInvoice.accounting_period_id,
                            invoice_number = recSalesInvoice.invoice_number,
                            sales_type_id = recSalesInvoice.sales_type_id,
                            invoice_type_id = recSalesInvoice.invoice_type_id,
                            customer_id = recSalesInvoice.customer_id,
                            seller_id = recSalesInvoice.seller_id,
                            bill_to = recSalesInvoice.bill_to,
                            contract_product_id = recSalesInvoice.contract_product_id,
                            notes = recSalesInvoice.notes,
                            bank_account_id = recSalesInvoice.bank_account_id,
                            downpayment = recSalesInvoice.downpayment,
                            total_price = recSalesInvoice.total_price,
                            quotation_type_id = recSalesInvoice.quotation_type_id,
                            currency_exchange_id = recSalesInvoice.currency_exchange_id,
                            lc_status = recSalesInvoice.lc_status,
                            lc_date_issue = recSalesInvoice.lc_date_issue,
                            lc_issuing_bank = recSalesInvoice.lc_issuing_bank,
                            freight_cost = recSalesInvoice.freight_cost,
                            correspondent_bank_id = recSalesInvoice.correspondent_bank_id,

                            sync_id = Guid.NewGuid().ToString("N"),
                            sync_type = "UPDATE",
                            sync_status = null,
                            error_msg = null,
                            response_code = responseCode,
                            response_text = responseText,
                            canceled = true
                        };

                        dbContext.sales_invoice_ell.Add(recEll);
                        await dbContext.SaveChangesAsync();

                        logger.Debug($"ApproveUnapprove; sales_invoice_ell; sales_invoice_id = {key}; isApproved = {isApproved}; Cancel invoice-New Record");
                    }
                    else //***** New invoice
                    {
                        var recSIE = dbContext.sales_invoice_ell
                            .Where(o => o.id == key).OrderByDescending(o => o.created_on)
                            .FirstOrDefault();
                        if (recSIE != null)
                        {
                            await tx.CommitAsync();
                            return Ok(result);
                        }

                        var recEll = new sales_invoice_ell()
                        {
                            id = recSalesInvoice.id,
                            created_by = CurrentUserContext.AppUserId,
                            created_on = System.DateTime.Now,
                            modified_by = null,
                            modified_on = null,
                            is_active = recSalesInvoice.is_active,
                            is_locked = recSalesInvoice.is_locked,
                            is_default = recSalesInvoice.is_default,
                            owner_id = recSalesInvoice.owner_id,
                            organization_id = recSalesInvoice.organization_id,
                            entity_id = recSalesInvoice.entity_id,

                            despatch_order_id = recSalesInvoice.despatch_order_id,
                            quantity = recSalesInvoice.quantity,
                            uom_id = recSalesInvoice.uom_id,
                            unit_price = recSalesInvoice.unit_price,
                            currency_id = recSalesInvoice.currency_id,
                            invoice_date = recSalesInvoice.invoice_date,
                            accounting_period_id = recSalesInvoice.accounting_period_id,
                            invoice_number = recSalesInvoice.invoice_number,
                            sales_type_id = recSalesInvoice.sales_type_id,
                            invoice_type_id = recSalesInvoice.invoice_type_id,
                            customer_id = recSalesInvoice.customer_id,
                            seller_id = recSalesInvoice.seller_id,
                            bill_to = recSalesInvoice.bill_to,
                            contract_product_id = recSalesInvoice.contract_product_id,
                            notes = recSalesInvoice.notes,
                            bank_account_id = recSalesInvoice.bank_account_id,
                            downpayment = recSalesInvoice.downpayment,
                            total_price = recSalesInvoice.total_price,
                            quotation_type_id = recSalesInvoice.quotation_type_id,
                            currency_exchange_id = recSalesInvoice.currency_exchange_id,
                            lc_status = recSalesInvoice.lc_status,
                            lc_date_issue = recSalesInvoice.lc_date_issue,
                            lc_issuing_bank = recSalesInvoice.lc_issuing_bank,
                            freight_cost = recSalesInvoice.freight_cost,
                            correspondent_bank_id = recSalesInvoice.correspondent_bank_id,

                            sync_id = Guid.NewGuid().ToString("N"),
                            sync_type = "INSERT",
                            sync_status = null,
                            error_msg = null,
                            response_code = null,
                            response_text = null,
                            canceled = null
                        };

                        dbContext.sales_invoice_ell.Add(recEll);
                        await dbContext.SaveChangesAsync();

                        logger.Debug($"ApproveUnapprove; sales_invoice_ell; sales_invoice_id = {key}; isApproved = {isApproved}; New Sales Invoice");
                    }

                    await tx.CommitAsync();
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.InnerException ?? ex);
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(sales_invoice),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var record = new sales_invoice();
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

                    dbContext.sales_invoice.Add(record);
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
                var record = dbContext.sales_invoice
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
        public async Task<IActionResult> SaveData([FromBody] sales_invoice Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.sales_invoice
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
                    else if (await mcsContext.CanCreate(dbContext, nameof(sales_invoice),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new sales_invoice();
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

                        dbContext.sales_invoice.Add(record);
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
                var record = dbContext.sales_invoice
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    var recordX = dbContext.sales_invoice_charges
                        .Where(o => o.sales_invoice_id == key)
                        ;
                    if (recordX != null)
                    {
                        foreach (sales_invoice_charges recX in recordX)
                        {
                            dbContext.sales_invoice_charges.Remove(recX);
                        }
                    }

                    dbContext.sales_invoice.Remove(record);
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

        [HttpGet("DespatchOrderDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DespatchOrderDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_despatch_order.Where(o => o.id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)),
                loadOptions);
        }

        [HttpGet("SalesInvoiceIdLookup")]
        public async Task<object> SalesInvoiceIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.sales_invoice
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.invoice_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("InvoiceCurrencyExchangeIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> InvoiceCurrencyExchangeIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                logger.Debug($"func InvoiceCurrencyExchangeIdLookup()");
                var lookup = dbContext.vw_currency_exchange
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderBy(o => o.end_date)
                    .Select(o => new { Value = o.id, Text = o.source_currency_code + "-" + o.target_currency_code, o.source_currency_id, o.start_date, o.end_date, Xchange = o.exchange_rate });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SalesInvoiceDetail/{Id}")]
        public async Task<object> SalesInvoiceDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.sales_invoice_detail.Where(o => o.organization_id == CurrentUserContext.AppUserId
                    && o.sales_invoice_id == Id),
                loadOptions);
        }

        [HttpGet("LookupBothOnDespatchOrder")]
        public async Task<object> LookupBothOnDespatchOrder(string despatch_order_id, DataSourceLoadOptions loadOptions)
        {
            if (despatch_order_id is null)
            {
                return BadRequest("null input parameter");
                //return 9.9999;
            }

            var invoiceTypeList = dbContext.vw_lookup_despatch_order_for_invoice.Where(o => o.despatch_order_id == despatch_order_id)
                .Select(o => new { Value = o.invoice_master_id, Text = o.invoice_name });
            var invoiceTypeArray = await invoiceTypeList.ToArrayAsync();
            var quotationTypeList = dbContext.vw_lookup_despatch_order_for_quotation.Where(o => o.despatch_order_id == despatch_order_id)
                .Select(o => new { Value = o.quotation_master_id, Text = o.quotation_name });
            var quotationTypeArray = await quotationTypeList.ToArrayAsync();

            var retVal2 = new
            {
                quotationType = quotationTypeArray,
                invoiceType = invoiceTypeArray
            };

            return retVal2;

        }

        [HttpPost("InsertPayment")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertPayment([FromForm] string values)
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

                    var lookupCurrency = await dbContext.vw_currency
                    .Where(o => o.currency_code == record.currency_code)
                    .FirstOrDefaultAsync();
                    record.currency_id = lookupCurrency.id;

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

    }
}
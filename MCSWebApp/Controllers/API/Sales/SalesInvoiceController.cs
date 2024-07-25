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
using NPOI.SS.Formula.Functions;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore.Query.Internal;
using DocumentFormat.OpenXml.Bibliography;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/[controller]")]
    [ApiController]
    public class SalesInvoiceController : ApiBaseController
    {
        private const string stringQuotationPeriod_4LIBL = "4LIBL";
        private const string stringQuotationPeriod_4LI1LD = "4LI1LD";
        private const string stringQuotationPeriod_QBQBL = "QBQBL";
        private const string stringQuotationPeriod_QBQLD = "QBQLD";
        private const string stringQuotationPeriod_MILD = "MILD";
        private const string stringQuotationPeriod_MIBLD = "MIBLD";
        private const string stringQuotationPeriod_APMILD = "APMILD";
        private const string stringQuotationPeriod_APMIBL = "APMIBL";

        private const string stringIndexCalc_IF4 = "IF4";
        private const string stringIndexCalc_EF3 = "EF3";
        private const string stringIndexCalc_EF4 = "EF4";

        private const string stringPricingMethod_Calculated = "Calculated";
        private const string stringPricingMethod_Fixed = "Fixed";
        private const string stringsalesChargeType_discount = "discount";
        private const string stringsalesChargeType_premium = "premium";
        private const string stringsalesChargeType_adjustment = "adjustment";
        private const string stringQuotationPrice = "baseunitprice()";
        //bobby 20220722 function baseunitprice
        private const string stringCurrentPrice = "currentinvoiceunitprice()";


        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        private decimal globalRemainedCreditLimit = 0;
        private string globalCustomerId;

        public SalesInvoiceController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                return await DataSourceLoader.LoadAsync(dbContext.vw_sales_invoice_datagrid
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

                    // update table sales_invoice_charges
                    vw_sales_contract_charges vwSalesContractCharges = new vw_sales_contract_charges();
                    JsonConvert.PopulateObject(values, vwSalesContractCharges);
                    var records_vw_sales_contract_charge = dbContext.vw_sales_contract_charges
                        .Where(o => o.sales_contract_term_id == vwSalesContractCharges.sales_contract_term_id && o.sales_charge_type_name == "Adjustment")
                        ;
                    if (records_vw_sales_contract_charge.Count() > 0)
                    {
                        var array_vw_contract_charges = await records_vw_sales_contract_charge.ToArrayAsync();
                        foreach (vw_sales_contract_charges item_contract_charge in array_vw_contract_charges)
                        {
                            var recordSalesCharge = new sales_invoice_charges();
                            JsonConvert.PopulateObject(values, recordSalesCharge);

                            recordSalesCharge.id = Guid.NewGuid().ToString("N");
                            recordSalesCharge.created_by = CurrentUserContext.AppUserId;
                            recordSalesCharge.created_on = System.DateTime.Now;
                            recordSalesCharge.modified_by = null;
                            recordSalesCharge.modified_on = null;
                            recordSalesCharge.is_active = true;
                            recordSalesCharge.is_default = null;
                            recordSalesCharge.is_locked = null;
                            recordSalesCharge.entity_id = null;
                            recordSalesCharge.owner_id = CurrentUserContext.AppUserId;
                            recordSalesCharge.organization_id = CurrentUserContext.OrganizationId;
                            recordSalesCharge.sales_invoice_id = record.id;
                            recordSalesCharge.sales_charge_id = item_contract_charge.sales_charge_id;
                            recordSalesCharge.sales_charge_code = item_contract_charge.sales_charge_code;
                            recordSalesCharge.sales_charge_name = item_contract_charge.sales_charge_name;

                            var valuesArray = values.Split("\"" + item_contract_charge.sales_charge_code + "\"");
                            if (valuesArray.Length > 1)
                            {
                                var valuesArray2 = valuesArray[1].Split(',');
                                var isPrice = valuesArray2[0].Substring(1).Replace(".", ",");

                                CultureInfo culture_curr = CultureInfo.CurrentCulture;
                                CultureInfo.CurrentCulture = new CultureInfo("en-ID", false);
                                recordSalesCharge.price = Convert.ToDecimal(isPrice, CultureInfo.CurrentCulture);
                                CultureInfo.CurrentCulture = culture_curr;

                                dbContext.sales_invoice_charges.Add(recordSalesCharge);
                            }

                        }

                    }

                    // look up currency_id from view vw_sales_contract_term
                    if ((record.currency_id == null) || (record.currency_id == ""))
                    {
                        var record_vw_sales_contract_term = await dbContext.vw_sales_contract_term
                        .Where(o => o.id == vwSalesContractCharges.sales_contract_term_id).FirstOrDefaultAsync()
                        ;
                        record.currency_id = record_vw_sales_contract_term.currency_id;
                    }

                    // update table customer
                    var recordCustId = dbContext.customer
                        .Where(o => o.id == globalCustomerId)
                        .FirstOrDefault();
                    if (recordCustId != null)
                    {
                        var e = new entity();
                        e.InjectFrom(recordCustId);

                        JsonConvert.PopulateObject(values, recordCustId);

                        recordCustId.InjectFrom(e);
                        recordCustId.modified_by = CurrentUserContext.AppUserId;
                        recordCustId.modified_on = System.DateTime.Now;
                        recordCustId.remained_credit_limit = (decimal)globalRemainedCreditLimit;
                        dbContext.customer.Update(recordCustId);

                    }

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
                var cekApproval = dbContext.sales_invoice_approval
                    .Where(o => o.sales_invoice_id == key && o.approve_status == "APPROVED").FirstOrDefault();
                if (cekApproval != null)
                    return BadRequest("Modification can't be saved, it's already been approved.");

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
                return Ok(record);
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

        [HttpPost("SaveDataPayment")]
        public async Task<IActionResult> SaveDataPayment([FromBody] sales_invoice_payment Record)
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


        //[HttpGet("Finalisasi/{Id}")]
        //public async Task<IActionResult> Finalisasi(string Id)
        //{
        //    using (var tx = await dbContext.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            var record = dbContext.sales_invoice
        //                .Where(o => o.id == Id)
        //                .FirstOrDefault();
        //            if (record != null)
        //            {
        //                record.approve_status = "APPROVED";
        //                record.approve_by_id = CurrentUserContext.AppUserId;

        //                await dbContext.SaveChangesAsync();
        //            }

        //            var sales_invoice_ell = dbContext.sales_invoice_ell
        //                .Where(o => o.id == Id && o.sync_status == "PENDING")
        //                .FirstOrDefault();
        //            if (sales_invoice_ell != null)
        //            {
        //                sales_invoice_ell.sync_status = null;
        //                await dbContext.SaveChangesAsync();
        //            }

        //            await tx.CommitAsync();
        //            return Ok(record);
        //        }
        //        catch (Exception ex)
        //        {
        //            await tx.RollbackAsync();
        //            logger.Error(ex.InnerException ?? ex);
        //            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        //        }
        //    }
        //}

        //[HttpGet("Unapprove/{Id}")]
        //public async Task<IActionResult> Unapprove(string Id)
        //{
        //    using (var tx = await dbContext.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            var record = dbContext.sales_invoice
        //                .Where(o => o.id == Id)
        //                .FirstOrDefault();
        //            if (record != null)
        //            {
        //                record.approve_status = "UNAPPROVED";
        //                record.disapprove_by_id = CurrentUserContext.AppUserId;

        //                await dbContext.SaveChangesAsync();
        //            }

        //            var sales_invoice_ell = dbContext.sales_invoice_ell
        //                .Where(o => o.id == Id && o.sync_status == "PENDING")
        //                .FirstOrDefault();
        //            if (sales_invoice_ell != null)
        //            {
        //                sales_invoice_ell.sync_status = null;

        //                await dbContext.SaveChangesAsync();
        //            }

        //            await tx.CommitAsync();
        //            return Ok(record);
        //        }
        //        catch (Exception ex)
        //        {
        //            await tx.RollbackAsync();
        //            logger.Error(ex.InnerException ?? ex);
        //            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        //        }
        //    }
        //}

        [HttpGet("RequestApproval/{Id}")]
        public async Task<IActionResult> RequestApproval(string Id)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    dynamic result;

                    var record = dbContext.sales_invoice_approval
                        .Where(o => o.sales_invoice_id == Id)
                        .FirstOrDefault();
                    if (record == null)
                    {
                        var recordSalesInvoiceApproval = new sales_invoice_approval();

                        recordSalesInvoiceApproval.id = Guid.NewGuid().ToString("N");
                        recordSalesInvoiceApproval.created_by = CurrentUserContext.AppUserId;
                        recordSalesInvoiceApproval.created_on = System.DateTime.Now;
                        recordSalesInvoiceApproval.modified_by = null;
                        recordSalesInvoiceApproval.modified_on = null;
                        recordSalesInvoiceApproval.is_active = true;
                        recordSalesInvoiceApproval.is_default = null;
                        recordSalesInvoiceApproval.is_locked = null;
                        recordSalesInvoiceApproval.entity_id = null;
                        recordSalesInvoiceApproval.owner_id = CurrentUserContext.AppUserId;
                        recordSalesInvoiceApproval.organization_id = CurrentUserContext.OrganizationId;

                        recordSalesInvoiceApproval.sales_invoice_id = Id;
                        recordSalesInvoiceApproval.approve_status = "UNAPPROVED";

                        dbContext.sales_invoice_approval.Add(recordSalesInvoiceApproval);
                        await dbContext.SaveChangesAsync();

                        result = recordSalesInvoiceApproval;
                        //await tx.CommitAsync();
                        //return Ok(recordSalesInvoiceApproval);
                    }
                    else
                    {
                        ////return null;

                        //record.modified_by = CurrentUserContext.AppUserId;
                        //record.modified_on = System.DateTime.Now;
                        //record.approve_status = "UNAPPROVED";
                        //record.approve_by_id = null;

                        //await dbContext.SaveChangesAsync();

                        result = record;
                    }

                    #region Send Email
                    var recEmail = new email_notification();

                    var recSalesInvoice = dbContext.vw_sales_invoice
                        .Where(o => o.id == Id)
                        .FirstOrDefault();
                    if (recSalesInvoice != null)
                    {
                        recEmail.id = Guid.NewGuid().ToString("N");
                        recEmail.created_by = CurrentUserContext.AppUserId;
                        recEmail.created_on = System.DateTime.Now;
                        recEmail.modified_by = null;
                        recEmail.modified_on = null;
                        recEmail.is_active = true;
                        recEmail.is_default = null;
                        recEmail.is_locked = null;
                        recEmail.entity_id = null;
                        recEmail.owner_id = CurrentUserContext.AppUserId;
                        recEmail.organization_id = CurrentUserContext.OrganizationId;

                        recEmail.email_subject = "SALES INVOICE #" + recSalesInvoice.invoice_number;
                        var url = HttpContext.Request.GetEncodedUrl();
                        url = url.Substring(0, url.IndexOf("/api"));

                        string teks = string.Concat("<p><strong style='style=width: 100%; font-size: 14pt; font-family: Tahoma; text-align: center'>",
                            "SALES INVOICE #", recSalesInvoice.invoice_number, "</strong>",
                            "<p>Dear Pak Brilianto,</p>",
                            "<p> </p>",
                            "<p>Mohon bantuannya untuk review dan konfirmasi shadow invoice terlampir.</p>",
                            "<p>Thank you.</p>",
                            "<p> </p>",
                            "<p>Please find the attachment and sales invoice detail in this link below:</p>",
                            "<div> <a href=", url, "/Sales/SalesInvoiceApproval/Index?Id=", Id, "&openEditingForm=false>",
                            "See Sales Invoice Approval</a> </div>",
                            "<p> </p>",
                            "<p>Thank You,</p>",
                            "<p>Best Regards.</p>"
                            );

                        recEmail.email_content = teks;
                        recEmail.delivery_schedule = System.DateTime.Now;
                        recEmail.table_name = "vw_sales_invoice";
                        recEmail.fields = "sales_contract_name, sales_contract_term_name";
                        recEmail.criteria = string.Format("id='{0}'", Id);
                        recEmail.email_code = "SalesInvoice-" + recSalesInvoice.invoice_number;

                        dbContext.email_notification.Add(recEmail);
                        await dbContext.SaveChangesAsync();

                        result = recEmail;
                    }
                    #endregion

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

        [HttpGet("GcvArb")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GcvArb(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_quality_sampling_analyte
                    .Where(o => o.despatch_order_id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin))
                    .Where(o => o.analyte_id == "ad0f6aafe6a14bdbb47522f8d0f15bea")
                    .Select(o => new { Value = o.id, Text = o.analyte_value });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("GcvAdb")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GcvAdb(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_quality_sampling_analyte
                    .Where(o => o.despatch_order_id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin))
                    .Where(o => o.analyte_id == "477c28cf7b8248d686eb0a6731210f15")
                    .Select(o => new { Value = o.id, Text = o.analyte_value });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("TsAdb")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> TsAdb(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_quality_sampling_analyte
                    .Where(o => o.despatch_order_id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin))
                    .Where(o => o.analyte_id == "fd7266e329be4311b2a05bf9776d7b75")
                    .Select(o => new { Value = o.id, Text = o.analyte_value });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("TsArb")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> TsArb(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_quality_sampling_analyte
                    .Where(o => o.despatch_order_id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin))
                    .Where(o => o.analyte_id == "2c816f00c9a64fd59ba22e11dcd27e3f")
                    .Select(o => new { Value = o.id, Text = o.analyte_value });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("AshAdb")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AshAdb(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_quality_sampling_analyte
                    .Where(o => o.despatch_order_id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin))
                    .Where(o => o.analyte_id == "98c6162bfa084fcd8378f158ce8b0388")
                    .Select(o => new { Value = o.id, Text = o.analyte_value });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("AshArb")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AshArb(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_quality_sampling_analyte
                    .Where(o => o.despatch_order_id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin))
                    .Where(o => o.analyte_id == "e07a0e0d60db4ec0a9e6a6034eb52e01")
                    .Select(o => new { Value = o.id, Text = o.analyte_value });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("TmArb")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> TmArb(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_quality_sampling_analyte
                    .Where(o => o.despatch_order_id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin))
                    .Where(o => o.analyte_id == "e410ab5fc90544168169dbee2fc08504")
                    .Select(o => new { Value = o.id, Text = o.analyte_value });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ImAdb")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ImAdb(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_quality_sampling_analyte
                    .Where(o => o.despatch_order_id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin))
                    .Where(o => o.analyte_id == "aea5c4a6a37c4680868c8ce4c815b6b5")
                    .Select(o => new { Value = o.id, Text = o.analyte_value });
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

        [HttpGet("ShippingInstructionDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ShippingInstructionDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.shipping_instruction.Where(o => o.despatch_order_id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)),
                loadOptions);
        }

        [HttpGet("ShippingCostDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ShippingCostDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_shipping_cost.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.despatch_order_id == Id),
                loadOptions);
        }

        [HttpGet("DespatchOrderIdLookup/{so}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DespatchOrderIdLookup(DataSourceLoadOptions loadOptions,string so,bool inserting)
        {
            try
            {
                var excludedShippingOrder = dbContext.vw_sales_invoice.Where(o => o.invoice_type_name == "Commercial")
                    .Select(o=>o.despatch_order_id)
                    .Distinct()
                    .ToArray();
                    if (so != "null")
                    {
                        var lookup1 = dbContext.despatch_order
                                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                                .Where(o => !excludedShippingOrder.Contains(o.id))
                                                .Select(o => new { Value = o.id, Text = o.despatch_order_number });
                        var lookup2 = dbContext.despatch_order.Where(o=>o.id == so).Select(o => new { Value = o.id, Text = o.despatch_order_number });
                        var lookup = lookup1.Union(lookup2);
                        return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                    }
                    else
                    {
                        var lookup = dbContext.despatch_order
                                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                                .Where(o => !excludedShippingOrder.Contains(o.id))
                                                .Select(o => new { Value = o.id, Text = o.despatch_order_number });
                        return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                    }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        [HttpGet("AllDespatchOrderIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AllDespatchOrderIdLookup(DataSourceLoadOptions loadOptions, string so, bool inserting)
        {
            try
            {
                var lookup = dbContext.despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
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

        [HttpGet("InvoiceCurrencyExchangeIdLookupByDo")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> InvoiceCurrencyExchangeIdLookupByDo(DataSourceLoadOptions loadOptions, string despatch_order_id, string invoice_date)
        {
            try
            {
                var exchDate = dbContext.vw_do_inv_currency_exchange
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == despatch_order_id)
                    .Select(o => new { Value = o.exchange_date })
                    .FirstOrDefault();

                if (exchDate != null)
                {
                    //var _exchDate = Convert.ToString(exchDate.Value);
                    //if (_exchDate != "{ Value =  }")
                    if (exchDate.Value != null)
                    {
                        invoice_date = exchDate.Value.ToString().Replace(" 00:00:00", "");
                    }
                }

                //logger.Debug($"func InvoiceCurrencyExchangeIdLookup()");
                //var lookup = dbContext.vw_currency_exchange
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.start_date == Convert.ToDateTime(invoice_date))// && o.end_date >= Convert.ToDateTime(invoice_date))
                //    .OrderBy(o => o.end_date)
                //    .Select(o => new { Value = o.id, Text = o.source_currency_code + "-" + o.target_currency_code, o.source_currency_id, o.start_date, o.end_date, Xchange = o.exchange_rate });

                var tgl = Convert.ToDateTime(invoice_date);
                if (invoice_date == null) tgl = System.DateTime.Now;

                //var daritgl = Convert.ToDateTime(tgl.AddDays(-1));
                //var hinggatgl = Convert.ToDateTime(tgl.AddDays(1));

                var lookup = dbContext.vw_currency_exchange
                    //.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.end_date >= daritgl
                    //    && o.end_date <= hinggatgl)
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.start_date <= tgl && tgl <= o.end_date)
                    .OrderBy(o => o.end_date)
                    .Select(o => new
                    {
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

        [HttpGet("SalesInvoiceDetail/{Id}")]
        public async Task<object> SalesInvoiceDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.sales_invoice_detail.Where(o => o.organization_id == CurrentUserContext.AppUserId
                    && o.sales_invoice_id == Id),
                loadOptions);
        }

        //public double RoundI(double number, double roundingInterval)
        //{
        //    return (double)((decimal)roundingInterval * Math.Round((decimal)number / (decimal)roundingInterval, MidpointRounding.AwayFromZero));
        //}

        decimal patternAdjustment(string the_text, List<AdjustmentCalculation> listAdjCalc)
        {
            decimal retVal = 0;
            var tText = the_text.Replace(" ", String.Empty).Replace("\n", String.Empty);
            string _value = ".value()";
            string _target = ".target()";
            string _min = ".min()";
            string _max = ".max()";
            string base_invoice_unit_price = stringQuotationPrice;
            string current_invoice_unit_price = stringCurrentPrice;

            var strUnitPrice = "0.0";
            var strCurrentPrice = "0.0";
            if (listAdjCalc.Count > 1)
            {
                try
                {
                    strUnitPrice = listAdjCalc[1].analyte_value.ToString().Replace(',', '.');
                    strCurrentPrice = listAdjCalc[2].analyte_value.ToString().Replace(',', '.');
                }
                catch (Exception e)
                {
                    logger.Debug($"got exception inside patternAdjustment = {e.Message}");
                }
            }
            if (listAdjCalc.Count != 0)
            {
                tText = tText.Replace(listAdjCalc[0].analyte_symbol + _value, listAdjCalc[0].analyte_value.ToString().Replace(',', '.'));
                tText = tText.Replace(listAdjCalc[0].analyte_symbol + _target, listAdjCalc[0].analyte_target.ToString().Replace(',', '.'));

                //tText = tText.Replace(listAdjCalc[0].analyte_symbol + _min, listAdjCalc[0].analyte_value.ToString().Replace(',', '.'));
                tText = tText.Replace(listAdjCalc[0].analyte_symbol + _min, listAdjCalc[0].analyte_minimum.ToString().Replace(',', '.'));

                //tText = tText.Replace(listAdjCalc[0].analyte_symbol + _max, listAdjCalc[0].analyte_target.ToString().Replace(',', '.'));
                tText = tText.Replace(listAdjCalc[0].analyte_symbol + _max, listAdjCalc[0].analyte_maximum.ToString().Replace(',', '.'));

                tText = tText.Replace(base_invoice_unit_price, strUnitPrice.Replace(',', '.'));
                tText = tText.Replace(current_invoice_unit_price, strCurrentPrice.Replace(',', '.'));
                if (listAdjCalc.Count > 3)
                {
                    tText = tText.Replace(listAdjCalc[3].analyte_symbol + _value, listAdjCalc[3].analyte_value.ToString().Replace(',', '.'));
                    tText = tText.Replace(listAdjCalc[3].analyte_symbol + _target, listAdjCalc[3].analyte_target.ToString().Replace(',', '.'));

                    tText = tText.Replace(listAdjCalc[3].analyte_symbol + _min, listAdjCalc[3].analyte_minimum.ToString().Replace(',', '.'));
                    tText = tText.Replace(listAdjCalc[3].analyte_symbol + _max, listAdjCalc[3].analyte_maximum.ToString().Replace(',', '.'));

                }
                if (listAdjCalc.Count > 6)
                {
                    tText = tText.Replace(listAdjCalc[6].analyte_symbol + _value, listAdjCalc[6].analyte_value.ToString().Replace(',', '.'));
                    tText = tText.Replace(listAdjCalc[6].analyte_symbol + _target, listAdjCalc[6].analyte_target.ToString().Replace(',', '.'));

                    tText = tText.Replace(listAdjCalc[6].analyte_symbol + _min, listAdjCalc[6].analyte_minimum.ToString().Replace(',', '.'));
                    tText = tText.Replace(listAdjCalc[6].analyte_symbol + _max, listAdjCalc[6].analyte_maximum.ToString().Replace(',', '.'));
                }
                if (listAdjCalc.Count > 9)
                {
                    tText = tText.Replace(listAdjCalc[9].analyte_symbol + _value, listAdjCalc[9].analyte_value.ToString().Replace(',', '.'));
                    tText = tText.Replace(listAdjCalc[9].analyte_symbol + _target, listAdjCalc[9].analyte_target.ToString().Replace(',', '.'));

                    tText = tText.Replace(listAdjCalc[9].analyte_symbol + _min, listAdjCalc[9].analyte_minimum.ToString().Replace(',', '.'));
                    tText = tText.Replace(listAdjCalc[9].analyte_symbol + _max, listAdjCalc[9].analyte_maximum.ToString().Replace(',', '.'));
                }
            }
            CultureInfo culture_curr = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("en-ID", false);
            var engine1 = new Engine();
            var expression1 = engine1.Parse(tText);
            var resultText = expression1.Execute().ToString();
            //bobby 20220419 menghitung formula
            retVal = Convert.ToDecimal(resultText, CultureInfo.CurrentCulture);

            logger.Debug($"resultText is {resultText} and retVal is {retVal}");
            CultureInfo.CurrentCulture = culture_curr;
            return retVal;
        }

        bool patternPrerequisitePremium(string the_text, string p_vessel_id)
        {
            bool isOK = false;
            bool lanjut = false;
            //string sqlPattern = @"([\+\*-/])([0-9]*)([.,]?)([0-9]*)";
            string defaultPattern = @"([_\-a-zA-z]+)[.]([_\-a-zA-z]+)\s*=\s*(.*)";
            //string numeric1Pattern = @"([\+\*-/])([0-9]*)([.,]?)([0-9]*)";

            //double returnValue = quotation_price;

            string replacement1 = "$1";
            string replacement2 = "$2";
            string replacement3 = "$3";
            string inputed_hex1, inputed_hex2, inputed_hex3;
            //double sales_contract_charge_premium_value = 0.0;
            if (the_text.Equals("true"))
            {
                lanjut = true;
                isOK = true;
            }
            else if (the_text.Equals("false"))
            {
                lanjut = false;
            }
            else
            {
                if (Regex.IsMatch(the_text, defaultPattern))
                {
                    lanjut = true;
                }
            }

            if (lanjut)
            {
                inputed_hex1 = Regex.Replace(the_text, defaultPattern, replacement1);
                inputed_hex2 = Regex.Replace(the_text, defaultPattern, replacement2);
                inputed_hex3 = Regex.Replace(the_text, defaultPattern, replacement3);

                if (inputed_hex1 == "vessel" && inputed_hex2 == "is_geared")
                {
                    var vessel_lookup = dbContext.vessel.Where(o => o.id == p_vessel_id)
                        .FirstOrDefault();

                    if (inputed_hex3 == "false")
                    {
                        isOK = !(bool)vessel_lookup.is_geared;
                    }
                    else if (inputed_hex3 == "true")
                    {
                        isOK = (bool)vessel_lookup.is_geared;
                    }
                    else
                    {

                    }

                }

            }
            else
            {
                Console.WriteLine("not match:");
            }
            return isOK;
        }

        private decimal patternFormulaPremium(string the_text, decimal quotation_price)
        {
            decimal returnValue = 0;
            returnValue = quotation_price;
            string pattern = @"([\+\*-/])([0-9]*)([.,]?)([0-9]*)";
            string replacement1 = "$1";
            string replacement2 = "$2$3$4";
            string replacement_separator = "$3";
            string inputed_hex1, inputed_hex2, separator;
            decimal sales_contract_charge_premium_value = 0;

            if (Regex.IsMatch(the_text, pattern))
            {
                inputed_hex1 = Regex.Replace(the_text, pattern, replacement1);
                inputed_hex2 = Regex.Replace(the_text, pattern, replacement2);
                separator = Regex.Replace(the_text, pattern, replacement_separator);
                Console.WriteLine("value1:" + inputed_hex1);
                Console.WriteLine("value2:" + inputed_hex2);

                NumberFormatInfo nfi = new NumberFormatInfo();
                if (separator.Length > 0)
                {
                    nfi.NumberDecimalSeparator = separator;
                    nfi.CurrencyDecimalSeparator = separator;
                }
                sales_contract_charge_premium_value = decimal.Parse(inputed_hex2, nfi);

                switch (inputed_hex1)
                {
                    case "+": returnValue += sales_contract_charge_premium_value; break;
                    case "-": returnValue -= sales_contract_charge_premium_value; break;
                    case "*": returnValue *= sales_contract_charge_premium_value; break;
                    case "/": returnValue /= sales_contract_charge_premium_value; break;

                    default: break;
                }
            }
            else
            {
                Console.WriteLine("not match:");
            }
            return returnValue;
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
                .Select(o => new { Value = o.quotation_master_id, Text = o.quotation_name }); ;

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
                .Select(o => new { Value = o.invoice_master_id, Text = o.invoice_name });

            return await DataSourceLoader.LoadAsync(invoiceTypeArray, loadOptions);

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

        [HttpGet("CountPrice")]
        public async Task<object> CountPrice(string despatch_order_id, string sales_contract_id, string sales_contract_term_id, string invoice_type_id, string quotation_type_id, bool isEditing, DataSourceLoadOptions loadOptions)
        {
            if (despatch_order_id is null && invoice_type_id is null && sales_contract_term_id is null)
            {
                return BadRequest("required input parameter is null");
                //return 9.9999;
            }

            logger.Debug($"string key = {despatch_order_id} dan  {invoice_type_id}");

            var splitInvoiceTypeId = invoice_type_id.Split('-');
            var sales_contract_payment_term_id = splitInvoiceTypeId[0];
            var real_invoice_type_id = splitInvoiceTypeId[1];

            // get how many payment term is listed in contract 
            var vw_payment_term_data = dbContext.vw_sales_contract_payment_term
                .Where(o => o.sales_contract_term_id == sales_contract_term_id)
                ;
            var array_vw_payment_term_data = await vw_payment_term_data.ToArrayAsync();
            if (array_vw_payment_term_data.Length == 0)
            {
                return BadRequest("no payment term is listed in " + array_vw_payment_term_data[0].contract_term_name);
            }

            // get how many invoice already generated by despatch_order_id
            var vw_sales_invoice_data = dbContext.vw_sales_invoice
                .Where(o => o.despatch_order_id == despatch_order_id)
                ;
            var array_vw_sales_invoice_data = await vw_sales_invoice_data.ToArrayAsync();

            var numberInvoicesCanBeIssued = array_vw_payment_term_data.Length - array_vw_sales_invoice_data.Length;
            //if (numberInvoicesCanBeIssued == 0)
            if (numberInvoicesCanBeIssued == 0 && isEditing == false)
            {
                return BadRequest("All invoice had already issued from Shipping order " + array_vw_sales_invoice_data[0].despatch_order_number);
            }

            // Get Invoice Type Data
            var invoice_type_data = await dbContext.master_list
            .Where(o => o.id == real_invoice_type_id).FirstOrDefaultAsync();

            //var quotation_type_data = await dbContext.master_list
            //    .Where(o => o.id == quotation_type_id).FirstOrDefaultAsync()
            //    ;

            List<SalesPrice> arrayRetVal = new List<SalesPrice>();

            var bill_of_lading = new System.DateTime(2020, 3, 30);
            var laycan_date = new System.DateTime(2020, 3, 30);
            var invoice_date = new System.DateTime(2020, 3, 30);
            decimal t_quotation_price = 0;
            decimal t_current_quotation_price = 0;

            var record_despatch_order = await dbContext.despatch_order
                .Where(o => o.id == despatch_order_id)
                .FirstOrDefaultAsync();
            if (record_despatch_order == null)
            {
                return BadRequest("Shipping Order Laycan Date is not available.");
            }
            // get how many loading term is listed in contract 
            //var invoice_type = dbContext.master_list
            //.Where(o => o.id == real_invoice_type_id).FirstOrDefault();
            if (invoice_type_data.item_in_coding.Substring(0, 2) != "dp")
            {
                var delivery_term = dbContext.master_list
                .Where(o => o.id == record_despatch_order.delivery_term_id).FirstOrDefault();
                if (delivery_term.item_in_coding == "FOBBG")
                {
                    if (record_despatch_order.multiple_barge == true)
                    {
                        var record_shipping_transaction = dbContext.vw_shipping_transaction
                           .Where(o => o.despatch_order_id == despatch_order_id && o.is_loading == true).FirstOrDefault();
                        if (record_shipping_transaction == null)
                        {
                            return BadRequest("There is no Shipping Loading (FOB Barge (Multiple)) Data in this Shipping Order");

                        }
                    }
                    else
                    {
                        var record_barging_transaction = dbContext.vw_barging_transaction
                       .Where(o => o.despatch_order_id == despatch_order_id && o.is_loading == true).FirstOrDefault();
                        if (record_barging_transaction == null)
                        {
                            return BadRequest("There is no Barging Loading (FOB Barge) Data in this Shipping Order");
                        }
                    }
                }
                else if (delivery_term.item_in_coding == "CIFBG")
                {
                    var record_barging_transaction = dbContext.vw_barging_transaction
                   .Where(o => o.despatch_order_id == despatch_order_id && o.is_loading == false).FirstOrDefault();
                    if (record_barging_transaction == null)
                    {
                        return BadRequest("There is no Barging Unloading (CIF Barge) Data in this Shipping Order");
                    }
                }
                else if (delivery_term.item_in_coding == "FOBMV")
                {
                    var record_shipping_transaction = dbContext.vw_shipping_transaction
                   .Where(o => o.despatch_order_id == despatch_order_id && o.is_loading == true).FirstOrDefault();
                    if (record_shipping_transaction == null)
                    {
                        return BadRequest("There is no Shipping Loading (FOB Vessel) Data in this Shipping Order");
                    }
                }

                else if (delivery_term.item_in_coding == "CIFMV")
                {
                    var record_shipping_transaction = dbContext.vw_shipping_transaction
                   .Where(o => o.despatch_order_id == despatch_order_id && o.is_loading == false).FirstOrDefault();
                    if (record_shipping_transaction == null)
                    {
                        return BadRequest("There is no Shipping Unloading (CIF Vessel) Data in this Shipping Order");
                    }
                }
                //else 
                //{
                //    var record_shipping_transaction = dbContext.vw_shipping_transaction
                //   .Where(o => o.despatch_order_id == despatch_order_id && o.is_loading == true).FirstOrDefault();
                //    if (record_shipping_transaction == null)
                //    {
                //        return BadRequest("There is no Shipping Data (FAS) in this Shipping Order");
                //    }
                //}
            }
            // get how many invoice already generated by despatch_order_id

            laycan_date = (System.DateTime)(record_despatch_order.laycan_start ?? Convert.ToDateTime("1900-01-01"));
            //Bobby 20220121 change quantity source
            decimal doQuantity = 0;
            //Bobby 20220405 change quantity source option
            if (invoice_type_data.item_name.ToLower().Contains("dp") || invoice_type_data.item_name.ToLower().Contains("proforma"))
            {
                doQuantity = record_despatch_order.required_quantity ?? 0;
                //Bobby 20220405 change bill_of_lading source
                bill_of_lading = (System.DateTime)(record_despatch_order.order_reference_date ?? Convert.ToDateTime("1900-01-01"));
            }
            else
            {
                //Bobby 20221103 commercial COW Flaging
                var cow = await dbContext.draft_survey.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.despatch_order_id == despatch_order_id && o.non_commercial != true).FirstOrDefaultAsync();
                if (cow != null)
                {
                    doQuantity = cow.quantity ?? 0;
                    //Bobby 20220218 change bill_of_lading source
                    bill_of_lading = (System.DateTime)cow.bill_lading_date;
                }
            }
            //var doQuantity = record_despatch_order.quantity;
            string the_contract_term_id = record_despatch_order.contract_term_id;
            string the_contract_product_id = record_despatch_order.contract_product_id;
            var v_vessel_id = record_despatch_order.vessel_id;


            // Find Quotation Type using Invoice Type Name
            //var masterlist_id_for_quotation_type = await dbContext.master_list
            //    .Where(o => o.item_name == quotation_type_data.item_name && o.item_group == "quotation-type").FirstOrDefaultAsync()
            //    ;

            var records_sales_contract_quotation_price = dbContext.vw_sales_contract_quotation_price
                .Where(o => o.sales_contract_term_id == the_contract_term_id
                && o.quotation_type_id == quotation_type_id)
                ;
            var arraySalesContractQuotationPrice = await records_sales_contract_quotation_price.ToArrayAsync();

            try
            {
                // menghitung data quotation price
                t_quotation_price = 0;
                t_current_quotation_price = 0;
                int countPrice = 0;
                int indexPrice = 0;
                var total_weightening_value = 0;
                string quotationTypeName = "";
                decimal highestPrice = 0;

                foreach (vw_sales_contract_quotation_price item1 in arraySalesContractQuotationPrice)
                {
                    countPrice = arraySalesContractQuotationPrice.Length;
                    indexPrice += 1;
                    total_weightening_value += (int)item1.weightening_value;
                    quotationTypeName = item1.quotation_type_name;

                    if (item1.pricing_method_in_coding == stringPricingMethod_Calculated)
                    {
                        if (item1.price_index_id is null)
                        {
                            t_quotation_price = 0;
                        }
                        else
                        {
                            // check quotation_period,  if its 4LIBL , 4LI1LD , QBQBL , QBQLD
                            decimal avg_price_index = 0;
                            decimal total_price_index = 0;
                            int count_price_include = 0;
                            int[] arrayMonth = new int[1];
                            List<int> listMonth = new List<int>();
                            var theDate = new System.DateTime(2020, 3, 30);
                            var errMsg = "";
                            switch (item1.quotation_period)
                            {
                                case "2WAPBLDE":
                                case "2WAPBLDI":
                                case "2WAPFLDE":
                                case "2WAPFLDI":
                                case "3WAPBLDI":
                                case "3WAPFLDE":
                                case "4WAPBLDE":
                                case "4WAPFLDI":
                                    string quotationPeriod = item1.quotation_period;
                                    int lastDaysCount = 0;

                                    if (quotationPeriod == "2WAPBLDE" || quotationPeriod == "2WAPBLDI")
                                    {
                                        theDate = bill_of_lading;
                                        lastDaysCount = -14;
                                    }
                                    else if (quotationPeriod == "2WAPFLDE" || quotationPeriod == "2WAPFLDI")
                                    {
                                        theDate = laycan_date;
                                        lastDaysCount = -14;
                                    }
                                    else if (quotationPeriod == "3WAPBLDI")
                                    {
                                        theDate = bill_of_lading;
                                        lastDaysCount = -21;
                                    }
                                    else if (quotationPeriod == "3WAPFLDE")
                                    {
                                        theDate = laycan_date;
                                        lastDaysCount = -21;
                                    }
                                    else if (quotationPeriod == "4WAPBLDE")
                                    {
                                        theDate = bill_of_lading;
                                        lastDaysCount = -28;
                                    }
                                    else if (quotationPeriod == "4WAPFLDI")
                                    {
                                        theDate = laycan_date;
                                        lastDaysCount = -28;
                                    }

                                    var rec1 = dbContext.vw_price_index_history
                                        .Where(o => o.is_forecast != true)
                                        .Where(o => o.price_index_id == item1.price_index_id
                                            && o.index_date.Value > theDate.AddDays(lastDaysCount));

                                    if (quotationPeriod == "2WAPBLDE" || quotationPeriod == "2WAPFLDE" || quotationPeriod == "3WAPFLDE" || quotationPeriod == "4WAPBLDE")
                                        rec1 = rec1.Where(o => o.index_date.Value < theDate);
                                    else
                                        rec1 = rec1.Where(o => o.index_date.Value <= theDate);

                                    rec1 = rec1.OrderByDescending(o => o.index_date);

                                    foreach (vw_price_index_history item in rec1)
                                    {
                                        total_price_index += (decimal)item.index_value;
                                        count_price_include++;
                                    }

                                    avg_price_index = (decimal)(total_price_index / count_price_include);
                                    t_quotation_price += avg_price_index;
                                    t_current_quotation_price = t_quotation_price;

                                    if (quotationTypeName == "Highest")
                                    {
                                        if (highestPrice <= t_quotation_price)
                                        {
                                            highestPrice = t_quotation_price;
                                        }
                                        else
                                        {
                                            t_quotation_price = highestPrice;
                                            t_current_quotation_price = t_quotation_price;
                                        }
                                    }
                                    break;

                                case stringQuotationPeriod_4LIBL:
                                case stringQuotationPeriod_4LI1LD:

                                    if (item1.quotation_period == stringQuotationPeriod_4LIBL)
                                    {
                                        theDate = bill_of_lading;
                                    }
                                    else
                                    {
                                        theDate = laycan_date;
                                    }
                                    string datename = theDate.ToString("dddd");
                                    int dayRange = 28;
                                    int recTake = 4;
                                    int minIndex = 4;
                                    if (datename == "Friday")
                                    {
                                        if (item1.index_calculation_code == stringIndexCalc_EF4)
                                        {
                                            dayRange = 27;
                                            recTake = 4;
                                            minIndex = 4;
                                        }
                                        else if (item1.index_calculation_code == stringIndexCalc_EF3)
                                        {
                                            dayRange = 27;
                                            recTake = 3;
                                            minIndex = 3;
                                        }
                                        else
                                        {
                                            dayRange = 28;
                                            recTake = 4;
                                            minIndex = 4;
                                        }
                                    }
                                    var records_price_index_history = dbContext.vw_price_index_history
                                        .Where(o => o.is_forecast != true)
                                        .Where(o => o.price_index_id == item1.price_index_id
                                            && o.index_date.Value.AddDays(28) >= theDate
                                            && o.index_date.Value.AddDays(28) <= theDate.AddDays(dayRange))
                                        .OrderByDescending(o => o.index_date)
                                        .Take(recTake);
                                    if (records_price_index_history.Count() < minIndex || records_price_index_history.Count() > 5)
                                    {
                                        errMsg = "Price is not available. Number of index price from " + records_price_index_history.FirstOrDefault().price_index_name + " in 28-days-range from " + theDate.ToString() + " is not " + minIndex.ToString();
                                        avg_price_index = 0;
                                        t_quotation_price = 0;
                                        t_current_quotation_price = 0;
                                        break;
                                    }
                                    var array_price_index_history = await records_price_index_history.ToArrayAsync();
                                    foreach (vw_price_index_history item2 in array_price_index_history)
                                    {
                                        if ((theDate <= item2.index_date.Value.AddDays(28)) && (count_price_include < 4))
                                        {
                                            total_price_index += (decimal)item2.index_value;
                                            count_price_include++;
                                        }
                                    }
                                    if (count_price_include != 4)
                                    {
                                        if (datename != "Friday")
                                        {
                                            errMsg = "Price is not available. Number of index price in 28-days-range from " + theDate.ToString() + " is not 4";
                                            avg_price_index = 0;
                                            t_quotation_price = 0;
                                            t_current_quotation_price = 0;
                                            break;
                                        }
                                    }
                                    avg_price_index = (decimal)(total_price_index / count_price_include);
                                    t_quotation_price += avg_price_index * (decimal)item1.weightening_value / 100;
                                    t_current_quotation_price = t_quotation_price;

                                    if (quotationTypeName == "Highest")
                                    {
                                        if (highestPrice <= t_quotation_price)
                                        {
                                            highestPrice = t_quotation_price;
                                        }
                                        else
                                        {
                                            t_quotation_price = highestPrice;
                                            t_current_quotation_price = t_quotation_price;
                                        }
                                    }
                                    break;

                                case stringQuotationPeriod_QBQBL:
                                case stringQuotationPeriod_QBQLD:
                                    if (item1.quotation_period == stringQuotationPeriod_QBQBL)
                                    {
                                        theDate = bill_of_lading;
                                    }
                                    else
                                    {
                                        theDate = laycan_date;
                                    }
                                    var records_price_index_history_quarter = dbContext.price_index_history
                                        .Where(o => o.price_index_id == item1.price_index_id)
                                        .Where(o => o.is_forecast != true)
                                        .OrderByDescending(o => o.index_date);

                                    var array_price_index_history_quarter = await records_price_index_history_quarter.ToArrayAsync();
                                    switch (theDate.Month)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                            listMonth.Add(10);
                                            listMonth.Add(11);
                                            listMonth.Add(12);
                                            break;
                                        case 4:
                                        case 5:
                                        case 6:
                                            listMonth.Add(1);
                                            listMonth.Add(2);
                                            listMonth.Add(3);
                                            break;
                                        case 7:
                                        case 8:
                                        case 9:
                                            listMonth.Add(4);
                                            listMonth.Add(5);
                                            listMonth.Add(6);
                                            break;
                                        case 10:
                                        case 11:
                                        case 12:
                                            listMonth.Add(7);
                                            listMonth.Add(8);
                                            listMonth.Add(9);
                                            break;
                                        default:
                                            listMonth.Add(0);
                                            break;
                                    }
                                    arrayMonth = listMonth.ToArray();
                                    foreach (price_index_history item2 in array_price_index_history_quarter)
                                    {
                                        if ((item2.index_date.Year == theDate.Year) && (arrayMonth.Contains(item2.index_date.Month)))
                                        {
                                            total_price_index += (decimal)item2.index_value;
                                            count_price_include++;
                                        }
                                    }
                                    avg_price_index = total_price_index / count_price_include;
                                    t_quotation_price += avg_price_index * (decimal)item1.weightening_value / 100;
                                    t_current_quotation_price = t_quotation_price;

                                    if (quotationTypeName == "Highest")
                                    {
                                        if (highestPrice <= t_quotation_price)
                                        {
                                            highestPrice = t_quotation_price;
                                        }
                                        else
                                        {
                                            t_quotation_price = highestPrice;
                                            t_current_quotation_price = t_quotation_price;
                                        }
                                    }
                                    break;

                                //Bobby Price HPB 20220520

                                case stringQuotationPeriod_MILD:
                                case stringQuotationPeriod_MIBLD:

                                    if (item1.quotation_period == stringQuotationPeriod_MIBLD)
                                    {
                                        theDate = bill_of_lading;
                                    }
                                    else
                                    {
                                        theDate = laycan_date;
                                    }
                                    var records_monthly_index_history = dbContext.vw_price_index_history
                                        .Where(o => o.price_index_id == item1.price_index_id
                                            && o.index_date.Value.Month == theDate.Month
                                            && o.index_date.Value.Year == theDate.Year)
                                        .OrderByDescending(o => o.index_date);
                                    if (records_monthly_index_history.Count() > 1)
                                    {
                                        //errMsg = "There is multiple index";
                                        //avg_price_index = 0;
                                        //t_quotation_price = 0;
                                        //t_current_quotation_price = 0;
                                        //break;
                                        var found = false;
                                        var listIndexHistory = new List<vw_price_index_history>();
                                        foreach (var perIndex in await records_monthly_index_history.ToArrayAsync())
                                        {
                                            if (theDate.Date == perIndex.index_date.Value.Date)
                                            {
                                                records_monthly_index_history = dbContext.vw_price_index_history
                                                    .Where(x => x.id == perIndex.id)
                                                    .OrderByDescending(x => x.index_date);
                                                found = true;
                                                break;
                                            }
                                            listIndexHistory.Add(perIndex);
                                        }
                                        if (!found)
                                        {
                                            listIndexHistory = listIndexHistory.OrderByDescending(x => x.index_date.Value.Date).ToList();
                                            var result = listIndexHistory.Where(x => x.index_date.Value.Date <= theDate.Date)
                                                                         .FirstOrDefault();

                                            records_monthly_index_history = dbContext.vw_price_index_history
                                                .Where(x => x.id == result.id)
                                                .Where(x => x.is_forecast != true)
                                                .OrderByDescending(x => x.index_date);
                                        }
                                    }
                                    var array_monthly_index_history = await records_monthly_index_history.ToArrayAsync();
                                    foreach (vw_price_index_history item2 in array_monthly_index_history)
                                    {
                                        // if ((theDate <= item2.index_date.Value.AddDays(28)) && (count_price_include < 4))
                                        // {
                                        total_price_index += (decimal)item2.index_value;
                                        count_price_include++;
                                        // }
                                    }
                                    if (count_price_include != 1)
                                    {
                                        errMsg = "Monthly Index is not available";
                                        avg_price_index = 0;
                                        t_quotation_price = 0;
                                        t_current_quotation_price = 0;
                                        break;
                                    }
                                    avg_price_index = total_price_index / count_price_include;
                                    t_quotation_price += avg_price_index * (decimal)item1.weightening_value / 100;
                                    t_current_quotation_price = t_quotation_price;

                                    if (quotationTypeName == "Highest")
                                    {
                                        if (highestPrice <= t_quotation_price)
                                        {
                                            highestPrice = t_quotation_price;
                                        }
                                        else
                                        {
                                            t_quotation_price = highestPrice;
                                            t_current_quotation_price = t_quotation_price;
                                        }
                                    }
                                    break;

                                //Bobby Price HPB 20220520

                                case stringQuotationPeriod_APMILD:
                                case stringQuotationPeriod_APMIBL:
                                    if (item1.quotation_period == stringQuotationPeriod_APMIBL)
                                    {
                                        theDate = bill_of_lading;
                                    }
                                    else
                                    {
                                        theDate = laycan_date;
                                    }
                                    theDate = theDate.AddMonths(-1);
                                    var records_monthly_index_history_apmi = dbContext.vw_price_index_history
                                        .Where(o => o.price_index_id == item1.price_index_id
                                            && o.index_date.Value.Month == theDate.Month
                                            && o.index_date.Value.Year == theDate.Year)
                                        .OrderByDescending(o => o.index_date);
                                    if (records_monthly_index_history_apmi.Count() < 1)
                                    {
                                        errMsg = "No index";
                                        avg_price_index = 0;
                                        t_quotation_price = 0;
                                        t_current_quotation_price = 0;
                                        break;
                                    }
                                    var array_monthly_index_history_apmi = await records_monthly_index_history_apmi.ToArrayAsync();
                                    foreach (vw_price_index_history item2 in array_monthly_index_history_apmi)
                                    {
                                        // if ((theDate <= item2.index_date.Value.AddDays(28)) && (count_price_include < 4))
                                        // {
                                        total_price_index += (decimal)item2.index_value;
                                        count_price_include++;
                                        // }
                                    }
                                    if (count_price_include < 1)
                                    {
                                        errMsg = "Monthly Index is not available";
                                        avg_price_index = 0;
                                        t_quotation_price = 0;
                                        t_current_quotation_price = 0;
                                        break;
                                    }
                                    avg_price_index = total_price_index / count_price_include;
                                    t_quotation_price += avg_price_index * (decimal)item1.weightening_value / 100;
                                    t_current_quotation_price = t_quotation_price;

                                    if (quotationTypeName == "Highest")
                                    {
                                        if (highestPrice <= t_quotation_price)
                                        {
                                            highestPrice = t_quotation_price;
                                        }
                                        else
                                        {
                                            t_quotation_price = highestPrice;
                                            t_current_quotation_price = t_quotation_price;
                                        }
                                    }
                                    break;
                                default: break;
                            } // end of switch:
                            if (errMsg.Length > 0)
                            {
                                arrayRetVal.Add(new SalesPrice("error", errMsg, -1));
                                //return arrayRetVal;
                                return BadRequest(errMsg);
                            }
                            //t_quotation_price = Math.Round(t_quotation_price, (int)item1.decimal_places, MidpointRounding.AwayFromZero);
                            if (indexPrice == countPrice)
                            {
                                t_quotation_price = Math.Round(t_quotation_price, (int)item1.decimal_places, MidpointRounding.AwayFromZero);
                            }
                            t_current_quotation_price = t_quotation_price;
                        } // end of else:  price_index_id is not null

                    }
                    else if (item1.pricing_method_in_coding == stringPricingMethod_Fixed)
                    {
                        t_quotation_price += (decimal)item1.price_value * (decimal)item1.weightening_value / 100;
                        t_current_quotation_price = t_quotation_price;
                    }

                }
            }
            catch (Exception e)
            {
                logger.Debug($"got exception when count quotationprice = {e.Message}");
                return BadRequest("Error when counting quotationprice = " + e.Message);
            }

            try
            {
                // menghitung sales charge jika Premium atau adjustment 
                var records_vw_sales_contract_charge = dbContext.vw_sales_contract_charges
                    .Where(o => o.sales_contract_term_id == the_contract_term_id)
                    .OrderBy(o => o.order);

                var array_vw_contract_charges = await records_vw_sales_contract_charge.ToArrayAsync();
                foreach (vw_sales_contract_charges item_contract_charge in array_vw_contract_charges)
                {
                    //bobby arvian 20220407 penambahan feature discount
                    if (item_contract_charge.sales_charge_type_name.ToLower() == stringsalesChargeType_discount)
                    {
                        string the_text = item_contract_charge.charge_formula;
                        //string the_prerequisite = item_contract_charge.prerequisite;
                        //bool isPrerequisite = patternPrerequisitePremium(the_prerequisite, v_vessel_id);
                        //if (isPrerequisite)
                        //{
                        t_quotation_price = patternFormulaPremium(the_text, t_quotation_price);
                        //Bobby 20220425 Rounding
                        if (item_contract_charge.rounding_type_name == "Round Up")
                        {
                            t_quotation_price = Math.Round(t_quotation_price, (int)item_contract_charge.decimal_places, MidpointRounding.ToEven);
                        }
                        else if (item_contract_charge.rounding_type_name == "Floor")
                        {
                            t_quotation_price = Math.Floor(t_quotation_price);
                        }
                        else if (item_contract_charge.rounding_type_name == "Truncate")
                        {
                            t_quotation_price = Math.Truncate(t_quotation_price);
                        }
                        else
                        {
                            t_quotation_price = Math.Round(t_quotation_price, (int)item_contract_charge.decimal_places, MidpointRounding.AwayFromZero);
                        }
                        t_current_quotation_price = t_quotation_price;
                        //}
                    }
                    if (item_contract_charge.sales_charge_type_name.ToLower() == stringsalesChargeType_premium)
                    {
                        string the_text = item_contract_charge.charge_formula;
                        string the_prerequisite = item_contract_charge.prerequisite;
                        if (the_prerequisite == null || the_prerequisite == "")
                        {
                            t_quotation_price = patternFormulaPremium(the_text, t_quotation_price);
                        }
                        else
                        {
                            bool isPrerequisite = patternPrerequisitePremium(the_prerequisite, v_vessel_id);
                            if (isPrerequisite)
                            {
                                t_quotation_price = patternFormulaPremium(the_text, t_quotation_price);
                            }
                        }
                        //Bobby 20220425 Rounding
                        if (item_contract_charge.rounding_type_name == "Round Up")
                        {
                            t_quotation_price = Math.Round(t_quotation_price, (int)item_contract_charge.decimal_places, MidpointRounding.ToEven);
                        }
                        else if (item_contract_charge.rounding_type_name == "Floor")
                        {
                            t_quotation_price = Math.Floor(t_quotation_price);
                        }
                        else if (item_contract_charge.rounding_type_name == "Truncate")
                        {
                            t_quotation_price = Math.Truncate(t_quotation_price);
                        }
                        else
                        {
                            t_quotation_price = Math.Round(t_quotation_price, (int)item_contract_charge.decimal_places, MidpointRounding.AwayFromZero);
                        }
                        t_current_quotation_price = t_quotation_price;
                    }

                    if (item_contract_charge.sales_charge_type_name.ToLower() == stringsalesChargeType_adjustment)
                    {
                        //var sementara = 0.0;
                        if (string.IsNullOrEmpty(item_contract_charge.charge_formula))
                            return BadRequest("Sales Charge Formula is empty.");

                        var tText = item_contract_charge.charge_formula.ToLower().Replace(" ", String.Empty);
                        var tName = item_contract_charge.sales_charge_name;
                        var tCode = item_contract_charge.sales_charge_code;
                        List<AdjustmentCalculation> adjCalc = new List<AdjustmentCalculation>();

                        AdjustmentCalculation ac1;
                        AdjustmentCalculation ac2;
                        //var valuenya = "";

                        //Bobby 20221103 commercial COA Flaging
                        var records_sales_contract_product_specification = dbContext.vw_quality_sampling_analyte
                            .Where(o => o.despatch_order_id == despatch_order_id && o.non_commercial != true);
                        var array_sales_contract_product_specification = await records_sales_contract_product_specification.ToArrayAsync();

                        //var records_sales_contract_product_specification = dbContext.vw_sales_contract_product_specifications
                        //    .Where(o => o.sales_contract_product_id == the_contract_product_id);
                        //var array_sales_contract_product_specification = await records_sales_contract_product_specification.ToArrayAsync();
                        //foreach (vw_sales_contract_product_specifications item1 in array_sales_contract_product_specification)

                        foreach (vw_quality_sampling_analyte item1 in array_sales_contract_product_specification)
                        {

                            var analytesym = item1.analyte_symbol.Replace(" ", String.Empty).ToLower();
                            if (tText.Contains(analytesym))
                            {
                                //Bobby 20220121 change quality source
                                //despatch_order_id
                                ac1 = new AdjustmentCalculation();
                                //ac1.analyte_maximum = (float)(item1.maximum ?? 0);
                                //ac1.analyte_minimum = (float)(item1.minimum ?? 0);
                                //ac1.analyte_target = (float)(item1.target ?? 0);
                                //ac1.analyte_value = (float)item1.value;
                                //20220425 bobby take out float
                                ac1.analyte_maximum = (decimal)(item1.maximum ?? 0);
                                ac1.analyte_minimum = (decimal)(item1.minimum ?? 0);
                                ac1.analyte_target = (decimal)(item1.target ?? 0);
                                ac1.analyte_value = (decimal)item1.analyte_value;
                                //ac1.analyte_value = (float)item1.analyte_value;
                                ac1.analyte_name = item1.analyte_name;
                                ac1.analyte_symbol = analytesym;
                                adjCalc.Add(ac1);

                                ac1 = new AdjustmentCalculation();
                                //20220425 bobby take out float
                                ac1.analyte_value = t_quotation_price;
                                //stringCurrentPrice
                                //stringQuotationPrice
                                ac1.analyte_name = stringQuotationPrice;
                                ac1.analyte_symbol = stringQuotationPrice;
                                adjCalc.Add(ac1);

                                ac2 = new AdjustmentCalculation();
                                //20220425 bobby take out float
                                ac2.analyte_value = t_current_quotation_price;
                                //stringCurrentPrice
                                //stringQuotationPrice
                                ac2.analyte_name = stringCurrentPrice;
                                ac2.analyte_symbol = stringCurrentPrice;
                                adjCalc.Add(ac2);
                                /*decimal hasilParsing = patternAdjustment(tText, adjCalc);
                                if (item_contract_charge.decimal_places == null)
                                {
                                    item_contract_charge.decimal_places = 2;
                                }
                                //Bobby 20220425 Rounding
                                if (item_contract_charge.rounding_type_name == "Round Up")
                                {
                                    hasilParsing = Math.Round(hasilParsing, (int)item_contract_charge.decimal_places, MidpointRounding.ToEven);
                                }
                                else if (item_contract_charge.rounding_type_name == "Floor")
                                {
                                    hasilParsing = Math.Floor(hasilParsing);
                                }
                                else if (item_contract_charge.rounding_type_name == "Truncate")
                                {
                                    hasilParsing = Math.Truncate(hasilParsing);
                                }
                                else
                                {
                                    hasilParsing = Math.Round(hasilParsing, (int)item_contract_charge.decimal_places, MidpointRounding.AwayFromZero);
                                }
                                arrayRetVal.Add(new SalesPrice(tName, tCode, hasilParsing));
                                t_current_quotation_price += hasilParsing;*/
                            }
                        }
                        if (adjCalc.Count > 0)
                        {
                            decimal hasilParsing = patternAdjustment(tText, adjCalc);

                            if (item_contract_charge.decimal_places == null)
                            {
                                item_contract_charge.decimal_places = 2;
                            }
                            //Bobby 20220425 Rounding
                            if (item_contract_charge.rounding_type_name == "Round Up")
                            {
                                hasilParsing = Math.Ceiling(hasilParsing);
                                //hasilParsing = Math.Round(hasilParsing, (int)item_contract_charge.decimal_places, MidpointRounding.AwayFromZero);
                            }
                            else if (item_contract_charge.rounding_type_name == "Floor")
                            {
                                hasilParsing = Math.Floor(hasilParsing);
                            }
                            else if (item_contract_charge.rounding_type_name == "Truncate")
                            {
                                hasilParsing = Math.Truncate(hasilParsing);
                            }
                            else
                            {
                                hasilParsing = Math.Round(hasilParsing, (int)item_contract_charge.decimal_places, MidpointRounding.AwayFromZero);
                            }
                            arrayRetVal.Add(new SalesPrice(tName, tCode, hasilParsing));
                            t_current_quotation_price += hasilParsing;

                        }
                    }

                }

            }
            catch (Exception e)
            {
                logger.Debug($"got exception when parsing prerequisite = {e.Message}");
                return BadRequest("Error when parsing prerequisite = " + e.Message);
            }

            var shippingCost = dbContext.vw_shipping_cost
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                    o.despatch_order_id == despatch_order_id).FirstOrDefault();



            arrayRetVal.Add(new SalesPrice("Quotation Price", "quotation_price", t_quotation_price));
            decimal totalInvoice = 0;
            try
            {
                // menghitung down payment
                decimal dpValue = 0;
                totalInvoice = getTotalInvoice(arrayRetVal, doQuantity);
                //bobby 20020422 Freight Cost
                arrayRetVal.Add(new SalesPrice("subtotal", "subtotal", totalInvoice));

                //hide shipping cost calculation
                //if(shippingCost!=null)
                //{
                //    totalInvoice += ((double)shippingCost.freight_rate * (double)shippingCost.quantity);//(double)doQuantity);
                //    totalInvoice += (double)shippingCost.insurance_cost;
                //}

                //bobby 20220425 rounding total
                var lookupSalesContractTerm = await dbContext.vw_sales_contract_term.Where(o => o.id == the_contract_term_id).FirstOrDefaultAsync();
                if (lookupSalesContractTerm != null)
                {
                    if (lookupSalesContractTerm.decimal_places == null)
                    {
                        lookupSalesContractTerm.decimal_places = 2;
                    }
                    if (lookupSalesContractTerm.rounding_type_name == "Round Up")
                    {
                        totalInvoice = Math.Round(totalInvoice, (int)lookupSalesContractTerm.decimal_places, MidpointRounding.ToEven);
                    }
                    else if (lookupSalesContractTerm.rounding_type_name == "Floor")
                    {
                        totalInvoice = Math.Floor(totalInvoice);
                    }
                    else if (lookupSalesContractTerm.rounding_type_name == "Truncate")
                    {
                        totalInvoice = Math.Truncate(totalInvoice);
                    }
                    else
                    {
                        totalInvoice = Math.Round(totalInvoice, (int)lookupSalesContractTerm.decimal_places, MidpointRounding.AwayFromZero);
                    }
                }
                //bobby 20220425 rounding total

                arrayRetVal.Add(new SalesPrice("Total Price", "total_price", totalInvoice));
                if (numberInvoicesCanBeIssued == 1)
                {
                    // last invoice 

                    decimal totalDPfromAllInvoice = 0;
                    foreach (vw_sales_invoice item1 in array_vw_sales_invoice_data)
                    {
                        totalDPfromAllInvoice += (decimal)item1.downpayment;
                    }

                    dpValue = totalInvoice - totalDPfromAllInvoice;

                }
                else
                {
                    // 
                    var outLoop = false;

                    foreach (vw_sales_contract_payment_term item1 in array_vw_payment_term_data)
                    {
                        if (item1.id == sales_contract_payment_term_id)
                        {
                            switch (item1.invoice_in_coding)
                            {
                                case "general":
                                    dpValue = 0;
                                    break;
                                case "dp_fixed_amount":
                                    if (item1.downpayment_value != null)
                                    {
                                        dpValue = (decimal)item1.downpayment_value;
                                    }
                                    outLoop = true;
                                    break;
                                case "dp_percentage":
                                    if (item1.downpayment_value != null)
                                    {
                                        // total = (based price + adjustment ) * quantity  
                                        // dpValue = total * percentage 
                                        dpValue = totalInvoice * (decimal)(item1.downpayment_value / 100);
                                    }
                                    outLoop = true;
                                    break;
                                case "TERMIN_PERCENTAGE":
                                    if (item1.downpayment_value != null)
                                    {
                                        dpValue = totalInvoice * (decimal)(item1.downpayment_value / 100);
                                    }
                                    outLoop = true;
                                    break;
                                default: break;
                            }
                        }
                        if (outLoop)
                        {
                            break;
                        }
                    }

                }
                //BObby 20220826 Parent DO
                decimal totalparentInvoice;
                decimal DPInvoiceParent;
                decimal amount_pershipment;
                var record_despatch_order_parent = await dbContext.despatch_order
                        .Where(o => o.id == record_despatch_order.parent_despatch_order_id)
                        .FirstOrDefaultAsync();
                if (record_despatch_order_parent != null)
                {
                    // get how many invoice already generated by despatch_order_id
                    var vw_sales_invoice_data_parent = await dbContext.vw_sales_invoice
                        .Where(o => o.despatch_order_id == record_despatch_order.parent_despatch_order_id)
                        .FirstOrDefaultAsync();
                    totalparentInvoice = (decimal)vw_sales_invoice_data_parent.total_price;
                    DPInvoiceParent = (decimal)vw_sales_invoice_data_parent.downpayment;
                    var vw_parent_despatch_order = await dbContext.vw_parent_despatch_order
                       .Where(o => o.despatch_order_id == record_despatch_order.parent_despatch_order_id)
                       .FirstOrDefaultAsync();
                    amount_pershipment = (decimal)vw_parent_despatch_order.down_payment_amount;
                    dpValue = amount_pershipment;
                }
                //BObby 20220826 Parent DO
                arrayRetVal.Add(new SalesPrice("Down Payment", "down_payment", dpValue));

            }
            catch (Exception e)
            {
                logger.Debug($"got exception when processing Down Payment = {e.Message}");
                return BadRequest("Error when processing Down Payment = " + e.Message);
            }

            // get credit limit data
            var vw_sales_contract_data = dbContext.vw_sales_contract
                .Where(o => o.id == sales_contract_id);

            var array_vw_sales_contract_data = await vw_sales_contract_data.ToArrayAsync();
            if (array_vw_sales_contract_data.Length == 0)
            {
                return BadRequest("No defined customer in " + array_vw_sales_contract_data[0].sales_contract_name);
            }

            // find all sales contract within the same customer
            globalCustomerId = array_vw_sales_contract_data[0].customer_id;
            var vw_sales_invoice_custid = dbContext.vw_sales_invoice
                .Where(o => o.customer_id == globalCustomerId);

            var array_vw_sales_invoice_custid = await vw_sales_invoice_custid.ToArrayAsync();
            decimal totalPaymentfromAllInvoice = 0;
            decimal totalPricefromAllInvoice = 0;
            foreach (vw_sales_invoice item1 in array_vw_sales_invoice_custid)
            {
                totalPricefromAllInvoice += (decimal)(item1.total_price ?? 0);

                var vw_sales_invoice_payment_data = dbContext.vw_sales_invoice_payment.Where(o => o.sales_invoice_number == item1.invoice_number);
                var array_vw_sales_invoice_payment_data = await vw_sales_invoice_payment_data.ToArrayAsync();
                foreach (vw_sales_invoice_payment itemX in array_vw_sales_invoice_payment_data)
                {
                    totalPaymentfromAllInvoice += (decimal)(itemX.payment_value ?? 0);
                }
            }

            decimal remainedCreditLimit = 0;
            decimal customerCreditLimit = 0;

            List<CreditLimitData> varCreditLimitData = (List<CreditLimitData>)CountCreditLimit(array_vw_sales_contract_data[0].customer_id).Result;
            //remainedCreditLimit = varCreditLimitData[0].RemainedCreditLimit;
            //customerCreditLimit = varCreditLimitData[0].InitialCreditLimit;

            if (varCreditLimitData.Count > 0)
            {
                remainedCreditLimit = varCreditLimitData[0].RemainedCreditLimit;
                customerCreditLimit = varCreditLimitData[0].InitialCreditLimit;
            }

            remainedCreditLimit = customerCreditLimit - totalInvoice - totalPricefromAllInvoice + totalPaymentfromAllInvoice;
            arrayRetVal.Add(new SalesPrice("Credit Limit", "credit_limit", remainedCreditLimit));
            globalRemainedCreditLimit = remainedCreditLimit;

            return arrayRetVal;
        }

        private async Task<object> CountCreditLimit(string customer_id)
        {
            List<CreditLimitData> retval = new List<CreditLimitData>();
            decimal remainedCreditLimit = 0;
            decimal customerCreditLimit = 0;
            try
            {
                var customer_data = dbContext.customer
                    .Where(o => o.id == customer_id)
                    .Select(o => o.credit_limit)
                    ;
                foreach (decimal v in customer_data)
                {
                    customerCreditLimit += (decimal)v;
                }

                // find all sales contract within the same customer
                var vw_sales_invoice_custid = dbContext.vw_sales_invoice
                    .Where(o => o.customer_id == customer_id)
                    ;
                var array_vw_sales_invoice_custid = await vw_sales_invoice_custid.ToArrayAsync();
                decimal totalPaymentfromAllInvoice = 0;

                decimal totalPricefromAllInvoice = 0;
                if (array_vw_sales_invoice_custid.Length > 0)
                {
                    foreach (vw_sales_invoice item1 in array_vw_sales_invoice_custid)
                    {
                        if (item1.total_price != null)
                        {
                            totalPricefromAllInvoice += (decimal)item1.total_price;
                        }
                        var vw_sales_invoice_payment_data = dbContext.vw_sales_invoice_payment.Where(o => o.sales_invoice_number == item1.invoice_number);
                        var array_vw_sales_invoice_payment_data = await vw_sales_invoice_payment_data.ToArrayAsync();
                        if (array_vw_sales_invoice_payment_data.Length > 0)
                        {
                            foreach (vw_sales_invoice_payment itemX in array_vw_sales_invoice_payment_data)
                            {
                                if (itemX.payment_value != null)
                                {
                                    totalPaymentfromAllInvoice += (decimal)itemX.payment_value;
                                }
                            }
                        }
                    }
                }
                remainedCreditLimit = customerCreditLimit - totalPricefromAllInvoice + totalPaymentfromAllInvoice;
                retval.Add(new CreditLimitData(customerCreditLimit, remainedCreditLimit));
                return retval;

            }
            catch (Exception ex)
            {
                logger.Debug($"exception error message = {ex.Message}");
                return retval;
            }
        }

        decimal getTotalInvoice(List<SalesPrice> arrayRetVal, decimal doQuantity)
        {
            decimal totalPrice = 0;
            foreach (SalesPrice eachSP in arrayRetVal)
            {
                totalPrice += eachSP.price;
            }
            decimal totalInvoice = totalPrice * doQuantity;
            return totalInvoice;
        }

        [HttpGet("SalesInvoiceOutline")]
        public async Task<object> SalesInvoiceOutline(string sales_invoice_id, DataSourceLoadOptions loadOptions)
        {
            //var notestext = "Note :\nPlease pay to (In Full Amount):  \n\nPT. Indexim Coalindo (USD)\t\nPT. Bank Ganesha TBK\t   BANK CORRESPONDENT:\nA/C No. 0910.2.00098.4\t   Bank Negara Indonesia, New York Agency\nWisma Hayam Wuruk Building\nJl. Hayam Wuruk No. 8\t   Swift Code: BNINUS33  \nJakarta 10120 Indonesia\t\t\t\nSwift Code : GNESIDJA\ninstruction to applicant’s bank as per our Central Bank Regulation\nPlease, insert below on the payment message:\n*Transaction Code : 1011\n*Invoice No.\t: XXX/IC-INV/XI/2021\n*Amount\t    \t: USD. ……………\n";
            //var notestext = @"Please pay to (In Full Amount): 
            //                PT. Indexim Coalindo (USD)  
            //                PT. Bank Ganesha TBK A/C No. 0910.2.00098.4   
            //                Swift Code : GNESIDJA, Wisma Hayam Wuruk Building, Jl. Hayam Wuruk No. 8 Jakarta 10120 Indonesia 
            //                BANK CORRESPONDENT: Bank Negara Indonesia, New York Agency Swift Code: BNINUS33

            //                instruction to applicant’s bank as per our Central Bank Regulation
            //                Please, insert below on the payment message:
            //                *Transaction Code : 1011
            //                *Invoice No.    : XXX/IC-INV/XI/2021
            //                *Amount     : USD. ……………";
            var lookupSalesInvoice = await dbContext.vw_sales_invoice.Where(o => o.id == sales_invoice_id).FirstOrDefaultAsync();
            var lookupSalesInvoiceDP = await dbContext.vw_sales_invoice.Where(o => o.id != sales_invoice_id && o.despatch_order_id == lookupSalesInvoice.despatch_order_id && o.invoice_date < lookupSalesInvoice.invoice_date).FirstOrDefaultAsync();
            var lookupDespatchOrder = await dbContext.vw_despatch_order.Where(o => o.id == lookupSalesInvoice.despatch_order_id).FirstOrDefaultAsync();

            decimal? advance_payment = lookupSalesInvoice.alias9 == null ? 0 : lookupSalesInvoice.alias9;
            var notestext = string.Format(@"Please pay to (In Full Amount): 
                            {0}  
                            {1} A/C No. {2}   
                            Swift Code : {3}, {4}
                            BANK CORRESPONDENT: {5}, {6} Swift Code: {7}
                                                      
                            instruction to applicant’s bank as per our Central Bank Regulation
                            Please, insert below on the payment message:
                            *Transaction Code : {8}
                            *Invoice No.    : {9}
                            *Amount     : {10}. ……………", lookupSalesInvoice.account_holder, lookupSalesInvoice.bank_name, lookupSalesInvoice.account_number,
                            lookupSalesInvoice.swift_code, lookupSalesInvoice.branch_information, lookupSalesInvoice.correspondent_bank_name,
                            lookupSalesInvoice.correspondent_branch_information, lookupSalesInvoice.correspondent_swift_code,
                            lookupSalesInvoice.transaction_code, lookupSalesInvoice.invoice_number, lookupSalesInvoice.currency_code);

            double exchange_rate = 1;
            var varFreightCost = lookupSalesInvoice.freight_cost;
            var varInvoiceCurrencySymbol = lookupSalesInvoice.currency_symbol;
            var varInvoiceCurrencyCode = lookupSalesInvoice.currency_code;
            var varInvoiceCurrencyName = lookupSalesInvoice.currency_name;
            if (lookupSalesInvoice.currency_exchange_id != null)
            {
                exchange_rate = (double)lookupSalesInvoice.exchange_rate;
                //*** ini jika diperlukan pembulatan
                //exchange_rate = (double)Math.Round((lookupSalesInvoice.exchange_rate ?? 0), 0, MidpointRounding.AwayFromZero);
                varInvoiceCurrencySymbol = lookupSalesInvoice.target_currency_symbol;
                varInvoiceCurrencyCode = lookupSalesInvoice.target_currency_code;
                varInvoiceCurrencyName = lookupSalesInvoice.target_currency_name;
            }
            var hargaDasar = new InvoiceOutline()
            {
                invoice_type = "export",
                invoice_item = "Harga Dasar",
                invoice_item_type = "Initial Price",
                quantity = 80000,
                adjustment_quantity = 80000,
                price = 55,
                adjustment_price = 56
            };

            hargaDasar.invoice_type = lookupSalesInvoice.sales_type_name ?? "";
            hargaDasar.quantity = (double)lookupSalesInvoice.quantity;
            hargaDasar.adjustment_quantity = (double)lookupSalesInvoice.quantity;
            hargaDasar.price = (double)lookupSalesInvoice.unit_price * exchange_rate;
            hargaDasar.adjustment_price = (double)lookupSalesInvoice.unit_price * exchange_rate;
            hargaDasar.value = hargaDasar.adjustment_price * hargaDasar.adjustment_quantity;
            hargaDasar.total_invoice = hargaDasar.value;

            List<InvoiceOutline> listTaxes = new List<InvoiceOutline>() { };
            List<InvoiceOutline> retVal = new List<InvoiceOutline>()
            {
                hargaDasar
            };


            double xTotalInvoice = hargaDasar.total_invoice;

            //Bobby 20221103 commercial COA Flaging
            var array1 = await dbContext.sales_invoice_charges
                .Where(o => o.sales_invoice_id == sales_invoice_id).ToArrayAsync();
            var tempQualitySamplingAnalyte = dbContext.vw_quality_sampling_analyte.Where(o => o.despatch_order_id == lookupSalesInvoice.despatch_order_id && o.non_commercial != true);


            foreach (sales_invoice_charges arrayX in array1)
            {
                var vAdjustment = new InvoiceOutline()
                {
                    //invoice_type = "exportB",
                    invoice_type = lookupSalesInvoice.sales_type_name ?? "",
                    invoice_item = arrayX.sales_charge_name, //"CV Price",
                    invoice_item_type = arrayX.sales_charge_name,  //"CV Price Adjustment",
                    quantity = (double)lookupSalesInvoice.quantity,
                    adjustment_quantity = (double)lookupSalesInvoice.quantity,
                    price = (double)lookupSalesInvoice.unit_price * exchange_rate,
                    adjustment_price = (double)arrayX.price * exchange_rate,
                    actualValue = 100,
                    advance_payment = (Double)advance_payment

                };

                dynamic lookup;
                switch (arrayX.sales_charge_code)
                {
                    //case "ash" : vAdjustment.actualValue = (double)tempQualitySamplingAnalyte.Where(o => o.analyte_symbol == "Ash (adb)").FirstOrDefault().analyte_value;
                    case "ash":
                        lookup = tempQualitySamplingAnalyte.Where(o => o.analyte_symbol.Contains("Ash")).FirstOrDefault();
                        if (lookup != null && lookup.analyte_value != null)
                            vAdjustment.actualValue = (double)lookup.analyte_value;
                        else
                            return BadRequest("'Ash' not found!");
                        break;

                    //case "sulphur_adjustment": vAdjustment.actualValue = (double)tempQualitySamplingAnalyte.Where(o => o.analyte_symbol == "TS (adb)").FirstOrDefault().analyte_value;
                    case "sulphur_adjustment":
                        lookup = tempQualitySamplingAnalyte.Where(o => o.analyte_symbol.Contains("TS")).FirstOrDefault();
                        if (lookup != null && lookup.analyte_value != null)
                            vAdjustment.actualValue = (double)lookup.analyte_value;
                        else
                            return BadRequest("'TS' not found!");
                        break;

                    case "cv_adjustment":
                        //lookup = tempQualitySamplingAnalyte.Where(o => o.analyte_symbol == "GCV (ar)").FirstOrDefault();
                        lookup = tempQualitySamplingAnalyte.Where(o => (o.analyte_symbol.Contains("NAR") || o.analyte_symbol.Contains("GCV")))
                            .FirstOrDefault();
                        if (lookup != null && lookup.analyte_value != null)
                            vAdjustment.actualValue = (double)lookup.analyte_value;
                        else
                            return BadRequest("'NAR' or 'GCV' not found!");
                        break;
                    default:
                        vAdjustment.actualValue = 0;
                        break;

                }
                vAdjustment.value = vAdjustment.adjustment_price * vAdjustment.adjustment_quantity;
                xTotalInvoice += vAdjustment.value;
                vAdjustment.total_invoice = xTotalInvoice;

                retVal.Add(vAdjustment);
            }

            //var shippingCost = dbContext.vw_shipping_cost
            //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
            //        o.despatch_order_id == lookupSalesInvoice.despatch_order_id).FirstOrDefault();

            ////bobby 20020420 freight cost
            //if (shippingCost != null)
            //{

            //    var cost1_Outline = new InvoiceOutline()
            //    {
            //        invoice_type = "shipping cost",
            //        invoice_item = "shipping cost",
            //        invoice_item_type = "freight cost",
            //        quantity = (double)lookupSalesInvoice.quantity,
            //        adjustment_quantity = (double)shippingCost.quantity, //lookupSalesInvoice.quantity,
            //        price = (Double)(shippingCost.freight_rate), // * exchange_rate,
            //        //adjustment_price = (Double)(shippingCost.freight_rate * (int)lookupSalesInvoice.quantity)
            //    };
            //    cost1_Outline.value = (Double)(shippingCost.freight_rate * shippingCost.quantity); //(int)lookupSalesInvoice.quantity) * exchange_rate ;
            //    cost1_Outline.total_invoice = xTotalInvoice + ((Double)(shippingCost.freight_rate * shippingCost.quantity)) ;//(int)lookupSalesInvoice.quantity) * exchange_rate);
            //    retVal.Add(cost1_Outline);
            //    xTotalInvoice += (Double)(shippingCost.freight_rate * shippingCost.quantity)/ exchange_rate;//(int)lookupSalesInvoice.quantity) * exchange_rate;

            //    var cost2_Outline = new InvoiceOutline()
            //    {
            //        invoice_type = "shipping cost",
            //        invoice_item = "shipping cost",
            //        invoice_item_type = "insurance",
            //        quantity = (double)lookupSalesInvoice.quantity,
            //        adjustment_quantity = (double)lookupSalesInvoice.quantity,
            //        price = (Double)shippingCost.insurance_cost,// * exchange_rate,
            //        adjustment_price = (Double)shippingCost.insurance_cost,// * exchange_rate
            //    };
            //    cost2_Outline.value = (Double)shippingCost.insurance_cost;
            //    cost2_Outline.total_invoice = xTotalInvoice + ((Double)shippingCost.insurance_cost);// * exchange_rate);
            //    retVal.Add(cost2_Outline);
            //    xTotalInvoice += (Double)shippingCost.insurance_cost / exchange_rate;
            //}

            //bobby 20220519 get from totalprice (final calculation)
            xTotalInvoice = (Double)lookupSalesInvoice.total_price * exchange_rate;
            xTotalInvoice -= (Double)advance_payment;
            var xTotalInvoice_beforTax = xTotalInvoice;


            //bobby 20220519 show subtotal
            var subtotalOutline = new InvoiceOutline()
            {
                invoice_type = "subtotal",
                invoice_item = "Subtotal",
                invoice_item_type = "Subtotal",
                quantity = 0,
                adjustment_quantity = 0,
                price = 0,
                adjustment_price = 0
            };
            //totalOutline.value = xTotalInvoice;
            subtotalOutline.total_invoice = xTotalInvoice;
            retVal.Add(subtotalOutline);
            listTaxes.Add(subtotalOutline);
            //

            //bobby 20221026 rubah posisi downpayment
            var lookupCustomer = await dbContext.vw_customer.Where(o => o.id == lookupDespatchOrder.customer_id).FirstOrDefaultAsync();
            var lookupSalesContract = await dbContext.vw_sales_contract.Where(o => o.id == lookupDespatchOrder.sales_contract_id).FirstOrDefaultAsync();
            //var lookupSalesContractTerm = await dbContext.vw_sales_contract_term.Where(o => o.id == lookupSalesInvoice.sales_contract_term_id).FirstOrDefaultAsync();

            var respDownPayment = 0.0;



            if ((lookupSalesInvoice.downpayment != null && lookupSalesInvoice.downpayment > 0) ||
                (lookupSalesInvoiceDP?.total_price != null))
            {
                var lookupDespatchOrder_parent = await dbContext.despatch_order
                    .Where(o => o.id == lookupDespatchOrder.parent_despatch_order_id)
                    .FirstOrDefaultAsync();
                if (lookupDespatchOrder_parent != null)
                {
                    respDownPayment = (double)lookupSalesInvoice.downpayment;
                }
                else
                {
                    respDownPayment = (double)lookupSalesInvoice.downpayment * exchange_rate;
                }


                //bobby 20220523 downpayment
                if (lookupSalesInvoice.downpayment != lookupSalesInvoice.total_price)
                {
                    //bobby 20220523 downpayment pelunasan
                    double dp_value = 0;
                    double dp_exchange_rate = 0;
                    if (lookupSalesInvoiceDP != null)
                    {
                        //var total_price = (double)lookupSalesInvoice.total_price;
                        dp_value = Convert.ToDouble(lookupSalesInvoiceDP.downpayment);
                        dp_exchange_rate = Convert.ToDouble(lookupSalesInvoiceDP.exchange_rate);
                        // var invoice_value = xTotalInvoice / exchange_rate;
                        // invoice_value -= dp_value;
                        var totaldownpayment2 = new InvoiceOutline()
                        {
                            invoice_type = "total",
                            invoice_item = "Down Payment",
                            invoice_item_type = "Down Payment",
                            quantity = 0,
                            adjustment_quantity = 0,
                            price = 0,
                            adjustment_price = 0
                        };
                        //totalOutline.value = xTotalInvoice;
                        totaldownpayment2.total_invoice = dp_value * dp_exchange_rate;

                        retVal.Add(totaldownpayment2);
                        listTaxes.Add(totaldownpayment2);

                    }
                    // downpayment pelunasan
                    if (lookupDespatchOrder_parent != null)
                    {
                        dp_value = (double)lookupSalesInvoice.downpayment;
                        dp_exchange_rate = (double)lookupSalesInvoice.exchange_rate;
                        // var invoice_value = xTotalInvoice / exchange_rate;
                        // invoice_value -= dp_value;
                        var totaldownpayment2 = new InvoiceOutline()
                        {
                            invoice_type = "total",
                            invoice_item = "Down Payment",
                            invoice_item_type = "Down Payment",
                            quantity = 0,
                            adjustment_quantity = 0,
                            price = 0,
                            adjustment_price = 0
                        };
                        //totalOutline.value = xTotalInvoice;
                        totaldownpayment2.total_invoice = dp_value;



                        retVal.Add(totaldownpayment2);
                        listTaxes.Add(totaldownpayment2);
                    }


                    var totaldownpayment = new InvoiceOutline()
                    {
                        invoice_type = "total",
                        invoice_item = "Total Invoice",
                        invoice_item_type = "Total Invoice",
                        quantity = 0,
                        adjustment_quantity = 0,
                        price = 0,
                        adjustment_price = 0
                    };
                    //totalOutline.value = xTotalInvoice;
                    if (lookupSalesInvoiceDP != null)
                    {

                        if (lookupDespatchOrder_parent != null)
                        {
                            totaldownpayment.total_invoice = xTotalInvoice_beforTax - dp_value;
                            //bobby
                            xTotalInvoice = totaldownpayment.total_invoice;
                        }
                        else
                        {
                            totaldownpayment.total_invoice = xTotalInvoice_beforTax - (dp_value * dp_exchange_rate);
                            //bobby
                            xTotalInvoice = totaldownpayment.total_invoice;
                        }
                    }
                    else
                    {
                        if (lookupDespatchOrder_parent != null)
                        {
                            totaldownpayment.total_invoice = xTotalInvoice - (double)lookupSalesInvoice.downpayment;
                            //bobby
                            xTotalInvoice = totaldownpayment.total_invoice;
                        }
                        else
                        {
                            totaldownpayment.total_invoice = respDownPayment;
                            //bobby
                            xTotalInvoice = totaldownpayment.total_invoice;
                        }
                    }
                    retVal.Add(totaldownpayment);
                    listTaxes.Add(totaldownpayment);
                }
                //


            }
            //bobby 20221026 rubah posisi downpayment

            var taxes1 = await dbContext.vw_sales_contract_taxes
                .Where(o => o.sales_contract_term_id == lookupSalesInvoice.sales_contract_term_id).ToArrayAsync();

            double tempTaxInvoice = 0.0;
            foreach (vw_sales_contract_taxes tax1 in taxes1)
            {
                var taxOutline = new InvoiceOutline()
                {
                    invoice_type = "tax",
                    invoice_item = tax1.sales_contract_tax_name,
                    invoice_item_type = tax1.tax_name,
                    quantity = (int)tax1.calculation_sign,
                    adjustment_quantity = (int)tax1.calculation_sign,
                    price = (double)(tax1.tax_rate),
                    adjustment_price = (double)(tax1.tax_rate)
                };
                // 
                taxOutline.value = taxOutline.adjustment_price * taxOutline.adjustment_quantity;
                taxOutline.total_invoice = Math.Round((xTotalInvoice * taxOutline.value / 100), 0, MidpointRounding.AwayFromZero);
                tempTaxInvoice += Math.Round(taxOutline.total_invoice, 0, MidpointRounding.AwayFromZero);
                retVal.Add(taxOutline);
                listTaxes.Add(taxOutline);
            }
            xTotalInvoice += tempTaxInvoice;

            //bobby 20220519 show total
            var totalOutline = new InvoiceOutline()
            {
                invoice_type = "total",
                invoice_item = "Total",
                invoice_item_type = "Total",
                quantity = 0,
                adjustment_quantity = 0,
                price = 0,
                adjustment_price = 0
            };
            //totalOutline.value = xTotalInvoice;
            totalOutline.total_invoice = xTotalInvoice;
            retVal.Add(totalOutline);
            listTaxes.Add(totalOutline);
            //

            if (lookupSalesInvoice.is_fobprice != null)
            {
                totalOutline = new InvoiceOutline()
                {
                    invoice_type = "total",
                    invoice_item = "Total Price HPB",
                    invoice_item_type = "Total Price HPB",
                    quantity = 0,
                    adjustment_quantity = 0,
                    price = 0,
                    adjustment_price = 0
                };
                //totalOutline.value = xTotalInvoice;
                totalOutline.total_invoice = (Double)lookupSalesInvoice.total_price_hpb;
                retVal.Add(totalOutline);
                listTaxes.Add(totalOutline);
            }

            var shippingCost = dbContext.vw_shipping_cost
               .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                   o.despatch_order_id == lookupSalesInvoice.despatch_order_id).FirstOrDefault();

            //bobby 20020420 freight cost
            if (shippingCost != null)
            {
                double xTotalInvoice2 = 0;
                var cost1_Outline = new InvoiceOutline()
                {
                    invoice_type = "total",
                    invoice_item = "shipping cost",
                    invoice_item_type = "freight cost",
                    quantity = 0,
                    adjustment_quantity = 0,
                    price = 0,
                    adjustment_price = 0
                };
                //totalOutline.value = xTotalInvoice;
                cost1_Outline.total_invoice = ((Double)(shippingCost.freight_rate * shippingCost.quantity));//(int)lookupSalesInvoice.quantity) * exchange_rate);
                retVal.Add(cost1_Outline);
                listTaxes.Add(cost1_Outline);
                xTotalInvoice2 += ((Double)(shippingCost.freight_rate * shippingCost.quantity));//(int)lookupSalesInvoice.quantity) * exchange_rate;

                //

                var cost2_Outline = new InvoiceOutline()
                {
                    invoice_type = "total",
                    invoice_item = "shipping cost",
                    invoice_item_type = "Insurance",
                    quantity = 0,
                    adjustment_quantity = 0,
                    price = 0,
                    adjustment_price = 0
                };
                //totalOutline.value = xTotalInvoice;
                cost2_Outline.total_invoice = ((Double)shippingCost.insurance_cost);// * exchange_rate);
                retVal.Add(cost2_Outline);
                listTaxes.Add(cost2_Outline);
                xTotalInvoice2 += ((Double)shippingCost.insurance_cost);// * exchange_rate);
                                                                        //
                                                                        //fariz
                var cost3_Outline = new InvoiceOutline()
                {
                    invoice_type = "total",
                    invoice_item = "shipping cost",
                    invoice_item_type = "Advance Payment",
                    quantity = 0,
                    adjustment_quantity = 0,
                    price = 0,
                    adjustment_price = 0
                };
                //totalOutline.value = xTotalInvoice;
                cost3_Outline.total_invoice = ((Double)(shippingCost.advance_payment ?? 0) * (Double)(shippingCost.quantity ?? 0));// * exchange_rate);
                retVal.Add(cost3_Outline);
                listTaxes.Add(cost3_Outline);
                xTotalInvoice2 -= ((Double)(shippingCost.advance_payment ?? 0) * (Double)(shippingCost.quantity ?? 0));// * exchange_rate);

                double tempTaxInvoice2 = 0.0;
                foreach (vw_sales_contract_taxes tax2 in taxes1)
                {
                    var taxOutline2 = new InvoiceOutline()
                    {
                        invoice_type = "tax",
                        invoice_item = tax2.sales_contract_tax_name,
                        invoice_item_type = tax2.tax_name,
                        quantity = (int)tax2.calculation_sign,
                        adjustment_quantity = (int)tax2.calculation_sign,
                        price = (double)(tax2.tax_rate),
                        adjustment_price = (double)(tax2.tax_rate)
                    };
                    // 
                    taxOutline2.value = taxOutline2.adjustment_price * taxOutline2.adjustment_quantity;
                    taxOutline2.total_invoice = Math.Round((xTotalInvoice2 * taxOutline2.value / 100), 0, MidpointRounding.AwayFromZero);
                    tempTaxInvoice2 += Math.Round(taxOutline2.total_invoice, 0, MidpointRounding.AwayFromZero);
                    retVal.Add(taxOutline2);
                    listTaxes.Add(taxOutline2);
                }

                //bobby 20220811 show total2
                var totalOutline2 = new InvoiceOutline()
                {
                    invoice_type = "total",
                    invoice_item = "Total",
                    invoice_item_type = "Total",
                    quantity = 0,
                    adjustment_quantity = 0,
                    price = 0,
                    adjustment_price = 0
                };
                //totalOutline.value = xTotalInvoice;
                totalOutline2.total_invoice = xTotalInvoice + xTotalInvoice2 + tempTaxInvoice2;
                retVal.Add(totalOutline2);
                listTaxes.Add(totalOutline2);
                //
            }


            if (lookupSalesInvoice.sales_type_name != null && lookupSalesInvoice.sales_type_name.ToLower().Trim() == "domestic pln")
                xTotalInvoice = (double)lookupSalesInvoice.total_price;

            var retVal2 = new
            {
                retVal,
                invoiceTotal = xTotalInvoice,
                invoiceFreightCost = varFreightCost,
                invoiceDownPayment = respDownPayment,
                invoiceDueDate = lookupSalesInvoice.invoice_date,
                invoiceName = lookupSalesInvoice.invoice_number,
                invoiceCurrencySymbol = varInvoiceCurrencySymbol,
                invoiceCurrencyCode = varInvoiceCurrencyCode,
                invoiceCurrencyName = varInvoiceCurrencyName,
                invoiceBuyerName = lookupDespatchOrder.customer_name,
                invoiceContractName = lookupDespatchOrder.sales_contract_name,
                invoiceVesselName = lookupDespatchOrder.vessel_name,
                invoiceLaycanDate = lookupDespatchOrder.laycan_start.ToString() + " - " + lookupDespatchOrder.laycan_end.ToString(),
                invoiceBuyerAddress = lookupCustomer.primary_address,
                invoiceBillOfLadingDate = lookupDespatchOrder.bill_of_lading_date,
                invoiceBillOfLadingNumber = "Bill 973 (TBD)",
                vesselFrom = "Indonesia (TBD)",
                vesselTo = lookupDespatchOrder.ship_to,
                invoicePayment = "Payment Term 1 (TBD)",
                letterOfCreditNumber = lookupDespatchOrder.letter_of_credit,
                dateOfIssue = "2022/01/23 (TBD)",
                issuedByUser = "User Issuer (TBD)",
                //invoiceNotes = lookupSalesInvoice.notes,
                invoiceNotes = notestext,
                salesContractNumber = lookupSalesContract.sales_contract_name,
                salesContractId = lookupSalesContract.id,
                salesContractDate = lookupSalesContract.start_date,
                lcStatus = lookupSalesInvoice.lc_status,
                lcDateIssue = lookupSalesInvoice.lc_date_issue,
                lcIssuingBank = lookupSalesInvoice.lc_issuing_bank,
                advance_payment = (Double)advance_payment
            };

            return retVal2;
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

        [HttpGet("GetSalesInvoiceAdjusmentBySalesInvoiceId")]
        public async Task<object> GetSalesInvoiceAdjusmentBySalesInvoiceId(string salesInvoiceId, DataSourceLoadOptions loadOptions)
        {
            var lookup = dbContext.sales_invoice_charges
                .Where(o => o.sales_invoice_id == salesInvoiceId);
            return await DataSourceLoader.LoadAsync(lookup, loadOptions);
        }

        [HttpGet("CheckSyntaxFormula")]
        public async Task<object> CheckSyntaxFormula(string tsyntax, DataSourceLoadOptions loadOptions)
        {
            string tstatus = "";
            string tmessage = "";
            string xsyntax = tsyntax.ToLower().Replace(" ", "");
            var lookupMasterList = await dbContext.master_list
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.item_group == "reserved-words").ToArrayAsync();
            foreach (master_list lookup in lookupMasterList)
            {
                xsyntax = xsyntax.Replace(lookup.item_name.ToLower(), "5");
            }
            var lookupAnalyteSymbol = dbContext.analyte.
                Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                           o.analyte_symbol.Length > 0);
            foreach (analyte lookup in lookupAnalyteSymbol)
            {
                xsyntax = xsyntax.Replace((lookup.analyte_symbol.Replace(" ", "") + ".value()").ToLower(), "4");
                xsyntax = xsyntax.Replace((lookup.analyte_symbol.Replace(" ", "") + ".target()").ToLower(), "3");

                xsyntax = xsyntax.Replace((lookup.analyte_symbol.Replace(" ", "") + ".min()").ToLower(), "4");
                xsyntax = xsyntax.Replace((lookup.analyte_symbol.Replace(" ", "") + ".max()").ToLower(), "3");
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

        [HttpGet("GetReservedWords")]
        public async Task<object> GetReservedWords(DataSourceLoadOptions loadOptions)
        {
            ReservedWords rw;
            List<ReservedWords> listRW = new List<ReservedWords>();
            var lookupMasterList = await dbContext.master_list.
                Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                           o.item_group == "reserved-words").ToArrayAsync();
            foreach (master_list lookup in lookupMasterList)
            {
                rw = new ReservedWords(lookup.item_name, lookup.notes);
                listRW.Add(rw);
            }
            var lookupAnalyteSymbol = await dbContext.analyte
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                           o.analyte_symbol.Length > 0)
                .OrderBy(o => o.analyte_name)
                .ToArrayAsync();
            foreach (analyte lookup in lookupAnalyteSymbol)
            {
                rw = new ReservedWords(lookup.analyte_symbol + ".Value()", "actual value of " + lookup.analyte_name);
                listRW.Add(rw);

                rw = new ReservedWords(lookup.analyte_symbol + ".Target()", "target value of " + lookup.analyte_name);
                listRW.Add(rw);

                rw = new ReservedWords(lookup.analyte_symbol + ".Min()", "minimum value of " + lookup.analyte_name);
                listRW.Add(rw);

                rw = new ReservedWords(lookup.analyte_symbol + ".Max()", "maximum value of " + lookup.analyte_name);
                listRW.Add(rw);
            }

            //listRW.Sort((p1, p2) => p1.reservedW.CompareTo(p2.reservedW));

            //return listRW;
            return DataSourceLoader.Load(listRW, loadOptions);
        }

        [HttpGet("GetReservedFunctions")]
        public async Task<object> GetReservedFunctions(DataSourceLoadOptions loadOptions)
        {
            ReservedWords rw;
            List<ReservedWords> listRW = new List<ReservedWords>();
            var lookupMasterList = await dbContext.master_list.
                Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                           o.item_group == "reserved-functions").ToArrayAsync();
            foreach (master_list lookup in lookupMasterList)
            {
                rw = new ReservedWords(lookup.item_name, lookup.notes);
                listRW.Add(rw);
            }

            //return listRW;
            return DataSourceLoader.Load(listRW, loadOptions);
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

        [HttpPost("UploadDocument")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UploadDocument([FromBody] dynamic FileDocument)
        {
            var result = new StandardResult();
            long size = 0;

            if (FileDocument == null)
            {
                return BadRequest("No file uploaded!");
            }

            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);

            var fileName = (string)FileDocument.filename;
            FilePath += $@"\{fileName}";

            string strfile = (string)FileDocument.data;
            byte[] arrfile = Convert.FromBase64String(strfile);

            await System.IO.File.WriteAllBytesAsync(FilePath, arrfile);

            size = fileName.Length;
            string sFileExt = Path.GetExtension(FilePath).ToLower();

            ISheet sheet;
            dynamic wb;
            if (sFileExt == ".xls")
            {
                FileStream stream = System.IO.File.OpenRead(FilePath);
                wb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats
                sheet = wb.GetSheetAt(0); //get first sheet from workbook
                stream.Close();
            }
            else
            {
                wb = new XSSFWorkbook(FilePath); //This will read 2007 Excel format
                sheet = wb.GetSheetAt(0); //get first sheet from workbook
            }

            string DO = string.Empty;
            string errormessage = string.Empty;
            string teks = string.Empty;
            bool gagal = false;

            // Begin TX
            using var tx = await dbContext.Database.BeginTransactionAsync();

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var SONumber = "";
                    var salesType = "";
                    var quotationType = "";
                    var invoiceType = "";
                    var AdvBank = "";
                    var billTo = "";
                    var incoTerm = "";

                    var SO = await dbContext.despatch_order
                        .Where(o => o.despatch_order_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower())
                        .FirstOrDefaultAsync();
                    if (SO != null) SONumber = SO.id.ToString();
                    DO = SONumber;

                    var ST = await dbContext.master_list
                        .Where(o => o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower())
                        .FirstOrDefaultAsync();
                    if (ST != null) salesType = ST.id.ToString();

                    var QT = await dbContext.master_list
                        .Where(o => o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower())
                        .FirstOrDefaultAsync();
                    if (QT != null) quotationType = QT.id.ToString();

                    var IT = await dbContext.master_list
                        .Where(o => o.item_name.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(4)).ToUpper())
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.item_group == "invoice-type")
                        .FirstOrDefaultAsync();
                    if (IT != null) invoiceType = IT.id.ToString();

                    vw_bank_account AB = null;
                    string[] accNumber = PublicFunctions.IsNullCell(row.GetCell(8)).Split("/");
                    if (accNumber.Length > 1)
                    {
                        AB = await dbContext.vw_bank_account
                        .Where(o => o.account_number.ToLower() == accNumber[1].ToLower())
                        .FirstOrDefaultAsync();
                    }
                    else
                    {
                        accNumber = PublicFunctions.IsNullCell(row.GetCell(8)).Split("-");
                        if (accNumber.Length > 1)
                        {
                            AB = await dbContext.vw_bank_account
                            .Where(o => o.account_number.ToLower() == accNumber[1].ToLower())
                            .FirstOrDefaultAsync();
                        }
                    }
                    if (AB != null) AdvBank = AB.id.ToString();

                    var CST = await dbContext.customer
                        .Where(o => o.alias_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(9)).ToLower())
                        .FirstOrDefaultAsync();
                    if (CST != null) billTo = CST.id.ToString();

                    var incotermDat = await dbContext.master_list
                        .Where(o => o.item_name.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(15)).ToString().ToLower())
                        .FirstOrDefaultAsync();
                    if (incotermDat != null) incoTerm = incotermDat.id.ToString();

                    var record = await dbContext.sales_invoice
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.invoice_number.ToUpper().Trim() == PublicFunctions.IsNullCell(row.GetCell(1)).ToUpper().Trim())
                        .FirstOrDefaultAsync();

                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = System.DateTime.Now;

                        record.despatch_order_id = SONumber;
                        record.sales_type_id = salesType;
                        record.quotation_type_id = quotationType;
                        record.invoice_type_id = invoiceType;
                        record.invoice_date = PublicFunctions.Tanggal(row.GetCell(5));
                        record.consignee = PublicFunctions.IsNullCell(row.GetCell(6));
                        record.description_of_goods = PublicFunctions.IsNullCell(row.GetCell(7));
                        record.bank_account_id = AdvBank;
                        record.bill_to = billTo;
                        record.alias4 = PublicFunctions.Tanggal(row.GetCell(10));
                        record.quantity = PublicFunctions.Desimal(row.GetCell(11));
                        record.unit_price = PublicFunctions.Desimal(row.GetCell(12));
                        //record.alias5 = PublicFunctions.Desimal(row.GetCell(13));
                        //record.alias6 = PublicFunctions.Desimal(row.GetCell(14));
                        record.alias7 = PublicFunctions.Desimal(row.GetCell(13));
                        record.alias8 = PublicFunctions.Desimal(row.GetCell(14));
                        record.inco_term = incoTerm;
                        record.szfreightcontractno = PublicFunctions.IsNullCell(row.GetCell(16));
                        record.decfreightpriceton = PublicFunctions.Desimal(row.GetCell(17));
                        record.decfreightamount = PublicFunctions.Desimal(row.GetCell(18));
                        record.dtmfreightinvoicedatereceive = PublicFunctions.Tanggal(row.GetCell(19));
                        record.szfreightcontractno = PublicFunctions.IsNullCell(row.GetCell(20));
                        record.fob_price = PublicFunctions.Desimal(row.GetCell(21));
                        record.subtotal = PublicFunctions.Desimal(row.GetCell(22));
                        record.total_price = PublicFunctions.Desimal(row.GetCell(23));
                        record.notes = PublicFunctions.IsNullCell(row.GetCell(25));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new sales_invoice();
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

                        record.despatch_order_id = SONumber;
                        record.sales_type_id = salesType;
                        record.quotation_type_id = quotationType;
                        record.invoice_type_id = invoiceType;
                        record.invoice_date = PublicFunctions.Tanggal(row.GetCell(5));
                        record.consignee = PublicFunctions.IsNullCell(row.GetCell(6));
                        record.description_of_goods = PublicFunctions.IsNullCell(row.GetCell(7));
                        record.bank_account_id = AdvBank;
                        record.bill_to = billTo;
                        record.alias4 = PublicFunctions.Tanggal(row.GetCell(10));
                        record.quantity = PublicFunctions.Desimal(row.GetCell(11));
                        record.unit_price = PublicFunctions.Desimal(row.GetCell(12));
                        //record.alias5 = PublicFunctions.Desimal(row.GetCell(13));
                        //record.alias6 = PublicFunctions.Desimal(row.GetCell(14));
                        record.alias7 = PublicFunctions.Desimal(row.GetCell(13));
                        record.alias8 = PublicFunctions.Desimal(row.GetCell(14));
                        record.inco_term = incoTerm;
                        record.szfreightcontractno = PublicFunctions.IsNullCell(row.GetCell(16));
                        record.decfreightpriceton = PublicFunctions.Desimal(row.GetCell(17));
                        record.decfreightamount = PublicFunctions.Desimal(row.GetCell(18));
                        record.dtmfreightinvoicedatereceive = PublicFunctions.Tanggal(row.GetCell(19));
                        record.szfreightcontractno = PublicFunctions.IsNullCell(row.GetCell(20));
                        record.fob_price = PublicFunctions.Desimal(row.GetCell(21));
                        record.subtotal = PublicFunctions.Desimal(row.GetCell(22));
                        record.total_price = PublicFunctions.Desimal(row.GetCell(23));
                        record.notes = PublicFunctions.IsNullCell(row.GetCell(25));
                        record.invoice_number = PublicFunctions.IsNullCell(row.GetCell(1));

                        dbContext.sales_invoice.Add(record);
                        await dbContext.SaveChangesAsync();
                    }

                    var recordCost = await dbContext.shipping_cost
                        .Where(x => x.despatch_order_id == record.despatch_order_id)
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();

                    if (recordCost == null)
                    {
                        recordCost = new shipping_cost();
                        recordCost.id = Guid.NewGuid().ToString("N");
                        recordCost.created_by = CurrentUserContext.AppUserId;
                        recordCost.created_on = System.DateTime.Now;
                        recordCost.modified_by = null;
                        recordCost.modified_on = null;
                        recordCost.is_active = true;
                        recordCost.is_default = null;
                        recordCost.is_locked = null;
                        recordCost.entity_id = null;
                        recordCost.owner_id = CurrentUserContext.AppUserId;
                        recordCost.organization_id = CurrentUserContext.OrganizationId;
                        recordCost.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        recordCost.despatch_order_id = record.despatch_order_id;
                        recordCost.late_penalty = PublicFunctions.Desimal(row.GetCell(24));

                        dbContext.shipping_cost.Add(recordCost);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var e = new entity();
                        e.InjectFrom(recordCost);

                        recordCost.InjectFrom(e);
                        recordCost.modified_by = CurrentUserContext.AppUserId;
                        recordCost.modified_on = System.DateTime.Now;

                        recordCost.late_penalty = PublicFunctions.Desimal(row.GetCell(24));

                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 1, Line " + (i + 1) + " : " + Environment.NewLine;
                    }
                    else errormessage = ex.Message;

                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                    gagal = true;
                    break;
                }
            }

            sheet = wb.GetSheetAt(1); //*** detail sheet
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var header = string.Empty;
                    var sales_invoicex = await dbContext.sales_invoice
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .Where(x => x.invoice_number == PublicFunctions.IsNullCell(row.GetCell(0)))
                        .FirstOrDefaultAsync();
                    if (sales_invoicex != null) header = sales_invoicex.id;

                    var analyte_id = "";
                    var analytex = await dbContext.analyte
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.analyte_symbol == PublicFunctions.IsNullCell(row.GetCell(1)))
                        .FirstOrDefaultAsync();
                    if (analytex != null) analyte_id = analytex.id;

                    var record = await dbContext.sales_invoice_product_specifications
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.sales_invoice_id == header)
                        .Where(o => o.analyte_id == analyte_id)
                        .FirstOrDefaultAsync();

                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = System.DateTime.Now;

                        record.nilai_penyesuaian = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.nilai_denda_penolakan = PublicFunctions.Desimal(row.GetCell(3));

                        await dbContext.SaveChangesAsync();
                    }

                    else if (record == null)
                    {
                        record = new sales_invoice_product_specifications();
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

                        record.nilai_penyesuaian = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.nilai_denda_penolakan = PublicFunctions.Desimal(row.GetCell(3));
                        record.sales_invoice_id = header;
                        record.analyte_id = analyte_id;

                        dbContext.sales_invoice_product_specifications.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 2, Line " + (i + 1) + ": " + Environment.NewLine;
                    }
                    else errormessage = ex.Message;

                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                    gagal = true;
                    break;
                }
            }
            wb.Close();
            if (gagal)
            {
                logger.Error(teks);
                await tx.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "Sales_Invoice");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await tx.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpGet("HBACalculate/{despatchOrder}/{hbaType}")]
        public async Task<object> HBACalculate(string despatchOrder, string hbaType)
        {
            try
            {
                var soData = await dbContext.vw_despatch_order
                    .Where(o => o.id == despatchOrder).FirstOrDefaultAsync();
                var dsData = await dbContext.draft_survey
                    .Where(o => o.despatch_order_id == soData.id).FirstOrDefaultAsync();
                var BlDate = Convert.ToDateTime(dsData.bill_lading_date);
                decimal result = 0;
                if (hbaType == "Kelistrikan")
                {
                    result = 70;
                }
                else if (hbaType == "Semen")
                {
                    result = 90;
                }
                else if (hbaType == "HBA")
                {
                    var recPIH = await dbContext.vw_price_index_history
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.price_index_code == "HBA" && o.index_date <= BlDate.Date)
                            .OrderByDescending(o => o.index_date).FirstOrDefaultAsync();
                    result = recPIH?.index_value ?? 0;
                }
                else if (hbaType == "HBAI")
                {
                    var recPIH = await dbContext.vw_price_index_history
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.price_index_code == "HBAI" && o.index_date <= BlDate.Date)
                            .OrderByDescending(o => o.index_date).FirstOrDefaultAsync();
                    result = recPIH?.index_value ?? 0;
                }
                else if (hbaType == "HBAII")
                {
                    var recPIH = await dbContext.vw_price_index_history
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.price_index_code == "HBAII" && o.index_date <= BlDate.Date)
                            .OrderByDescending(o => o.index_date).FirstOrDefaultAsync();
                    result = recPIH?.index_value ?? 0;
                }
                else if (hbaType == "HBAIII")
                {
                    var recPIH = await dbContext.vw_price_index_history
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.price_index_code == "HBAIII" && o.index_date <= BlDate.Date)
                            .OrderByDescending(o => o.index_date).FirstOrDefaultAsync();
                    result = recPIH?.index_value ?? 0;
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("CalculateFormulaById/{despatchOrder}/{salesChargeId}/{sHBA}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> CalculateFormulaById(string despatchOrder, string salesChargeId, string sHBA, DataSourceLoadOptions loadOptions)
        {
            string tstatus = "";
            decimal resultValue = 0;
            string formula = "";
            dynamic retVal;

            double hbaValue = 0;
            bool result = double.TryParse(sHBA, out hbaValue);

            var record = await dbContext.sales_charge
                .Where(o => o.id == salesChargeId).FirstOrDefaultAsync();

            if (record != null)
            {
                formula = record.charge_formula.ToLower().Replace(" ", "").Replace(".royalty()", "");
            }
            else
            {
                tstatus = "Sales Charge not found";
                resultValue = -1;

                retVal = new
                {
                    status = tstatus,
                    value = resultValue
                };
                return retVal;
            }

            string xsyntax = formula;
            var qualitySampling = await dbContext.vw_quality_sampling
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.despatch_order_id == despatchOrder).FirstOrDefaultAsync();
            var lookupAnalyteSymbol = dbContext.vw_quality_sampling_analyte
                .Where(o => o.quality_sampling_id == qualitySampling.id);
            foreach (vw_quality_sampling_analyte lookup in lookupAnalyteSymbol)
            {
                xsyntax = xsyntax.Replace((lookup.analyte_symbol.ToLower().Replace(" ", "")), lookup.analyte_value.ToString().Replace(",", "."));
            }

            xsyntax = xsyntax.Replace("baseunitprice()", hbaValue.ToString());
            xsyntax = xsyntax.Replace(".value()", "".ToString());

            try
            {
                var engine1 = new Engine();
                var expression1 = engine1.Parse(xsyntax);
                System.Data.DataTable dt = new System.Data.DataTable();
                var a = dt.Compute(xsyntax, "");
                resultValue = Convert.ToDecimal(a);

                tstatus = "OK";
            }
            catch (Exception ex)
            {
                tstatus = ex.Message;
                resultValue = -1;
            }

            retVal = new
            {
                status = tstatus,
                value = resultValue
            };
            return retVal;
        }

        [HttpGet("SalesChargeIdLookup")]
        public async Task<object> SalesChargeIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_sales_charge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.charge_type_name != "Royalty")
                    .Select(o => new { Value = o.id, Text = o.sales_charge_name })
                    .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpGet("RecalculateHPB")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> RecalculateHPB(string despatchOrder, string salesChargeId, string sHBA, DataSourceLoadOptions loadOptions)
        {
            #region Deklarasi Variable
            string tstatus = "";
            decimal resultValue = 0;
            string formula = "";
            dynamic retVal;
            var hbaValue = System.DateTime.MinValue;
            //List<shipment_plan> sp = await dbContext.shipment_plan.Where(o=>o.hpb_forecast == null).ToListAsync();
            List<shipment_plan> sp = await dbContext.shipment_plan
                .Where(o=>o.hpb_forecast == null && o.etc != null && o.product_id != null && o.end_user != null && o.etc != System.DateTime.MinValue)
                .ToListAsync();
            var totalData = sp.Count;
            var countData = 0;
            List<customer> c = await dbContext.customer.ToListAsync();
            List<vw_sales_charge> sc = await dbContext.vw_sales_charge.ToListAsync();
            List<vw_product_specification> ps = await dbContext.vw_product_specification.ToListAsync();
            List<price_adjustment_product_specification> paps = await dbContext.price_adjustment_product_specification.ToListAsync();
            List<price_index_product_specification> pips = await dbContext.price_index_product_specification.ToListAsync();
            List<price_index_history> pih = await dbContext.price_index_history.ToListAsync();
            //string salesChargeIds = sp.Select(o=>o.sale)
            //List<sales_charge> sc = await dbContext.sales_charge.Where(o=>sp.Contains(o.id)).ToListAsync();
            decimal count = 0; //variabel untuk cek progress pada saat debug
            #endregion
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (shipment_plan plan in sp)
                    {
                        count++;
                        var customerIndustryType = c.Where(o => o.id == plan.end_user).Select(o => o.industry_type_id).FirstOrDefault();
                        var salesChargeIndustryType = sc.Where(o => o.industry_type_id == customerIndustryType).FirstOrDefault();
                        if (salesChargeIndustryType != null && salesChargeIndustryType.industry_type_id != null)
                        {
                            formula = salesChargeIndustryType.charge_formula;
                            formula = Regex.Replace(formula, @"(?<=\s)-\s*BaseUnitPrice\(\)\s*$", string.Empty);
                            var productSpec = ps.Where(o => o.product_id == plan.product_id && o.analyte_symbol == "GCV (arb)").FirstOrDefault();
                            var tm = ps.Where(o => o.product_id == plan.product_id && o.analyte_symbol == "TM (arb)").FirstOrDefault();
                            var ts = ps.Where(o => o.product_id == plan.product_id && o.analyte_symbol == "TS (arb)").FirstOrDefault();
                            var ash = ps.Where(o => o.product_id == plan.product_id && o.analyte_symbol == "Ash (arb)").FirstOrDefault();

                            if (formula.Contains("BaseUnitPrice()"))
                            {
                                if (salesChargeIndustryType.industry_type.ToLower() == "semen" || salesChargeIndustryType.industry_type.ToLower() == "smelter")
                                {
                                    formula = formula.Replace("BaseUnitPrice()", 90.ToString());
                                }
                                else if (salesChargeIndustryType.industry_type.ToLower() == "kelistrikan")
                                {
                                    formula = formula.Replace("BaseUnitPrice()", 70.ToString());
                                }
                                else
                                {
                                    var priceIndexProductSpec = pips
                                    .Where(o => o.minimum <= productSpec.target_value && o.maximum >= productSpec.target_value).FirstOrDefault();
                                    if (priceIndexProductSpec != null)
                                    {
                                        var PriceIndexHistory = pih.Where(o => o.price_index_id == priceIndexProductSpec.price_index_id)
                                            .Where(o => o.index_date <= plan.etc).OrderByDescending(o => o.index_date)
                                            .FirstOrDefault();
                                        if (PriceIndexHistory != null)
                                        {
                                            var indexValue = PriceIndexHistory.index_value != null ? PriceIndexHistory.index_value : 0;
                                            formula = formula.Replace("BaseUnitPrice()", indexValue.ToString());
                                        }
                                        else
                                        {
                                            formula = formula.Replace("BaseUnitPrice()", 0.ToString());
                                        }
                                    }
                                    else
                                    {
                                        formula = formula.Replace("BaseUnitPrice()", 0.ToString());
                                    }
                                }
                            }
                            var gcvValue = productSpec != null ? productSpec.target_value : 0;
                            var tmValue = tm != null ? tm.target_value : 0;
                            var tsValue = ts != null ? ts.target_value : 0;
                            var ashValue = ash != null ? ash.target_value : 0;
                            if (formula.Contains("GCV (arb).Value()"))
                            {
                                formula = formula.Replace("GCV (arb).Value()", gcvValue.ToString());
                            }
                            if (formula.Contains("TM (arb).Value()"))
                            {
                                formula = formula.Replace("TM (arb).Value()", tmValue.ToString());
                            }
                            if (formula.Contains("TS (arb).Value()"))
                            {
                                formula = formula.Replace("TS (arb).Value()", tsValue.ToString());
                            }
                            if (formula.Contains("Ash (arb).Value()"))
                            {
                                formula = formula.Replace("Ash (arb).Value()", ashValue.ToString());
                            }
                            try
                            {
                                var engine1 = new Engine();
                                var expression1 = engine1.Parse(formula);
                                System.Data.DataTable dt = new System.Data.DataTable();
                                var a = dt.Compute(formula, "");
                                resultValue = Convert.ToDecimal(a);
                                resultValue = Math.Round(resultValue, 2);
                                plan.hpb_forecast = resultValue;
                                await dbContext.SaveChangesAsync();
                                countData++;
                                tstatus = "OK";
                            }
                            catch (Exception ex)
                            {
                                await tx.RollbackAsync();
                                logger.Error(ex.InnerException ?? ex);
                                retVal = new
                                {
                                    status = ex.Message,
                                    success = false
                                };
                                return retVal;
                            }
                        }
                        else
                        {
                            var productSpec = ps.Where(o => o.product_id == plan.product_id && o.analyte_symbol == "GCV (arb)").FirstOrDefault();
                            var tm = ps.Where(o => o.product_id == plan.product_id && o.analyte_symbol == "TM (arb)").FirstOrDefault();
                            var ts = ps.Where(o => o.product_id == plan.product_id && o.analyte_symbol == "TS (arb)").FirstOrDefault();
                            var ash = ps.Where(o => o.product_id == plan.product_id && o.analyte_symbol == "Ash (arb)").FirstOrDefault();
                            if (productSpec != null)
                            {
                                var priceAdjustment = paps
                                    .Where(o => o.analyte_id == productSpec.analyte_id)
                                    .Where(o => o.minimum <= productSpec.target_value && o.maximum >= productSpec.target_value)
                                    .FirstOrDefault();
                                if (priceAdjustment != null) 
                                { 
                                    var salesCharge = sc.Where(o => o.id == priceAdjustment.price_adjustment_id).FirstOrDefault();
                                    if (salesCharge != null)
                                    {
                                        formula = salesCharge.charge_formula;
                                        formula = Regex.Replace(formula, @"(?<=\s)-\s*BaseUnitPrice\(\)\s*$", string.Empty);
                                        // Find the index of the last occurrence of "-BaseUnitPrice()"
                                        int lastIndex = formula.LastIndexOf("-BaseUnitPrice()");

                                        if (lastIndex != -1)
                                        {
                                            // Remove the last occurrence of "-BaseUnitPrice()"
                                            formula = formula.Remove(lastIndex, "-BaseUnitPrice()".Length);
                                        }
                                        if (formula.Contains("BaseUnitPrice()"))
                                        {
                                            var priceIndexProductSpec = pips
                                            .Where(o => o.minimum <= productSpec.target_value && o.maximum >= productSpec.target_value).FirstOrDefault();
                                            if (priceIndexProductSpec != null)
                                            {
                                                var PriceIndexHistory = pih.Where(o => o.price_index_id == priceIndexProductSpec.price_index_id)
                                                    .Where(o => o.index_date <= plan.etc).OrderByDescending(o => o.index_date)
                                                    .FirstOrDefault();
                                                if (PriceIndexHistory != null)
                                                {
                                                    var indexValue = PriceIndexHistory.index_value != null ? PriceIndexHistory.index_value : 0;
                                                    formula = formula.Replace("BaseUnitPrice()", indexValue.ToString());
                                                }
                                                else
                                                {
                                                    formula = formula.Replace("BaseUnitPrice()", 0.ToString());
                                                }
                                            }
                                            else
                                            {
                                                formula = formula.Replace("BaseUnitPrice()", 0.ToString());
                                            }
                                        }
                                        var tmValue = tm != null ? tm.target_value : 0;
                                        var tsValue = ts != null ? ts.target_value : 0;
                                        var ashValue = ash != null ? ash.target_value : 0;
                                        if (formula.Contains("GCV (arb).Value()"))
                                        {
                                            formula = formula.Replace("GCV (arb).Value()", productSpec.target_value.ToString());
                                        }
                                        if (formula.Contains("TM (arb).Value()"))
                                        {
                                            formula = formula.Replace("TM (arb).Value()", tmValue.ToString());
                                        }
                                        if (formula.Contains("TS (arb).Value()"))
                                        {
                                            formula = formula.Replace("TS (arb).Value()", tsValue.ToString());
                                        }
                                        if (formula.Contains("Ash (arb).Value()"))
                                        {
                                            formula = formula.Replace("Ash (arb).Value()", ashValue.ToString());
                                        }
                                        try
                                        {
                                            var engine1 = new Engine();
                                            var expression1 = engine1.Parse(formula);
                                            System.Data.DataTable dt = new System.Data.DataTable();
                                            var a = dt.Compute(formula, "");
                                            resultValue = Convert.ToDecimal(a);
                                            resultValue = Math.Round(resultValue, 2);
                                            plan.hpb_forecast = resultValue;
                                            await dbContext.SaveChangesAsync();
                                            countData++;
                                            tstatus = "OK";
                                        }
                                        catch (Exception ex)
                                        {
                                            await tx.RollbackAsync();
                                            logger.Error(ex.InnerException ?? ex);
                                            retVal = new
                                            {
                                                status = ex.Message,
                                                success = false
                                            };
                                            return retVal;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    retVal = new
                    {
                        status = tstatus,
                        total = totalData,
                        value = countData,
                        success = true
                    };
                    await tx.CommitAsync();
                    return retVal;
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.InnerException ?? ex);
                    retVal = new
                    {
                        status = ex.Message,
                        success = false
                    };
                    return retVal;
                }
            }
        }

        //[HttpGet("RemainedCreditLimit")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> RemainedCreditLimit(string customer_id, DataSourceLoadOptions loadOptions)
        //{
        //    double remainedCreditLimit = 0.0;
        //    try
        //    {
        //        // find all sales contract within the same customer
        //        var vw_sales_invoice_custid = dbContext.vw_sales_invoice
        //            .Where(o => o.customer_id == customer_id)
        //            ;
        //        var array_vw_sales_invoice_custid = await vw_sales_invoice_custid.ToArrayAsync();
        //        double totalPaymentfromAllInvoice = 0.0;
        //        double totalPricefromAllInvoice = 0.0;
        //        double customerCreditLimit = 0.0;


        //        if (array_vw_sales_invoice_custid.Length > 0)
        //        {
        //            if (array_vw_sales_invoice_custid[0].credit_limit != null)
        //            {
        //                customerCreditLimit = (double)array_vw_sales_invoice_custid[0].credit_limit;
        //            }

        //            foreach (vw_sales_invoice item1 in array_vw_sales_invoice_custid)
        //            {
        //                if (item1.total_price != null)
        //                {
        //                    totalPricefromAllInvoice += (double)item1.total_price;
        //                }
        //                var vw_sales_invoice_payment_data = dbContext.vw_sales_invoice_payment.Where(o => o.sales_invoice_number == item1.invoice_number);
        //                var array_vw_sales_invoice_payment_data = await vw_sales_invoice_payment_data.ToArrayAsync();
        //                if (array_vw_sales_invoice_payment_data.Length > 0)
        //                {
        //                    foreach (vw_sales_invoice_payment itemX in array_vw_sales_invoice_payment_data)
        //                    {
        //                        if (itemX.payment_value != null)
        //                        {
        //                            totalPaymentfromAllInvoice += (double)itemX.payment_value;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        remainedCreditLimit = customerCreditLimit - totalPricefromAllInvoice + totalPaymentfromAllInvoice;

        //        return Ok(remainedCreditLimit);

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}


    }

    public class AdjustmentCalculation
    {
        public string analyte_symbol { get; set; }
        public string analyte_name { get; set; }
        public decimal analyte_value { get; set; }
        public decimal analyte_target { get; set; }
        public decimal analyte_minimum { get; set; }
        public decimal analyte_maximum { get; set; }

        public AdjustmentCalculation()
        {
            analyte_symbol = "";
            analyte_name = "";
            analyte_value = 0;
            analyte_target = 0;
            analyte_minimum = 0;
            analyte_maximum = 0;
        }
    }

    public class SalesPrice
    {
        public string chargeCode { get; set; }
        public string chargeName { get; set; }
        public decimal price { get; set; }
        public SalesPrice()
        {
            chargeName = "name";
            price = 0;
            chargeCode = "code";
        }
        public SalesPrice(string chargeN, string chargeC, decimal chargeP)
        {
            chargeName = chargeN;
            price = chargeP;
            chargeCode = chargeC;
        }

    }

    public class ReservedWords
    {
        public string reservedW { get; set; }
        public string description { get; set; }
        public ReservedWords()
        {
            reservedW = "";
            description = "";
        }
        public ReservedWords(string rw, string desc)
        {
            reservedW = rw;
            description = desc;
        }
    }

    public class InvoiceOutline
    {
        public string invoice_type { get; set; }
        public string invoice_item { get; set; }
        public string invoice_item_type { get; set; }
        public double quantity { get; set; }
        public double adjustment_quantity { get; set; }
        public double price { get; set; }
        public double adjustment_price { get; set; }
        public double actualValue { get; set; }

        public double value { get; set; }
        public double total_invoice { get; set; }
        public double advance_payment { get; set; }

    }

    public class CreditLimitData
    {
        public decimal InitialCreditLimit { get; set; }
        public decimal RemainedCreditLimit { get; set; }
        public CreditLimitData()
        {
            InitialCreditLimit = 0;
            RemainedCreditLimit = 0;
        }
        public CreditLimitData(decimal initCL, decimal remainedCL)
        {
            InitialCreditLimit = initCL;
            RemainedCreditLimit = remainedCL;
        }

    }
}
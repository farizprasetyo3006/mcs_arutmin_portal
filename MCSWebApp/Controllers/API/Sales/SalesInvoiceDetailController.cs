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
using Common;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Office2010.Excel;
using FastReport.Export.Dbf;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/[controller]")]
    [ApiController]
    public class SalesInvoiceDetailController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SalesInvoiceDetailController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                return await DataSourceLoader.LoadAsync(dbContext.sales_invoice_detail
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
                dbContext.sales_invoice_detail.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(sales_invoice_detail),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var record = new sales_invoice_detail();
                    JsonConvert.PopulateObject(values, record);

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

                    dbContext.sales_invoice_detail.Add(record);
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
                var record = dbContext.sales_invoice_detail
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
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
                var record = dbContext.sales_invoice_detail
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.sales_invoice_detail.Remove(record);
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

        [HttpGet("PSDataGrid/{key}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProductSpecificationDataGrid(string key, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_sales_invoice_product_specification
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.sales_invoice_id == key)
                .Where(o => o.non_commercial != true),
                loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("PSInsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ProductSpecificationInsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            quality_sampling qs = null;
            quality_sampling_analyte record = null;

            var tx = await dbContext.Database.BeginTransactionAsync();

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(quality_sampling_analyte),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new quality_sampling_analyte();
                    JsonConvert.PopulateObject(values, record);
                    var tempRec = dbContext.quality_sampling_analyte.Where(x => x.quality_sampling_id == record.quality_sampling_id).OrderByDescending(x => x.order).FirstOrDefault();
                    qs = dbContext.quality_sampling.Where(x => x.id == record.quality_sampling_id).FirstOrDefault();

                    var sampling_template = dbContext.sampling_template
                        .Where(o => o.id == qs.sampling_template_id).FirstOrDefault();
                    if (sampling_template != null)
                    {
                        if (sampling_template.is_stock_state == true && record.analyte_value == 0)
                        {
                            return BadRequest("Analyte Value can't be zero if Sampling Template as Stock State.");
                        }
                    }

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
                    if (tempRec == null)
                        record.order = 1;
                    else
                        record.order = tempRec.order == null ? 1 : tempRec.order + 1;

                    dbContext.quality_sampling_analyte.Add(record);
                    await dbContext.SaveChangesAsync();

                    //*** tambah di Mine Location tab Quality
                    var Qid = record.quality_sampling_id;
                    var header = dbContext.quality_sampling.Where(o => o.id == Qid).FirstOrDefault();
                    var stockLocationId = header.stock_location_id;
                    var ML = dbContext.mine_location.Where(o => o.id == stockLocationId).FirstOrDefault();
                    if (ML != null)
                    {
                        decimal tm = 0, ts = 0, ash = 0, im = 0, vm = 0, fc = 0, gcv_arb = 0, gcv_adb = 0;

                        var analyte = dbContext.vw_quality_sampling_analyte
                            .Where(o => o.quality_sampling_id == record.id)
                            .ToList();
                        foreach (var d in analyte)
                        {
                            var symbol = d.analyte_symbol.ToUpper().Trim();
                            if (symbol == "TM (ARB)") tm = (decimal)d.analyte_value;
                            else if (symbol == "TS (ADB)") ts = (decimal)d.analyte_value;
                            else if (symbol == "ASH (ADB)") ash = (decimal)d.analyte_value;
                            else if (symbol == "IM (ADB)") im = (decimal)d.analyte_value;
                            else if (symbol == "VM (ADB)") vm = (decimal)d.analyte_value;
                            else if (symbol == "FC (ADB)") fc = (decimal)d.analyte_value;
                            else if (symbol == "GCV (AR)" || symbol == "GCV (ARB)") gcv_arb = (decimal)d.analyte_value;
                            else if (symbol == "GCV (ADB)") gcv_adb = (decimal)d.analyte_value;
                        }

                        var newRec = dbContext.mine_location_quality.Where(o => o.mine_location_id == ML.id).FirstOrDefault();

                        newRec.tm = tm;
                        newRec.ts = ts;
                        newRec.ash = ash;
                        newRec.im = im;
                        newRec.vm = vm;
                        newRec.fc = fc;
                        newRec.gcv_ar = gcv_arb;
                        newRec.gcv_adb = gcv_adb;

                        dbContext.mine_location_quality.Add(newRec);
                        await dbContext.SaveChangesAsync();
                    }


                    await tx.CommitAsync();
                }
                else
                {
                    return BadRequest("User is not authorized.");
                }
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                logger.Error(ex.ToString());
                return BadRequest(ex.Message);
            }
            return Ok(record);
        }

        [HttpPut("PSUpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ProductSpecificationUpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            sales_invoice_product_specifications specs = null;
            quality_sampling_analyte analyte = null;
            quality_sampling quality = null;
            var tx = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var specDat = JsonConvert.DeserializeObject<vw_sales_invoice_product_specification>(values);
                specs = await dbContext.sales_invoice_product_specifications
                    .Where(x => x.sales_invoice_id == specDat.sales_invoice_id)
                    .Where(x => x.analyte_id == specDat.analyte_id)
                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefaultAsync();
                if (specs == null)
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(sales_invoice_product_specifications),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new sales_invoice_product_specifications();
                        JsonConvert.PopulateObject(values, record);
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
                        dbContext.sales_invoice_product_specifications.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("User is not authorized.");
                    }
                }
                else
                {
                    if (await mcsContext.CanUpdate(dbContext, specs.id, 
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = specs;
                        var e = new entity();
                        e.InjectFrom(record);
                        JsonConvert.PopulateObject(values, record);
                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("User is not authorized.");
                    }
                }

                string sampling_analyte_id = JsonConvert.DeserializeObject<vw_sales_invoice_product_specification>(values).sampling_analyte_id;
                var temp_analyte = JsonConvert.DeserializeObject<quality_sampling_analyte>(values);
                analyte = await dbContext.quality_sampling_analyte
                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                    .Where(x => x.id == sampling_analyte_id)
                    .FirstOrDefaultAsync();
                if (analyte != null)
                {
                    var record = analyte;
                    quality = await dbContext.quality_sampling
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .Where(x => x.id == record.quality_sampling_id)
                        .FirstOrDefaultAsync();
                    var e = new entity();
                    e.InjectFrom(record);
                    JsonConvert.PopulateObject(values, record);
                    var sampling_template = dbContext.sampling_template
                        .Where(o => o.id == quality.sampling_template_id).FirstOrDefault();
                    if (sampling_template != null)
                    {
                        if (sampling_template.is_stock_state == true && record.analyte_value == 0)
                        {
                            throw new Exception("Analyte Value can't be zero if Sampling Template as Stock State.");
                        }
                    }
                    record.InjectFrom(e);
                    record.modified_by = CurrentUserContext.AppUserId;
                    record.modified_on = DateTime.Now;
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    if(temp_analyte.coa_display != null || temp_analyte.trace_elements != null)
                        throw new Exception("COA Result Doesn't Exist");
                }
                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                logger.Error(ex.ToString());
                return BadRequest(ex.Message);
            }
            return Ok(specs);
        }

        [HttpDelete("PSDeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ProductSpecificationDeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");
            var success = false;
            quality_sampling qs = null;
            quality_sampling_analyte record = null;

            try
            {
                record = dbContext.quality_sampling_analyte
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    qs = dbContext.quality_sampling.Where(x => x.id == record.quality_sampling_id).FirstOrDefault();

                    dbContext.quality_sampling_analyte.Remove(record);
                    await dbContext.SaveChangesAsync();
                    success = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
            /*
            if (success && qs != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.quality_sampling();
                    _record.InjectFrom(qs);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.QualitySampling.UpdateStockStateAnalyte(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok();
        }

        [HttpGet("PriceIndex")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> PriceIndex(string salesInvoiceId, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_price_index_history
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
                    loadOptions);
                //return await DataSourceLoader.LoadAsync(
                //    dbContext.vw_sales_invoice_attachment
                //        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.sales_invoice_id == salesInvoiceId),
                //    loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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

        [HttpGet("BySalesInvoiceId/{Id}")]
        public async Task<object> BySalesInvoiceId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.sales_invoice_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.sales_invoice_id == Id),
                    loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

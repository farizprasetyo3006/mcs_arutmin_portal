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
using Common;
using NPOI.SS.Formula.Functions;
using DocumentFormat.OpenXml.Drawing;

namespace MCSWebApp.Controllers.API.StockpileManagement
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class BlendingPlanProductController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public BlendingPlanProductController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }
    
        [HttpGet("ByBlendingPlanId/{Id}")]
        public async Task<object> ByBlendingPlanId(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_blending_plan_product
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.blending_plan_id == Id),
                loadOptions);
        }

        [HttpGet("GetTotalAnalyteByBlendingPlanId/{Id}")]
        public async Task<IActionResult> GetTotalAnalyteByBlendingPlanId(string Id)
        {
            try
            {
                var record = await dbContext.vw_blending_plan
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                        && o.id == Id)
                    .FirstOrDefaultAsync();

                return Ok(record);
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

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_blending_plan_product
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_blending_plan_product.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            
            var tbl = new blending_plan_product();
            JsonConvert.PopulateObject(values, tbl);
            var tx = await dbContext.Database.BeginTransactionAsync();
            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(blending_plan_product),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new blending_plan_product();
                    JsonConvert.PopulateObject(values, record);

                    var a = dbContext.vw_blending_plan_source
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.blending_plan_id == record.blending_plan_id);

                    if (!a.Any())
                    {
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

                        dbContext.blending_plan_product.Add(record);
                        await dbContext.SaveChangesAsync();

                        await tx.CommitAsync();

                        return Ok(record);
                    }
                    else
                    {
                        return BadRequest("Action Prevented. You already have a data in other tab.");
                    }
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
        }

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            //--- check qty input
            var editData = new blending_plan_product();
            JsonConvert.PopulateObject(values, editData);

            /*var blending_plan_id = dbContext.blending_plan_source
                .Where(o => o.id == key).FirstOrDefault().blending_plan_id;
            if (blending_plan_id == null) blending_plan_id = "";

            decimal headerQty = 0;
            var header = dbContext.blending_plan
                .Where(o => o.id == blending_plan_id).FirstOrDefault();
            if (header != null) headerQty = (decimal)header.unloading_quantity;

            decimal detailQty = dbContext.blending_plan_source
                .Where(o => o.blending_plan_id == blending_plan_id && o.id != key)
                .Sum(o => o.loading_quantity);

            decimal maxQty = headerQty - detailQty;

            if (editData.loading_quantity > maxQty)
                return BadRequest("Maximum Quantity is " + maxQty + ".");*/
            //------

            var tx = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var record = dbContext.blending_plan_product
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
                else
                {
                    await tx.RollbackAsync();
                    return BadRequest("No default organization");
                }
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                logger.Error(ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var record = dbContext.blending_plan_product
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.blending_plan_product.Remove(record);
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

        [HttpGet("ProductIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProductIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                //var lookup = dbContext.vw_stock_location
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                //    .Select(o => new { Value = o.id, Text = o.business_area_name + " > " + o.stock_location_name });
                var lookup = dbContext.vw_product
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.product_name});
                //var lookup = mine_location.Union(stockpile_location).Union(port_location)
                    //.OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        [HttpGet("SourceLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SourceLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                //var lookup = dbContext.vw_stock_location
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                //    .Select(o => new { Value = o.id, Text = o.business_area_name + " > " + o.stock_location_name });
                var mine_location = dbContext.vw_mine_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.business_area_name + " > " + o.stock_location_name });
                var stockpile_location = dbContext.vw_stockpile_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.business_area_name + " > " + o.stock_location_name });
                var port_location = dbContext.vw_port_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.business_area_name + " > " + o.stock_location_name });

                var lookup = mine_location.Union(stockpile_location).Union(port_location)
                    .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("AnalyteBySourceLocationId")]
        public async Task<mine_location_quality> AnalyteBySourceLocationId(string Id, DataSourceLoadOptions loadOptions)
        {
            decimal tm = 0, ts = 0, ash = 0, im = 0, vm = 0, fc = 0, gcv_arb = 0, gcv_adb = 0;

            var stockpile_summary = await dbContext.vw_stockpile_summary
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.stock_location_id == Id)
                .FirstOrDefaultAsync();

            //dynamic stockpile_summary;

            //var record = dbContext.organization
            //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
            //    .FirstOrDefault();
            //if (record != null && record.organization_code == "KM01")
            //{
            //    stockpile_summary = await dbContext.vw_stockpile_summary_kmia
            //        .Where(o => o.stock_location_id == Id)
            //        .FirstOrDefaultAsync();
            //}
            //else
            //{
            //    stockpile_summary = await dbContext.vw_stockpile_summary
            //        .Where(o => o.stock_location_id == Id)
            //        .FirstOrDefaultAsync();
            //}

            var quality = new mine_location_quality();

            if (stockpile_summary != null)
            {
                quality.tm = (decimal)stockpile_summary.a_tm;
                quality.ts = (decimal)stockpile_summary.a_sulfur;
                quality.ash = (decimal)stockpile_summary.a_ash;
                quality.im = (decimal)stockpile_summary.a_im;
                quality.vm = (decimal)stockpile_summary.a_vm;
                quality.fc = (decimal)stockpile_summary.a_fc;
                quality.gcv_ar = (decimal)stockpile_summary.a_gcv1;
                quality.gcv_adb = (decimal)stockpile_summary.a_gcv2;
            }
            else
            {
                quality.tm = tm;
                quality.ts = ts;
                quality.ash = ash;
                quality.im = im;
                quality.vm = vm;
                quality.fc = fc;
                quality.gcv_ar = gcv_arb;
                quality.gcv_adb = gcv_adb;
            }

            return quality;
        }

        [HttpGet("SurveyIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SurveyIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.survey
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && (o.is_draft_survey == null || o.is_draft_survey == false))
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.survey_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("SamplingNumberBySourceLocationId")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SamplingNumberBySourceLocationId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_quality_sampling
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.sampling_type_name.ToUpper() == "CHANNEL SAMPLING"
                        && o.stock_location_id == Id)
                    .Select(o => new { Value = o.sampling_number, Text = o.sampling_number + 
                        (o.despatch_order_number != null ? " - DO. " + o.despatch_order_number : "") }).Distinct();
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("AnalyteByProductId")]
        public async Task<mine_location_quality> AnalyteByProductId(string Id, DataSourceLoadOptions loadOptions)
        {
            decimal tm = 0, ts = 0, ash = 0, im = 0, vm = 0, fc = 0, gcv_arb = 0, gcv_adb = 0, rd = 0, rdi = 0, hgi = 0;
            try
            {
            var analyte = await dbContext.vw_product_specification
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.product_id == Id)
                .ToListAsync();
            
            foreach (var d in analyte)
            {
                    decimal nilai = (decimal)d.target_value;
                    /*decimal nilai = 0;
                    if (d.maximum_value == null && d.minimum_value != null)
                    {
                        nilai = (decimal)d.minimum_value;
                    }
                    else if(d.minimum_value == null && d.maximum_value != null){
                        nilai = (decimal)d.maximum_value;
                    }
                    else
                    {
                        nilai = 0;
                    }*/
                    var symbol = d.analyte_symbol.ToUpper().Trim();
                    if (symbol == "TM (ARB)") tm = nilai;
                else if (symbol == "TS (ADB)") ts = nilai;
                else if (symbol == "ASH (ADB)") ash = nilai;
                else if (symbol == "IM (ADB)") im = nilai;
                else if (symbol == "VM (ADB)") vm = nilai;
                else if (symbol == "FC (ADB)") fc = nilai;
                else if (symbol == "GCV (ARB)") gcv_arb = nilai;
                else if (symbol == "GCV (ADB)") gcv_adb = nilai;
                else if (symbol == "RD") rd = nilai;
                else if (symbol == "RDI") rdi = nilai;
                else if (symbol == "HGI") hgi = nilai;
                }

            var quality = new mine_location_quality();
            quality.id = Id;
            quality.tm = tm;
            quality.ts = ts;
            quality.ash = ash;
            quality.im = im;
            quality.vm = vm;
            quality.fc = fc;
            quality.gcv_ar = gcv_arb;
            quality.gcv_adb = gcv_adb;
            quality.rd = rd;
            quality.rdi = rdi;
            quality.hgi = hgi;
                //return await DataSourceLoader.ToList(quality, loadOptions);
                return quality;
            }
            catch(Exception)
            {
                Console.WriteLine($"An error occurred: gabisa coi");
                return null;    
            }
        }
        [HttpGet("AnalyteBySamplingNumber")]
        public async Task<mine_location_quality> AnalyteBySamplingNumber(string SamplingNumber, DataSourceLoadOptions loadOptions)
        {
            decimal tm = 0, ts = 0, ash = 0, im = 0, vm = 0, fc = 0, gcv_arb = 0, gcv_adb = 0;

            var analyte = await dbContext.vw_quality_sampling_analyte
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.sampling_number == SamplingNumber)
                .ToListAsync();
            foreach (var d in analyte)
            {
                var symbol = d.analyte_symbol.ToUpper().Trim();
                if (symbol == "TM (ARB)") tm = (decimal)d.analyte_value;
                else if (symbol == "TS (ADB)") ts = (decimal)d.analyte_value;
                else if (symbol == "ASH (ADB)") ash = (decimal)d.analyte_value;
                else if (symbol == "IM (ADB)") im = (decimal)d.analyte_value;
                else if (symbol == "VM (ADB)") vm = (decimal)d.analyte_value;
                else if (symbol == "FC (ADB)") fc = (decimal)d.analyte_value;
                else if (symbol == "GCV (ARB)") gcv_arb = (decimal)d.analyte_value;
                else if (symbol == "GCV (ADB)") gcv_adb = (decimal)d.analyte_value;
            }

            var quality = new mine_location_quality();
            quality.tm = tm;
            quality.ts = ts;
            quality.ash = ash;
            quality.im = im;
            quality.vm = vm;
            quality.fc = fc;
            quality.gcv_ar = gcv_arb;
            quality.gcv_adb = gcv_adb;

            return quality;
        }

        [HttpGet("TransactionNumberLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> TransactionNumberLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.blending_plan
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.planning_category.ToUpper() == "IKH PIT")
                    .Select(o => new { Value = o.id, Text = o.transaction_number })
                    .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}

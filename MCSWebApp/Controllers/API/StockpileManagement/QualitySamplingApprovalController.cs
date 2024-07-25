using BusinessLogic;
using BusinessLogic.Entity;
using DataAccess.DTO;
using DataAccess.EFCore.Repository;
using DataAccess.Select2;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Omu.ValueInjecter;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;

namespace MCSWebApp.Controllers.API.StockpileManagement
{
    [Route("api/StockpileManagement/[controller]")]
    [ApiController]
    public class QualitySamplingApprovalController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public QualitySamplingApprovalController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
                    loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling
                .Where(o =>
                    o.sampling_datetime >= dt1
                    && o.sampling_datetime <= dt2
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}/{samplingTypeId}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, string samplingTypeId, DataSourceLoadOptions loadOptions)
        {
            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
                    loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            var vw_quality_sampling = dbContext.vw_quality_sampling
                .Where(o =>
                    o.sampling_datetime >= dt1
                    && o.sampling_datetime <= dt2
                    && o.organization_id == CurrentUserContext.OrganizationId);
            if (samplingTypeId != null)
            {
                vw_quality_sampling = vw_quality_sampling.Where(o => o.sampling_type_id == samplingTypeId);
            }

            return await DataSourceLoader.LoadAsync(vw_quality_sampling, loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling
                .Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            quality_sampling record = null;
            var success = false;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(quality_sampling),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new quality_sampling();
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

                        dbContext.quality_sampling.Add(record);
                        await dbContext.SaveChangesAsync();

                        #region Insert quality_sampling analytes

                        if (!string.IsNullOrEmpty(record.sampling_template_id))
                        {
                            var st = dbContext.sampling_template
                                .Where(o => o.id == record.sampling_template_id)
                                .FirstOrDefault();
                            if (st != null)
                            {
                                if ((st.is_despatch_order_required ?? false) && string.IsNullOrEmpty(record.despatch_order_id))
                                {
                                    return BadRequest($"Shipping Order is required on sampling template {st.sampling_template_name}. Please select Shipping Order.");
                                }

                                var details = dbContext.sampling_template_detail
                                    .Where(o => o.sampling_template_id == st.id)
                                    .OrderBy(o => o.order)
                                    .ToList();
                                if (details != null && details.Count > 0)
                                {
                                    foreach (var d in details)
                                    {
                                        dbContext.quality_sampling_analyte.Add(new quality_sampling_analyte()
                                        {
                                            id = Guid.NewGuid().ToString("N"),
                                            created_by = CurrentUserContext.AppUserId,
                                            created_on = DateTime.Now,
                                            owner_id = CurrentUserContext.AppUserId,
                                            organization_id = CurrentUserContext.OrganizationId,
                                            quality_sampling_id = record.id,
                                            analyte_id = d.analyte_id,
                                            uom_id = d.uom_id,
                                            order = d.order
                                        });
                                    }

                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }

                        #endregion

                        //*** tambah di Mine Location tab Quality
                        var stockLocationId = record.stock_location_id;
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
                                if (symbol == "TM (AR)") tm = (decimal)d.analyte_value;
                                else if (symbol == "TS (ADB)") ts = (decimal)d.analyte_value;
                                else if (symbol == "ASH (ADB)") ash = (decimal)d.analyte_value;
                                else if (symbol == "IM (ADB)") im = (decimal)d.analyte_value;
                                else if (symbol == "VM (ADB)") vm = (decimal)d.analyte_value;
                                else if (symbol == "FC (ADB)") fc = (decimal)d.analyte_value;
                                else if (symbol == "GCV (AR)" || symbol == "GCV (ARB)") gcv_arb = (decimal)d.analyte_value;
                                else if (symbol == "GCV (ADB)") gcv_adb = (decimal)d.analyte_value;
                            }

                            var newRec = new mine_location_quality();

                            newRec.id = Guid.NewGuid().ToString("N");
                            newRec.created_by = CurrentUserContext.AppUserId;
                            newRec.created_on = DateTime.Now;
                            newRec.modified_by = null;
                            newRec.modified_on = null;
                            newRec.is_active = true;
                            newRec.is_default = null;
                            newRec.is_locked = null;
                            newRec.entity_id = null;
                            newRec.owner_id = CurrentUserContext.AppUserId;
                            newRec.organization_id = CurrentUserContext.OrganizationId;

                            newRec.mine_location_id = stockLocationId;
                            newRec.sampling_datetime = record.sampling_datetime;
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
                        success = true;                        
                    }
                    else
                    {
                        logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }

            return Ok(record);
        }

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            quality_sampling record = null;
            var success = false;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.quality_sampling
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

                            var st = dbContext.sampling_template
                                .Where(o => o.id == record.sampling_template_id)
                                .FirstOrDefault();
                            if (st != null)
                            {
                                if ((st.is_despatch_order_required ?? false) && string.IsNullOrEmpty(record.despatch_order_id))
                                {
                                    return BadRequest($"Shipping Order is required on sampling template {st.sampling_template_name}. Please select Shipping Order.");
                                }
                            }

                            await dbContext.SaveChangesAsync();

                            //if (!string.IsNullOrEmpty(record.sampling_template_id))
                            //{
                            //    var details = dbContext.sampling_template_detail
                            //        .FromSqlRaw(" SELECT * FROM sampling_template_detail "
                            //            + $" WHERE sampling_template_id = {record.sampling_template_id} "
                            //            + " AND analyte_id NOT IN ( "
                            //            + " SELECT analyte_id FROM survey_analyte "
                            //            + $" WHERE quality_sampling_id = {record.id} "
                            //            + " ) ")
                            //        .ToList();
                            //    if (details != null && details.Count > 0)
                            //    {
                            //        foreach (var d in details)
                            //        {
                            //            var sa = await dbContext.quality_sampling_analyte
                            //                .Where(o => o.analyte_id == d.analyte_id)
                            //                .FirstOrDefaultAsync();
                            //            if (sa == null)
                            //            {
                            //                dbContext.quality_sampling_analyte.Add(
                            //                    new quality_sampling_analyte()
                            //                {
                            //                    id = Guid.NewGuid().ToString("N"),
                            //                    created_by = CurrentUserContext.AppUserId,
                            //                    created_on = DateTime.Now,
                            //                    owner_id = CurrentUserContext.AppUserId,
                            //                    organization_id = CurrentUserContext.OrganizationId,
                            //                    quality_sampling_id = record.id,
                            //                    analyte_id = d.analyte_id
                            //                });
                            //            }
                            //        }

                            //        await dbContext.SaveChangesAsync();
                            //    }
                            //}

                            var stockLocationId = record.stock_location_id;
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
                                    if (symbol == "TM (AR)") tm = (decimal)d.analyte_value;
                                    else if (symbol == "TS (ADB)") ts = (decimal)d.analyte_value;
                                    else if (symbol == "ASH (ADB)") ash = (decimal)d.analyte_value;
                                    else if (symbol == "IM (ADB)") im = (decimal)d.analyte_value;
                                    else if (symbol == "VM (ADB)") vm = (decimal)d.analyte_value;
                                    else if (symbol == "FC (ADB)") fc = (decimal)d.analyte_value;
                                    else if (symbol == "GCV (AR)" || symbol == "GCV (ARB)") gcv_arb = (decimal)d.analyte_value;
                                    else if (symbol == "GCV (ADB)") gcv_adb = (decimal)d.analyte_value;
                                }

                                var MLQ = dbContext.mine_location_quality
                                    .Where(o => o.mine_location_id == stockLocationId
                                        && o.sampling_datetime == record.sampling_datetime).FirstOrDefault();
                                if (MLQ == null)
                                {
                                    var newRec = new mine_location_quality();

                                    newRec.id = Guid.NewGuid().ToString("N");
                                    newRec.created_by = CurrentUserContext.AppUserId;
                                    newRec.created_on = DateTime.Now;
                                    newRec.modified_by = null;
                                    newRec.modified_on = null;
                                    newRec.is_active = true;
                                    newRec.is_default = null;
                                    newRec.is_locked = null;
                                    newRec.entity_id = null;
                                    newRec.owner_id = CurrentUserContext.AppUserId;
                                    newRec.organization_id = CurrentUserContext.OrganizationId;

                                    newRec.mine_location_id = stockLocationId;
                                    newRec.sampling_datetime = record.sampling_datetime;
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
                                else
                                {
                                    MLQ.modified_by = CurrentUserContext.AppUserId;
                                    MLQ.modified_on = DateTime.Now;

                                    MLQ.tm = tm;
                                    MLQ.ts = ts;
                                    MLQ.ash = ash;
                                    MLQ.im = im;
                                    MLQ.vm = vm;
                                    MLQ.fc = fc;
                                    MLQ.gcv_ar = gcv_arb;
                                    MLQ.gcv_adb = gcv_adb;

                                    await dbContext.SaveChangesAsync();
                                }
                            }

                            await tx.CommitAsync();
                            success = true;                            
                        }
                        else
                        {
                            logger.Debug("User is not authorized.");
                            return Unauthorized();
                        }
                    }
                    else
                    {
                        logger.Debug("Record is not found.");
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }

            return Ok(record);
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");
            quality_sampling record = null;
            var success = false;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.quality_sampling
                        .Where(o => o.id == key
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (!string.IsNullOrEmpty(record.despatch_order_id))
                        {
                            return BadRequest("Can not be deleted since Shipping Order is not empty.");
                        }

                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.quality_sampling.Remove(record);
                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            success = true;
                        }
                        else
                        {
                            logger.Debug("User is not authorized.");
                            return Unauthorized();
                        }
                    }
                    else
                    {
                        logger.Debug("Record is not found.");
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }

            return Ok();
        }

        [HttpGet("StockpileLocationIdLookup")]
        public async Task<object> StockpileLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var stockpile = dbContext.vw_stockpile_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {   value = o.id,
                            text =  (o.business_area_name != null) ?  o.business_area_name + " > " + o.stock_location_name :  o.stock_location_name,
                            index = 0
                        });
                var ports = dbContext.vw_port_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {   value = o.id,
                            text = (o.business_area_name != null) ? o.business_area_name + " > " + o.stock_location_name : o.stock_location_name,
                            index = 1
                        });
                var mine_location = dbContext.vw_mine_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {   value = o.id,
                            text = (o.business_area_name != null) ? o.business_area_name + " > " + o.stock_location_name : o.stock_location_name,
                            index = 2
                        });
                var barges = dbContext.barge
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { value = o.id, text = "Barge > "  +  o.vehicle_name, index = 3 });
                var vessels = dbContext.vessel
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { value = o.id, text = "Vessel > " +  o.vehicle_name, index = 4 });
                var lookup = stockpile.Union(ports).Union(barges).Union(vessels).Union(mine_location)
                                .OrderBy(o => o.index).ThenBy(o => o.text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("SamplingTemplateIdLookup")]
        public async Task<object> SamplingTemplateIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.sampling_template
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sampling_template_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("SamplingTypeIdLookup")]
        public object SamplingTypeIdLookup()
        {
            try
            {
                var lookup = dbContext.sampling_type
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sampling_type_name })
                    .ToArray();
                return lookup;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SurveyorIdLookup")]
        public async Task<object> SurveyorIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.contractor
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.is_surveyor == true)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.business_partner_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("QualitySamplingIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> QualitySamplingIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.quality_sampling
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sampling_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.vw_quality_sampling
                    .Where(o => o.id == Id
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefaultAsync();
                return Ok(record);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] survey Record)
        {
            try
            {
                var record = dbContext.quality_sampling
                    .Where(o => o.id == Record.id
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    var e = new entity();
                    e.InjectFrom(record);
                    record.InjectFrom(Record);
                    record.InjectFrom(e);
                    record.modified_by = CurrentUserContext.AppUserId;
                    record.modified_on = DateTime.Now;

                    await dbContext.SaveChangesAsync();
                    return Ok(record);
                }
                else
                {
                    record = new quality_sampling();
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

                    dbContext.quality_sampling.Add(record);
                    await dbContext.SaveChangesAsync();

                    return Ok(record);
                }
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpDelete("DeleteById/{Id}")]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Debug($"string Id = {Id}");

            try
            {
                var record = dbContext.quality_sampling
                    .Where(o => o.id == Id
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.quality_sampling.Remove(record);
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

        [HttpPost("ApplyToTransactions")]
        public async Task<IActionResult> ApplyToTransactions([FromBody] dynamic Data)
        {
            var result = new StandardResult();

            try
            {
                if (Data != null && Data.id != null && Data.production_ids != null)
                {
                    var category = (string)Data.category;
                    var id = (string)Data.id;
                    logger.Debug($"Category = {category}");
                    logger.Debug($"Quality sampling id = {id}");
                    logger.Debug($"Production tx id = {(string)Data.production_ids}");

                    var production_ids = ((string)Data.production_ids)
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    var qs = new BusinessLogic.Entity.QualitySampling(CurrentUserContext);
                    result = await qs.ApplyToTransactions(category, id, production_ids);
                    logger.Debug($"{JsonConvert.SerializeObject(result)}");
                }
                else
                {
                    result.Message = "Invalid data.";
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return new JsonResult(result);
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
            if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);

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

            string teks = "";
            bool gagal = false; string errormessage = "";

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var surveyor_id = "";
                    var surveyor = dbContext.contractor
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.business_partner_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(2)).Trim())
                        .FirstOrDefault();
                    if (surveyor != null) surveyor_id = surveyor.id.ToString();

                    var stock_location_id = "";
                    var stock_location = dbContext.stockpile_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.stockpile_location_code == PublicFunctions.IsNullCell(row.GetCell(3))).FirstOrDefault();
                    if (stock_location != null)
                        stock_location_id = stock_location.id.ToString();
                    else
                    {
                        var barges = dbContext.barge
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.vehicle_name.Trim().ToUpper() == PublicFunctions.IsNullCell(row.GetCell(3)).Trim().ToUpper())
                            .FirstOrDefault();
                        if (barges != null)
                            stock_location_id = barges.id.ToString();
                        else
                        {
                            var vessels = dbContext.vessel
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.vehicle_name.Trim().ToUpper() == PublicFunctions.IsNullCell(row.GetCell(3)).Trim().ToUpper())
                                .FirstOrDefault();
                            if (vessels != null)
                                stock_location_id = vessels.id.ToString();
                            else
                            {
                                var port_location = dbContext.port_location
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                        o.port_location_code == PublicFunctions.IsNullCell(row.GetCell(3))).FirstOrDefault();
                                if (port_location != null)
                                    stock_location_id = port_location.id.ToString();
                                else
                                {
                                    var mine_location = dbContext.mine_location
                                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.mine_location_code == PublicFunctions.IsNullCell(row.GetCell(3))).FirstOrDefault();
                                    if (mine_location != null)
                                        stock_location_id = mine_location.id.ToString();
                                }
                            }
                        }
                    }

                    var product_id = "";
                    var product = dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.product_code == PublicFunctions.IsNullCell(row.GetCell(4))).FirstOrDefault();
                    if (product != null) product_id = product.id.ToString();

                    var sampling_template_id = "";
                    var sampling_template = dbContext.sampling_template
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.sampling_template_code == PublicFunctions.IsNullCell(row.GetCell(5))).FirstOrDefault();
                    if (sampling_template != null) sampling_template_id = sampling_template.id.ToString();

                    var despatch_order_id = "";
                    var despatch_order = dbContext.despatch_order
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.despatch_order_number.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(6)).Trim().ToLower())
                        .FirstOrDefault();
                    if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();

                    //var loading_type_id = "";
                    //var master_list = dbContext.master_list
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                    //        o.item_group == "loading-type" &&
                    //        o.item_in_coding.Trim() == PublicFunctions.IsNullCell(row.GetCell(7)).Trim())
                    //    .FirstOrDefault();
                    //if (master_list != null) loading_type_id = master_list.id.ToString();

                    var sampling_type_id = "";
                    //var master_list = dbContext.master_list
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                    //        o.item_group == "sampling-type" &&
                    //        o.item_in_coding.Trim() == PublicFunctions.IsNullCell(row.GetCell(8)).Trim())
                    //    .FirstOrDefault();
                    var sampling_type = dbContext.sampling_type
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sampling_type_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(8)).Trim())
                        .FirstOrDefault();
                    if (sampling_type != null) sampling_type_id = sampling_type.id.ToString();

                    var shift_id = "";
                    var shift = dbContext.shift
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.shift_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(9)).Trim())
                        .FirstOrDefault();
                    if (shift != null) shift_id = shift.id.ToString();

                    var record = dbContext.quality_sampling
                        .Where(o => o.sampling_number == PublicFunctions.IsNullCell(row.GetCell(0))
							&& o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.sampling_datetime = PublicFunctions.Waktu(row.GetCell(1));
                        record.surveyor_id = surveyor_id;
                        record.stock_location_id = stock_location_id;
                        record.product_id = product_id;
                        record.sampling_template_id = sampling_template_id;
                        record.despatch_order_id = despatch_order_id;
                        record.is_adjust = PublicFunctions.BenarSalah(row.GetCell(8));
                        record.sampling_type_id = sampling_type_id;
                        record.shift_id = shift_id;
                        record.is_draft = PublicFunctions.BenarSalah(row.GetCell(10));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new quality_sampling();
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

                        record.sampling_number = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.sampling_datetime = PublicFunctions.Waktu(row.GetCell(1));
                        record.surveyor_id = surveyor_id;
                        record.stock_location_id = stock_location_id;
                        record.product_id = product_id;
                        record.sampling_template_id = sampling_template_id;
                        record.despatch_order_id = despatch_order_id;
                        record.is_adjust = PublicFunctions.BenarSalah(row.GetCell(8));
                        record.sampling_type_id = sampling_type_id;
                        record.shift_id = shift_id;
                        record.is_draft = PublicFunctions.BenarSalah(row.GetCell(10));

                        dbContext.quality_sampling.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 1, Line " + (i+1) + " : " + Environment.NewLine;
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

                    var quality_sampling_id = "";
                    var quality_sampling = dbContext.quality_sampling
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sampling_number.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).Trim().ToLower())
                        .FirstOrDefault();
                    if (quality_sampling != null) quality_sampling_id = quality_sampling.id.ToString();

                    var analyte_id = "";
                    var analyte = dbContext.analyte
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.analyte_symbol.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).Trim().ToLower())
                        .FirstOrDefault();
                    if (analyte != null) analyte_id = analyte.id.ToString();

                    var uom_id = "";
                    var uom = dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.uom_symbol.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).Trim().ToLower())
                        .FirstOrDefault();
                    if (uom != null) uom_id = uom.id.ToString();

                    var record = dbContext.quality_sampling_analyte
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.quality_sampling_id == quality_sampling_id
                            && o.analyte_id == analyte_id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        //record.analyte_id = analyte_id;
                        record.uom_id = uom_id;
                        record.analyte_value = PublicFunctions.Desimal(row.GetCell(3));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new quality_sampling_analyte();
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

                        record.quality_sampling_id = quality_sampling_id;
                        record.analyte_id = analyte_id;
                        record.uom_id = uom_id;
                        record.analyte_value = PublicFunctions.Desimal(row.GetCell(3));

                        dbContext.quality_sampling_analyte.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 2, Line " + (i+1) + " : " + Environment.NewLine;
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
                await transaction.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "QualitySampling");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpGet("select2")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Select2([FromQuery] string q)
        {
            var result = new Select2Response();

            try
            {
                var s2Request = new Select2Request()
                {
                    q = q
                };
                if (s2Request != null)
                {
                    var svc = new QualitySampling(CurrentUserContext);
                    result = await svc.Select2(s2Request, "sampling_number");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return new JsonResult(result);
        }

        [HttpGet("GetQualitySamplingApproval/{Id}")]
        public object GetQualitySamplingApproval(string Id)
        {
            try
            {
                var result = dbContext.quality_sampling.Where(o => o.id == Id).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("GiveApproval")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GiveApproval([FromForm] string key, [FromForm] string values)
        {
            try
            {
                var record = dbContext.quality_sampling_approval
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                        && o.quality_sampling_id == key)
                    .FirstOrDefault();
                if (await mcsContext.CanCreate(dbContext, nameof(quality_sampling_approval),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var newRec = new quality_sampling_approval();
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

                    newRec.quality_sampling_id = key;
                    newRec.approved_on = System.DateTime.Now;
                    newRec.approved_by_id = CurrentUserContext.AppUserId;

                    dbContext.quality_sampling_approval.Add(newRec);
                    await dbContext.SaveChangesAsync();
                    return Ok(newRec);
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

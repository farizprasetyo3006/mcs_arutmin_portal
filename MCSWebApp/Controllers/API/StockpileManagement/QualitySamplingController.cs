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
using DevExtreme.AspNet.Mvc.Builders;
using DocumentFormat.OpenXml.InkML;

namespace MCSWebApp.Controllers.API.StockpileManagement
{
    [Route("api/StockpileManagement/[controller]")]
    [ApiController]
    public class QualitySamplingController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public QualitySamplingController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            //&& CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                            //    || CurrentUserContext.IsSysAdmin
                            ),
                    loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling
                .Where(o =>
                    o.sampling_datetime >= dt1
                    && o.sampling_datetime <= dt2
                    && o.organization_id == CurrentUserContext.OrganizationId
                            //&& (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                            //    || CurrentUserContext.IsSysAdmin)
                            ),
                loadOptions);
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}/{samplingTypeId}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, string samplingTypeId, DataSourceLoadOptions loadOptions)
        {
            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            //&& CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                            //    || CurrentUserContext.IsSysAdmin
                            ),
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
        [HttpGet("DataGrids/{sampId}")]
        public async Task<object> DataGrids(string sampId, DataSourceLoadOptions loadOptions)
        {
            var vw_quality_sampling = dbContext.vw_quality_sampling
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId);
            if (sampId != null)
            {
                vw_quality_sampling = vw_quality_sampling.Where(o => o.id == sampId);
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

        [HttpGet("DataDetailByDespatchOrder")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetailByDespatchOrder(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling
                .Where(o => o.despatch_order_id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            quality_sampling record = null;
            //var success = false;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(quality_sampling),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new quality_sampling();
                        JsonConvert.PopulateObject(values, record);

                        //if (!string.IsNullOrEmpty(record.despatch_order_id) && record.non_commercial != true)
                        //{
                        //    var tempQualitySampling = dbContext.quality_sampling
                        //        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        //            && o.despatch_order_id == record.despatch_order_id);
                        //    if (tempQualitySampling != null)
                        //    {
                        //        int recCount = tempQualitySampling.Count();

                        //        var DO = dbContext.vw_despatch_order
                        //            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        //                && o.id == record.despatch_order_id)
                        //            .FirstOrDefault();
                        //        string term = DO?.delivery_term_name?.Substring(0, 3) ?? "";
                        //        if (term == "CIF")
                        //        {
                        //            if (recCount > 1)
                        //                return BadRequest($"This Shipping Order Number can not be used in more than 2 Quality Sampling.");
                        //        }
                        //        else
                        //            if (recCount > 0)
                        //                return BadRequest($"This Shipping Order Number has been used in other Quality Sampling. Can't use this Shipping Order Number anymore.");
                        //    }

                        //    //var tempQualitySampling = dbContext.quality_sampling
                        //    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        //    //        && o.despatch_order_id == record.despatch_order_id
                        //    //        && o.non_commercial != true)
                        //    //    .FirstOrDefault();
                        //    //if (tempQualitySampling != null)
                        //    //{
                        //    //    return BadRequest($"This Shipping Order Number has been used in other Quality Sampling. Can't use this Shipping Order Number anymore.");
                        //    //}
                        //}

                        //var cekDup = dbContext.vw_quality_sampling
                        //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        //        && o.sampling_number == record.sampling_number
                        //        && !o.sampling_type_name.ToUpper().Contains("CHANEL")
                        //        && !o.sampling_type_name.ToUpper().Contains("CHANNEL"))
                        //    .FirstOrDefault();
                        //if (cekDup != null)
                        //{
                        //    return BadRequest($"Can not use Sampling Number more than once for Sampling Type other than 'Channel Sampling'.");
                        //}

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
                        //record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

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
                                if (symbol == "TM (ARB)") tm = (decimal)d.analyte_value;
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
                        //success = true;                        
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
            /*
            if (success && record != null && (record.is_adjust ?? false))
            {
                try
                {
                    var _record = new DataAccess.Repository.quality_sampling();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.QualitySampling.UpdateStockStateAnalyte(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok(record);
        }

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            quality_sampling record = null;
            //var success = false;

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

                            //record.InjectFrom(e);
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

                            //if (!string.IsNullOrEmpty(record.despatch_order_id) && record.non_commercial != true)
                            //{
                            //    var tempQualitySampling = dbContext.quality_sampling
                            //        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            //            && o.despatch_order_id == record.despatch_order_id
                            //            && o.id != record.id);
                            //    if (tempQualitySampling != null)
                            //    {
                            //        int recCount = tempQualitySampling.Count();

                            //        var DO = dbContext.vw_despatch_order
                            //            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            //                && o.id == record.despatch_order_id)
                            //            .FirstOrDefault();
                            //        string term = DO?.delivery_term_name?.Substring(0, 3) ?? "";
                            //        if (term == "CIF")
                            //        {
                            //            if (recCount > 1)
                            //                return BadRequest($"This Shipping Order Number can not be used in more than 2 Quality Sampling.");
                            //        }
                            //        else
                            //            if (recCount > 0)
                            //                return BadRequest($"This Shipping Order Number has been used in other Quality Sampling. Can't use this Shipping Order Number anymore.");
                            //    }

                            //    //var tempQualitySampling = dbContext.quality_sampling
                            //    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            //    //        && o.despatch_order_id == record.despatch_order_id
                            //    //        && o.non_commercial != true
                            //    //        && o.id != record.id)
                            //    //    .FirstOrDefault();
                            //    //if (tempQualitySampling != null)
                            //    //{
                            //    //    return BadRequest($"This Shipping Order Number has been used in other Quality Sampling. Can't use this Shipping Order Number anymore.");
                            //    //}
                            //}

                            //var cekDup = dbContext.vw_quality_sampling
                            //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            //        && o.sampling_number == record.sampling_number
                            //        && o.id != record.id
                            //        && !o.sampling_type_name.ToUpper().Contains("CHANEL")
                            //        && !o.sampling_type_name.ToUpper().Contains("CHANNEL"))
                            //    .FirstOrDefault();
                            //if (cekDup != null)
                            //{
                            //    return BadRequest($"Can not use Sampling Number more than once for Sampling Type other than 'Channel Sampling'.");
                            //}

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
                                var mine_location_quality = dbContext.mine_location_quality
                                    .Where(o => o.mine_location_id == stockLocationId).ToList();
                                if (mine_location_quality != null)
                                {
                                    dbContext.mine_location_quality.RemoveRange(mine_location_quality);
                                }

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
                            //success = true;                            
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
            /*
            if (success && record != null && (record.is_adjust ?? false))
            {
                try
                {
                    var _record = new DataAccess.Repository.quality_sampling();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.QualitySampling.UpdateStockStateAnalyte(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
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
            /*
            if (success && record != null && (record.is_adjust ?? false))
            {
                try
                {
                    var _record = new DataAccess.Repository.quality_sampling();
                    _record.InjectFrom(record);
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

        [HttpGet("DespatchOrderIdLookup")]
        public async Task<object> DespatchOrderIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number })
                    .Distinct();
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("StockpileLocationIdLookup")]
        public async Task<object> StockpileLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var stockpile = dbContext.vw_stockpile_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new
                    {
                        value = o.id,
                        text = (o.business_area_name != null) ? o.business_area_name + " > " + o.stock_location_name : o.stock_location_name,
                        index = 0,
                        search = (o.business_area_name != null) ? o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() : o.stock_location_name.ToLower()
                            + ((o.business_area_name != null) ? o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper() : o.stock_location_name.ToUpper())
                    });
                var ports = dbContext.vw_port_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = (o.business_area_name != null) ? o.business_area_name + " > " + o.stock_location_name : o.stock_location_name,
                            index = 1,
                            search = (o.business_area_name != null) ? o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() : o.stock_location_name.ToLower()
                            + ((o.business_area_name != null) ? o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper() : o.stock_location_name.ToUpper())
                        });
                var mine_location = dbContext.vw_mine_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = (o.business_area_name != null) ? o.business_area_name + " > " + o.stock_location_name : o.stock_location_name,
                            index = 2,
                            search = (o.business_area_name != null) ? o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() : o.stock_location_name.ToLower()
                            + ((o.business_area_name != null) ? o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper() : o.stock_location_name.ToUpper())
                        });
                var barges = dbContext.barge
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Select(o => new { value = o.id, text = "Barge > " + o.vehicle_name, index = 3, search = "barge > " + o.vehicle_name.ToLower() + "BARGE > " + o.vehicle_name.ToUpper() });
                var vessels = dbContext.vessel
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Select(o => new { value = o.id, text = "Vessel > " + o.vehicle_name, index = 4, search = "vessel > " + o.vehicle_name.ToLower() + "VESSEL > " + o.vehicle_name.ToUpper() });
                var lookup = stockpile.Union(ports).Union(vessels).Union(mine_location)
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
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sampling_template_name, search = o.sampling_template_name.ToLower() + o.sampling_template_name.ToUpper() });
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
                //var lookup = dbContext.master_list
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                //        && o.item_group == "sampling-type")
                var lookup = dbContext.sampling_type
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sampling_type_name, search = o.sampling_type_name.ToLower() + o.sampling_type_name.ToUpper() })
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
                //var lookup = dbContext.business_partner
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                //        && o.is_vendor == true)
                //    .Select(o => new { Value = o.id, Text = o.business_partner_name });

                var lookup = dbContext.contractor
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.is_surveyor == true)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.business_partner_name, search = o.business_partner_name.ToLower() + o.business_partner_name.ToUpper() });
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
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sampling_number, search = o.sampling_number.ToLower() + o.sampling_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("QualitySamplingIdLookupNoFilterWithShippingOrder")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> QualitySamplingIdLookupNoFilterWithShippingOrder(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.quality_sampling
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.despatch_order_id != null && o.despatch_order_id != "")
                    .Select(o => new { Value = o.id, Text = o.sampling_number, search = o.sampling_number.ToLower() + o.sampling_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("QualitySamplingByDo")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> QualitySamplingByDo(String Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.quality_sampling
       .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
       //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
       .Where(o => o.despatch_order_id == Id)
       .Join(
           dbContext.sampling_template,
           quality => quality.sampling_template_id,
           template => template.id,
           (quality, template) => new { Quality = quality, Template = template }
       )
       .Where(joined => joined.Template.sampling_template_code == "COA")
       .Select(joined => new { Value = joined.Quality.id, Text = joined.Quality.sampling_number, Issued_date = joined.Quality.sampling_datetime });

                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("BargingTransactionIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> BargingTransactionIdLookup(DataSourceLoadOptions loadOptions, string id)
        {
            try
            {
                var lookup = dbContext.barging_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderBy(o => o.transaction_number)
                    .Select(o => new { Value = o.id, Text = o.transaction_number, search = o.transaction_number.ToLower() + o.transaction_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("FetchAnalyteFromSamplingTemplate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ApiResponse> FetchAnalyteFromSamplingTemplate(string Id)
        {
            var result = new ApiResponse();
            result.Status.Success = true;
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    //fariz***
                    var samplingTemplateId = await dbContext.quality_sampling.Where(o => o.id == Id).Select(o => o.sampling_template_id).FirstOrDefaultAsync(); // get the template id in header data
                    var currentAnalyteList = await dbContext.quality_sampling_analyte.Where(o => o.quality_sampling_id == Id).Select(o => o.analyte_id).ToArrayAsync(); // get the list analyte in detail header

                    var detailTemplate = await dbContext.sampling_template_detail
                                        .Where(o => o.sampling_template_id == samplingTemplateId && !currentAnalyteList.Contains(o.analyte_id))
                                        .ToArrayAsync(); // get the new analyte exclduding the exist analyte
                    //end fariz***
                    foreach (var item in detailTemplate)
                    {
                        var record = new quality_sampling_analyte()
                        {
                            id = Guid.NewGuid().ToString("N"),
                            created_by = CurrentUserContext.AppUserId,
                            created_on = DateTime.Now,
                            modified_by = null,
                            modified_on = null,
                            is_active = true,
                            is_default = null,
                            is_locked = null,
                            entity_id = null,
                            owner_id = CurrentUserContext.AppUserId,
                            organization_id = CurrentUserContext.OrganizationId,

                            analyte_id = item.analyte_id,
                            uom_id = item.uom_id,
                            order = item.order,
                            quality_sampling_id = Id
                        };
                        dbContext.quality_sampling_analyte.Add(record);
                        await dbContext.SaveChangesAsync();
                    }

                    if (result.Status.Success)
                    {
                        await tx.CommitAsync();
                        result.Status.Message = "Ok";
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    result.Status.Success = false;
                    result.Status.Message = ex.InnerException?.Message ?? ex.Message;
                    return result;
                }
            }
        }

        [HttpGet("BargingTransactionIdLookupFiltered")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> BargingTransactionIdLookupFiltered(DataSourceLoadOptions loadOptions, string id)
        {
            try
            {
                var current = dbContext.quality_sampling
           .Select(o => o.barging_transaction_id)
           .ToList();

                var lookup = dbContext.barging_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => !current.Contains(o.id))
                    .OrderBy(o => o.transaction_number)
                    .Select(o => new { Value = o.id, Text = o.transaction_number, search = o.transaction_number.ToLower() + o.transaction_number.ToUpper() });


                if (id != null)
                {
                    var edit = dbContext.barging_transaction
                        .Where(o => o.id == id)
                        .OrderBy(o => o.transaction_number)
                        .Select(o => new { Value = o.id, Text = o.transaction_number, search = o.transaction_number.ToLower() + o.transaction_number.ToUpper() });

                    if (edit != null)
                    {
                        lookup = lookup.Union(edit);
                    }
                }
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
            /*
            if (result.Success)
            {
                try
                {
                    var _repo = new QualitySampling(CurrentUserContext);
                    var _record = await _repo.GetByIdAsync((string)Data.id);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.QualitySampling.UpdateStockStateAnalyte(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return new JsonResult(result);
        }

        [HttpPost("UploadDocumentIC")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UploadDocumentIC([FromBody] dynamic FileDocument)
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
                                && o.vehicle_name.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(3)).ToUpper())
                            .FirstOrDefault();
                        if (barges != null)
                            stock_location_id = barges.id.ToString();
                        else
                        {
                            var vessels = dbContext.vessel
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.vehicle_name.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(3)).ToUpper())
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

                    var sampling_type_id = ""; var sampling_type_code = "";
                    var sampling_type = dbContext.sampling_type
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sampling_type_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(8)).Trim())
                        .FirstOrDefault();
                    if (sampling_type != null)
                    {
                        sampling_type_id = sampling_type.id.ToString();
                        sampling_type_code = sampling_type.sampling_type_code.ToString();
                    }

                    var shift_id = "";
                    var shift = dbContext.shift
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.shift_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(9)).Trim())
                        .FirstOrDefault();
                    if (shift != null) shift_id = shift.id.ToString();

                    var barging_transaction_id = "";
                    var barging_transaction = dbContext.barging_transaction
                        .Where(o => o.transaction_number == PublicFunctions.IsNullCell(row.GetCell(11)).Trim())
                        .FirstOrDefault();
                    if (barging_transaction != null) barging_transaction_id = barging_transaction.id.ToString();


                    //var record = dbContext.quality_sampling
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                    //                o.sampling_number == PublicFunctions.IsNullCell(row.GetCell(0)) && 
                    //                o.sampling_template_id == sampling_template_id)
                    //    .FirstOrDefault();
                    var record = dbContext.quality_sampling
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.sampling_number == PublicFunctions.IsNullCell(row.GetCell(0)) &&
                                    o.sampling_template_id == sampling_template_id &&
                                    o.sampling_datetime == PublicFunctions.Tanggal(row.GetCell(1)))
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;


                        //record.sampling_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                        record.sampling_type_id = sampling_type_id;
                        record.surveyor_id = surveyor_id;
                        record.stock_location_id = stock_location_id;
                        record.product_id = product_id;
                        record.sampling_template_id = sampling_template_id;
                        record.despatch_order_id = despatch_order_id;
                        record.non_commercial = PublicFunctions.BenarSalah(row.GetCell(7));
                        record.shift_id = shift_id;
                        record.is_draft = PublicFunctions.BenarSalah(row.GetCell(10));
                        record.barging_transaction_id = barging_transaction_id;

                        await dbContext.SaveChangesAsync();
                        //}
                        //else if (record.sampling_datetime == PublicFunctions.Tanggal(row.GetCell(1)))
                        //{
                        //    var e = new entity();
                        //    e.InjectFrom(record);

                        //    record.InjectFrom(e);
                        //    record.modified_by = CurrentUserContext.AppUserId;
                        //    record.modified_on = DateTime.Now;
                        //    

                        //    //record.sampling_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                        //    //record.sampling_type_id = sampling_type_id;
                        //    record.surveyor_id = surveyor_id;
                        //    record.stock_location_id = stock_location_id;
                        //    record.product_id = product_id;
                        //    record.sampling_template_id = sampling_template_id;
                        //    record.despatch_order_id = despatch_order_id;
                        //    record.non_commercial = PublicFunctions.BenarSalah(row.GetCell(7));
                        //    record.shift_id = shift_id;
                        //    record.is_draft = PublicFunctions.BenarSalah(row.GetCell(10));
                        //    record.barging_transaction_id = barging_transaction_id;

                        //    await dbContext.SaveChangesAsync();
                        //}
                        //else
                        //{
                        //    record = new quality_sampling();
                        //    record.id = Guid.NewGuid().ToString("N");
                        //    record.created_by = CurrentUserContext.AppUserId;
                        //    record.created_on = DateTime.Now;
                        //    record.modified_by = null;
                        //    record.modified_on = null;
                        //    record.is_active = true;
                        //    record.is_default = null;
                        //    record.is_locked = null;
                        //    record.entity_id = null;
                        //    record.owner_id = CurrentUserContext.AppUserId;
                        //    record.organization_id = CurrentUserContext.OrganizationId;

                        //    record.sampling_number = PublicFunctions.IsNullCell(row.GetCell(0)).Trim();
                        //    record.sampling_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                        //    record.surveyor_id = surveyor_id;
                        //    record.stock_location_id = stock_location_id;
                        //    record.product_id = product_id;
                        //    record.sampling_template_id = sampling_template_id;
                        //    record.despatch_order_id = despatch_order_id;
                        //    record.non_commercial = PublicFunctions.BenarSalah(row.GetCell(7));
                        //    record.sampling_type_id = sampling_type_id;
                        //    record.shift_id = shift_id;
                        //    record.is_draft = PublicFunctions.BenarSalah(row.GetCell(10));
                        //    record.barging_transaction_id = barging_transaction_id;

                        //    dbContext.quality_sampling.Add(record);
                        //    await dbContext.SaveChangesAsync();
                        //}
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

                        record.sampling_number = PublicFunctions.IsNullCell(row.GetCell(0)).Trim();
                        record.sampling_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                        record.surveyor_id = surveyor_id;
                        record.stock_location_id = stock_location_id;
                        record.product_id = product_id;
                        record.sampling_template_id = sampling_template_id;
                        record.despatch_order_id = despatch_order_id;
                        record.non_commercial = PublicFunctions.BenarSalah(row.GetCell(7));
                        record.sampling_type_id = sampling_type_id;
                        record.shift_id = shift_id;
                        record.is_draft = PublicFunctions.BenarSalah(row.GetCell(10));
                        record.barging_transaction_id = barging_transaction_id;

                        dbContext.quality_sampling.Add(record);
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

                    var detail_sampling_template_id = "";
                    var sampling_template = dbContext.sampling_template
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sampling_template_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(2)).Trim())
                        .FirstOrDefault();
                    if (sampling_template != null) detail_sampling_template_id = sampling_template.id.ToString();

                    var quality_sampling_id = "";
                    var quality_sampling = dbContext.quality_sampling
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.sampling_number.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).Trim().ToLower() &&
                                    o.sampling_template_id == detail_sampling_template_id &&
                                    o.sampling_datetime == PublicFunctions.Tanggal(row.GetCell(1)))
                        .FirstOrDefault();
                    if (quality_sampling != null)
                        quality_sampling_id = quality_sampling.id.ToString();
                    else
                    {
                        errormessage = "Sampling Number or Sampling Template or Sampling DateTime not found.";
                        teks += errormessage + Environment.NewLine;
                        gagal = true;
                    }

                    var analyte_id = "";

                    string[] analyteSymbols = { "tm(arb)", "im(adb)", "ash(adb)", "vm(adb)", "fc(adb)", "ts(adb)", "gcv(adb)", "gcv(arb)", "ts(arb)", "ash(arb)" };

                    for (int analyteCol = 3; analyteCol <= 12; analyteCol++)
                    {
                        decimal analyteValue = 0;
                        if (PublicFunctions.IsNullCell(row.GetCell(analyteCol)) != "")
                        {
                            analyteValue = PublicFunctions.Desimal(row.GetCell(analyteCol));

                            var analyte = dbContext.analyte
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.analyte_symbol.Replace(" ", "").ToLower() == analyteSymbols[analyteCol - 3])
                                .FirstOrDefault();
                            if (analyte != null)
                            {
                                analyte_id = analyte.id.ToString();

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

                                    record.analyte_value = analyteValue;

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
                                    record.analyte_value = analyteValue;

                                    dbContext.quality_sampling_analyte.Add(record);
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 2, Line " + (i + 1) + " : " + Environment.NewLine;
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

        [HttpPost("UploadDocumentAI")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UploadDocumentAI([FromBody] dynamic FileDocument)
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
                                && o.vehicle_name.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(3)).ToUpper())
                            .FirstOrDefault();
                        if (barges != null)
                            stock_location_id = barges.id.ToString();
                        else
                        {
                            var vessels = dbContext.vessel
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.vehicle_name.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(3)).ToUpper())
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

                    var sampling_type_id = ""; var sampling_type_code = "";
                    var sampling_type = dbContext.sampling_type
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sampling_type_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(8)).Trim())
                        .FirstOrDefault();
                    if (sampling_type != null)
                    {
                        sampling_type_id = sampling_type.id.ToString();
                        sampling_type_code = sampling_type.sampling_type_code.ToString();
                    }

                    var shift_id = "";
                    var shift = dbContext.shift
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.shift_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(9)).Trim())
                        .FirstOrDefault();
                    if (shift != null) shift_id = shift.id.ToString();

                    var barging_transaction_id = "";
                    var barging_transaction = dbContext.barging_transaction
                        .Where(o => o.transaction_number == PublicFunctions.IsNullCell(row.GetCell(11)).Trim())
                        .FirstOrDefault();
                    if (barging_transaction != null) barging_transaction_id = barging_transaction.id.ToString();


                    var record = dbContext.quality_sampling
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.sampling_number == PublicFunctions.IsNullCell(row.GetCell(0)) &&
                                    o.sampling_template_id == sampling_template_id &&
                                    o.sampling_datetime == PublicFunctions.Tanggal(row.GetCell(1)))
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;


                        //record.sampling_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                        record.sampling_type_id = sampling_type_id;
                        record.surveyor_id = surveyor_id;
                        record.stock_location_id = stock_location_id;
                        record.product_id = product_id;
                        record.sampling_template_id = sampling_template_id;
                        record.despatch_order_id = despatch_order_id;
                        record.non_commercial = PublicFunctions.BenarSalah(row.GetCell(7));
                        record.shift_id = shift_id;
                        record.is_draft = PublicFunctions.BenarSalah(row.GetCell(10));
                        record.barging_transaction_id = barging_transaction_id;

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

                        record.sampling_number = PublicFunctions.IsNullCell(row.GetCell(0)).Trim();
                        record.sampling_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                        record.surveyor_id = surveyor_id;
                        record.stock_location_id = stock_location_id;
                        record.product_id = product_id;
                        record.sampling_template_id = sampling_template_id;
                        record.despatch_order_id = despatch_order_id;
                        record.non_commercial = PublicFunctions.BenarSalah(row.GetCell(7));
                        record.sampling_type_id = sampling_type_id;
                        record.shift_id = shift_id;
                        record.is_draft = PublicFunctions.BenarSalah(row.GetCell(10));
                        record.barging_transaction_id = barging_transaction_id;

                        dbContext.quality_sampling.Add(record);
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

                    var detail_sampling_template_id = "";
                    var sampling_template = dbContext.sampling_template
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sampling_template_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(2)).Trim())
                        .FirstOrDefault();
                    if (sampling_template != null) detail_sampling_template_id = sampling_template.id.ToString();

                    var quality_sampling_id = "";
                    var quality_sampling = dbContext.quality_sampling
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.sampling_number.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).Trim().ToLower() &&
                                    o.sampling_template_id == detail_sampling_template_id &&
                                    o.sampling_datetime == PublicFunctions.Tanggal(row.GetCell(1)))
                        .FirstOrDefault();
                    if (quality_sampling != null)
                        quality_sampling_id = quality_sampling.id.ToString();
                    else
                    {
                        errormessage = "Sampling Number or Sampling Template or Sampling DateTime not found.";
                        teks += errormessage + Environment.NewLine;
                        gagal = true;
                    }

                    var analyte = "";
                    var a = dbContext.analyte.Where(o => o.analyte_name == PublicFunctions.IsNullCell(row.GetCell(3)).Trim()).FirstOrDefault();
                    if (a != null) analyte = a.id;

                    var record = dbContext.quality_sampling_analyte
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                        && o.quality_sampling_id == quality_sampling_id
                                        && o.analyte_id == analyte)
                                    .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.analyte_value = PublicFunctions.Desimal(row.GetCell(4));

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
                        record.analyte_id = analyte;    
                        record.analyte_value = PublicFunctions.Desimal(row.GetCell(4));

                        dbContext.quality_sampling_analyte.Add(record);
                        await dbContext.SaveChangesAsync();
                    }

                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 2, Line " + (i + 1) + " : " + Environment.NewLine;
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

        //   [HttpPost("UploadDocumentKMIA")]
        //   [ApiExplorerSettings(IgnoreApi = true)]
        //   public async Task<object> UploadDocumentKMIA([FromBody] dynamic FileDocument)
        //   {
        //       var result = new StandardResult();
        //       long size = 0;

        //       if (FileDocument == null)
        //       {
        //           return BadRequest("No file uploaded!");
        //       }

        //       string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
        //       if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);

        //       var fileName = (string)FileDocument.filename;
        //       FilePath += $@"\{fileName}";

        //       string strfile = (string)FileDocument.data;
        //       byte[] arrfile = Convert.FromBase64String(strfile);

        //       await System.IO.File.WriteAllBytesAsync(FilePath, arrfile);

        //       size = fileName.Length;
        //       string sFileExt = Path.GetExtension(FilePath).ToLower();

        //       ISheet sheet;
        //       dynamic wb;
        //       if (sFileExt == ".xls")
        //       {
        //           FileStream stream = System.IO.File.OpenRead(FilePath);
        //           wb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats
        //           sheet = wb.GetSheetAt(0); //get first sheet from workbook
        //           stream.Close();
        //       }
        //       else
        //       {
        //           wb = new XSSFWorkbook(FilePath); //This will read 2007 Excel format
        //           sheet = wb.GetSheetAt(0); //get first sheet from workbook
        //       }

        //       string teks = "";
        //       bool gagal = false; string errormessage = "";

        //       using var transaction = await dbContext.Database.BeginTransactionAsync();
        //       for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
        //       {
        //           try
        //           {
        //               IRow row = sheet.GetRow(i);
        //               if (row == null) continue;

        //               var surveyor_id = "";
        //               var surveyor = dbContext.contractor
        //                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                       o.business_partner_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(2)).Trim())
        //                   .FirstOrDefault();
        //               if (surveyor != null) surveyor_id = surveyor.id.ToString();

        //               var stock_location_id = "";
        //               var stock_location = dbContext.stockpile_location
        //                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                       o.stockpile_location_code == PublicFunctions.IsNullCell(row.GetCell(3))).FirstOrDefault();
        //               if (stock_location != null)
        //                   stock_location_id = stock_location.id.ToString();
        //               else
        //               {
        //                   var port_location = dbContext.port_location
        //                       .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                           o.port_location_code == PublicFunctions.IsNullCell(row.GetCell(3))).FirstOrDefault();
        //                   if (port_location != null)
        //                       stock_location_id = port_location.id.ToString();
        //                   else
        //                   {
        //                       var mine_location = dbContext.mine_location
        //                           .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                               o.mine_location_code == PublicFunctions.IsNullCell(row.GetCell(3))).FirstOrDefault();
        //                       if (mine_location != null)
        //                           stock_location_id = mine_location.id.ToString();
        //                       else
        //                       {
        //                           var barges = dbContext.barge
        //                               .Where(o => o.organization_id == CurrentUserContext.OrganizationId
        //                                   && o.vehicle_name.Trim().ToUpper() == PublicFunctions.IsNullCell(row.GetCell(3)).Trim().ToUpper())
        //                               .FirstOrDefault();
        //                           if (barges != null)
        //                               stock_location_id = barges.id.ToString();
        //                           else
        //                           {
        //                               var vessels = dbContext.vessel
        //                                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId
        //                                       && o.vehicle_name.Trim().ToUpper() == PublicFunctions.IsNullCell(row.GetCell(3)).Trim().ToUpper())
        //                                   .FirstOrDefault();
        //                               if (vessels != null)
        //                                   stock_location_id = vessels.id.ToString();
        //                           }
        //                       }
        //                   }
        //               }

        //               var product_id = "";
        //               var product = dbContext.product
        //                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                       o.product_code == PublicFunctions.IsNullCell(row.GetCell(4))).FirstOrDefault();
        //               if (product != null) product_id = product.id.ToString();

        //               var sampling_template_id = "";
        //               var sampling_template = dbContext.sampling_template
        //                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                       o.sampling_template_code == PublicFunctions.IsNullCell(row.GetCell(5))).FirstOrDefault();
        //               if (sampling_template != null) sampling_template_id = sampling_template.id.ToString();

        //               var despatch_order_id = "";
        //               var despatch_order = dbContext.despatch_order
        //                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                       o.despatch_order_number.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(6)).Trim().ToLower())
        //                   .FirstOrDefault();
        //               if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();

        //               var sampling_type_id = ""; var sampling_type_code = "";
        //               var sampling_type = dbContext.sampling_type
        //                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                       o.sampling_type_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(8)).Trim())
        //                   .FirstOrDefault();
        //               if (sampling_type != null)
        //               {
        //                   sampling_type_id = sampling_type.id.ToString();
        //                   sampling_type_code = sampling_type.sampling_type_code.ToString();
        //               }

        //               var shift_id = "";
        //               var shift = dbContext.shift
        //                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                       o.shift_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(9)).Trim())
        //                   .FirstOrDefault();
        //               if (shift != null) shift_id = shift.id.ToString();

        //               var barging_transaction_id = "";
        //               var barging_transaction = dbContext.barging_transaction
        //                   .Where(o => o.transaction_number == PublicFunctions.IsNullCell(row.GetCell(11)).Trim())
        //                   .FirstOrDefault();
        //               if (barging_transaction != null) barging_transaction_id = barging_transaction.id.ToString();


        //               //var record = dbContext.quality_sampling
        //               //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
        //               //                o.sampling_number == PublicFunctions.IsNullCell(row.GetCell(0)) && 
        //               //                o.sampling_template_id == sampling_template_id)
        //               //    .FirstOrDefault();
        //               var record = dbContext.quality_sampling
        //                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                               o.sampling_number == PublicFunctions.IsNullCell(row.GetCell(0)) &&
        //                               o.sampling_template_id == sampling_template_id &&
        //                               o.sampling_datetime == PublicFunctions.Tanggal(row.GetCell(1)))
        //                   .FirstOrDefault();
        //               if (record != null)
        //               {
        //                   var e = new entity();
        //                   e.InjectFrom(record);

        //                   record.InjectFrom(e);
        //                   record.modified_by = CurrentUserContext.AppUserId;
        //                   record.modified_on = DateTime.Now;

        //                   record.sampling_type_id = sampling_type_id;
        //                   record.surveyor_id = surveyor_id;
        //                   record.stock_location_id = stock_location_id;
        //                   record.product_id = product_id;
        //                   record.sampling_template_id = sampling_template_id;
        //                   record.despatch_order_id = despatch_order_id;
        //                   record.non_commercial = PublicFunctions.BenarSalah(row.GetCell(7));
        //                   record.shift_id = shift_id;
        //                   record.is_draft = PublicFunctions.BenarSalah(row.GetCell(10));
        //                   record.barging_transaction_id = barging_transaction_id;

        //                   await dbContext.SaveChangesAsync();
        //               }
        //               else
        //               {
        //                   record = new quality_sampling();
        //                   record.id = Guid.NewGuid().ToString("N");
        //                   record.created_by = CurrentUserContext.AppUserId;
        //                   record.created_on = DateTime.Now;
        //                   record.modified_by = null;
        //                   record.modified_on = null;
        //                   record.is_active = true;
        //                   record.is_default = null;
        //                   record.is_locked = null;
        //                   record.entity_id = null;
        //                   record.owner_id = CurrentUserContext.AppUserId;
        //                   record.organization_id = CurrentUserContext.OrganizationId;
        //record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

        //                   record.sampling_number = PublicFunctions.IsNullCell(row.GetCell(0)).Trim();
        //                   record.sampling_datetime = PublicFunctions.Tanggal(row.GetCell(1));
        //                   record.surveyor_id = surveyor_id;
        //                   record.stock_location_id = stock_location_id;
        //                   record.product_id = product_id;
        //                   record.sampling_template_id = sampling_template_id;
        //                   record.despatch_order_id = despatch_order_id;
        //                   record.non_commercial = PublicFunctions.BenarSalah(row.GetCell(7));
        //                   record.sampling_type_id = sampling_type_id;
        //                   record.shift_id = shift_id;
        //                   record.is_draft = PublicFunctions.BenarSalah(row.GetCell(10));
        //                   record.barging_transaction_id = barging_transaction_id;

        //                   dbContext.quality_sampling.Add(record);
        //                   await dbContext.SaveChangesAsync();
        //               }
        //           }
        //           catch (Exception ex)
        //           {
        //               if (ex.InnerException != null)
        //               {
        //                   errormessage = ex.InnerException.Message;
        //                   teks += "==>Error Sheet 1, Line " + (i + 1) + " : " + Environment.NewLine;
        //               }
        //               else errormessage = ex.Message;

        //               teks += errormessage + Environment.NewLine + Environment.NewLine;
        //               gagal = true;
        //               break;
        //           }
        //       }

        //       sheet = wb.GetSheetAt(1); //*** detail sheet
        //       for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
        //       {
        //           try
        //           {
        //               IRow row = sheet.GetRow(i);
        //               if (row == null) continue;

        //               var detail_sampling_template_id = "";
        //               var sampling_template = dbContext.sampling_template
        //                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                       o.sampling_template_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(2)).Trim())
        //                   .FirstOrDefault();
        //               if (sampling_template != null) detail_sampling_template_id = sampling_template.id.ToString();

        //               var quality_sampling_id = "";
        //               var quality_sampling = dbContext.quality_sampling
        //                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                               o.sampling_number.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).Trim().ToLower() &&
        //                               o.sampling_template_id == detail_sampling_template_id &&
        //                               o.sampling_datetime == PublicFunctions.Tanggal(row.GetCell(1)))
        //                   .FirstOrDefault();
        //               if (quality_sampling != null) 
        //                   quality_sampling_id = quality_sampling.id.ToString();
        //               else
        //               {
        //                   errormessage = "Sampling Number or Sampling Template or Sampling DateTime not found.";
        //                   teks += errormessage + Environment.NewLine;
        //                   gagal = true;
        //               }

        //               var analyte_id = "";

        //               string[] analyteSymbols = { "tm(arb)", "im(adb)", "ash(adb)", "vm(adb)", "fc(adb)", "ts(adb)", "cv(adb)", "cv(daf)", "cv(arb)" };

        //               for (int analyteCol = 3; analyteCol <= 11; analyteCol++)
        //               {
        //                   decimal analyteValue = 0;
        //                   if (PublicFunctions.IsNullCell(row.GetCell(analyteCol)) != "")
        //                   {
        //                       analyteValue = PublicFunctions.Desimal(row.GetCell(analyteCol));

        //                       var analyte = dbContext.analyte
        //                           .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                               o.analyte_symbol.Replace(" ", "").ToLower() == analyteSymbols[analyteCol - 3])
        //                           .FirstOrDefault();
        //                       if (analyte != null)
        //                       {
        //                           analyte_id = analyte.id.ToString();

        //                           var record = dbContext.quality_sampling_analyte
        //                               .Where(o => o.organization_id == CurrentUserContext.OrganizationId
        //                                   && o.quality_sampling_id == quality_sampling_id
        //                                   && o.analyte_id == analyte_id)
        //                               .FirstOrDefault();
        //                           if (record != null)
        //                           {
        //                               var e = new entity();
        //                               e.InjectFrom(record);

        //                               record.InjectFrom(e);
        //                               record.modified_by = CurrentUserContext.AppUserId;
        //                               record.modified_on = DateTime.Now;

        //                               record.analyte_value = analyteValue;

        //                               await dbContext.SaveChangesAsync();
        //                           }
        //                           else
        //                           {
        //                               record = new quality_sampling_analyte();
        //                               record.id = Guid.NewGuid().ToString("N");
        //                               record.created_by = CurrentUserContext.AppUserId;
        //                               record.created_on = DateTime.Now;
        //                               record.modified_by = null;
        //                               record.modified_on = null;
        //                               record.is_active = true;
        //                               record.is_default = null;
        //                               record.is_locked = null;
        //                               record.entity_id = null;
        //                               record.owner_id = CurrentUserContext.AppUserId;
        //                               record.organization_id = CurrentUserContext.OrganizationId;
        //record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

        //                               record.quality_sampling_id = quality_sampling_id;
        //                               record.analyte_id = analyte_id;
        //                               record.analyte_value = analyteValue;

        //                               dbContext.quality_sampling_analyte.Add(record);
        //                               await dbContext.SaveChangesAsync();
        //                           }
        //                       }
        //                   }
        //               }

        //           }
        //           catch (Exception ex)
        //           {
        //               if (ex.InnerException != null)
        //               {
        //                   errormessage = ex.InnerException.Message;
        //                   teks += "==>Error Sheet 2, Line " + (i + 1) + " : " + Environment.NewLine;
        //               }
        //               else errormessage = ex.Message;

        //               teks += errormessage + Environment.NewLine + Environment.NewLine;
        //               gagal = true;
        //               break;
        //           }
        //       }
        //       wb.Close();
        //       if (gagal)
        //       {
        //           await transaction.RollbackAsync();
        //           HttpContext.Session.SetString("errormessage", teks);
        //           HttpContext.Session.SetString("filename", "QualitySampling");
        //           return BadRequest("File gagal di-upload");
        //       }
        //       else
        //       {
        //           await transaction.CommitAsync();
        //           return "File berhasil di-upload!";
        //       }
        //   }

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

        [HttpPost("DeleteSelectedRows")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteSelectedRows([FromBody] dynamic Data)
        {
            var result = new StandardResult();
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (Data != null && Data.selectedIds != null)
                    {
                        var selectedIds = ((string)Data.selectedIds)
                            .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .ToList();

                        foreach (string key in selectedIds)
                        {
                            var record = dbContext.quality_sampling
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.quality_sampling.Remove(record);
                                await dbContext.SaveChangesAsync();
                            }
                        }

                        await tx.CommitAsync();

                        result.Success = true;
                        return Ok(result);
                    }
                    else
                    {
                        result.Message = "Invalid data.";
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    result.Message = ex.Message;
                }
            }

            return new JsonResult(result);
        }

    }
}

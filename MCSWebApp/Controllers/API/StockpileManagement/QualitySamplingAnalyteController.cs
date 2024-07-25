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

namespace MCSWebApp.Controllers.API.StockpileManagement
{
    [Route("api/StockpileManagement/[controller]")]
    [ApiController]
    public class QualitySamplingAnalyteController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public QualitySamplingAnalyteController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("ByQualitySamplingId/{Id}")]
        public async Task<object> ByQualitySamplingId(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling_analyte
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.quality_sampling_id == Id),
                loadOptions);
        }

        [HttpGet("ByDespatchOrderId/{Id}")]
        public async Task<object> DataGridByDespatchOrderId(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling_analyte
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.despatch_order_id == Id
                    && o.non_commercial != true),
                loadOptions);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling_analyte
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling_analyte
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
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
                    success = true;                    
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
            return Ok(record);
        }

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            var success = false;
            quality_sampling qs = null;
            quality_sampling_analyte record = null;

            var tx = await dbContext.Database.BeginTransactionAsync();
            try
            {
                record = dbContext.quality_sampling_analyte
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    qs = dbContext.quality_sampling.Where(x => x.id == record.quality_sampling_id).FirstOrDefault();

                    var e = new entity();
                    e.InjectFrom(record);

                    JsonConvert.PopulateObject(values, record);

                    var sampling_template = dbContext.sampling_template
                        .Where(o => o.id == qs.sampling_template_id).FirstOrDefault();
                    if (sampling_template != null)
                    {
                        if (sampling_template.is_stock_state == true && record.analyte_value == 0)
                        {
                            return BadRequest("Analyte Value can't be zero if Sampling Template as Stock State.");
                        }
                    }

                    record.InjectFrom(e);
                    record.modified_by = CurrentUserContext.AppUserId;
                    record.modified_on = DateTime.Now;
                    await dbContext.SaveChangesAsync();

                    await tx.CommitAsync();
                    success = true;                    
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
            return Ok(record);
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
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

        [HttpGet("AnalyteIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AnalyteIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.analyte
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.analyte_name });
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
                var record = await dbContext.vw_quality_sampling_analyte
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

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] survey Record)
        {
            try
            {
                var record = dbContext.quality_sampling_analyte
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Record.id)
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
                    record = new quality_sampling_analyte();
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

                    dbContext.quality_sampling_analyte.Add(record);
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
                var record = dbContext.quality_sampling_analyte
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.quality_sampling_analyte.Remove(record);
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

        [HttpPut("UpdateOrderData")]
        public async Task<IActionResult> UpdateOrderData([FromForm] string key, [FromForm] string id, [FromForm] int type)
        {
            logger.Trace($"string values = {key}");
            logger.Trace($"string values = {id}");
            logger.Trace($"string values = {type}");

            try
            {
                var record = dbContext.quality_sampling_analyte
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                var listRecord = dbContext.quality_sampling_analyte
                    .Where(o => o.quality_sampling_id == id)
                    .OrderBy(x => x.created_on)
                    .OrderBy(x => x.order).ToList();
                if (record != null)
                {
                    if(listRecord.Count() < 2)
                    {
                        return BadRequest("data only 1");
                    }

                    int indexChoosen = listRecord.FindIndex(x => x.id == key);
                    
                    if (type == 1)
                    {
                        if (listRecord.Count() == indexChoosen + 1)
                        {
                            return BadRequest("data already in last order");
                        }

                        var recordOther = listRecord[indexChoosen + 1];
                                                
                        recordOther.order = indexChoosen;
                        record.order = indexChoosen + 1;
                        recordOther.modified_by = CurrentUserContext.AppUserId;
                        recordOther.modified_on = DateTime.Now;

                    }
                    else
                    {
                        if (indexChoosen == 0)
                        {
                            return BadRequest("data already in first order");
                        }

                        var recordOther = listRecord[indexChoosen - 1];
                        
                        recordOther.order = indexChoosen;
                        record.order = indexChoosen - 1;
                        recordOther.modified_by = CurrentUserContext.AppUserId;
                        recordOther.modified_on = DateTime.Now;

                    }
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
    }
}

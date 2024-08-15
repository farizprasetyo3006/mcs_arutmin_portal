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
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using NPOI.SS.Formula.Functions;
using Microsoft.AspNetCore.SignalR;
using System.Threading;

namespace MCSWebApp.Controllers.API.Mining
{
    [Route("api/Mining/[controller]")]
    [ApiController]
    public class ProcessingController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly IHubContext<ProgressHub> _hubContext;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public ProcessingController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption, IHubContext<ProgressHub> hubContext)
             : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
            _hubContext = hubContext;
        }

        //[HttpGet("DataGrid")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        //{
        //    return await DataSourceLoader.LoadAsync(dbContext.processing_transaction
        //        .Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
        //                || CurrentUserContext.IsSysAdmin)
        //        .Where(o => o.organization_id == CurrentUserContext.OrganizationId), 
        //        loadOptions);
        //}

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            logger.Debug($"tanggal1 = {tanggal1}");
            logger.Debug($"tanggal2 = {tanggal2}");

            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.processing_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            logger.Debug($"dt1 = {dt1}");
            logger.Debug($"dt2 = {dt2}");

            return await DataSourceLoader.LoadAsync(dbContext.processing_transaction
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.loading_datetime >= dt1 && o.loading_datetime <= dt2),
                    loadOptions);
        }

        [HttpGet("CHLSCoalProduce/{tanggal1}/{tanggal2}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> CHLSCoalProduce(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);
            var category = "Coal Produce";

            /*dynamic haulings = await dbContext.chls_cpp.FromSqlRaw(
                "SELECT ch.* " +
                "FROM chls_cpp ch " +
                "LEFT JOIN process_flow pf ON pf.id = ch.process_flow_id " +
                "WHERE pf.process_flow_category = {0} AND ch.start_time >= {1} AND ch.end_time <= {2}", category, dt1, dt2)
                .Where(o => o.approved == true)
                .ToListAsync();*/
            return await DataSourceLoader.LoadAsync(dbContext.chls_cpp.FromSqlRaw(
                "SELECT ch.* " +
                "FROM chls_cpp ch " +
                "LEFT JOIN process_flow pf ON pf.id = ch.process_flow_id " +
                "WHERE pf.process_flow_category = {0} AND ch.start_time >= {1} AND ch.end_time <= {2} AND pf.business_unit_id = {3}", category, dt1, dt2, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                .Where(o => o.approved == true), loadOptions);

        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.processing_transaction
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            processing_transaction record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
					if (await mcsContext.CanCreate(dbContext, nameof(processing_transaction),
						CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
					{
                        record = new processing_transaction();
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
					    //record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                        if (record.unloading_quantity == null || record.unloading_quantity.Value == 0)
                        {
                            record.unloading_quantity = record.loading_quantity;
                        }
                        #region Validation

                        if (record.loading_datetime > record.unloading_datetime)
                            return BadRequest("Loading Date tidak boleh melampaui Unloading Date");

                      //  if (record.source_uom_id != record.destination_uom_id)
                            //return BadRequest("The Source Unit must be the same as the Destination Unit");

                        //if (record.unloading_quantity > record.loading_quantity)
                           // return BadRequest("The Unloading Quantity must not exceed the Loading Quantity");

                        // Source location != destination location
                        if (record.source_location_id == record.destination_location_id)
                        {
                            return BadRequest("Source location must be different from destination location");
                        }

                        // Capacity
                        if (record.transport_id != null)
                        {
                            var tr1 = await dbContext.transport
                                .Where(o => o.id == record.transport_id)
                                .FirstOrDefaultAsync();
                            if (tr1 != null)
                            {
                                if (record.loading_quantity != null)
                                {
                                    if ((decimal)(tr1?.capacity ?? 0) < record.loading_quantity)
                                    {
                                        return BadRequest("Transport capacity is less than loading quantity");
                                    }
                                }

                                if (record.unloading_quantity != null)
                                {
                                    if ((decimal)(tr1?.capacity ?? 0) < record.unloading_quantity)
                                    {
                                        return BadRequest("Transport capacity is less than unloading quantity");
                                    }
                                }
                            }
                        }

                        #endregion

                        #region Get transcation number

                        var conn = dbContext.Database.GetDbConnection();
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            await conn.OpenAsync();
                        }
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                try
                                {
                                    cmd.CommandText = $"SELECT nextval('seq_processing_number')";
                                    var r = await cmd.ExecuteScalarAsync();
                                    record.transaction_number = $"PC-{DateTime.Now:yyyyMMdd}-{r}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        #endregion

                        dbContext.processing_transaction.Add(record);

                        #region Calculate actual progress claim

                        if (!string.IsNullOrEmpty(record.progress_claim_id))
                        {
                            var pc = await dbContext.progress_claim
                                .Where(o => o.id == record.progress_claim_id)
                                .FirstOrDefaultAsync();
                            if (pc != null)
                            {
                                var actualQty = await dbContext.processing_transaction
                                    .Where(o => o.progress_claim_id == pc.id)
                                    .SumAsync(o => o.unloading_quantity);
                                pc.actual_quantity = actualQty;
                            }
                        }

                        #endregion

                        await dbContext.SaveChangesAsync();
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
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
				}
            }
            /*
            if (success && record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.processing_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ProcessingTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.ContinueJobWith(_jobId, 
                        () => BusinessLogic.Entity.ProcessingTransaction.UpdateStockStateAnalyte(connectionString, _record),
                        null, JobContinuationOptions.OnlyOnSucceededState);
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

            processing_transaction record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.processing_transaction
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.id == key)
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
                            if (record.unloading_quantity == null || record.unloading_quantity.Value == 0)
                            {
                                record.unloading_quantity = record.loading_quantity;
                            }
                            #region Validation
                            if (record.loading_datetime > record.unloading_datetime)
                                return BadRequest("Loading Date tidak boleh melampaui Unloading Date");

                            //if (record.source_uom_id != record.destination_uom_id)
                              //  return BadRequest("The Source Unit must be the same as the Destination Unit");

                            //if (record.source_uom_id != record.destination_uom_id)
                            //    return BadRequest("The Source Unit must be the same as the Destination Unit");

                           // if (record.unloading_quantity > record.loading_quantity)
                             //   return BadRequest("The Unloading Quantity must not exceed the Loading Quantity");

                            // Must be in open accounting period
                            var ap1 = await dbContext.accounting_period
                                .Where(o => o.id == record.accounting_period_id)
                                .FirstOrDefaultAsync();
                            if (ap1 != null && (ap1?.is_closed ?? false))
                            {
                                return BadRequest("Data update is not allowed");
                            }

                            // Source location != destination location
                            if (record.source_location_id == record.destination_location_id)
                            {
                                return BadRequest("Source location must be different from destination location");
                            }

                            // Capacity
                            if (record.transport_id != null)
                            {
                                var tr1 = await dbContext.transport
                                    .Where(o => o.id == record.transport_id)
                                    .FirstOrDefaultAsync();
                                if (tr1 != null)
                                {
                                    if (record.loading_quantity != null)
                                    {
                                        if ((decimal)(tr1?.capacity ?? 0) < record.loading_quantity)
                                        {
                                            return BadRequest("Transport capacity is less than loading quantity");
                                        }
                                    }

                                    if (record.unloading_quantity != null)
                                    {
                                        if ((decimal)(tr1?.capacity ?? 0) < record.unloading_quantity)
                                        {
                                            return BadRequest("Transport capacity is less than unloading quantity");
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region Calculate actual progress claim

                            if (!string.IsNullOrEmpty(record.progress_claim_id))
                            {
                                var pc = await dbContext.progress_claim
                                    .Where(o => o.id == record.progress_claim_id)
                                    .FirstOrDefaultAsync();
                                if (pc != null)
                                {
                                    var actualQty = await dbContext.processing_transaction
                                        .Where(o => o.progress_claim_id == pc.id)
                                        .SumAsync(o => o.unloading_quantity);
                                    pc.actual_quantity = actualQty;
                                }
                            }

                            #endregion

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
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
            if (success && record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.processing_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ProcessingTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.ContinueJobWith(_jobId, 
                        () => BusinessLogic.Entity.ProcessingTransaction.UpdateStockStateAnalyte(connectionString, _record),
                        null, JobContinuationOptions.OnlyOnSucceededState);
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

            processing_transaction record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.processing_transaction
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.processing_transaction.Remove(record);

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
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
            if (success && record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.processing_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ProcessingTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.ContinueJobWith(_jobId, 
                        () => BusinessLogic.Entity.ProcessingTransaction.UpdateStockStateAnalyte(connectionString, _record),
                        null, JobContinuationOptions.OnlyOnSucceededState);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok();
        }

        [HttpPut("RequestIntegration")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> RequestIntegration([FromBody] dynamic Data)
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
                        var records = await dbContext.processing_transaction.Where(o => selectedIds.Contains(o.id)).ToListAsync();
                        await _semaphore.WaitAsync();
                        try
                        {
                            foreach (var record in records)
                            {
                                switch (record.integration_status)
                                {
                                    case "NOT APPROVED":
                                        record.integration_status = "REQUESTED FOR APPROVAL";
                                        break;
                                    case "REQUESTED FOR APPROVAL":
                                        record.integration_status = "NOT APPROVED";
                                        break;
                                    case "APPROVED":
                                        record.integration_status = "REQUESTED FOR UNAPPROVAL";
                                        break;
                                    case "REQUESTED FOR UNAPPROVAL":
                                        record.integration_status = "APPROVED";
                                        break;
                                }
                            }
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                        await dbContext.SaveChangesAsync();
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
        [HttpGet("ProcessFlowIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProcessFlowIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.process_flow
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.process_flow_category == Common.ProcessFlowCategory.COAL_PRODUCE)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                    new
                    {
                        Value = o.id,
                        Text = o.process_flow_name,
                        search = o.process_flow_name.ToLower() + o.process_flow_name.ToUpper()
                    });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("SurveyIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SurveyIdLookup(string DestinationLocationId,
            DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"DestinationLocationId = {DestinationLocationId}");

            try
            {
                if (string.IsNullOrEmpty(DestinationLocationId))
                {
                    var lookup = dbContext.survey
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && (o.is_draft_survey == null || o.is_draft_survey == false))
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.survey_number, search = o.survey_number.ToLower() + o.survey_number.ToUpper() });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.survey.FromSqlRaw(
                          " SELECT s.* FROM survey s "
                        + " INNER JOIN stock_location sl ON sl.id = s.stock_location_id "
                        + " WHERE COALESCE(s.is_draft_survey, FALSE) = FALSE "
                        + " AND s.organization_id = {0} "                        
                        + " AND sl.id = {1} ",
                          CurrentUserContext.OrganizationId, DestinationLocationId
                        )
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.survey_number,
                                o.product_id,
                                search = o.survey_number.ToLower() + o.survey_number.ToUpper() 
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

        [HttpGet("ProcessingCategoryIdLookup")]
        public async Task<object> ProcessingCategoryIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.processing_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.processing_category_name, search = o.processing_category_name.ToLower() + o.processing_category_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("EquipmentIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> EquipmentIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.equipment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.equipment_code, search = o.equipment_code.ToLower() + o.equipment_code.ToUpper() });
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
        public async Task<object> SourceLocationIdLookup(string ProcessFlowId,
            DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (string.IsNullOrEmpty(ProcessFlowId))
                {
                    var lookup = dbContext.stockpile_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)  
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.stock_location_name,
                                o.product_id,
                                search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper()
                            }).OrderBy(o => o.text);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup1 = dbContext.stockpile_location.FromSqlRaw(
                         " SELECT l.* FROM stockpile_location l "
                       + " WHERE l.organization_id = {0} "
                       + " AND l.business_area_id IN ( "
                       + "     SELECT ba.id FROM vw_business_area_structure ba, process_flow pf "
                       + "     WHERE position(pf.source_location_id in ba.id_path) > 0"
                       + "         AND pf.id = {1} "
                       + " ) ",
                         CurrentUserContext.OrganizationId, ProcessFlowId
                       )
                       .Select(o =>
                           new
                           {
                               value = o.id,
                               text = o.stock_location_name,
                               urutan = 1,
                               search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper()
                           }).OrderBy(o => o.text);
                    var lookup2 = dbContext.mine_location.FromSqlRaw(
                          " SELECT l.* FROM mine_location l "
                        + " WHERE l.organization_id = {0} "
                        + " AND l.business_area_id IN ( "
                        + "     SELECT ba.id FROM vw_business_area_structure ba, process_flow pf "
                        + "     WHERE position(pf.source_location_id in ba.id_path) > 0"
                        + "         AND pf.id = {1} "
                        + " ) ",
                          CurrentUserContext.OrganizationId, ProcessFlowId
                        )
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.stock_location_name,
                                urutan = 2,
                                search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper()
                            }).OrderBy(o => o.text);
                    var lookup3 = dbContext.port_location.FromSqlRaw(
                          " SELECT l.* FROM port_location l "
                        + " WHERE l.organization_id = {0} "
                        + " AND l.business_area_id IN ( "
                        + "     SELECT ba.id FROM vw_business_area_structure ba, process_flow pf "
                        + "     WHERE position(pf.source_location_id in ba.id_path) > 0"
                        + "         AND pf.id = {1} "
                        + " ) ",
                          CurrentUserContext.OrganizationId, ProcessFlowId
                        )
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.stock_location_name,
                                urutan = 3,
                                search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper()
                            }).OrderBy(o => o.text);
                    var lookup4 = dbContext.waste_location.FromSqlRaw(
                          " SELECT l.* FROM waste_location l "
                        + " WHERE l.organization_id = {0} "
                        + " AND l.business_area_id IN ( "
                        + "     SELECT ba.id FROM vw_business_area_structure ba, process_flow pf "
                        + "     WHERE position(pf.source_location_id in ba.id_path) > 0"
                        + "         AND pf.id = {1} "
                        + " ) ",
                          CurrentUserContext.OrganizationId, ProcessFlowId
                        )
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.stock_location_name,
                                urutan = 4,
                                search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper()
                            }).OrderBy(o => o.text);
                    var lookup = lookup1.Union(lookup2).Union(lookup3).Union(lookup4).OrderBy(o => o.urutan);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("DestinationLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DestinationLocationIdLookup(string ProcessFlowId,
            DataSourceLoadOptions loadOptions)
        {
            try
            {
                /*var lookup1 = dbContext.port_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin
                            && o.stock_location_name.ToUpper() == "SHS JETTY")
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = o.stock_location_name,
                            urutan = 1
                        });*/

                if (string.IsNullOrEmpty(ProcessFlowId))
                {
                    //var lookup = dbContext.stockpile_location
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //    .Select(o =>
                    //        new
                    //        {
                    //            value = o.id,
                    //            text = o.stock_location_name,
                    //            o.product_id
                    //        });

                    var lookup2 = dbContext.stockpile_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.stock_location_name,
                                urutan = 2,
                                search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper()
                            }).OrderBy(o => o.text);

                    //var lookup = lookup1.Union(lookup2).OrderBy(o => o.urutan);

                    return await DataSourceLoader.LoadAsync(lookup2, loadOptions);
                }
                else
                {
                    var lookup1 = dbContext.vw_stock_location.FromSqlRaw(
                        " SELECT l1.id, l1.stock_location_name, l1.business_area_name, l1.product_id FROM vw_stockpile_location l1 "
                        + " WHERE l1.organization_id = {0} "
                        + " AND l1.business_area_id IN ( "
                        + "     SELECT ba1.id FROM vw_business_area_structure ba1, process_flow pf1 "
                        + "     WHERE position(pf1.destination_location_id in ba1.id_path) > 0"
                        + "         AND pf1.id = {1} ) "
                        + " UNION "
                        + " SELECT l2.id, l2.stock_location_name, l2.business_area_name, l2.product_id FROM vw_port_location l2 "
                        + " WHERE l2.organization_id = {0} "
                        + " AND l2.business_area_id IN ( "
                        + "     SELECT ba2.id FROM vw_business_area_structure ba2, process_flow pf2 "
                        + "     WHERE position(pf2.destination_location_id in ba2.id_path) > 0"
                        + "         AND pf2.id = {1} ) "
                        + " UNION "
                        + " SELECT l3.id, l3.stock_location_name, l3.business_area_name, l3.product_id FROM vw_mine_location l3 "
                        + " WHERE l3.organization_id = {0} "
                        + " AND l3.business_area_id IN ( "
                        + "     SELECT ba3.id FROM vw_business_area_structure ba3, process_flow pf3 "
                        + "     WHERE position(pf3.destination_location_id in ba3.id_path) > 0"
                        + "         AND pf3.id = {1} ) "
                        + " UNION "
                        + " SELECT l4.id, l4.stock_location_name, l4.business_area_name, l4.product_id FROM vw_waste_location l4 "
                        + " WHERE l4.organization_id = {0} "
                        + " AND l4.business_area_id IN ( "
                        + "     SELECT ba4.id FROM vw_business_area_structure ba4, process_flow pf4 "
                        + "     WHERE position(pf4.destination_location_id in ba4.id_path) > 0"
                        + "         AND pf4.id = {1} ) "
                        /*+ " UNION "
                        + " SELECT l3.id, l3.vehicle_name as stock_location_name, '' as business_area_name, '' as product_id FROM barge l3 "
                        + " WHERE l3.organization_id = {0} "*/

                        , CurrentUserContext.OrganizationId, ProcessFlowId)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                //text = o.business_area_name + " > " + o.stock_location_name,
                                text = (o.business_area_name != "" ? o.business_area_name + " > " : "") + o.stock_location_name,
                                urutan = 1,
                                search = (o.business_area_name != "" ? o.business_area_name.ToLower() + " > " : "") + o.stock_location_name.ToLower() + o.business_area_name.ToUpper() + " > "  +o.stock_location_name.ToUpper()
                            })
                        .OrderBy(o => o.text);

                    /*var lookup2 = dbContext.port_location.FromSqlRaw(
                          " SELECT l.* FROM port_location l "
                        + " WHERE l.organization_id = {0} ",
                           CurrentUserContext.OrganizationId, ProcessFlowId
                         )
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.stock_location_name,
                                urutan = 2
                            });*/
                    //var lookup = lookup1.Union(lookup2).OrderBy(o => o.urutan);
                    return await DataSourceLoader.LoadAsync(lookup1, loadOptions);

                    //return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("DespatchOrderIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DespatchOrderIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number, search = o.despatch_order_number.ToLower() + o.despatch_order_number.ToUpper() });
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
                    .Select(o => new { Value = o.id, Text = o.sampling_number, search = o.sampling_number.ToLower() + o.sampling_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("ProgressClaimIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProgressClaimIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.progress_claim
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.progress_claim_name, search = o.progress_claim_name.ToLower() + o.progress_claim_name.ToUpper() });
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
                var record = await dbContext.vw_processing_transaction
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
        public async Task<IActionResult> SaveData([FromBody] processing_transaction Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.processing_transaction
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.id == Record.id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            if (string.IsNullOrEmpty(record.transaction_number))
                            {
                                #region Get transcation number

                                var conn = dbContext.Database.GetDbConnection();
                                if (conn.State != System.Data.ConnectionState.Open)
                                {
                                    await conn.OpenAsync();
                                }
                                if (conn.State == System.Data.ConnectionState.Open)
                                {
                                    using (var cmd = conn.CreateCommand())
                                    {
                                        try
                                        {
                                            cmd.CommandText = $"SELECT nextval('seq_processing_number')";
                                            var r = await cmd.ExecuteScalarAsync();
                                            record.transaction_number = $"PC-{DateTime.Now:yyyyMMdd}-{r}";
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex.ToString());
                                            return BadRequest(ex.Message);
                                        }
                                    }
                                }

                                #endregion
                            }

                            #region Update record

                            var e = new entity();
                            e.InjectFrom(record);
                            record.InjectFrom(Record);
                            record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            #endregion

                            #region Update stockpile state

                            var qtyOut = await dbContext.stockpile_state
                                .Where(o => o.stockpile_location_id == record.source_location_id
                                    && o.transaction_id == record.id)
                                .FirstOrDefaultAsync();
                            if (qtyOut != null)
                            {
                                qtyOut.modified_by = CurrentUserContext.AppUserId;
                                qtyOut.modified_on = DateTime.Now;
                                qtyOut.qty_out = record.loading_quantity;
                                qtyOut.transaction_datetime = 
                                    (record.loading_datetime ?? record.unloading_datetime) ?? DateTime.Now;
                            }
                            else
                            {
                                qtyOut = new stockpile_state
                                {
                                    id = Guid.NewGuid().ToString("N"),
                                    created_by = CurrentUserContext.AppUserId,
                                    created_on = DateTime.Now,
                                    is_active = true,
                                    owner_id = CurrentUserContext.AppUserId,
                                    organization_id = CurrentUserContext.OrganizationId,
                                    stockpile_location_id = record.source_location_id,
                                    transaction_id = record.id,
                                    qty_out = record.loading_quantity,
                                    transaction_datetime =
                                        (record.loading_datetime ?? record.unloading_datetime) ?? DateTime.Now
                                };

                                dbContext.stockpile_state.Add(qtyOut);
                            }

                            var qtyIn = await dbContext.stockpile_state
                                .Where(o => o.stockpile_location_id == record.destination_location_id
                                    && o.transaction_id == record.id)
                                .FirstOrDefaultAsync();
                            if (qtyIn != null)
                            {
                                qtyIn.modified_by = CurrentUserContext.AppUserId;
                                qtyIn.modified_on = DateTime.Now;
                                qtyIn.qty_in = record.unloading_quantity;
                                qtyIn.transaction_datetime = 
                                    (record.unloading_datetime ?? record.loading_datetime) ?? DateTime.Now;
                            }
                            else
                            {
                                qtyIn = new stockpile_state
                                {
                                    id = Guid.NewGuid().ToString("N"),
                                    created_by = CurrentUserContext.AppUserId,
                                    created_on = DateTime.Now,
                                    is_active = true,
                                    owner_id = CurrentUserContext.AppUserId,
                                    organization_id = CurrentUserContext.OrganizationId,
                                    stockpile_location_id = record.destination_location_id,
                                    transaction_id = record.id,
                                    qty_in = record.unloading_quantity,
                                    transaction_datetime =
                                        (record.unloading_datetime ?? record.loading_datetime) ?? DateTime.Now
                                };

                                dbContext.stockpile_state.Add(qtyIn);
                            }

                            #endregion

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();

                            Task.Run(() =>
                            {
                                var ss = new BusinessLogic.Entity.StockpileState(CurrentUserContext);
                                ss.Update(record.source_location_id, record.id);
                                ss.Update(record.destination_location_id, record.id);
                            }).Forget();

                            return Ok(record);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else if (await mcsContext.CanCreate(dbContext, nameof(processing_transaction),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        #region Add record

                        record = new processing_transaction();
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

                        #endregion

                        #region Get transcation number

                        var conn = dbContext.Database.GetDbConnection();
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            await conn.OpenAsync();
                        }
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                try
                                {
                                    cmd.CommandText = $"SELECT nextval('seq_processing_number')";
                                    var r = await cmd.ExecuteScalarAsync();
                                    record.transaction_number = $"PC-{DateTime.Now:yyyyMMdd}-{r}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        #endregion

                        dbContext.processing_transaction.Add(record);

                        #region Add to stockpile state

                        var qtyOut = await dbContext.stockpile_state
                            .Where(o => o.stockpile_location_id == record.source_location_id
                                && o.transaction_id == record.id)
                            .FirstOrDefaultAsync();
                        if (qtyOut == null)
                        {
                            qtyOut = new stockpile_state
                            {
                                id = Guid.NewGuid().ToString("N"),
                                created_by = CurrentUserContext.AppUserId,
                                created_on = DateTime.Now,
                                is_active = true,
                                owner_id = CurrentUserContext.AppUserId,
                                organization_id = CurrentUserContext.OrganizationId,
                                stockpile_location_id = record.source_location_id,
                                transaction_id = record.id,
                                qty_out = record.loading_quantity,
                                transaction_datetime =
                                    (record.loading_datetime ?? record.unloading_datetime) ?? DateTime.Now
                            };

                            dbContext.stockpile_state.Add(qtyOut);
                        }

                        var qtyIn = await dbContext.stockpile_state
                            .Where(o => o.stockpile_location_id == record.destination_location_id
                                && o.transaction_id == record.id)
                            .FirstOrDefaultAsync();
                        if (qtyIn == null)
                        {
                            qtyIn = new stockpile_state
                            {
                                id = Guid.NewGuid().ToString("N"),
                                created_by = CurrentUserContext.AppUserId,
                                created_on = DateTime.Now,
                                is_active = true,
                                owner_id = CurrentUserContext.AppUserId,
                                organization_id = CurrentUserContext.OrganizationId,
                                stockpile_location_id = record.destination_location_id,
                                transaction_id = record.id,
                                qty_in = record.unloading_quantity,
                                transaction_datetime = 
                                    (record.unloading_datetime ?? record.loading_datetime) ?? DateTime.Now
                            };

                            dbContext.stockpile_state.Add(qtyOut);
                        }

                        #endregion

                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();

                        Task.Run(() =>
                        {
                            var ss = new BusinessLogic.Entity.StockpileState(CurrentUserContext);
                            ss.Update(record.source_location_id, record.id);
                            ss.Update(record.destination_location_id, record.id);
                        }).Forget();

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

        [HttpDelete("DeleteById/{Id}")]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Debug($"string Id = {Id}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.processing_transaction
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.id == Id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        #region Delete stockpile state

                        var qtyOut = await dbContext.stockpile_state
                            .Where(o => o.stockpile_location_id == record.source_location_id
                                && o.transaction_id == record.id)
                            .FirstOrDefaultAsync();
                        if (qtyOut != null)
                        {
                            qtyOut.qty_in = null;
                            qtyOut.qty_out = null;
                            qtyOut.qty_adjustment = null;
                        }

                        var qtyIn = await dbContext.stockpile_state
                            .Where(o => o.stockpile_location_id == record.destination_location_id
                                && o.transaction_id == record.id)
                            .FirstOrDefaultAsync();
                        if (qtyIn != null)
                        {
                            qtyIn.qty_in = null;
                            qtyIn.qty_out = null;
                            qtyIn.qty_adjustment = null;
                        }

                        #endregion

                        dbContext.processing_transaction.Remove(record);

                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();

                        Task.Run(() =>
                        {
                            var ss = new BusinessLogic.Entity.StockpileState(CurrentUserContext);
                            ss.Update(record.source_location_id, record.id);
                            ss.Update(record.destination_location_id, record.id);
                        }).Forget();
                    }

                    return Ok();
                }
				catch (Exception ex)
				{
					logger.Error(ex.InnerException ?? ex);
					return BadRequest(ex.InnerException?.Message ?? ex.Message);
				}
            }
        }

        [HttpGet("ContractRefIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ContractRefIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.advance_contract
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.advance_contract_number, search = o.advance_contract_number.ToLower() + o.advance_contract_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
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
                            var record = dbContext.processing_transaction
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.processing_transaction.Remove(record);
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

        [HttpPost("ApproveUnapprove")]    //**** tanpa trigger trf_sales_invoice_approval_sync di table sales_invoice_approval
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ApproveUnapprove([FromForm] string key, [FromForm] string values)
        {
            dynamic result;
            bool? isApproved = null;

            var recCHLSCP = await dbContext.chls_cpp
                            .Where(o => o.id == key)
                            .FirstOrDefaultAsync();
            var recCHLS = await dbContext.chls
                           .Where(o => o.id == recCHLSCP.header_id)
                           .FirstOrDefaultAsync();
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var CP = dbContext.processing_transaction
                   .Where(o => o.chls_id == key)
                   .FirstOrDefault();
                    bool? checkCP = null;

                    if (CP != null)
                    {
                        dbContext.processing_transaction.Remove(CP);
                        await dbContext.SaveChangesAsync();
                        checkCP = true;
                    }
                    if (checkCP == true || CP == null && recCHLSCP != null)
                    {
                        recCHLSCP.modified_by = CurrentUserContext.AppUserId;
                        recCHLSCP.modified_on = System.DateTime.Now;
                        if (recCHLSCP.second_approved == true)
                        {
                            recCHLSCP.second_approved = false;
                            recCHLSCP.approved_by = CurrentUserContext.AppUserId;
                            isApproved = false;
                            result = recCHLSCP;
                        }
                        else
                        {
                            recCHLSCP.second_approved = true;
                            recCHLSCP.approved_by = CurrentUserContext.AppUserId;
                            isApproved = true;
                            result = recCHLSCP;
                        }
                        await dbContext.SaveChangesAsync();
                        result = recCHLS;
                    }
                    if (isApproved == true)
                    {
                        //approve data in tab olc/transfer
                        var recPT = new processing_transaction();
                        recPT.id = Guid.NewGuid().ToString("N");
                        recPT.created_by = CurrentUserContext.AppUserId;
                        recPT.created_on = System.DateTime.Now;
                        recPT.modified_by = null;
                        recPT.modified_on = null;
                        recPT.is_active = true;
                        recPT.is_default = null;
                        recPT.is_locked = null;
                        recPT.entity_id = null;
                        recPT.owner_id = CurrentUserContext.AppUserId;
                        recPT.organization_id = CurrentUserContext.OrganizationId;
                        recPT.chls_id = key;
                        var conn = dbContext.Database.GetDbConnection();
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            await conn.OpenAsync();
                        }
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                var r = await cmd.ExecuteScalarAsync();
                                recPT.transaction_number = $"CT-{DateTime.Now:yyyyMMdd}-{r}";
                            }
                        }

                        recPT.process_flow_id = recCHLSCP.process_flow_id;
                        recPT.source_shift_id = recCHLS.shift_id;
                        recPT.source_location_id = recCHLSCP.source_location_id;
                        recPT.source_product_id = recCHLS.product_id;
                        recPT.loading_datetime = recCHLSCP.start_time;
                        recPT.loading_quantity = recCHLSCP.quantity;
                        recPT.destination_uom_id = recCHLSCP.uom;
                        recPT.source_uom_id = recCHLSCP.uom;
                        recPT.destination_location_id = recCHLSCP.destination_location_id;
                        recPT.equipment_id = recCHLSCP.equipment_id;
                        recPT.pic = recCHLS.approved_by;
                        recPT.business_unit_id = recCHLS.business_unit_id;
                        recPT.unloading_quantity = recCHLSCP.quantity;

                        dbContext.processing_transaction.Add(recPT);
                        await dbContext.SaveChangesAsync();
                    }
                    result = recCHLSCP;
                    await tx.CommitAsync();
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    //logger.Error(ex.InnerException ?? ex);
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }

        }


        [HttpPost("UploadDocument")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UploadDocument([FromBody] dynamic FileDocument)
        {
            var operationId = (string)FileDocument.operationId;
            using (await GlobalUploadQueue.EnterQueueAsync(operationId, _hubContext))
            {
                await _semaphore.WaitAsync();
                try
                {
                    await _hubContext.Clients.Group(operationId).SendAsync("QueueUpdate", -1);
                    var result = new StandardResult();
                    var records = new List<processing_transaction>();
                    long size = 0;
                    bool isError = false;
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
                    int totalRows = sheet.LastRowNum - sheet.FirstRowNum;
                    using var transaction = await dbContext.Database.BeginTransactionAsync();
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        try
                        {
                            await _hubContext.Clients.Group(operationId).SendAsync("UpdateUploaderProgress", i - sheet.FirstRowNum, totalRows);

                            IRow row = sheet.GetRow(i);
                            if (row == null) continue;
                            //cek row kosong tapi mengandung format cell, dianggap ada datanya, padahal tdk valid
                            if (row.Cells.Count() < 6) continue;

                            var process_flow_id = "";
                            var process_flow = await dbContext.process_flow
                                .Where(o => o.process_flow_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(3)).Trim()).FirstOrDefaultAsync();
                            if (process_flow != null) process_flow_id = process_flow.id.ToString();

                            string equipment_id = "";
                            var equipment = await dbContext.equipment
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.equipment_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower()).FirstOrDefaultAsync();
                            if (equipment != null) equipment_id = equipment.id.ToString();

                            /*var processing_category_id = "";
                            var processing_category = dbContext.processing_category
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                                            o.processing_category_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower()).FirstOrDefault();
                            if (processing_category != null) processing_category_id = processing_category.id.ToString();

                            var transport_id = "";
                            var transport = dbContext.transport
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                                            o.vehicle_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                            if (transport != null) transport_id = transport.id.ToString();

                            var accounting_period_id = "";
                            var accounting_period = dbContext.accounting_period
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                                    o.accounting_period_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower()).FirstOrDefault();
                            if (accounting_period != null) accounting_period_id = accounting_period.id.ToString();*/

                            var source_location_id = "";
                            var source_location = await dbContext.stockpile_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.stockpile_location_code == PublicFunctions.IsNullCell(row.GetCell(5))).FirstOrDefaultAsync();
                            if (source_location != null) source_location_id = source_location.id.ToString();

                            var destination_location_id = "";
                            var destination_location = await dbContext.stockpile_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.stockpile_location_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(6)).ToLower()).FirstOrDefaultAsync();
                            if (destination_location != null) destination_location_id = destination_location.id.ToString();

                            var source_shift_id = "";
                            var shift = await dbContext.shift
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.shift_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower()).FirstOrDefaultAsync();
                            if (shift != null) source_shift_id = shift.id.ToString();

                            var source_product_id = "";
                            var product = await dbContext.product
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.product_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(9)).Trim()).FirstOrDefaultAsync();
                            if (product != null) source_product_id = product.id.ToString();

                            //var source_uom_id = "";
                            //var source_uom = dbContext.uom
                            //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            //        o.uom_symbol.ToLower() == PublicFunctions.IsNullCell(row.GetCell(8)).ToLower()).FirstOrDefault();
                            //if (source_uom != null) source_uom_id = source_uom.id.ToString();

                            /* var destination_shift_id = "";
                             var destination_shift = dbContext.shift
                                 .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                                     o.shift_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(13)).ToLower()).FirstOrDefault();
                             if (destination_shift != null) destination_shift_id = destination_shift.id.ToString();

                             var destination_product_id = "";
                             var destination_product = dbContext.product
                                 .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                                     o.product_name == PublicFunctions.IsNullCell(row.GetCell(14))).FirstOrDefault();
                             if (destination_product != null) destination_product_id = destination_product.id.ToString(); */

                            /*     var despatch_order_id = "";
                                var despatch_order = dbContext.despatch_order
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                                        o.despatch_order_number == PublicFunctions.IsNullCell(row.GetCell(10))).FirstOrDefault();
                                if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();*/

                            var quality_sampling_id = "";
                            var quality_sampling = await dbContext.quality_sampling
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.sampling_number == PublicFunctions.IsNullCell(row.GetCell(10))).FirstOrDefaultAsync();
                            if (quality_sampling != null) quality_sampling_id = quality_sampling.id.ToString();

                            string employee_id = "";
                            var employee = await dbContext.employee
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.employee_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(12)).ToLower()).FirstOrDefaultAsync();
                            if (employee != null) employee_id = employee.id.ToString();

                            var advance_contract_id = "";
                            var advance_contract = await dbContext.advance_contract
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.advance_contract_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(11)).ToLower()).FirstOrDefaultAsync();
                            if (advance_contract != null) advance_contract_id = advance_contract.id.ToString();

                            var business_unit_id = "";
                            var business_unit = await dbContext.business_unit
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.business_unit_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(14)).ToLower()).FirstOrDefaultAsync();
                            if (business_unit != null)
                            {
                                business_unit_id = business_unit.id.ToString();
                            }
                            else
                            {
                                teks += "Error in Line : " + (i + 1) + " ==> Business Unit Not Found" + Environment.NewLine;
                                teks += errormessage + Environment.NewLine + Environment.NewLine;
                                gagal = true;
                                isError = true;
                                break;

                            }

                            var contractor = "";
                            var c = dbContext.contractor.Where(o => o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(15)).ToLower()).FirstOrDefault();
                            if (c != null) contractor = c.id.ToString();
                            else
                            {
                                teks += "Error in Line : " + (i + 1) + " ==> Contractor not found. Please ensure that your data includes contractor information. " + Environment.NewLine;
                                teks += errormessage + Environment.NewLine + Environment.NewLine;
                                gagal = true;
                                break;
                            }

                            var record = dbContext.processing_transaction
                                .Where(o => o.transaction_number.Trim() == PublicFunctions.IsNullCell(row.GetCell(0)).Trim()
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                var e = new entity();
                                e.InjectFrom(record);

                                record.modified_by = CurrentUserContext.AppUserId;
                                record.modified_on = DateTime.Now;

                                record.loading_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                                record.source_shift_id = source_shift_id;
                                record.process_flow_id = process_flow_id;
                                record.equipment_id = equipment_id;
                                record.source_location_id = source_location_id;
                                record.destination_location_id = destination_location_id;
                                record.loading_quantity = PublicFunctions.Desimal(row.GetCell(7));
                                record.unloading_quantity = PublicFunctions.Desimal(row.GetCell(8));
                                record.source_product_id = source_product_id;
                                record.quality_sampling_id = quality_sampling_id;
                                record.advance_contract_id1 = advance_contract_id;
                                record.pic = employee_id;
                                record.note = PublicFunctions.IsNullCell(row.GetCell(13));
                                record.business_unit_id = business_unit_id;
                                record.contractor_id = contractor;
                                if (record.unloading_quantity == null || record.unloading_quantity.Value == 0)
                                {
                                    record.unloading_quantity = record.loading_quantity;
                                }
                                await dbContext.SaveChangesAsync();
                            }
                            else
                            {
                                record = new processing_transaction();
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
                                record.business_unit_id = business_unit_id;

                                var TransactionNumber = "";
                                if (PublicFunctions.IsNullCell(row.GetCell(0)) == "")
                                {
                                    #region Get transaction number
                                    var conn = dbContext.Database.GetDbConnection();
                                    if (conn.State != System.Data.ConnectionState.Open)
                                    {
                                        await conn.OpenAsync();
                                    }
                                    if (conn.State == System.Data.ConnectionState.Open)
                                    {
                                        using (var cmd = conn.CreateCommand())
                                        {
                                            try
                                            {
                                                cmd.CommandText = $"SELECT nextval('seq_processing_number')";
                                                var r = await cmd.ExecuteScalarAsync();
                                                TransactionNumber = $"PC-{DateTime.Now:yyyyMMdd}-{r}";
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error(ex.ToString());
                                                return BadRequest(ex.Message);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else
                                    TransactionNumber = PublicFunctions.IsNullCell(row.GetCell(0));

                                record.transaction_number = TransactionNumber;
                                record.loading_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                                record.source_shift_id = source_shift_id;
                                record.process_flow_id = process_flow_id;
                                record.equipment_id = equipment_id;
                                record.source_location_id = source_location_id;
                                record.destination_location_id = destination_location_id;
                                record.loading_quantity = PublicFunctions.Desimal(row.GetCell(7));
                                record.source_product_id = source_product_id;
                                record.quality_sampling_id = quality_sampling_id;
                                record.advance_contract_id1 = advance_contract_id;
                                record.pic = employee_id;
                                record.note = PublicFunctions.IsNullCell(row.GetCell(13));
                                record.business_unit_id = business_unit_id;
                                record.unloading_quantity = PublicFunctions.Desimal(row.GetCell(8));
                                record.contractor_id = contractor;
                                if (record.unloading_quantity == null || record.unloading_quantity.Value == 0)
                                {
                                    record.unloading_quantity = record.loading_quantity;
                                }
                                dbContext.processing_transaction.Add(record);
                                await dbContext.SaveChangesAsync();
                            }

                            records.Add(record);
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
                        if (isError) break;
                    }
                    wb.Close();
                    if (gagal)
                    {
                        await transaction.RollbackAsync();
                        HttpContext.Session.SetString("errormessage", teks);
                        HttpContext.Session.SetString("filename", "Coal Produce");
                        return BadRequest("File gagal di-upload");
                    }
                    else
                    {
                        await transaction.CommitAsync();
                        return "File berhasil di-upload!";
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }


    }
}

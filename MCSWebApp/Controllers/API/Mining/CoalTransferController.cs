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
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.SignalR;
using System.Threading;

namespace MCSWebApp.Controllers.API.Mining
{
    [Route("api/Mining/[controller]")]
    [ApiController]
    public class CoalTransferController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly IHubContext<ProgressHub> _hubContext;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public CoalTransferController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption, IHubContext<ProgressHub> hubContext)
             : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
            _hubContext = hubContext;
        }
        [HttpGet("NewTransactionNumber")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> NewTransactionNumber()
        {
            string result;

            try
            {
                var conn = dbContext.Database.GetDbConnection();
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {                        
                        if(conn.State != System.Data.ConnectionState.Open)
                        {
                            await conn.OpenAsync();
                        }

                        cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                        var r = await cmd.ExecuteScalarAsync();
                        result = $"HA-{DateTime.Now:yyyyMMdd}-{r}";
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
                }
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}

            return result;
        }

        //[HttpGet("DataGrid")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        //{
        //    return await DataSourceLoader.LoadAsync(dbContext.hauling_transaction
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
                return await DataSourceLoader.LoadAsync(dbContext.coal_transfer
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            logger.Debug($"dt1 = {dt1}");
            logger.Debug($"dt2 = {dt2}");

            return await DataSourceLoader.LoadAsync(dbContext.coal_transfer
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.loading_datetime >= dt1 && o.loading_datetime <= dt2),
                    loadOptions);
        }

        [HttpGet("CHLSCoalTransfer/{tanggal1}/{tanggal2}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> CHLSCoalTransfer(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);
            var category = "Coal Transfer";

            return await DataSourceLoader.LoadAsync(dbContext.chls_hauling.FromSqlRaw(
                "SELECT ch.* " +
                "FROM chls_hauling ch " +
                "LEFT JOIN process_flow pf ON pf.id = ch.process_flow_id " +
                "WHERE pf.process_flow_category = {0} AND ch.start_time >= {1} AND ch.end_time <= {2} AND pf.business_unit_id = {3}", category,dt1,dt2, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                .Where(o=>o.approved == true), loadOptions);
           // return haulings;

        }

        [HttpGet("DataGridCHLS/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGridCHLS(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            logger.Debug($"tanggal1 = {tanggal1}");
            logger.Debug($"tanggal2 = {tanggal2}");

            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_chls
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            logger.Debug($"dt1 = {dt1}");
            logger.Debug($"dt2 = {dt2}");

            return await DataSourceLoader.LoadAsync(dbContext.vw_chls
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.date >= dt1 && o.date <= dt2),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.coal_transfer.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            coal_transfer record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(coal_transfer),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new coal_transfer();
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

                        #region Validation

                        if (record.loading_datetime > record.unloading_datetime)
                            return BadRequest("The Loading Date should not exceed the Unloading Date");

                        // Source location != destination location
                        if (record.source_location_id == record.destination_location_id)
                        {
                            return BadRequest("Source location must be different from destination location");
                        }

                        //if (record.unloading_quantity > record.loading_quantity)
                           // return BadRequest("The Unloading Quantity must not exceed the Loading Quantity");

                        // Capacity
                        /*if (record.transport_id != null)
                        {
                            var tr1 = await dbContext.transport
                                .Where(o => o.id == record.transport_id)
                                .FirstOrDefaultAsync();
                            if(tr1 != null)
                            {
                                if((decimal)(tr1?.capacity ?? 0) < record.loading_quantity)
                                {
                                    return BadRequest("Transport capacity is less than loading quantity");
                                }

                                if(record.unloading_quantity != null)
                                {
                                    if((decimal)(tr1?.capacity ?? 0) < record.unloading_quantity)
                                    {
                                        return BadRequest("Transport capacity is less than unloading quantity");
                                    }
                                }
                            }
                        }*/

                        #endregion

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
                                    cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                    var r = await cmd.ExecuteScalarAsync();
                                    record.transaction_number = $"CT-{DateTime.Now:yyyyMMdd}-{r}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        #endregion

                        dbContext.coal_transfer.Add(record);

                        #region Calculate actual progress claim

                        if (!string.IsNullOrEmpty(record.progress_claim_id))
                        {
                            var pc = await dbContext.progress_claim
                                .Where(o => o.id == record.progress_claim_id)
                                .FirstOrDefaultAsync();
                            if (pc != null)
                            {
                                var actualQty = await dbContext.coal_transfer
                                    .Where(o => o.progress_claim_id == pc.id)
                                    .SumAsync(o => o.unloading_quantity);
                                pc.actual_quantity = actualQty;
                            }
                        }

                        #endregion

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
                    var _record = new DataAccess.Repository.hauling_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.HaulingTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.ContinueJobWith(_jobId, 
                        () => BusinessLogic.Entity.HaulingTransaction.UpdateStockStateAnalyte(connectionString, _record),
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

            var success = false;
            coal_transfer record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.coal_transfer
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

                           // record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            #region Validation

                            // Must be in open accounting period
                            var ap1 = await dbContext.accounting_period
                                .Where(o => o.id == record.accounting_period_id)
                                .FirstOrDefaultAsync();
                            if (ap1 != null && (ap1?.is_closed ?? false))
                            {
                                return BadRequest("Data update is not allowed");
                            }

                            if (record.loading_datetime > record.unloading_datetime)
                                return BadRequest("The Loading Date should not exceed the Unloading Date");

                            // Source location != destination location
                            if (record.source_location_id == record.destination_location_id)
                            {
                                return BadRequest("Source location must be different from destination location");
                            }

                            //if (record.unloading_quantity > record.loading_quantity)
                               // return BadRequest("The Unloading Quantity must not exceed the Loading Quantity");

                            // Capacity
                            /*if (record.transport_id != null)
                            {
                                var tr1 = await dbContext.transport
                                    .Where(o => o.id == record.transport_id)
                                    .FirstOrDefaultAsync();
                                if (tr1 != null)
                                {
                                    if ((decimal)(tr1?.capacity ?? 0) < record.loading_quantity)
                                    {
                                        return BadRequest("Transport capacity is less than loading quantity");
                                    }

                                    if (record.unloading_quantity != null)
                                    {
                                        if ((decimal)(tr1?.capacity ?? 0) < record.unloading_quantity)
                                        {
                                            return BadRequest("Transport capacity is less than unloading quantity");
                                        }
                                    }
                                }
                            } */

                            #endregion

                            #region Calculate actual progress claim

                            if (!string.IsNullOrEmpty(record.progress_claim_id))
                            {
                                var pc = await dbContext.progress_claim
                                    .Where(o => o.id == record.progress_claim_id)
                                    .FirstOrDefaultAsync();
                                if (pc != null)
                                {
                                    var actualQty = await dbContext.coal_transfer
                                        .Where(o => o.progress_claim_id == pc.id)
                                        .SumAsync(o => o.unloading_quantity);
                                    pc.actual_quantity = actualQty;
                                }
                            }

                            #endregion

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
            if (success && record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.hauling_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.HaulingTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.ContinueJobWith(_jobId, 
                        () => BusinessLogic.Entity.HaulingTransaction.UpdateStockStateAnalyte(connectionString, _record),
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

            var success = false;
            coal_transfer record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.coal_transfer
                        .Where(o => o.id == key
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.coal_transfer.Remove(record);

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
            if (success && record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.hauling_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.HaulingTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.ContinueJobWith(_jobId, 
                        () => BusinessLogic.Entity.HaulingTransaction.UpdateStockStateAnalyte(connectionString, _record), 
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

        [HttpGet("ProcessFlowIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProcessFlowIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.process_flow
                    .Where(o => o.process_flow_category == Common.ProcessFlowCategory.COAL_TRANSFER
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => 
                    new 
                    { 
                        Value = o.id, 
                        Text = o.process_flow_name 
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
        public async Task<object> SurveyIdLookup(string SourceLocationId,
            DataSourceLoadOptions loadOptions)
        {
            //logger.Trace($"SourceLocationId = {SourceLocationId}");

            try
            {
                //if (string.IsNullOrEmpty(SourceLocationId))
                //{
                var lookup = dbContext.survey
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && (o.is_draft_survey == null || o.is_draft_survey == false))
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.survey_number, Search = o.survey_number.ToLower() + o.survey_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                //}
                //else
                //{
                //    var lookup = dbContext.survey.FromSqlRaw(
                //          " SELECT s.* FROM survey s "
                //        + " INNER JOIN stock_location sl ON sl.id = s.stock_location_id "
                //        + " WHERE COALESCE(s.is_draft_survey, FALSE) = FALSE "
                //        + " AND s.organization_id = {0} "                        
                //        + " AND sl.id = {1} ",
                //           CurrentUserContext.OrganizationId, SourceLocationId)
                //        .Select(o =>
                //            new
                //            {
                //                value = o.id,
                //                text = o.survey_number,
                //                o.product_id
                //            });
                //    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                //}
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
                    .Select(o => new { Value = o.id, Text = o.equipment_code, Search = o.equipment_code.ToLower() + o.equipment_code.ToUpper() });
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
            //logger.Trace($"ProcessFlowId = {ProcessFlowId}");
            //logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                if (string.IsNullOrEmpty(ProcessFlowId))
                {
                    var lookup = dbContext.vw_stock_location
                        .FromSqlRaw(" SELECT l1.id, l1.stock_location_name, l1.business_area_name, l1.product_id FROM vw_stockpile_location l1 "
                            + " WHERE l1.organization_id = {0} AND l1.business_unit_id = {1} "
                            + " UNION SELECT l2.id, l2.stock_location_name, l2.business_area_name, l2.product_id FROM vw_port_location l2 "
                            + " WHERE l2.organization_id = {0} AND l2.business_unit_id = {1} ",
                                CurrentUserContext.OrganizationId, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = (o.business_area_name != null) ? o.business_area_name + " > " + o.stock_location_name : o.stock_location_name,
                                o.product_id,
                                search = (o.business_area_name != null) ? o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() : o.stock_location_name.ToLower()
                                + o.business_unit_name.ToUpper() + o.stock_location_name.ToUpper()
                            });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.vw_stock_location.FromSqlRaw(
                        " SELECT l1.id, l1.stock_location_name, l1.business_area_name, l1.product_id FROM vw_stockpile_location l1 "
                        + " WHERE l1.organization_id = {0} "
                        + " AND l1.business_area_id IN ( "
                        + "     SELECT ba1.id FROM vw_business_area_structure ba1, process_flow pf1 "
                        + "     WHERE position(pf1.source_location_id in ba1.id_path) > 0"
                        + "         AND pf1.id = {1} "
                        + " ) ", CurrentUserContext.OrganizationId, ProcessFlowId)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.business_area_name + " > " + o.stock_location_name,
                                o.product_id,
                                search = (o.business_area_name != null) ? o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() : o.stock_location_name.ToLower()
                                + o.business_unit_name.ToUpper() + o.stock_location_name.ToUpper()
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

        [HttpGet("DestinationLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DestinationLocationIdLookup(string ProcessFlowId,
            DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (string.IsNullOrEmpty(ProcessFlowId))
                {
                    var lookup = dbContext.vw_stock_location
                        //.FromSqlRaw(" SELECT l1.id, l1.stock_location_name, l1.product_id FROM vw_stockpile_location l1 "
                        //    + " WHERE l1.organization_id = {0} "
                        //    + " UNION SELECT l2.id, l2.stock_location_name, l2.product_id FROM vw_port_location l2 "
                        //    + " WHERE l2.organization_id = {0} ",
                        //        CurrentUserContext.OrganizationId
                        //    )
                        .FromSqlRaw(" SELECT l1.id, l1.stock_location_name, l1.product_id FROM vw_stockpile_location l1 "
                            + " WHERE l1.organization_id = {0} AND l1.business_unit_id = {1} "
                            + " UNION SELECT l2.id, l2.stock_location_name, l2.product_id FROM vw_port_location l2 "
                            + " WHERE l2.organization_id = {0} AND l2.business_unit_id = {1} "
                            + " UNION SELECT l3.id, l3.vehicle_name as stock_location_name, l3.vehicle_id as product_id FROM barge l3 "
                            + " WHERE l3.organization_id = {0} AND l3.business_unit_id = {1} ",
                                CurrentUserContext.OrganizationId, HttpContext.Session.GetString("BUSINESS_UNIT_ID")
                            )
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.stock_location_name,
                                o.product_id,
                                search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper()
                            })
                        .OrderBy(o => o.text);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
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

                       , CurrentUserContext.OrganizationId, ProcessFlowId)
                       .Select(o =>
                           new
                           {
                               value = o.id,
                               //text = o.business_area_name + " > " + o.stock_location_name,
                               text = (o.business_area_name != "" ? o.business_area_name + " > " : "") + o.stock_location_name,
                               urutan = 1,
                               search = (o.business_area_name != null) ? o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() : o.stock_location_name.ToLower()
                                + o.business_unit_name.ToUpper() + o.stock_location_name.ToUpper()
                           })
                       .OrderBy(o => o.text);

                    return await DataSourceLoader.LoadAsync(lookup1, loadOptions);
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
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number, Search = o.despatch_order_number.ToLower() + o.despatch_order_number.ToUpper() });
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
                    .Select(o => new { Value = o.id, Text = o.sampling_number, Search = o.sampling_number.ToLower() + o.sampling_number.ToUpper() });
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
                    .Select(o => new { Value = o.id, Text = o.progress_claim_name, Search = o.progress_claim_name.ToLower() + o.progress_claim_name.ToUpper() });
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
                var record = await dbContext.vw_coal_transfer
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
        public async Task<IActionResult> SaveData([FromBody] coal_transfer Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.coal_transfer
                        .Where(o => o.id == Record.id
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            if (string.IsNullOrEmpty(record.transaction_number))
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
                                            cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                            var r = await cmd.ExecuteScalarAsync();
                                            record.transaction_number = $"HA-{DateTime.Now:yyyyMMdd}-{r}";
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

                            #region Validation

                            // Source location != destination location
                            if (record.source_location_id == record.destination_location_id)
                            {
                                return BadRequest("Source location must be different from destination location");
                            }

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
                                qtyOut.transaction_datetime = record.loading_datetime;
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
                                    transaction_datetime = record.loading_datetime
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
                                qtyIn.transaction_datetime = record.unloading_datetime ?? record.loading_datetime;
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
                                    transaction_datetime = record.unloading_datetime ?? record.loading_datetime
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
                    else if (await mcsContext.CanCreate(dbContext, nameof(coal_transfer),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                            #region Add record

                        record = new coal_transfer();
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
                                    cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                    var r = await cmd.ExecuteScalarAsync();
                                    record.transaction_number = $"HA-{DateTime.Now:yyyyMMdd}-{r}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        #endregion

                        dbContext.coal_transfer.Add(record);

                        #region Validation

                        // Source location != destination location
                        if (record.source_location_id == record.destination_location_id)
                        {
                            return BadRequest("Source location must be different from destination location");
                        }

                        #endregion

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
                                transaction_datetime = record.loading_datetime
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
                                transaction_datetime = record.unloading_datetime ?? record.loading_datetime
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
                    var record = dbContext.coal_transfer
                        .Where(o => o.id == Id
                            && o.organization_id == CurrentUserContext.OrganizationId)
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

                        dbContext.coal_transfer.Remove(record);

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
                        var records = await dbContext.coal_transfer.Where(o => selectedIds.Contains(o.id)).ToListAsync();
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
        [HttpGet("ContractRefIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ContractRefIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.advance_contract
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.advance_contract_number, Search = o.advance_contract_number.ToLower() + o.advance_contract_number.ToUpper() });
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
                            var record = dbContext.coal_transfer
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.coal_transfer.Remove(record);
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
            
            var recCHLSCT = await dbContext.chls_hauling
                            .Where(o => o.id == key)
                            .FirstOrDefaultAsync();
            var recCHLS = await dbContext.chls
                           .Where(o => o.id == recCHLSCT.header_id)
                           .FirstOrDefaultAsync();
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var CT = dbContext.coal_transfer
                   .Where(o => o.chls_id == key)
                   .FirstOrDefault();
                    bool? checkCT = null;
                    
                    if (CT != null)
                    {
                        dbContext.coal_transfer.Remove(CT);
                        await dbContext.SaveChangesAsync();
                        checkCT = true;
                    }
                    if (checkCT == true || CT == null && recCHLSCT != null)
                    {
                        recCHLSCT.modified_by = CurrentUserContext.AppUserId;
                        recCHLSCT.modified_on = System.DateTime.Now;
                        if (recCHLSCT.second_approved == true)
                        {
                            recCHLSCT.second_approved = false;
                            recCHLSCT.approved_by = CurrentUserContext.AppUserId;
                            isApproved = false;
                            result = recCHLSCT;
                        }
                        else
                        {
                            recCHLSCT.second_approved = true;
                            recCHLSCT.approved_by = CurrentUserContext.AppUserId;
                            isApproved = true;
                            result = recCHLSCT;
                        }
                        await dbContext.SaveChangesAsync();
                        result = recCHLS;
                    }
                    if (await mcsContext.CanCreate(dbContext, nameof(chls),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        if (isApproved == true)
                        {
                            //approve data in tab olc/transfer
                            var recCT = new coal_transfer();
                            recCT.id = Guid.NewGuid().ToString("N");
                            recCT.created_by = CurrentUserContext.AppUserId;
                            recCT.created_on = System.DateTime.Now;
                            recCT.modified_by = null;
                            recCT.modified_on = null;
                            recCT.is_active = true;
                            recCT.is_default = null;
                            recCT.is_locked = null;
                            recCT.entity_id = null;
                            recCT.owner_id = CurrentUserContext.AppUserId;
                            recCT.organization_id = CurrentUserContext.OrganizationId;
                            recCT.chls_id = key;
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
                                    recCT.transaction_number = $"CT-{DateTime.Now:yyyyMMdd}-{r}";
                                }
                            }

                            recCT.process_flow_id = recCHLSCT.process_flow_id;
                            recCT.source_shift_id = recCHLS.shift_id;
                            recCT.source_location_id = recCHLSCT.source_location_id;
                            recCT.product_id = recCHLS.product_id;
                            recCT.loading_datetime = (DateTime)recCHLSCT.start_time;
                            recCT.unloading_datetime = (DateTime)recCHLSCT.end_time;
                            recCT.loading_quantity = recCHLSCT.quantity ?? 0;
                            recCT.uom_id = recCHLSCT.uom;
                            recCT.destination_location_id = recCHLSCT.destination_location_id;
                            recCT.equipment_id = recCHLSCT.equipment_id;
                            recCT.pic = recCHLS.approved_by;
                            recCT.business_unit_id = recCHLS.business_unit_id;
                            dbContext.coal_transfer.Add(recCT);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    result = recCHLSCT;
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
                    var records = new List<coal_transfer>();
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
                    int totalRows = sheet.LastRowNum - sheet.FirstRowNum;

                    using var transaction = await dbContext.Database.BeginTransactionAsync();
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        try
                        {
                            await _hubContext.Clients.Group(operationId).SendAsync("UpdateUploaderProgress", i - sheet.FirstRowNum, totalRows);
                            IRow row = sheet.GetRow(i);
                            if (row == null) continue;

                            var process_flow_id = "";
                            var process_flow = dbContext.process_flow
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.process_flow_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower()).FirstOrDefault();
                            if (process_flow != null) process_flow_id = process_flow.id.ToString();

                            var transport_id = "";
                            var truck = dbContext.truck
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.vehicle_id.Trim() == PublicFunctions.IsNullCell(row.GetCell(5)).Trim())
                                .FirstOrDefault();
                            if (truck != null) transport_id = truck.id.ToString();

                            var contractor_id = "";
                            var contractor = dbContext.contractor
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.business_partner_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(16)).Trim())
                                .FirstOrDefault();
                            if (contractor != null) contractor_id = contractor.id.ToString();

                            var source_location_id = "";
                            var source_location = dbContext.stockpile_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.stockpile_location_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(6)).ToLower()).FirstOrDefault();
                            if (source_location != null) source_location_id = source_location.id.ToString();

                            var source_shift_id = "";
                            var shift = dbContext.shift
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.shift_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                            if (shift != null) source_shift_id = shift.id.ToString();

                            var product_id = "";
                            var product = dbContext.product
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.product_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(10)).ToLower()).FirstOrDefault();
                            if (product != null) product_id = product.id.ToString();

                            var uom_id = "";
                            var uom = dbContext.uom
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.uom_name == PublicFunctions.IsNullCell(row.GetCell(11))).FirstOrDefault();
                            if (uom != null) uom_id = uom.id.ToString();

                            var destination_location_id = "";
                            //var destination_location = dbContext.stockpile_location
                            //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            //                o.stock_location_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(7)).ToLower()).FirstOrDefault();

                            var destination_location = dbContext.vw_stockpile_location
                                .FromSqlRaw(" SELECT l1.id, l1.stockpile_location_code FROM vw_stockpile_location l1 "
                                    + " WHERE l1.organization_id = {0} ",
                                        CurrentUserContext.OrganizationId
                                    )
                                .Select(o =>
                                    new
                                    {
                                        id = o.id,
                                        stock_location_code = o.stockpile_location_code
                                    })
                                .Where(o => o.stock_location_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(7)).ToLower()).FirstOrDefault();
                            if (destination_location != null) destination_location_id = destination_location.id.ToString();

                            var equipment_id = "";
                            var equipment = dbContext.equipment
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.equipment_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(21)).Trim())
                                .FirstOrDefault();
                            if (equipment != null) equipment_id = equipment.id.ToString();

                            //  var survey_id = "";
                            // var survey = dbContext.survey
                            //     .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            //                 o.survey_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(14)).ToLower()).FirstOrDefault();
                            //  if (survey != null) survey_id = survey.id.ToString();

                            /* var despatch_order_id = "";
                             var despatch_order = dbContext.despatch_order
                                 .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                                             o.despatch_order_number == PublicFunctions.IsNullCell(row.GetCell(15))).FirstOrDefault();
                             if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();*/

                            var quality_sampling_id = "";
                            var quality_sampling = dbContext.quality_sampling
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.sampling_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(14)).ToLower()).FirstOrDefault();
                            if (quality_sampling != null) quality_sampling_id = quality_sampling.id.ToString();

                            var advance_contract_id = "";
                            var advance_contract = dbContext.advance_contract
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.advance_contract_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(16)).ToLower()).FirstOrDefault();
                            if (advance_contract != null) advance_contract_id = advance_contract.id.ToString();

                            string employee_id = "";
                            var employee = dbContext.employee
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.employee_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(17)).ToLower()).FirstOrDefault();
                            if (employee != null) employee_id = employee.id.ToString();

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
                                            cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                            var r = await cmd.ExecuteScalarAsync();
                                            TransactionNumber = $"HA-{DateTime.Now:yyyyMMdd}-{r}";
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

                            var business_unit_id = "";
                            var business_unit = dbContext.business_unit.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.business_unit_code.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(20)).ToUpper()).FirstOrDefault();
                            if (business_unit != null)
                            {
                                business_unit_id = business_unit.id.ToString();
                            }
                            else
                            {
                                teks += "Error in Line : " + (i + 1) + " ==> Business Unit Not Found" + Environment.NewLine;
                                teks += errormessage + Environment.NewLine + Environment.NewLine;
                                gagal = true;
                                break;

                            }
                            var record = dbContext.coal_transfer
                                .Where(o => o.transaction_number.Trim() == TransactionNumber.Trim() &&
                                            o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                var e = new entity();
                                e.InjectFrom(record);

                                //record.InjectFrom(e);
                                record.modified_by = CurrentUserContext.AppUserId;
                                record.modified_on = DateTime.Now;

                                record.loading_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                                record.unloading_datetime = PublicFunctions.Tanggal(row.GetCell(2));
                                record.source_shift_id = source_shift_id;
                                record.process_flow_id = process_flow_id;
                                record.transport_id = transport_id;
                                record.source_location_id = source_location_id;
                                record.destination_location_id = destination_location_id;
                                record.loading_quantity = PublicFunctions.Desimal(row.GetCell(8));
                                record.uom_id = uom_id;
                                record.product_id = product_id;
                                record.distance = PublicFunctions.Desimal(row.GetCell(11));
                                record.quality_sampling_id = quality_sampling_id;
                                record.advance_contract_id = advance_contract_id;
                                record.pic = employee_id;
                                record.note = PublicFunctions.IsNullCell(row.GetCell(15));
                                record.contractor_id = contractor_id;
                                record.equipment_id = equipment_id;
                                record.gross = PublicFunctions.Desimal(row.GetCell(18));
                                record.tare = PublicFunctions.Desimal(row.GetCell(19));
                                record.business_unit_id = business_unit_id;

                                await dbContext.SaveChangesAsync();
                            }
                            else
                            {
                                record = new coal_transfer();
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

                                record.transaction_number = TransactionNumber;
                                record.loading_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                                record.unloading_datetime = PublicFunctions.Tanggal(row.GetCell(2));
                                record.source_shift_id = source_shift_id;
                                record.process_flow_id = process_flow_id;
                                record.transport_id = transport_id;
                                record.source_location_id = source_location_id;
                                record.destination_location_id = destination_location_id;
                                record.loading_quantity = PublicFunctions.Desimal(row.GetCell(8));
                                record.uom_id = uom_id;
                                record.product_id = product_id;
                                record.distance = PublicFunctions.Desimal(row.GetCell(11));
                                record.quality_sampling_id = quality_sampling_id;
                                record.advance_contract_id = advance_contract_id;
                                record.pic = employee_id;
                                record.note = PublicFunctions.IsNullCell(row.GetCell(15));
                                record.contractor_id = contractor_id;
                                record.equipment_id = equipment_id;
                                record.gross = PublicFunctions.Desimal(row.GetCell(18));
                                record.tare = PublicFunctions.Desimal(row.GetCell(19));

                                dbContext.coal_transfer.Add(record);
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
                    }
                    wb.Close();
                    if (gagal)
                    {
                        await transaction.RollbackAsync();
                        HttpContext.Session.SetString("errormessage", teks);
                        HttpContext.Session.SetString("filename", "Coal Transfer");
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

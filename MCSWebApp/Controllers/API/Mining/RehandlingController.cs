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
using BusinessLogic;
using Omu.ValueInjecter;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Common;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.SignalR;
using System.Threading;

namespace MCSWebApp.Controllers.API.Mining
{
    [Route("api/Mining/[controller]")]
    [ApiController]
    public class RehandlingController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly IHubContext<ProgressHub> _hubContext;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public RehandlingController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption, IHubContext<ProgressHub> hubContext)
             : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
            _hubContext = hubContext;
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
                    .Select(o => new { Value = o.id, Text = o.advance_contract_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpGet("NewTransactionNumber")]
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
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            await conn.OpenAsync();
                        }

                        cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                        var r = await cmd.ExecuteScalarAsync();
                        result = $"RH-{DateTime.Now:yyyyMMdd}-{r}";
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

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.rehandling_transaction
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.rehandling_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            logger.Debug($"dt1 = {dt1}");
            logger.Debug($"dt2 = {dt2}");

            return await DataSourceLoader.LoadAsync(dbContext.rehandling_transaction
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.loading_datetime >= dt1 && o.loading_datetime <= dt2),
                    loadOptions);
        }

        [HttpGet("CHLSRehandling/{tanggal1}/{tanggal2}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> CHLSRehandling(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);
            var category = "Rehandling";

            var haulings = dbContext.chls_hauling.FromSqlRaw(
                "SELECT ch.* " +
                "FROM chls_hauling ch " +
                "LEFT JOIN process_flow pf ON pf.id = ch.process_flow_id " +
                "WHERE pf.process_flow_category = {0} AND ch.start_time >= {1} AND ch.end_time <= {2} AND pf.business_unit_id = {3}", category, dt1, dt2, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                .Where(o => o.approved == true)
                .ToList();
            return haulings;

        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.rehandling_transaction
                .Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            rehandling_transaction record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(rehandling_transaction),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new rehandling_transaction();
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
                            return BadRequest("Source location must be different from destination location");

                        if (record.unloading_quantity > record.loading_quantity)
                            return BadRequest("The Unloading Quantity must not exceed the Loading Quantity");

                        //// Capacity
                        //if (record.transport_id != null)
                        //{
                        //    var tr1 = await dbContext.transport
                        //        .Where(o => o.id == record.transport_id)
                        //        .FirstOrDefaultAsync();
                        //    if (tr1 != null)
                        //    {
                        //        if ((decimal)(tr1?.capacity ?? 0) < record.loading_quantity)
                        //        {
                        //            return BadRequest("Transport capacity is less than loading quantity");
                        //        }

                        //        if (record.unloading_quantity != null)
                        //        {
                        //            if ((decimal)(tr1?.capacity ?? 0) < record.unloading_quantity)
                        //            {
                        //                return BadRequest("Transport capacity is less than unloading quantity");
                        //            }
                        //        }
                        //    }
                        //}

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
                                    record.transaction_number = $"RH-{DateTime.Now:yyyyMMdd}-{r}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        #endregion

                        dbContext.rehandling_transaction.Add(record);

                        #region Calculate actual progress claim

                        if (!string.IsNullOrEmpty(record.progress_claim_id))
                        {
                            var pc = await dbContext.progress_claim
                                .Where(o => o.id == record.progress_claim_id)
                                .FirstOrDefaultAsync();
                            if (pc != null)
                            {
                                var actualQty = await dbContext.rehandling_transaction
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
                    var _record = new DataAccess.Repository.rehandling_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.RehandlingTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.ContinueJobWith(
                        _jobId, 
                        () => BusinessLogic.Entity.RehandlingTransaction.UpdateStockStateAnalyte(connectionString, _record),
                        null,
                        JobContinuationOptions.OnlyOnSucceededState);
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
            rehandling_transaction record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.rehandling_transaction
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

                            if (record.unloading_quantity > record.loading_quantity)
                                return BadRequest("The Unloading Quantity must not exceed the Loading Quantity");

                            // Source location != destination location
                            if (record.source_location_id == record.destination_location_id)
                                return BadRequest("Source location must be different from destination location");

                            //// Capacity
                            //if (record.transport_id != null)
                            //{
                            //    var tr1 = await dbContext.transport
                            //        .Where(o => o.id == record.transport_id)
                            //        .FirstOrDefaultAsync();
                            //    if (tr1 != null)
                            //    {
                            //        if ((decimal)(tr1?.capacity ?? 0) < record.loading_quantity)
                            //        {
                            //            return BadRequest("Transport capacity is less than loading quantity");
                            //        }

                            //        if (record.unloading_quantity != null)
                            //        {
                            //            if ((decimal)(tr1?.capacity ?? 0) < record.unloading_quantity)
                            //            {
                            //                return BadRequest("Transport capacity is less than unloading quantity");
                            //            }
                            //        }
                            //    }
                            //}

                            #endregion

                            #region Calculate actual progress claim

                            if (!string.IsNullOrEmpty(record.progress_claim_id))
                            {
                                var pc = await dbContext.progress_claim
                                    .Where(o => o.id == record.progress_claim_id)
                                    .FirstOrDefaultAsync();
                                if (pc != null)
                                {
                                    var actualQty = await dbContext.rehandling_transaction
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
                    var _record = new DataAccess.Repository.rehandling_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.RehandlingTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.ContinueJobWith(
                        _jobId,
                        () => BusinessLogic.Entity.RehandlingTransaction.UpdateStockStateAnalyte(connectionString, _record),
                        null,
                        JobContinuationOptions.OnlyOnSucceededState);
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
            rehandling_transaction record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.rehandling_transaction
                        .Where(o => o.id == key
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.rehandling_transaction.Remove(record);

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
                    var _record = new DataAccess.Repository.rehandling_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.RehandlingTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.ContinueJobWith(
                        _jobId,
                        () => BusinessLogic.Entity.RehandlingTransaction.UpdateStockStateAnalyte(connectionString, _record),
                        null,
                        JobContinuationOptions.OnlyOnSucceededState);
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
                    .Where(o => o.process_flow_category == Common.ProcessFlowCategory.REHANDLING
                        && o.organization_id == CurrentUserContext.OrganizationId)
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
        public async Task<object> SurveyIdLookup(string SourceLocationId,
            DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"SourceLocationId = {SourceLocationId}");

            try
            {
                if (string.IsNullOrEmpty(SourceLocationId))
                {
                    var lookup = dbContext.survey
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.survey_number, search = o.survey_number.ToLower() + o.survey_number.ToUpper() });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.survey.FromSqlRaw(
                          " SELECT s.* FROM survey s "
                        + " INNER JOIN stock_location sl ON sl.id = s.stock_location_id "
                        + " WHERE s.organization_id = {0} "
                        + " AND sl.id = {1} ",
                            CurrentUserContext.OrganizationId, SourceLocationId)
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

        [HttpGet("EquipmentIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> EquipmentIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.equipment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new
                    {
                        Value = o.id,
                        Text = o.equipment_code + " - " + o.equipment_name,
                        search = o.equipment_code.ToLower() + " - " + o.equipment_name.ToLower() + o.equipment_code.ToUpper() + " - " + o.equipment_name.ToUpper()
                    });
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
            logger.Trace($"ProcessFlowId = {ProcessFlowId}");
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

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
                                //text = o.business_area_name + " > " + o.stock_location_name,
                                text = (o.business_area_name != null) ? o.business_area_name + " > " + o.stock_location_name : o.stock_location_name,
                                o.product_id,
                                search = (o.business_area_name != null) ? o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower() : o.stock_location_name.ToLower()
                                + ((o.business_area_name != null) ? o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper() : o.stock_location_name.ToUpper()),
                            });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.vw_stock_location.FromSqlRaw(
                        " SELECT l1.id, l1.stock_location_name, l1.business_area_name, l1.product_id FROM vw_stockpile_location l1 "
                        + " WHERE l1.organization_id = {0} AND l1.business_unit_id = {2} "
                        + " AND l1.business_area_id IN ( "
                        + "     SELECT ba1.id FROM vw_business_area_structure ba1, process_flow pf1 "
                        + "     WHERE position(pf1.source_location_id in ba1.id_path) > 0"
                        + "         AND pf1.id = {1} "
                        + " ) UNION "
                        + " SELECT l2.id, l2.stock_location_name, l2.business_area_name, l2.product_id FROM vw_port_location l2 "
                        + " WHERE l2.organization_id = {0} AND l2.business_unit_id = {2} "
                        + " AND l2.business_area_id IN ( "
                        + "     SELECT ba2.id FROM vw_business_area_structure ba2, process_flow pf2 "
                        + "     WHERE position(pf2.source_location_id in ba2.id_path) > 0"
                        + "         AND pf2.id = {1} "
                        + " ) ", CurrentUserContext.OrganizationId, ProcessFlowId, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.business_area_name + " > " + o.stock_location_name,
                                o.product_id,
                                search = o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower()
                                + o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper(),
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
            logger.Trace($"ProcessFlowId = {ProcessFlowId}");
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                if (string.IsNullOrEmpty(ProcessFlowId))
                {
                    var lookup = dbContext.vw_stock_location
                        .FromSqlRaw(" SELECT l1.id, l1.stock_location_name, l1.product_id FROM vw_stockpile_location l1 "
                            + " WHERE l1.organization_id = {0} AND l1.business_unit_id = {1} "
                            + " UNION SELECT l2.id, l2.stock_location_name, l2.product_id FROM vw_port_location l2 "
                            + " WHERE l2.organization_id = {0} AND l2.business_unit_id = {1} ",
                                CurrentUserContext.OrganizationId, HttpContext.Session.GetString("BUSINESS_UNIT_ID")
                            )
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.stock_location_name,
                                o.product_id,
                                search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper()
                            });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.vw_stock_location.FromSqlRaw(
                        " SELECT l1.id, l1.stock_location_name, l1.business_area_name, l1.product_id FROM vw_stockpile_location l1 "
                        + " WHERE l1.organization_id = {0} AND l1.business_unit_id = {2} "
                        + " AND l1.business_area_id IN ( "
                        + "     SELECT ba1.id FROM vw_business_area_structure ba1, process_flow pf1 "
                        + "     WHERE position(pf1.destination_location_id in ba1.id_path) > 0"
                        + "         AND pf1.id = {1} "
                        + " ) UNION "
                        + " SELECT l2.id, l2.stock_location_name, l2.business_area_name, l2.product_id FROM vw_port_location l2 "
                        + " WHERE l2.organization_id = {0} AND l2.business_unit_id = {2} "
                        + " AND l2.business_area_id IN ( "
                        + "     SELECT ba2.id FROM vw_business_area_structure ba2, process_flow pf2 "
                        + "     WHERE position(pf2.destination_location_id in ba2.id_path) > 0"
                        + "         AND pf2.id = {1} "
                        + " ) ", CurrentUserContext.OrganizationId, ProcessFlowId, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.business_area_name + " > " + o.stock_location_name,
                                o.product_id,
                                search = o.business_area_name.ToLower() + " > " + o.stock_location_name.ToLower()
                                + o.business_area_name.ToUpper() + " > " + o.stock_location_name.ToUpper(),
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
                var record = await dbContext.vw_rehandling_transaction
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                return Ok(record);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] rehandling_transaction Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.rehandling_transaction
                        .Where(o => o.id == Record.id
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
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
                            record.modified_on = DateTime.Now;

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
                    else if (await mcsContext.CanCreate(dbContext, nameof(rehandling_transaction),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new rehandling_transaction();
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

                        dbContext.rehandling_transaction.Add(record);

                        #region Update stockpile state

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
                    var record = dbContext.rehandling_transaction
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

                        dbContext.rehandling_transaction.Remove(record);

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
                            var record = dbContext.rehandling_transaction
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.rehandling_transaction.Remove(record);
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
                        var records = await dbContext.rehandling_transaction.Where(o => selectedIds.Contains(o.id)).ToListAsync();
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
        [HttpPost("ApproveUnapprove")]    //**** tanpa trigger trf_sales_invoice_approval_sync di table sales_invoice_approval
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ApproveUnapprove([FromForm] string key, [FromForm] string values)
        {
            dynamic result;
            bool? isApproved = null;

            var recCHLSRH = await dbContext.chls_hauling
                            .Where(o => o.id == key)
                            .FirstOrDefaultAsync();
            var recCHLS = await dbContext.chls
                           .Where(o => o.id == recCHLSRH.header_id)
                           .FirstOrDefaultAsync();
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var RH = dbContext.rehandling_transaction
                   .Where(o => o.chls_id == key)
                   .FirstOrDefault();
                    bool? checkRH = null;

                    if (RH != null)
                    {
                        dbContext.rehandling_transaction.Remove(RH);
                        await dbContext.SaveChangesAsync();
                        checkRH = true;
                    }
                    if (checkRH == true || RH == null && recCHLSRH != null)
                    {
                        recCHLSRH.modified_by = CurrentUserContext.AppUserId;
                        recCHLSRH.modified_on = System.DateTime.Now;
                        if (recCHLSRH.second_approved == true)
                        {
                            recCHLSRH.second_approved = false;
                            recCHLSRH.approved_by = CurrentUserContext.AppUserId;
                            isApproved = false;
                            result = recCHLSRH;
                        }
                        else
                        {
                            recCHLSRH.second_approved = true;
                            recCHLSRH.approved_by = CurrentUserContext.AppUserId;
                            isApproved = true;
                            result = recCHLSRH;
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
                            var recRH = new rehandling_transaction();
                            recRH.id = Guid.NewGuid().ToString("N");
                            recRH.created_by = CurrentUserContext.AppUserId;
                            recRH.created_on = System.DateTime.Now;
                            recRH.modified_by = null;
                            recRH.modified_on = null;
                            recRH.is_active = true;
                            recRH.is_default = null;
                            recRH.is_locked = null;
                            recRH.entity_id = null;
                            recRH.owner_id = CurrentUserContext.AppUserId;
                            recRH.organization_id = CurrentUserContext.OrganizationId;
                            recRH.chls_id = key;
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
                                    recRH.transaction_number = $"RH-{DateTime.Now:yyyyMMdd}-{r}";
                                }
                            }

                            recRH.process_flow_id = recCHLSRH.process_flow_id;
                            recRH.source_shift_id = recCHLS.shift_id;
                            recRH.source_location_id = recCHLSRH.source_location_id;
                            recRH.product_id = recCHLS.product_id;
                            recRH.loading_datetime = (DateTime)recCHLSRH.start_time;
                            recRH.loading_quantity = recCHLSRH.quantity ?? 0;
                            recRH.uom_id = recCHLSRH.uom;
                            recRH.destination_location_id = recCHLSRH.destination_location_id;
                            recRH.equipment_id = recCHLSRH.equipment_id;
                            recRH.pic = recCHLS.approved_by;
                            recRH.business_unit_id = recCHLS.business_unit_id;
                            dbContext.rehandling_transaction.Add(recRH);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    result = recCHLSRH;
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
                    var records = new List<rehandling_transaction>();
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
                            //cek row kosong tapi mengandung format cell, dianggap ada datanya, padahal tdk valid
                            if (row.Cells.Count() < 5) continue;

                            var process_flow_id = "";
                            var process_flow = dbContext.process_flow
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.process_flow_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower())
                                .FirstOrDefault();
                            if (process_flow != null) process_flow_id = process_flow.id.ToString();

                            var transport_id = "";
                            var truck = dbContext.truck
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.vehicle_id.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower())
                                .FirstOrDefault();
                            if (truck != null) transport_id = truck.id.ToString();

                            string equipment_id = "";
                            var equipment = dbContext.equipment
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.equipment_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower())
                                .FirstOrDefault();
                            if (equipment != null) equipment_id = equipment.id.ToString();

                            var source_location_id = "";
                            var stockpile_location = dbContext.stockpile_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.stockpile_location_code == PublicFunctions.IsNullCell(row.GetCell(6))).FirstOrDefault();
                            if (stockpile_location != null)
                                source_location_id = stockpile_location.id.ToString();
                            else
                            {
                                var port_location = dbContext.port_location
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                        o.port_location_code == PublicFunctions.IsNullCell(row.GetCell(6))).FirstOrDefault();
                                if (port_location != null)
                                    source_location_id = port_location.id.ToString();
                                else
                                {
                                    teks += "==>Error Sheet 1, Line " + (i + 1) + ", 'Source' is empty or not found!" + Environment.NewLine;
                                    gagal = true;
                                    break;
                                }
                            }

                            var destination_location_id = "";
                            var stockpile_location2 = dbContext.stockpile_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.stockpile_location_code == PublicFunctions.IsNullCell(row.GetCell(7)))
                                .FirstOrDefault();
                            if (stockpile_location2 != null)
                                destination_location_id = stockpile_location2.id.ToString();
                            else
                            {
                                var port_location = dbContext.port_location
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                        o.port_location_code == PublicFunctions.IsNullCell(row.GetCell(7))).FirstOrDefault();
                                if (port_location != null)
                                    destination_location_id = port_location.id.ToString();
                                else
                                {
                                    var barge = dbContext.barge
                                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                            o.vehicle_id.ToUpper().Trim() == PublicFunctions.IsNullCell(row.GetCell(7)).ToUpper().Trim())
                                        .FirstOrDefault();
                                    if (barge != null)
                                        destination_location_id = barge.id.ToString();
                                    else
                                    {
                                        teks += "==>Error Sheet 1, Line " + (i + 1) + ", 'Destination' is empty or not found!" + Environment.NewLine;
                                        gagal = true;
                                        break;
                                    }
                                }
                            }

                            var uom_id = "";
                            var uom = dbContext.uom
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.uom_name == PublicFunctions.IsNullCell(row.GetCell(9))).FirstOrDefault();
                            if (uom != null) uom_id = uom.id.ToString();

                            var product_id = "";
                            var product = dbContext.product
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.product_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(10)).ToLower())
                                .FirstOrDefault();
                            if (product != null) product_id = product.id.ToString();

                            var source_shift_id = "";
                            var shift = dbContext.shift
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.shift_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower()).FirstOrDefault();
                            if (shift != null) source_shift_id = shift.id.ToString();

                            var quality_sampling_id = "";
                            var quality_sampling = dbContext.quality_sampling
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.sampling_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(12)).ToLower())
                                .FirstOrDefault();
                            if (quality_sampling != null) quality_sampling_id = quality_sampling.id.ToString();

                            var despatch_order_id = "";
                            var despatch_order = dbContext.despatch_order
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.despatch_order_number == PublicFunctions.IsNullCell(row.GetCell(13))).FirstOrDefault();
                            if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();

                            var advance_contract_id = "";
                            var advance_contract = dbContext.advance_contract
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.advance_contract_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(14)).ToLower())
                                .FirstOrDefault();
                            if (advance_contract != null) advance_contract_id = advance_contract.id.ToString();

                            string employee_id = "";
                            var employee = dbContext.employee
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.employee_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(15)).ToLower())
                                .FirstOrDefault();
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
                                            TransactionNumber = $"RH-{DateTime.Now:yyyyMMdd}-{r}";
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

                            var business_unit = "";
                            var bu = dbContext.business_unit
                                .Where(o => o.business_unit_code == PublicFunctions.IsNullCell(row.GetCell(17))).FirstOrDefault();
                            if (business_unit != null)
                            {
                                business_unit = bu.id.ToString();
                            }
                            else
                            {
                                teks += "Error in Line : " + (i + 1) + " ==> Business Unit Not Found" + Environment.NewLine;
                                teks += errormessage + Environment.NewLine + Environment.NewLine;
                                gagal = true;
                                break;

                            }
                            var contractor = "";
                            var c = dbContext.contractor.Where(o => o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(18)).ToLower()).FirstOrDefault();
                            if (c != null) contractor = c.id.ToString();

                            var record = dbContext.rehandling_transaction
                                .Where(o => o.transaction_number.ToLower() == TransactionNumber.ToLower()
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();

                            if (record == null)
                            {
                                record = new rehandling_transaction();
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

                                record.transaction_number = TransactionNumber;
                                record.loading_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                                record.source_shift_id = source_shift_id;
                                record.process_flow_id = process_flow_id;
                                record.transport_id = transport_id;
                                record.equipment_id = equipment_id;
                                record.source_location_id = source_location_id;
                                record.destination_location_id = destination_location_id;
                                record.loading_quantity = PublicFunctions.Desimal(row.GetCell(8));
                                record.uom_id = uom_id;
                                record.product_id = product_id;
                                record.distance = PublicFunctions.Desimal(row.GetCell(11));
                                record.quality_sampling_id = quality_sampling_id;
                                record.despatch_order_id = despatch_order_id;
                                record.advance_contract_id1 = advance_contract_id;
                                record.pic = employee_id;
                                record.note = PublicFunctions.IsNullCell(row.GetCell(16));
                                record.business_unit_id = business_unit;
                                record.contractor_id = contractor;

                                dbContext.rehandling_transaction.Add(record);
                                await dbContext.SaveChangesAsync();
                            }
                            else
                            {
                                var e = new entity();
                                e.InjectFrom(record);

                                record.modified_by = CurrentUserContext.AppUserId;
                                record.modified_on = DateTime.Now;

                                record.loading_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                                record.source_shift_id = source_shift_id;
                                record.process_flow_id = process_flow_id;
                                record.transport_id = transport_id;
                                record.equipment_id = equipment_id;
                                record.source_location_id = source_location_id;
                                record.destination_location_id = destination_location_id;
                                record.loading_quantity = PublicFunctions.Desimal(row.GetCell(8));
                                record.uom_id = uom_id;
                                record.product_id = product_id;
                                record.distance = PublicFunctions.Desimal(row.GetCell(11));
                                record.quality_sampling_id = quality_sampling_id;
                                record.despatch_order_id = despatch_order_id;
                                record.advance_contract_id1 = advance_contract_id;
                                record.pic = employee_id;
                                record.note = PublicFunctions.IsNullCell(row.GetCell(16));
                                record.business_unit_id = business_unit;
                                record.contractor_id = contractor;

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
                        HttpContext.Session.SetString("filename", "Rehandling");
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

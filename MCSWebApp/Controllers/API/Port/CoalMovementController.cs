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
using DocumentFormat.OpenXml.Office2010.Excel;

namespace MCSWebApp.Controllers.API.Port
{
    [Route("api/Port/[controller]")]
    [ApiController]
    public class CoalMovementController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public CoalMovementController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("ShippingOrderIdLookup")]
        public async Task<object> ShippingOrderIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
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
            return await DataSourceLoader.LoadAsync(dbContext.coal_movement
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.coal_movement
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            logger.Debug($"dt1 = {dt1}");
            logger.Debug($"dt2 = {dt2}");

            return await DataSourceLoader.LoadAsync(dbContext.coal_movement
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.loading_datetime >= dt1 && o.loading_datetime <= dt2),
                    loadOptions);
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
            coal_movement record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(rehandling_transaction),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new coal_movement();
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
                                    record.transaction_number = $"CM-{DateTime.Now:yyyyMMdd}-{r}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        #endregion

                        dbContext.coal_movement.Add(record);

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
            coal_movement record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.coal_movement
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
            coal_movement record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.coal_movement
                        .Where(o => o.id == key
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.coal_movement.Remove(record);

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

        [HttpGet("SourceLocationIdLookupB")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SourceLocationIdLookupB(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup1 = dbContext.vw_stockpile_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                    new
                    {
                        Value = o.id,
                        Text = o.business_area_name + " > " + o.stock_location_name,
                        SortNum = 1
                    });
                var lookup2 = dbContext.barge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o =>
                    new
                    {
                        Value = o.id,
                        Text = o.vehicle_name,
                        SortNum = 2
                    });
                var lookup = lookup1.Union(lookup2).OrderBy(o => o.SortNum).ThenBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DestinationLocationIdLookupB")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DestinationLocationIdLookupB(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup1 = dbContext.stockpile_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                    new
                    {
                        Value = o.id,
                        Text = o.stock_location_name
                    });
                var lookup2 = dbContext.vessel
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                    new
                    {
                        Value = o.id,
                        Text = o.vehicle_name
                    });
                var lookup = lookup1.Union(lookup2);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProcessFlowIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProcessFlowIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.process_flow
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
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

        [HttpGet("VoyageNumberLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> VoyageNumberLookup(DataSourceLoadOptions loadOptions, string id)
        {
            try
            {
                var destination = dbContext.master_list.Where(o => o.item_name == "NPLCT").Where(o => o.item_group == "sils-barging-type").FirstOrDefault();
                if (id == "newData")
                {
                    var lookup = dbContext.barge_rotation
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.destination_location == destination.id)
                    .OrderByDescending(o => o.created_on)
                    .Select(o => new { Value = o.id, Text = o.voyage_number, search = o.voyage_number.ToLower() + o.voyage_number.ToUpper() });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else if (id == null)
                {
                    var lookup = dbContext.barge_rotation
                                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                        .Select(o => new { Value = o.id, Text = o.voyage_number, search = o.voyage_number.ToLower() + o.voyage_number.ToUpper() });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.barge_rotation.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.destination_location == destination.id)
                        .OrderByDescending(o => o.created_on)
                        .Select(o => new { Value = o.id, Text = o.voyage_number, search = o.voyage_number.ToLower() + o.voyage_number.ToUpper() });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
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
                        .Select(o => new { Value = o.id, Text = o.survey_number });
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
                                o.product_id
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
                    .Select(o => new { Value = o.id, Text = o.equipment_code + " - " + o.equipment_name });
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
                                o.product_id
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
                                o.product_id
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
                                o.product_id
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
                                o.product_id
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
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number });
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
                    //   .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sampling_number });
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
                    .Select(o => new { Value = o.id, Text = o.progress_claim_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        // KHUSUS COAL MOVEMENT TIDAK DI FILTER BUSINESS UNIT
        [HttpGet("ProductIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProductIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.product
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.product_name, Search = o.product_name.ToLower() + o.product_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("TransportIdLookup")]
        public async Task<object> TransportIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var equipments = dbContext.vw_equipment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new
                    {
                        Value = o.id,
                        Text = o.equipment_code + " - " + o.equipment_name,
                        Type = o.equipment_name,
                        Search = o.equipment_code.ToLower() + " - " + o.equipment_name.ToLower()
                    + o.equipment_code.ToUpper() + " - " + o.equipment_name.ToUpper()
                    });
                var trucks = dbContext.truck
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.vehicle_id + " - " + o.vehicle_name, Type = o.vehicle_name, Search = o.vehicle_id.ToLower() + " - " + o.vehicle_name.ToLower() + o.vehicle_id.ToUpper() + " - " + o.vehicle_name.ToUpper() });

                var lookup = equipments.Union(trucks).OrderBy(o => o.Text);
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
                            var record = dbContext.coal_movement
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.coal_movement.Remove(record);
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

        [HttpGet("GetItemsById")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetItemsById(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.lq_proportion
                .Where(o => o.header_id == Id && o.module == "Coal-Movement"
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertItemData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            lq_proportion record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(lq_proportion),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new lq_proportion();
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
                        record.module = "Coal-Movement";
                        var header = dbContext.coal_movement
                            .Where(o => o.id == record.header_id)
                            .FirstOrDefault();
                        /*var detail = dbContext.barging_loading_detail
                           .Where(x => x.header_id == record.header_id)
                           .ToList();
                        decimal? sum = 0;
                        if (detail != null)
                        {
                            foreach (var data in detail)
                            {
                                sum = sum + data.quantity;
                            }
                        }
                        sum = sum + record.quantity;*/
                        // decimal? quantity = 0;
                        /*var hasil = ((header.beltscale_quantity - header.quantity) / header.quantity) * 100;
                        if (hasil >= -1 || hasil <= 1)
                        {
                            quantity = header.quantity == 0 ? 1 : header.quantity;

                        }
                        else
                        {
                            quantity = header.beltscale_quantity == 0 ? 1 : header.beltscale_quantity;
                        }*/
                        var quantity = header.loading_quantity == 0 ? 1 : header.loading_quantity;
                        var presentage = (record.quantity / quantity) * 100;
                        record.presentage = presentage;
                        dbContext.lq_proportion.Add(record);

                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();
                        //}

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

        [HttpPut("UpdateItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateItemData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.lq_proportion
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

                        var header = dbContext.coal_movement
                            .Where(o => o.id == record.header_id).FirstOrDefault();
                        /*var detail = dbContext.barging_loading_detail
                           .Where(x => x.header_id == record.header_id)
                           .ToList();
                        decimal? sum = 0;
                        if (detail != null)
                        {
                            foreach (var data in detail)
                            {
                                sum = sum + data.quantity;
                            }
                        }*/
                        //sum = sum + record.quantity;
                        var quantity = header.loading_quantity == 0 ? 1 : header.loading_quantity;
                        var presentage = (record.quantity / quantity) * 100;
                        record.presentage = presentage;

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

        [HttpDelete("DeleteItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteItemData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {

                var record = dbContext.lq_proportion
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.lq_proportion.Remove(record);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        return BadRequest("User is not authorized.");
                    }
                }
                return Ok();
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
            var records = new List<coal_movement>();
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
                    //cek row kosong tapi mengandung format cell, dianggap ada datanya, padahal tdk valid
                    if (row.Cells.Count() < 5) continue;

                    var process_flow_id = "";
                    var process_flow = dbContext.process_flow
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.process_flow_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower())
                        .FirstOrDefault();
                    if (process_flow != null) process_flow_id = process_flow.id.ToString();

                    var so_id = "";
                    var truck = dbContext.despatch_order
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.despatch_order_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower())
                        .FirstOrDefault();
                    if (truck != null) so_id = truck.id.ToString();
                    /*else
                    {
                        var equipment1 = dbContext.equipment
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                o.equipment_code == PublicFunctions.IsNullCell(row.GetCell(4))).FirstOrDefault();
                        if (equipment1 != null)
                            transport_id = equipment1.id.ToString();
                        else
                        {
                            teks += "==>Error Sheet 1, Line " + (i + 1) + ", 'Transport' is empty or not found!" + Environment.NewLine;
                            gagal = true;
                            break;
                        }
                    }*/
                    string equipment_id = "";
                    var equipment = dbContext.equipment
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.equipment_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(10)).ToLower())
                        .FirstOrDefault();
                    if (equipment != null) equipment_id = equipment.id.ToString();

                    var source_location_id = "";
                    var stockpile_location = dbContext.stockpile_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.stockpile_location_code == PublicFunctions.IsNullCell(row.GetCell(5))).FirstOrDefault();
                    if (stockpile_location != null)
                        source_location_id = stockpile_location.id.ToString();
                    else
                    {
                        var port_location = dbContext.barge
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                o.vehicle_id == PublicFunctions.IsNullCell(row.GetCell(5))).FirstOrDefault();
                        if (port_location != null)
                            source_location_id = port_location.id.ToString();
                    }

                    var destination_location_id = "";
                    var stockpile_location2 = dbContext.stockpile_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.stockpile_location_code == PublicFunctions.IsNullCell(row.GetCell(6)))
                        .FirstOrDefault();
                    if (stockpile_location2 != null)
                        destination_location_id = stockpile_location2.id.ToString();
                    else
                    {
                        var port_location = dbContext.vessel
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                o.vehicle_name == PublicFunctions.IsNullCell(row.GetCell(6))).FirstOrDefault();
                        if (port_location != null)
                            destination_location_id = port_location.id.ToString();
                        /*else
                        {
                                teks += "==>Error Sheet 1, Line " + (i + 1) + ", 'Destination' is empty or not found!" + Environment.NewLine;
                                gagal = true;
                                break;
                        }*/
                    }

                    var product_id = "";
                    var product = dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.product_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(8)).ToLower())
                        .FirstOrDefault();
                    if (product != null) product_id = product.id.ToString();

                    var source_shift_id = "";
                    var shift = dbContext.shift
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.shift_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower()).FirstOrDefault();
                    if (shift != null) source_shift_id = shift.id.ToString();

                    var contractor = "";
                    var cc = await dbContext.contractor.Where(o => o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(14)).ToLower()).FirstOrDefaultAsync();
                    if (cc != null) contractor = cc.id.ToString();

                    var quality_sampling_id = "";
                    var quality_sampling = dbContext.quality_sampling
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sampling_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(9)).ToLower())
                        .FirstOrDefault();
                    if (quality_sampling != null) quality_sampling_id = quality_sampling.id.ToString();

                    var business_unit_id = "";
                    var BU = dbContext.business_unit.Where(o => o.business_unit_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(12)).ToLower()).FirstOrDefault();
                    if (BU != null)
                    {
                        business_unit_id = BU.id.ToString();
                    }
                    else
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Business Unit Not Found" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        break;

                    }

                    string pic = "";
                    var employee = dbContext.employee
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.employee_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(11)).ToLower())
                        .FirstOrDefault();
                    if (employee != null) pic = employee.id.ToString();

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
                                    TransactionNumber = $"CM-{DateTime.Now:yyyyMMdd}-{r}";
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

                    var record = dbContext.coal_movement
                        .Where(o => o.transaction_number.ToLower() == TransactionNumber.ToLower()
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record == null)
                    {
                        record = new coal_movement();
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
                        record.business_unit_id = business_unit_id; // Row 12

                        record.transaction_number = TransactionNumber; // Row 0
                        record.source_shift_id = source_shift_id; // Row 2
                        record.process_flow_id = process_flow_id; // Row 3
                        record.shipping_order_id = so_id; // Row 4
                        record.equipment_id = equipment_id; // Row 10
                        record.source_location_id = source_location_id; // Row 5
                        record.destination_location_id = destination_location_id; // Row 6
                        record.loading_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                        record.loading_quantity = PublicFunctions.Desimal(row.GetCell(7));
                        record.product_id = product_id; // Row 8
                        record.quality_sampling_id = quality_sampling_id; // Row 9
                        record.pic = pic; // Row 11
                        record.note = PublicFunctions.IsNullCell(row.GetCell(13));
                        record.contractor_id = contractor;

                        dbContext.coal_movement.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        // record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.transaction_number = TransactionNumber;
                        record.source_shift_id = source_shift_id;
                        record.process_flow_id = process_flow_id;
                        record.shipping_order_id = so_id; // Row 4
                        record.equipment_id = equipment_id;
                        record.source_location_id = source_location_id;
                        record.destination_location_id = destination_location_id;
                        record.loading_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                        record.loading_quantity = PublicFunctions.Desimal(row.GetCell(7));
                        record.product_id = product_id;
                        record.quality_sampling_id = quality_sampling_id;
                        record.pic = pic;
                        record.note = PublicFunctions.IsNullCell(row.GetCell(13));
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

            /* sheet = wb.GetSheetAt(1); //*** detail sheet
             for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
             {
                 try
                 {
                     IRow row = sheet.GetRow(i);
                     if (row == null) continue;

                     var business_unit = "";
                     var BU = dbContext.business_unit.Where(o => o.business_unit_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower()).FirstOrDefault();
                     if (BU != null) business_unit = BU.id.ToString();

                     var product = "";
                     var p = dbContext.product.Where(o => o.product_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower()).FirstOrDefault();
                     if (p != null) product = p.id.ToString();

                     var contractor = "";
                     var c = dbContext.contractor.Where(o => o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                     if (c != null) contractor = c.id.ToString();

                     var header = dbContext.coal_movement.Where(o => o.transaction_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()).FirstOrDefault();

                     var record = dbContext.lq_proportion
                         .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                             o.header_id == header.id)
                         .FirstOrDefault();
                     if (record != null)
                     {

                         record.modified_by = CurrentUserContext.AppUserId;
                         record.modified_on = DateTime.Now;

                         record.business_unit_id = business_unit;
                         record.product_id = product;
                         record.contractor_id = contractor;
                         record.quantity = PublicFunctions.Desimal(row.GetCell(4));
                         record.presentage = PublicFunctions.Desimal(row.GetCell(5));
                         record.adjustment = PublicFunctions.Desimal(row.GetCell(6));
                         await dbContext.SaveChangesAsync();
                     }
                     else
                     {
                         record = new lq_proportion();
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
                         record.business_unit_id = business_unit;
                         record.product_id = product;
                         record.contractor_id = contractor;
                         record.quantity = PublicFunctions.Desimal(row.GetCell(4));
                         record.presentage = PublicFunctions.Desimal(row.GetCell(5));
                         record.adjustment = PublicFunctions.Desimal(row.GetCell(6));

                         dbContext.lq_proportion.Add(record);
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
             }*/
            wb.Close();
            if (gagal)
            {
                await transaction.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "CoalMovement");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                /*
                try
                {
                    if (!gagal && records != null && records.Count > 0)
                    {
                        var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;

                        foreach (var record in records)
                        {
                            try
                            {
                                var _record = new DataAccess.Repository.rehandling_transaction();
                                _record.InjectFrom(record);
                                var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.RehandlingTransaction.UpdateStockState(connectionString, _record));
                                _backgroundJobClient.ContinueJobWith(_jobId, () => BusinessLogic.Entity.RehandlingTransaction.UpdateStockStateAnalyte(connectionString, _record));
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                */
                return "File berhasil di-upload!";
            }
        }

    }
}

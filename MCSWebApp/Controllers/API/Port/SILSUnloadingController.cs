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
using Microsoft.Data.SqlClient;
using DataAccess.EFCore.Repository;
using DataAccess.Select2;
using Microsoft.AspNetCore.Hosting;
using BusinessLogic;
using BusinessLogic.Entity;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Common;
using DocumentFormat.OpenXml.Office2010.Excel;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using System.Xml.Linq;
using NPOI.OpenXmlFormats.Spreadsheet;
using DocumentFormat.OpenXml.InkML;

namespace MCSWebApp.Controllers.API.Port
{
    [Route("api/Port/[controller]")]
    [ApiController]
    public class SILSUnloadingController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SILSUnloadingController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        //[HttpGet("DataGrid")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        //{
        //    return await DataSourceLoader.LoadAsync(dbContext.vw_production_transaction
        //        .Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
        //                || CurrentUserContext.IsSysAdmin)
        //        .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
        //        loadOptions);
        //}

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            //return (tanggal1,tanggal2, loadOptions);
            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_sils_unloading
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.vw_sils_unloading
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.finish_loading >= dt1 && o.finish_loading <= dt2),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_production_transaction
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
            sils_unloading record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(sils_unloading),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new sils_unloading();
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
                                    DateTime currentDate = DateTime.Now;
                                    string datePrefix = currentDate.ToString("MMddyy");

                                    cmd.CommandText = $"SELECT MAX(barge_loading_number) FROM sils WHERE barge_loading_number LIKE '{datePrefix}-%'";
                                    var latestTransactionNumber = await cmd.ExecuteScalarAsync() as string;

                                    int nextR = 1;
                                    if (!string.IsNullOrEmpty(latestTransactionNumber))
                                    {
                                        //var rString = latestTransactionNumber.Split('-'); // Get the r part
                                        var parts = latestTransactionNumber.Split('-');
                                        if (parts.Length >= 3)
                                        {
                                            var rString = parts[2]; // Get the r part
                                            int.TryParse(rString, out nextR);
                                            nextR++; // Increment for the next entry
                                        }
                                        /*int.TryParse(rString, out nextR);
                                        nextR++; // Increment for the next entry*/
                                    }

                                    string r = nextR.ToString("D4"); // Format r as "001"
                                   //record.barge_loading_number = $"{datePrefix}-{r}";

                                    /* cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                     var r = await cmd.ExecuteScalarAsync();
                                     record.transaction_number = $"BP-{DateTime.Now:yyyyMMdd}-{r}";*/
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        #endregion
                        try
                        {

                        }
                        catch
                        {

                        }
                        dbContext.sils_unloading.Add(record);
                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();

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

            var success = false;
            sils_unloading record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.sils_unloading
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
                            /*var ap1 = await dbContext.accounting_period
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
                            }*/

                            // Capacity
                            //if (record.transport_id != null)
                            //{
                            //    var tr1 = await dbContext.transport
                            //        .Where(o => o.id == record.transport_id)
                            //        .FirstOrDefaultAsync();
                            //    if (tr1 != null)
                            //    {
                            //        if ((decimal)(tr1?.capacity ?? 0) < record.unloading_quantity)
                            //        {
                            //            return BadRequest("Transport capacity is less than unloading quantity");
                            //        }
                            //    }
                            //}

                            #endregion

                            #region Calculate actual progress claim

                            /*if (!string.IsNullOrEmpty(record.progress_claim_id))
                            {
                                var pc = await dbContext.progress_claim
                                    .Where(o => o.id == record.progress_claim_id)
                                    .FirstOrDefaultAsync();
                                if (pc != null)
                                {
                                    var actualQty = await dbContext.production_transaction
                                        .Where(o => o.progress_claim_id == pc.id)
                                        .SumAsync(o => o.unloading_quantity);
                                    pc.actual_quantity = actualQty;
                                }
                            }*/

                            #endregion

                            await dbContext.SaveChangesAsync();

                            #region Header Update
/*
                            if (await mcsContext.CanUpdate(dbContext, headerData.id, CurrentUserContext.AppUserId)
                                || CurrentUserContext.IsSysAdmin)
                            {
                                headerData.ritase = sumRitase;
                                headerData.loading_quantity = sumNetQuantity;
                                if (headerData.density == null)
                                    headerData.density = 1;
                                headerData.volume = headerData.density * sumNetQuantity;
                                await dbContext.SaveChangesAsync();
                            }*/

                            #endregion

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
                    var _record = new DataAccess.Repository.production_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ProductionTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ProductionTransaction.UpdateStockStateAnalyte(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok();
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Trace($"string key = {key}");

            var success = false;
            sils_unloading record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.sils_unloading
                        .Where(o => o.id == key
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.sils_unloading.Remove(record);

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
                    var _record = new DataAccess.Repository.production_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ProductionTransaction.UpdateStockState(connectionString, _record));
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ProductionTransaction.UpdateStockStateAnalyte(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok();
        }

        [HttpGet("LocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> LocationIdLookup(string Id,DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = dbContext.event_definition_category.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Id)
                    .Select(o => o.event_definition_category_name).FirstOrDefault();
                if (data == null || data.ToUpper() == "IDLE")
                {
                    var barge = dbContext.barge
                                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                   .Select(o => new { Value = o.id, Text = o.vehicle_name, index = 0 });
                    var equipment = dbContext.vw_equipment
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .Where(o => o.equipment_type_name.ToUpper() == "CONVEYOR")
                                    .Select(o => new { Value = o.id, Text = o.equipment_name, index = 0 });

                    var lookup = barge.Union(equipment)
                    .OrderBy(o => o.Text);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else if (data.ToUpper() == "OPERATION")
                {
                    var barge = dbContext.barge
                               .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                               .Select(o => new { Value = o.id, Text = o.vehicle_name, index = 0 });
                    return await DataSourceLoader.LoadAsync(barge, loadOptions);
                }
                else
                {
                    var equipment = dbContext.vw_equipment
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.equipment_type_name.ToUpper() == "CONVEYOR")
                                .Select(o => new { Value = o.id, Text = o.equipment_name, index = 0 });
                    return await DataSourceLoader.LoadAsync(equipment, loadOptions);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("Operator2IdLookup")]
        public async Task<object> Operator2IdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_operator
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.operator_name });
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
                    var lookup = dbContext.vw_mine_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.business_area_name + " > " + o.stock_location_name,
                                o.product_id
                            });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.vw_mine_location.FromSqlRaw(
                        " SELECT l.* FROM vw_mine_location l "
                        + " WHERE l.organization_id = {0} "
                        + " AND l.business_area_id IN ( "
                        + "     SELECT ba.id FROM vw_business_area_structure ba, process_flow pf "
                        + "     WHERE position(pf.source_location_id in ba.id_path) > 0"
                        + "         AND pf.id = {1} "
                        + " ) ",
                        CurrentUserContext.OrganizationId, ProcessFlowId)
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
                    var lookup = dbContext.vw_stockpile_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.business_area_name + " > " + o.stock_location_name,
                                o.product_id
                            });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.vw_stockpile_location.FromSqlRaw(
                        " SELECT l.* FROM vw_stockpile_location l "
                        + " WHERE l.organization_id = {0} "
                        + " AND l.business_area_id IN ( "
                        + "     SELECT ba.id FROM vw_business_area_structure ba, process_flow pf "
                        + "     WHERE position(pf.destination_location_id in ba.id_path) > 0"
                        + "         AND pf.id = {1} "
                        + " ) ",
                        CurrentUserContext.OrganizationId, ProcessFlowId)
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
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && (o.is_draft_survey == null || o.is_draft_survey == false))
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.survey_number });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.survey.FromSqlRaw(
                          " SELECT s.* FROM survey s "
                        + " INNER JOIN stock_location sl ON sl.id = s.stock_location_id "
                        + " AND COALESCE(s.is_draft_survey, FALSE) = FALSE "
                        + " AND s.organization_id = {0} "
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

        [HttpGet("ProcessFlowIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProcessFlowIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.process_flow
                    .Where(o => o.process_flow_category == Common.ProcessFlowCategory.COAL_MINED
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderByDescending(o => o.is_active).ThenBy(o => o.process_flow_name)
                    .Select(o =>
                    new
                    {
                        Value = o.id,
                        Text = o.process_flow_name + (o.is_active == true ? "" : "( ## Not Active )")
                    });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("EventCategoryIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> EventCategoryIdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.event_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.event_definition_category_id == Id)
                    .Select(o => new { Value = o.id, Text = o.event_category_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SourceIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SourceIdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.barge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    //.Where(o => o.event_definition_category_id == Id)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("EquiptmentIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> EquiptmentIdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.equipment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    //.Where(o => o.event_definition_category_id == Id)
                    .Select(o => new { Value = o.id, Text = o.equipment_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ShiftIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ShiftIdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.shift
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    //.Where(o => o.event_definition_category_id == Id)
                    .Select(o => new { Value = o.id, Text = o.shift_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("Type2IdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> Type2IdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = dbContext.event_definition_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.id == Id)
                    .Where(o => o.is_problem_productivity == true);
                /*if (data != null)
                {
                    return await DataSourceLoader.LoadAsync(data, loadOptions);
                }
                else
                {
                    return await DataSourceLoader.LoadAsync(data, loadOptions);
                }*/
                if (data.Any())
                {
                    var lookupA = dbContext.event_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.event_definition_category_id == Id)
                    .Select(o => new { Value = o.id, Text = o.event_category_name, Index = 1 });
                    return await DataSourceLoader.LoadAsync(lookupA, loadOptions);
                }
                else
                {
                    var lookupB = dbContext.process_flow
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = "Process Flow > " + o.process_flow_name, Index = 0 });
                    return await DataSourceLoader.LoadAsync(lookupB, loadOptions);
                }
                /*var lookup1 = dbContext.process_flow
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = "Process Flow > " + o.process_flow_name, Index = 0 });
                var lookup2 = dbContext.event_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = "Category Event > " + o.event_category_name, Index = 1 });
                var lookup = lookup1.Union(lookup2);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);*/
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("TypeIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> TypeIdLookup(string Id,DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = dbContext.event_definition_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.id == Id)
                    .Where(o => o.is_problem_productivity == true);
                /*if (data != null)
                {
                    return await DataSourceLoader.LoadAsync(data, loadOptions);
                }
                else
                {
                    return await DataSourceLoader.LoadAsync(data, loadOptions);
                }*/
                if (data.Any())
                {
                    var lookupA = dbContext.event_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.event_definition_category_id == Id)
                    .Select(o => new { Value = o.id, Text = o.event_category_name, Index = 1 });
                    return await DataSourceLoader.LoadAsync(lookupA, loadOptions);
                }
                else
                {
                    var lookupB = dbContext.process_flow
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.process_flow_name, Index = 0 });
                    return await DataSourceLoader.LoadAsync(lookupB, loadOptions);
                }
                /*var lookup1 = dbContext.process_flow
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = "Process Flow > " + o.process_flow_name, Index = 0 });
                var lookup2 = dbContext.event_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = "Category Event > " + o.event_category_name, Index = 1 });
                var lookup = lookup1.Union(lookup2);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);*/
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        

        [HttpGet("AllIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AllIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup1 = dbContext.process_flow
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = "Process Flow > " + o.process_flow_name, Index = 0 });
                var lookup2 = dbContext.event_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.event_category_name, Index = 1 });
                var lookup = lookup1.Union(lookup2);
                return await DataSourceLoader.LoadAsync(lookup2, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("CategoryIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> CategoryIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.event_definition_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                   // .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.event_definition_category_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
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

        [HttpGet("ProductDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProductDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            decimal ts = 0, ash = 0;
            try
            {
                var analyte = await dbContext.vw_product_specification
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.product_id == Id)
                    .ToListAsync();

                foreach (var d in analyte)
                {
                    decimal nilai = (decimal)d.target_value;
                    /*if (d.target_value != 0 || d.target_value != null)
                    {
                        nilai = (decimal)d.target_value;
                    }*/
                    /*if (d.maximum_value == null && d.minimum_value != null)
                    {
                        nilai = (decimal)d.minimum_value;
                    }
                    else if (d.minimum_value == null && d.maximum_value != null)
                    {
                        nilai = (decimal)d.maximum_value;
                    }
                    else
                    {
                        nilai = 0;
                    }*/
                    var symbol = d.analyte_symbol.ToUpper().Trim();
                    if (symbol == "TS (ARB)") ts = nilai;
                    else if (symbol == "ASH (ARB)") ash = nilai;
                }

                var quality = new mine_location_quality();
                quality.id = Id;
                quality.ts = ts;
                quality.ash = ash;
                //return await DataSourceLoader.ToList(quality, loadOptions);
                return quality;
            }
            catch (Exception)
            {
                Console.WriteLine($"An error occurred: gabisa coi");
                return null;
            }
        }

        [HttpGet("VoyageIdLookupB")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> VoyageIdLookupB(string Id,DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                {
                    var lookupb = dbContext.barge_rotation
                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                   .Where(o => !dbContext.sils_unloading.Any(x => x.barge_rotation_id == o.id && x.approve_status == true))
                   //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                   .Select(o => new { Value = o.id, Text = o.voyage_number });
                    return await DataSourceLoader.LoadAsync(lookupb, loadOptions);
                }
                else
                {
                    var data = await dbContext.sils_unloading
                   .Where(o => o.approve_status == true)
                   .Select(o => o.barge_rotation_id)
                   .ToListAsync();
                    var current = dbContext.sils_unloading
                        .Where(o => o.id == Id)
                        .Select(o => o.barge_rotation_id)
                        .FirstOrDefault();
                    var lookup2 = dbContext.barge_rotation
                        .Where(o => o.id == current)
                        .Select(o => new { Value = o.id, Text = o.voyage_number });
                    var lookup1 = await (
                        from rotation in dbContext.barge_rotation
                        where rotation.organization_id == CurrentUserContext.OrganizationId
                              && !data.Contains(rotation.id)
                        // Additional conditions if needed
                        // && (rotation.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        select new { Value = rotation.id, Text = rotation.voyage_number }
                    ).ToListAsync();
                    var lookup = lookup1.Union(lookup2);
                    return DataSourceLoader.Load(lookup, loadOptions);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ViewVoyageIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ViewVoyageIdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                {
                    var lookupb = dbContext.barge_rotation
                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                   .Select(o => new { Value = o.id, Text = o.voyage_number });
                    return await DataSourceLoader.LoadAsync(lookupb, loadOptions);
                }
                
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("VoyageIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> VoyageIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await dbContext.sils_unloading
                .Select(o => o.barge_rotation_id)
                .ToListAsync();

                var lookup = await (
                    from rotation in dbContext.barge_rotation
                    where rotation.organization_id == CurrentUserContext.OrganizationId
                          && !data.Contains(rotation.id)
                    // Additional conditions if needed
                    // && (rotation.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    select new { Value = rotation.id, Text = rotation.voyage_number }
                ).ToListAsync();

                return DataSourceLoader.Load(lookup, loadOptions);

                /*var data = dbContext.sils_unloading
                    .Select(o=>o.barge_rotation_id).ToListAsync();
                var lookup = dbContext.barge_rotation
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.voyage_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);*/
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
                var lookup = dbContext.product
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.product_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("OperatorIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> OperatorIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_employee
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.is_operator == true)
                    .Select(o => new { Value = o.id, Text = o.employee_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        /*
                [HttpGet("ForemanIdLookup")]
                [ApiExplorerSettings(IgnoreApi = true)]
                public async Task<object> ForemanIdLookup(DataSourceLoadOptions loadOptions)
                {
                    try
                    {
                        var lookup = dbContext.vw_employee
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                            .Select(o => new { Value = o.id, Text = o.employee_name });
                        return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.InnerException ?? ex);
                        return BadRequest(ex.InnerException?.Message ?? ex.Message);
                    }
                }*/

        /*[HttpGet("VoyageDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> VoyageDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            //return await DataSourceLoader.LoadAsync(
                var b = await dbContext.vw_barge_rotation
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .ToListAsync();
            
            var a = new barge_rotation();
            a = b.destination_location;

        }*/

        [HttpGet("EmployeeIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> EmployeeIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_employee
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.employee_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpGet("BusinessIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> BusinessIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.business_unit
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.business_unit_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("BargeIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> BargeIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup1 = dbContext.vw_barge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name, Index = 0 });
                /*var lookup2 = dbContext.vw_transport
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name, Index = 1 });
                var lookup = lookup1.Union(lookup2);*/
                return await DataSourceLoader.LoadAsync(lookup1, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("TugIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> TugIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.tug
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name });
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

        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.vw_production_transaction
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
        public async Task<IActionResult> SaveData([FromBody] sils_unloading Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.sils
                        .Where(o => o.id == Record.id
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            if (string.IsNullOrEmpty(record.barge_loading_number))
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
                                            DateTime currentDate = DateTime.Now;
                                            string datePrefix = currentDate.ToString("MMddyy");

                                            cmd.CommandText = $"SELECT MAX(barge_loading_number) FROM sils WHERE barge_loading_number LIKE '{datePrefix}-%'";
                                            var latestTransactionNumber = await cmd.ExecuteScalarAsync() as string;

                                            int nextR = 1;
                                            if (!string.IsNullOrEmpty(latestTransactionNumber))
                                            {
                                                //var rString = latestTransactionNumber.Split('-'); // Get the r part
                                                var parts = latestTransactionNumber.Split('-');
                                                if (parts.Length >= 3)
                                                {
                                                    var rString = parts[2]; // Get the r part
                                                    int.TryParse(rString, out nextR);
                                                    nextR++; // Increment for the next entry
                                                }
                                                /*int.TryParse(rString, out nextR);
                                                nextR++; // Increment for the next entry*/
                                            }

                                            string r = nextR.ToString("D4"); // Format r as "001"
                                            record.barge_loading_number = $"{datePrefix}-{r}";

                                            /* cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                             var r = await cmd.ExecuteScalarAsync();
                                             record.transaction_number = $"BP-{DateTime.Now:yyyyMMdd}-{r}";*/
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
                            /*if (record.source_location_id == record.destination_location_id)
                            {
                                return BadRequest("Source location must be different from destination location");
                            }*/

                            #endregion

                            #region Update stockpile state

                           /* var qtyOut = await dbContext.stockpile_state
                                .Where(o => o.stockpile_location_id == record.source_location_id
                                    && o.transaction_id == record.id)
                                .FirstOrDefaultAsync();
                            if (qtyOut != null)
                            {
                                qtyOut.modified_by = CurrentUserContext.AppUserId;
                                qtyOut.modified_on = DateTime.Now;
                                qtyOut.qty_out = record.loading_quantity ?? record.unloading_quantity;
                                qtyOut.transaction_datetime = record.unloading_datetime;
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
                                    qty_out = record.loading_quantity ?? record.unloading_quantity,
                                    transaction_datetime = record.loading_datetime ?? record.unloading_datetime
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
                                qtyIn.transaction_datetime = record.loading_datetime ?? record.unloading_datetime;
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
                                    transaction_datetime = record.unloading_datetime
                                };

                                dbContext.stockpile_state.Add(qtyIn);
                            }*/

                            #endregion

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();

                           /* Task.Run(() =>
                            {
                                var ss = new BusinessLogic.Entity.StockpileState(CurrentUserContext);
                                ss.Update(record.source_location_id, record.id);
                                ss.Update(record.destination_location_id, record.id);
                            }).Forget();
*/
                            return Ok(record);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else if (await mcsContext.CanCreate(dbContext, nameof(sils_unloading),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        #region Add record

                        record = new sils();
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

                        if (string.IsNullOrEmpty(record.barge_loading_number))
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
                                        DateTime currentDate = DateTime.Now;
                                        string datePrefix = currentDate.ToString("MMddyy");

                                        cmd.CommandText = $"SELECT MAX(barge_loading_number) FROM sils WHERE barge_loading_number LIKE '{datePrefix}-%'";
                                        var latestTransactionNumber = await cmd.ExecuteScalarAsync() as string;

                                        int nextR = 1;
                                        if (!string.IsNullOrEmpty(latestTransactionNumber))
                                        {
                                            //var rString = latestTransactionNumber.Split('-'); // Get the r part
                                            var parts = latestTransactionNumber.Split('-');
                                            if (parts.Length >= 3)
                                            {
                                                var rString = parts[2]; // Get the r part
                                                int.TryParse(rString, out nextR);
                                                nextR++; // Increment for the next entry
                                            }
                                            /*int.TryParse(rString, out nextR);
                                            nextR++; // Increment for the next entry*/
                                        }

                                        string r = nextR.ToString("D4"); // Format r as "001"
                                        record.barge_loading_number = $"{datePrefix}-{r}";

                                        /* cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                         var r = await cmd.ExecuteScalarAsync();
                                         record.transaction_number = $"BP-{DateTime.Now:yyyyMMdd}-{r}";*/
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

                        dbContext.sils.Add(record);

                        #region Validation

                        // Source location != destination location
                        /*if (record.source_location_id == record.destination_location_id)
                        {
                            return BadRequest("Source location must be different from destination location");
                        }*/

                        #endregion

                        #region Add to stockpile state

                        /*var qtyOut = await dbContext.stockpile_state
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
                                qty_out = record.loading_quantity ?? record.unloading_quantity,
                                transaction_datetime = record.loading_datetime ?? record.unloading_datetime
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
                                transaction_datetime = record.unloading_datetime
                            };

                            dbContext.stockpile_state.Add(qtyOut);
                        }*/

                        #endregion

                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();

                        /*Task.Run(() =>
                        {
                            var ss = new BusinessLogic.Entity.StockpileState(CurrentUserContext);
                            ss.Update(record.source_location_id, record.id);
                            ss.Update(record.destination_location_id, record.id);
                        }).Forget();*/

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
                    var record = dbContext.production_transaction
                        .Where(o => o.id == Id
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        #region Empty stockpile state

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

                        dbContext.production_transaction.Remove(record);

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
                            var record = dbContext.production_transaction
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.production_transaction.Remove(record);
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

        [HttpGet("GetSILS/{Id}")]
        public object GetSILS(string Id)
        {
            try
            {
                var result = new sils_unloading();

                result = dbContext.sils_unloading.Where(x => x.id == Id).FirstOrDefault();

                if (result == null)
                {
                    result = new sils_unloading();
                    result.id = Id;
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("ApproveUnapprove")]    //**** tanpa trigger trf_sales_invoice_approval_sync di table sales_invoice_approval
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ApproveUnapprove([FromForm] string key, [FromForm] string values)
        {
            dynamic result;
            bool? isApproved = null;

           
            var recSILS = dbContext.sils_unloading
                            .Where(o => o.id == key)
                            .FirstOrDefault();
           
           
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanUpdate(dbContext, recSILS.id, CurrentUserContext.AppUserId)
                           || CurrentUserContext.IsSysAdmin)
                    {

                        var e = new entity();
                        e.InjectFrom(recSILS);

                       // JsonConvert.PopulateObject(values, recSILS);
                       // recSILS.InjectFrom(e);

                        recSILS.modified_by = CurrentUserContext.AppUserId;
                        recSILS.modified_on = System.DateTime.Now;
                        if (recSILS.approve_status == true)
                        {
                            recSILS.approve_status = false;
                            recSILS.disapprove_by_id = CurrentUserContext.AppUserId;
                            isApproved = false;
                            result = recSILS;
                        }
                        else
                        {
                            recSILS.approve_status = true;
                            recSILS.approve_by_id = CurrentUserContext.AppUserId;
                            isApproved = true;
                            result = recSILS;
                        }
                        await dbContext.SaveChangesAsync();
                        result = recSILS;

                        await tx.CommitAsync();

                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.InnerException ?? ex);
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            result = recSILS;
            return Ok(result);

        }
        /*
                [HttpPost("ApproveUnapprove")]    //**** tanpa trigger trf_sales_invoice_approval_sync di table sales_invoice_approval
                [ApiExplorerSettings(IgnoreApi = true)]
                public async Task<IActionResult> ApproveUnapprove([FromForm] string key, [FromForm] string values)
                {
                    dynamic result;
                    bool? isApproved = null;

                    var recBargingTransaction = dbContext.barging_transaction
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    //if (recBargingTransaction == null) return BadRequest("Data not found!.");

                    using (var tx = await dbContext.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            var record = dbContext.barging_transaction
                                .Where(o => o.id == key)
                                .FirstOrDefault();
                            if (record == null)
                            {
                                logger.Debug($"ApproveUnapprove; sils; new record; id = {key}");

                                var newRec = new sales_invoice_approval();
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

                                newRec.approve_status = "APPROVED";
                                newRec.approve_by_id = CurrentUserContext.AppUserId;

                                dbContext.sales_invoice_approval.Add(newRec);
                                await dbContext.SaveChangesAsync();

                                result = newRec;
                                isApproved = true;
                            }
                            else
                            {
                                #region *** Three minutes validation ***
                                if (record.modified_on != null)
                                {
                                    if (Convert.ToDateTime(record.modified_on) > System.DateTime.Now.AddMinutes(-3))
                                    {
                                        logger.Debug($"ApproveUnapprove; sales_invoice_approval; Three minutes validation-modified_on; sales_invoice_id = {key}");

                                        return BadRequest("Please wait for about 3 minutes after last edit.");
                                    }
                                }
                                else if (Convert.ToDateTime(record.created_on) > System.DateTime.Now.AddMinutes(-3))
                                {
                                    logger.Debug($"ApproveUnapprove; sales_invoice_approval; Three minutes validation-created_on; sales_invoice_id = {key}");

                                    return BadRequest("Please wait for about 3 minutes after last edit.");
                                }
                                #endregion

                                var e = new entity();
                                e.InjectFrom(record);

                                JsonConvert.PopulateObject(values, record);
                                record.InjectFrom(e);

                                record.modified_by = CurrentUserContext.AppUserId;
                                record.modified_on = System.DateTime.Now;

                                if (record.approve_status == "APPROVED")
                                {
                                    record.approve_status = "UNAPPROVED";
                                    record.disapprove_by_id = CurrentUserContext.AppUserId;
                                    isApproved = false;
                                }
                                else
                                {
                                    record.approve_status = "APPROVED";
                                    record.approve_by_id = CurrentUserContext.AppUserId;
                                    isApproved = true;
                                }

                                logger.Debug($"ApproveUnapprove; sales_invoice_approval; sales_invoice_id = {key}; approve_status = {record.approve_status};" +
                                    " isApproved = {isApproved}");

                                await dbContext.SaveChangesAsync();
                                result = record;
                            }

                            string responseCode = "";
                            string responseText = "";

                            if (isApproved == false)    //*** Cancel invoice
                            {
                                //var recSIE = dbContext.sales_invoice_ell
                                //   .Where(o => o.id == key && o.sync_type == "UPDATE" && o.sync_status == "FAILED");
                                //if (recSIE.Count() > 0)
                                //    dbContext.sales_invoice_ell.RemoveRange(recSIE);

                                var SIERespCode = dbContext.sales_invoice_ell
                                   .Where(o => o.id == key && o.sync_status == "SUCCESS" && o.response_code != null)
                                   .OrderByDescending(o => o.created_on).FirstOrDefault();
                                if (SIERespCode != null)
                                {
                                    responseCode = SIERespCode.response_code;
                                    responseText = SIERespCode.response_text;
                                }
                                else
                                {
                                    logger.Debug($"ApproveUnapprove; sales_invoice_ell; sales_invoice_id = {key}; isApproved = {isApproved}; Cancel invoice-CommitAsync");

                                    await tx.CommitAsync();
                                    return Ok(result);
                                }

                                var recEll = new sales_invoice_ell()
                                {
                                    id = recSalesInvoice.id,
                                    created_by = CurrentUserContext.AppUserId,
                                    created_on = System.DateTime.Now,
                                    modified_by = null,
                                    modified_on = null,
                                    is_active = recSalesInvoice.is_active,
                                    is_locked = recSalesInvoice.is_locked,
                                    is_default = recSalesInvoice.is_default,
                                    owner_id = recSalesInvoice.owner_id,
                                    organization_id = recSalesInvoice.organization_id,
                                    entity_id = recSalesInvoice.entity_id,

                                    despatch_order_id = recSalesInvoice.despatch_order_id,
                                    quantity = recSalesInvoice.quantity,
                                    uom_id = recSalesInvoice.uom_id,
                                    unit_price = recSalesInvoice.unit_price,
                                    currency_id = recSalesInvoice.currency_id,
                                    invoice_date = recSalesInvoice.invoice_date,
                                    accounting_period_id = recSalesInvoice.accounting_period_id,
                                    invoice_number = recSalesInvoice.invoice_number,
                                    sales_type_id = recSalesInvoice.sales_type_id,
                                    invoice_type_id = recSalesInvoice.invoice_type_id,
                                    customer_id = recSalesInvoice.customer_id,
                                    seller_id = recSalesInvoice.seller_id,
                                    bill_to = recSalesInvoice.bill_to,
                                    contract_product_id = recSalesInvoice.contract_product_id,s
                                    notes = recSalesInvoice.notes,
                                    bank_account_id = recSalesInvoice.bank_account_id,
                                    downpayment = recSalesInvoice.downpayment,
                                    total_price = recSalesInvoice.total_price,
                                    quotation_type_id = recSalesInvoice.quotation_type_id,
                                    currency_exchange_id = recSalesInvoice.currency_exchange_id,
                                    lc_status = recSalesInvoice.lc_status,
                                    lc_date_issue = recSalesInvoice.lc_date_issue,
                                    lc_issuing_bank = recSalesInvoice.lc_issuing_bank,
                                    freight_cost = recSalesInvoice.freight_cost,
                                    correspondent_bank_id = recSalesInvoice.correspondent_bank_id,

                                    sync_id = Guid.NewGuid().ToString("N"),
                                    sync_type = "UPDATE",
                                    sync_status = null,
                                    error_msg = null,
                                    response_code = responseCode,
                                    response_text = responseText,
                                    canceled = true
                                };

                                dbContext.sales_invoice_ell.Add(recEll);
                                await dbContext.SaveChangesAsync();

                                logger.Debug($"ApproveUnapprove; sales_invoice_ell; sales_invoice_id = {key}; isApproved = {isApproved}; Cancel invoice-New Record");
                            }
                            else //***** New invoice
                            {
                                var recSIE = dbContext.sales_invoice_ell
                                    .Where(o => o.id == key).OrderByDescending(o => o.created_on)
                                    .FirstOrDefault();
                                if (recSIE != null)
                                {
                                    await tx.CommitAsync();
                                    return Ok(result);
                                }

                                var recEll = new sales_invoice_ell()
                                {
                                    id = recSalesInvoice.id,
                                    created_by = CurrentUserContext.AppUserId,
                                    created_on = System.DateTime.Now,
                                    modified_by = null,
                                    modified_on = null,
                                    is_active = recSalesInvoice.is_active,
                                    is_locked = recSalesInvoice.is_locked,
                                    is_default = recSalesInvoice.is_default,
                                    owner_id = recSalesInvoice.owner_id,
                                    organization_id = recSalesInvoice.organization_id,
                                    entity_id = recSalesInvoice.entity_id,

                                    despatch_order_id = recSalesInvoice.despatch_order_id,
                                    quantity = recSalesInvoice.quantity,
                                    uom_id = recSalesInvoice.uom_id,
                                    unit_price = recSalesInvoice.unit_price,
                                    currency_id = recSalesInvoice.currency_id,
                                    invoice_date = recSalesInvoice.invoice_date,
                                    accounting_period_id = recSalesInvoice.accounting_period_id,
                                    invoice_number = recSalesInvoice.invoice_number,
                                    sales_type_id = recSalesInvoice.sales_type_id,
                                    invoice_type_id = recSalesInvoice.invoice_type_id,
                                    customer_id = recSalesInvoice.customer_id,
                                    seller_id = recSalesInvoice.seller_id,
                                    bill_to = recSalesInvoice.bill_to,
                                    contract_product_id = recSalesInvoice.contract_product_id,
                                    notes = recSalesInvoice.notes,
                                    bank_account_id = recSalesInvoice.bank_account_id,
                                    downpayment = recSalesInvoice.downpayment,
                                    total_price = recSalesInvoice.total_price,
                                    quotation_type_id = recSalesInvoice.quotation_type_id,
                                    currency_exchange_id = recSalesInvoice.currency_exchange_id,
                                    lc_status = recSalesInvoice.lc_status,
                                    lc_date_issue = recSalesInvoice.lc_date_issue,
                                    lc_issuing_bank = recSalesInvoice.lc_issuing_bank,
                                    freight_cost = recSalesInvoice.freight_cost,
                                    correspondent_bank_id = recSalesInvoice.correspondent_bank_id,

                                    sync_id = Guid.NewGuid().ToString("N"),
                                    sync_type = "INSERT",
                                    sync_status = null,
                                    error_msg = null,
                                    response_code = null,
                                    response_text = null,
                                    canceled = null
                                };

                                dbContext.sales_invoice_ell.Add(recEll);
                                await dbContext.SaveChangesAsync();

                                logger.Debug($"ApproveUnapprove; sales_invoice_ell; sales_invoice_id = {key}; isApproved = {isApproved}; New Sales Invoice");
                            }

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
                }*/

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
                    var kunci = new Dictionary<string, object>(){
                        {"sampling_type_name", "Hauling Sampling"}
                    };
                    result = await svc.Select2(s2Request, "sampling_number", null, kunci);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return new JsonResult(result);
        }

        [HttpPost("UploadDocument")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UploadDocument([FromBody] dynamic FileDocument)
        {
            var result = new StandardResult();
            var records = new List<production_transaction>();
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
                    if (row.Cells.Count() < 10) continue;

                    var barge = "";
                    var b = await dbContext.barge.Where(o => o.vehicle_id.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()).FirstOrDefaultAsync();
                    if (b != null) { barge = b.id.ToString(); }

                    var voyage_number = "";
                    var vn = await dbContext.barge_rotation.Where(o => o.voyage_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower()).FirstOrDefaultAsync();
                    if (vn != null)
                    {
                        voyage_number = vn.id.ToString();
                    }
                    else
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Can't Find this Voyage Number in Barging Rotation" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        break;
                    }

                    var simultan_with = "";
                    var sw = await dbContext.barge.Where(o => o.vehicle_id.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefaultAsync();
                    if (sw != null) simultan_with = sw.id.ToString();

                    var destination = "";
                    var d = await dbContext.business_unit.Where(o => o.business_unit_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower()).FirstOrDefaultAsync();
                    if (d != null) destination = d.id.ToString();

                    var business_unit = "";
                    var bu = await dbContext.business_unit.Where(o => o.business_unit_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(11)).ToLower()).FirstOrDefaultAsync();
                    if (bu != null) business_unit = bu.id.ToString();

                    var product = "";
                    var p = await dbContext.product.Where(o => o.product_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(12)).ToLower()).FirstOrDefaultAsync();
                    if (p != null) product = p.id.ToString();

                    var record = dbContext.sils_unloading
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.barge_rotation_id.Trim() == voyage_number.Trim())
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        //record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.barge_rotation_id = voyage_number;
                        record.business_unit_id = business_unit;
                        record.simultan_with = simultan_with;
                        record.mine_of_origin = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.business_unit_id = business_unit;
                        record.product_id = product;
                        record.barge_destination = destination;
                        record.barge_id = barge;
                        record.date_arrived = PublicFunctions.Tanggal(row.GetCell(5));
                        record.date_berthed = PublicFunctions.Tanggal(row.GetCell(6));
                        record.start_loading = PublicFunctions.Tanggal(row.GetCell(7));
                        record.finish_loading = PublicFunctions.Tanggal(row.GetCell(8));
                        record.unberthed_time = PublicFunctions.Tanggal(row.GetCell(9));
                        record.departed_time = PublicFunctions.Tanggal(row.GetCell(10));
                        record.analyte_1 = PublicFunctions.Desimal(row.GetCell(13));
                        record.analyte_2 = PublicFunctions.Desimal(row.GetCell(14));
                        record.analyte_3 = PublicFunctions.Desimal(row.GetCell(15));
                        record.site_scale = PublicFunctions.Desimal(row.GetCell(16));
                        record.draft_scale = PublicFunctions.Desimal(row.GetCell(17));
                        record.belt_scale = PublicFunctions.Desimal(row.GetCell(18));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new sils_unloading();
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
                        //    record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        record.barge_rotation_id = voyage_number;
                        record.business_unit_id = business_unit;
                        record.simultan_with = simultan_with;
                        record.mine_of_origin = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.business_unit_id = business_unit;
                        record.product_id = product;
                        record.barge_destination = destination;
                        record.barge_id = barge;
                        record.date_arrived = PublicFunctions.Tanggal(row.GetCell(5));
                        record.date_berthed = PublicFunctions.Tanggal(row.GetCell(6));
                        record.start_loading = PublicFunctions.Tanggal(row.GetCell(7));
                        record.finish_loading = PublicFunctions.Tanggal(row.GetCell(8));
                        record.unberthed_time = PublicFunctions.Tanggal(row.GetCell(9));
                        record.departed_time = PublicFunctions.Tanggal(row.GetCell(10));
                        record.analyte_1 = PublicFunctions.Desimal(row.GetCell(13));
                        record.analyte_2 = PublicFunctions.Desimal(row.GetCell(14));
                        record.analyte_3 = PublicFunctions.Desimal(row.GetCell(15));
                        record.site_scale = PublicFunctions.Desimal(row.GetCell(16));
                        record.draft_scale = PublicFunctions.Desimal(row.GetCell(17));
                        record.belt_scale = PublicFunctions.Desimal(row.GetCell(18));

                        dbContext.sils_unloading.Add(record);
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
            wb.Close();
            if (gagal)
            {
                await transaction.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "Production");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        #region Item Data Section

        [HttpGet("GetItemsById")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetItemsById(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_sils_unloading_detail
                .Where(o => o.sils_unload_id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpGet("subDetailById")] //subdetail data grid
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> subDetailById(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.sils_unloading_subdetail
                .Where(o => o.sils_unload_detail_id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId)
                .OrderBy(o => o.start_datetime),
                loadOptions);
        }

        [HttpPost("InsertItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertItemData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            sils_unloading_detail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(sils_unloading_detail),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new sils_unloading_detail();
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
                        //record.ritase = 0;

                        /*if (record.start_flow_time != null && record.stop_flow_time != null)
                        {
                            if (record.start_flow_time <= record.stop_flow_time)
                            {
                                *//*var start = record.start_flow_time;
                                var stop = record.stop_flow_time;
                                TimeSpan total = ((stop - start) * 24 ) * 60;
                                record.total = total;*//*
                                DateTime start = Convert.ToDateTime(record.start_flow_time);
                                DateTime stop = Convert.ToDateTime(record.stop_flow_time);
                                TimeSpan difference = (TimeSpan)(stop - start);
                                double totalHours = difference.TotalHours;
                                record.total = (decimal)totalHours;


                            }
                            else
                            {
                                return BadRequest("Start Flow Time must be newer than Stop Flow Time.");
                            }
                        }
                        if (record.down_time_from != null && record.down_time_to != null)
                        {
                            if (record.down_time_from <= record.down_time_to)
                            {
                                *//*var start = record.start_flow_time;
                                var stop = record.stop_flow_time;
                                TimeSpan total = ((stop - start) * 24 ) * 60;
                                record.total = total;*//*
                                DateTime start = Convert.ToDateTime(record.down_time_from);
                                DateTime stop = Convert.ToDateTime(record.down_time_to);
                                TimeSpan difference = (TimeSpan)(stop - start);
                                double totalHours = difference.TotalHours;
                                record.total_down_time = (decimal)totalHours;


                            }
                            else
                            {
                                return BadRequest("Down Time From must be newer than Down Town To.");
                            }
                        }*/


                        dbContext.sils_unloading_detail.Add(record);

                        /*var headerData = dbContext.production_transaction
                            .Where(x => x.id == record.sils_id).FirstOrDefault();
                        *//*var itemData = dbContext.production_transaction_item
                            .Where(x => x.production_transaction_id == record.production_transaction_id).ToList();*//*

                        decimal? sumRitase = 0;
                        decimal? sumNetQuantity = 0;

                        

                        #region Header Update

                        if (await mcsContext.CanUpdate(dbContext, headerData.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            headerData.ritase = sumRitase;
                            headerData.loading_quantity = sumNetQuantity;
                            if (headerData.density == null)
                                headerData.density = 1;
                            headerData.volume = headerData.density * sumNetQuantity;
                            await dbContext.SaveChangesAsync();
                        }*/

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
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            return Ok(record);
        }

        [HttpPost("InsertSubDetailData")] //insert sub detail data
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertSubDetailData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            sils_unloading_subdetail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(sils_unloading_subdetail),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new sils_unloading_subdetail();
                        JsonConvert.PopulateObject(values, record);

                        record.id = Guid.NewGuid().ToString("N");
                        record.created_by = CurrentUserContext.AppUserId;
                        record.operator_id = CurrentUserContext.AppUserId;
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

                        if (record.start_datetime != null && record.stop_datetime != null)
                        {
                            if (record.start_datetime <= record.stop_datetime)
                            {
                                DateTime start = Convert.ToDateTime(record.start_datetime);
                                DateTime stop = Convert.ToDateTime(record.stop_datetime);
                                TimeSpan difference = (TimeSpan)(stop - start);
                                double totalHours = difference.TotalMinutes;
                                record.total = (decimal)totalHours;


                            }
                            else
                            {
                                return BadRequest("Start Flow Time must be newer than Stop Flow Time.");
                            }
                        }
                        dbContext.sils_unloading_subdetail.Add(record);
                        await dbContext.SaveChangesAsync();

                        /*var subdetail = dbContext.sils_unloading_subdetail.Where(o => o.sils_unload_detail_id == record.sils_unload_detail_id).ToList();

                        foreach*/

                        await tx.CommitAsync();
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

            sils_unloading_detail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.sils_unloading_detail
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

                            

                           /* var headerData = dbContext.sils_unloading
                            .Where(x => x.id == record.sils_id).FirstOrDefault();
                            var itemData = dbContext.sils_detail
                                .Where(x => x.sils_id == record.sils_id).ToList();
                            if (record.start_flow_time != null && record.stop_flow_time != null)
                            {
                                if (record.start_flow_time <= record.stop_flow_time)
                                {
                                    *//*var start = record.start_flow_time;
                                    var stop = record.stop_flow_time;
                                    TimeSpan total = ((stop - start) * 24 ) * 60;
                                    record.total = total;*//*
                                    DateTime start = Convert.ToDateTime(record.start_flow_time);
                                    DateTime stop = Convert.ToDateTime(record.stop_flow_time);
                                    TimeSpan difference = (TimeSpan)(stop - start);
                                    double totalHours = difference.TotalHours;
                                    record.total = (decimal)totalHours;


                                }
                                else
                                {
                                    return BadRequest("Start Flow Time must be newer than Stop Flow Time.");
                                }
                            }
                            if (record.down_time_from != null && record.down_time_to != null)
                            {
                                if (record.down_time_from <= record.down_time_to)
                                {
                                    *//*var start = record.start_flow_time;
                                    var stop = record.stop_flow_time;
                                    TimeSpan total = ((stop - start) * 24 ) * 60;
                                    record.total = total;*//*
                                    DateTime start = Convert.ToDateTime(record.down_time_from);
                                    DateTime stop = Convert.ToDateTime(record.down_time_to);
                                    TimeSpan difference = (TimeSpan)(stop - start);
                                    double totalHours = difference.TotalHours;
                                    record.total_down_time = (decimal)totalHours;


                                }
                                else
                                {
                                    return BadRequest("Down Time From must be newer than Down Town To.");
                                }
                            }*/

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok(record);
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

        [HttpPut("UpdateSubDetailData")] //update sub detail data
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateSubDetailData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            sils_unloading_subdetail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.sils_unloading_subdetail
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

                            if (record.start_datetime != null && record.stop_datetime != null)
                            {
                                if (record.start_datetime <= record.stop_datetime)
                                {
                                    DateTime start = Convert.ToDateTime(record.start_datetime);
                                    DateTime stop = Convert.ToDateTime(record.stop_datetime);
                                    TimeSpan difference = (TimeSpan)(stop - start);
                                    double totalHours = difference.TotalMinutes;
                                    record.total = (decimal)totalHours;


                                }
                                else
                                {
                                    return BadRequest("Start Flow Time must be newer than Stop Flow Time.");
                                }
                            }

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok(record);
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

        [HttpDelete("DeleteItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteItemData([FromForm] string key)
        {
            logger.Trace($"string key = {key}");

            sils_unloading_detail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.sils_unloading_detail
                        .Where(o => o.id == key
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.sils_unloading_detail.Remove(record);

                            await dbContext.SaveChangesAsync();

                            var headerData = dbContext.sils_unloading
                            .Where(x => x.id == record.sils_unload_id).FirstOrDefault();
                            var itemData = dbContext.sils_detail
                                .Where(x => x.sils_id == record.sils_unload_id).ToList();


                            #region Header Update

                            /*if (await mcsContext.CanUpdate(dbContext, headerData.id, CurrentUserContext.AppUserId)
                                || CurrentUserContext.IsSysAdmin)
                            {
                                headerData.ritase = sumRitase;
                                headerData.loading_quantity = sumNetQuantity;
                                if (headerData.density == null)
                                    headerData.density = 1;
                                headerData.volume = headerData.density * sumNetQuantity;
                                await dbContext.SaveChangesAsync();
                            }*/

                            #endregion

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
            return Ok();
        }

        [HttpDelete("DeleteSubDetailData")] //delete sub detail data
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteSubDetailData([FromForm] string key)
        {
            logger.Trace($"string key = {key}");

            sils_unloading_subdetail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.sils_unloading_subdetail
                        .Where(o => o.id == key
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.sils_unloading_subdetail.Remove(record);

                            await dbContext.SaveChangesAsync();

                            var headerData = dbContext.sils_unloading
                            .Where(x => x.id == record.sils_unload_detail_id).FirstOrDefault();
                            var itemData = dbContext.sils_detail
                                .Where(x => x.sils_id == record.sils_unload_detail_id).ToList();


                            #region Header Update

                            /*if (await mcsContext.CanUpdate(dbContext, headerData.id, CurrentUserContext.AppUserId)
                                || CurrentUserContext.IsSysAdmin)
                            {
                                headerData.ritase = sumRitase;
                                headerData.loading_quantity = sumNetQuantity;
                                if (headerData.density == null)
                                    headerData.density = 1;
                                headerData.volume = headerData.density * sumNetQuantity;
                                await dbContext.SaveChangesAsync();
                            }*/

                            #endregion

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
            return Ok();
        }

        [HttpGet("GetSubDetail/{Id}")] //mapping subdetail to detail
        public async Task<IActionResult> GetSubDetail(string Id)
        {
            sils_unloading_detail record;
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = await dbContext.sils_unloading_detail
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.id == Id)
                        .FirstOrDefaultAsync();
                    var response = await dbContext.sils_unloading_subdetail
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.sils_unload_detail_id == Id)
                        .Where(o => o.category_id == "bb0df3ede2784a11b5936c057664eda8")
                        .FirstOrDefaultAsync();
                    var response2 = dbContext.sils_unloading_subdetail
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.sils_unload_detail_id == Id)
                        .Where(o => o.category_id == "bb0df3ede2784a11b5936c057664eda8")
                        .Select(o => o);
                    if (record != null && response != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;
                            record.stockpile_1 = response.stockpile_1;
                            record.stockpile_2 = response.stockpile_2;
                            record.stockpile_3 = response.stockpile_3;
                            record.total_flow_time = response2.Sum(o => o.total * 60);
                        }
                    }
                    return Ok(record);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.InnerException ?? ex);
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
        //#endregion

    }
}

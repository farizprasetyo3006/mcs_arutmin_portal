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
using Common;
using Microsoft.AspNetCore.Hosting;
using BusinessLogic;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.InkML;
using System.Threading;

namespace MCSWebApp.Controllers.API.Port
{
    [Route("api/Port/[controller]")]
    [ApiController]
    public class BargingController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly mcsContext dbContext;

        public BargingController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }
        /*
                [HttpGet("Loading/DataGrid")]
                public async Task<object> DataGridLoading(DataSourceLoadOptions loadOptions)
                {
                    return await DataSourceLoader.LoadAsync(dbContext.vw_barging_transaction
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.is_loading == true),
                        loadOptions);
                }*/

        [HttpGet("Loading/DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGridLoading(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            logger.Debug($"tanggal1 = {tanggal1}");
            logger.Debug($"tanggal2 = {tanggal2}");

            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_barging_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.is_loading == true
                            //&& CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                            //    || CurrentUserContext.IsSysAdmin
                            )
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.vw_barging_transaction
                .Where(o =>
                    o.end_datetime >= dt1
                    && o.end_datetime <= dt2
                    && o.organization_id == CurrentUserContext.OrganizationId
                    && o.is_loading == true
                        //&& (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        //    || CurrentUserContext.IsSysAdmin)
                        )
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                loadOptions);
        }

        /*  [HttpGet("Unloading/DataGrid")]
          public async Task<object> DataGridUnloading(DataSourceLoadOptions loadOptions)
          {
              return await DataSourceLoader.LoadAsync(dbContext.vw_barging_transaction
                  .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                      && o.is_loading == false),
                  loadOptions);
          }*/

        [HttpGet("Unloading/DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGridUnloading(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            logger.Debug($"tanggal1 = {tanggal1}");
            logger.Debug($"tanggal2 = {tanggal2}");

            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_barging_transaction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.is_loading == false
                            //&& CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                            //    || CurrentUserContext.IsSysAdmin
                            )
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.vw_barging_transaction
                .Where(o =>
                    o.start_datetime >= dt1
                    && o.start_datetime <= dt2
                    && o.organization_id == CurrentUserContext.OrganizationId
                    && o.is_loading == false
                        //&& (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        //    || CurrentUserContext.IsSysAdmin)
                        )
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                loadOptions);
        }

        [HttpGet("DataDetail")]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {

            var sils = await dbContext.sils.Where(o=>o.barge_rotation_id  == Id).FirstOrDefaultAsync();
            if (sils != null && sils.approve_status == "APPROVED")
            {
                // Return sils with an additional flag indicating the source
                return new { Record = sils, Source = "SILS" };
            }
            else
            {
                var bargeRotation = await dbContext.vw_barge_rotation.FirstOrDefaultAsync(o => o.id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin));

                // Return bargeRotation with an additional flag indicating the source
                return new { Record = bargeRotation, Source = "BargeRotation" };
            }
        }

        [HttpPost("Loading/DeleteSelectedRows")]
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
                            var record = dbContext.barging_transaction
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.barging_transaction.Remove(record);
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

        [HttpPost("Loading/InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertDataLoading([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            var record = new barging_transaction();

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(barging_transaction),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        #region Add record

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
                        record.is_loading = true;

                        #endregion

                        #region Validation

                        if (record.quantity <= 0)
                        {
                            return BadRequest("Draft Survey Quantity must be more than zero.");
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
                                if ((decimal)(tr1?.capacity ?? 0) < record.quantity)
                                {
                                    return BadRequest("Transport capacity is less than loading quantity");
                                }
                            }
                        }

                        if (record.berth_datetime <= record.arrival_datetime)
                            return BadRequest("Alongside DateTime must be newer than Arrival DateTime.");
                        if (record.start_datetime <= record.berth_datetime)
                            return BadRequest("Commenced Loading DateTime must be newer than Alongside DateTime.");
                        if (record.end_datetime <= record.start_datetime)
                            return BadRequest("Completed Loading DateTime must be newer than Commenced Loading DateTime.");
                        if (record.unberth_datetime <= record.end_datetime)
                            return BadRequest("Cast Off DateTime must be newer than Completed Loading DateTime.");
                        if (record.departure_datetime <= record.unberth_datetime)
                            return BadRequest("Departure DateTime must be newer than Cast Off DateTime.");

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
                                    record.transaction_number = $"BGL-{record.end_datetime:yyyyMMdd}-{r}";

                                    cmd.CommandText = $" UPDATE barge SET barge_status = {(int)Common.Constants.BargeStatus.Cargo_On_Water} "
                                        + $" WHERE id = '{record.destination_location_id}' ";
                                    await cmd.ExecuteScalarAsync();
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        #endregion

                        dbContext.barging_transaction.Add(record);

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
                    var _record = new DataAccess.Repository.barging_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.BargingTransaction.UpdateStockState(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok(record);
        }

        [HttpPost("Unloading/InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertDataUnloading([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            var record = new barging_transaction();

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(barging_transaction),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        #region Add record

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
                        record.is_loading = false;

                        #endregion

                        #region Validation

                        var cekdata = dbContext.barging_transaction
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.despatch_order_id == record.despatch_order_id
                                && o.is_loading == false)
                            .FirstOrDefault();
                        if (cekdata != null)
                        {
                            await tx.RollbackAsync();
                            return BadRequest("The Shipping Order Number already been used on another transaction.");
                        }

                        if (record.quantity <= 0)
                        {
                            return BadRequest("Draft Survey Quantity must be more than zero.");
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
                                if ((decimal)(tr1?.capacity ?? 0) < record.quantity)
                                {
                                    return BadRequest("Transport capacity is less than unloading quantity");
                                }
                            }
                        }

                        if (record.berth_datetime <= record.arrival_datetime)
                            return BadRequest("Alongside DateTime must be later than Arrival DateTime.");
                        if (record.start_datetime <= record.berth_datetime)
                            return BadRequest("Commenced Loading DateTime must be later than Alongside DateTime.");
                        if (record.end_datetime <= record.start_datetime)
                            return BadRequest("Completed Loading DateTime must be later than Commenced Loading DateTime.");
                        if (record.unberth_datetime <= record.end_datetime)
                            return BadRequest("Cast Off DateTime must be later than Completed Loading DateTime.");
                        if (record.departure_datetime <= record.unberth_datetime)
                            return BadRequest("Departure DateTime must be later than Cast Off DateTime.");

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
                                    record.transaction_number = $"BGU-{record.end_datetime:yyyyMMdd}-{r}";

                                    cmd.CommandText = $" UPDATE barge SET barge_status = {(int)Common.Constants.BargeStatus.Cargo_On_Water} "
                                        + $" WHERE id = '{record.destination_location_id}' ";
                                    await cmd.ExecuteScalarAsync();
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        #endregion

                        dbContext.barging_transaction.Add(record);

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
                    var _record = new DataAccess.Repository.barging_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.BargingTransaction.UpdateStockState(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok(record);
        }

        [HttpPut("Loading/UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateDataLoading([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            barging_transaction record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.barging_transaction
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

                            //record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;
                            record.is_loading = true;

                            #region Validation

                            if (record.quantity <= 0)
                            {
                                return BadRequest("Draft Survey Quantity must be more than zero.");
                            }

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
                                    if ((decimal)(tr1?.capacity ?? 0) < record.quantity)
                                    {
                                        return BadRequest("Transport capacity is less than loading quantity");
                                    }
                                }
                            }

                            if (record.berth_datetime <= record.arrival_datetime)
                                return BadRequest("Alongside DateTime must be later than Arrival DateTime.");
                            if (record.start_datetime <= record.berth_datetime)
                                return BadRequest("Commenced Loading DateTime must be later than Alongside DateTime.");
                            if (record.end_datetime <= record.start_datetime)
                                return BadRequest("Completed Loading DateTime must be later than Commenced Loading DateTime.");
                            if (record.unberth_datetime <= record.end_datetime)
                                return BadRequest("Cast Off DateTime must be later than Completed Loading DateTime.");
                            if (record.departure_datetime <= record.unberth_datetime)
                                return BadRequest("Departure DateTime must be later than Cast Off DateTime.");

                            #endregion


                            #region Set barge status

                            var conn = dbContext.Database.GetDbConnection();
                            if (conn.State != System.Data.ConnectionState.Open)
                            {
                                await conn.OpenAsync();
                                if (conn.State == System.Data.ConnectionState.Open)
                                {
                                    using (var cmd = conn.CreateCommand())
                                    {
                                        try
                                        {
                                            cmd.CommandText = $" UPDATE barge SET barge_status = {(int)Common.Constants.BargeStatus.Cargo_On_Water} "
                                                + $" WHERE id = '{record.destination_location_id}' ";
                                            await cmd.ExecuteScalarAsync();
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex.ToString());
                                            return BadRequest(ex.Message);
                                        }
                                    }
                                }
                            }

                            #endregion
                            #region Get transaction number
                            if (record.end_datetime != null)
                            {
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
                                            record.transaction_number = $"BGL-{record.end_datetime:yyyyMMdd}-{r}";
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex.ToString());
                                            return BadRequest(ex.Message);
                                        }
                                    }
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
                    var _record = new DataAccess.Repository.barging_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.BargingTransaction.UpdateStockState(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok(record);
        }

        [HttpPut("Unloading/UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateDataUnloading([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            barging_transaction record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.barging_transaction
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

                            //record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;
                            record.is_loading = false;

                            #region Validation

                            var cekdata = dbContext.barging_transaction
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.despatch_order_id == record.despatch_order_id
                                    && o.is_loading == false && o.id != record.id)
                                .FirstOrDefault();
                            if (cekdata != null)
                            {
                                await tx.RollbackAsync();
                                return BadRequest("The Shipping Order Number already been used on another transaction.");
                            }

                            if (record.quantity <= 0)
                            {
                                return BadRequest("Draft Survey Quantity must be more than zero.");
                            }

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
                                    if ((decimal)(tr1?.capacity ?? 0) < record.quantity)
                                    {
                                        return BadRequest("Transport capacity is less than unloading quantity");
                                    }
                                }
                            }

                            if (record.berth_datetime <= record.arrival_datetime)
                                return BadRequest("Alongside DateTime must be newer than Arrival DateTime.");
                            if (record.start_datetime <= record.berth_datetime)
                                return BadRequest("Commenced Loading DateTime must be newer than Alongside DateTime.");
                            if (record.end_datetime <= record.start_datetime)
                                return BadRequest("Completed Loading DateTime must be newer than Commenced Loading DateTime.");
                            if (record.unberth_datetime <= record.end_datetime)
                                return BadRequest("Cast Off DateTime must be  newer than Completed Loading DateTime.");
                            if (record.departure_datetime <= record.unberth_datetime)
                                return BadRequest("Departure DateTime must be newer than Cast Off DateTime.");

                            #endregion

                            #region Get transaction number

                            var conn = dbContext.Database.GetDbConnection();
                            if (record.end_datetime != null) { }
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
                                        record.transaction_number = $"BGU-{record.end_datetime:yyyyMMdd}-{r}";

                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex.ToString());
                                        return BadRequest(ex.Message);
                                    }
                                }
                            }

                            #endregion

                            #region Set barge status

                            if (conn.State != System.Data.ConnectionState.Open)
                            {
                                await conn.OpenAsync();
                                if (conn.State == System.Data.ConnectionState.Open)
                                {
                                    using (var cmd = conn.CreateCommand())
                                    {
                                        try
                                        {
                                            if (record.end_datetime != null)
                                            {
                                                cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                                var r = await cmd.ExecuteScalarAsync();
                                                record.transaction_number = $"BGU-{record.end_datetime:yyyyMMdd}-{r}";
                                            }
                                            cmd.CommandText = $" UPDATE barge SET barge_status = {(int)Common.Constants.BargeStatus.Cargo_On_Water} "
                                                + $" WHERE id = '{record.destination_location_id}' ";
                                            await cmd.ExecuteScalarAsync();
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex.ToString());
                                            return BadRequest(ex.Message);
                                        }
                                    }
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
                    var _record = new DataAccess.Repository.barging_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.BargingTransaction.UpdateStockState(connectionString, _record));
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
            barging_transaction record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.barging_transaction
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.barging_transaction.Remove(record);

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
                    var _record = new DataAccess.Repository.barging_transaction();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.BargingTransaction.UpdateStockState(connectionString, _record));
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
                        var records = await dbContext.barging_transaction.Where(o => selectedIds.Contains(o.id)).ToListAsync();
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
        [HttpGet("Loading/GetItemsById")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetItemsById(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.lq_proportion
                .Where(o => o.header_id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("Loading/InsertItemData")]
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
                        record.module = "Barging-Loading";
                        var header = dbContext.barging_transaction
                            .Where(o => o.is_loading == true && o.id == record.header_id)
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
                        decimal? quantity = 0;
                        var beltscale = header.beltscale_quantity == null ? 0 : header.beltscale_quantity;
                        var hasil = ((beltscale - header.quantity) / header.quantity) * 100;
                        if (hasil >= -1 || hasil <= 1)
                        {
                            quantity = header.quantity == 0 ? 1 : header.quantity;

                        }
                        else
                        {
                            quantity = beltscale;
                        }
                        //var quantity = header.quantity == 0 ? 1 : header.quantity;
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

        [HttpPut("Loading/UpdateItemData")]
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
                    if (await mcsContext.CanUpdate(dbContext, record.entity_id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);

                        // record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        var header = dbContext.barging_transaction
                            .Where(o => o.is_loading == true && o.id == record.header_id).FirstOrDefault();
                        var detail = dbContext.lq_proportion
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
                        //sum = sum + record.quantity;
                        var quantity = header.quantity == 0 || header.quantity == null ? 1 : header.quantity;
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

        [HttpDelete("Loading/DeleteItemData")]
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
                    if (await mcsContext.CanDelete(dbContext, record.entity_id, CurrentUserContext.AppUserId)
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

        [HttpGet("ProcessFlowIdLookup")]
        public async Task<object> ProcessFlowIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.process_flow
                    .Where(o => o.process_flow_category == Common.ProcessFlowCategory.BARGING
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
                logger.Error(ex.ToString());
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
                    .Select(o =>
                    new
                    {
                        Value = o.id,
                        Text = o.product_name,
                        search = o.product_name.ToLower() + o.product_name.ToUpper()
                    });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("Unloading/ProductIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProductIdLookupUnloading(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.product
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                    new
                    {
                        Value = o.id,
                        Text = o.product_name,
                        search = o.product_name.ToLower() + o.product_name.ToUpper()
                    });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ShippingInstructionIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ShippingInstructionIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.shipping_instruction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.si_number != null)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new
                    {
                        Value = o.id,
                        Text = o.si_number,
                        search = o.si_number.ToLower() + o.si_number.ToUpper()
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
        public async Task<object> SurveyIdLookup(string LocationId,
            DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"Location Id = {LocationId}");

            try
            {
                if (string.IsNullOrEmpty(LocationId))
                {
                    var lookup = dbContext.survey
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.is_draft_survey == true)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.survey_number, search = o.survey_number.ToLower() + o.survey_number.ToUpper() });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.survey.FromSqlRaw(
                          " SELECT s.* FROM survey s "
                        + " INNER JOIN stock_location sl ON sl.id = s.stock_location_id "
                        + " WHERE COALESCE(s.is_draft_survey, FALSE) = TRUE "
                        + " AND s.organization_id = {0} "
                        + " AND sl.id = {1} ",
                            CurrentUserContext.OrganizationId, LocationId
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

        [HttpGet("EquipmentIdLookup")]
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

        [HttpGet("Unloading/EquipmentIdLookup")]
        public async Task<object> EquipmentIdLookupUnloading(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.equipment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.equipment_code, search = o.equipment_code.ToLower() + o.equipment_code.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("Loading/SourceLocationIdLookup")]
        public async Task<object> LoadingSourceLocationIdLookup(string ProcessFlowId,
            DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"ProcessFlowId = {ProcessFlowId}");
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                var lookup = dbContext.port_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
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
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        //     [HttpGet("Unloading/SourceLocationIdLookup")]
        //     [ApiExplorerSettings(IgnoreApi = true)]
        //     public async Task<object> UnloadingSourceLocationIdLookup(string ProcessFlowId,
        //         DataSourceLoadOptions loadOptions)
        //     {
        //         logger.Trace($"ProcessFlowId = {ProcessFlowId}");
        //         logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

        //         try
        //         {
        //             //if (string.IsNullOrEmpty(ProcessFlowId))
        //             //{
        //             //var lookup = from b in dbContext.barge
        //             //             join bt in dbContext.vw_barging_transaction
        //             //             on b.id equals bt.destination_location_id
        //             //             select new
        //             //             {
        //             //                 value = b.id,
        //             //                 text = b.vehicle_name,
        //             //             };
        //             var lookup = from b in dbContext.barge
        //                          join bt in dbContext.vw_barging_transaction
        //                          on b.id equals bt.destination_location_id
        //                          select new
        //                          {
        //                              value = b.id,
        //                              text = b.vehicle_name,
        //                          };
        //             return await DataSourceLoader.LoadAsync(lookup, loadOptions);

        //             //}
        //             //else
        //             //{
        //             //    var lookup = dbContext.barge.FromSqlRaw(
        //             //          " SELECT l.* FROM barge l "
        //             //        + " WHERE l.organization_id = {0} "
        //             //        + " AND l.business_area_id IN ( "
        //             //        + "     SELECT ba.id FROM vw_business_area_structure ba, process_flow pf "
        //             //        + "     WHERE position(pf.source_location_id in ba.id_path) > 0"
        //             //        + "         AND pf.id = {1} "
        //             //        + " ) ", 
        //             //          CurrentUserContext.OrganizationId, ProcessFlowId
        //             //        )
        //             //        .Select(o =>
        //             //            new
        //             //            {
        //             //                value = o.id,
        //             //                text = o.stock_location_name,
        //             //                o.product_id
        //             //            });
        //             //    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
        //             //}

        //         }
        //         catch (Exception ex)
        //{
        //	logger.Error(ex.InnerException ?? ex);
        //             return BadRequest(ex.InnerException?.Message ?? ex.Message);
        //}
        //     }

        [HttpGet("Unloading/SourceLocationIdLookup")]
        public async Task<object> UnloadingSourceLocationIdLookup(string ProcessFlowId,
            DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"ProcessFlowId = {ProcessFlowId}");
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");
            //public async Task<object> UnloadingSourceLocationIdLookup(string DespatchOrderId,
            //  DataSourceLoadOptions loadOptions)
            //{
            //  logger.Trace($"DespatchOrderId = {DespatchOrderId}");
            //logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {

                var lookup = dbContext.barge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = o.vehicle_name,
                            o.product_id,
                            search = o.vehicle_name.ToLower() + o.vehicle_name.ToUpper()
                        });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                /*var lookup = from b in dbContext.barge
                             join bt in dbContext.vw_barging_transaction
                             on b.id equals bt.destination_location_id
                             where b.organization_id == CurrentUserContext.OrganizationId
                                && bt.despatch_order_id == DespatchOrderId
                             select new
                             {
                                 value = b.id,
                                 text = b.vehicle_name,
                             };*/
                //vessel
                /*var lookup = dbContext.vessel
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = o.vehicle_name
                        });*/


                //1
                /*
                var barge = dbContext.barge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name });
                var vessel = dbContext.vessel
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name });
                var lookup = vessel.Union(barge).OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);

                */

                //if (!string.IsNullOrEmpty(DespatchOrderId))
                //    {
                //    var lookup = dbContext.vw_barging_transaction
                //        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                //            && o.despatch_order_id == DespatchOrderId
                //            && String.IsNullOrEmpty(o.source_location_name) == false)
                //        .Select(o => new
                //        {
                //            value = o.id,
                //            text = o.source_location_name,
                //        });
                //    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                //}
                //else
                //{
                //    var lookup = dbContext.vw_barging_transaction
                //        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                //            && String.IsNullOrEmpty(o.source_location_name) == false)
                //        .Select(o => new
                //        {
                //            value = o.id,
                //            text = o.source_location_name,
                //        });
                //    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ShiftIdLookup")]
        public async Task<object> ShiftIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.shift
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.shift_name, search = o.shift_name.ToLower() + o.shift_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpGet("Loading/DestinationLocationIdLookup")]
        public async Task<object> LoadingDestinationLocationIdLookup(string ProcessFlowId,
            DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"ProcessFlowId = {ProcessFlowId}");
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                var lookup = dbContext.barge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = o.vehicle_name,
                            o.product_id,
                            search = o.vehicle_name.ToLower() + o.vehicle_name.ToUpper()
                        });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("Unloading/DestinationLocationIdLookup")]
        public async Task<object> UnloadingDestinationLocationIdLookup(string ProcessFlowId,
            DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"ProcessFlowId = {ProcessFlowId}");
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                if (string.IsNullOrEmpty(ProcessFlowId))
                {
                    var lookup = dbContext.port_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                        //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
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
                    var lookup = dbContext.port_location.FromSqlRaw(
                          " SELECT l.* FROM port_location l "
                        + " WHERE l.organization_id = {0} "
                        + " AND l.business_area_id IN ( "
                        + "     SELECT ba.id FROM vw_business_area_structure ba, process_flow pf "
                        + "     WHERE position(pf.destination_location_id in ba.id_path) > 0"
                        + "         AND pf.id = {1} "
                        + " ) ",
                          CurrentUserContext.OrganizationId, ProcessFlowId
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
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DespatchOrderIdFilterSalesInvoiceCommerceLookup")]
        public async Task<object> DespatchOrderIdFilterSalesInvoiceCommerceLookup(DataSourceLoadOptions loadOptions) 
        {
            try
            {
                var lookup = dbContext.despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => !dbContext.vw_sales_invoice.Any(x => x.invoice_type_name == "Commercial" && x.despatch_order_id == o.id))
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number, search = o.despatch_order_number.ToLower() + o.despatch_order_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DespatchOrderIdFilterIsFinish")]
        public async Task<object> DespatchOrderIdFilterIsFinish(DataSourceLoadOptions loadOptions,string Id)
        {
            try
            {
                var lookup1 = dbContext.despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => !dbContext.vw_sales_invoice.Any(x => x.invoice_type_name == "Commercial" && x.despatch_order_id == o.id))
                    .Where(o => !dbContext.barging_transaction.Any(x => x.is_loading == true && x.is_finish == true && x.despatch_order_id == o.id))
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number, search = o.despatch_order_number.ToLower() + o.despatch_order_number.ToUpper() });
                var lookup2 = dbContext.despatch_order.Where(o => o.id == Id)
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number, search = o.despatch_order_number.ToLower() + o.despatch_order_number.ToUpper() });
                var lookup = lookup1.Union(lookup2);

                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DespatchOrderIdLoadingLookup")]
        public async Task<object> DespatchOrderIdLoadingLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number, search = o.despatch_order_number.ToLower() + o.despatch_order_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("Unloading/AccountingPeriodIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AccountingPeriodIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.accounting_period
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    //&& o.is_closed == null || o.is_closed == false)
                    .OrderByDescending(o => !o.is_closed).ThenBy(o => o.accounting_period_name)
                    .Select(o => new {
                        Value = o.id,
                        Text = o.accounting_period_name + (o.is_closed == true ? " ## Closed" : ""),
                        search = o.accounting_period_name.ToLower() + (o.is_closed == true ? " ## closed" : "")
                    + o.accounting_period_name.ToUpper() + (o.is_closed == true ? " ## CLOSED" : "")
                    });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("Unloading/DespatchOrderIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DespatchOrderIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number, search = o.despatch_order_number.ToLower() + o.despatch_order_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DespatchOrderIdLookupUnloading")]
        public async Task<object> DespatchOrderIdLookupUnloading(DataSourceLoadOptions loadOptions, string id)
        {
            try
            {
                var current = dbContext.barging_transaction
            .Where(o => o.is_loading == false)
            .Select(o => o.despatch_order_id)
            .ToList();

                var lookup = dbContext.vw_despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => !current.Contains(o.id) && o.delivery_term_name.Contains("BARGE"))
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number, search = o.despatch_order_number.ToLower() + o.despatch_order_number.ToUpper() });

                if (id != null)
                {
                    var edit = dbContext.despatch_order
                        .Where(o => o.id == id)
                        .Select(o => new { Value = o.id, Text = o.despatch_order_number, search = o.despatch_order_number.ToLower() + o.despatch_order_number.ToUpper() });

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
                var record = await dbContext.vw_barging_transaction
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
        public async Task<IActionResult> SaveData([FromBody] barging_transaction Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.barging_transaction
                        .Where(o => o.id == Record.id
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);
                        record.InjectFrom(Record);
                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        #region Validation

                        if (record.quantity <= 0)
                        {
                            return BadRequest("Draft Survey Quantity must be more than zero.");
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
                                if ((decimal)(tr1?.capacity ?? 0) < record.quantity)
                                {
                                    return BadRequest("Transport capacity is less than quantity");
                                }
                            }
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
                            qtyOut.qty_out = record.quantity;
                            qtyOut.transaction_datetime = record.end_datetime ?? record.start_datetime;
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
                                qty_out = record.quantity,
                                transaction_datetime = record.end_datetime ?? record.start_datetime
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
                            qtyIn.qty_in = record.quantity;
                            qtyIn.transaction_datetime = record.end_datetime ?? record.start_datetime;
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
                                qty_in = record.quantity,
                                transaction_datetime = record.end_datetime ?? record.start_datetime
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
                        #region Add record

                        record = new barging_transaction();
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

                        dbContext.barging_transaction.Add(record);

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
                                qty_out = record.quantity,
                                transaction_datetime = record.end_datetime ?? record.start_datetime
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
                                qty_in = record.quantity,
                                transaction_datetime = record.end_datetime ?? record.start_datetime
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
                }
                catch (Exception ex)
                {
                    logger.Error(ex.InnerException ?? ex);
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        [HttpDelete("DeleteById/{Id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Debug($"string Id = {Id}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.barging_transaction
                        .Where(o => o.id == Id
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();
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

                        dbContext.barging_transaction.Remove(record);

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

        [HttpGet("BargingLoadingByDespatchOrder")]
        public async Task<object> BargingLoadingByDespatchOrder(DataSourceLoadOptions loadOptions, string id)
        {

                
                var result = new StandardResult();
            try
            {
                var record = dbContext.barging_transaction
                        .Where(o => o.is_loading == true && o.despatch_order_id == id)
                        .FirstOrDefault();
                if (record != null)
                {
                    result.Success = record != null ? true : false;
                    result.Message = result.Success ? "Ok" : "Record not found";
                    result.Data = record;
                }
                else
                {
                    throw new Exception("Data Not Found.");

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                result.Success = false;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
             return result;
        }

        [HttpGet("VoyageNumberIdLookupLoading")]
        public async Task<object> VoyageNumberIdLookupLoading(DataSourceLoadOptions loadOptions, string id)
        {
            try
            {
                var current = dbContext.barging_transaction
            .Where(o => o.is_loading == true)
            .Select(o => o.voyage_number)
            .ToList();

                var lookup = dbContext.barge_rotation
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                   // .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => !current.Contains(o.id))
                    .Select(o => new { Value = o.id, Text = o.voyage_number, search = o.voyage_number.ToLower() + o.voyage_number.ToUpper() });

                if (id != null)
                {
                    var edit = dbContext.barge_rotation
                        .Where(o => o.id == id)
                        .Select(o => new { Value = o.id, Text = o.voyage_number, search = o.voyage_number.ToLower() + o.voyage_number.ToUpper() });

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

        [HttpPost("Loading/UploadDocument")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> LoadingUploadDocument([FromBody] dynamic FileDocument)
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
            /*string currentBarging = string.Empty;

            var barges = dbContext.barge
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Select(o => new { Id = o.id, Text = o.vehicle_name });*/

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                bool isError = false;
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    //cek row kosong tapi mengandung format cell, dianggap ada datanya, padahal tdk valid
                    if (row.Cells.Count() < 12) continue;

                    var process_flow_id = "";
                    var process_flow = dbContext.process_flow
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.process_flow_code == PublicFunctions.IsNullCell(row.GetCell(3))).FirstOrDefault();
                    if (process_flow != null)
                    {
                        process_flow_id = process_flow.id.ToString();
                    }
                    else
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Process Flow Not Found" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        // break;
                        isError = true;

                    }

                    var source_location_id = "";
                    var stockpile_location = dbContext.port_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.port_location_code.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower().Trim()).FirstOrDefault();
                    if (stockpile_location != null)
                    {
                        source_location_id = stockpile_location.id.ToString();
                    }
                    else
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Source Location Not Found" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        // break;
                        isError = true;

                    }

                    var destination_location_id = "";
                    var barge_id = "";
                    var barge = dbContext.barge
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.vehicle_name.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower().Trim()).FirstOrDefault();
                    if (barge != null)
                    {
                        destination_location_id = barge.id.ToString();
                        barge_id = barge.tug_id ?? "";
                    }
                    else
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Barge Not Found" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        // break;
                        isError = true;

                    }

                    var product_id = "";
                    var product = dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.product_code == PublicFunctions.IsNullCell(row.GetCell(12))).FirstOrDefault();
                    if (product != null) product_id = product.id.ToString();

                    var uom_id = "";
                    var uom = dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.uom_name.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(11)).ToLower().Trim()).FirstOrDefault();
                    if (uom != null)
                    {
                        uom_id = uom.id.ToString();
                    }
                    else
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Unit Not Found" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        // break;
                        isError = true;

                    }

                    var equipment_id = "";
                    var equipment = dbContext.equipment
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.equipment_code == PublicFunctions.IsNullCell(row.GetCell(13))).FirstOrDefault();
                    if (equipment != null) equipment_id = equipment.id.ToString();

                    var survey_id = "";
                    var survey = dbContext.survey
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.survey_number == PublicFunctions.IsNullCell(row.GetCell(2))).FirstOrDefault();
                    if (survey != null) survey_id = survey.id.ToString();

                    var business_unit = "";
                    var bu = dbContext.business_unit.Where(o => o.business_unit_code.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(27)).ToLower().Trim()).FirstOrDefault();
                    if (bu != null)
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

                    var voyageNumber = "";
                    var voyage = dbContext.barge_rotation.Where(o => o.voyage_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(14)).ToLower()).FirstOrDefault();
                    if (voyage != null) voyageNumber = voyage.id.ToString();

                    var despatch_order_id = "";
                    var despatch_order = dbContext.vw_despatch_order
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.despatch_order_number == PublicFunctions.IsNullCell(row.GetCell(1))).FirstOrDefault();
                    if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();

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
                                    TransactionNumber = $"BGL-{DateTime.Now:yyyyMMdd}-{r}";
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

                    var record = dbContext.barging_transaction
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.transaction_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower())
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        record.is_loading = true;
                        record.business_unit_id = business_unit;

                        record.voyage_number = voyageNumber;
                        record.despatch_order_id = string.Empty; // despatch_order_id // Temporer untuk Sekarang. // Row 1
                        //record.survey_id = survey_id; // Row 2
                        record.process_flow_id = process_flow_id; // Row 3
                        record.source_location_id = source_location_id; // Row 4
                        record.tug_id = barge_id; // Row 5
                        //record.reference_number = Convert.ToString(row.GetCell(6)); // Row 6
                        //record.initial_draft_survey = PublicFunctions.Tanggal(row.GetCell(7)); // Row 7
                        //record.final_draft_survey = PublicFunctions.Tanggal(row.GetCell(8)); // Row 8
                        record.quantity = PublicFunctions.Desimal(row.GetCell(9)); // Row 9
                        record.beltscale_quantity = PublicFunctions.Desimal(row.GetCell(10)); // Row 10
                        record.uom_id = uom_id; // Row 11
                        record.product_id = product_id; // Row 12
                        //record.equipment_id = equipment_id; // Row 13
                        //record.hour_usage = PublicFunctions.Desimal(row.GetCell(14)); // Row 14
                        //record.note = Convert.ToString(row.GetCell(15)); // Row 15
                        record.arrival_datetime = PublicFunctions.Tanggal(row.GetCell(16)); // Row 16
                        //record.berth_datetime = PublicFunctions.Tanggal(row.GetCell(17)); // Row 17
                        record.start_datetime = PublicFunctions.Tanggal(row.GetCell(19)); //Row 18
                        if (record.start_datetime.Equals(new DateTime(1900, 1, 1, 0, 0, 0)) || record.start_datetime.Equals(new DateTime(0001, 1, 1, 0, 0, 0)))
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Commenced Loading DateTime Must have Correct Format (yyyy-MM-dd HH:mm) or It cannot be null" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            isError = true;
                            break;
                        }

                        record.end_datetime = PublicFunctions.Tanggal(row.GetCell(20)); // Row 19
                        if (record.end_datetime.Equals(new DateTime(1900, 1, 1, 0, 0, 0)) || record.end_datetime.Equals(new DateTime(0001, 1, 1, 0, 0, 0)))
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Completed Loading DateTime Must have Correct Format (yyyy-MM-dd HH:mm) or It cannot be null" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            isError = true;
                            break;
                        }
                        //record.unberth_datetime = PublicFunctions.Tanggal(row.GetCell(20)); // Row 20
                        //record.departure_datetime = PublicFunctions.Tanggal(row.GetCell(21)); // Row 21
                        //record.distance = PublicFunctions.Desimal(row.GetCell(22)); // Row 22
                        record.ref_work_order = PublicFunctions.IsNullCell(row.GetCell(24)); // Row 23
                        //record.intermediate_quantity = PublicFunctions.Desimal(row.GetCell(24)); // Row 24
                        //record.intermediate_time = PublicFunctions.Tanggal(row.GetCell(25)); // Row 25

                        if (despatch_order != null)
                        {
                            record.despatch_order_id = despatch_order.id;
                            record.sales_contract_id = despatch_order.sales_contract_id ?? "";
                            record.customer_id = despatch_order.customer_id ?? "";
                            record.destination_location_id = despatch_order.vessel_id ?? "";
                        }
                        else
                        {
                            record.destination_location_id = destination_location_id;
                        }

                        /* if (PublicFunctions.IsNullCell(row.GetCell(7), "") == "")
                         {
                             record.initial_draft_survey = null;
                         }
                         else
                         {*/
                        record.initial_draft_survey = PublicFunctions.Tanggal(row.GetCell(7));
                        //}

                        /*   if (PublicFunctions.IsNullCell(row.GetCell(8), "") == "")
                           {
                               record.final_draft_survey = null;
                           }
                           else
                           {*/
                        record.final_draft_survey = PublicFunctions.Tanggal(row.GetCell(8));

                        //}

                        /*   if (PublicFunctions.IsNullCell(row.GetCell(15), "") == "")
                           {
                               record.arrival_datetime = null;
                           }
                           else
                           {*/
                        record.arrival_datetime = PublicFunctions.Tanggal(row.GetCell(17));
                        // }

                        /*  if (PublicFunctions.IsNullCell(row.GetCell(16), "") == "")
                          {
                              record.berth_datetime = null;
                          }
                          else
                          {*/
                        record.berth_datetime = PublicFunctions.Tanggal(row.GetCell(18));
                        //  }

                        /* if (PublicFunctions.IsNullCell(row.GetCell(17), "") == "")
                         {
                             teks += "Error in Line : " + (i + 1) + " ==> Commenced Datetime Not Found" + Environment.NewLine;
                             teks += errormessage + Environment.NewLine + Environment.NewLine;
                             gagal = true;
                             isError = true;
                         }
                         else
                         {*/
                        //}
                        /*
                                                if (PublicFunctions.IsNullCell(row.GetCell(18), "") == "")
                                                {
                                                    teks += "Error in Line : " + (i + 1) + " ==> Completed Datetime Not Found" + Environment.NewLine;
                                                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                                                    gagal = true;
                                                    isError = true;
                                                }
                                                else
                                                {*/
                        // }
                        /*
                                                if (PublicFunctions.IsNullCell(row.GetCell(19), "") == "")
                                                {
                                                    record.unberth_datetime = null;
                                                }
                                                else
                                                {*/
                        record.unberth_datetime = PublicFunctions.Tanggal(row.GetCell(21));
                        //}

                        /*if (PublicFunctions.IsNullCell(row.GetCell(20), "") == "")
                        {
                            record.departure_datetime = null;
                        }
                        else
                        {*/
                        record.departure_datetime = PublicFunctions.Tanggal(row.GetCell(22));
                        //}

                        record.distance = PublicFunctions.Desimal(row.GetCell(23));
                        record.intermediate_quantity = PublicFunctions.Desimal(row.GetCell(25));
                        record.intermediate_time = PublicFunctions.Tanggal(row.GetCell(26));

                        /*                        if (record.berth_datetime < record.arrival_datetime)
                                                {
                                                    teks += "Error in Line : " + (i + 1) + " ==> Alongside DateTime must be newer than Arrival DateTime" + Environment.NewLine;
                                                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                                                    gagal = true;
                                                    isError = true;
                                                }
                                                if (record.start_datetime < record.berth_datetime)
                                                {
                                                    teks += "Error in Line : " + (i + 1) + " ==> Commenced Loading DateTime must be newer than Alongside DateTime" + Environment.NewLine;
                                                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                                                    gagal = true;
                                                    isError = true;
                                                }
                                                if (record.unberth_datetime < record.end_datetime)
                                                {
                                                    teks += "Error in Line : " + (i + 1) + " ==> Cast Off DateTime must be  newer than Completed Loading DateTime" + Environment.NewLine;
                                                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                                                    gagal = true;
                                                    isError = true;
                                                }
                                                if (record.berth_datetime < record.arrival_datetime)
                                                {
                                                    teks += "Error in Line : " + (i + 1) + " ==> Alongside DateTime must be newer than Arrival DateTime" + Environment.NewLine;
                                                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                                                    gagal = true;
                                                    isError = true;
                                                }
                                                if (record.departure_datetime < record.unberth_datetime)
                                                {
                                                    teks += "Error in Line : " + (i + 1) + " ==> Departure DateTime must be newer than Cast Off DateTime" + Environment.NewLine;
                                                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                                                    gagal = true;
                                                    isError = true;
                                                }*/

                        /*
                        if (record.berth_datetime <= record.arrival_datetime)
                        {
                            teks += "Error in Line : " + (i + 1)+" ==> Alongside DateTime must be newer than Arrival DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            //break;
                            isError = true;
                        }
                            //return BadRequest("Alongside DateTime must be newer than Arrival DateTime.");
                        if (record.start_datetime <= record.berth_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Commenced Loading DateTime must be newer than Alongside DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            //break;
                            isError = true;
                        }*/
                        //return BadRequest("Commenced Loading DateTime must be newer than Alongside DateTime.");

                        /*
                        //return BadRequest("Completed Loading DateTime must be newer than Commenced Loading DateTime.");
                        if (record.unberth_datetime <= record.end_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Cast Off DateTime must be  newer than Completed Loading DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            //break;
                            isError = true;
                        }
                        //return BadRequest("Cast Off DateTime must be  newer than Completed Loading DateTime.");
                        if (record.departure_datetime <= record.unberth_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Departure DateTime must be newer than Cast Off DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            //break;
                            isError = true;
                        }
                        */
                        //return BadRequest("Departure DateTime must be newer than Cast Off DateTime.");

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new barging_transaction();
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
                        record.is_loading = true;

                        record.transaction_number = TransactionNumber;
                        record.voyage_number = voyageNumber;
                        record.despatch_order_id = string.Empty; // despatch_order_id // Temporer untuk Sekarang. // Row 1
                        //record.survey_id = survey_id; // Row 2
                        record.process_flow_id = process_flow_id; // Row 3
                        record.source_location_id = source_location_id; // Row 4
                        record.tug_id = barge_id; // Row 5
                        //record.reference_number = Convert.ToString(row.GetCell(6)); // Row 6
                        //record.initial_draft_survey = PublicFunctions.Tanggal(row.GetCell(7)); // Row 7
                        //record.final_draft_survey = PublicFunctions.Tanggal(row.GetCell(8)); // Row 8
                        record.quantity = PublicFunctions.Desimal(row.GetCell(9)); // Row 9
                        record.beltscale_quantity = PublicFunctions.Desimal(row.GetCell(10)); // Row 10
                        record.uom_id = uom_id; // Row 11
                        record.product_id = product_id; // Row 12
                        //record.equipment_id = equipment_id; // Row 13
                        //record.hour_usage = PublicFunctions.Desimal(row.GetCell(14)); // Row 14
                        //record.note = Convert.ToString(row.GetCell(15)); // Row 15
                        record.arrival_datetime = PublicFunctions.Tanggal(row.GetCell(17)); // Row 16
                        //record.berth_datetime = PublicFunctions.Tanggal(row.GetCell(17)); // Row 17
                        record.start_datetime = PublicFunctions.Tanggal(row.GetCell(19)); //Row 18

                        if (record.start_datetime.Equals(new DateTime(1900, 1, 1, 0, 0, 0)) || record.start_datetime.Equals(new DateTime(0001, 1, 1, 0, 0, 0)))
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Commenced Loading DateTime Must have Correct Format (yyyy-MM-dd HH:mm) or It cannot be null" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            isError = true;
                            break;
                        }

                        record.end_datetime = PublicFunctions.Tanggal(row.GetCell(20)); // Row 19
                        if (record.end_datetime.Equals(new DateTime(1900, 1, 1, 0, 0, 0)) || record.end_datetime.Equals(new DateTime(0001, 1, 1, 0, 0, 0)))
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Completed Loading DateTime Must have Correct Format (yyyy-MM-dd HH:mm) or It cannot be null" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            isError = true;
                            break;
                        }
                        //record.unberth_datetime = PublicFunctions.Tanggal(row.GetCell(20)); // Row 20
                        //record.departure_datetime = PublicFunctions.Tanggal(row.GetCell(21)); // Row 21
                        //record.distance = PublicFunctions.Desimal(row.GetCell(22)); // Row 22
                        record.ref_work_order = PublicFunctions.IsNullCell(row.GetCell(24)); // Row 23
                        //record.intermediate_quantity = PublicFunctions.Desimal(row.GetCell(24)); // Row 24
                        //record.intermediate_time = PublicFunctions.Tanggal(row.GetCell(25)); // Row 25

                        if (despatch_order != null)
                        {
                            record.despatch_order_id = despatch_order.id;
                            record.sales_contract_id = despatch_order.sales_contract_id ?? "";
                            record.customer_id = despatch_order.customer_id ?? "";
                            record.destination_location_id = despatch_order.vessel_id ?? "";
                        }
                        else
                        {
                            record.destination_location_id = destination_location_id;
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(7), "") == "")
                        {
                            record.initial_draft_survey = null;
                        }
                        else
                        {
                            record.initial_draft_survey = PublicFunctions.Tanggal(row.GetCell(7));

                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(8), "") == "")
                        {
                            record.final_draft_survey = null;
                        }
                        else
                        {
                            record.final_draft_survey = PublicFunctions.Tanggal(row.GetCell(8));

                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(17), "") == "")
                        {
                            record.arrival_datetime = null;
                        }
                        else
                        {
                            record.arrival_datetime = PublicFunctions.Tanggal(row.GetCell(17));

                        }

                        /*if (PublicFunctions.IsNullCell(row.GetCell(16), "") == "")
                        {
                            record.berth_datetime = null;
                        }
                        else
                        {*/
                        record.berth_datetime = PublicFunctions.Tanggal(row.GetCell(18));
                        //if (record.berth_datetime < record.arrival_datetime)
                        /*{
                            teks += "Error in Line : " + (i + 1) + " ==> Alongside DateTime must be newer than Arrival DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            isError = true;
                        }*/
                        //}

                        /*if (PublicFunctions.IsNullCell(row.GetCell(17), "") == "")
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Commenced Datetime Not Found" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            isError = true;
                        }
                        else
                        {*/
                        /*if (record.start_datetime < record.berth_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Commenced Loading DateTime must be newer than Alongside DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            isError = true;
                        }*/
                        // }

                        /* if (PublicFunctions.IsNullCell(row.GetCell(18), "") == "")
                         {
                             teks += "Error in Line : " + (i + 1) + " ==> Completed Datetime Not Found" + Environment.NewLine;
                             teks += errormessage + Environment.NewLine + Environment.NewLine;
                             gagal = true;
                             isError = true;
                         }
                         else
                         {*/
                        /*if (record.unberth_datetime < record.end_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Cast Off DateTime must be newer than Completed Loading DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            isError = true;
                        }*/
                        //}

                        /*  if (PublicFunctions.IsNullCell(row.GetCell(19), "") == "")
                          {
                              record.unberth_datetime = null;
                          }
                          else
                          {*/
                        record.unberth_datetime = PublicFunctions.Tanggal(row.GetCell(21));
                        /*if (record.berth_datetime <= record.arrival_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Alongside DateTime must be newer than Arrival DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            isError = true;
                        }*/
                        //}

                        /*  if (PublicFunctions.IsNullCell(row.GetCell(20), "") == "")
                          {
                              record.departure_datetime = null;
                          }
                          else
                          {*/
                        record.departure_datetime = PublicFunctions.Tanggal(row.GetCell(22));
                        /*if (record.departure_datetime < record.unberth_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Departure DateTime must be newer than Cast Off DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            isError = true;
                        }*/
                        //}

                        record.distance = PublicFunctions.Desimal(row.GetCell(23));
                        record.intermediate_quantity = PublicFunctions.Desimal(row.GetCell(25));
                        record.intermediate_time = PublicFunctions.Tanggal(row.GetCell(26));

                        /*
                        //return BadRequest("Completed Loading DateTime must be newer than Commenced Loading DateTime.");
                        if (record.unberth_datetime <= record.end_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Cast Off DateTime must be  newer than Completed Loading DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            //break;
                            isError = true;
                        }
                        //return BadRequest("Cast Off DateTime must be  newer than Completed Loading DateTime.");
                        if (record.departure_datetime <= record.unberth_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Departure DateTime must be newer than Cast Off DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                            //break;
                            isError = true;
                        }*/

                        dbContext.barging_transaction.Add(record);
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
                if (isError) break;
            }
            wb.Close();
            if (gagal)
            {
                await transaction.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "BargeLoading");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpPost("Unloading/UploadDocument")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UnloadingUploadDocument([FromBody] dynamic FileDocument)
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
                    //cek row kosong tapi mengandung format cell, dianggap ada datanya, padahal tdk valid
                    if (row.Cells.Count() < 15) continue;

                    var process_flow_id = "";
                    var process_flow = dbContext.process_flow
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                        o.process_flow_code.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower().Trim()).FirstOrDefault();
                    if (process_flow != null)
                    {
                        process_flow_id = process_flow.id.ToString();
                    }
                    else
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Process Flow Not Found" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        // break;
                    }

                    var source_location_id = "";
                    var tug_id = "";
                    var barge = dbContext.barge
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.vehicle_name.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower().Trim()).FirstOrDefault();
                    if (barge != null)
                    {
                        source_location_id = barge.id.ToString();
                        tug_id = barge.tug_id ?? "";
                    }
                    else
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Barge Not Found" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        // break;
                    }

                    /*var destination_location_id = "";
                    var port_location = dbContext.port_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.port_location_code.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower().Trim()).FirstOrDefault();
                    if (port_location != null)
                    {
                        destination_location_id = port_location.id.ToString();
                    }
                    else
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Destination Location Not Found" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        // break;
                    }*/

                    var product_id = "";
                    var product = dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.product_code == PublicFunctions.IsNullCell(row.GetCell(11))).FirstOrDefault();
                    if (product != null) product_id = product.id.ToString();

                    var uom_id = "";
                    var uom = dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.uom_name.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(10)).ToLower().Trim()).FirstOrDefault();
                    if (uom != null)
                    {
                        uom_id = uom.id.ToString();
                    }
                    else
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Unit Not Found" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        // break;
                    }

                    var equipment_id = "";
                    var equipment = dbContext.equipment
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.equipment_code == PublicFunctions.IsNullCell(row.GetCell(12))).FirstOrDefault();
                    if (equipment != null) equipment_id = equipment.id.ToString();

                    var survey_id = "";
                    var survey = dbContext.survey
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.survey_number == PublicFunctions.IsNullCell(row.GetCell(2))).FirstOrDefault();
                    if (survey != null) survey_id = survey.id.ToString();

                    //var despatch_order_id = "";
                    //var despatch_order = dbContext.despatch_order
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                    //        o.despatch_order_number == PublicFunctions.IsNullCell(row.GetCell(1))).FirstOrDefault();
                    //if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();
                    var despatch_order = dbContext.vw_despatch_order
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.despatch_order_number == PublicFunctions.IsNullCell(row.GetCell(1))).FirstOrDefault();

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
                                    TransactionNumber = $"BGU-{DateTime.Now:yyyyMMdd}-{r}";
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

                    var record = dbContext.barging_transaction
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.transaction_number.ToLower() == TransactionNumber.ToLower())
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        record.is_loading = true;

                        record.process_flow_id = process_flow_id;
                        record.reference_number = PublicFunctions.IsNullCell(row.GetCell(6));
                        //record.accounting_period_id = accounting_period_id;
                        record.source_location_id = source_location_id;
                        record.destination_location_id = PublicFunctions.IsNullCell(row.GetCell(5));
                        //record.start_datetime = PublicFunctions.Tanggal(row.GetCell(17));
                        //record.end_datetime = PublicFunctions.Tanggal(row.GetCell(18));
                        //record.source_shift_id = shift_id;
                        record.product_id = product_id;
                        record.quantity = PublicFunctions.Desimal(row.GetCell(9));
                        record.uom_id = uom_id;
                        record.equipment_id = equipment_id;
                        record.hour_usage = PublicFunctions.Desimal(row.GetCell(13));
                        record.survey_id = survey_id;
                        //record.despatch_order_id = despatch_order_id;
                        record.note = PublicFunctions.IsNullCell(row.GetCell(14));
                        record.ref_work_order = PublicFunctions.IsNullCell(row.GetCell(22));
                        //record.initial_draft_survey = PublicFunctions.Tanggal(row.GetCell(7));
                        //record.final_draft_survey = PublicFunctions.Tanggal(row.GetCell(8));
                        //record.arrival_datetime = PublicFunctions.Tanggal(row.GetCell(15));
                        //record.berth_datetime = PublicFunctions.Tanggal(row.GetCell(16));
                        //record.unberth_datetime = PublicFunctions.Tanggal(row.GetCell(19));
                        //record.departure_datetime = PublicFunctions.Tanggal(row.GetCell(20));
                        record.tug_id = tug_id;

                        if (despatch_order != null)
                        {
                            record.despatch_order_id = despatch_order.id;
                            record.sales_contract_id = despatch_order.sales_contract_id ?? "";
                            record.customer_id = despatch_order.customer_id ?? "";
                            //record.destination_location_id = despatch_order.vessel_id ?? "";
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(7), "") == "")
                        {
                            record.initial_draft_survey = null;
                        }
                        else
                        {
                            record.initial_draft_survey = PublicFunctions.Tanggal(row.GetCell(7));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(8), "") == "")
                        {
                            record.final_draft_survey = null;
                        }
                        else
                        {
                            record.final_draft_survey = PublicFunctions.Tanggal(row.GetCell(8));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(15), "") == "")
                        {
                            record.arrival_datetime = null;
                        }
                        else
                        {
                            record.arrival_datetime = PublicFunctions.Tanggal(row.GetCell(15));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(16), "") == "")
                        {
                            record.berth_datetime = null;
                        }
                        else
                        {
                            record.berth_datetime = PublicFunctions.Tanggal(row.GetCell(16));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(17), "") == "")
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Commenced Datetime Not Found" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }
                        else
                        {
                            record.start_datetime = PublicFunctions.Tanggal(row.GetCell(17));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(18), "") == "")
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Completed Datetime Not Found" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }
                        else
                        {
                            record.end_datetime = PublicFunctions.Tanggal(row.GetCell(18));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(19), "") == "")
                        {
                            record.unberth_datetime = null;
                        }
                        else
                        {
                            record.unberth_datetime = PublicFunctions.Tanggal(row.GetCell(19));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(20), "") == "")
                        {
                            record.departure_datetime = null;
                        }
                        else
                        {
                            record.departure_datetime = PublicFunctions.Tanggal(row.GetCell(20));
                        }

                        record.distance = PublicFunctions.Desimal(row.GetCell(21));

                        if (record.berth_datetime < record.arrival_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Alongside DateTime must be newer than Arrival DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }
                        if (record.start_datetime < record.berth_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Commenced Loading DateTime must be newer than Alongside DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }
                        if (record.unberth_datetime < record.end_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Cast Off DateTime must be  newer than Completed Loading DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }
                        if (record.berth_datetime < record.arrival_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Alongside DateTime must be newer than Arrival DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }
                        if (record.departure_datetime < record.unberth_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Departure DateTime must be newer than Cast Off DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new barging_transaction();
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
                        record.is_loading = false;

                        record.transaction_number = TransactionNumber;
                        record.process_flow_id = process_flow_id;
                        record.reference_number = PublicFunctions.IsNullCell(row.GetCell(6));
                        //record.accounting_period_id = accounting_period_id;
                        record.source_location_id = source_location_id;
                        record.destination_location_id = PublicFunctions.IsNullCell(row.GetCell(5));
                        //record.start_datetime = PublicFunctions.Tanggal(row.GetCell(17));
                        //record.end_datetime = PublicFunctions.Tanggal(row.GetCell(18));
                        //record.source_shift_id = shift_id;
                        record.product_id = product_id;
                        record.quantity = PublicFunctions.Desimal(row.GetCell(9));
                        record.uom_id = uom_id;
                        record.equipment_id = equipment_id;
                        record.hour_usage = PublicFunctions.Desimal(row.GetCell(13));
                        record.survey_id = survey_id;
                        //record.despatch_order_id = despatch_order_id;
                        record.note = PublicFunctions.IsNullCell(row.GetCell(14));
                        record.ref_work_order = PublicFunctions.IsNullCell(row.GetCell(22));
                        //record.initial_draft_survey = PublicFunctions.Tanggal(row.GetCell(7));
                        //record.final_draft_survey = PublicFunctions.Tanggal(row.GetCell(8));
                        // record.arrival_datetime = PublicFunctions.Tanggal(row.GetCell(15));
                        //record.berth_datetime = PublicFunctions.Tanggal(row.GetCell(16));
                        //record.unberth_datetime = PublicFunctions.Tanggal(row.GetCell(19));
                        //record.departure_datetime = PublicFunctions.Tanggal(row.GetCell(20));
                        record.tug_id = tug_id;

                        if (despatch_order != null)
                        {
                            record.despatch_order_id = despatch_order.id;
                            record.sales_contract_id = despatch_order.sales_contract_id ?? "";
                            record.customer_id = despatch_order.customer_id ?? "";
                            //record.destination_location_id = despatch_order.vessel_id ?? "";
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(7), "") == "")
                        {
                            record.initial_draft_survey = null;
                        }
                        else
                        {
                            record.initial_draft_survey = PublicFunctions.Tanggal(row.GetCell(7));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(8), "") == "")
                        {
                            record.final_draft_survey = null;
                        }
                        else
                        {
                            record.final_draft_survey = PublicFunctions.Tanggal(row.GetCell(8));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(15), "") == "")
                        {
                            record.arrival_datetime = null;
                        }
                        else
                        {
                            record.arrival_datetime = PublicFunctions.Tanggal(row.GetCell(15));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(16), "") == "")
                        {
                            record.berth_datetime = null;
                        }
                        else
                        {
                            record.berth_datetime = PublicFunctions.Tanggal(row.GetCell(16));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(17), "") == "")
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Commenced Datetime Not Found" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }
                        else
                        {
                            record.start_datetime = PublicFunctions.Tanggal(row.GetCell(17));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(18), "") == "")
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Completed Datetime Not Found" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }
                        else
                        {
                            record.end_datetime = PublicFunctions.Tanggal(row.GetCell(18));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(19), "") == "")
                        {
                            record.unberth_datetime = null;
                        }
                        else
                        {
                            record.unberth_datetime = PublicFunctions.Tanggal(row.GetCell(19));
                        }

                        if (PublicFunctions.IsNullCell(row.GetCell(20), "") == "")
                        {
                            record.departure_datetime = null;
                        }
                        else
                        {
                            record.departure_datetime = PublicFunctions.Tanggal(row.GetCell(20));
                        }

                        record.distance = PublicFunctions.Desimal(row.GetCell(21));

                        if (record.berth_datetime <= record.arrival_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Alongside DateTime must be newer than Arrival DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }
                        if (record.start_datetime <= record.berth_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Commenced Loading DateTime must be newer than Alongside DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }
                        if (record.unberth_datetime <= record.end_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Cast Off DateTime must be  newer than Completed Loading DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }
                        if (record.berth_datetime <= record.arrival_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Alongside DateTime must be newer than Arrival DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }
                        if (record.departure_datetime <= record.unberth_datetime)
                        {
                            teks += "Error in Line : " + (i + 1) + " ==> Departure DateTime must be newer than Cast Off DateTime" + Environment.NewLine;
                            teks += errormessage + Environment.NewLine + Environment.NewLine;
                            gagal = true;
                        }

                        dbContext.barging_transaction.Add(record);
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
                HttpContext.Session.SetString("filename", "BargeLoading");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpGet("QualitySamplingIdLookup")]
        public async Task<object> QualitySamplingIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.quality_sampling.FromSqlRaw(
                        "select * from quality_sampling where id not in " +
                        "(select quality_sampling_id from barging_transaction where quality_sampling_id is not null)"
                    )
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

        [HttpGet("Unloading/QualitySamplingIdLookup")]
        public async Task<object> QualitySamplingIdLookupUnloading(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.quality_sampling.FromSqlRaw(
                        "select * from quality_sampling where id not in " +
                        "(select quality_sampling_id from barging_transaction where quality_sampling_id is not null)"
                    )
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

        [HttpGet("Unloading/BusinessUnitIdLookup")]
        public async Task<object> BusinessUnitIdLookupUnloading(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_unit
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.business_unit_name, Search = o.business_unit_name.ToLower() + o.business_unit_name.ToUpper() })
                    .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("Unloading/DraftSurveyIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DraftSurveyIdLookupUnloading(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.draft_survey
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.survey_number, search = o.survey_number.ToLower() + o.survey_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ByBargingTransactionId/{Id}")]
        public async Task<object> ByBargingTransactionId(string Id, DataSourceLoadOptions loadOptions)
        {
            var record = dbContext.vw_barging_transaction.Where(o => o.id == Id).FirstOrDefault();
            var quality_sampling_id = "";
            if (record != null) quality_sampling_id = record.quality_sampling_id;

            return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling_analyte
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.quality_sampling_id == quality_sampling_id),
                loadOptions);
        }

        [HttpGet("GetBargingTransactionLoading/{despatchOrderId}/{destinationLocationId}")]
        public async Task<ApiResponse> GetBargingTransactionLoadingByDespatchOrderId(string despatchOrderId, string destinationLocationId)
        {
            var result = new ApiResponse();
            result.Status.Success = true;
            try
            {
                var record = await dbContext.barging_transaction
                    .Where(r => r.organization_id == CurrentUserContext.OrganizationId &&
                                r.despatch_order_id == despatchOrderId &&
                                r.destination_location_id == destinationLocationId &&
                                r.is_loading == true).FirstOrDefaultAsync();
                result.Data = record;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                result.Status.Success = false;
                result.Status.Message = ex.Message;
            }
            return result;
        }

        [HttpGet("GetBargingTransactionUnloading/{despatchOrderId}/{destinationLocationId}")]
        public async Task<ApiResponse> GetBargingTransactionUnloadingByDespatchOrderId(string despatchOrderId, string destinationLocationId)
        {
            var result = new ApiResponse();
            result.Status.Success = true;
            try
            {
                var record = await dbContext.barging_transaction
                    .Where(r => r.organization_id == CurrentUserContext.OrganizationId &&
                                r.despatch_order_id == despatchOrderId &&
                                r.destination_location_id == destinationLocationId
                                ).FirstOrDefaultAsync();
                result.Data = record;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                result.Status.Success = false;
                result.Status.Message = ex.Message;
            }
            return result;
        }

        [HttpGet("FetchBargingTransactionLoadingIntoShppingTransactionDetail/{despatchOrderId}/{shippingTransactionId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ApiResponse> FetchBargingTransactionLoadingIntoShppingTransactionDetail(string despatchOrderId, string shippingTransactionId)
        {
            var result = new ApiResponse();
            result.Status.Success = true;
            try
            {
                var bargingTransactions = await dbContext.barging_transaction
                    .Where(r => r.organization_id == CurrentUserContext.OrganizationId &&
                                r.despatch_order_id == despatchOrderId &&
                                r.is_loading == true).ToListAsync();

                if (bargingTransactions == null || bargingTransactions.Count <= 0)
                {

                }


                using (var tx = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        decimal totalQuantity = 0;
                        foreach (var item in bargingTransactions)
                        {
                            if (result.Status.Success)
                            {
                                var checkDataByBargingTransactionId = await dbContext.shipping_transaction_detail
                                    .Where(r => r.shipping_transaction_id == shippingTransactionId && r.barging_transaction_id == item.id).ToListAsync();
                                if (checkDataByBargingTransactionId.Count == 0)
                                {
                                    var newRecord = new shipping_transaction_detail();

                                    if (await mcsContext.CanCreate(dbContext, nameof(barging_transaction),
                                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                                    {
                                        #region Add record

                                        newRecord.id = Guid.NewGuid().ToString("N");
                                        newRecord.created_by = CurrentUserContext.AppUserId;
                                        newRecord.created_on = DateTime.Now;
                                        newRecord.modified_by = null;
                                        newRecord.modified_on = null;
                                        newRecord.is_active = true;
                                        newRecord.is_default = null;
                                        newRecord.is_locked = null;
                                        newRecord.entity_id = null;
                                        newRecord.owner_id = CurrentUserContext.AppUserId;
                                        newRecord.organization_id = CurrentUserContext.OrganizationId;


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
                                                    newRecord.transaction_number = $"SHL-{DateTime.Now:yyyyMMdd}-{r}";
                                                }
                                                catch (Exception ex)
                                                {
                                                    await tx.RollbackAsync();
                                                    result.Status.Success = false;
                                                    logger.Error(ex.ToString());
                                                    result.Status.Message = ex.Message;
                                                }
                                            }
                                        }

                                        #endregion

                                        newRecord.shipping_transaction_id = shippingTransactionId;
                                        newRecord.uom_id = item.uom_id;
                                        newRecord.quantity = item.quantity;
                                        newRecord.detail_location_id = item.destination_location_id;
                                        newRecord.barging_transaction_id = item.id;
                                        dbContext.shipping_transaction_detail.Add(newRecord);

                                        await dbContext.SaveChangesAsync();
                                        result.Status.Success &= true;
                                        var subdetail = await dbContext.lq_proportion.Where(o => o.header_id == item.id).ToListAsync();
                                        foreach (var detail in subdetail)
                                        {
                                            var detailRecord = new lq_proportion();

                                            #region Add record

                                            detailRecord.id = Guid.NewGuid().ToString("N");
                                            detailRecord.created_by = CurrentUserContext.AppUserId;
                                            detailRecord.created_on = DateTime.Now;
                                            detailRecord.modified_by = null;
                                            detailRecord.modified_on = null;
                                            detailRecord.is_active = true;
                                            detailRecord.is_default = null;
                                            detailRecord.is_locked = null;
                                            detailRecord.entity_id = null;
                                            detailRecord.owner_id = CurrentUserContext.AppUserId;
                                            detailRecord.organization_id = CurrentUserContext.OrganizationId;
                                            detailRecord.module = "Shipping-Loading-Source";
                                            #endregion
                                            detailRecord.business_unit_id = detail.business_unit_id;
                                            detailRecord.product_id = detail.product_id;
                                            detailRecord.contractor_id = detail.contractor_id;
                                            detailRecord.quantity = detail.quantity;
                                            detailRecord.presentage = detail.presentage;
                                            detailRecord.header_id = newRecord.id;
                                            dbContext.lq_proportion.Add(detailRecord);
                                            await dbContext.SaveChangesAsync();

                                        }
                                    }
                                    else
                                    {
                                        result.Status.Success = false;
                                        result.Status.Message = "User is not authorized.";
                                    }
                                }
                            }
                        }
                        shipping_transaction record;
                        record = dbContext.shipping_transaction
                            .Where(o => o.id == shippingTransactionId)
                            .FirstOrDefault();
                        totalQuantity = await dbContext.shipping_transaction_detail.Where(o => o.shipping_transaction_id == shippingTransactionId).SumAsync(o => o.quantity);
                        if (record != null)
                        {
                            if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                                || CurrentUserContext.IsSysAdmin)
                            {
                                record.quantity = totalQuantity;
                                await dbContext.SaveChangesAsync();
                            }
                        }


                        if (result.Status.Success)
                        {
                            await tx.CommitAsync();
                            result.Status.Message = "Ok";
                        }
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
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                result.Status.Success = false;
                result.Status.Message = ex.Message;
            }
            return result;
        }

    }
}

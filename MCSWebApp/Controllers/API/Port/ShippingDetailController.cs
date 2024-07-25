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
using Microsoft.AspNetCore.Hosting;
using BusinessLogic;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Common;
using Microsoft.AspNetCore.Http;
using Jace.Operations;
using Microsoft.Net.Http.Headers;
using Google.Protobuf.WellKnownTypes;

namespace MCSWebApp.Controllers.API.Port
{
    [Route("api/Port/[controller]")]
    [ApiController]
    public class ShippingDetailController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ShippingDetailController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("ByShippingId/{Id}")]
        public async Task<object> ByShippingIdLoading(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_shipping_transaction_detail
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.shipping_transaction_id == Id),
                loadOptions);
        }

        [HttpGet("LQ/ByShippingId/{Id}")]
        public async Task<object> ByShippingIdLoadingLQ(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.shipping_transaction_lq
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.shipping_transaction_id == Id),
                loadOptions);
        }

        [HttpPost("Loading/InsertData")]
        public async Task<IActionResult> InsertDataLoading([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            shipping_transaction_detail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(shipping_transaction_detail),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        #region Add record

                        record = new shipping_transaction_detail();
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

                        var shippingTx = await dbContext.shipping_transaction
                            .Where(o => o.id == record.shipping_transaction_id)
                            .FirstOrDefaultAsync();
                        if (shippingTx != null)
                        {
                            if (!shippingTx.is_loading)
                            {
                                return BadRequest("Invalid ship loading transaction");
                            }
                        }
                        else
                        {
                            return BadRequest("Invalid ship loading transaction");
                        }

                        #endregion

                        #region Validation

                        if (record.final_quantity.ToString().Trim() != "" && record.reason_id == null)
                        {
                            return BadRequest("The Reason column is required if Return Cargo Quantity not empty.");
                        }

                        // Must be in open accounting period
                        var ap1 = await dbContext.accounting_period
                            .Where(o => o.id == shippingTx.accounting_period_id)
                            .FirstOrDefaultAsync();
                        if (ap1 != null && (ap1?.is_closed ?? false))
                        {
                            return BadRequest("Data update is not allowed");
                        }

                        // Source location != destination location
                        if (record.detail_location_id == shippingTx.ship_location_id)
                        {
                            return BadRequest("Source location must be different from destination location");
                        }

                        if (record.end_datetime <= record.start_datetime)
                            return BadRequest("Complete Unloading Date must be newer than Commenced Unloading Date.");

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
                                    record.transaction_number = $"SHL-{DateTime.Now:yyyyMMdd}-{r}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        #endregion

                        dbContext.shipping_transaction_detail.Add(record);
                        await dbContext.SaveChangesAsync();

                        #region Add sum to shipping_transaction

                        var sum = await dbContext.shipping_transaction_detail
                            .Where(o => o.shipping_transaction_id == shippingTx.id)
                            .SumAsync(o => o.quantity);
                        var returnCargo = await dbContext.shipping_transaction_detail
                            .Where(o => o.shipping_transaction_id == shippingTx.id)
                            .SumAsync(o => o.final_quantity);
                        shippingTx.quantity = (decimal)returnCargo != 0 || (decimal)returnCargo != null ? sum - (decimal)returnCargo : sum;
                        await dbContext.SaveChangesAsync();

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
                    var _record = new DataAccess.Repository.shipping_transaction_detail();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ShippingTransaction.UpdateStockStateLoading(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok(record);
        }

        [HttpPost("InsertItemData")]
        public async Task<IActionResult> InsertItemData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            shipping_transaction_lq record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(shipping_transaction_lq),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        #region Add record

                        record = new shipping_transaction_lq();
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

                        var shippingTx = await dbContext.shipping_transaction
                            .Where(o => o.id == record.shipping_transaction_id)
                            .FirstOrDefaultAsync();
                        if (shippingTx != null)
                        {
                            if (!shippingTx.is_loading)
                            {
                                return BadRequest("Invalid ship loading transaction");
                            }
                        }
                        else
                        {
                            return BadRequest("Invalid ship loading transaction");
                        }

                        #endregion

                        #region Validation

                        if (record.final_quantity.ToString().Trim() != "" && record.reason_id == null)
                        {
                            return BadRequest("The Reason column is required if Return Cargo Quantity not empty.");
                        }

                        // Must be in open accounting period
                        var ap1 = await dbContext.accounting_period
                            .Where(o => o.id == shippingTx.accounting_period_id)
                            .FirstOrDefaultAsync();
                        if (ap1 != null && (ap1?.is_closed ?? false))
                        {
                            return BadRequest("Data update is not allowed");
                        }

                        // Source location != destination location
                        if (record.detail_location_id == shippingTx.ship_location_id)
                        {
                            return BadRequest("Source location must be different from destination location");
                        }

                        if (record.end_datetime <= record.start_datetime)
                            return BadRequest("Complete Unloading Date must be newer than Commenced Unloading Date.");

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
                                    record.transaction_number = $"SHL-{DateTime.Now:yyyyMMdd}-{r}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        #endregion

                        dbContext.shipping_transaction_lq.Add(record);
                        await dbContext.SaveChangesAsync();

                        #region Add sum to shipping_transaction

                        var sum = await dbContext.shipping_transaction_detail
                            .Where(o => o.shipping_transaction_id == shippingTx.id)
                            .SumAsync(o => o.quantity);
                        var returnCargo = await dbContext.shipping_transaction_detail
                            .Where(o => o.shipping_transaction_id == shippingTx.id)
                            .SumAsync(o => o.final_quantity);
                        shippingTx.quantity = (decimal)returnCargo != 0 || (decimal)returnCargo != null ? sum - (decimal)returnCargo : sum;
                        await dbContext.SaveChangesAsync();

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
                    var _record = new DataAccess.Repository.shipping_transaction_detail();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ShippingTransaction.UpdateStockStateLoading(connectionString, _record));
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
        public async Task<IActionResult> UpdateDataLoading([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.shipping_transaction_detail
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            JsonConvert.PopulateObject(values, record);

                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            var shippingTx = await dbContext.shipping_transaction
                                .Where(o => o.id == record.shipping_transaction_id)
                                .FirstOrDefaultAsync();
                            if (!shippingTx.is_loading)
                            {
                                return BadRequest("Invalid ship loading transaction");
                            }

                            #region Validation

                            if (record.final_quantity.ToString().Trim() != "" && record.reason_id == null)
                            {
                                return BadRequest("The Reason column is required if Return Cargo Quantity not empty.");
                            }

                            // Must be in open accounting period
                            var ap1 = await dbContext.accounting_period
                                .Where(o => o.id == shippingTx.accounting_period_id)
                                .FirstOrDefaultAsync();
                            if (ap1 != null && (ap1?.is_closed ?? false))
                            {
                                return BadRequest("Data update is not allowed");
                            }

                            // Source location != destination location
                            if (record.detail_location_id == shippingTx.ship_location_id)
                            {
                                return BadRequest("Source location must be different from destination location");
                            }

                            if (record.end_datetime <= record.start_datetime)
                                return BadRequest("Complete Unloading Date must be newer than Commenced Unloading Date.");

                            #endregion

                            #region Get transaction number

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
                                            cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                            var r = await cmd.ExecuteScalarAsync();
                                            record.transaction_number = $"SH-{DateTime.Now:yyyyMMdd}-{r}";
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

                            #region Add sum to shipping_transaction

                            var sum = await dbContext.shipping_transaction_detail
                                .Where(o => o.shipping_transaction_id == shippingTx.id)
                                .SumAsync(o => o.quantity);
                            var returnCargo = await dbContext.shipping_transaction_detail
                            .Where(o => o.shipping_transaction_id == shippingTx.id)
                            .SumAsync(o => o.final_quantity);
                            if (returnCargo != null)
                            {
                                shippingTx.quantity = sum - (decimal)returnCargo;
                            }
                            else
                            {
                                shippingTx.quantity = sum;
                            }
                            await dbContext.SaveChangesAsync();

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
                    var _record = new DataAccess.Repository.shipping_transaction_detail();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ShippingTransaction.UpdateStockStateLoading(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok(success);
        }

        [HttpPut("UpdateItemData")]
        public async Task<IActionResult> UpdateItemData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            shipping_transaction_lq record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.shipping_transaction_lq
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            JsonConvert.PopulateObject(values, record);

                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            

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
                    var _record = new DataAccess.Repository.shipping_transaction_detail();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ShippingTransaction.UpdateStockStateLoading(connectionString, _record));
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
        public async Task<IActionResult> InsertDataUnloading([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            shipping_transaction_detail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(shipping_transaction_detail),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        #region Add record

                        record = new shipping_transaction_detail();
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

                        var shippingTx = await dbContext.shipping_transaction
                            .Where(o => o.id == record.shipping_transaction_id)
                            .FirstOrDefaultAsync();
                        if (shippingTx.is_loading)
                        {
                            return BadRequest("Invalid ship unloading transaction");
                        }

                        #endregion

                        #region Validation

                        // Must be in open accounting period
                        var ap1 = await dbContext.accounting_period
                            .Where(o => o.id == shippingTx.accounting_period_id)
                            .FirstOrDefaultAsync();
                        if (ap1 != null && (ap1?.is_closed ?? false))
                        {
                            return BadRequest("Data update is not allowed");
                        }

                        // Source location != destination location
                        if (record.detail_location_id == shippingTx.ship_location_id)
                        {
                            return BadRequest("Source location must be different from destination location");
                        }

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
                                    record.transaction_number = $"SHU-{DateTime.Now:yyyyMMdd}-{r}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        #endregion

                        dbContext.shipping_transaction_detail.Add(record);
                        await dbContext.SaveChangesAsync();

                        #region Add sum to shipping_transaction

                        var sum = await dbContext.shipping_transaction_detail
                            .Where(o => o.shipping_transaction_id == shippingTx.id)
                            .SumAsync(o => o.quantity);
                        shippingTx.quantity = sum;
                        await dbContext.SaveChangesAsync();

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
                    var _record = new DataAccess.Repository.shipping_transaction_detail();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ShippingTransaction.UpdateStockStateLoading(connectionString, _record));
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
        public async Task<IActionResult> UpdateDataUnloading([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            shipping_transaction_detail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.shipping_transaction_detail
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            JsonConvert.PopulateObject(values, record);

                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            var shippingTx = await dbContext.shipping_transaction
                                .Where(o => o.id == record.shipping_transaction_id)
                                .FirstOrDefaultAsync();
                            if (shippingTx.is_loading)
                            {
                                return BadRequest("Invalid ship unloading transaction");
                            }

                            #region Validation

                            // Must be in open accounting period
                            var ap1 = await dbContext.accounting_period
                                .Where(o => o.id == shippingTx.accounting_period_id)
                                .FirstOrDefaultAsync();
                            if (ap1 != null && (ap1?.is_closed ?? false))
                            {
                                return BadRequest("Data update is not allowed");
                            }

                            // Source location != destination location
                            if (record.detail_location_id == shippingTx.ship_location_id)
                            {
                                return BadRequest("Source location must be different from destination location");
                            }

                            #endregion

                            #region Get transaction number

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
                                            cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                            var r = await cmd.ExecuteScalarAsync();
                                            record.transaction_number = $"SH-{DateTime.Now:yyyyMMdd}-{r}";
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

                            #region Add sum to shipping_transaction

                            var sum = await dbContext.shipping_transaction_detail
                                .Where(o => o.shipping_transaction_id == shippingTx.id)
                                .SumAsync(o => o.quantity);
                            shippingTx.quantity = sum;
                            await dbContext.SaveChangesAsync();

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
                    var _record = new DataAccess.Repository.shipping_transaction_detail();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ShippingTransaction.UpdateStockStateLoading(connectionString, _record));
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
            shipping_transaction_detail record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                     record = dbContext.shipping_transaction_detail
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.shipping_transaction_detail.Remove(record);
                            await dbContext.SaveChangesAsync();

                            #region Update sum of shipping_transaction

                            var shippingTx = await dbContext.shipping_transaction
                                .Where(o => o.id == record.shipping_transaction_id)
                                .FirstOrDefaultAsync();
                            if (shippingTx != null)
                            {
                                var sum = await dbContext.shipping_transaction_detail
                                    .Where(o => o.shipping_transaction_id == shippingTx.id)
                                    .SumAsync(o => o.quantity);
                                shippingTx.quantity = sum;
                                await dbContext.SaveChangesAsync();
                            }

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
                    var _record = new DataAccess.Repository.shipping_transaction_detail();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ShippingTransaction.UpdateStockStateLoading(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok();
        }
        [HttpDelete("DeleteItemData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteItemData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            var success = false;
            shipping_transaction_lq record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.shipping_transaction_lq
                       .Where(o => o.id == key)
                       .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.shipping_transaction_lq.Remove(record);
                            await dbContext.SaveChangesAsync();

                            #region Update sum of shipping_transaction

                          

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
                    var _record = new DataAccess.Repository.shipping_transaction_detail();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.ShippingTransaction.UpdateStockStateLoading(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok();
        }

        //     [HttpGet("Loading/SourceLocationIdLookup")]
        //     public async Task<object> LoadingSourceLocationIdLookup(string ProcessFlowId,
        //         DataSourceLoadOptions loadOptions)
        //     {
        //         logger.Trace($"ProcessFlowId = {ProcessFlowId}");
        //         logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

        //         try
        //         {
        //             var lookup = from b in dbContext.barge
        //                          join bt in dbContext.vw_barging_transaction
        //                          on b.id equals bt.destination_location_id
        //                          select new
        //                          {
        //                              value = b.id,
        //                              text = b.vehicle_name,
        //                          };
        //             return await DataSourceLoader.LoadAsync(lookup, loadOptions);

        //             //if (string.IsNullOrEmpty(ProcessFlowId))
        //             //{
        //             //    var lookup = dbContext.barge
        //             //        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //             //        .Select(o =>
        //             //            new
        //             //            {
        //             //                value = o.id,
        //             //                text = o.stock_location_name
        //             //            });
        //             //    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
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
        //             //                text = o.stock_location_name
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

        [HttpGet("Loading/SourceLocationIdLookup")]
        public async Task<object> LoadingSourceLocationIdLookup(string DespatchOrderId, DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"DespatchOrderId = {DespatchOrderId}");
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                if (DespatchOrderId != null)
                {
                    var barging = dbContext.barge
                          .Where(b => b.organization_id == CurrentUserContext.OrganizationId)
                          .Join(
                              dbContext.vw_barging_transaction,
                              b => b.id,
                              bt => bt.destination_location_id,
                              (b, bt) => new
                              {
                                  value = b.id,
                                  text = b.vehicle_name,
                                  despatch_order_id = bt.despatch_order_id  // Include the needed property here
                              })
                          .Where(joined => joined.despatch_order_id == DespatchOrderId)
                          .Select(o=> new {value = o.value, text = o.text});
                    //return await DataSourceLoader.LoadAsync(lookup, loadOptions);

                    var stockpileLocation = dbContext.stockpile_location.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        //.Where(o => o.is_virtual == false || o.is_virtual == null)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                        .Select(o => new { value = o.id, text = o.stockpile_location_code });

                    var lookup = barging.Union(stockpileLocation).OrderBy(o => o.text);

                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);

                    // where b.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID")
                    // 20020813 tanpa filter DO
                    //   && bt.despatch_order_id == DespatchOrderId
                }
                else
                {
                    return await DataSourceLoader.LoadAsync(dbContext.barge.Where(o => o.organization_id == CurrentUserContext.OrganizationId).Select(o=> new {Value = o.id, Text = o.vehicle_name}), loadOptions);
                }
                //if (!string.IsNullOrEmpty(DespatchOrderId))
                //{
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
                                text = o.stock_location_name
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
                        + "     WHERE position(pf.source_location_id in ba.id_path) > 0"
                        + "         AND pf.id = {1} "
                        + " ) ", 
                          CurrentUserContext.OrganizationId, ProcessFlowId
                        )
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.stock_location_name
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

        [HttpGet("SurveyorIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SurveyorIdLookup(DataSourceLoadOptions loadOptions)
        {
            //logger.Trace($"Location Id = {LocationId}");

            try
            {
                var lookup = dbContext.contractor
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.is_surveyor == true)
                    // .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { value = o.id, text = o.business_partner_name });
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
            //logger.Trace($"Location Id = {LocationId}");

            try
            {
                if (string.IsNullOrEmpty(LocationId))
                {
                    var lookup = dbContext.survey
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.is_draft_survey == true)
                       // .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { value = o.id, text = o.survey_number });
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
                                text = o.survey_number
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
                    .Select(o => new { Value = o.id, Text = o.equipment_code });
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

        [HttpGet("CheckQuantity")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<decimal> CheckQuantity(string id)
        {
            var header = dbContext.shipping_transaction.Where(o => o.id == id).FirstOrDefault();
            var detail = await dbContext.shipping_transaction_lq.Where(o=>o.shipping_transaction_id == id).ToListAsync();
            decimal? total = 0;
            foreach (var item in detail)
            {
                //if (item.quantity != null)
                //{
                //    total += item.quantity;
                //}
                total += item.quantity;
            }
            decimal difference = (decimal)total - (decimal)header.original_quantity;
            return (difference);

        }

            [HttpGet("FetchSourceIntoLQ")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ApiResponse> FetchSourceIntoLQ(string Id)
        {
            var result = new ApiResponse();
            result.Status.Success = true;
            try
            {
                var sourceList = await dbContext.shipping_transaction_detail
                    .Where(r => r.organization_id == CurrentUserContext.OrganizationId && r.shipping_transaction_id == Id).ToListAsync();
               

                if (sourceList.Any())
                {
                   // return BadRequest("You Don't Have Data in Source Tab");
                }


                using (var tx = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        /*var current = await dbContext.shipping_transaction_lq.ToListAsync();
                        dbContext.shipping_transaction_lq.RemoveRange(current);
                        await dbContext.SaveChangesAsync();*/
                        decimal totalquantity = 0;
                        decimal? totalreturn= 0;
                        foreach (var list in sourceList) // quantity = original quantity | origina quantity = draft suryey quantity | final quantity = return cargo quantity
                        {
                            totalquantity += list.quantity;
                            if (list.final_quantity != null)
                            {
                                totalreturn += list.final_quantity;
                            }
                        }
                        foreach (var item in sourceList)
                        {

                            if (result.Status.Success)
                            {
                                var checkDataByShippingTransactionId = await dbContext.shipping_transaction_lq
                                    .Where(r => r.shipping_transaction_id == Id && r.barging_transaction_id == item.barging_transaction_id && r.header_id == item.id).FirstOrDefaultAsync();
                                var headerQuantity = dbContext.shipping_transaction.Where(o => o.id == item.shipping_transaction_id).FirstOrDefault();
                                if (checkDataByShippingTransactionId == null)
                                {
                                    var record = new shipping_transaction_lq();
                                    if (await mcsContext.CanCreate(dbContext, nameof(shipping_transaction),
                                            CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
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
                                        record.business_unit_id = item.business_unit_id;

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
                                                    record.transaction_number = $"SHL-{DateTime.Now:yyyyMMdd}-{r}";
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
                                        decimal? A1 = (decimal)(item.quantity - (item.final_quantity == null ? 0 : item.final_quantity));
                                   
                                    	decimal? A2 = (decimal)headerQuantity.quantity;
                                   
                                    	decimal? A3 = (decimal)(headerQuantity.original_quantity - headerQuantity.quantity);
                                    
                                    	decimal? A4 = (decimal)(item.quantity - (item.final_quantity == null ? 0 : item.final_quantity));
                                 
                                    	decimal? quantity = (decimal) ( (A1 / A2) * A3 ) + A4;
                                        record.detail_location_id = item.detail_location_id;
                                        record.quantity = (decimal)quantity;
                                        record.uom_id = item.uom_id;
                                        record.reference_number = item.reference_number;
                                        record.arrival_datetime = item.arrival_datetime;
                                        record.arrival_datetime = item.arrival_datetime;
                                        record.start_datetime = item.start_datetime;
                                        record.end_datetime = item.end_datetime;
                                        record.departure_datetime = item.departure_datetime;
                                        record.equipment_id = item.equipment_id;
                                        record.hour_usage = item.hour_usage;
                                        record.survey_id = item.survey_id;
                                        record.barging_transaction_id = item.barging_transaction_id;
                                        record.final_quantity = item.final_quantity;
                                        record.reason_id = item.reason_id;
                                        record.note = item.note;
                                        record.shipping_transaction_id = Id;
                                        record.header_id = item.id;
                                        dbContext.shipping_transaction_lq.Add(record);
                                        await dbContext.SaveChangesAsync();

                                        var detailSource = dbContext.lq_proportion.Where(o => o.header_id == item.id && o.module == "Shipping-Loading-Source").ToList();
                                        foreach(var  detail in detailSource)
                                        {
                                            var newDetail = new lq_proportion();
                                            newDetail.InjectFrom(detail);
                                            newDetail.id = Guid.NewGuid().ToString("N");
                                            newDetail.created_by = CurrentUserContext.AppUserId;
                                            newDetail.created_on = DateTime.Now;
                                            newDetail.modified_by = null;
                                            newDetail.modified_on = null;
                                            newDetail.is_active = true;
                                            newDetail.is_default = null;
                                            newDetail.is_locked = null;
                                            newDetail.entity_id = null;
                                            newDetail.owner_id = CurrentUserContext.AppUserId;
                                            newDetail.organization_id = CurrentUserContext.OrganizationId;
                                            newDetail.business_unit_id = detail.business_unit_id;

                                            newDetail.alias_id = detail.id;
                                            newDetail.module = "Shipping-Loading-LQ";
                                            newDetail.header_id = record.id;

                                            dbContext.lq_proportion.Add(newDetail);
                                            await dbContext.SaveChangesAsync();
                                        }
                                        result.Status.Success &= true;
                                    }
                                    else
                                    {
                                        result.Status.Success = false;
                                        result.Status.Message = "User is not authorized.";
                                    }
                                }else
                                {
                                    decimal? A1 = (decimal)(item.quantity - (item.final_quantity == null ? 0 : item.final_quantity));
                                    // A1 = QUANTITY - RETURN CARGO = 5.900
                                    decimal? A2 = (decimal)headerQuantity.quantity;
                                    // A2 = HEADER ORIGINAL QUANTITY = 14.000
                                    decimal? A3 = (decimal)(headerQuantity.original_quantity - headerQuantity.quantity);
                                    // A3 = DRAFT SURVEY - ORIGINAL QUANTITY = 6.000
                                    decimal? A4 = (decimal)(item.quantity - (item.final_quantity == null ? 0 : item.final_quantity));
                                    // A4 = A1 = 5900

                                    // RUMUS = ( (5.900 / 14.000) * 6.000) + 5.900;
                                    decimal? quantity = (decimal) ( (A1 / A2) * A3 ) + A4;
                                    //RESULT = 8428.57

                                    //decimal quantity = (6900 - 1000 ) / 13000m;

                                    checkDataByShippingTransactionId.detail_location_id = item.detail_location_id;
                                    checkDataByShippingTransactionId.quantity = (decimal)quantity;
                                    checkDataByShippingTransactionId.uom_id = item.uom_id;
                                    checkDataByShippingTransactionId.reference_number = item.reference_number;
                                    checkDataByShippingTransactionId.arrival_datetime = item.arrival_datetime;
                                    checkDataByShippingTransactionId.arrival_datetime = item.arrival_datetime;
                                    checkDataByShippingTransactionId.start_datetime = item.start_datetime;
                                    checkDataByShippingTransactionId.end_datetime = item.end_datetime;
                                    checkDataByShippingTransactionId.departure_datetime = item.departure_datetime;
                                    checkDataByShippingTransactionId.equipment_id = item.equipment_id;
                                    checkDataByShippingTransactionId.hour_usage = item.hour_usage;
                                    checkDataByShippingTransactionId.survey_id = item.survey_id;
                                    checkDataByShippingTransactionId.barging_transaction_id = item.barging_transaction_id;
                                    checkDataByShippingTransactionId.reason_id = item.reason_id;
                                    checkDataByShippingTransactionId.note = item.note;
                                    checkDataByShippingTransactionId.shipping_transaction_id = Id;
                                    checkDataByShippingTransactionId.header_id = item.id;
                                    await dbContext.SaveChangesAsync();
                                    var detailSource = dbContext.lq_proportion.Where(o => o.header_id == item.id && o.module == "Shipping-Loading-Source").ToList();
                                    foreach (var detail in detailSource)
                                    {
                                        var currentDetail = dbContext.lq_proportion.Where(o => o.alias_id == detail.id).FirstOrDefault();
                                        if (currentDetail == null)
                                        {
                                            var newDetail = new lq_proportion();
                                            newDetail.InjectFrom(detail);
                                            newDetail.id = Guid.NewGuid().ToString("N");
                                            newDetail.created_by = CurrentUserContext.AppUserId;
                                            newDetail.created_on = DateTime.Now;
                                            newDetail.modified_by = null;
                                            newDetail.modified_on = null;
                                            newDetail.is_active = true;
                                            newDetail.is_default = null;
                                            newDetail.is_locked = null;
                                            newDetail.entity_id = null;
                                            newDetail.owner_id = CurrentUserContext.AppUserId;
                                            newDetail.organization_id = CurrentUserContext.OrganizationId;
                                            newDetail.business_unit_id = detail.business_unit_id;

                                            newDetail.alias_id = detail.id;
                                            newDetail.module = "Shipping-Loading-LQ";
                                            newDetail.header_id = checkDataByShippingTransactionId.id;

                                            dbContext.lq_proportion.Add(newDetail);
                                            await dbContext.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            currentDetail.product_id = detail.product_id;
                                            currentDetail.business_unit_id= detail.business_unit_id;
                                            currentDetail.contractor_id = detail.contractor_id;
                                            currentDetail.quantity = detail.quantity;
                                            currentDetail.presentage = detail.presentage;
                                            currentDetail.is_return = detail.is_return;
                                            currentDetail.adjustment = detail.adjustment;
                                            currentDetail.alias_id = detail.id;
                                            currentDetail.module = "Shipping-Loading-LQ";
                                            currentDetail.header_id = checkDataByShippingTransactionId.id;

                                            await dbContext.SaveChangesAsync();
                                        }
                                    }
                                }
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

        [HttpGet("HoursUsage/{startTime}/{endTime}")]
        public async Task<object> HoursUsage(DateTime endTime, DateTime startTime)
        {
            try
            {
                if(endTime != null && startTime != null)
                {
                    TimeSpan duration;
                    duration = endTime - startTime;
                    return duration.ToString();
                }
                return "00:00:00";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

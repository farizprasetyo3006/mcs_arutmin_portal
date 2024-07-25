using DataAccess.DTO;
using DataAccess.EFCore.Repository;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NLog;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Omu.ValueInjecter;

namespace MCSWebApp.Controllers.API.Port
{
    [Route("api/Port/Barging/[controller]")]
    [ApiController]
    public class RotationController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        public RotationController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGridLoading(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            logger.Debug($"tanggal1 = {tanggal1}");
            logger.Debug($"tanggal2 = {tanggal2}");

            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_barge_rotation
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.is_active == true
                            ).OrderBy(o => o.eta_loading_port),
                    loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.barge_rotation
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.is_active == true
                        )
                .OrderBy(o=>o.eta_loading_port),
                loadOptions);
        }

        [HttpGet("DataDetail")]
        public async Task<object> DataDetail(string Id)
        {
            try
            {
                var recordDat = await dbContext.vw_barge_rotation
                    .Where(o => o.id == Id)
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                return recordDat;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertDataRotation([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var record = new barge_rotation();

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(barge_rotation),
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

                        #endregion

                        #region Validation

                        if (record.net_quantity <= 0)
                        {
                            return BadRequest("Quantity must be more than zero.");
                        }

                        // Source location != destination location
                        if (record.source_location == record.destination_location)
                        {
                            return BadRequest("Source location must be different from destination location");
                        }

                        if (record.eta_loading_port != null && record.eta_discharge_port != null)
                        {
                            DateTime dtmLoading = (DateTime)record.eta_loading_port;
                            DateTime dtmDischarge = Convert.ToDateTime(record.eta_discharge_port);

                            if (dtmDischarge <= dtmLoading)
                                return BadRequest("Loading DateTime must be newer than Discharge DateTime.");
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
                            await using var cmd = conn.CreateCommand();
                            try
                            {
                                cmd.CommandText = "SELECT nextval('seq_transaction_number')";
                                var r = await cmd.ExecuteScalarAsync();
                                record.transaction_id = $"BGR-{DateTime.Now:yyyyMMdd}-{r}";

                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.ToString());
                                return BadRequest(ex.Message);
                            }
                        }

                        #endregion

                        #region Get Voyage Number

                        var digitBarge = record.transport_id;
                        var digitSite = record.source_location;
                        if (record.eta_loading_port != null)
                        {
                            var digitYear = record.eta_loading_port.Value.Year % 100;

                            var barge = digitBarge;
                            var transportDat = await dbContext.transport
                                .Where(x => x.id == barge).SingleOrDefaultAsync();
                            digitBarge = transportDat.vehicle_id;

                            var site = digitSite;
                            var masterListDat = await dbContext.master_list
                                .Where(x => x.id == site).SingleOrDefaultAsync();
                            digitSite = masterListDat.notes;

                            var szLike = digitBarge + "-" + digitSite + "-" + digitYear;

                            var searchVoyage = await dbContext.barge_rotation
                                .Where(x => x.voyage_number.Contains(szLike)).ToListAsync();

                            var digitIndex = (searchVoyage.Count + 1).ToString("000");
                            var voyageNumber = digitIndex + "-" + digitBarge + "-" + digitSite + "-" + digitYear;
                            record.voyage_number = voyageNumber;
                        }

                        #endregion

                        dbContext.barge_rotation.Add(record);

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
        public async Task<IActionResult> UpdateDataRotation([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            barge_rotation record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.barge_rotation
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

                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;
                            record.is_active = true;

                            #region Validation

                            if (record.net_quantity <= 0)
                            {
                                return BadRequest("Quantity must be more than zero.");
                            }

                            // Source location != destination location
                            if (record.source_location == record.destination_location)
                            {
                                return BadRequest("Source location must be different from destination location");
                            }

                            if (record.eta_loading_port != null && record.eta_discharge_port != null)
                            {
                                DateTime dtmLoading = (DateTime)record.eta_loading_port;
                                DateTime dtmDischarge = Convert.ToDateTime(record.eta_discharge_port);

                                if (dtmDischarge <= dtmLoading)
                                    return BadRequest("Loading DateTime must be newer than Discharge DateTime.");
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
            return Ok(record);
        }

        [HttpGet("VoyageNumberLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> TugIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.barge_rotation
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.voyage_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");
            barge_rotation record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.barge_rotation
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.barge_rotation.Remove(record);

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
            return Ok();
        }
    }
}

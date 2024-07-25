using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataAccess.DTO;
using Omu.ValueInjecter;
using DataAccess.EFCore.Repository;
using FastReport.Data;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.Formula.Functions;

namespace MCSWebApp.Controllers.API.Mining
{
    [Route("api/Mining/[controller]")]
    [ApiController]
    public class ChlsController : ApiBaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ChlsController(IConfiguration configuration, IOptions<SysAdmin> sysAdminOption)
            : base(configuration, sysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        Dictionary<int, string> dicGroup = new Dictionary<int, string>
        {
            {1, "Production Today" },
            {2, "Daily Stock CPP" },
            {3, "Use of Fuel of Genset 4" },
            {4, "Use of Fuel of Genset 5" },
            {5, "PLN CPP" }
        };

        Dictionary<int, string> dicDescription = new Dictionary<int, string>
        {
            {1, "Total ritase dari tambang ke Hopper" },
            {2, "Total ritase keluar dari G1" },
            {3, "Total ritase ke ROM" },
            {4, "Total ritase langsiran dari ROM ke Hopper" },
            {5, "Belt Scale CV 11" },
            {6, "Belt Scale CV 13" },
            {7, "Belt Scale OLC" },
            {8, "Stock Daily Fuel tank 5,000 ltr" },
            {9, "Stock Daily Fuel tank 30,000 ltr" },
            {10, "Stock Air DSS Setlling Pond (awal shift)" },
            {11, "Stock Air DSS Setlling Pond (akhir shift)" },
            {12, "Stock air bersih, reservoir tank (awal shift)" },
            {13, "Stock air bersih, reservoir tank (akhir shift)" },
            {14, "Pemakaian air DSS" },
            {15, "Pemakaian Chemical iso tank PIC 103" },
            {16, "Input" },
            {17, "Output" },
            {18, "KWh" },
            {19, "Kvarh" },
            {20, "Run Hour" },
            {21, "Input" },
            {22, "Output" },
            {23, "KWh" },
            {24, "Kvarh" },
            {25, "Run Hour" },
            {26, "Reading Meter" },
            {27, "Total ritase keluar dari G2" },
            {28, "Total ritase keluar dari G3" },
            {29, "Total ritase keluar dari G4" },
            {30, "Total ritase keluar dari G5" },
            {31, "Total ritase keluar dari G6" },
            {32, "Total ritase keluar dari G7" }
        };

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_chls
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

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
                dbContext.vw_chls
                .Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            Logger.Trace($"string values = {values}");

            chls record;

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(chls),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new chls();
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
                        // record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                        record.approved = false;
                        record.approved_by = "";

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
                                cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                var r = await cmd.ExecuteScalarAsync();
                                record.transaction_number = $"CHLS-{DateTime.Now:yyyyMMdd}-{r}";
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.ToString());
                                return BadRequest(ex.Message);
                            }
                        }
                        #endregion

                        dbContext.chls.Add(record);

                        await dbContext.SaveChangesAsync();

                        await tx.CommitAsync();
                    }
                    else
                    {
                        Logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            await AutoGenerateDetail(record.id);
            return Ok(record);
        }

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            Logger.Trace($"string values = {values}");

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    //bool isChlsApproved = false;
                    var record = dbContext.chls
                        .FirstOrDefault(o => o.id == key && o.organization_id == CurrentUserContext.OrganizationId);

                    bool? isApproved = record.approved;

                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            var e = new entity();
                            e.InjectFrom(record);

                            JsonConvert.PopulateObject(values, record);

                            #region Validation

                            //if (isApproved == true)
                            //{
                            //    if (isApproved != record.approved)
                            //    {
                            //        throw new Exception("Error CHLS already approved.");
                            //    }
                            //}
                            //else
                            //{
                            //    if (isApproved != record.approved)
                            //    {
                            //        record.approved_by = CurrentUserContext.AppUserId;
                            //        isCHLSApproved = true;
                            //    }
                            //}

                            #endregion

                            //record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            await dbContext.SaveChangesAsync();

                            //if (isCHLSApproved)
                            //{
                            //    #region Entry CPP Processing Transaction

                            //    var cppList = await dbContext.chls_cpp
                            //        .Where(x => x.header_id == record.id)
                            //        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            //        .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                            //        .ToListAsync();

                            //    if (cppList != null)
                            //    {
                            //        decimal? sumQuantity = 0;
                            //        foreach (var c in cppList)
                            //        {
                            //            sumQuantity = sumQuantity + c.quantity;
                            //        }
                            //        processing_transaction cpp;
                            //        if (await mcsContext.CanCreate(dbContext, nameof(processing_transaction),
                            //        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                            //        {
                            //            cpp = new processing_transaction();

                            //            cpp.id = Guid.NewGuid().ToString("N");
                            //            cpp.created_by = CurrentUserContext.AppUserId;
                            //            cpp.created_on = DateTime.Now;
                            //            cpp.modified_by = null;
                            //            cpp.modified_on = null;
                            //            cpp.is_active = true;
                            //            cpp.is_default = null;
                            //            cpp.is_locked = null;
                            //            cpp.entity_id = null;
                            //            cpp.owner_id = CurrentUserContext.AppUserId;
                            //            cpp.organization_id = CurrentUserContext.OrganizationId;
                            //            cpp.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                            //            cpp.process_flow_id = cppList[0].process_flow_id;
                            //            cpp.source_shift_id = record.shift_id;
                            //            cpp.source_location_id = cppList[0].source_location_id;
                            //            cpp.source_product_id = record.product_id;
                            //            cpp.loading_quantity = sumQuantity;
                            //            cpp.destination_uom_id = cppList[0].uom;
                            //            cpp.destination_location_id = cppList[0].destination_location_id;
                            //            cpp.equipment_id = cppList[0].equipment_id;
                            //            cpp.pic = record.approved_by;
                            //            cpp.business_unit_id = record.business_unit_id;
                            //        }
                            //    }

                            //    #endregion
                            ////}

                            await tx.CommitAsync();
                        }
                        else
                        {
                            Logger.Debug("User is not authorized.");
                            return Unauthorized();
                        }
                    }
                    else
                    {
                        Logger.Debug("Record is not found.");
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            return Ok();
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            Logger.Trace($"string key = {key}");

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.chls
                        .FirstOrDefault(o => o.id == key && o.organization_id == CurrentUserContext.OrganizationId);

                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.chls.Remove(record);

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                        }
                        else
                        {
                            Logger.Debug("User is not authorized.");
                            return Unauthorized();
                        }
                    }
                    else
                    {
                        Logger.Debug("Record is not found.");
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            return Ok();
        }

        [HttpGet("RecalculateCHLS")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> RecalculateCHLS(string Id)
        {
            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanUpdate(dbContext, Id, CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var cppRecord = await dbContext.chls_cpp
                            .Where(x => x.header_id == Id && x.organization_id == CurrentUserContext.OrganizationId).ToListAsync();
                        decimal? cppGross = 0;
                        decimal? cppNet = 0;
                        if (cppRecord != null)
                        {
                            foreach (var item in cppRecord)
                            {
                                cppGross += item.duration;
                                if (item.event_definition_category == "bb0df3ede2784a11b5936c057664eda8")
                                    cppNet += item.duration;
                            }
                        }
                        var olcRecord = await dbContext.chls_hauling
                            .Where(x => x.header_id == Id && x.organization_id == CurrentUserContext.OrganizationId).ToListAsync();
                        decimal? olcGross = 0;
                        decimal? olcNet = 0;
                        if (olcRecord != null)
                        {
                            foreach (var item in olcRecord)
                            {
                                olcGross += item.duration;
                                if (item.event_definition_category == "bb0df3ede2784a11b5936c057664eda8")
                                    olcNet += item.duration;
                            }
                        }
                        var record = await dbContext.chls
                            .Where(x => x.id == Id && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                        if (record != null)
                        {
                            record.gross_loading_cpp = cppGross;
                            record.net_loading_cpp = cppNet;
                            record.gross_loading_olc = olcGross;
                            record.net_loading_olc = olcNet;
                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                        }
                        else
                        {
                            Logger.Debug("Record is not found.");
                            return NotFound();
                        }
                    }
                    else
                    {
                        Logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
                return Ok();
            }
        }

        #region CPP Set

        [HttpGet("DataCppDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataCppDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.chls_cpp
                .Where(o => o.header_id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId)
                .OrderBy(o => o.start_time),
                loadOptions);
        }

        [HttpPost("InsertCppData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertCppData([FromForm] string values)
        {
            Logger.Trace($"string values = {values}");

            chls_cpp record;

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(chls_cpp),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new chls_cpp();
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
                        record.formula = record.id;
                        // start (fariz)
                        DateTime start = Convert.ToDateTime(record.start_time);
                        DateTime end = Convert.ToDateTime(record.end_time);
                        TimeSpan difference = end - start;
                        double TotalMinutes = difference.TotalMinutes;
                        record.duration = (decimal?)TotalMinutes; // jangan make "value.TimeOfDay", result nya suka jadi mines. make it efficient from this code if u can. to: maul 
                        // end (fariz)
                        var eventDefinitionCategoryDat = await dbContext.event_definition_category
                                .Where(x => x.id == record.event_definition_category)
                                .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefaultAsync();
                        if (eventDefinitionCategoryDat.event_definition_category_name.ToUpper() == "OPERATION")
                        {
                            var uom = await dbContext.uom
                            .Where(x => x.uom_symbol.ToUpper() == "MT")
                            .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            .FirstOrDefaultAsync();
                            if (uom == null) throw new Exception("No UOM 'MT' Found!");
                            record.uom = uom.id;
                            record.nett_loading_rate = CalculateNettLoadingRateCpp(record);
                        }
                        else
                        {
                            record.nett_loading_rate = null;
                            record.uom = null;
                        }
                        dbContext.chls_cpp.Add(record);
                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();
                    }
                    else
                    {
                        Logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            await RecalculateCHLS(record.id);
            return Ok(record);
        }

        [HttpPut("UpdateCppData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateCppData([FromForm] string key, [FromForm] string values)
        {
            Logger.Trace($"string values = {values}");

            chls_cpp record;

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.chls_cpp
                        .FirstOrDefault(o => o.id == key && o.organization_id == CurrentUserContext.OrganizationId);
                    var header = dbContext.chls
                       .FirstOrDefault(o => o.id == record.header_id && o.organization_id == CurrentUserContext.OrganizationId);
                    if (header.approved == true)
                    {
                        return BadRequest("You Cannot Update This Data, Please Unapprove the Header Data First");
                    }
                    else
                    {
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

                                // start (fariz)
                                DateTime start = Convert.ToDateTime(record.start_time);
                                DateTime end = Convert.ToDateTime(record.end_time);
                                TimeSpan difference = end - start;
                                double TotalMinutes = difference.TotalMinutes;
                                record.duration = (decimal?)TotalMinutes; // jangan make "value.TimeOfDay", result nya suka jadi mines. make it efficient from this code if u can. to: maul 
                                // end (fariz)
                                var eventDefinitionCategoryDat = await dbContext.event_definition_category
                                        .Where(x => x.id == record.event_definition_category)
                                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                                        .FirstOrDefaultAsync();
                                if (eventDefinitionCategoryDat.event_definition_category_name.ToUpper() == "OPERATION")
                                {
                                    var uom = await dbContext.uom
                                    .Where(x => x.uom_symbol.ToUpper() == "MT")
                                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                                    .FirstOrDefaultAsync();
                                    if (uom == null) throw new Exception("No UOM 'MT' Found!");
                                    record.uom = uom.id;
                                    record.nett_loading_rate = CalculateNettLoadingRateCpp(record);
                                }
                                else
                                {
                                    record.nett_loading_rate = null;
                                    record.uom = null;
                                }
                                await dbContext.SaveChangesAsync();
                                await tx.CommitAsync();
                            }
                            else
                            {
                                Logger.Debug("User is not authorized.");
                                return Unauthorized();
                            }
                        }
                        else
                        {
                            Logger.Debug("Record is not found.");
                            return NotFound();
                        }
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            await RecalculateCHLS(record.id);
            return Ok();
        }

        [HttpDelete("DeleteCppData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteCppData([FromForm] string key)
        {
            Logger.Trace($"string key = {key}");

            chls_cpp record;

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.chls_cpp
                        .FirstOrDefault(o => o.id == key && o.organization_id == CurrentUserContext.OrganizationId);
                    var header = dbContext.chls
                       .FirstOrDefault(o => o.id == record.header_id && o.organization_id == CurrentUserContext.OrganizationId);
                    if (header.approved == true)
                    {
                        return BadRequest("You Cannot Delete This Data, Please Unapprove the Header Data First");
                    }
                    else
                    {
                        if (record != null)
                        {
                            if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                                || CurrentUserContext.IsSysAdmin)
                            {
                                dbContext.chls_cpp.Remove(record);

                                await dbContext.SaveChangesAsync();
                                await tx.CommitAsync();
                            }
                            else
                            {
                                Logger.Debug("User is not authorized.");
                                return Unauthorized();
                            }
                        }
                        else
                        {
                            Logger.Debug("Record is not found.");
                            return NotFound();
                        }
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            await RecalculateCHLS(record.id);
            return Ok();
        }

        [HttpGet("GetCHLS/{Id}")]
        public object GetCHLS(string Id)
        {
            try
            {
                var result = new chls();

                result = dbContext.chls.Where(x => x.id == Id).FirstOrDefault();

                if (result == null)
                {
                    result = new chls();
                    result.id = Id;
                }

                return result;
            }
            catch (Exception ex)
            {
                //logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("ApproveUnapprove")]    //**** tanpa trigger trf_sales_invoice_approval_sync di table sales_invoice_approval
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ApproveUnapprove([FromForm] string key, [FromForm] string values)
        {
            dynamic result;
            bool? isApproved = null;
            bool? isUnapproved = null;


            var recCHLS = dbContext.chls
                            .Where(o => o.id == key)
                            .FirstOrDefault();
            var recCHLSCPP = await dbContext.chls_cpp
                            .Where(o => o.header_id == key && o.event_definition_category == "bb0df3ede2784a11b5936c057664eda8")
                            .ToListAsync();
            var recCHLSHauling = await dbContext.chls_hauling
                            .Where(o => o.header_id == key && o.event_definition_category == "bb0df3ede2784a11b5936c057664eda8")
                            .ToListAsync();


            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var PT = dbContext.processing_transaction
                   .Where(o => o.chls_id == key)
                   .FirstOrDefault();
                    var CT = dbContext.coal_transfer
                   .Where(o => o.chls_id == key)
                   .FirstOrDefault();
                    var RH = dbContext.rehandling_transaction
                   .Where(o => o.chls_id == key)
                   .FirstOrDefault();
                    bool? checkPT = null; bool? checkCT = null; bool? checkRH = null;
                    if (PT != null)
                    {
                        dbContext.processing_transaction.Remove(PT);
                        await dbContext.SaveChangesAsync();
                        checkPT = true;
                    }
                    if (CT != null)
                    {
                        dbContext.coal_transfer.Remove(CT);
                        await dbContext.SaveChangesAsync();
                        checkCT = true;
                    }
                    if (RH != null)
                    {
                        dbContext.rehandling_transaction.Remove(RH);
                        await dbContext.SaveChangesAsync();
                        checkRH = true;
                    }
                    if (checkPT == true || PT == null && checkCT == true || CT == null && checkRH == true || RH == null && recCHLS != null)
                    {
                        /*if (await mcsContext.CanUpdate(dbContext, recSILS.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                        {*/

                        JsonConvert.PopulateObject(values, recCHLS);

                        recCHLS.modified_by = CurrentUserContext.AppUserId;
                        recCHLS.modified_on = System.DateTime.Now;
                        if (recCHLS.approved == true)
                        {
                            recCHLS.approved = false;
                            recCHLS.disapprove_by_id = CurrentUserContext.AppUserId;
                            recCHLS.product_id = recCHLS.product_id;
                            recCHLS.operator_id = recCHLS.operator_id;
                            recCHLS.foreman_id = recCHLS.foreman_id;
                            isApproved = false;
                            isUnapproved = true;
                            result = recCHLS;
                        }
                        else
                        {
                            recCHLS.approved = true;
                            recCHLS.approved_by = CurrentUserContext.AppUserId;
                            recCHLS.product_id = recCHLS.product_id;
                            recCHLS.operator_id = recCHLS.operator_id;
                            recCHLS.foreman_id = recCHLS.foreman_id;
                            isApproved = true;
                            result = recCHLS;
                        }
                        //}
                        await dbContext.SaveChangesAsync();
                        result = recCHLS;
                    }
                    //if (await mcsContext.CanCreate(dbContext, nameof(chls),
                    //    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    //{
                    if (isApproved == true || isUnapproved == true)
                    {
                        //approve data in CPP tab
                        foreach (var detail in recCHLSCPP)
                        {
                            var type = dbContext.process_flow
                                .Where(o => o.id == detail.process_flow_id)
                                .FirstOrDefault();
                            if (type.process_flow_category == "Coal Produce")
                            {

                                detail.modified_by = CurrentUserContext.AppUserId;
                                detail.modified_on = DateTime.Now;
                                if (detail.approved == true)
                                {
                                    detail.approved = false;
                                    //detail.approved_by = CurrentUserContext.AppUserId;
                                }
                                else
                                {
                                    detail.approved = true;
                                    //detail.approved_by = CurrentUserContext.AppUserId;
                                }
                                await dbContext.SaveChangesAsync();
                            }
                        }
                        //approve data in tab olc/transfer
                        foreach (var detail in recCHLSHauling)
                        {
                            var type = dbContext.process_flow
                            .Where(o => o.id == detail.process_flow_id)
                            .FirstOrDefault();
                            if (type.process_flow_category == "Coal Transfer")
                            {
                                detail.modified_by = CurrentUserContext.AppUserId;
                                detail.modified_on = DateTime.Now;
                                if (detail.approved == true)
                                {
                                    detail.approved = false;
                                    //detail.approved_by = CurrentUserContext.AppUserId;
                                }
                                else
                                {
                                    detail.approved = true;
                                    //detail.approved_by = CurrentUserContext.AppUserId;
                                }
                                await dbContext.SaveChangesAsync();
                            }
                            else if (type.process_flow_category == "Rehandling")
                            {
                                detail.modified_by = CurrentUserContext.AppUserId;
                                detail.modified_on = DateTime.Now;
                                if (detail.approved == true)
                                {
                                    detail.approved = false;
                                    //detail.approved_by = CurrentUserContext.AppUserId;
                                }
                                else
                                {
                                    detail.approved = true;
                                    //detail.approved_by = CurrentUserContext.AppUserId;
                                }
                                await dbContext.SaveChangesAsync();
                            }
                        }
                    }
                    //}
                    result = recCHLS;
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

        [HttpGet("MiningStockpileLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> MiningStockpileLookup(DataSourceLoadOptions loadOptions)
        {
            var miningLookup = dbContext.mine_location
                .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                .Select(x => new { Value = x.id, Text = x.stock_location_name })
                .OrderBy(x => x.Text);
            var stockpileLookup = dbContext.stockpile_location
                .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                .Select(x => new { Value = x.id, Text = x.stock_location_name })
                .OrderBy(x => x.Text);
            var lookup = miningLookup.Union(stockpileLookup).Distinct();
            return await DataSourceLoader.LoadAsync(lookup, loadOptions);
        }

        private decimal? CalculateNettLoadingRateCpp(chls_cpp record)
        {
            var timeHours = record.duration / 60;
            var nett_rate = record.quantity / timeHours;
            return nett_rate;
        }

        #endregion

        #region Hauling Set

        [HttpGet("DataHaulingDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataHaulingDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.chls_hauling
                .Where(o => o.header_id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId)
                .OrderBy(o => o.start_time),
                loadOptions);
        }

        [HttpPost("InsertHaulingData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertHaulingData([FromForm] string values)
        {
            Logger.Trace($"string values = {values}");

            chls_hauling record;

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(chls_hauling),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new chls_hauling();
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
                        record.formula = record.id;

                        // start (fariz)
                        DateTime start = Convert.ToDateTime(record.start_time);
                        DateTime end = Convert.ToDateTime(record.end_time);
                        TimeSpan difference = end - start;
                        double TotalMinutes = difference.TotalMinutes;
                        record.duration = (decimal?)TotalMinutes; // jangan make "value.TimeOfDay", result nya suka jadi mines. make it efficient from this code if u can. to: maul 
                        // end (fariz)
                        var eventDefinitionCategoryDat = await dbContext.event_definition_category
                                        .Where(x => x.id == record.event_definition_category)
                                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                                        .FirstOrDefaultAsync();
                        if (eventDefinitionCategoryDat.event_definition_category_name.ToUpper() == "OPERATION")
                        {
                            var uom = await dbContext.uom
                            .Where(x => x.uom_symbol.ToUpper() == "MT")
                            .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            .FirstOrDefaultAsync();
                            if (uom == null) throw new Exception("No UOM 'MT' Found!");
                            record.uom = uom.id;
                            record.nett_loading_rate = CalculateNettLoadingRateOLC(record);
                        }
                        else
                        {
                            record.nett_loading_rate = null;
                            record.uom = null;
                        }
                        dbContext.chls_hauling.Add(record);
                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();
                        await RecalculateCHLS(record.id);
                    }
                    else
                    {
                        Logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            return Ok(record);
        }

        [HttpPut("UpdateHaulingData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateHaulingData([FromForm] string key, [FromForm] string values)
        {
            Logger.Trace($"string values = {values}");

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.chls_hauling
                        .FirstOrDefault(o => o.id == key && o.organization_id == CurrentUserContext.OrganizationId);
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

                            // start (fariz)
                            DateTime start = Convert.ToDateTime(record.start_time);
                            DateTime end = Convert.ToDateTime(record.end_time);
                            TimeSpan difference = end - start;
                            double TotalMinutes = difference.TotalMinutes;
                            record.duration = (decimal?)TotalMinutes; // jangan make "value.TimeOfDay", result nya suka jadi mines. make it efficient from this code if u can. to: maul 
                            // end (fariz)
                            var eventDefinitionCategoryDat = await dbContext.event_definition_category
                                                                    .Where(x => x.id == record.event_definition_category)
                                                                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                                                                    .FirstOrDefaultAsync();
                            if (eventDefinitionCategoryDat.event_definition_category_name.ToUpper() == "OPERATION")
                            {
                                var uom = await dbContext.uom
                                .Where(x => x.uom_symbol.ToUpper() == "MT")
                                .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefaultAsync();
                                if (uom == null) throw new Exception("No UOM 'MT' Found!");
                                record.uom = uom.id;
                                record.nett_loading_rate = CalculateNettLoadingRateOLC(record);
                            }
                            else
                            {
                                record.nett_loading_rate = null;
                                record.uom = null;
                            }
                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            await RecalculateCHLS(record.id);
                        }
                        else
                        {
                            Logger.Debug("User is not authorized.");
                            return Unauthorized();
                        }
                    }
                    else
                    {
                        Logger.Debug("Record is not found.");
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            return Ok();
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
                    .Select(o => new { Value = o.id, Text = o.process_flow_name, Index = 0, search = o.process_flow_name.ToLower() + o.process_flow_name.ToUpper() })
                       .OrderBy(o => o.Text);
                var lookup2 = dbContext.event_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.event_category_name, Index = 1, search = o.event_category_name.ToLower() + o.event_category_name.ToUpper() })
                       .OrderBy(o => o.Text);
                var lookup = lookup1.Union(lookup2);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                //  logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("StockpileLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> StockpileLocationIdLookup(string ProcessFlowId,
            DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup2 = dbContext.stockpile_location.FromSqlRaw("SELECT r.id as id, r.business_unit_id as business_unit_id, r.stock_location_name as stock_location_name,COALESCE(child_6,child_5,child_4,child_3,child_2,child_1) AS stockpile_location_code FROM vw_stockpile_location r LEFT JOIN vw_business_area_breakdown_structure b ON b.id = r.business_area_id")
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = o.stock_location_name + ">" + o.stockpile_location_code,
                            urutan = 2,
                            search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper() + o.stockpile_location_code.ToUpper() + o.stockpile_location_code.ToLower()
                        }).OrderBy(o => o.text);

                //var lookup = lookup1.Union(lookup2).OrderBy(o => o.urutan);

                return await DataSourceLoader.LoadAsync(lookup2, loadOptions);

            }
            catch (Exception ex)
            {
                // logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("AllDestinationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AllDestinationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup1 = dbContext.port_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.stock_location_name, Index = 0, search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper() })
                       .OrderBy(o => o.Text);
                var lookup2 = dbContext.equipment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.equipment_name, Index = 0, search = o.equipment_name.ToLower() + o.equipment_name.ToUpper() })
                       .OrderBy(o => o.Text);
                var lookup3 = dbContext.barge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name, Index = 0, search = o.vehicle_name.ToLower() + o.vehicle_name.ToUpper() })
                       .OrderBy(o => o.Text);
                var lookup = lookup1.Union(lookup2).Union(lookup3);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                //  logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("FilteredDestinationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> FilteredDestinationIdLookup(DataSourceLoadOptions loadOptions, string id)
        {
            try
            {
                var context = dbContext.event_definition_category.Where(o => o.id == id).FirstOrDefault();

                if (context != null && context.event_definition_category_name == "Ready" || context.event_definition_category_name == "Shipping Delay")
                {
                    var lookup = dbContext.barge
                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                   .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                   .Select(o => new { Value = o.id, Text = o.vehicle_name, Index = 0, search = o.vehicle_name.ToLower() + o.vehicle_name.ToUpper() })
                      .OrderBy(o => o.Text);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup1 = dbContext.port_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.stock_location_name, Index = 0, search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper() })
                           .OrderBy(o => o.Text);
                    var lookup2 = dbContext.equipment
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.equipment_name, Index = 0, search = o.equipment_name.ToLower() + o.equipment_name.ToUpper() })
                           .OrderBy(o => o.Text);
                    var lookup = lookup1.Union(lookup2);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
            catch (Exception ex)
            {
                //  logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("TypeIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> TypeIdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = dbContext.event_definition_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    // .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.id == Id).FirstOrDefault();
                if (data != null && data.event_definition_category_name.ToUpper() == "OPERATION")
                {
                    var lookupB = dbContext.process_flow
                                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                        .Select(o => new { Value = o.id, Text = o.process_flow_name, Index = 0, search = o.process_flow_name.ToLower() + o.process_flow_name.ToUpper() })
                                           .OrderBy(o => o.Text);
                    return await DataSourceLoader.LoadAsync(lookupB, loadOptions);
                }
                else
                {
                    var lookupA = dbContext.event_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    // .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.event_definition_category_id == Id)
                    .Select(o => new { Value = o.id, Text = o.event_category_name, search = o.event_category_name.ToLower() + o.event_category_name.ToUpper() })
                    .OrderBy(o => o.Text);
                    return await DataSourceLoader.LoadAsync(lookupA, loadOptions);
                }
            }
            catch (Exception ex)
            {
                // logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DestinationLocationOLCIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DestinationLocationOLCIdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {

                var lookup1 = dbContext.vw_stockpile_location
                   .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.item_name == "Port Product/Crsuhed")
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                   .Select(o =>
                       new
                       {
                           value = o.id,
                           text = o.stock_location_name,
                           urutan = 2,
                           search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper()
                       }).OrderBy(o => o.text);


                return await DataSourceLoader.LoadAsync(lookup1, loadOptions);

            }
            catch (Exception ex)
            {
                // logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DestinationLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DestinationLocationIdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {

                var lookup1 = dbContext.vw_port_location
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

                return await DataSourceLoader.LoadAsync(lookup1, loadOptions);

                #region Commented Code
                /*if (string.IsNullOrEmpty(ProcessFlowId))
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
                                urutan = 2
                            });

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

                        , CurrentUserContext.OrganizationId, ProcessFlowId)
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                //text = o.business_area_name + " > " + o.stock_location_name,
                                text = (o.business_area_name != "" ? o.business_area_name + " > " : "") + o.stock_location_name,
                                urutan = 1
                            })
                        .OrderBy(o => o.text);
                    return await DataSourceLoader.LoadAsync(lookup1, loadOptions);
                }*/
                #endregion
            }
            catch (Exception ex)
            {
                // logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("DeleteHaulingData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteHaulingData([FromForm] string key)
        {
            Logger.Trace($"string key = {key}");

            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.chls_hauling
                        .FirstOrDefault(o => o.id == key && o.organization_id == CurrentUserContext.OrganizationId);
                    var header = dbContext.chls
                        .FirstOrDefault(o => o.id == record.header_id && o.organization_id == CurrentUserContext.OrganizationId);
                    if (header.approved == true)
                    {
                        return BadRequest("You Cannot Delete This Data, Please Unapprove the Header Data First");
                    }
                    else
                    {
                        if (record != null)
                        {
                            if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                                || CurrentUserContext.IsSysAdmin)
                            {
                                dbContext.chls_hauling.Remove(record);

                                await dbContext.SaveChangesAsync();
                                await tx.CommitAsync();
                                await RecalculateCHLS(record.id);
                            }
                            else
                            {
                                Logger.Debug("User is not authorized.");
                                return Unauthorized();
                            }
                        }
                        else
                        {
                            Logger.Debug("Record is not found.");
                            return NotFound();
                        }
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            return Ok();
        }

        [HttpGet("EmployeeForemanIdLookup")]
        public async Task<object> EmployeeForemanIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext._operator
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.is_foreman == true)
                    .Select(o => new { Value = o.id, Text = o.operator_name, search = o.operator_name.ToLower() + o.operator_name.ToUpper() })
                    .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("EquipmentIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> EquipmentIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var filter = await dbContext.equipment_type
                    .Where(x => x.equipment_type_name.ToUpper().Contains("CONVEYOR") || x.equipment_type_name.ToUpper().Contains("CRUSHER")
                    || x.equipment_type_name.ToUpper().Contains("HOPPER") || x.equipment_type_name.ToUpper().Contains("TRANSFER CUTE")
                    || x.equipment_type_name.ToUpper().Contains("SAMPLER")).Select(o => o.id).ToListAsync();

                var lookup = dbContext.equipment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => filter.Contains(o.equipment_type_id))
                    .Select(o => new { Value = o.id, Text = o.equipment_code, search = o.equipment_code.ToLower() + o.equipment_code.ToUpper() })
                       .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        private decimal? CalculateNettLoadingRateOLC(chls_hauling record)
        {
            var timeHours = record.duration / 60;
            var nett_rate = record.quantity / timeHours;
            return nett_rate;
        }

        #endregion

        #region Additional Info Set

        [HttpGet("DataAdditionalDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataAdditionalDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_chls_additional_info
                .Where(o => o.chls_id == Id)
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .OrderBy(o => o.sort_number),
                loadOptions);
        }

        [HttpGet("AutoGenerateDetail")]
        public async Task<IActionResult> AutoGenerateDetail(string chls_id)
        {
            Logger.Trace($"string values = {chls_id}");
            chls_additional_info record = null;
            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(chls_additional_info),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        List<uom> uom = await dbContext.uom
                               //.Where(x => x.uom_symbol.ToUpper() == "LT")
                               .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                               .ToListAsync();

                        #region Production Today

                        var masterList1 = await dbContext.master_list
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.item_group == "production-today").ToListAsync();

                        foreach (var items in masterList1)
                        {
                            record = GenerateEntity();
                            record.chls_id = chls_id;
                            record.group_id = items.item_group;
                            record.description_id = items.id;
                            record.start_count = 0;
                            record.stop_count = 0;
                            record.total_count = 0;
                            record.uom_id = null;
                            dbContext.chls_additional_info.Add(record);
                            await dbContext.SaveChangesAsync();
                        }

                        #endregion

                        #region Daily Stock CPP

                        var masterList2 = await dbContext.master_list
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.item_group == "daily-stock-cpp").ToListAsync();

                        foreach (var items in masterList2)
                        {
                            record = GenerateEntity();
                            record.chls_id = chls_id;
                            record.group_id = items.item_group;
                            record.description_id = items.id;
                            record.start_count = 0;
                            record.stop_count = 0;
                            record.total_count = 0;
                            record.uom_id = null;
                            dbContext.chls_additional_info.Add(record);
                            await dbContext.SaveChangesAsync();
                        }

                        #endregion

                        #region Use of Fuel Genset 4

                        var masterList3 = await dbContext.master_list
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.item_group == "use-of-fuel-genset-4").ToListAsync();

                        foreach (var items in masterList3)
                        {
                            record = GenerateEntity();
                            record.chls_id = chls_id;
                            record.group_id = items.item_group;
                            record.description_id = items.id;
                            record.start_count = 0;
                            record.stop_count = 0;
                            record.total_count = 0;
                            record.uom_id = null;
                            dbContext.chls_additional_info.Add(record);
                            await dbContext.SaveChangesAsync();
                        }

                        #endregion

                        #region Use of Fuel Genset 5

                        var masterList4 = await dbContext.master_list
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.item_group == "use-of-fuel-genset-5").ToListAsync();

                        foreach (var items in masterList4)
                        {
                            record = GenerateEntity();
                            record.chls_id = chls_id;
                            record.group_id = items.item_group;
                            record.description_id = items.id;
                            record.start_count = 0;
                            record.stop_count = 0;
                            record.total_count = 0;
                            record.uom_id = null;
                            dbContext.chls_additional_info.Add(record);
                            await dbContext.SaveChangesAsync();
                        }

                        #endregion

                        #region PLN CPP

                        var masterList5 = await dbContext.master_list
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.item_group == "pln-cpp").ToListAsync();

                        foreach (var items in masterList5)
                        {
                            record = GenerateEntity();
                            record.chls_id = chls_id;
                            record.group_id = items.item_group;
                            record.description_id = items.id;
                            record.start_count = 0;
                            record.stop_count = 0;
                            record.total_count = 0;
                            record.uom_id = null;
                            dbContext.chls_additional_info.Add(record);
                            await dbContext.SaveChangesAsync();
                        }

                        #endregion

                        await tx.CommitAsync();
                    }
                    else
                    {
                        Logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            return Ok(record);
        }

        private chls_additional_info GenerateEntity()
        {
            var record = new chls_additional_info();
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
            return record;
        }

        [HttpPut("UpdateDataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateDataDetail([FromForm] string key, [FromForm] string values)
        {
            Logger.Trace($"string values = {values}");
            await using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.chls_additional_info
                        .FirstOrDefault(o => o.id == key && o.organization_id == CurrentUserContext.OrganizationId);
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            JsonConvert.PopulateObject(values, record);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;
                            record.total_count = record.stop_count - record.start_count;
                            if (record.total_count < 0)
                            {
                                record.total_count = record.total_count * -1;
                            }

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                        }
                        else
                        {
                            Logger.Debug("User is not authorized.");
                            return Unauthorized();
                        }
                    }
                    else
                    {
                        Logger.Debug("Record is not found.");
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            return Ok();
        }

        [HttpGet("GroupLookup")]
        public object GroupLookup(DataSourceLoadOptions options)
        {
            var lookup = dicGroup.Select(x => new { value = x.Key, text = x.Value });
            return DataSourceLoader.Load(lookup, options);
        }

        [HttpGet("DescriptionLookup")]
        public object DescriptionLookup(DataSourceLoadOptions options)
        {
            var lookup = dicDescription.Select(x => new { value = x.Key, text = x.Value });
            return DataSourceLoader.Load(lookup, options);
        }

        #endregion
    }
}
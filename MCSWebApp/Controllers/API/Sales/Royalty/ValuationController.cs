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

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/Royalty/[controller]")]
    [ApiController]
    public class ValuationController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ValuationController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        //[HttpGet("DataGrid")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> DataGrid(string royaltyId, DataSourceLoadOptions loadOptions)
        //{
        //    try
        //    {
        //        return await DataSourceLoader.LoadAsync(dbContext.vw_royalty_valuation
        //            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //            .Where(o => o.id == royaltyId),
        //                loadOptions);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet("DataDetail")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        //{
        //    return await DataSourceLoader.LoadAsync(
        //        dbContext.vw_royalty_valuation.Where(o => o.id == Id),
        //        loadOptions);
        //}

   //     [HttpPost("InsertData")]
   //     [ApiExplorerSettings(IgnoreApi = true)]
   //     public async Task<IActionResult> InsertData([FromForm] string values)
   //     {
   //         logger.Trace($"string values = {values}");

   //         try
   //         {
			//	if (await mcsContext.CanCreate(dbContext, nameof(royalty_valuation),
			//		CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
			//	{
   //                 var record = new royalty_valuation();
   //                 JsonConvert.PopulateObject(values, record);

   //                 record.id = Guid.NewGuid().ToString("N");
   //                 record.created_by = CurrentUserContext.AppUserId;
   //                 record.created_on = DateTime.Now;
   //                 record.modified_by = null;
   //                 record.modified_on = null;
   //                 record.is_active = true;
   //                 record.is_default = null;
   //                 record.is_locked = null;
   //                 record.entity_id = null;
   //                 record.owner_id = CurrentUserContext.AppUserId;
   //                 record.organization_id = CurrentUserContext.OrganizationId;
			//		record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

   //                 dbContext.royalty_valuation.Add(record);
   //                 await dbContext.SaveChangesAsync();

   //                 return Ok(record);
			//	}
			//	else
			//	{
			//		return BadRequest("User is not authorized.");
			//	}
   //         }
			//catch (Exception ex)
			//{
			//	logger.Error(ex.InnerException ?? ex);
   //             return BadRequest(ex.InnerException?.Message ?? ex.Message);
			//}
   //     }

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.royalty_valuation
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

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

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

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var record = dbContext.royalty_valuation
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.royalty_valuation.Remove(record);
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

        [HttpDelete("DeleteById/{Id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Debug($"string Id = {Id}");

            try
            {
                var record = dbContext.royalty_valuation
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.royalty_valuation.Remove(record);
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

        [HttpGet("Detail/{Id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.vw_royalty_valuation
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                if (record != null)
                {
                    var royaltyHeader = await dbContext.vw_royalty
                        .Where(o => o.id == Id).FirstOrDefaultAsync();
                    var deliveryTerm = royaltyHeader?.delivery_term ?? "";

                    var Pricing = await dbContext.vw_royalty_pricing
                        .Where(o => o.id == Id).FirstOrDefaultAsync();
                    var hba0 = Pricing?.hba_0 ?? 0;
                    if (hba0 > 100) record.dhpb = 0.28;
                    else if (hba0 > 90) record.dhpb = 0.25;
                    else if (hba0 > 80) record.dhpb = 0.23;
                    else if (hba0 > 70) record.dhpb = 0.17;
                    else record.dhpb = 0.14;

                    var Cost = await dbContext.vw_royalty_cost
                        .Where(o => o.id == Id).FirstOrDefaultAsync();

                    if ("fob barge, cif barge, cif truck, fob truck".Contains(deliveryTerm.ToLower()))
                    {
                        var BPR = Math.Max((decimal)(Pricing?.fob_price??0), (decimal)(Pricing?.hpb_barge??0));
                        record.base_price_royalty = BPR;
                    }
                    else
                    {
                        var BPR = Math.Max((decimal)(Pricing?.fob_price ?? 0), (decimal)(Pricing?.hpb_vessel ?? 0));
                        record.base_price_royalty = BPR - Cost.total_join_cost??0;
                    }

                    record.bmn = 0.021;

                    var RQQ = await dbContext.vw_royalty_quantity_quality
                        .Where(o => o.royalty_id == Id && o.analyte_symbol.Replace(" ", "").ToLower() == "gcv(arb)")
                        .FirstOrDefaultAsync();
                    var gcv = RQQ?.analyte_value ?? 0;

                    if (royaltyHeader.destination_name == "Kelistrikan")
                    {
                        if (gcv <= 4200) record.royalty = 0.06;
                        else if (gcv <= 5199) record.royalty = 0.085;
                        else record.royalty = 0.115;
                    }
                    else
                    {
                        if (Pricing.hba_0 < 70)
                        {
                            if (gcv <= 4200) record.royalty = 0.05;
                            else if (gcv <= 5199) record.royalty = 0.07;
                            else record.royalty = 0.095;
                        }
                        else if (Pricing.hba_0 < 90)
                        {
                            if (gcv <= 4200) record.royalty = 0.06;
                            else if (gcv <= 5199) record.royalty = 0.085;
                            else record.royalty = 0.115;
                        }
                        else if (Pricing.hba_0 >= 90)
                        {
                            if (gcv <= 4200) record.royalty = 0.08;
                            else if (gcv <= 5199) record.royalty = 0.105;
                            else record.royalty = 0.135;
                        }
                    }

                    var royaltyAkhir = await dbContext.vw_royalty.Where(o => o.despatch_order_id == record.despatch_order_id
                            && o.status_code == "AKH").FirstOrDefaultAsync();
                    var royaltyAkhirId = royaltyAkhir?.id??"";
                    if (royaltyAkhirId != "")
                    {
                        var paymentAkhir = await dbContext.vw_royalty_payment
                            .Where(o => o.id == royaltyAkhirId).FirstOrDefaultAsync();
                        record.royalty_awal = paymentAkhir.royalty_value ?? 0;
                        record.bmn_awal = paymentAkhir.bmn_value ?? 0;
                        record.pht_awal = paymentAkhir.pht_value ?? 0;
                        record.dhpb_final_awal = paymentAkhir.dhpb_final_value ?? 0;
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

        [HttpGet("Detail1/{Id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Detail1(string Id)
        {
            try
            {
                decimal currentExchange = 1;
                var record = await dbContext.vw_royalty_valuation
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                if (record != null)
                {
                    decimal exchangeRate = Convert.ToDecimal(record.currency_exchange_id);
                    if (exchangeRate > 0) currentExchange = exchangeRate;
                    var royaltyHeader = await dbContext.vw_royalty
                        .Where(o => o.id == Id).FirstOrDefaultAsync();
                    var deliveryTerm = royaltyHeader?.delivery_term ?? "";

                    var Pricing = await dbContext.vw_royalty_pricing
                        .Where(o => o.id == Id).FirstOrDefaultAsync();
                    var hba0 = Pricing?.hba_0 ?? 0;
                    if (hba0 > 100) record.dhpb = 0.28;
                    else if (hba0 > 90) record.dhpb = 0.25;
                    else if (hba0 > 80) record.dhpb = 0.23;
                    else if (hba0 > 70) record.dhpb = 0.17;
                    else record.dhpb = 0.14;
                    if (Pricing.hba_type.ToUpper() == "KELISTRIKAN" || Pricing.hba_type.ToUpper() == "SEMEN")
                        record.dhpb = 0.14;

                    var Cost = await dbContext.vw_royalty_cost
                        .Where(o => o.id == Id).FirstOrDefaultAsync();

                    //if ("fob barge, cif barge, cif truck, fob truck".Contains(deliveryTerm.ToLower()))
                    if ("fob barge".Contains(deliveryTerm.ToLower()))
                    {
                        var BPR = Math.Max((decimal)(Pricing?.fob_price ?? 0), (decimal)(Pricing?.hpb_barge ?? 0));
                        record.base_price_royalty = BPR;
                    }
                    else if ("fob vessel".Contains(deliveryTerm.ToLower()))
                    {
                        var BPR = Math.Max((decimal)(Pricing?.fob_price ?? 0), (decimal)(Pricing?.hpb_vessel ?? 0));
                        record.base_price_royalty = BPR - (Cost.total_join_cost == null ? 0 : Cost.total_join_cost) ?? 0;
                    }

                    record.bmn = 0.0021;

                    var RQQ = await dbContext.vw_royalty_quantity_quality
                        .Where(o => o.royalty_id == Id && o.analyte_symbol.Replace(" ", "").ToLower() == "gcv(arb)")
                        .FirstOrDefaultAsync();
                    var gcv = RQQ?.analyte_value ?? 0;

                    if (royaltyHeader.destination_name == "Kelistrikan")
                    {
                        if (gcv <= 4200) record.royalty = 0.06;
                        else if (gcv <= 5199) record.royalty = 0.085;
                        else record.royalty = 0.115;
                    }
                    else
                    {
                        if (Pricing.hba_0 < 70)
                        {
                            if (gcv <= 4200) record.royalty = 0.05;
                            else if (gcv <= 5199) record.royalty = 0.07;
                            else record.royalty = 0.095;
                        }
                        else if (Pricing.hba_0 < 90)
                        {
                            if (gcv <= 4200) record.royalty = 0.06;
                            else if (gcv <= 5199) record.royalty = 0.085;
                            else record.royalty = 0.115;
                        }
                        else if (Pricing.hba_0 >= 90)
                        {
                            if (gcv <= 4200) record.royalty = 0.08;
                            else if (gcv <= 5199) record.royalty = 0.105;
                            else record.royalty = 0.135;
                        }
                    }

                    var royaltyAkhir = await dbContext.vw_royalty.Where(o => o.despatch_order_id == record.despatch_order_id
                            && o.status_code == "AKH").FirstOrDefaultAsync();
                    var royaltyAkhirId = royaltyAkhir?.id ?? "";
                    if (royaltyAkhirId != "")
                    {
                        var paymentAkhir = await dbContext.vw_royalty_payment
                            .Where(o => o.id == royaltyAkhirId).FirstOrDefaultAsync();
                        record.royalty_awal = paymentAkhir.royalty_value ?? 0;
                        record.bmn_awal = paymentAkhir.bmn_value ?? 0;
                        record.pht_awal = paymentAkhir.pht_value ?? 0;
                        record.dhpb_final_awal = paymentAkhir.dhpb_final_value ?? 0;
                    }

                    // Exchange into Current Exhcange.
                    record.base_price_royalty *= currentExchange;
                }
                return Ok(record);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        //[HttpGet("Recalculate/{Id}")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> Recalculate(string Id, DataSourceLoadOptions loadOptions)
        //{
        //    try
        //    {
        //        var record = await dbContext.vw_royalty_valuation_recalc
        //            .Where(o => o.id == Id).FirstOrDefaultAsync();
        //        return Ok(record);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.InnerException?.Message ?? ex.Message);
        //    }
        //}

        [HttpPost("Recalculate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> Recalculate([FromForm] string key, [FromForm] string values)
        {
            try
            {
                string Id = key;

                var record = await dbContext.vw_royalty_valuation
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                o.id == Id)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    var e = new entity();
                    e.InjectFrom(record);

                    JsonConvert.PopulateObject(values, record);

                    decimal volumeLoading = record.volume_loading ?? 0;
                    decimal BPR = record?.base_price_royalty ?? 0;
                    decimal Royalty = Convert.ToDecimal(record.royalty ?? 0);
                    decimal BMN = Convert.ToDecimal(record.bmn ?? 0);
                    decimal PHT = Convert.ToDecimal(record.pht ?? 0);
                    decimal DHPB = Convert.ToDecimal(record.dhpb ?? 0);

                    record.royalty_calc = BPR * volumeLoading * Royalty;
                    record.bmn_calc = BPR * volumeLoading * BMN;

                    //record.pht_calc = record.royalty_calc + record.bmn_calc + PHT;
                    record.pht_calc = BPR * volumeLoading * PHT;

                    record.dhpb_final_calc = record.royalty_calc + record.bmn_calc + record.pht_calc;

                    var royaltyHeader = await dbContext.vw_royalty
                        .Where(o => o.id == Id).FirstOrDefaultAsync();
                    if (royaltyHeader.status_code == "AWL")
                    {
                        record.royalty_awal = 0;
                        record.bmn_awal = 0;
                        record.pht_awal = 0;
                        record.dhpb_final_awal = 0;
                    }
                    else
                    {
                        var royaltyAkhir = await dbContext.vw_royalty.Where(o => o.despatch_order_id == record.despatch_order_id
                                && o.status_code == "AWL").FirstOrDefaultAsync();
                        var royaltyAwalId = royaltyAkhir.id;

                        var paymentAwal = await dbContext.vw_royalty_payment
                            .Where(o => o.id == royaltyAwalId).FirstOrDefaultAsync();
                        record.royalty_awal = paymentAwal.royalty_paid_off ?? 0;
                        record.bmn_awal = paymentAwal.bmn_paid_off ?? 0;
                        record.pht_awal = paymentAwal.pht_paid_off ?? 0;
                        record.dhpb_final_awal = paymentAwal.dhpb_paid_off ?? 0;
                    }

                    record.royalty_value = record.royalty_calc - record.royalty_awal;
                    record.bmn_value = record.bmn_calc - record.bmn_awal;
                    record.pht_value = record.pht_calc - record.pht_awal;
                    record.dhpb_final_value = record.dhpb_final_calc - record.dhpb_final_awal;
                }

                return Ok(record);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPost("SaveData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SaveData([FromForm] string key, [FromForm] string values, [FromForm] string values2, [FromForm] string values3)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    string Id = key;

                    var record = await dbContext.royalty_valuation
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.royalty_id == Id)
                        .FirstOrDefaultAsync();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, Id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            var e = new entity();
                            e.InjectFrom(record);

                            JsonConvert.PopulateObject(values, record);
                            JsonConvert.PopulateObject(values2, record);

                            record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok(record);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else
                    {
                        if (await mcsContext.CanCreate(dbContext, nameof(royalty_valuation),
                            CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                        {
                            var newRecord = new royalty_valuation();

                            JsonConvert.PopulateObject(values, newRecord);
                            JsonConvert.PopulateObject(values2, newRecord);

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

                            newRecord.royalty_id = key;

                            dbContext.royalty_valuation.Add(newRecord);

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok(newRecord);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
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
}

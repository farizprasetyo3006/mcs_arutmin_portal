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
using Microsoft.AspNetCore.Http;
using DataAccess.DTO;
using Omu.ValueInjecter;
using DataAccess.EFCore.Repository;
using Common;
using BusinessLogic;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Collections;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/Royalty/[controller]")]
    [ApiController]
    public class PricingController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;


        public PricingController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }
        private DateTime BlDate;
        private class HbaClass
        {
            public string Text = string.Empty;
            public string Value = string.Empty;
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(string royaltyId, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_royalty_pricing
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.id == royaltyId),
                        loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.royalty_pricing.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(royalty_pricing),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var record = new royalty_pricing();
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


                    dbContext.royalty_pricing.Add(record);
                    await dbContext.SaveChangesAsync();

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

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.royalty_pricing
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
                var record = dbContext.royalty_pricing
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.royalty_pricing.Remove(record);
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
                var record = dbContext.royalty_pricing
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.royalty_pricing.Remove(record);
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
                var record = await dbContext.vw_royalty_pricing
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                if (record != null)
                {
                    if (record.fob_price == 0 || record.fob_price == null)
                    {
                        var recCost = await dbContext.vw_royalty_cost
                            .Where(o => o.id == Id).FirstOrDefaultAsync();
                        record.freight_cost = recCost?.freight_cost ?? 0;
                    }
                    var recDOR = dbContext.despatch_order.Where(o => o.id == record.despatch_order_id).FirstOrDefault();
                    var recSI = dbContext.sales_invoice.Where(o => o.despatch_order_id == record.despatch_order_id).FirstOrDefault();
                    var FOBPrice = recSI?.unit_price ?? 0;
                    if (record.fob_price == null)
                    {
                        if (record.status_code == "AWL")    //*** status = AWAL
                        {
                            var salesContractTermId = recDOR.contract_term_id;
                            var recSCQP = dbContext.vw_sales_contract_quotation_price.Where(o => o.sales_contract_term_id == salesContractTermId).FirstOrDefault();
                            var quotationPeriod = recSCQP.quotation_period ?? "";
                            if (quotationPeriod == "4LI1LD")
                            {
                                decimal price = 0;
                                decimal avgPrice = 0;
                                var recPrice = dbContext.vw_price_index_history.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                             && o.price_index_code == "HBA" && o.index_date <= Convert.ToDateTime(record.bl_date).Date)
                                    .OrderByDescending(o => o.index_date)
                                    .Take(4);
                                foreach (vw_price_index_history item in recPrice)
                                {
                                    price += Convert.ToDecimal(item.index_value);
                                }
                                avgPrice = price / 4;
                                record.fob_price = avgPrice;
                            }
                            else
                            {
                                decimal price = 0;
                                decimal avgPrice = 0;
                                record.fob_price = recSCQP.price_value;
                            }

                        }   //*** end status=AWAL
                        else
                        {
                            //*** status=AKHIR
                            record.fob_price = FOBPrice;
                        }
                    }
                    record.total_selling_price = record.fob_price + record.freight_cost;
                    record.total_amount = record.total_selling_price * record.volume_loading;

                    DateTime tanggal;
                    if (record.status_code == "AWL")
                        tanggal = Convert.ToDateTime(record.royalty_date).Date;
                    else
                        tanggal = Convert.ToDateTime(record.bl_date).Date;

                    var recHBA = dbContext.vw_price_index_history
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.price_index_code == "HBA" && o.index_date <= tanggal)
                        .OrderByDescending(o => o.index_date).FirstOrDefault();
                    record.hba_0 = recHBA?.index_value ?? 0;

                    //var recPIH = dbContext.vw_price_index_history.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    //    && o.price_index_code == "HBAII" && o.index_date <= Convert.ToDateTime(record.bl_date).Date).FirstOrDefault();
                    //record.hba_type = "HBAII";
                    //record.hba_value = recPIH?.index_value ?? 0;
                    if (record.hba_value == null)
                    {
                        if (record.destination_code == "LSTRK")
                        {
                            record.hba_type = "Kelistrikan";
                            record.hba_value = 70;
                        }
                        else if (record.destination_code == "SMN")
                        {
                            record.hba_type = "Semen";
                            record.hba_value = 90;
                        }
                        else
                        {
                            var recGCV = dbContext.vw_royalty_quantity_quality
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.royalty_id == Id
                                    && o.analyte_symbol.Replace(" ", "").ToLower() == "gcv(arb)").FirstOrDefault();
                            var gcvValue = recGCV?.analyte_value ?? 0;
                            if (gcvValue > 5300)
                            {
                                var recPIH = dbContext.vw_price_index_history
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                        && o.price_index_code == "HBA" && o.index_date <= Convert.ToDateTime(record.bl_date).Date)
                                    .OrderByDescending(o => o.index_date).FirstOrDefault();
                                record.hba_type = "HBA";
                                record.hba_value = recPIH?.index_value ?? 0;
                            }
                            else if (gcvValue >= 4101)
                            {
                                var recPIH = dbContext.vw_price_index_history.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.price_index_code == "HBAI" && o.index_date <= Convert.ToDateTime(record.bl_date).Date).FirstOrDefault();
                                record.hba_type = "HBAI";
                                record.hba_value = recPIH?.index_value ?? 0;
                            }
                            else if (gcvValue >= 3401)
                            {
                                var recPIH = dbContext.vw_price_index_history.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.price_index_code == "HBAII" && o.index_date <= Convert.ToDateTime(record.bl_date).Date).FirstOrDefault();
                                record.hba_type = "HBAII";
                                record.hba_value = recPIH?.index_value ?? 0;
                            }
                            else
                            {
                                var recPIH = dbContext.vw_price_index_history.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.price_index_code == "HBAIII" && o.index_date <= Convert.ToDateTime(record.bl_date).Date).FirstOrDefault();
                                record.hba_type = "HBAIII";
                                record.hba_value = recPIH?.index_value ?? 0;
                            }
                        }
                        BlDate = Convert.ToDateTime(record.bl_date);
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

        [HttpGet("HBALookup")]
        public async Task<object> HBALookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var Kelistrikan = new HbaClass
                {
                    Value = "Kelistrikan",
                    Text = "Kelistrikan"
                };
                var Semen = new HbaClass
                {
                    Value = "Semen",
                    Text = "Semen"
                };
                var HBA = new HbaClass
                {
                    Value = "HBA",
                    Text = "HBA"
                };
                var HBAI = new HbaClass
                {
                    Value = "HBAI",
                    Text = "HBAI"
                };
                var HBAII = new HbaClass
                {
                    Value = "HBAII",
                    Text = "HBAII"
                };
                var HBAIII = new HbaClass
                {
                    Value = "HBAIII",
                    Text = "HBAIII"
                };
                var lookup = new List<HbaClass>
                {
                    Kelistrikan,
                    Semen,
                    HBA,
                    HBAI,
                    HBAII,
                    HBAIII
                };
                IEnumerable<HbaClass> iQuery =  lookup.AsEnumerable();
                return DataSourceLoader.Load(iQuery, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("HBACalculate/{royaltyId}/{hbaType}")]
        public async Task<object> HBACalculate(string royaltyId, string hbaType)
        {
            try
            {
                var record = await dbContext.vw_royalty_pricing
                    .Where(o => o.id == royaltyId).FirstOrDefaultAsync();
                BlDate = Convert.ToDateTime(record.bl_date);
                decimal result = 0;
                if (hbaType == "Kelistrikan")
                {
                    result = 70;
                }
                else if (hbaType == "Semen")
                {
                    result = 90;
                }
                else if (hbaType == "HBA")
                {
                    var recPIH = await dbContext.vw_price_index_history
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.price_index_code == "HBA" && o.index_date <= BlDate.Date)
                            .OrderByDescending(o => o.index_date).FirstOrDefaultAsync();
                    result = recPIH?.index_value ?? 0;
                }
                else if (hbaType == "HBAI")
                {
                    var recPIH = await dbContext.vw_price_index_history
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.price_index_code == "HBAI" && o.index_date <= BlDate.Date)
                            .OrderByDescending(o => o.index_date).FirstOrDefaultAsync();
                    result = recPIH?.index_value ?? 0;
                }
                else if (hbaType == "HBAII")
                {
                    var recPIH = await dbContext.vw_price_index_history
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.price_index_code == "HBAII" && o.index_date <= BlDate.Date)
                            .OrderByDescending(o => o.index_date).FirstOrDefaultAsync();
                    result = recPIH?.index_value ?? 0;
                }
                else if (hbaType == "HBAIII")
                {
                    var recPIH = await dbContext.vw_price_index_history
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.price_index_code == "HBAIII" && o.index_date <= BlDate.Date)
                            .OrderByDescending(o => o.index_date).FirstOrDefaultAsync();
                    result = recPIH?.index_value ?? 0;
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("RecalculatePricing/{Id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> RecalculatePricing(string Id)
        {
            try
            {
                var recRoyalty = await dbContext.vw_royalty
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                if (recRoyalty == null) return null;

                var hba_0 = dbContext.vw_price_index_history
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.price_index_code == "HBA")
                    .FirstOrDefault()?.index_value ?? 0;

                var record = await dbContext.vw_royalty_pricing
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                if (record != null)
                {
                    record.hba_0 = hba_0;

                    return Ok(record);
                }
                else return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("SaveData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SaveData([FromForm] string key, [FromForm] string values)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    string Id = key;

                    var record = await dbContext.royalty_pricing
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
                        if (await mcsContext.CanCreate(dbContext, nameof(royalty_pricing),
                            CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                        {
                            var newRecord = new royalty_pricing();
                            JsonConvert.PopulateObject(values, newRecord);

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

                            dbContext.royalty_pricing.Add(newRecord);

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

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
using Microsoft.AspNetCore.Http;
using DataAccess.EFCore.Repository;
using Common;
using Newtonsoft.Json.Linq;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/Royalty/[controller]")]
    [ApiController]
    public class PaymentController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public PaymentController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(string royaltyId, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_royalty_payment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
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
                dbContext.vw_royalty_payment.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpGet("RoyaltypaymentByShippingOrder")] //fariz
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> RoyaltypaymentByShippingOrder(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_royalty_payment.Where(o => o.despatch_order_id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(royalty_payment),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new royalty_payment();
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

                    dbContext.royalty_payment.Add(record);
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
                var record = dbContext.royalty_payment
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
                var record = dbContext.royalty_payment
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.royalty_payment.Remove(record);
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
                var record = dbContext.royalty_payment
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.royalty_payment.Remove(record);
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
                var record = await dbContext.vw_royalty_payment
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
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SaveData([FromForm] string key, [FromForm] string values)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    string Id = key;

                    var record = await dbContext.royalty_payment
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
                            JObject obj = JObject.Parse(values);

                            // Access the 'royalty_values' property
                            JToken royaltyValues = obj["royalty_value"];
                            JToken bmnValues = obj["bmn_value"];
                            JToken dhpbValues = obj["dhpb_final_value"];
                            JToken phtValues = obj["pht_value"];
                            JsonConvert.PopulateObject(values, record);

                           record.InjectFrom(e);
                            record.royalty_outstanding = Convert.ToDecimal(royaltyValues);
                            record.pht_outstanding = Convert.ToDecimal(phtValues);
                            record.dhpb_outstanding = Convert.ToDecimal(dhpbValues);
                            record.bmn_outstanding = Convert.ToDecimal(bmnValues);
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
                        if (await mcsContext.CanCreate(dbContext, nameof(royalty_payment),
                            CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                        {
                            var newRecord = new royalty_payment();
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

                            dbContext.royalty_payment.Add(newRecord);

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

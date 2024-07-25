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
using Microsoft.AspNetCore.Http;
using Common;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/[controller]")]
    [ApiController]
    public class SalesContractTermController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SalesContractTermController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(string salesContractId, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.sales_contract_term
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.sales_contract_id == salesContractId),
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
                dbContext.vw_sales_contract_term.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(sales_contract_term),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new sales_contract_term();
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
                    

                    dbContext.sales_contract_term.Add(record);
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
                var record = dbContext.sales_contract_term
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
                var record = dbContext.sales_contract_term
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.sales_contract_term.Remove(record);
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
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Debug($"string Id = {Id}");

            try
            {
                var record = dbContext.sales_contract_term
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.sales_contract_term.Remove(record);
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

        [HttpGet("SalesContractTermIdLookup")]
        public async Task<object> SalesContractTermIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.sales_contract_term
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.contract_term_name, o.sales_contract_id });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("MasterListIdLookup")]
        public async Task<object> MasterListIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.master_list
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.item_group == "analyte-standard")
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.item_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("FetchProductAnalyteIntoSalesContractProduct/{Id}")]
        public async Task<ApiResponse> FetchProductAnalyteIntoSalesContractProduct(string Id)
        {
            var result = new ApiResponse();
            result.Status.Success = true;
            try
            {
                var contract_product = await dbContext.sales_contract_product
                    .Where(r => r.organization_id == CurrentUserContext.OrganizationId &&
                                r.id == Id).FirstOrDefaultAsync();

                var product = await dbContext.vw_product_specification
                    .Where(r => r.organization_id == CurrentUserContext.OrganizationId &&
                                r.product_id == contract_product.product_id).ToListAsync();

                var checkDataByProductId = dbContext.sales_contract_product_specifications
                    .Where(r => r.sales_contract_product_id == Id);
                if (checkDataByProductId.Count() > 0)
                {
                    dbContext.sales_contract_product_specifications.RemoveRange(checkDataByProductId);
                }

                using (var tx = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var item in product)
                        {
                            if (result.Status.Success)
                            {
                                var newRecord = new sales_contract_product_specifications();

                                if (await mcsContext.CanCreate(dbContext, nameof(sales_contract_product_specifications),
                                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                                {
                                    #region Add record

                                    newRecord.id = Guid.NewGuid().ToString("N");
                                    newRecord.sales_contract_product_id = contract_product.id;
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

                                    newRecord.analyte_id = item.analyte_id;
                                    newRecord.analyte_standard_id = contract_product.analyte_standard_id;
                                    newRecord.target = item.target_value;
                                    newRecord.minimum = item.minimum_value;
                                    newRecord.maximum = item.maximum_value;
                                    newRecord.uom_id = item.uom_id;

                                    dbContext.sales_contract_product_specifications.Add(newRecord);

                                    await dbContext.SaveChangesAsync();
                                    #endregion

                                    result.Status.Success &= true;
                                }
                                else
                                {
                                    result.Status.Success = false;
                                    result.Status.Message = "User is not authorized.";
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

    }
}

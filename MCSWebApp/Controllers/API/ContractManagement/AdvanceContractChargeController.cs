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
using System.Data;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Controllers.API.ContractManagement
{
    [Route("api/ContractManagement/[controller]")]
    [ApiController]
    public class AdvanceContractChargeController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public AdvanceContractChargeController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_advance_contract_charge
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("GetBaseFormulaByContractID")]
        public async Task<object> GetBaseFormulaByContractID(DataSourceLoadOptions loadOptions, string advance_contract_id)
        {
            //var list = dbContext.vw_price_index_map
            //    .Where(r => (CustomFunctions.CanRead(r.id, CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin))
            //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
            //    .Where(r => r.is_base_index ?? true);
            
            //
                
            var list = dbContext.advance_contract_charge
            .Where(o => o.advance_contract_id == advance_contract_id
                && o.is_base_formula == true
                && o.organization_id == CurrentUserContext.OrganizationId); 
            
            return await DataSourceLoader.LoadAsync(list, loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_advance_contract_charge.Where(o => o.id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(advance_contract_charge),
                                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new advance_contract_charge();
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

                        if (IsChargeNameExist("", record.charge_name))
                        {
                            return BadRequest("Charge Name already exist");
                        }

                        var lastRecord = dbContext.advance_contract_charge
                            .OrderByDescending(r => r.created_on)
                            .FirstOrDefault(r => r.advance_contract_item_id == record.advance_contract_item_id && r.organization_id == CurrentUserContext.OrganizationId);
                        if (lastRecord == null)
                            record.variable = "001";
                        else
                        {

                            int lastCode;
                            bool success = int.TryParse(lastRecord.variable, out lastCode);
                            if (success)
                            {
                                lastCode++;
                                if (lastCode.ToString().Length == 1)
                                    record.variable = "00" + lastCode.ToString();
                                else if (lastCode.ToString().Length == 2)
                                    record.variable = "0" + lastCode.ToString();
                                else
                                    record.variable = lastCode.ToString();
                            }
                            else
                            {
                                record.variable = "001";
                            }
                        }


                        dbContext.advance_contract_charge.Add(record);

                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();

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

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.advance_contract_charge
                        .Where(o => o.id == key)
                        .FirstOrDefaultAsync();
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

                            if (IsChargeNameExist(record.id, record.charge_name))
                            {
                                return BadRequest("Charge Name already exist");
                            }

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
                        return BadRequest("Record does not exist.");
                    }
                }
				catch (Exception ex)
				{
					logger.Error(ex.InnerException ?? ex);
					return BadRequest(ex.InnerException?.Message ?? ex.Message);
				}
            }
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.advance_contract_charge
                        .Where(o => o.id == key)
                        .FirstOrDefaultAsync();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.advance_contract_charge.Remove(record);

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok();
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else
                    {
                        return BadRequest("Record does not exist.");
                    }
                }
				catch (Exception ex)
				{
					logger.Error(ex.InnerException ?? ex);
					return BadRequest(ex.InnerException?.Message ?? ex.Message);
				}
            }
        }

        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.vw_advance_contract_charge
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if(record != null)
                {
                    if (await mcsContext.CanRead(dbContext, Id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        return Ok(record);
                    }
                    else
                    {
                        return BadRequest("User is not authorized.");
                    }
                }
                else
                {
                    return BadRequest("Record does not exist.");
                }
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] advance_contract Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.advance_contract_charge
                        .Where(o => o.id == Record.id)
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

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok(record);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else if (await mcsContext.CanCreate(dbContext, nameof(advance_contract_charge),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new advance_contract_charge();
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

                        dbContext.advance_contract_charge.Add(record);
                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();
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

        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Debug($"string Id = {Id}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.advance_contract_charge
                        .Where(o => o.id == Id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, Id, CurrentUserContext.AppUserId)
                                    || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.advance_contract_charge.Remove(record);
                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok();
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else
                    {
                        return BadRequest("Record does not exist.");
                    }
                }
				catch (Exception ex)
				{
					logger.Error(ex.InnerException ?? ex);
					return BadRequest(ex.InnerException?.Message ?? ex.Message);
				}
            }
        }

        [HttpGet("AdvanceContractChargeIdLookupByAdvanceContractId")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ContractorIdLookupByIsEquipmentOwner(DataSourceLoadOptions loadOptions, string advance_contract_id)
        {
            try
            {
                var lookup = dbContext.vw_advance_contract_charge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                           o.advance_contract_id == advance_contract_id)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.charge_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("AdvanceContractItemById")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AdvanceContractItemById(string Id, DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_advance_contract_item
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.advance_contract_id == Id)
                    .Select(o => new { value = o.id, text = o.item_name + (" - " + o.description ?? "") }),
                        loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("CalculateFormulaById")]
        public async Task<object> CalculateFormulaById(DataSourceLoadOptions loadOptions, string advance_contract_id)
        {
            try
            {
                var result = new BusinessLogic.Formula.AdvanceContractChargeResult();
                result.success = true;
                var record = await dbContext.advance_contract_charge.Where(o => o.advance_contract_id == advance_contract_id).FirstOrDefaultAsync();
                if (record != null)
                {
                    var price_index = dbContext.price_index_history.Where(r => r.index_date.Month == record.created_on.Value.Month).Average(r => r.index_value);
                    result.formula = record.formula;

                    if (!string.IsNullOrEmpty(result.formula))
                    {
                        result.formula = (result.formula).Replace("AVG(ThisMonth)", price_index.ToString());
                    }

                    #region Get Advance Contract Reference
                    var advanceContractReferenceRecord = await dbContext.advance_contract_reference.Where(o => o.advance_contract_id == advance_contract_id).FirstOrDefaultAsync();
                    if (advanceContractReferenceRecord == null)
                    {
                        result.message = "Advance Contract Reference Record is not exist.";
                        result.success = false;
                        return Ok(result);
                    }
                    #endregion

                    #region Get Joint Survey Record
                    int isQuantityExist = (result.formula).IndexOf("Quantity");
                    int isDistanceExist = (result.formula).IndexOf("Distance");
                    int isElevationExist = (result.formula).IndexOf("Elevation");

                    if (isQuantityExist >= 0 || isDistanceExist >= 0 || isElevationExist >= 0)
                    { 

                        var jointSurveyRecord = await dbContext.joint_survey.Where(o => o.advance_contract_reference_id == advanceContractReferenceRecord.id).FirstOrDefaultAsync();
                        if (jointSurveyRecord == null)
                        {
                            result.message = "Joint Survey record is not exist.";
                            result.success = false;
                            return Ok(result);
                        }

                        if (isQuantityExist >= 0)
                        {
                            result.formula = (result.formula).Replace("Quantity", jointSurveyRecord.quantity.ToString());
                        }
                        if (isDistanceExist >= 0)
                        {
                            result.formula = (result.formula).Replace("Distance", jointSurveyRecord.distance.ToString());
                        }
                        if (isElevationExist >= 0)
                        {
                            result.formula = (result.formula).Replace("Elevation", jointSurveyRecord.elevation.ToString());
                        }
                    }
                    #endregion

                    result.average_of_price_index = (double)price_index;
                    result.result = calculate(result.formula);
                }
                
                if (record != null)
                {
                    if (await mcsContext.CanRead(dbContext, advance_contract_id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest("User is not authorized.");
                    }
                }
                else
                {
                    return BadRequest("Record does not exist.");
                }
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [NonAction]
        private double calculate(string formula)
        {
            var result = Convert.ToDouble(new DataTable().Compute(formula, null));
            return result;
        }

        private bool IsChargeNameExist(string id, string charge_name)
        {
            var record = dbContext.vw_advance_contract_charge
                    .Where(r => r.charge_name.Trim().ToLower() == charge_name.Trim().ToLower() &&
                           r.organization_id == CurrentUserContext.OrganizationId &&
                           r.id != id)
                    .FirstOrDefault();
            if (record != null)
                return true;
            else
                return false;
        }
    }
}

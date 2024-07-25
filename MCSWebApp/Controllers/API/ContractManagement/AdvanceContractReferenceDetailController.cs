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
using System.Text.RegularExpressions;
using Jace;
using BusinessLogic.Formula;
using Microsoft.AspNetCore.Http;

namespace MCSWebApp.Controllers.API.ContractManagement
{
    [Route("api/ContractManagement/[controller]")]
    [ApiController]
    public class AdvanceContractReferenceDetailController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public AdvanceContractReferenceDetailController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid/{Id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.advance_contract_reference_detail
                .Where(o => o.advance_contract_reference_id == Id), 
                loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_advance_contract_reference_detail.Where(o => o.id == Id &&
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
                    if (await mcsContext.CanCreate(dbContext, nameof(advance_contract_reference_detail),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new advance_contract_reference_detail();
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

                        dbContext.advance_contract_reference_detail.Add(record);

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
                    var record = await dbContext.advance_contract_reference_detail
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
                    var record = await dbContext.advance_contract_reference_detail
                        .Where(o => o.id == key)
                        .FirstOrDefaultAsync();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, record.entity_id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.advance_contract_reference_detail.Remove(record);

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                            return Ok();
                        }
                        else
                        {
                            return BadRequest("User is not authorized");
                        }
                    }
                    else
                    {
                        return BadRequest("Record does not exist");
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
                var record = await dbContext.vw_advance_contract_reference_detail
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

        [HttpGet("AdvanceContractChargeIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AdvanceContractChargeIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_advance_contract_charge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = (o.variable + " - " + o.charge_name) });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("AdvanceContractChargeIdLookup/{advance_contract_item_id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AdvanceContractChargeIdLookup(DataSourceLoadOptions loadOptions,  string advance_contract_item_id)
        {
            try
            {
                var lookup = dbContext.vw_advance_contract_charge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.advance_contract_item_id == advance_contract_item_id)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = (o.variable + " - " + o.charge_name) });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("AdvanceContractReferenceIdLookupByAdvanceContractReferenceId")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ContractorIdLookupByIsEquipmentOwner(DataSourceLoadOptions loadOptions, string advance_contract_reference_id)
        {
            try
            {
                var lookup = dbContext.vw_advance_contract_reference_detail
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                           o.advance_contract_reference_id == advance_contract_reference_id)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.charge_name});
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("AdvanceContractReferenceDetailIdLookupByAdvanceContractId")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AdvanceContractReferenceDetailIdLookupByAdvanceContractId(DataSourceLoadOptions loadOptions, string advance_contract_id)
        {
            try
            {
                var lookup = dbContext.vw_advance_contract_reference_detail
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

        [HttpGet("AdvanceContractReferenceDetailIdLookupByAdvanceContractReferenceId")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> AdvanceContractReferenceDetailIdLookupByAdvanceContractReferenceId(DataSourceLoadOptions loadOptions, string advance_contract_reference_id)
        {
            try
            {
                var lookup = dbContext.vw_advance_contract_reference_detail
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                           o.advance_contract_reference_id == advance_contract_reference_id)
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

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] advance_contract Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.advance_contract_reference_detail
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
                    else if (await mcsContext.CanCreate(dbContext, nameof(advance_contract_reference_detail),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new advance_contract_reference_detail();
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

                        dbContext.advance_contract_reference_detail.Add(record);
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
                    var record = dbContext.advance_contract_reference_detail
                        .Where(o => o.id == Id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, Id, CurrentUserContext.AppUserId)
                                    || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.advance_contract_reference_detail.Remove(record);
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

        [HttpGet("CalculateFormulaById")]
        public async Task<object> CalculateFormulaById(DataSourceLoadOptions loadOptions, string recordId)
        {
            try
            {
                var result = new AdvanceContractChargeResult();
                result.success = true;
                var record = await dbContext.vw_advance_contract_reference_detail
                    .Where(o => o.id == recordId && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    result.formula = record.formula;
                    if (string.IsNullOrEmpty(result.formula))
                    {
                        result.message = "Formula is empty";
                        result.success = false;
                        return Ok(result);
                    }
                    logger.Info("Formula: " + result.formula);

                    #region Round Formula
                    //var isRound = (result.formula).IndexOf("ROUND");
                    //if (isRound >= 0)
                    //{
                    //    //string patternText = @"^ROUND([a-zA-Z0-9],[0-9])$";
                    //    //Regex reg = new Regex(patternText);
                    //    //Console.WriteLine(reg.IsMatch(result.formula));
                    //    //var isf = reg.IsMatch(result.formula);

                    //    var roundStringFormula = "";
                    //    var foundBracket = false;
                    //    var totalOpenBracket = 0;
                    //    var totalCloseBracket = 0;
                    //    for (var i = isRound; i < (result.formula).Length; i++)
                    //    {
                    //        roundStringFormula += result.formula[i];
                    //        if (result.formula[i] == '(')
                    //        {
                    //            totalOpenBracket++;
                    //        }
                    //        if (result.formula[i] == ')')
                    //        {
                    //            totalCloseBracket++;
                    //        }

                    //        if (totalOpenBracket == totalCloseBracket && totalOpenBracket > 0 && totalCloseBracket > 0)
                    //        {

                    //        }
                    //    }
                    //}
                    #endregion

                    //result = CalculateStringFormula(record, result.formula);

                    string multyFormula = string.Empty;
                    // base formula
                    var getFormula = dbContext.vw_advance_contract_reference_detail
                    .Where(o => o.advance_contract_id == record.advance_contract_id 
                        && o.is_base_formula == true
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .ToList();

                    foreach (var item in getFormula)
                    {
                        //Bobby 20220302 Base Formula Error Reference
                        item.advance_contract_reference_id = record.advance_contract_reference_id;
                        //Bobby 20220302 Base Formula Error Reference

                        int isVariableNameExist = (result.formula).IndexOf("$" + item.charge_name);
                        if (isVariableNameExist >= 0)
                        {
                            // start base formula -- loop 2
                            var resultFormula = new AdvanceContractChargeResult();
                            foreach (var item2 in getFormula)
                            {
                                int isVariableNameExist2 = (item.formula).IndexOf("$" + item2.charge_name);
                                if (isVariableNameExist2 >= 0)
                                {
                                    var resultFormula2 = new AdvanceContractChargeResult();
                                    resultFormula2 = CalculateStringFormula(item2, item2.formula);
                                    multyFormula = (item.formula).Replace("$" + item2.charge_name, item2.formula);
                                    resultFormula2.value = rounding(Convert.ToDecimal(resultFormula2.value), item2.rounding_type, Convert.ToInt32(item2.decimal_places));
                                    resultFormula.formula = (item.formula).Replace("$" + item2.charge_name, resultFormula2.value.ToString());
                                }
                            }
                            // end base formula  -- loop 2
                            if (resultFormula.formula != null)
                            {
                                resultFormula = CalculateStringFormula(item, resultFormula.formula);
                                resultFormula.value = rounding(Convert.ToDecimal(resultFormula.value), item.rounding_type, Convert.ToInt32(item.decimal_places));
                                multyFormula += " <==" + (result.formula).Replace("$" + item.charge_name, resultFormula.formula);
                                result.formula = (result.formula).Replace("$" + item.charge_name, resultFormula.value.ToString());
                            }
                            else
                            {
                                resultFormula = CalculateStringFormula(item, item.formula);
                                resultFormula.value = rounding(Convert.ToDecimal(resultFormula.value), item.rounding_type, Convert.ToInt32(item.decimal_places));
                                multyFormula = (result.formula).Replace("$" + item.charge_name, item.formula);
                                result.formula = (result.formula).Replace("$" + item.charge_name, resultFormula.value.ToString());

                            }
                            
                        }
                    }
                  
                    result = CalculateStringFormula(record, result.formula);
                    result.value = rounding(Convert.ToDecimal(result.value), record.rounding_type, Convert.ToInt32(record.decimal_places));
                    if (multyFormula != string.Empty)
                    {
                        result.formula += " <==" + multyFormula;
                    }
                }

                if (record != null)
                    return Ok(result);
                else
                    return BadRequest("Record does not exist.");
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        
        [NonAction]
        private double rounding(decimal value, string roundingType,int decimalPlaces)
        {
            decimal Result = 0;
            switch (roundingType)
            {
                case "Round":
                    Result = Math.Round(value, decimalPlaces);
                    break;
                case "Round Down":
                    Result = Math.Floor(value);
                    break;
                case "Round Up":
                    Result = Math.Ceiling(value);
                    break;
                case "Truncate":
                    Result = Math.Truncate(value);
                    break;
                default:
                    Result = value;
                    break;
            }
            return Convert.ToDouble(Result);
        }

        [NonAction]
        private AdvanceContractChargeResult CalculateStringFormula(vw_advance_contract_reference_detail record, string stringFormula)
        {
            var result = new AdvanceContractChargeResult();
            result.formula = stringFormula;
            result.success = true;
            try
            {
                #region Get Advance Contract Item Detail
                var advanceContractItemDetailRecords = dbContext.vw_advance_contract_item_detail
                    .Where(o => o.advance_contract_id == record.advance_contract_id && o.organization_id == CurrentUserContext.OrganizationId)
                    .ToList();
                if (advanceContractItemDetailRecords == null || advanceContractItemDetailRecords.Count() == 0)
                {
                    result.message = "There is no Advance Contract Item Detail Record exist. Please check your Advance Contract and items.";
                    result.success = false;
                    return result;
                }

                foreach (var item in advanceContractItemDetailRecords)
                {
                    int isVariableNameExist = (result.formula).IndexOf(item.variable);
                    if (isVariableNameExist >= 0)
                    {
                        result.formula = (result.formula).Replace(item.variable, item.amount.ToString());
                    }
                }
                #endregion

                //bobby 20220302 Based Formula
               
               var advance_contract_reference = dbContext.advance_contract_reference
                        .FirstOrDefault(r => r.id == record.advance_contract_reference_id && r.organization_id == CurrentUserContext.OrganizationId);
                if (advance_contract_reference == null)
                { 
                        result.message = "There is no record Advance contract reference.";
                        result.success = false;
                        return result;
                }
                //bobby 20220302 Based Formula



                #region Price Index
                var priceIndexRecords = dbContext.price_index
                    .Where(r => r.organization_id == CurrentUserContext.OrganizationId).ToList();
                if (priceIndexRecords == null || priceIndexRecords.Count <= 0)
                {
                    result.message = "There is no Price Index Record available";
                    result.success = false;
                    return result;
                }

                var advanceContractChargeRecord = dbContext.vw_advance_contract_charge.Where(r => r.id == record.advance_contract_charge_id).FirstOrDefault();
                if (advanceContractChargeRecord == null)
                {
                    result.message = "There is no Advance Charge Record Record available";
                    result.success = false;
                    return result;
                }

                var priceIndexHistoryRecords = dbContext.price_index_history
                    .Where(r => r.index_date >= advance_contract_reference.start_date.Value && r.index_date <= advance_contract_reference.end_date.Value)
                    .Where(r => r.price_index_id == advanceContractChargeRecord.price_index_id)
                    .Where(r => r.organization_id == CurrentUserContext.OrganizationId)
                    .ToList();

                if (!string.IsNullOrEmpty(result.formula) && (result.formula).IndexOf("AVG(ThisMonth)") >= 0)
                {
                    if (priceIndexHistoryRecords.Count > 0)
                        result.formula = (result.formula).Replace("AVG(ThisMonth)", Math.Round(priceIndexHistoryRecords.Average(r => r.index_value), 3).ToString());
                    else
                        result.formula = (result.formula).Replace("AVG(ThisMonth)", "0");
                }

                var priceIndexBaseRecord = priceIndexRecords
                    .Where(r => r.is_base_index ?? false && r.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (priceIndexBaseRecord == null)
                {
                    result.message = "There is no record Price Index Base. Please set Price Index as a Price Index Base.";
                    result.success = false;
                    return result;
                }

                var priceIndexBaseName = priceIndexBaseRecord.price_index_name;

                var priceIndexMapRecords = dbContext.vw_price_index_map
                    .Where(r => r.price_index_id == priceIndexBaseRecord.id)
                    .Where(r => r.organization_id == CurrentUserContext.OrganizationId)
                    .ToList();
                if (priceIndexMapRecords == null || priceIndexMapRecords.Count == 0)
                {
                    result.message = "There is no record Price Index Detail. Please create Price Index Detail base on Price Index Base";
                    result.success = false;
                    return result;
                }

                foreach (var item in priceIndexMapRecords)
                {
                    int isVariablePriceIndexBaseExist = (result.formula).IndexOf(item.name);
                    if (isVariablePriceIndexBaseExist >= 0)
                    {
                        var priceIndexMapDetails = dbContext.vw_price_index_map_detail
                            .Where(r => r.is_base_index ?? false)
                            .Where(r => r.price_index_map_id == item.id)
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId).ToList();
                        if (priceIndexMapDetails.Count == 0 || priceIndexMapDetails == null)
                        {
                            result.message = "There is no record Price Index Detail.";
                            result.success = false;
                            return result;
                        }
                        // Base Index using 3 month
                        var lastThreeMonthsRecords = dbContext.price_index_history
                            .Where(r => r.index_date >= advance_contract_reference.end_date.Value.AddMonths(-3) && r.index_date <= advance_contract_reference.end_date.Value)
                            .Where(r => r.price_index_id == priceIndexBaseRecord.id)
                            .Where(r => r.organization_id == CurrentUserContext.OrganizationId)
                            .ToList();
                        if (lastThreeMonthsRecords.Count == 0 || lastThreeMonthsRecords == null)
                        {
                            result.message = "There is no record on Price Index history with name " + priceIndexBaseName + ".";
                            result.success = false;
                            return result;
                        }

                        var avg = lastThreeMonthsRecords.Average(r => r.index_value);
                        var finalMap = priceIndexMapDetails
                            .Where(r => avg >= r.start_range && avg <= r.end_range)
                            .Where(r => r.price_index_map_id == item.id)
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .FirstOrDefault();
                        if (finalMap == null)
                        {
                            result.message = "There is no record of average last three months in range.";
                            result.success = false;
                            return result;
                        }

                        if ((result.formula).IndexOf("Raise&Fall*" + item.name) >= 0)
                        {
                            var mopsRecord = priceIndexRecords.FirstOrDefault(r => r.price_index_name.ToUpper().Contains("MOPS") || r.price_index_code.ToUpper().Contains("MOPS"));
                            if (mopsRecord == null)
                            {
                                result.message = "There is no Price Index record of MOPS.";
                                result.success = false;
                                return result;
                            }

                            var avgMopsHistoryRecords = dbContext.price_index_history
                                    .Where(r => r.index_date >= advance_contract_reference.start_date.Value && r.index_date <= advance_contract_reference.end_date.Value)
                                    .Where(r => r.price_index_id == mopsRecord.id)
                                    .Where(r => r.organization_id == CurrentUserContext.OrganizationId)
                                    .ToList();
                            if (avgMopsHistoryRecords.Count == 0)
                            {
                                result.message = "There is no history Price Index of MOPS.";
                                result.success = false;
                                return result;
                            }

                            var temp = avgMopsHistoryRecords.Average(r => r.index_value);
                            var afpResult = Math.Round(calculate(temp.ToString() + "/158.9873*1.15*(1+0.075)"), 3);
                      
                            {
                                var rfFormula = "((" + afpResult + ")/0.57*0.325+0.675)*" + finalMap.value.ToString();
                                var rfResult = Math.Round(calculate(rfFormula), 3);
                                result.formula = (result.formula).Replace("Raise&Fall*" + item.name, rfResult.ToString());
                            }
                        } 
                        else
                        {
                            result.formula = (result.formula).Replace(item.name, finalMap.value.ToString());
                        }

                    }
                }
                #endregion

                #region Production Closing
                int isProdVolumeExist = (result.formula).IndexOf("ProdVolume");
                int isProdDistanceExist = (result.formula).IndexOf("ProdDistance");
                if (isProdVolumeExist >= 0 || isProdDistanceExist >= 0)
                {

                    var productionClosingRecord = dbContext.vw_production_closing
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                               o.advance_contract_reference_id == record.advance_contract_reference_id &&
                               o.transaction_date >= advance_contract_reference.start_date.Value &&
                               o.transaction_date <= advance_contract_reference.end_date.Value)
                        .GroupBy(o => o.organization_id)
                        .Select(o =>
                        new
                        {
                            volume = o.Sum(p => p.volume),
                            distance = o.Sum(p => p.distance)
                        }).FirstOrDefault(); ;
                    if (isProdVolumeExist >= 0)
                        result.formula = (result.formula).Replace("ProdVolume", productionClosingRecord.volume.ToString());
                    if (isProdDistanceExist >= 0)
                        result.formula = (result.formula).Replace("ProdDistance", productionClosingRecord.distance.ToString());
                }
                #endregion

                #region DayWork
                int isDWTotalHMExist = (result.formula).IndexOf("DWTotalHM");
                int isDWTotalValueExist = (result.formula).IndexOf("DWTotalValue");
                if (isDWTotalHMExist >= 0 || isDWTotalValueExist >= 0)
                {

                    var dayworkRecord = dbContext.daywork_closing
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                               o.advance_contract_reference_id == record.advance_contract_reference_id &&
                               o.transaction_date >= advance_contract_reference.start_date.Value &&
                               o.transaction_date <= advance_contract_reference.end_date.Value)
                        .GroupBy(o => o.organization_id)
                        .Select(o =>
                        new
                        {
                            hm_duration = o.Sum(p => p.total_hm),
                            hm_value = o.Sum(p => p.total_value)
                        }).FirstOrDefault(); ;
                    if (isDWTotalHMExist >= 0)
                        result.formula = (result.formula).Replace("DWTotalHM", dayworkRecord.hm_duration.ToString());
                    if (isDWTotalValueExist >= 0)
                        result.formula = (result.formula).Replace("DWTotalValue", dayworkRecord.hm_value.ToString());
                }
                #endregion

                result = additionalFormula(advance_contract_reference, result.formula);

                #region Get Joint Survey Record
                int isQuantityExist = (result.formula).IndexOf("Quantity");
                int isDistanceExist = (result.formula).IndexOf("Distance");
                int isElevationExist = (result.formula).IndexOf("Elevation");
                int isQuantityCarryOverExist = (result.formula).IndexOf("QuantityCarryOver");
                int isDistanceCarryOverExist = (result.formula).IndexOf("DistanceCarryOver");
                int isElevationCarryOverExist = (result.formula).IndexOf("ElevationCarryOver");

                if (isQuantityExist >= 0 || isDistanceExist >= 0 || isElevationExist >= 0)
                {

                    var jointSurveyRecord = dbContext.joint_survey
                        .Where(o => o.advance_contract_reference_id == record.advance_contract_reference_id && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (jointSurveyRecord == null)
                    {
                        result.message = "Joint Survey record is not exist.";
                        result.success = false;
                        return result;
                    }

                    if (isQuantityExist >= 0)
                        result.formula = (result.formula).Replace("Quantity", jointSurveyRecord.quantity.ToString());
                    if (isDistanceExist >= 0)
                        result.formula = (result.formula).Replace("Distance", jointSurveyRecord.distance.ToString());
                    if (isElevationExist >= 0)
                        result.formula = (result.formula).Replace("Elevation", jointSurveyRecord.elevation.ToString());

                    if (isDistanceCarryOverExist >= 0)
                    {
                        if (jointSurveyRecord.distance_carry_over == null)
                        {
                            result.message = "Distance Carry Over Value is emtpy. Please check value first.";
                            result.success = false;
                            return result;
                        }
                        result.formula = (result.formula).Replace("DistanceCarryOver", jointSurveyRecord.distance_carry_over.ToString());
                    }
                    if (isElevationCarryOverExist >= 0)
                    {
                        if (jointSurveyRecord.elevation_carry_over == null)
                        {
                            result.message = "Elevation Carry Over Value is emtpy. Please check value first.";
                            result.success = false;
                            return result;
                        }
                        result.formula = (result.formula).Replace("ElevationCarryOver", jointSurveyRecord.elevation_carry_over.ToString());
                    }
                    if (isQuantityCarryOverExist >= 0)
                    {
                        if (jointSurveyRecord.quantity_carry_over == null)
                        {
                            result.message = "Quantity Carry Over Value is emtpy. Please check value first.";
                            result.success = false;
                            return result;
                        }
                        result.formula = (result.formula).Replace("QuantityCarryOver", jointSurveyRecord.quantity_carry_over.ToString());
                    }
                }
                #endregion

                //result.average_of_price_index = (double)price_index;
                var finalAmount = Math.Round(calculate(result.formula), 3);

                #region Amount Convertion
                var advanceContractRecord = dbContext.advance_contract
                            .Where(r => r.organization_id == CurrentUserContext.OrganizationId)
                            .Where(r => r.id == record.advance_contract_id)
                            .FirstOrDefault();
                if (advanceContractRecord == null)
                {
                    result.message = "There is no Advance Contract Record.";
                    result.success = false;
                    return result;
                }

                var currencyRecord = new currency();
                if (advanceContractRecord.contract_currency_id == null)
                {
                    currencyRecord = dbContext.currency
                        .Where(r => r.currency_code.ToUpper() == "USD")
                        .Where(r => r.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();

                    if (currencyRecord == null)
                    {
                        result.message = "There is no record of USD.";
                        result.success = false;
                        return result;
                    }
                }
                else
                {
                    currencyRecord = dbContext.currency
                        .Where(r => r.id == advanceContractRecord.contract_currency_id)
                        .Where(r => r.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();

                    if (currencyRecord == null)
                    {
                        result.message = "There is no currency record.";
                        result.success = false;
                        return result;
                    }
                }

                var avgCurrencyHistoryRecords = dbContext.vw_price_index_history
                            .Where(r => r.index_date >= advance_contract_reference.start_date.Value && r.index_date <= advance_contract_reference.end_date.Value)
                            .Where(r => r.price_index_code.ToUpper().Contains("KURS BI") || r.price_index_code.ToUpper().Contains("KURS-BI"))
                            .Where(r => r.organization_id == CurrentUserContext.OrganizationId)
                            .ToList();
                if (avgCurrencyHistoryRecords.Count == 0)
                {
                    result.message = "There is no record KURS BI.";
                    result.success = false;
                    return result;
                }

                if (currencyRecord.currency_code.ToUpper() == "USD")
                    result.convertion_amount = Math.Round((finalAmount * (double)avgCurrencyHistoryRecords.Average(r => r.index_value)), 3);
                else
                    result.convertion_amount = Math.Round((finalAmount / (double)avgCurrencyHistoryRecords.Average(r => r.index_value)), 3);
                #endregion

                result.value = finalAmount;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw new Exception(ex.Message);
            }
            return result;
        }

        [NonAction]
        private AdvanceContractChargeResult additionalFormula(advance_contract_reference advance_contract_reference, string stringFormula)
        {
            var result = new AdvanceContractChargeResult();
            result.formula = stringFormula;
            result.success = true;
            try
            {

                var priceIndexRecords = dbContext.price_index
                    .Where(r => r.organization_id == CurrentUserContext.OrganizationId).ToList();
                if (priceIndexRecords == null || priceIndexRecords.Count <= 0)
                {
                    result.message = "There is no Price Index Record available";
                    result.success = false;
                    return result;
                }

                #region Additionals
                var isRF = (result.formula).IndexOf("Raise&Fall");
                var isAFP = (result.formula).IndexOf("AFP");
                int isFC = (result.formula).IndexOf("FC");
                int isBFP = (result.formula).IndexOf("BFP");

                if (isAFP >= 0 || isRF >= 0)
                {
                    var mopsRecord = priceIndexRecords.FirstOrDefault(r => r.price_index_name.ToUpper().Contains("MOPS") || r.price_index_code.ToUpper().Contains("MOPS"));
                    if (mopsRecord == null)
                    {
                        result.message = "There is no Price Index record of MOPS.";
                        result.success = false;
                        return result;
                    }

                    var avgMopsHistoryRecords = dbContext.price_index_history
                            .Where(r => r.index_date >= advance_contract_reference.start_date.Value && r.index_date <= advance_contract_reference.end_date.Value)
                            .Where(r => r.price_index_id == mopsRecord.id)
                            .Where(r => r.organization_id == CurrentUserContext.OrganizationId)
                            .ToList();
                    if (avgMopsHistoryRecords.Count == 0)
                    {
                        result.message = "There is no history Price Index of MOPS.";
                        result.success = false;
                        return result;
                    }

                    var temp = avgMopsHistoryRecords.Average(r => r.index_value);
                    var afpResult = Math.Round(calculate(temp.ToString() + "/158.9873*1.15*(1+0.075)"), 3);
                    if (isAFP >= 0)
                    {
                        result.formula = (result.formula).Replace("AFP", afpResult.ToString());
                    }
                    if (isRF >= 0)
                    {
                        //var temp = avgMopsHistoryRecords.Average(r => r.index_value);
                        //var afpResult = calculate("(("+ temp.ToString() + "/158.9873*1.15*1.075/0.57)*0.325+0.675)");
                        var rfFormula = "(" + afpResult + ")/0.57*0.325+0.675";
                        var rfResult = Math.Round(calculate(rfFormula), 4);
                        result.formula = (result.formula).Replace("Raise&Fall", rfResult.ToString());
                    }
                }
                if (isFC >= 0)
                {
                    result.formula = (result.formula).Replace("FC", "0.325");
                }
                if (isBFP >= 0)
                {
                    result.formula = (result.formula).Replace("BFP", "0.57");
                }
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw new Exception(ex.Message);
            }
            return result;
        }

        [NonAction]
        private double calculate(string stringFormula)
        {
            double result = 0;
            try
            {
                //-- 3800000 * 1800 * (1.5+(-0.098))
                DataTable dt = new DataTable();
                dt.TableName = "EMP";
                dt.Columns.Add("Name", typeof(string)); // Datatype string  
                dt.Columns.Add("Salary", typeof(int)); // DataType int  
                dt.Columns.Add("Commission", typeof(int));

                var foundBracket = false;
                var totalOpenBracket = 0;
                var totalCloseBracket = 0;
                var newFormula = "";
                var temp = "";
                //-- Find '('
                for (var i = 0; i < stringFormula.Length; i++)
                {
                    if (stringFormula[i] == '(')
                    {
                        totalOpenBracket++;
                    }
                    if (stringFormula[i] == ')')
                    {
                        totalCloseBracket++;
                    }
                }

                if (totalOpenBracket != totalCloseBracket)
                {
                    throw new Exception("Syntax Formula is Error. Please check your formula.");
                }

                CalculationEngine engine = new CalculationEngine();
                logger.Info("String Formula");
                logger.Info(stringFormula);
                result = engine.Calculate(stringFormula);
                logger.Info("Result");
                logger.Info(result);
                logger.Info("String Formula");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}

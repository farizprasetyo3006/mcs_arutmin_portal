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
using DocumentFormat.OpenXml.InkML;
using HiSystems.Interpreter;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using NPOI.SS.Formula.Functions;
using DocumentFormat.OpenXml.Drawing;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/Royalty/[controller]")]
    [ApiController]
    public class RoyaltyController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public RoyaltyController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.royalty
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
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
                dbContext.vw_royalty.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpGet("GetReferenceOnStatus")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetReferenceOnStatus(string Id, DataSourceLoadOptions loadOptions)
        {
            var cekRecord = dbContext.vw_royalty.Where(o => o.despatch_order_id == Id).FirstOrDefault();
            if (cekRecord != null && cekRecord.status_code.ToUpper() == "AWL")
            {
                var status = await dbContext.master_list.Where(o => o.item_in_coding == "AKH").Select(o => o.id).FirstOrDefaultAsync();
                var royaltyAwal = await dbContext.royalty.Where(o => o.despatch_order_id == Id).Select(o => o.royalty_reference).FirstOrDefaultAsync();
                var lookup = new List<object>();
                if (status != null)
                    lookup.Add(status);

                if (royaltyAwal != null)
                    lookup.Add(royaltyAwal);

                return DataSourceLoader.Load(lookup, loadOptions);
               // return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            return null;
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
                    if (await mcsContext.CanCreate(dbContext, nameof(royalty),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new royalty();
                        JsonConvert.PopulateObject(values, record);

                        record.id = Guid.NewGuid().ToString("N");
                        record.created_by = CurrentUserContext.AppUserId;
                        record.created_on = System.DateTime.Now;
                        record.modified_by = null;
                        record.modified_on = null;
                        record.is_active = true;
                        record.is_default = null;
                        record.is_locked = null;
                        record.entity_id = null;
                        record.owner_id = CurrentUserContext.AppUserId;
                        record.organization_id = CurrentUserContext.OrganizationId;
                        record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        dbContext.royalty.Add(record);
                        await dbContext.SaveChangesAsync();

                        //var analyte = dbContext.master_list
                        //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        //             && o.item_group == "royalty-analytes")
                        //    .Select(o => new
                        //    {
                        //        id = o.id,
                        //        symbol = o.item_in_coding
                        //    }).ToList();

                        var recStatus = await dbContext.master_list.Where(o => o.id == record.status_id).FirstOrDefaultAsync();
                        var statusCode = recStatus?.item_in_coding??"";

                        var arrayAnalyte = await dbContext.analyte
                            .FromSqlRaw(@"select * from analyte where lower(replace(analyte_symbol, ' ','')) IN ('tm(arb)', 
                                'im(adb)', 'ash(arb)', 'ash(adb)', 'ts(arb)', 'ts(adb)', 'gcv(arb)', 'gcv(adb)') and organization_id = {0}",
                                CurrentUserContext.OrganizationId)
                            .Select(o => new
                            {
                                id = o.id,
                                analyte_symbol = o.analyte_symbol
                            }).ToArrayAsync();

                        var recSCPS = await dbContext.vw_quality_sampling_analyte
                            .FromSqlRaw(@"select an.id, an.analyte_symbol, scps.target as analyte_value from analyte an
	                            left join sales_contract_product_specifications scps on scps.analyte_id = an.id
	                            left join sales_contract_product scp on scp.id = scps.sales_contract_product_id
	                            left join sales_contract_term sct on sct.id = scp.sales_contract_term_id
	                            left join despatch_order dor on dor.contract_term_id = sct.id
	                            where dor.id = {0} and dor.organization_id = {1} ", record.despatch_order_id,
                                CurrentUserContext.OrganizationId)
                            .Select(o => new { id = o.id, analyte_symbol = o.analyte_symbol, analyte_value = o.analyte_value }).ToArrayAsync();

                        var recQSA = await dbContext.vw_quality_sampling_analyte
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.despatch_order_id == record.despatch_order_id)
                            .Select(o => new { id = o.id, analyte_symbol = o.analyte_symbol, analyte_value = o.analyte_value }).ToArrayAsync();

                        foreach (var item in arrayAnalyte)
                        {
                            var cekData = await dbContext.royalty_quantity_quality
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.royalty_id == record.id
                                    && o.analyte_id == item.id)
                                .FirstOrDefaultAsync();
                            if (cekData == null)
                            {
                                var newRecord = new royalty_quantity_quality();

                                newRecord.id = Guid.NewGuid().ToString("N");
                                newRecord.created_by = CurrentUserContext.AppUserId;
                                newRecord.created_on = System.DateTime.Now;
                                newRecord.modified_by = null;
                                newRecord.modified_on = null;
                                newRecord.is_active = true;
                                newRecord.is_default = null;
                                newRecord.is_locked = null;
                                newRecord.entity_id = null;
                                newRecord.owner_id = CurrentUserContext.AppUserId;
                                newRecord.organization_id = CurrentUserContext.OrganizationId;

                                newRecord.royalty_id = record.id;
                                newRecord.analyte_id = item.id;

                                dynamic recVal;
                                if (statusCode == "AWL")
                                {
                                    recVal = recSCPS.Where(o => o.analyte_symbol.Replace(" ", "").ToLower() == item.analyte_symbol.Replace(" ", "").ToLower())
                                        .FirstOrDefault();
                                    if (recVal != null) newRecord.analyte_value = recVal.analyte_value;
                                }
                                else
                                {
                                    recVal = recQSA.Where(o => o.analyte_symbol.Replace(" ", "").ToLower() == item.analyte_symbol.Replace(" ", "").ToLower())
                                        .FirstOrDefault();
                                    if (recVal != null) newRecord.analyte_value = recVal.analyte_value;
                                }

                                dbContext.royalty_quantity_quality.Add(newRecord);
                                await dbContext.SaveChangesAsync();
                            }
                        }

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
                    await tx.RollbackAsync();
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

            try
            {
                var record = dbContext.royalty
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
                        record.modified_on = System.DateTime.Now;

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
                var record = dbContext.royalty
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dynamic recordX = dbContext.royalty_quantity_quality
                        .Where(o => o.royalty_id == key);
                    if (recordX != null) dbContext.royalty_quantity_quality.RemoveRange(recordX);

                    recordX = dbContext.royalty_cost
                        .Where(o => o.royalty_id == key);
                    if (recordX != null) dbContext.royalty_cost.RemoveRange(recordX);

                    recordX = dbContext.royalty_pricing
                        .Where(o => o.royalty_id == key);
                    if (recordX != null) dbContext.royalty_pricing.RemoveRange(recordX);

                    recordX = dbContext.royalty_valuation
                        .Where(o => o.royalty_id == key);
                    if (recordX != null) dbContext.royalty_valuation.RemoveRange(recordX);

                    recordX = dbContext.royalty_payment
                        .Where(o => o.royalty_id == key);
                    if (recordX != null) dbContext.royalty_payment.RemoveRange(recordX);

                    dbContext.royalty.Remove(record);
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

        [HttpPost("UpdateVolumeLoading")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UpdateVolumeLoading([FromForm] string key, [FromForm] string volumeLoading)
        {
            try
            {
                string Id = key;

                var record = await dbContext.royalty
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    record.volume_loading = Convert.ToDecimal(volumeLoading);
                    await dbContext.SaveChangesAsync();
                }

                return Ok(record);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpGet("Detail/{Id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.vw_royalty
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                return Ok(record);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("GetRoyaltyAnalytes")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetRoyaltyAnalytes(DataSourceLoadOptions loadOptions)
        {
            ReservedWords rw;
            List<ReservedWords> listRW = new List<ReservedWords>();
            var lookupMasterList = await dbContext.master_list.
                Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                           o.item_group == "royalty-analytes").ToArrayAsync();
            foreach (master_list lookup in lookupMasterList)
            {
                rw = new ReservedWords(lookup.item_name + ".Royalty()", lookup.notes);
                listRW.Add(rw);
            }
            //return listRW;
            return DataSourceLoader.Load(listRW, loadOptions);
        }

        [HttpGet("CheckSyntaxFormula")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> CheckSyntaxFormula(string tsyntax, DataSourceLoadOptions loadOptions)
        {
            string tstatus = "";
            string tmessage = "";
            string xsyntax = tsyntax.ToLower().Replace(" ", "");
            var lookupMasterList = await dbContext.master_list
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.item_group == "royalty-analytes").ToArrayAsync();
            foreach (master_list lookup in lookupMasterList)
            {
                xsyntax = xsyntax.Replace(lookup.item_name.ToLower(), "5");
            }
            var lookupAnalyteSymbol = dbContext.analyte.
                Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                           o.analyte_symbol.Length > 0);
            foreach (analyte lookup in lookupAnalyteSymbol)
            {
                xsyntax = xsyntax.Replace((lookup.analyte_symbol.Replace(" ", "") + ".royalty()").ToLower(), "4");
            }
            xsyntax = xsyntax.Replace("hbavalue.royalty()", "1");

            try
            {
                var engine1 = new Engine();
                var expression1 = engine1.Parse(xsyntax);
                var resultText = expression1.Execute().ToString();
                tstatus = "OK";
                tmessage = "OK";
            }
            catch (Exception ex)
            {
                tstatus = "Error in syntax";
                tmessage = ex.Message;
            }
            var retVal = new
            {
                status = tstatus,
                message = tmessage
            };
            return retVal;
        }

        [HttpGet("CalculateFormulaById/{royaltyId}/{salesChargeId}/{sHBA}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> CalculateFormulaById(string royaltyId, string salesChargeId, string sHBA, DataSourceLoadOptions loadOptions)
        {
            string tstatus = "";
            decimal resultValue = 0;
            string formula = "";
            dynamic retVal;

            double hbaValue = 0;
            bool result = double.TryParse(sHBA, out hbaValue);

            var record = await dbContext.sales_charge
                .Where(o => o.id == salesChargeId).FirstOrDefaultAsync();

            if (record != null)
            {
                formula = record.charge_formula.ToLower().Replace(" ", "").Replace(".royalty()", "");
            }
            else
            {
                tstatus = "Sales Charge not found";
                resultValue = -1;

                retVal = new
                {
                    status = tstatus,
                    value = resultValue
                };
                return retVal;
            }

            string xsyntax = formula;
            var lookupAnalyteSymbol = dbContext.vw_royalty_quantity_quality
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.royalty_id == royaltyId &&
                            o.analyte_symbol.Length > 0);
            foreach (vw_royalty_quantity_quality lookup in lookupAnalyteSymbol)
            {
                xsyntax = xsyntax.Replace((lookup.analyte_symbol.ToLower().Replace(" ", "")), lookup.analyte_value.ToString().Replace(",", "."));
            }

            //var pricing = dbContext.vw_royalty_pricing
            //    .Where(o => o.id == royaltyId).FirstOrDefault();
            //var hbaValue = pricing?.hba_value ?? 0;

            xsyntax = xsyntax.Replace("hbavalue", hbaValue.ToString());

            try
            {
                var engine1 = new Engine();
                var expression1 = engine1.Parse(xsyntax);
                //var resultText = expression1.Execute().ToString();
                //resultValue = Convert.ToDecimal(resultText, CultureInfo.CurrentCulture);

                System.Data.DataTable dt = new System.Data.DataTable();
                resultValue = Convert.ToDecimal(dt.Compute(xsyntax, ""));

                tstatus = "OK";
            }
            catch (Exception ex)
            {
                tstatus = ex.Message;
                resultValue = -1;
            }

            retVal = new
            {
                status = tstatus,
                value = resultValue
            };
            return retVal;
        }

        [HttpGet("SalesChargeIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SalesChargeIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_sales_charge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.charge_type_name.ToUpper() == "ROYALTY")
                    .Select(o => new { Value = o.id, Text = o.sales_charge_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}

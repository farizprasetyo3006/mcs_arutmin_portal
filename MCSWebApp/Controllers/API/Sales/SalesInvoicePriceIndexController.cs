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
using Microsoft.AspNetCore.Http;
using NLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataAccess.DTO;
using Omu.ValueInjecter;
using DataAccess.EFCore.Repository;
using Common;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/[controller]")]
    [ApiController]
    public class SalesInvoicePriceIndexController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SalesInvoicePriceIndexController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                return await DataSourceLoader.LoadAsync(dbContext.sales_invoice_detail
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
                dbContext.sales_invoice_detail.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(sales_invoice_detail),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var record = new sales_invoice_detail();
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

                    dbContext.sales_invoice_detail.Add(record);
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
                var record = dbContext.sales_invoice_detail
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
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
                var record = dbContext.sales_invoice_detail
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.sales_invoice_detail.Remove(record);
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

        [HttpGet("SalesInvoiceIdLookup")]
        public async Task<object> SalesInvoiceIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.sales_invoice
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.invoice_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("PriceIndexHistoryLookup")]
        public async Task<object> PriceIndexHistoryLookup(string despatch_order_id, string sales_contract_id, string sales_contract_term_id, string invoice_type_id, string quotation_type_id, DataSourceLoadOptions loadOptions)
        {
            const string stringQuotationPeriod_4LIBL = "4LIBL";
            const string stringQuotationPeriod_4LI1LD = "4LI1LD";
            const string stringQuotationPeriod_QBQBL = "QBQBL";
            const string stringQuotationPeriod_QBQLD = "QBQLD";
            const string stringPricingMethod_Calculated = "Calculated";
            const string stringPricingMethod_Fixed = "Fixed";
            //const string stringsalesChargeType_premium = "premium";
            //const string stringsalesChargeType_adjustment = "adjustment";

            const string stringIndexCalc_IF4 = "IF4";
            const string stringIndexCalc_EF3 = "EF3";
            const string stringIndexCalc_EF4 = "EF4";

            if (despatch_order_id is null && invoice_type_id is null && sales_contract_term_id is null)
            {
                return BadRequest("Shipping order is null");
                //return 9.9999;
            }

            //logger.Debug($"string key = {despatch_order_id} dan  {invoice_type_id}");

            //var splitInvoiceTypeId = invoice_type_id.Split('-');
            //var sales_contract_payment_term_id = splitInvoiceTypeId[0];
            //var real_invoice_type_id = splitInvoiceTypeId[1];

            // get how many payment term is listed in contract 
            var vw_payment_term_data = dbContext.vw_sales_contract_payment_term
                .Where(o => o.sales_contract_term_id == sales_contract_term_id)
                ;
            var array_vw_payment_term_data = await vw_payment_term_data.ToArrayAsync();
            if (array_vw_payment_term_data.Length == 0)
            {
                return BadRequest("no payment term is listed in " + array_vw_payment_term_data[0].contract_term_name);
            }

            //// get how many invoice already generated by despatch_order_id
            //var vw_sales_invoice_data = dbContext.vw_sales_invoice
            //    .Where(o => o.despatch_order_id == despatch_order_id);
            
            //var array_vw_sales_invoice_data = await vw_sales_invoice_data.ToArrayAsync();

            //var numberInvoicesCanBeIssued = array_vw_payment_term_data.Length - array_vw_sales_invoice_data.Length;
            //if (numberInvoicesCanBeIssued == 0)
            //{
            //    return BadRequest("All invoice had already issued from Shipping order " + array_vw_sales_invoice_data[0].despatch_order_number);
            //}

            // Get Invoice Type Data
            //var invoice_type_data = await dbContext.master_list
            //.Where(o => o.id == real_invoice_type_id).FirstOrDefaultAsync();

            List<SalesPrice> arrayRetVal = new List<SalesPrice>();

            var bill_of_lading = new System.DateTime(2020, 3, 30);
            var laycan_date = new System.DateTime(2020, 3, 30);
            var invoice_date = new System.DateTime(2020, 3, 30);
            //var t_quotation_price = 0.0;

            var record_despatch_order = await dbContext.despatch_order
                .Where(o => o.id == despatch_order_id)
                .FirstOrDefaultAsync();

            laycan_date = (System.DateTime)record_despatch_order.laycan_start;
           // decimal doQuantity = 0;
            var cow = await dbContext.draft_survey.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                && o.despatch_order_id == despatch_order_id).FirstOrDefaultAsync();
            if (cow != null)
            {
                // doQuantity = cow.quantity ?? 0;
                if (cow.bill_lading_date != null)
                {
                    bill_of_lading = (System.DateTime)cow.bill_lading_date;
                }
            }


            //var doQuantity = record_despatch_order.quantity;
            //string the_contract_term_id = record_despatch_order.contract_term_id;
            //string the_contract_product_id = record_despatch_order.contract_product_id;
            //var v_vessel_id = record_despatch_order.vessel_id;


            // Find Quotation Type using Invoice Type Name

            var records_sales_contract_quotation_price = dbContext.vw_sales_contract_quotation_price
                .Where(o => o.sales_contract_term_id == record_despatch_order.contract_term_id  //the_contract_term_id
                && o.quotation_type_id == quotation_type_id);
            var arraySalesContractQuotationPrice = await records_sales_contract_quotation_price.ToArrayAsync();

            try
            {
                // menghitung data quotation price
            //    t_quotation_price = 0.0;
             //   var total_weightening_value = 0;
                foreach (vw_sales_contract_quotation_price scqp in arraySalesContractQuotationPrice)
                {
                 //   total_weightening_value += (int)scqp.weightening_value;

                    if (scqp.pricing_method_in_coding == stringPricingMethod_Calculated)
                    {

                        if (scqp.price_index_id is null)
                        {
                            // t_quotation_price = 0.0;
                            return BadRequest("No data price index");
                        }
                        else
                        {
                            // check quotation_period,  if its 4LIBL , 4LI1LD , QBQBL , QBQLD
                            //var avg_price_index = 0.0;
                            //var total_price_index = 0.0;
                            //var count_price_include = 0;
                            //int[] arrayMonth = new int[1];
                            //List<int> listMonth = new List<int>();
                            var theDate = new System.DateTime(2020, 3, 30);
                            var errMsg = "";
                            switch (scqp.quotation_period)
                            {
                                case stringQuotationPeriod_4LIBL:
                                case stringQuotationPeriod_4LI1LD:

                                    if (scqp.quotation_period == stringQuotationPeriod_4LIBL)
                                    {
                                        theDate = bill_of_lading;
                                    }
                                    else
                                    {
                                        theDate = laycan_date;
                                    }
                                    //var records_price_index_history = dbContext.vw_price_index_history
                                    //    .Where(o => o.price_index_id == scqp.price_index_id
                                    //        && o.index_date.Value.AddDays(28) >= theDate
                                    //        && o.index_date.Value.AddDays(28) <= theDate.AddDays(28))
                                    //    .OrderByDescending(o => o.index_date);

                                    string datename = theDate.ToString("dddd");
                                    int dayRange = 28;
                                    int recTake = 5;
                                    if (datename == "Friday")
                                    {
                                        if (scqp.index_calculation_code == stringIndexCalc_EF4)
                                        {
                                            dayRange = 27;
                                            recTake = 4;
                                        }
                                        else if (scqp.index_calculation_code == stringIndexCalc_EF3)
                                        {
                                            dayRange = 27;
                                            recTake = 3;
                                        }
                                        else
                                        {
                                            dayRange = 28;
                                            recTake = 4;
                                        }
                                    }

                                    return await DataSourceLoader.LoadAsync(
                                            dbContext.vw_price_index_history
                                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                                && o.price_index_id == scqp.price_index_id
                                            && o.index_date.Value.AddDays(28) >= theDate
                                            && o.index_date.Value.AddDays(28) <= theDate.AddDays(dayRange))
                                                 .OrderByDescending(o => o.index_date)
                                                .Take(recTake),
                                            loadOptions);

                                //    if (records_price_index_history.Count() < 4 || records_price_index_history.Count() > 5)
                                //    {
                                //        errMsg = "Price is not available. Number of index price from " + records_price_index_history.FirstOrDefault().price_index_name + " in 28-days-range from " + theDate.ToString() + " is not 4";
                                //        //avg_price_index = 0;
                                //        //t_quotation_price = 0;
                                //        break;
                                //    }
                                //    var array_price_index_history = await records_price_index_history.ToArrayAsync();
                                //    foreach (vw_price_index_history item2 in array_price_index_history)
                                //    {
                                //        if ((theDate <= item2.index_date.Value.AddDays(28)) && (count_price_include < 4))
                                //        {
                                //            total_price_index += (float)item2.index_value;
                                //            count_price_include++;
                                //        }
                                //    }
                                //    //if (count_price_include != 4)
                                //    //{
                                //    //    errMsg = "Price is not available. Number of index price in 28-days-range from " + theDate.ToString() + " is not 4";
                                //    //    avg_price_index = 0;
                                //    //    t_quotation_price = 0;
                                //    //    break;
                                //    //}
                                //    avg_price_index = total_price_index / count_price_include;
                                //    t_quotation_price += avg_price_index * (double)scqp.weightening_value / 100;
                                //    break;

                                case stringQuotationPeriod_QBQBL:
                                case stringQuotationPeriod_QBQLD:
                                    if (scqp.quotation_period == stringQuotationPeriod_QBQBL)
                                    {
                                        theDate = bill_of_lading;
                                    }
                                    else
                                    {
                                        theDate = laycan_date;
                                    }
                                    //var records_price_index_history_quarter = dbContext.price_index_history
                                    //    .Where(o => o.price_index_id == scqp.price_index_id).OrderByDescending(o => o.index_date)
                                    //    ;

                                    return await DataSourceLoader.LoadAsync(
                                           dbContext.vw_price_index_history
                                               .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                               && o.price_index_id == scqp.price_index_id)
                                                .OrderByDescending(o => o.index_date),
                                           loadOptions);

                                //    var array_price_index_history_quarter = await records_price_index_history_quarter.ToArrayAsync();
                                //    switch (theDate.Month)
                                //    {
                                //        case 1:
                                //        case 2:
                                //        case 3:
                                //            listMonth.Add(10);
                                //            listMonth.Add(11);
                                //            listMonth.Add(12);
                                //            break;
                                //        case 4:
                                //        case 5:
                                //        case 6:
                                //            listMonth.Add(1);
                                //            listMonth.Add(2);
                                //            listMonth.Add(3);
                                //            break;
                                //        case 7:
                                //        case 8:
                                //        case 9:
                                //            listMonth.Add(4);
                                //            listMonth.Add(5);
                                //            listMonth.Add(6);
                                //            break;
                                //        case 10:
                                //        case 11:
                                //        case 12:
                                //            listMonth.Add(7);
                                //            listMonth.Add(8);
                                //            listMonth.Add(9);
                                //            break;
                                //        default:
                                //            listMonth.Add(0);
                                //            break;
                                //    }
                                //    arrayMonth = listMonth.ToArray();
                                //    foreach (price_index_history item2 in array_price_index_history_quarter)
                                //    {
                                //        if ((item2.index_date.Year == theDate.Year) && (arrayMonth.Contains(item2.index_date.Month)))
                                //        {
                                //            total_price_index += (float)item2.index_value;
                                //            count_price_include++;
                                //        }
                                //    }
                                //    avg_price_index = total_price_index / count_price_include;
                                //    t_quotation_price += avg_price_index * (double)scqp.weightening_value / 100;
                                //    break;

                                default: break;
                            } // end of switch:
                            if (errMsg.Length > 0)
                            {
                                arrayRetVal.Add(new SalesPrice("error", errMsg, -1));
                                //return arrayRetVal;
                                return BadRequest(errMsg);
                            }
                            //t_quotation_price = Math.Round(t_quotation_price, (int)scqp.decimal_places);
                        } // end of else:  price_index_id is not null

                    }
                    else if (scqp.pricing_method_in_coding == stringPricingMethod_Fixed)
                    {
                        return BadRequest("No price index");
                        //t_quotation_price += (double)(double)scqp.price_value * (double)scqp.weightening_value / 100;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug($"got exception when count quotationprice = {e.Message}");
            }

            //try
            //{
            //    // menghitung sales charge jika Premium atau adjustment 
            //    var records_vw_sales_contract_charge = dbContext.vw_sales_contract_charges
            //        .Where(o => o.sales_contract_term_id == the_contract_term_id)
            //        ;
            //}
            //catch (Exception e)
            //{
            //    logger.Debug($"got exception when parsing prerequisite = {e.Message}");
            //}

            //arrayRetVal.Add(new SalesPrice("Quotation Price", "quotation_price", t_quotation_price));
            //double totalInvoice = 0.0;
        

            return arrayRetVal;
        }

    }
}

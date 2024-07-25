using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using DataAccess.EFCore;
using NLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataAccess.DTO;
using Omu.ValueInjecter;
using Microsoft.EntityFrameworkCore;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using BusinessLogic;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using Common;
using FastReport.Utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.SS.Formula.Functions;
using System.Threading;
using DocumentFormat.OpenXml.Wordprocessing;

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class ShipmentPlanController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        public ShipmentPlanController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            DateTime date1 = DateTime.ParseExact(tanggal1, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(tanggal2, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

            string year1 = date1.Year.ToString();
            int month1 = date1.Month;
            string year2 = date2.Year.ToString();
            int month2 = date2.Month;

            var yearParam1 = "";
            var master_list1 = dbContext.master_list.Where(o => o.item_name == year1).FirstOrDefault();
            if (master_list1 != null) yearParam1 = master_list1.id.ToString();

            var yearParam2 = "";
            var master_list2 = dbContext.master_list.Where(o => o.item_name == year2).FirstOrDefault();
            if (master_list2 != null) yearParam2 = master_list1.id.ToString();

            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_shipment_plan
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.vw_shipment_plan
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.shipment_year == yearParam1 && o.month_id >= month1 && o.month_id <= month2 || o.shipment_year == yearParam2 && o.month_id >= month1 && o.month_id <= month2),
                    loadOptions);
        }

        [HttpGet("DataGrid")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions, string latestUpdate)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_shipment_plan
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
                        //   .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("SalesContractIdLookup")]
        public async Task<object> SalesContractLookup(DataSourceLoadOptions loadOptions, string CustomerId)
        {
            try
            {
                if (CustomerId != null)
                {
                    var lookup = dbContext.sales_contract
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.customer_id == CustomerId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.sales_contract_name });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.sales_contract
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Select(o => new { Value = o.id, Text = o.sales_contract_name });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        [HttpGet("ShipmentPlanByID")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ShipmentPlanByID(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_shipment_plan.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Id),
                loadOptions);
        }

        [HttpGet("SalesPlanDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SalesPlanDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_sales_plan_customer_list.Where(o => o.id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)),
                loadOptions);
        }

        [HttpGet("TransportIdLookup")]
        public async Task<object> TransportIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_transport
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("TransportByIncoterm")]
        public async Task<object> TransportByIncoterm(string Incoterm, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var IncotermName = dbContext.master_list
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.id == Incoterm)
                    .Select(o => new { o.item_name }).FirstOrDefault().item_name ?? "";

                var searchTerm = "";
                if (IncotermName.Contains("TRUCK")) { searchTerm = "DT-"; }
                if (IncotermName.Contains("VESSEL")) { searchTerm = "MV"; }
                if (IncotermName.Contains("BARGE")) { searchTerm = "BG-"; }

                var lookup = await dbContext.transport
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(x => x.vehicle_name.ToUpper().Contains(searchTerm))
                    .Select(o => new { Value = o.id, Text = o.vehicle_name }).ToListAsync();
                return DataSourceLoader.Load(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ShipmentPlanIdLookup")]
        public async Task<object> ShipmentPlanIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_shipment_plan
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.shipment_code + " (" + o.business_partner_name + " - " + o.sales_contract_name + ")" });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("MonthIndexLookup")]
        public object MonthIndexLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var months = new Dictionary<int, string>();
                for (var i = 1; i <= 12; i++)
                {
                    months.Add(i, i.ToString("00") + " " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToUpper());
                }
                var lookup = months
                    .Select(o => new { Value = o.Key, Text = o.Value });
                return DataSourceLoader.Load(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("YearIdLookup")]
        public object YearIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var yearlist = new Dictionary<int, int>();
                for (var i = 2022; i <= 2040; i++)
                {
                    yearlist.Add(i, i);
                }
                var lookup = yearlist
                    .Select(o => new { Value = o.Key, Text = o.Value });
                return DataSourceLoader.Load(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DespatchDemurageLookUp")]
        public async Task<object> DespatchDemurageLookUp(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {

                var lookup = dbContext.vw_sales_plan_customer_list
     .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
     .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
     .Join(dbContext.sales_contract_term,
         salesPlan => salesPlan.sales_contract_id,
         salesContractTerm => salesContractTerm.sales_contract_id,
         (salesPlan, salesContractTerm) => new
         {
             SalesPlan = salesPlan,
             SalesContractTerm = salesContractTerm
         })
     .Join(dbContext.sales_contract_despatch_demurrage_term,
         combined => combined.SalesContractTerm.id,
         despatchDemurageTerm => despatchDemurageTerm.sales_contract_term_id,
         (combined, despatchDemurageTerm) => new
         {
             Value = combined.SalesPlan.id,
             despatchDemurageTerm.loading_rate_geared,
             despatchDemurageTerm.rate // Kolom loading_rate_geared dari tabel sales_contract_despatch_demurage_term
                                       // Add other columns from different tables as needed
         });

                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("GenerateReport")]
        public object GenerateReport()
        {
            //try
            //{
            //    var temp = dbContext.vw_shipment_plan.Where(x => x.year == DateTime.Now.Year);
            //    List<vw_shipment_plan> result = new List<vw_shipment_plan>();
            //    int ctr = 1;
            //    double monthTemp = 1;
            //    foreach(var item in temp)
            //    {
            //        if(item.month != monthTemp)
            //        {
            //            monthTemp = (double)item.month;
            //            ctr = 1;
            //            item.no = ctr.ToString();
            //        }
            //        else
            //        {
            //            item.no = ctr.ToString();
            //            ctr++;
            //        }
            //        result.Add(item);
            //    }
            //    var tempForecast = dbContext.shipment_forecast.Where(x => x.year == DateTime.Now.Year);
            //    if(tempForecast != null && tempForecast.Count() > 0)
            //    {
            //        foreach (var item in tempForecast)
            //        {
            //            vw_shipment_plan vwTemp = new vw_shipment_plan
            //            {
            //                id = item.id
            //                , year = (double)item.year
            //                , month = (double)item.month
            //                , no = item.no
            //                , customer_id = item.customer_id
            //                , business_partner_name = item.business_partner_name
            //                , country_name = item.country_name
            //                , shipment_no = item.shipment_no
            //                , total_shipment = (long)item.total_shipment
            //                , delivery_term = item.delivery_term
            //                , vessel_id = item.vessel_id
            //                , vessel = item.vessel
            //                , laycan_start = item.laycan_start
            //                , laycan_end = item.laycan_end
            //                , order_reference_date = item.order_reference_date
            //                , quantity_plan = item.quantity_plan
            //                , quantity_actual = item.quantity_actual
            //                , eta = item.eta
            //                , comm_date = item.comm_date
            //                , bl_date = item.bl_date
            //                , remark = item.remark
            //                , traffic_officer = item.traffic_officer
            //                , payment_method = item.payment_method
            //                , destination_bank = item.destination_bank
            //                , invoice_ref = item.invoice_ref
            //                , invoice_amount = item.invoice_amount
            //                , invoice_date = item.invoice_date
            //                , invoice_price = item.invoice_price
            //                , exchange_rate = item.exchange_rate
            //                , si_currency_id = item.si_currency_id
            //                , scpt_currency_id = item.scpt_currency_id
            //                , invoice_due_date = item.invoice_due_date
            //                , tax_invoice_ref_no = item.tax_invoice_ref_no
            //                , payment_receiving_date = item.payment_receiving_date
            //                , amount_received = item.amount_received
            //            };
            //            result.Add(vwTemp);
            //        }

            //    }
            //    //temp = result.AsQueryable();
            //    return result.OrderBy(x => x.month);
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(ex.Message);
            //}
            return null;
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(shipment_plan),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var record = new shipment_plan();
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

                    dbContext.shipment_plan.Add(record);
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
                var record = dbContext.shipment_plan
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

                        //record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        if (record.certain == null)
                        {
                            record.certain = false;
                        }
                        record.lineup_number = await GenerateLineupNumber(record.product_id,record.sales_contract_id,record.customer_id,record.qty_sp,record.lineup_number);
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
        string FormatQuantityThreshold(int quantity)
        {
            const int threshold = 1000;

            if (quantity >= threshold)
            {
                double formattedValue = quantity / 1000.0;
                return $"{formattedValue:0.#}K";
            }

            return quantity.ToString();
        }
        private async Task<string> GenerateLineupNumber(string product_id,string sales_contract_id,string customer_id,decimal? qty_sp,string lineup_number)
        {
            string code = string.Empty;
            var product_name = await dbContext.product.Where(o => o.id == product_id).Select(o => o.product_name).FirstOrDefaultAsync();
            var contract_term = await dbContext.sales_contract_term.Where(o => o.id == sales_contract_id).FirstOrDefaultAsync();
            var customer = await dbContext.customer.Where(o => o.id == customer_id).Select(o => o.alias_name).FirstOrDefaultAsync();
            var date = "";
            if (contract_term != null)
            {
                date = contract_term.start_date.Value.ToString("MMMyy") + contract_term.end_date.Value.ToString("MMMyy");
            }
            var quantityFormatted = FormatQuantityThreshold(Convert.ToInt32(qty_sp));
            var shippingProgramNumber = lineup_number.Split('-');
            // Split the string based on '-' and get the last part
            //string r = shippingProgramNumber.LastOrDefault();
            var conn = dbContext.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            if (conn.State == System.Data.ConnectionState.Open)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT nextval('seq_shipping_program_number')";
                    var r = await cmd.ExecuteScalarAsync();
                    r = Convert.ToInt32(r).ToString("D6");
                    code = $"LU-{customer}-{shippingProgramNumber[2]}-{product_name}-{shippingProgramNumber[4]}-{date}-{quantityFormatted}-{r}";
                }
            }
           // var code = $"LU-{customer}-{shippingProgramNumber[2]}-{product_name}-{shippingProgramNumber[4]}-{date}-{quantityFormatted}-{r}";
            return code;
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var despatch_order = dbContext.despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.despatch_plan_id == key).FirstOrDefault();
                if (despatch_order != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var record = dbContext.shipment_plan
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.shipment_plan.Remove(record);
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
        private string FormatQuantity(int quantity)
        {
            const int threshold = 1000;

            if (quantity >= threshold)
            {
                double formattedValue = quantity / 1000.0;
                return $"{formattedValue:0.#}K";
            }

            return quantity.ToString();
        }
        [HttpPost("DeleteSelectedRows")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteSelectedRows([FromBody] dynamic Data)
        {
            var result = new StandardResult();
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (Data != null && Data.selectedIds != null)
                    {
                        var selectedIds = ((string)Data.selectedIds)
                            .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                        var despatch_order = dbContext.despatch_order
                                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                            && selectedIds.Contains(o.despatch_plan_id)).ToList();
                        if (despatch_order != null) throw new Exception("Can not be deleted since it is already have one or more transactions.");

                        foreach (string key in selectedIds)
                        {
                            var record = dbContext.shipment_plan
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.shipment_plan.Remove(record);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                        await tx.CommitAsync();
                        result.Success = true;
                        return Ok(result);
                    }
                    else
                    {
                        result.Message = "Invalid data.";
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    result.Message = ex.Message;
                }
            }

            return new JsonResult(result);
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] shipment_plan Record)
        {
            try
            {
                var record = dbContext.shipment_plan
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
                        return Ok(record);
                    }
                    else
                    {
                        return BadRequest("User is not authorized.");
                    }
                }
                else if (await mcsContext.CanCreate(dbContext, nameof(shipment_plan),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new shipment_plan();
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

                    dbContext.shipment_plan.Add(record);
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

        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.vw_shipment_plan
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if (record != null)
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

        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Trace($"string Id = {Id}");

            if (await mcsContext.CanDelete(dbContext, Id, CurrentUserContext.AppUserId)
                || CurrentUserContext.IsSysAdmin)
            {
                try
                {
                    var record = dbContext.shipment_plan
                        .Where(o => o.id == Id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        dbContext.shipment_plan.Remove(record);
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
            else
            {
                return BadRequest("User is not authorized.");
            }
        }

        [HttpGet("FetchShippingProgramToShipmentPlan")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ApiResponse> FetchShippingProgramToShipmentPlan()
        {
            var result = new ApiResponse();
            result.Status.Success = true;
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                var count = 0;
                await _semaphoreSlim.WaitAsync();
                try
                {
                    List<master_list> ml = await dbContext.master_list.Where(o => o.item_group == "tipe-penjualan").ToListAsync();
                    List<sales_contract_despatch_demurrage_term> r = await dbContext.sales_contract_despatch_demurrage_term.ToListAsync();
                    List<shipment_plan> sps = await dbContext.shipment_plan.ToListAsync();                                                                   
                    var shippingPrograms = await dbContext.shipping_program.ToListAsync(); 

                    foreach (var item in shippingPrograms)
                    {
                        count++;
                        var code = item.shipping_program_number.Replace("SP-", "LU-");
                        var current = sps
                            .Where(o => o.shipping_program_id == item.id).FirstOrDefault();
                        if (current != null)
                        {
                            if(count == 49)
                            {
                                string a = "a";
                            }
                            if (current.certain != true)
                            {
                                current.modified_by = null;
                                current.modified_on = null;
                                current.month_id = item.month_id;
                                current.shipment_year = item.plan_year_id;
                                current.qty_sp = item.quantity;
                                current.product_id = item.product_category_id;
                                current.customer_id = item.customer_id;
                                current.sales_contract_id = item.sales_contract_id;
                                current.loading_port = item.source_coal_id;
                                current.shipping_program_id = item.id;
                                current.vessel_id = item.tipe_penjualan_id;
                                current.end_user = item.end_user_id;

                                if (!String.IsNullOrEmpty(current.vessel_id))
                                {
                                    var quantity = r.Where(o => o.sales_contract_term_id == current.sales_contract_id).FirstOrDefault();
                                    var type = ml.Where(o => o.id == current.vessel_id).FirstOrDefault();
                                    if (quantity != null && type != null && type.item_in_coding == "MV G&G")
                                    {
                                        current.loading_rate = quantity.loading_rate_geared;
                                    }
                                    else if (quantity != null && type != null && type.item_in_coding == "MV GL")
                                    {
                                        current.loading_rate = quantity.loading_rate_gearless;
                                    }
                                    else if (quantity != null && type != null && type.item_in_coding == "NPLCT")
                                    {
                                        current.loading_rate = quantity.loading_rate_nplct;
                                    }
                                    else
                                    {
                                        current.loading_rate = 0;
                                    }
                                }
                                current.lineup_number = await GenerateLineupNumber(current.product_id,current.sales_contract_id,current.customer_id,current.qty_sp,item.shipping_program_number);

                                await dbContext.SaveChangesAsync();
                            }
                        }
                        else if (current == null)
                        {
                            var sp = new shipment_plan();

                            sp.id = Guid.NewGuid().ToString("N");
                            sp.created_by = CurrentUserContext.AppUserId;
                            sp.created_on = DateTime.Now;
                            sp.modified_by = null;
                            sp.modified_on = null;
                            sp.is_active = true;
                            sp.is_default = null;
                            sp.is_locked = null;
                            sp.entity_id = null;
                            sp.owner_id = CurrentUserContext.AppUserId;
                            sp.organization_id = CurrentUserContext.OrganizationId;
                            sp.business_unit_id = item.business_unit_id;

                            sp.modified_by = CurrentUserContext.AppUserId;
                            sp.modified_on = DateTime.Now;
                            sp.month_id = item.month_id;
                            sp.shipment_year = item.plan_year_id;
                            sp.qty_sp = item.quantity;
                            sp.declared_month_id = item.declared_month_id;
                            sp.product_id = item.product_category_id;
                            sp.customer_id = item.customer_id;
                            sp.sales_contract_id = item.sales_contract_id;
                            sp.loading_port = item.source_coal_id;
                            sp.shipping_program_id = item.id;
                            sp.vessel_id = item.tipe_penjualan_id;
                            sp.certain = null;
                            sp.end_user = item.end_user_id;

                            if (!String.IsNullOrEmpty(sp.vessel_id))
                            {
                                var quantity = r.Where(o => o.sales_contract_term_id == sp.sales_contract_id).FirstOrDefault();
                                var type = ml.Where(o => o.id == sp.vessel_id).FirstOrDefault();
                                if (quantity != null && type != null && type.item_in_coding == "MV G&G")
                                {
                                    sp.loading_rate = quantity.loading_rate_geared;
                                }
                                else if (quantity != null && type != null && type.item_in_coding == "MV GL")
                                {
                                    sp.loading_rate = quantity.loading_rate_gearless;
                                }
                                else if (quantity != null && type != null && type.item_in_coding == "NPLCT")
                                {
                                    sp.loading_rate = quantity.loading_rate_nplct;
                                }
                                else
                                {
                                    sp.loading_rate = 0;
                                }
                            }
                            sp.lineup_number = await GenerateLineupNumber(sp.product_id, sp.sales_contract_id, sp.customer_id, sp.qty_sp, item.shipping_program_number); ;

                            dbContext.shipment_plan.Add(sp);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    if (result.Status.Success)
                    {
                        await tx.CommitAsync();
                        result.Status.Message = "Ok";
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    result.Status.Success = false;
                    result.Status.Message = ex.InnerException?.Message ?? ex.Message;
                    return result;
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
            }

        [HttpGet("GetSalesPlanCustomer")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetSalesPlanCustomer(string id)
        {
            var result = await dbContext.vw_sales_plan_customer
                .Where(x => x.id.Equals(id)).FirstOrDefaultAsync();
            return result;
        }

        [HttpGet("GetSalesPlanCustomerList")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetSalesPlanCustomerList(string id)
        {
            var result = await dbContext.vw_sales_plan_customer_list
                .Where(x => x.id.Equals(id)).FirstOrDefaultAsync();
            return result;
        }

        [HttpGet("GetCoalBrand")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetCoalBrand(string id)
        {
            var result1 = await dbContext.sales_plan_customer
                .Where(x => x.id == id).FirstOrDefaultAsync();
            var result2 = await dbContext.sales_contract
                .Where(x => x.id == result1.sales_contract_id).FirstOrDefaultAsync();
            var result3 = await dbContext.sales_contract_term
                .Where(x => x.sales_contract_id == result2.id).FirstOrDefaultAsync();
            var result = await dbContext.sales_contract_product
                .Where(x => x.sales_contract_term_id == result3.id).FirstOrDefaultAsync();
            return result;
        }

        [HttpPost("UploadDocument")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UploadDocument([FromBody] dynamic FileDocument)
        {
            var result = new StandardResult();
            long size = 0;

            if (FileDocument == null)
            {
                return BadRequest("No file uploaded!");
            }

            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);

            var fileName = (string)FileDocument.filename;
            FilePath += $@"\{fileName}";

            string strfile = (string)FileDocument.data;
            byte[] arrfile = Convert.FromBase64String(strfile);

            await System.IO.File.WriteAllBytesAsync(FilePath, arrfile);

            size = fileName.Length;
            string sFileExt = Path.GetExtension(FilePath).ToLower();

            ISheet sheet;
            dynamic wb;
            if (sFileExt == ".xls")
            {
                FileStream stream = System.IO.File.OpenRead(FilePath);
                wb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats
                sheet = wb.GetSheetAt(0); //get first sheet from workbook
                stream.Close();
            }
            else
            {
                wb = new XSSFWorkbook(FilePath); //This will read 2007 Excel format
                sheet = wb.GetSheetAt(0); //get first sheet from workbook
            }

            string teks = "";
            bool gagal = false; string errormessage = "";
            int count = 1;
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    #region Variables
                    var szLineCode = string.Empty;
                    var szCustomerId = string.Empty;
                    var szContractId = string.Empty;
                    var szYearId = string.Empty;
                    var iMonth = 0;
                    var szProductId = string.Empty;
                    var szDestination = string.Empty;
                    var szShipmentNo = string.Empty;
                    var szIncoterm = string.Empty;
                    var szTransportId = string.Empty;
                    DateTime? dtmLaycanStart = DateTime.Now;
                    DateTime? dtmLaycanEnd = DateTime.Now;
                    var bVesselType = false;
                    var szInvoiceNo = string.Empty;
                    var szRoyalty = string.Empty;
                    DateTime? dtmETA = DateTime.Now;
                    decimal decContracTon = 0;
                    var szBusinessUnit = string.Empty;
                    var szRemark = string.Empty;
                    decimal decLoadContract = 0;
                    decimal decLoadStandard = 0;
                    decimal decDemUsd = 0;
                    string iDeclaredMonth = string.Empty;
                    bool? szCertain = null;
                    DateTime? dtmNora = DateTime.Now;
                    DateTime? dtmEtb = DateTime.Now;
                    DateTime? dtmEtc = DateTime.Now;
                    var szPlnSchedule = string.Empty;
                    var szOriginSchedule = string.Empty;
                    var szLoadingPortId = string.Empty;
                    DateTime? dtmEtaDisc = DateTime.Now;
                    DateTime? dtmEtbDisc = DateTime.Now;
                    DateTime? dtmEtcCommemceDisc = DateTime.Now;
                    DateTime? dtmEtcCompleteDisc = DateTime.Now;
                    decimal decStowPlan = 0;
                    var szTransportType = string.Empty;
                    var szLoadportAgent = string.Empty;
                    decimal decHPBForecast = 0;
                    #endregion

                    #region Lineup Code
                    var resultLineCode = await dbContext.shipment_plan
                        .Where(x => x.lineup_number == PublicFunctions.IsNullCell(row.GetCell(0))
                        && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                    if (resultLineCode != null)
                    {
                        szLineCode = resultLineCode.lineup_number;
                    }
                    else
                    {
                        szLineCode = PublicFunctions.IsNullCell(row.GetCell(0));
                    }
                        #endregion

                        #region Cutomer
                        var resultCustomer = await dbContext.customer
                            .Where(x => x.business_partner_name == PublicFunctions.IsNullCell(row.GetCell(1))
                            && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                    if (resultCustomer != null) szCustomerId = resultCustomer.id;
                    #endregion

                    #region Sales Contract Term Name
                    var resultContractTerm = await dbContext.sales_contract_term
                        .Where(x => x.contract_term_name.ToUpper().Trim() == PublicFunctions.IsNullCell(row.GetCell(2)).ToUpper().Trim()
                        && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                    if (resultContractTerm != null)
                    {
                        szContractId = resultContractTerm.id;
                    }
                    else
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Sales Contract Term Cannot be null / Not Found" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                    }
                    #endregion

                    #region Year
                    var resultYear = await dbContext.master_list
                        .Where(x => x.item_name == PublicFunctions.IsNullCell(row.GetCell(3))
                        && x.item_group == "years".ToLower()
                        && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                    if (resultYear != null) szYearId = resultYear.id;
                    #endregion

                    #region Month
                    var resultMonth = PublicFunctions.IsNullCell(row.GetCell(4)).ToLower();
                    switch (resultMonth.ToLower())
                    {
                        case "january":
                            iMonth = 1;
                            break;
                        case "february":
                            iMonth = 2;
                            break;
                        case "march":
                            iMonth = 3;
                            break;
                        case "april":
                            iMonth = 4;
                            break;
                        case "may":
                            iMonth = 5;
                            break;
                        case "june":
                            iMonth = 6;
                            break;
                        case "july":
                            iMonth = 7;
                            break;
                        case "august":
                            iMonth = 8;
                            break;
                        case "september":
                            iMonth = 9;
                            break;
                        case "october":
                            iMonth = 10;
                            break;
                        case "november":
                            iMonth = 11;
                            break;
                        case "december":
                            iMonth = 12;
                            break;
                        default:
                            break;
                    }
                    #endregion

                    #region Product
                    var resultProduct = await dbContext.product
                        .Where(x => x.product_name == PublicFunctions.IsNullCell(row.GetCell(5))
                        && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                    if (resultProduct != null)
                    {
                        szProductId = resultProduct.id;
                    }
                    else
                    {
                        teks += "Error in Line : " + (i + 1) + " ==> Product Cannot be null / Not Found" + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                    } 
                    #endregion

                    #region Destination
                    szDestination = PublicFunctions.IsNullCell(row.GetCell(6));
                    #endregion

                    #region Shipment Number
                    szShipmentNo = PublicFunctions.IsNullCell(row.GetCell(7));
                    #endregion

                    #region Incoterm
                    var resultIncoterm = await dbContext.master_list
                        .Where(x => x.item_name == PublicFunctions.IsNullCell(row.GetCell(8))
                        && x.item_group == "delivery-term".ToLower()
                        && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                    if (resultIncoterm != null) szIncoterm = resultIncoterm.id;
                    #endregion

                    #region Transport Barge / Vessel
                    var szTransId = PublicFunctions.IsNullCell(row.GetCell(9));
                    var resultBarge = await dbContext.barge
                        .Where(x => x.vehicle_name == szTransId
                        && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                    if (resultBarge != null) szTransportId = resultBarge.id;
                    else
                    {
                        var resultVessel = await dbContext.vessel
                            .Where(x => x.vehicle_name == szTransId
                            && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                        if (resultVessel != null) szTransportId = resultVessel.id;
                    }
                    #endregion

                    #region Laycan Start

                    dtmLaycanStart = PublicFunctions.TanggalNull(row.GetCell(10));

                    #endregion

                    #region Laycan End

                    dtmLaycanEnd = PublicFunctions.TanggalNull(row.GetCell(11));

                    #endregion

                    // Laycan Status
                    string laycanStatusId = "";
                    var laycanStatus = await dbContext.master_list
                        .Where(x => x.item_name == PublicFunctions.IsNullCell(row.GetCell(12))
                            && x.item_group == "time-status"
                            && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                    if (laycanStatus != null) laycanStatusId = laycanStatus.id;

                    #region Vessel Type

                    var resultVesselType = PublicFunctions.IsNullCell(row.GetCell(13)).ToLower();
                    if (resultVesselType == "g&g") bVesselType = true;

                    #endregion

                    #region Invoice No

                    szInvoiceNo = PublicFunctions.IsNullCell(row.GetCell(14));

                    #endregion

                    #region Royalty

                    szRoyalty = PublicFunctions.IsNullCell(row.GetCell(15));

                    #endregion

                    #region ETA

                    dtmETA = PublicFunctions.TanggalNull(row.GetCell(16));

                    #endregion

                    // ETA Status
                    string ETAStatusId = "";
                    var ETAStatus = await dbContext.master_list
                        .Where(x => x.item_name == PublicFunctions.IsNullCell(row.GetCell(17))
                            && x.item_group == "time-status"
                            && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                    if (ETAStatus != null) ETAStatusId = ETAStatus.id;

                    #region Contract Tonnage

                    decContracTon = PublicFunctions.Desimal(row.GetCell(18));

                    #endregion

                    #region Business Unit

                    var resultBusinessUnit = await dbContext.business_unit
                        .Where(x => x.business_unit_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(19)).ToLower()
                        && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                    if (resultBusinessUnit != null) szBusinessUnit = resultBusinessUnit.id;

                    #endregion

                    #region Remarks

                    szRemark = PublicFunctions.IsNullCell(row.GetCell(20));

                    #endregion

                    #region Loaded Contract

                    decLoadContract = PublicFunctions.Desimal(row.GetCell(21));

                    #endregion

                    #region Loaded Standard

                    decLoadStandard = PublicFunctions.Desimal(row.GetCell(22));

                    #endregion

                    #region Dem USD

                    decDemUsd = PublicFunctions.Desimal(row.GetCell(23));

                    #endregion

                    #region Declared Month

                    iDeclaredMonth = PublicFunctions.IsNullCell(row.GetCell(24));

                    #endregion

                    #region Certain

                    var vCertain = PublicFunctions.IsNullCell(row.GetCell(25));
                    if (!string.IsNullOrEmpty(vCertain))
                        szCertain = Convert.ToBoolean(vCertain.Trim());

                    #endregion

                    #region Nora

                    dtmNora = PublicFunctions.TanggalNull(row.GetCell(26));

                    #endregion

                    #region ETB

                    dtmEtb = PublicFunctions.TanggalNull(row.GetCell(27));

                    #endregion

                    #region ETC

                    dtmEtc = PublicFunctions.TanggalNull(row.GetCell(28));
                    if(dtmEtc.Equals(new DateTime(1900, 1, 1, 0, 0, 0)) || dtmEtc.Equals(new DateTime(0001, 1, 1, 0, 0, 0)))

                    #endregion

                    #region PLN Schedule

                    szPlnSchedule = PublicFunctions.IsNullCell(row.GetCell(29));

                    #endregion

                    #region Original Schedule

                    szOriginSchedule = PublicFunctions.IsNullCell(row.GetCell(30));

                    #endregion

                    #region Loading Port

                    var resultLoadPort = await dbContext.port_location
                        .Where(x => x.stock_location_name.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(31)).ToUpper()
                        && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                    if (resultLoadPort != null) szLoadingPortId = resultLoadPort.id;

                    #endregion

                    #region ETA Disc

                    dtmEtaDisc = PublicFunctions.TanggalNull(row.GetCell(32));

                    #endregion

                    #region ETB Disc

                    dtmEtbDisc = PublicFunctions.TanggalNull(row.GetCell(33));

                    #endregion

                    #region ETC Commence Disc

                    dtmEtcCommemceDisc = PublicFunctions.TanggalNull(row.GetCell(34));

                    #endregion

                    #region ETC Completed Disc

                    dtmEtcCompleteDisc = PublicFunctions.TanggalNull(row.GetCell(35));

                    #endregion

                    #region Stow Plan

                    decStowPlan = PublicFunctions.Desimal(row.GetCell(36));

                    #endregion

                    #region Transport Type
                    var transportType = await dbContext.master_list
                        .Where(x => x.item_name == PublicFunctions.IsNullCell(row.GetCell(37))
                        && x.item_group == "tipe-penjualan"
                        && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefaultAsync();
                    if (transportType != null) szTransportType = transportType.id;
                    #endregion
                    #region Dem USD

                    decHPBForecast = PublicFunctions.Desimal(row.GetCell(41));

                    #endregion
                    var fc = "";
                    var contractor = await dbContext.contractor
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(38)).ToLower())
                        .FirstOrDefaultAsync();
                    if (contractor != null) fc = contractor.id.ToString();

                    var transport_provider = "";
                    var contractor1 = await dbContext.contractor
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(39)).ToLower())
                        .FirstOrDefaultAsync();
                    if (contractor1 != null) transport_provider = contractor1.id.ToString();

                    #region Loadport Agent

                    szLoadportAgent = PublicFunctions.IsNullCell(row.GetCell(40));

                    #endregion

                    var record = dbContext.shipment_plan
                        .Where(o => o.lineup_number.ToLower() == szLineCode.ToLower()
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();

                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.customer_id = szCustomerId;
                        record.sales_contract_id = szContractId;
                        record.shipment_year = szYearId;
                        record.month_id = iMonth;
                        record.product_id = szProductId;
                        record.destination = szDestination;
                        record.shipment_number = szShipmentNo;
                        record.incoterm = szIncoterm;
                        record.transport_id = szTransportId;
                        record.laycan_start = dtmLaycanStart;
                        record.laycan_end = dtmLaycanEnd;
                        record.is_geared = bVesselType;
                        record.invoice_no = szInvoiceNo;
                        record.royalti = szRoyalty;
                        record.eta = dtmETA;
                        record.qty_sp = decContracTon;
                        record.business_unit_id = szBusinessUnit;
                        record.remarks = szRemark;
                        record.loading_rate = decLoadContract;
                        record.loading_standart = decLoadStandard;
                        record.despatch_demurrage_rate = decDemUsd;
                        record.declared_month_id = iDeclaredMonth;
                        record.certain = szCertain;
                        record.nora = dtmNora;
                        record.etb = dtmEtb;
                        record.etc = dtmEtc;
                        record.pln_schedule = szPlnSchedule;
                        record.original_schedule = szOriginSchedule;
                        record.loading_port = szLoadingPortId;
                        record.eta_disc = dtmEtaDisc;
                        record.etb_disc = dtmEtbDisc;
                        record.etcommence_disc = dtmEtcCommemceDisc;
                        record.etcompleted_disc = dtmEtcCompleteDisc;
                        record.stow_plan = decStowPlan;
                        record.vessel_id = szTransportType;
                        record.fc_provider_id = fc;
                        record.transport_provider_id = transport_provider;
                        record.loadport_agent = szLoadportAgent;
                        record.laycan_status = laycanStatusId;
                        record.eta_status = ETAStatusId;
                        record.hpb_forecast = decHPBForecast;
                        #region update lineup code
                        var product_name = dbContext.product.Where(o => o.id == record.product_id).Select(o => o.product_name).FirstOrDefault();
                        //var item_name = dbContext.master_list.Where(o => o.id == record.tipe_penjualan_id).Select(o => o.item_name).FirstOrDefault();
                        var contract_term = dbContext.sales_contract_term.Where(o => o.id == record.sales_contract_id).FirstOrDefault();
                        var customer = dbContext.customer.Where(o => o.id == record.customer_id).Select(o => o.alias_name).FirstOrDefault();
                        var date = "";
                        if (contract_term != null)
                        {
                            date = contract_term.start_date.Value.ToString("MMMyy") + contract_term.end_date.Value.ToString("MMMyy");
                        }
                        var quantityFormatted = FormatQuantity(Convert.ToInt32(record.qty_sp));
                        var shippingProgramNumber = record.lineup_number.Split('-');
                        // Split the string based on '-' and get the last part
                        string r = shippingProgramNumber.LastOrDefault();

                        #endregion
                        var code = $"LU-{customer}-{shippingProgramNumber[2]}-{product_name}-{shippingProgramNumber[4]}-{date}-{quantityFormatted}-{r}";
                        record.lineup_number = code;
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("No record found for the provided lineup number: " + szLineCode);
                    }
                    count++;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 1, Line " + (i + 1) + " : " + Environment.NewLine;
                    }
                    else errormessage = ex.Message;

                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                    gagal = true;
                    break;
                }
            }
            wb.Close();
            if (gagal)
            {
                await transaction.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "ShipmentPlan");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpGet("GetLoadrateContracted/{salesContractTerm}/{transportTypeId}/{loadingPortId}")]
        public async Task<object> GetLoadrateContracted(string salesContractTerm,
            string transportTypeId, string loadingPortId)
        {
            try
            {
                decimal loadrateContracted = 0;

                var desDemTerm = await dbContext.sales_contract_despatch_demurrage_term
                    .Where(x => x.sales_contract_term_id == salesContractTerm)
                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefaultAsync();

                var transportType = await dbContext.master_list
                    .Where(x => x.id == transportTypeId)
                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefaultAsync();

                var loadingPort = await dbContext.port_location
                    .Where(x => x.id == loadingPortId)
                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefaultAsync();

                if (desDemTerm == null) throw new Exception("Contract Term Not Found!");
                if (transportType == null) throw new Exception("Transport Type Not Found!");
                if (loadingPort == null) throw new Exception("Loading Port Not Found!");

                var loadingPortName = loadingPort.stock_location_name.ToUpper();
                if (loadingPortName.Contains("NPLCT")) loadrateContracted = desDemTerm.loading_rate_nplct ?? 0;
                else
                {
                    var transportTypeName = transportType.item_name.ToUpper();
                    if (transportTypeName.Contains("GEARLESS")) loadrateContracted = desDemTerm.loading_rate_gearless ?? 0;
                    if (transportTypeName.Contains("G&G")) loadrateContracted = desDemTerm.loading_rate_geared ?? 0;
                }
                return loadrateContracted;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}

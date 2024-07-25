using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
using DocumentFormat.OpenXml.Office.Word;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using System.Globalization;

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class ShippingProgram : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ShippingProgram(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
            if (master_list2 != null) yearParam2 = master_list2.id.ToString();

            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.shipping_program
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.shipping_program
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.plan_year_id == yearParam1 && o.month_id >= month1 && o.month_id <= month2)
                    .Where(o => o.plan_year_id == yearParam2 && o.month_id >= month1 && o.month_id <= month2),
                    loadOptions);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.shipping_program
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin),
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
                dbContext.sales_plan.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    //string end_buyer_name = "";
                    if (await mcsContext.CanCreate(dbContext, nameof(shipping_program),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new shipping_program();
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
                        await dbContext.SaveChangesAsync();
                        decimal result = 0;
                        for (int i = 1; i <= 35; i++)
                        {
                            string propertyName = "product_" + i;
                            var property = record.GetType().GetProperty(propertyName);

                            // Check if the property exists and is not null
                            if (property != null)
                            {
                                var propertyValue = property.GetValue(record);
                                if (propertyValue != null)
                                {
                                    decimal decimalValue;
                                    if (decimal.TryParse(propertyValue.ToString(), out decimalValue))
                                    {
                                        result += decimalValue;
                                    }
                                }
                            }
                        }
                        record.quantity = result;
                        await dbContext.SaveChangesAsync();
                        #region get shipping number code
                        string FormatQuantity(int quantity)
                        {
                            const int threshold = 1000;

                            if (quantity >= threshold)
                            {
                                double formattedValue = quantity / 1000.0;
                                return $"{formattedValue:0.#}K";
                            }

                            return quantity.ToString();
                        }

                        var endBuyer = dbContext.customer.Where(o => o.id == record.end_user_id).Select(o => o.alias_name).FirstOrDefault();
                        var product_name = dbContext.product.Where(o => o.id == record.product_category_id).Select(o => o.product_name).FirstOrDefault();
                        var item_name = dbContext.master_list.Where(o => o.id == record.tipe_penjualan_id).Select(o => o.item_in_coding).FirstOrDefault();
                        var contract_term = dbContext.sales_contract_term.Where(o => o.id == record.sales_contract_id).FirstOrDefault();
                        var customer = dbContext.customer.Where(o => o.id == record.customer_id).Select(o => o.alias_name).FirstOrDefault();
                        var date = "";
                        if (contract_term != null)
                        {
                            date = contract_term.start_date.Value.ToString("MMMyy") + contract_term.end_date.Value.ToString("MMMyy");
                        }
                        var quantityFormatted = FormatQuantity(Convert.ToInt32(record.quantity));
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
                                //cmd.CommandText = $"SELECT nextval('seq_despatch_order_number')";
                                var r = await cmd.ExecuteScalarAsync();
                                r = Convert.ToInt32(r).ToString("D6");

                                #endregion
                                var code = $"SP-{customer}-{endBuyer}-{product_name}-{item_name}-{date}-{quantityFormatted}-{r}";
                                record.shipping_program_number = code;
                                dbContext.shipping_program.Add(record);
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
                    logger.Error(ex.ToString());
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPut("UpdateData")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.shipping_program
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
                        await dbContext.SaveChangesAsync();

                        decimal result = 0;
                        for (int i = 1; i <= 35; i++)
                        {
                            string propertyName = "product_" + i;
                            var property = record.GetType().GetProperty(propertyName);

                            // Check if the property exists and is not null
                            if (property != null)
                            {
                                var propertyValue = property.GetValue(record);
                                if (propertyValue != null)
                                {
                                    decimal decimalValue;
                                    if (decimal.TryParse(propertyValue.ToString(), out decimalValue))
                                    {
                                        result += decimalValue;
                                    }
                                }
                            }
                        }
                        record.quantity = result;
                        #region get shipping number code
                        string FormatQuantity(int quantity)
                        {
                            const int threshold = 1000;

                            if (quantity >= threshold)
                            {
                                double formattedValue = quantity / 1000.0;
                                return $"{formattedValue:0.#}K";
                            }

                            return quantity.ToString();
                        }

                        var endBuyer = dbContext.customer.Where(o => o.id == record.end_user_id).Select(o => o.alias_name).FirstOrDefault();
                        var product_name = dbContext.product.Where(o => o.id == record.product_category_id).Select(o => o.product_name).FirstOrDefault();
                        var item_name = dbContext.master_list.Where(o => o.id == record.tipe_penjualan_id).Select(o => o.item_in_coding).FirstOrDefault();
                        var contract_term = dbContext.sales_contract_term.Where(o => o.id == record.sales_contract_id).FirstOrDefault();
                        var customer = dbContext.customer.Where(o => o.id == record.customer_id).Select(o => o.alias_name).FirstOrDefault();
                        var date = "";
                        if (contract_term != null)
                        {
                            date = contract_term.start_date.Value.ToString("MMMyy") + contract_term.end_date.Value.ToString("MMMyy");
                        }
                        var quantityFormatted = FormatQuantity(Convert.ToInt32(record.quantity));
                        var conn = dbContext.Database.GetDbConnection();
                        string shippingProgramNumber = record.shipping_program_number;

                        // Split the string based on '-' and get the last part
                        string[] parts = shippingProgramNumber.Split('-');
                        string r = parts.LastOrDefault();

                        #endregion
                        var code = $"SP-{customer}-{endBuyer}-{product_name}-{item_name}-{date}-{quantityFormatted}-{r}";
                        record.shipping_program_number = code;
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
                var record = dbContext.shipping_program
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.shipping_program.Remove(record);
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
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.sales_plan
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
        public async Task<IActionResult> SaveData([FromBody] sales_plan Record)
        {
            try
            {
                var record = dbContext.sales_plan
                    .Where(o => o.id == Record.id)
                    .FirstOrDefault();
                if (record != null)
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
                    record = new sales_plan();
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

                    dbContext.sales_plan.Add(record);
                    await dbContext.SaveChangesAsync();

                    return Ok(record);
                }
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
                var record = dbContext.sales_plan
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.sales_plan.Remove(record);
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
        [HttpGet("ByShippingProgramId")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ByShippingProgramId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.shipping_program_detail
                    .Where(o => o.shipping_program_id == Id),
                        loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region detail
        [HttpPost("InsertDetail")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertDetail([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(shipping_program_detail),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new shipping_program_detail();
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
                        dbContext.shipping_program_detail.Add(record);
                        await dbContext.SaveChangesAsync();

                        /*for(var i = 1; i <= 12; i++)
                        {
                            var d = new sales_plan_detail();
                            d.id = Guid.NewGuid().ToString("N");
                            d.created_by = CurrentUserContext.AppUserId;
                            d.created_on = DateTime.Now;
                            d.modified_by = null;
                            d.modified_on = null;
                            d.is_active = true;
                            d.is_default = null;
                            d.is_locked = null;
                            d.entity_id = null;
                            d.owner_id = CurrentUserContext.AppUserId;
                            d.organization_id = CurrentUserContext.OrganizationId;

                            d.sales_plan_id = record.id;
                            d.month_id = i;
                            //d.percentage = (decimal?)(100.0 / 12.0);
                            d.quantity = (decimal?)(1.0 / 12.0) * record.quantity;

                            dbContext.sales_plan_detail.Add(d);
                            await dbContext.SaveChangesAsync();
                        }*/

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
                    logger.Error(ex.ToString());
                    return BadRequest(ex.Message);
                }
            }
        }
        [HttpPut("UpdateDetail")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateDetail([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.shipping_program_detail
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

        [HttpDelete("DeleteDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteDetail([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var record = dbContext.shipping_program_detail
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.shipping_program_detail.Remove(record);
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
        #endregion
        #region split
        [HttpPost("SplitQuantity")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SplitQuantity([FromForm] string key, [FromForm] string values, [FromForm] int number)
        {
            if (number < 2 || number > 60) { return BadRequest("Please input number from 2 to 60"); }
            dynamic result;
            var insert = number - 1;
            var recSP = dbContext.shipment_plan
                            .Where(o => o.id == key)
                            .FirstOrDefault();
            if (recSP.certain == null)
            {
                recSP.certain = false;
            }
            if (recSP.qty_sp == null || recSP.qty_sp == 0) { return BadRequest("This Data Does Not Have Quantity"); }
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var quantity = recSP.qty_sp / number;

                    recSP.modified_by = CurrentUserContext.AppUserId;
                    recSP.modified_on = DateTime.Now;
                    recSP.qty_sp = quantity;
                    await dbContext.SaveChangesAsync();
                    for (int i = 0; i < insert; i++)
                    {
                        var newSP = new shipment_plan()
                        {
                            id = Guid.NewGuid().ToString("N"),
                            created_by = CurrentUserContext.AppUserId,
                            created_on = DateTime.Now,
                            modified_by = CurrentUserContext.AppUserId,
                            modified_on = DateTime.Now,
                            is_active = true,
                            is_default = null,
                            is_locked = null,
                            entity_id = null,
                            owner_id = CurrentUserContext.AppUserId,
                            organization_id = CurrentUserContext.OrganizationId,
                            business_unit_id = recSP.business_unit_id,

                            customer_id = recSP.customer_id,
                            sales_contract_id = recSP.sales_contract_id,
                            shipment_year = recSP.shipment_year,
                            month_id = recSP.month_id,
                            product_id = recSP.product_id,
                            destination = recSP.destination,
                            shipment_number = recSP.shipment_number,
                            incoterm = recSP.incoterm,
                            transport_id = recSP.transport_id,
                            laycan_start = recSP.laycan_start,
                            laycan_end = recSP.laycan_end,
                            is_geared = recSP.is_geared,
                            invoice_no = recSP.invoice_no,
                            royalti = recSP.royalti,
                            laycan = recSP.laycan,
                            eta = recSP.eta,
                            qty_sp = quantity,
                            remarks = recSP.remarks,
                            loading_rate = recSP.loading_rate,
                            loading_standart = recSP.loading_standart,
                            despatch_demurrage_rate = recSP.despatch_demurrage_rate,
                            declared_month_id = recSP.declared_month_id,
                            certain = recSP.certain,
                            nora = recSP.nora,
                            etb = recSP.etb,
                            etc = recSP.etc,
                            pln_schedule = recSP.pln_schedule,
                            original_schedule = recSP.original_schedule,
                            loading_port = recSP.loading_port,
                            eta_disc = recSP.eta_disc,
                            etb_disc = recSP.etb_disc,
                            etcommence_disc = recSP.etcommence_disc,
                            etcompleted_disc = recSP.etcompleted_disc,
                            stow_plan = recSP.stow_plan,
                            vessel_id = recSP.vessel_id,
                            traffic_officer_id = recSP.traffic_officer_id,
                            sales_plan_customer_id = recSP.sales_plan_customer_id,
                            laycan_status = recSP.laycan_status,
                            eta_status = recSP.eta_status,
                            contract_product_id = recSP.contract_product_id,
                            shipping_agent = recSP.shipping_agent,
                            surveyor = recSP.surveyor,
                            end_user = recSP.end_user,
                            sp_month_id = recSP.sp_month_id,
                            shipping_program_id = recSP.shipping_program_id
                        };
                        await dbContext.SaveChangesAsync();

                        string FormatQuantity(int quantity)
                        {
                            const int threshold = 1000;

                            if (quantity >= threshold)
                            {
                                double formattedValue = quantity / 1000.0;
                                return $"{formattedValue:0.#}K";
                            }

                            return quantity.ToString();
                        }

                        var splitedQuantity = recSP.lineup_number.Split('-');
                        var quantityFormatted = FormatQuantity(Convert.ToInt32(quantity));

                        splitedQuantity[6] = quantityFormatted.ToString();
                        string modifiedLineupNumber = string.Join("-", splitedQuantity);
                        recSP.lineup_number = modifiedLineupNumber;

                        var conn = dbContext.Database.GetDbConnection();
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            await conn.OpenAsync();
                        }
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                string organizationCode = dbContext.organization
                                     .Where(o => o.id == CurrentUserContext.OrganizationId)
                                     .Select(o => o.organization_code)
                                     .FirstOrDefault() ?? "";
                                cmd.CommandText = $"SELECT nextval('seq_shipping_program_number')";

                                var r = await cmd.ExecuteScalarAsync();
                                r = Convert.ToInt32(r).ToString("D6");
                                splitedQuantity[7] = (string)r;

                                // join the array back into a string
                                modifiedLineupNumber = string.Join("-", splitedQuantity);

                                newSP.lineup_number = modifiedLineupNumber;

                                dbContext.shipment_plan.Add(newSP);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                    }

                    result = recSP;
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
        #endregion

        [HttpGet("PortIdLookup")]
        public async Task<object> PortIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.port_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.stock_location_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SalesPlanYearIdLookup/{YearId}")]
        public async Task<IActionResult> SalesPlanYearIdLookup(string YearId)
        {
            try
            {
                var record = await dbContext.sales_plan
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.plan_year_id == YearId).FirstOrDefaultAsync();
                return Ok(record);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SalesPlanIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SalesPlanIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_sales_plan
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.plan_year });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ShippingProduct")]
        public async Task<object> ShippingProduct(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.master_list
                    .Where(o => o.item_group == "shipping-program-product").OrderBy(o => o.item_in_coding).Select(o => o.item_name);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SalesPlanCustomerListLookup")]
        public async Task<object> SalesPlanCustomerListLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_sales_plan_customer_list
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SalesContractIdLookup")]
        public async Task<object> SalesContractLookup(DataSourceLoadOptions loadOptions, string CustomerId)
        {
            try
            {
                //string salesId = "";
                if (!string.IsNullOrEmpty(CustomerId))
                {
                    var lookup = dbContext.sales_contract
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.customer_id == CustomerId).Select(o => o.id).ToList();
                    // s = lookup;
                    var contractTerm = dbContext.sales_contract_term
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && lookup.Contains(o.sales_contract_id))
                        .Select(o => new { Value = o.id, Text = o.contract_term_name });
                    return await DataSourceLoader.LoadAsync(contractTerm, loadOptions);
                }
                else
                {
                    var contractTerm = dbContext.sales_contract_term.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Select(o => new { Value = o.id, Text = o.contract_term_name });
                    return await DataSourceLoader.LoadAsync(contractTerm, loadOptions);
                }


            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SalesContractEndUserIdLookup")]
        public async Task<object> SalesContractEndUserIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var contractTerm = dbContext.vw_sales_contract_end_user.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.customer_id, Text = o.business_partner_name })
                    .Distinct();
                return await DataSourceLoader.LoadAsync(contractTerm, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProductDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProductDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.product.Where(o => o.id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)),
                loadOptions);
        }

        [HttpGet("SalesPlanCustomerIdLookup")]
        public async Task<object> SalesPlanCustomerIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                /*var lookup = dbContext.vw_sales_plan_customer_list
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    //.Select(o => new { Value = o.id, Text = o.business_partner_code + " - " + o.sales_contract_name + " - " + o.plan_name 
                    //    + " - Month(" + o.month_id + ")" });
                    .Select(o => new { Value = o.id, Text = o.business_partner_code + " - " + o.sales_contract_name + " - " 
                        + o.plan_name + " - " + o.month_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);*/

                var lookup = dbContext.vw_sales_plan_customer_list
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
           .Join(dbContext.master_list, // Gantilah OtherTable dengan nama tabel lain yang ingin Anda join
                 salesPlan => salesPlan.plan_name,
                 masterList => masterList.id,
                 (salesPlan, masterList) => new // Sesuaikan SomeColumn dengan kolom yang sesuai untuk melakukan join
                 {
                     Value = salesPlan.id,
                     Text = salesPlan.business_partner_code + " - " + salesPlan.sales_contract_name + " - "
                         + masterList.item_name + " - " + salesPlan.month_name,
                     //AdditionalField = otherTable.SomeField // Tambahkan kolom dari tabel lain ke dalam lookup jika diperlukan
                 });

                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("MasterListYearIdLookup")]
        public async Task<object> MasterListYearIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.master_list
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.item_group == "years")
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.item_name, o.item_group, o.item_in_coding });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
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

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    var year = "";
                    var master_list = await dbContext.master_list
                        .Where(o => o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower())
                        .FirstOrDefaultAsync();
                    if (master_list != null) year = master_list.id.ToString(); //2

                    var commitment = "";
                    var ml = await dbContext.master_list
                        .Where(o => o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower())
                        .FirstOrDefaultAsync();
                    if (ml != null) commitment = ml.id.ToString(); //5

                    var product_name1 = "";
                    var product = await dbContext.product
                        .Where(o => o.product_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower())
                        .FirstOrDefaultAsync();
                    if (product != null) product_name1 = product.id.ToString(); //4

                    var customer_name = "";
                    var cust = await dbContext.customer
                        .Where(o => o.alias_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(6)).ToLower())
                        .FirstOrDefaultAsync();
                    if (cust != null && cust.alias_name != null) customer_name = cust.id.ToString(); //6

                    var sales_contract = "";
                    var sales = await dbContext.sales_contract_term
                        .Where(o => o.contract_term_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(7)).ToLower())
                        .FirstOrDefaultAsync(); //7
                    if (sales != null) sales_contract = sales.id.ToString();

                    var port_1 = "";
                    var port1 = await dbContext.master_list
                        .Where(o => o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(8)).ToLower())
                        .Where(o => o.item_group == "tipe-penjualan")
                        .FirstOrDefaultAsync(); //8
                    if (port1 != null) port_1 = port1.id.ToString();

                    var port_2 = "";
                    var port2 = await dbContext.port_location
                        .Where(o => o.stock_location_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(9)).ToLower())
                        .FirstOrDefaultAsync(); //9
                    if (port2 != null) port_2 = port2.id.ToString();

                    var end_buyer = "";
                    var eb = await dbContext.customer
                        .Where(o => o.business_partner_name == PublicFunctions.IsNullCell(row.GetCell(11)))
                        .FirstOrDefaultAsync(); //9
                    if (eb != null) end_buyer = eb.id.ToString();

                    //var declared_month_id = string.Empty;
                    //var declared_month = PublicFunctions.IsNullCell(row.GetCell(3));
                    //if (declared_month != null) declared_month_id = declared_month;

                    var shippingProgramNumber = PublicFunctions.IsNullCell(row.GetCell(0));

                    var record = await dbContext.shipping_program
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.shipping_program_number.ToUpper() == shippingProgramNumber.ToUpper())
                        .FirstOrDefaultAsync();

                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.plan_year_id = year;
                        record.month_id = PublicFunctions.Bulat(row.GetCell(2));
                        record.declared_month_id = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.product_category_id = product_name1;
                        record.quantity = PublicFunctions.Desimal(row.GetCell(10));
                        record.commitment_id = commitment;
                        record.customer_id = customer_name;
                        record.tipe_penjualan_id = port_1;
                        record.source_coal_id = port_2;
                        record.end_user_id = end_buyer;
                        record.sales_contract_id = sales_contract;

                        record.product_1 = PublicFunctions.Desimal(row.GetCell(12));
                        record.product_2 = PublicFunctions.Desimal(row.GetCell(13));
                        record.product_3 = PublicFunctions.Desimal(row.GetCell(14));
                        record.product_4 = PublicFunctions.Desimal(row.GetCell(15));
                        record.product_5 = PublicFunctions.Desimal(row.GetCell(16));
                        record.product_6 = PublicFunctions.Desimal(row.GetCell(17));
                        record.product_7 = PublicFunctions.Desimal(row.GetCell(18));
                        record.product_8 = PublicFunctions.Desimal(row.GetCell(19));
                        record.product_9 = PublicFunctions.Desimal(row.GetCell(20));
                        record.product_10 = PublicFunctions.Desimal(row.GetCell(21));
                        record.product_11 = PublicFunctions.Desimal(row.GetCell(22));
                        record.product_12 = PublicFunctions.Desimal(row.GetCell(23));
                        record.product_13 = PublicFunctions.Desimal(row.GetCell(24));
                        record.product_14 = PublicFunctions.Desimal(row.GetCell(25));
                        record.product_15 = PublicFunctions.Desimal(row.GetCell(26));
                        record.product_16 = PublicFunctions.Desimal(row.GetCell(27));
                        record.product_17 = PublicFunctions.Desimal(row.GetCell(28));
                        record.product_18 = PublicFunctions.Desimal(row.GetCell(29));
                        record.product_19 = PublicFunctions.Desimal(row.GetCell(30));
                        record.product_20 = PublicFunctions.Desimal(row.GetCell(31));
                        record.product_21 = PublicFunctions.Desimal(row.GetCell(32));
                        record.product_22 = PublicFunctions.Desimal(row.GetCell(33));
                        record.product_23 = PublicFunctions.Desimal(row.GetCell(34));
                        record.product_24 = PublicFunctions.Desimal(row.GetCell(35));
                        record.product_25 = PublicFunctions.Desimal(row.GetCell(36));
                        record.product_26 = PublicFunctions.Desimal(row.GetCell(37));
                        record.product_27 = PublicFunctions.Desimal(row.GetCell(38));
                        record.product_28 = PublicFunctions.Desimal(row.GetCell(39));
                        record.product_29 = PublicFunctions.Desimal(row.GetCell(40));
                        record.product_30 = PublicFunctions.Desimal(row.GetCell(41));
                        record.product_31 = PublicFunctions.Desimal(row.GetCell(42));
                        record.product_32 = PublicFunctions.Desimal(row.GetCell(43));
                        record.product_33 = PublicFunctions.Desimal(row.GetCell(44));
                        record.product_34 = PublicFunctions.Desimal(row.GetCell(45));
                        record.product_35 = PublicFunctions.Desimal(row.GetCell(46));
                        await dbContext.SaveChangesAsync();
                        #region get shipping number code
                        string FormatQuantity(int quantity)
                        {
                            const int threshold = 1000;

                            if (quantity >= threshold)
                            {
                                double formattedValue = quantity / 1000.0;
                                return $"{formattedValue:0.#}K";
                            }

                            return quantity.ToString();
                        }
                        var endBuyer = await dbContext.customer.Where(o => o.id == record.end_user_id).Select(o => o.alias_name).FirstOrDefaultAsync();
                        var product_name = await dbContext.product.Where(o => o.id == record.product_category_id).Select(o => o.product_name).FirstOrDefaultAsync();
                        var item_name = await dbContext.master_list.Where(o => o.id == record.tipe_penjualan_id).Select(o => o.item_in_coding).FirstOrDefaultAsync();
                        var contract_term = await dbContext.sales_contract_term.Where(o => o.id == record.sales_contract_id).FirstOrDefaultAsync();
                        var customer = await dbContext.customer.Where(o => o.id == record.customer_id).Select(o => o.alias_name).FirstOrDefaultAsync();
                        var date = "";

                        if (contract_term != null)
                            date = contract_term.start_date.Value.ToString("MMMyy") + contract_term.end_date.Value.ToString("MMMyy");

                        var quantityFormatted = FormatQuantity(Convert.ToInt32(record.quantity));
                        var conn = dbContext.Database.GetDbConnection();
                        var arrProgramNumber = record.shipping_program_number.Split("-");
                        var r = arrProgramNumber.LastOrDefault();

                        #endregion
                        var code = string.Format("SP-{0}-{1}-{2}-{3}-{4}-{5}-{6}",
                            customer, endBuyer, product_name, item_name, date, quantityFormatted, r);
                        record.shipping_program_number = code;
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        //string end_buyer_name = "";

                        record = new shipping_program();
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
                        record.business_unit_id = "cd1cdffbbc034feca3b293e95c9fef7d";

                        record.plan_year_id = year;
                        record.month_id = PublicFunctions.Bulat(row.GetCell(2));
                        record.declared_month_id = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.product_category_id = product_name1;
                        record.quantity = PublicFunctions.Desimal(row.GetCell(10));
                        record.commitment_id = commitment;
                        record.customer_id = customer_name;
                        record.tipe_penjualan_id = port_1;
                        record.source_coal_id = port_2;
                        record.end_user_id = end_buyer;
                        record.sales_contract_id = sales_contract;

                        record.product_1 = PublicFunctions.Desimal(row.GetCell(12));
                        record.product_2 = PublicFunctions.Desimal(row.GetCell(13));
                        record.product_3 = PublicFunctions.Desimal(row.GetCell(14));
                        record.product_4 = PublicFunctions.Desimal(row.GetCell(15));
                        record.product_5 = PublicFunctions.Desimal(row.GetCell(16));
                        record.product_6 = PublicFunctions.Desimal(row.GetCell(17));
                        record.product_7 = PublicFunctions.Desimal(row.GetCell(18));
                        record.product_8 = PublicFunctions.Desimal(row.GetCell(19));
                        record.product_9 = PublicFunctions.Desimal(row.GetCell(20));
                        record.product_10 = PublicFunctions.Desimal(row.GetCell(21));
                        record.product_11 = PublicFunctions.Desimal(row.GetCell(22));
                        record.product_12 = PublicFunctions.Desimal(row.GetCell(23));
                        record.product_13 = PublicFunctions.Desimal(row.GetCell(24));
                        record.product_14 = PublicFunctions.Desimal(row.GetCell(25));
                        record.product_15 = PublicFunctions.Desimal(row.GetCell(26));
                        record.product_16 = PublicFunctions.Desimal(row.GetCell(27));
                        record.product_17 = PublicFunctions.Desimal(row.GetCell(28));
                        record.product_18 = PublicFunctions.Desimal(row.GetCell(29));
                        record.product_19 = PublicFunctions.Desimal(row.GetCell(30));
                        record.product_20 = PublicFunctions.Desimal(row.GetCell(31));
                        record.product_21 = PublicFunctions.Desimal(row.GetCell(32));
                        record.product_22 = PublicFunctions.Desimal(row.GetCell(33));
                        record.product_23 = PublicFunctions.Desimal(row.GetCell(34));
                        record.product_24 = PublicFunctions.Desimal(row.GetCell(35));
                        record.product_25 = PublicFunctions.Desimal(row.GetCell(36));
                        record.product_26 = PublicFunctions.Desimal(row.GetCell(37));
                        record.product_27 = PublicFunctions.Desimal(row.GetCell(38));
                        record.product_28 = PublicFunctions.Desimal(row.GetCell(39));
                        record.product_29 = PublicFunctions.Desimal(row.GetCell(40));
                        record.product_30 = PublicFunctions.Desimal(row.GetCell(41));
                        record.product_31 = PublicFunctions.Desimal(row.GetCell(42));
                        record.product_32 = PublicFunctions.Desimal(row.GetCell(43));
                        record.product_33 = PublicFunctions.Desimal(row.GetCell(44));
                        record.product_34 = PublicFunctions.Desimal(row.GetCell(45));
                        record.product_35 = PublicFunctions.Desimal(row.GetCell(46));
                        await dbContext.SaveChangesAsync();
                        #region get shipping number code
                        string FormatQuantity(int quantity)
                        {
                            const int threshold = 1000;

                            if (quantity >= threshold)
                            {
                                double formattedValue = quantity / 1000.0;
                                return $"{formattedValue:0.#}K";
                            }

                            return quantity.ToString();
                        }
                            
                        var endBuyer = await dbContext.customer.Where(o => o.id == record.end_user_id).Select(o => o.alias_name).FirstOrDefaultAsync();
                        var product_name = await dbContext.product.Where(o => o.id == record.product_category_id).Select(o => o.product_name).FirstOrDefaultAsync();
                        var item_name = await dbContext.master_list.Where(o => o.id == record.tipe_penjualan_id).Select(o => o.item_in_coding).FirstOrDefaultAsync();
                        var contract_term = await dbContext.sales_contract_term.Where(o => o.id == record.sales_contract_id).FirstOrDefaultAsync();
                        var customer = await dbContext.customer.Where(o => o.id == record.customer_id).Select(o => o.alias_name).FirstOrDefaultAsync();
                        var date = "";
                        if (contract_term != null)
                        {
                            date = contract_term.start_date.Value.ToString("MMMyy") + contract_term.end_date.Value.ToString("MMMyy");
                        }
                        var quantityFormatted = FormatQuantity(Convert.ToInt32(record.quantity));
                        var conn = dbContext.Database.GetDbConnection();
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            await conn.OpenAsync();
                        }
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                string organizationCode = await dbContext.organization
                                     .Where(o => o.id == CurrentUserContext.OrganizationId)
                                     .Select(o => o.organization_code)
                                     .FirstOrDefaultAsync() ?? "";
                                cmd.CommandText = $"SELECT nextval('seq_shipping_program_number')";
                                var r = await cmd.ExecuteScalarAsync();
                                r = Convert.ToInt32(r).ToString("D6");

                                #endregion
                                var code = string.Format("SP-{0}-{1}-{2}-{3}-{4}-{5}-{6}",
                                    customer, endBuyer, product_name, item_name, date, quantityFormatted, r);
                                record.shipping_program_number = code;
                                dbContext.shipping_program.Add(record);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 1, Line " + (i + 1) + " : " + Environment.NewLine;
                    }
                    else errormessage = "==> Error on Sheet 1, Line " + (i + 1) + " : " + ex.Message;
                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                    gagal = true;
                    break;
                }

            }
            sheet = wb.GetSheetAt(1); //*** detail sheet
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var productCategory = "";
                    var pc = await dbContext.product_category.Where(o => o.product_category_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower()).FirstOrDefaultAsync();
                    if (pc != null) productCategory = pc.id.ToString();

                    var product_ = "";
                    var p = await dbContext.product.Where(o => o.product_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower()).FirstOrDefaultAsync();
                    if (p != null) product_ = p.id.ToString();

                    var contractor = "";
                    var c = await dbContext.contractor.Where(o => o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefaultAsync();
                    if (c != null) contractor = c.id.ToString();

                    var header = await dbContext.shipping_program
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.shipping_program_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower())
                        .FirstOrDefaultAsync();

                    var record = await dbContext.shipping_program_detail
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.shipping_program_id == header.id && o.product_id == product_)
                        .FirstOrDefaultAsync();

                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = System.DateTime.Now;

                        record.contractor_id = contractor;
                        record.product_category_id = productCategory;
                        record.product_id = product_;
                        record.quantity = PublicFunctions.Desimal(row.GetCell(4));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var newRecord = new shipping_program_detail();
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
                        newRecord.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        newRecord.shipping_program_id = header.id;
                        newRecord.contractor_id = contractor;
                        newRecord.product_category_id = productCategory;
                        newRecord.product_id = product_;
                        newRecord.quantity = PublicFunctions.Desimal(row.GetCell(4));

                        dbContext.shipping_program_detail.Add(newRecord);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 2, Line " + (i + 1) + ": " + Environment.NewLine;
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
                HttpContext.Session.SetString("filename", "ShippingProgram");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
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

                        foreach (string key in selectedIds)
                        {
                            var record = dbContext.shipping_program
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.shipping_program.Remove(record);
                                await dbContext.SaveChangesAsync();

                                var itemList = dbContext.production_transaction_item
                                .Where(o => o.production_transaction_id == record.id).ToList();
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
    }
}

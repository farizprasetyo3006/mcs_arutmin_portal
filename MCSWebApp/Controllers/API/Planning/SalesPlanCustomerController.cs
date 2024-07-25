using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataAccess.DTO;
using NLog;
using DataAccess.EFCore.Repository;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Omu.ValueInjecter;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Common;
using System.Dynamic;

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class SalesPlanCustomerController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public SalesPlanCustomerController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("BySalesPlanDetailId/{Id}")]
        public async Task<object> BySalesPlanDetailId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_sales_plan_customer.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.sales_plan_detail_id == Id)
                    .OrderBy(o => o.customer_id),
                    loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("BySalesPlanCustomerId/{Id}")]
        public async Task<object> BySalesPlanCustomerId(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.sales_plan_customer
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sales_plan_detail_id == Id)
                    .GroupBy(o => o.sales_plan_detail_id)
                    .Select(o =>
                    new
                    {
                        quantity = o.Sum(p => p.quantity)
                    });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.sales_plan_customer
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
                dbContext.sales_plan_customer.Where(o => o.id == Id),
                loadOptions);
        }

        private bool CheckQuantity(int new_spc_qty, string spd_id)
        {
            //bool retval = true;

            var record_spd = dbContext.sales_plan_detail
            .Where(o => o.id == spd_id)
            .FirstOrDefault();

            if (record_spd != null)
            {
                //var records_spc = dbContext.sales_plan_customer
                //.Where(o => o.sales_plan_detail_id == spd_id);

                //int total_qty_customers = 0;
                //foreach (var record_spc in records_spc)
                //{
                //    total_qty_customers += (int)record_spc.quantity;
                //}

                int total_qty_customers = (int)dbContext.sales_plan_customer
                    .Where(o => o.sales_plan_detail_id == spd_id)
                    .Sum(o => o.quantity);

                //if (new_spc_qty > (record_spd.quantity - total_qty_customers))
                if (record_spd.quantity < (new_spc_qty + total_qty_customers))
                    return false;
                else
                    return true;
            }
            else
            {
                return false;
            }

            //return retval;
        }

        [HttpPost("InsertData")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(sales_plan_customer),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var record = new sales_plan_customer();
                    JsonConvert.PopulateObject(values, record);

                    if (CheckQuantity((int)record.quantity, record.sales_plan_detail_id))
                    {
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

                        dbContext.sales_plan_customer.Add(record);
                        await dbContext.SaveChangesAsync();

                        return Ok(record);

                    }
                    else
                    {
                        return BadRequest("Total quantity of all customers should be lower than a month's quantity");
                    }
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
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.sales_plan_customer
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        decimal oldQty = record.quantity ?? 0;

                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);

                        decimal newQty = record.quantity ?? 0;
                        newQty = newQty - oldQty;

                        //if (CheckQuantity((int)record.quantity, record.sales_plan_detail_id) == false)
                        if (CheckQuantity((int)newQty, record.sales_plan_detail_id) == false)
                        {
                            return BadRequest("Total quantity of all customers should be lower than a month's quantity");
                        }

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
                var shipment_plan = dbContext.shipment_plan
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.sales_plan_customer_id == key).FirstOrDefault();
                if (shipment_plan != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var record = dbContext.sales_plan_customer
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.sales_plan_customer.Remove(record);
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

        [HttpGet("SalesContractIdLookup")]
        public async Task<object> SalesContractLookup(DataSourceLoadOptions loadOptions, string CustomerId)
        {
            try
            {
                if (!string.IsNullOrEmpty(CustomerId))
                {
                    var lookup = dbContext.sales_contract
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.customer_id == CustomerId)
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

        [HttpGet("DMOLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public object DMOLookup()
        {
            var result = new List<dynamic>();
            try
            {
                foreach (var item in Common.Constants.DMOList)
                {
                    dynamic obj = new ExpandoObject();
                    obj.value = item;
                    obj.text = item;
                    result.Add(obj);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }

            return result;
        }

        [HttpGet("ElectricityLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public object ElectricityLookup()
        {
            var result = new List<dynamic>();
            try
            {
                foreach (var item in Common.Constants.ElectricityList)
                {
                    dynamic obj = new ExpandoObject();
                    obj.value = item;
                    obj.text = item;
                    result.Add(obj);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }

            return result;
        }

        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.sales_plan_customer
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
        public async Task<IActionResult> SaveData([FromBody] sales_plan_customer Record)
        {
            try
            {
                var record = dbContext.sales_plan_customer
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
                    record = new sales_plan_customer();
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

                    dbContext.sales_plan_customer.Add(record);
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
                var record = dbContext.sales_plan_customer
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.sales_plan_customer.Remove(record);
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

        [HttpGet("VesselIdBargeIdLookup")]
        public async Task<object> VesselIdBargeIdLookup(DataSourceLoadOptions loadOptions, string CustomerId)
        {
            try
            {
                var vessel = dbContext.vessel
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name });
                var barge = dbContext.barge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name });
                var vessel_barge = vessel.Union(barge);
                return await DataSourceLoader.LoadAsync(vessel_barge, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}

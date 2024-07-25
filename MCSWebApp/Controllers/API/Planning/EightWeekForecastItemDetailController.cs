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
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore.Storage;
using FastReport.Export.Dbf;

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class EightWeekForecastItemDetailController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public EightWeekForecastItemDetailController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(string item_id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.eight_week_forecast_item_detail
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.item_id == item_id),
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
                dbContext.eight_week_forecast_item_detail
                .Where(o => o.id == Id),
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
                    if (await mcsContext.CanCreate(dbContext, nameof(eight_week_forecast_item_detail),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new eight_week_forecast_item_detail();
                        JsonConvert.PopulateObject(values, record);

                        var recordItem = await dbContext.eight_week_forecast_item
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            .Where(o => o.id == record.item_id)
                            .FirstOrDefaultAsync();

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
                        record.business_unit_id = recordItem.business_unit_id;
                        record.is_using = true;

                        dbContext.eight_week_forecast_item_detail.Add(record);
                        await dbContext.SaveChangesAsync();

                        bool bUpdated = await RecalculateHeaderItem(record.item_id);
                        if (!bUpdated) throw new Exception("Error Updating Total, Going to Revert.");

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

        private async Task<bool> RecalculateHeaderItem(string item_id)
        {
            try
            {
                var itemDetails = await dbContext.eight_week_forecast_item_detail
                    .Where(x => x.item_id == item_id)
                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                    .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .ToListAsync();

                decimal? total_item = 0;
                var header_id = string.Empty;
                foreach (var item in itemDetails)
                {
                    total_item += item.quantity;
                    header_id = item.header_id;
                }

                var dataItem = await dbContext.eight_week_forecast_item
                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                    .Where(x => x.id == item_id)
                    .FirstOrDefaultAsync();

                dataItem.total = total_item;

                await dbContext.SaveChangesAsync();

                var headerItems = await dbContext.eight_week_forecast_item
                    .Where(x => x.header_id == header_id)
                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                    .Where(x => x.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .ToListAsync();

                decimal? total_header = 0;
                foreach (var item in headerItems)
                {
                    total_header += item.total;
                }

                var dataHeader = await dbContext.eight_week_forecast
                    .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                    .Where(x => x.id == header_id)
                    .FirstOrDefaultAsync();

                dataHeader.total = total_header;

                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
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
                    var record = await dbContext.eight_week_forecast_item_detail
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

                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            await dbContext.SaveChangesAsync();

                            bool bUpdated = await RecalculateHeaderItem(record.item_id);
                            if (!bUpdated) throw new Exception("Error Updating Total, Going to Revert.");

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
                    await tx.RollbackAsync();
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
                    var record = dbContext.eight_week_forecast_item_detail
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        dbContext.eight_week_forecast_item_detail.Remove(record);

                        bool bUpdated = await RecalculateHeaderItem(record.item_id);
                        if (!bUpdated) throw new Exception("Error Updating Total, Going to Revert.");

                        await dbContext.SaveChangesAsync();
                        await tx.CommitAsync();
                    }

                    return Ok();
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.InnerException ?? ex);
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        #region Lookup Item

        [HttpGet("LocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> LocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_area_breakdown_structure
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.child_2 != null)
                    .Where(o => o.child_3 == null)
                    .Select(o => new { Value = o.id, Text = o.business_area_name, Search = o.business_area_name.ToLower() + o.business_area_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("PitIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> PitIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_area_breakdown_structure
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.child_4 != null)
                    .Where(o => o.child_5 == null)
                    .Select(o => new { Value = o.id, Text = o.business_area_name, Search = o.business_area_name.ToLower() + o.business_area_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProductCategoryIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProductCategoryIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.product_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.product_category_name, Search = o.product_category_name.ToLower() + o.product_category_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProductIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProductIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.product
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.product_name, Search = o.product_name.ToLower() + o.product_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ContractorIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ContractorIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var customer = dbContext.customer
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderBy(o => o.business_partner_code)
                    .Select(o => new { Value = o.id, Text = o.business_partner_code + " - " + o.business_partner_name });
                var contractor = dbContext.contractor
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderBy(o => o.business_partner_code)
                    .Select(o => new { Value = o.id, Text = o.business_partner_code + " - " + o.business_partner_name });
                var lookup = customer.Union(contractor).OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("DuplicateRow")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DuplicateRow([FromForm] string key)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var oldRecord = await dbContext.eight_week_forecast_item_detail
                        .Where(x => x.id == key)
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefaultAsync();

                    if (oldRecord == null)
                        throw new Exception("Record Not Found!");

                    var record = new eight_week_forecast_item_detail();
                    record.InjectFrom(oldRecord);

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
                    record.is_using = true;

                    dbContext.eight_week_forecast_item_detail.Add(record);
                    await dbContext.SaveChangesAsync();

                    await tx.CommitAsync();

                    await RecalculateHeaderItem(record.item_id);
                    return Ok(record);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.InnerException ?? ex);
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        #endregion

    }
}

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

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class ProductionPlanController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ProductionPlanController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                return await DataSourceLoader.LoadAsync(dbContext.vw_production_plan
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
                dbContext.production_plan.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using(var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
					if (await mcsContext.CanCreate(dbContext, nameof(production_plan),
						CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
					{
                        var record = new production_plan();
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
					//record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        dbContext.production_plan.Add(record);
                        await dbContext.SaveChangesAsync();

                        for(var i = 1; i <= 12; i++)
                        {
                            var monthlyPlanRecord = new production_plan_monthly();
                            monthlyPlanRecord.id = Guid.NewGuid().ToString("N");
                            monthlyPlanRecord.created_by = CurrentUserContext.AppUserId;
                            monthlyPlanRecord.created_on = DateTime.Now;
                            monthlyPlanRecord.modified_by = null;
                            monthlyPlanRecord.modified_on = null;
                            monthlyPlanRecord.is_active = true;
                            monthlyPlanRecord.is_default = null;
                            monthlyPlanRecord.is_locked = null;
                            monthlyPlanRecord.entity_id = null;
                            monthlyPlanRecord.owner_id = CurrentUserContext.AppUserId;
                            monthlyPlanRecord.organization_id = CurrentUserContext.OrganizationId;

                            monthlyPlanRecord.production_plan_id = record.id;
                            monthlyPlanRecord.month_id = i;
                            monthlyPlanRecord.quantity = (decimal?)0;

                            dbContext.production_plan_monthly.Add(monthlyPlanRecord);
                            await dbContext.SaveChangesAsync();
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
                var record = dbContext.production_plan
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
                    return BadRequest("Record does not exist.");
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
                var record = dbContext.production_plan
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.production_plan.Remove(record);
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
                var record = await dbContext.production_plan
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                return Ok(record);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
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
                            var record = dbContext.production_plan
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.production_plan.Remove(record);
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

        [HttpGet("ContractorIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ContractorIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.contractor
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderBy(o=>o.business_partner_name)
                    .Select(o => new { Value = o.id, Text = o.business_partner_name, Search = o.business_partner_name.ToLower() });
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
                    .Where(o=>o.product_category_name.ToUpper() == "BIT COAL" ||  o.product_category_name.ToUpper() == "ECO COAL" || o.product_category_name.ToUpper() == "SARONGGA COAL")
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.product_category_name, Search = o.product_category_name.ToLower() + o.product_category_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("StockpileLocationIdLookup")]
        public async Task<object> StockpileLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var stockpile = dbContext.vw_stockpile_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = (o.business_area_name != null) ? o.business_area_name + " > " + o.stock_location_name : o.stock_location_name,
                            index = 0
                        }); ;
                var ports = dbContext.vw_port_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = (o.business_area_name != null) ? o.business_area_name + " > " + o.stock_location_name : o.stock_location_name,
                            index = 1
                        }); ;
                var lookup = stockpile.Union(ports);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] production_plan Record)
        {
            try
            {
                var record = dbContext.production_plan
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
                else if (await mcsContext.CanCreate(dbContext, nameof(production_plan),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new production_plan();
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

                    dbContext.production_plan.Add(record);
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

        [HttpDelete("DeleteById/{Id}")]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Debug($"string Id = {Id}");

            try
            {
                var record = dbContext.production_plan
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.production_plan.Remove(record);
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

        [HttpGet("SalesPlanYearIdLookup/{YearId}")]
        public async Task<IActionResult> SalesPlanYearIdLookup(string YearId)
        {
            try
            {
                var record = await dbContext.production_plan
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                        && o.master_list_id == YearId).FirstOrDefaultAsync();
                return Ok(record);
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

                    var plan_year_id = "";
                    var master_list = await dbContext.master_list
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.item_group == "years"
                            && o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower())
                        .FirstOrDefaultAsync();
                    if (master_list != null) plan_year_id = master_list.id.ToString();

                    var business_area_id = "";
                    var business_area = await dbContext.business_area
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.business_area_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower()).FirstOrDefaultAsync();
                    if (business_area != null) business_area_id = business_area.id.ToString();

                    var seam = "";
                    var seam_id = await dbContext.mine_location
                        .Where(o => o.mine_location_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower())
                        .FirstOrDefaultAsync();
                    if (seam_id != null) seam = seam_id.id.ToString();

                    var plan_type = "";
                    var type = await dbContext.master_list
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                        && o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower())
                        .FirstOrDefaultAsync();
                    if (type != null) plan_type = type.id.ToString();

                    var activity_plan = "";
                    var activity = dbContext.master_list.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower()).FirstOrDefault();
                    if (activity != null) activity_plan = activity.id.ToString();

                    var product_name = "";
                    var product = await dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                        && o.product_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(7)).ToLower())
                        .FirstOrDefaultAsync();
                    if (product != null) product_name = product.id;

                    var product_category = "";
                    var productC = await dbContext.product_category
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                        && o.product_category_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(6)).ToLower())
                        .FirstOrDefaultAsync();
                    if (productC != null) product_category = productC.id;

                    var contractor_name = "";
                    var contractor = await dbContext.contractor
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                        && o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(8)).ToLower())
                        .FirstOrDefaultAsync();
                    if (contractor != null) contractor_name = contractor.id.ToString();

                    var business_unit_id = "";
                    var business_unit = await dbContext.business_unit
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                        && o.business_unit_code.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(9)).ToUpper())
                        .FirstOrDefaultAsync();
                    if (business_unit != null) business_unit_id = business_unit.id.ToString();

                    var record = await dbContext.production_plan
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                 && o.production_plan_number == PublicFunctions.IsNullCell(row.GetCell(0)))
                        .FirstOrDefaultAsync();
                    var month = Convert.ToInt32(PublicFunctions.IsNullCell(row.GetCell(10)));
                    production_plan_monthly detail = null;
                    if (record != null)
                    {
                        detail = await dbContext.production_plan_monthly.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                     && o.production_plan_id == record.id && o.month_id == month)
                            .FirstOrDefaultAsync();
                    }
                    if (detail != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);
                        if (detail.quantity != PublicFunctions.Desimal(row.GetCell(11)))
                        {
                            #region History
                            var history = new production_plan_monthly_history();
                            history.InjectFrom(detail);
                            history.id = Guid.NewGuid().ToString("N");
                            history.production_plan_monthly_id = detail.id;
                            history.created_by = CurrentUserContext.AppUserId;
                            history.created_on = DateTime.Now;
                            dbContext.production_plan_monthly_history.Add(history);
                            #endregion
                        }
                        // record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.location_id = business_area_id;
                        record.master_list_id = plan_year_id;
                        record.mine_location_id = seam;
                        record.plan_type = plan_type;
                        record.activity_plan = activity_plan;
                        record.product_category_id = product_category;
                        record.product_id = product_name;
                        record.contractor_id = contractor_name;
                        record.business_unit_id = business_unit_id;
                        //detail.month_id = month;
                        detail.quantity = PublicFunctions.Desimal(row.GetCell(11));
                        await dbContext.SaveChangesAsync();

                        var planRecord = await dbContext.vw_production_plan.FirstOrDefaultAsync(o => o.id == detail.production_plan_id);
                        if (planRecord == null)
                            return BadRequest("No Production Plan Record.");

                        #region Daily all daily records
                        var planDailyRecords = await dbContext.production_plan_daily.Where(o => o.production_plan_monthly_id == detail.id).ToListAsync();
                        foreach (var item in planDailyRecords)
                            dbContext.production_plan_daily.Remove(item);
                        #endregion

                        int days = DateTime.DaysInMonth(int.Parse(planRecord.plan_year), (int)month);

                        decimal avgQuantity = 0;
                        if (detail.quantity > 0)
                            avgQuantity = (decimal)(detail.quantity / days);

                        for (var j = 1; j <= days; j++)
                        {
                            var dailyPlanRecord = new production_plan_daily();
                            dailyPlanRecord.id = Guid.NewGuid().ToString("N");
                            dailyPlanRecord.created_by = CurrentUserContext.AppUserId;
                            dailyPlanRecord.created_on = DateTime.Now;
                            dailyPlanRecord.modified_by = null;
                            dailyPlanRecord.modified_on = null;
                            dailyPlanRecord.is_active = true;
                            dailyPlanRecord.is_default = null;
                            dailyPlanRecord.is_locked = null;
                            dailyPlanRecord.entity_id = null;
                            dailyPlanRecord.owner_id = CurrentUserContext.AppUserId;
                            dailyPlanRecord.organization_id = CurrentUserContext.OrganizationId;

                            dailyPlanRecord.production_plan_monthly_id = detail.id;
                            dailyPlanRecord.daily_date = new DateTime(int.Parse(planRecord.plan_year), (int)month, j);
                            dailyPlanRecord.quantity = avgQuantity;

                            dbContext.production_plan_daily.Add(dailyPlanRecord);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        record = new production_plan()
                        {
                            id = Guid.NewGuid().ToString("N"),
                            created_by = CurrentUserContext.AppUserId,
                            created_on = DateTime.Now,
                            modified_by = null,
                            modified_on = null,
                            is_active = true,
                            is_default = null,
                            is_locked = null,
                            entity_id = null,
                            owner_id = CurrentUserContext.AppUserId,
                            organization_id = CurrentUserContext.OrganizationId,

                            production_plan_number = PublicFunctions.IsNullCell(row.GetCell(0)),
                            location_id = business_area_id,
                            master_list_id = plan_year_id,
                            mine_location_id = seam,
                            plan_type = plan_type,
                            activity_plan = activity_plan,
                            product_category_id = product_category,
                            product_id = product_name,
                            contractor_id = contractor_name,
                            business_unit_id = business_unit_id,

                        };
                        dbContext.production_plan.Add(record);
                        await dbContext.SaveChangesAsync();

                        for (var j = 1; j <= 12; j++)
                        {
                            var monthlyPlanRecord = new production_plan_monthly()
                            {
                                id = Guid.NewGuid().ToString("N"),
                                created_by = CurrentUserContext.AppUserId,
                                created_on = DateTime.Now,
                                modified_by = null,
                                modified_on = null,
                                is_active = true,
                                is_default = null,
                                is_locked = null,
                                entity_id = null,
                                owner_id = CurrentUserContext.AppUserId,
                                organization_id = CurrentUserContext.OrganizationId,

                                production_plan_id = record.id,
                                month_id = j,
                                quantity = (decimal?)0,
                            };

                            dbContext.production_plan_monthly.Add(monthlyPlanRecord);
                            await dbContext.SaveChangesAsync();
                        }
                        var detail2 = await dbContext.production_plan_monthly
                            .Where(o => o.production_plan_id == record.id && o.month_id == month)
                            .FirstOrDefaultAsync();
                        if (detail2 != null)
                        {
                            detail2.quantity = PublicFunctions.Desimal(row.GetCell(11));

                            var planRecord = await dbContext.vw_production_plan.FirstOrDefaultAsync(o => o.id == detail2.production_plan_id);
                            if (planRecord == null)
                                return BadRequest("No Production Plan Record.");
                            #region Daily all daily records
                            var planDailyRecords = await dbContext.production_plan_daily.Where(o => o.production_plan_monthly_id == detail2.id).ToListAsync();
                            foreach (var item in planDailyRecords)
                                dbContext.production_plan_daily.Remove(item);
                            #endregion

                            int days = DateTime.DaysInMonth(int.Parse(planRecord.plan_year), (int)month);

                            decimal avgQuantity = 0;
                            if (detail2.quantity > 0)
                                avgQuantity = (decimal)(detail2.quantity / days);

                            for (var j = 1; j <= days; j++)
                            {
                                var dailyPlanRecord = new production_plan_daily();
                                dailyPlanRecord.id = Guid.NewGuid().ToString("N");
                                dailyPlanRecord.created_by = CurrentUserContext.AppUserId;
                                dailyPlanRecord.created_on = DateTime.Now;
                                dailyPlanRecord.modified_by = null;
                                dailyPlanRecord.modified_on = null;
                                dailyPlanRecord.is_active = true;
                                dailyPlanRecord.is_default = null;
                                dailyPlanRecord.is_locked = null;
                                dailyPlanRecord.entity_id = null;
                                dailyPlanRecord.owner_id = CurrentUserContext.AppUserId;
                                dailyPlanRecord.organization_id = CurrentUserContext.OrganizationId;

                                dailyPlanRecord.production_plan_monthly_id = detail2.id;
                                dailyPlanRecord.daily_date = new DateTime(int.Parse(planRecord.plan_year), (int)month, j);
                                dailyPlanRecord.quantity = avgQuantity;

                                dbContext.production_plan_daily.Add(dailyPlanRecord);
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
                    else errormessage = ex.Message;

                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                    gagal = true;
                    break;
                }
            }

           /* sheet = wb.GetSheetAt(1); //*** detail sheet
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var production_plan_id = "";
                    var sales_plan = dbContext.vw_sales_plan
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.plan_year.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()).FirstOrDefault();
                    if (sales_plan != null) production_plan_id = sales_plan.id.ToString();

                    var detail = dbContext.production_plan_monthly.Where(o=>o.production_plan_id == production_plan_id).FirstOrDefault();

                    var record = dbContext.sales_plan_detail
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sales_plan_id.ToLower() == production_plan_id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.month_id = PublicFunctions.Bulat(row.GetCell(1));
                        record.quantity = PublicFunctions.Desimal(row.GetCell(2));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new sales_plan_detail();
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

                        record.sales_plan_id = production_plan_id;
                        record.month_id = PublicFunctions.Bulat(row.GetCell(1));
                        record.quantity = PublicFunctions.Desimal(row.GetCell(2));

                        dbContext.sales_plan_detail.Add(record);
                        await dbContext.SaveChangesAsync();

                      
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 2, Line " + (i + 1) + " : " + Environment.NewLine;
                    }
                    else errormessage = ex.Message;

                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                    gagal = true;
                    break;
                }
            }*/
            wb.Close();
            if (gagal)
            {
                await transaction.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "ProductionPlan");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }
    }
}

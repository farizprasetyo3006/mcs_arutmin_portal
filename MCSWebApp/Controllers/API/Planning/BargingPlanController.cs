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

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class BargingPlanController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public BargingPlanController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                return await DataSourceLoader.LoadAsync(dbContext.vw_barging_plan
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
                dbContext.barging_plan.Where(o => o.id == Id),
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
					if (await mcsContext.CanCreate(dbContext, nameof(barging_plan),
						CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
					{
                        var record = new barging_plan();
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

                        dbContext.barging_plan.Add(record);
                        await dbContext.SaveChangesAsync();
                        
                        if (string.IsNullOrEmpty(record.master_list_id))
                            return BadRequest("Year cannot be empty.");

                        var yearRecord = dbContext.master_list
                            .FirstOrDefault(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == record.master_list_id);

                        #region Barging Monthly
                        for (var i = 1; i <= 12; i++)
                        {
                            decimal totalShipmentQty = dbContext.shipment_plan
                                .Where(r => r.shipment_year == record.master_list_id && r.month_id == i)
                                .Select(t => t.qty_sp ?? 0).Sum();

                            var monthlyPlanRecord = new barging_plan_monthly();
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

                            monthlyPlanRecord.barging_plan_id = record.id;
                            monthlyPlanRecord.month_id = i;
                            monthlyPlanRecord.quantity = (decimal?)totalShipmentQty;

                            dbContext.barging_plan_monthly.Add(monthlyPlanRecord);
                            await dbContext.SaveChangesAsync();

                            #region Barging Daily
                            int days = DateTime.DaysInMonth(int.Parse(yearRecord.item_in_coding), i);
                            for (var j = 1; j <= days; j++)
                            {
                                var dailyPlanRecord = new barging_plan_daily();
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

                                dailyPlanRecord.barging_plan_monthly_id = monthlyPlanRecord.id;
                                dailyPlanRecord.daily_date = new DateTime(int.Parse(yearRecord.item_in_coding), i, j);
                                dailyPlanRecord.quantity = totalShipmentQty > 0 ? (totalShipmentQty / days) : 0;
                                dailyPlanRecord.operational_hours = 20;
                                dailyPlanRecord.loading_rate = dailyPlanRecord.quantity > 0 ? (dailyPlanRecord.quantity / dailyPlanRecord.operational_hours) : 0;

                                dbContext.barging_plan_daily.Add(dailyPlanRecord);
                                await dbContext.SaveChangesAsync();
                            }
                            #endregion
                        }
                        #endregion


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
                var record = dbContext.barging_plan
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

                       // record.InjectFrom(e);
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
                var record = dbContext.barging_plan
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.barging_plan.Remove(record);
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
                            var record = dbContext.barging_plan
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.barging_plan.Remove(record);
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

        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.barging_plan
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
        public async Task<IActionResult> SaveData([FromBody] barging_plan Record)
        {
            try
            {
                var record = dbContext.barging_plan
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
                else if (await mcsContext.CanCreate(dbContext, nameof(barging_plan),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new barging_plan();
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

                    dbContext.barging_plan.Add(record);
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
                var record = dbContext.barging_plan
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.barging_plan.Remove(record);
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
                var record = await dbContext.barging_plan
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

                    var plan_type = "";
                    var type = await dbContext.master_list
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower())
                        .FirstOrDefaultAsync();
                    if (type != null) plan_type = type.id.ToString();

                    var product_name = "";
                    var product = await dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.product_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower())
                        .FirstOrDefaultAsync();
                    if (product != null) product_name = product.id;

                    var business_unit_id = "";
                    var business_unit = await dbContext.business_unit
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.business_unit_code.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(7)).ToUpper())
                        .FirstOrDefaultAsync();
                    if (business_unit != null) business_unit_id = business_unit.id.ToString();

                    var contractor = "";
                    var c = await dbContext.contractor.Where(o => o.business_partner_code.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(6)).ToUpper()).FirstOrDefaultAsync();
                    if (c != null) contractor = c.id.ToString();

                    var productCategory = "";
                    var pc = await dbContext.product_category.Where(o => o.product_category_code.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(5)).ToUpper()).FirstOrDefaultAsync();
                    if (pc != null) productCategory = pc.id.ToString();

                    var activityPlan = "";
                    var ap = dbContext.master_list.Where(o => o.item_group == "activity-plan-type" && o.item_name.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(10)).ToLower().Trim()).FirstOrDefault();
                    if (ap != null) activityPlan = ap.id.ToString();

                    var record = await dbContext.barging_plan
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                 && o.barging_plan_number == PublicFunctions.IsNullCell(row.GetCell(0)))
                        .FirstOrDefaultAsync();
                    var month = Convert.ToInt32(PublicFunctions.IsNullCell(row.GetCell(8)));
                    barging_plan_monthly detail = null;
                    if (record != null)
                    {
                        detail = await dbContext.barging_plan_monthly.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                     && o.barging_plan_id == record.id && o.month_id == month)
                            .FirstOrDefaultAsync();
                    }
                    if (detail != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);
                        if (detail.quantity != PublicFunctions.Desimal(row.GetCell(9)))
                        {
                            #region History
                            var history = new barging_plan_monthly_history();
                            history.InjectFrom(detail);
                            history.id = Guid.NewGuid().ToString("N");
                            history.barging_plan_monthly_id = detail.id;
                            history.created_by = CurrentUserContext.AppUserId;
                            history.created_on = DateTime.Now;
                            dbContext.barging_plan_monthly_history.Add(history);
                            #endregion
                        }
                        // record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.activity_plan = activityPlan;
                        record.location_id = business_area_id;
                        record.master_list_id = plan_year_id;
                        record.plan_type = plan_type;
                        record.product_id = product_name;
                        record.business_unit_id = business_unit_id;
                        record.contractor_id = contractor;
                        record.product_category_id = productCategory;
                        //detail.month_id = month;
                        detail.quantity = PublicFunctions.Desimal(row.GetCell(9));
                        await dbContext.SaveChangesAsync();

                        var planRecord = await dbContext.vw_barging_plan.FirstOrDefaultAsync(o => o.id == detail.barging_plan_id);
                        if (planRecord == null)
                            return BadRequest("No Barging Plan Record.");

                        #region Daily all daily records
                        var planDailyRecords = await dbContext.barging_plan_daily.Where(o => o.barging_plan_monthly_id == detail.id).ToListAsync();
                        foreach (var item in planDailyRecords)
                            dbContext.barging_plan_daily.Remove(item);
                        #endregion

                        int days = DateTime.DaysInMonth(int.Parse(planRecord.plan_year), (int)month);

                        decimal avgQuantity = 0;
                        if (detail.quantity > 0)
                            avgQuantity = (decimal)(detail.quantity / days);

                        for (var j = 1; j <= days; j++)
                        {
                            var dailyPlanRecord = new barging_plan_daily();
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

                            dailyPlanRecord.barging_plan_monthly_id = detail.id;
                            dailyPlanRecord.daily_date = new DateTime(int.Parse(planRecord.plan_year), (int)month, j);
                            dailyPlanRecord.quantity = avgQuantity;

                            dbContext.barging_plan_daily.Add(dailyPlanRecord);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        record = new barging_plan()
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

                            barging_plan_number = PublicFunctions.IsNullCell(row.GetCell(0)),
                            location_id = business_area_id,
                            master_list_id = plan_year_id,
                            plan_type = plan_type,
                            product_id = product_name,
                            business_unit_id = business_unit_id,
                            contractor_id = contractor,
                            product_category_id = productCategory,
                            activity_plan = activityPlan
                        };
                        dbContext.barging_plan.Add(record);
                        await dbContext.SaveChangesAsync();

                        for (var j = 1; j <= 12; j++)
                        {
                            var monthlyPlanRecord = new barging_plan_monthly()
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

                                barging_plan_id = record.id,
                                month_id = j,
                                quantity = (decimal?)0,
                            };

                            dbContext.barging_plan_monthly.Add(monthlyPlanRecord);
                            await dbContext.SaveChangesAsync();
                        }
                        var detail2 = await dbContext.barging_plan_monthly
                            .Where(o => o.barging_plan_id == record.id && o.month_id == month)
                            .FirstOrDefaultAsync();
                        if (detail2 != null)
                        {
                            detail2.quantity = PublicFunctions.Desimal(row.GetCell(9));

                            var planRecord = await dbContext.vw_barging_plan.FirstOrDefaultAsync(o => o.id == detail2.barging_plan_id);
                            if (planRecord == null)
                                return BadRequest("No Barging Plan Record.");
                            #region Daily all daily records
                            var planDailyRecords = await dbContext.barging_plan_daily.Where(o => o.barging_plan_monthly_id == detail2.id).ToListAsync();
                            foreach (var item in planDailyRecords)
                                dbContext.barging_plan_daily.Remove(item);
                            #endregion

                            int days = DateTime.DaysInMonth(int.Parse(planRecord.plan_year), (int)month);

                            decimal avgQuantity = 0;
                            if (detail2.quantity > 0)
                                avgQuantity = (decimal)(detail2.quantity / days);

                            for (var j = 1; j <= days; j++)
                            {
                                var dailyPlanRecord = new barging_plan_daily();
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

                                dailyPlanRecord.barging_plan_monthly_id = detail2.id;
                                dailyPlanRecord.daily_date = new DateTime(int.Parse(planRecord.plan_year), (int)month, j);
                                dailyPlanRecord.quantity = avgQuantity;

                                dbContext.barging_plan_daily.Add(dailyPlanRecord);
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

            wb.Close();
            if (gagal)
            {
                await transaction.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "BargingPlan");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
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
    }
}

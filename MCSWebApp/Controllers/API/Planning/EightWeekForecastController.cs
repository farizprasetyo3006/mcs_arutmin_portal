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
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class EightWeekForecastController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public EightWeekForecastController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                return await DataSourceLoader.LoadAsync(dbContext.vw_eight_week_forecast
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
                dbContext.eight_week_forecast
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
                    if (await mcsContext.CanCreate(dbContext, nameof(eight_week_forecast),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new eight_week_forecast();
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

                        var versioningDat = await dbContext.eight_week_forecast
                            .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            .Where(x => x.business_unit_id == record.business_unit_id)
                            .Where(x => x.planning_number == record.planning_number)
                            .Where(x => x.year_id == record.year_id)
                            .OrderByDescending(x => x.version)
                            .FirstOrDefaultAsync();

                        if (versioningDat != null) record.version = versioningDat.version + 1;
                        else record.version = 1;

                        dbContext.eight_week_forecast.Add(record);
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
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.Message);
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
                var record = await dbContext.eight_week_forecast
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
                var record = dbContext.eight_week_forecast
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.eight_week_forecast.Remove(record);
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
                            var record = dbContext.eight_week_forecast
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.eight_week_forecast.Remove(record);
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

            using var tx = await dbContext.Database.BeginTransactionAsync();
            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var business_unit_id = string.Empty;
                    var business_unit = await dbContext.business_unit
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_name == PublicFunctions.IsNullCell(row.GetCell(0)))
                        .FirstOrDefaultAsync();
                    if (business_unit != null) business_unit_id = business_unit.id;

                    var planning_number = string.Empty;
                    planning_number = PublicFunctions.IsNullCell(row.GetCell(1));

                    var year_id = string.Empty;
                    var year = await dbContext.master_list
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.item_name == PublicFunctions.IsNullCell(row.GetCell(2)))
                        .Where(o => o.item_group == "years")
                        .FirstOrDefaultAsync();
                    if (year != null) year_id = year.id;

                    var activity_plan_id = string.Empty;
                    var activity_plan = await dbContext.master_list
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.item_name == PublicFunctions.IsNullCell(row.GetCell(3)))
                        .Where(o => o.item_group == "activity-plan-type")
                        .FirstOrDefaultAsync();
                    if (activity_plan != null) activity_plan_id = activity_plan.id;

                    var location_id = string.Empty;
                    var location = await dbContext.vw_business_area_breakdown_structure
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.child_2 != null)
                        .Where(o => o.child_3 == null)
                        .Where(o => o.business_area_name == PublicFunctions.IsNullCell(row.GetCell(4)))
                        .FirstOrDefaultAsync();
                    if (location != null) location_id = location.id;

                    var pit_id = string.Empty;
                    var pit = await dbContext.vw_business_area_breakdown_structure
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.child_4 != null)
                        .Where(o => o.child_5 == null)
                        .Where(o => o.business_area_name == PublicFunctions.IsNullCell(row.GetCell(5)))
                        .FirstOrDefaultAsync();
                    if (pit != null) pit_id = pit.id;

                    var product_category_id = string.Empty;
                    var product_category = await dbContext.product_category
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.product_category_name == PublicFunctions.IsNullCell(row.GetCell(6)))
                        .FirstOrDefaultAsync();
                    if (product_category != null) product_category_id = product_category.id;

                    var contractor_id = string.Empty;
                    var contractor = await dbContext.contractor
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_partner_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(7)).ToLower())
                        .FirstOrDefaultAsync();
                    if (contractor != null) contractor_id = contractor.id;

                    var uom_id = string.Empty;
                    var uom = await dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.uom_symbol == PublicFunctions.IsNullCell(row.GetCell(8)))
                        .FirstOrDefaultAsync();
                    if (uom != null) uom_id = uom.id;

                    int week = 0;
                    week = PublicFunctions.Bulat(row.GetCell(9));

                    DateTime? from_date = null;
                    from_date = PublicFunctions.TanggalNull(row.GetCell(10));

                    DateTime? to_date = null;
                    to_date = PublicFunctions.TanggalNull(row.GetCell(11));

                    var product_id = string.Empty;
                    var product = await dbContext.product
                        .Where(o => o.product_name == PublicFunctions.IsNullCell(row.GetCell(12)))
                        .FirstOrDefaultAsync();
                    if (product != null) product_id = product.id;

                    var recordHeader = await dbContext.eight_week_forecast
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == business_unit_id)
                        .Where(o => o.planning_number == planning_number)
                        .Where(o => o.year_id == year_id)
                        .FirstOrDefaultAsync();

                    if (recordHeader != null)
                    {
                        recordHeader.modified_by = CurrentUserContext.AppUserId;
                        recordHeader.modified_on = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                    }

                    else
                    {
                        recordHeader = new eight_week_forecast();
                        recordHeader.id = Guid.NewGuid().ToString("N");
                        recordHeader.created_by = CurrentUserContext.AppUserId;
                        recordHeader.created_on = DateTime.Now;
                        recordHeader.modified_by = null;
                        recordHeader.modified_on = null;
                        recordHeader.is_active = true;
                        recordHeader.is_default = null;
                        recordHeader.is_locked = null;
                        recordHeader.entity_id = null;
                        recordHeader.owner_id = CurrentUserContext.AppUserId;
                        recordHeader.organization_id = CurrentUserContext.OrganizationId;
                        recordHeader.business_unit_id = business_unit_id;

                        recordHeader.planning_number = planning_number;
                        recordHeader.year_id = year_id;
                        recordHeader.version = 1;
                        dbContext.eight_week_forecast.Add(recordHeader);
                        await dbContext.SaveChangesAsync();
                    }

                    var recordItem = await dbContext.eight_week_forecast_item
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.header_id == recordHeader.id)
                        .Where(o => o.activity_plan_id == activity_plan_id)
                        .Where(o => o.location_id == location_id)
                        .Where(o => o.business_area_pit_id == pit_id)
                        .Where(o=>o.contractor_id == contractor_id)
                        .FirstOrDefaultAsync();

                    if (recordItem != null)
                    {
                        recordItem.modified_by = CurrentUserContext.AppUserId;
                        recordItem.modified_on = DateTime.Now;

                        recordItem.product_category_id = product_category_id;
                        recordItem.contractor_id = contractor_id;
                        recordItem.uom_id = uom_id;

                        await dbContext.SaveChangesAsync();
                    }

                    else
                    {
                        recordItem = new eight_week_forecast_item();
                        recordItem.id = Guid.NewGuid().ToString("N");
                        recordItem.created_by = CurrentUserContext.AppUserId;
                        recordItem.created_on = DateTime.Now;
                        recordItem.modified_by = null;
                        recordItem.modified_on = null;
                        recordItem.is_active = true;
                        recordItem.is_default = null;
                        recordItem.is_locked = null;
                        recordItem.entity_id = null;
                        recordItem.owner_id = CurrentUserContext.AppUserId;
                        recordItem.organization_id = CurrentUserContext.OrganizationId;
                        recordItem.business_unit_id = business_unit_id;

                        recordItem.header_id = recordHeader.id;
                        recordItem.activity_plan_id = activity_plan_id;
                        recordItem.location_id = location_id;
                        recordItem.business_area_pit_id = pit_id;
                        recordItem.product_category_id = product_category_id;
                        recordItem.contractor_id = contractor_id;
                        recordItem.uom_id = uom_id;
                        dbContext.eight_week_forecast_item.Add(recordItem);
                        await dbContext.SaveChangesAsync();
                    }

                    var recordDetail = await dbContext.eight_week_forecast_item_detail
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.header_id == recordHeader.id)
                        .Where(o => o.item_id == recordItem.id)
                        .Where(o => o.from_date == from_date)
                        .Where(o => o.to_date == to_date)
                        .Where(o => o.product_id == product_id)
                        .FirstOrDefaultAsync();

                    if (recordDetail != null)
                    {
                        recordDetail.modified_by = CurrentUserContext.AppUserId;
                        recordDetail.modified_on = DateTime.Now;

                        recordDetail.quantity = PublicFunctions.Desimal(row.GetCell(13));
                        recordDetail.ash_adb = PublicFunctions.Desimal(row.GetCell(14));
                        recordDetail.ts_adb = PublicFunctions.Desimal(row.GetCell(15));
                        recordDetail.im_adb = PublicFunctions.Desimal(row.GetCell(16));
                        recordDetail.tm_arb = PublicFunctions.Desimal(row.GetCell(17));
                        recordDetail.gcv_gad = PublicFunctions.Desimal(row.GetCell(18));
                        recordDetail.gcv_gar = PublicFunctions.Desimal(row.GetCell(19));
                        recordDetail.is_using = PublicFunctions.BenarSalah(row.GetCell(20));
                        bool bUpdated = await RecalculateHeaderItem(recordDetail.item_id);
                        await dbContext.SaveChangesAsync();
                    }

                    else
                    {
                        recordDetail = new eight_week_forecast_item_detail();
                        recordDetail.id = Guid.NewGuid().ToString("N");
                        recordDetail.created_by = CurrentUserContext.AppUserId;
                        recordDetail.created_on = DateTime.Now;
                        recordDetail.modified_by = null;
                        recordDetail.modified_on = null;
                        recordDetail.is_active = true;
                        recordDetail.is_default = null;
                        recordDetail.is_locked = null;
                        recordDetail.entity_id = null;
                        recordDetail.owner_id = CurrentUserContext.AppUserId;
                        recordDetail.organization_id = CurrentUserContext.OrganizationId;
                        recordDetail.business_unit_id = business_unit_id;

                        recordDetail.header_id = recordHeader.id;
                        recordDetail.item_id = recordItem.id;
                        recordDetail.week_id = week.ToString();
                        recordDetail.from_date = from_date;
                        recordDetail.to_date = to_date;
                        recordDetail.product_id = product_id;
                        recordDetail.quantity = PublicFunctions.Desimal(row.GetCell(13));
                        recordDetail.ash_adb = PublicFunctions.Desimal(row.GetCell(14));
                        recordDetail.ts_adb = PublicFunctions.Desimal(row.GetCell(15));
                        recordDetail.im_adb = PublicFunctions.Desimal(row.GetCell(16));
                        recordDetail.tm_arb = PublicFunctions.Desimal(row.GetCell(17));
                        recordDetail.gcv_gad = PublicFunctions.Desimal(row.GetCell(18));
                        recordDetail.gcv_gar = PublicFunctions.Desimal(row.GetCell(19));
                        recordDetail.is_using = PublicFunctions.BenarSalah(row.GetCell(20));
                        bool bUpdated = await RecalculateHeaderItem(recordDetail.item_id);
                        dbContext.eight_week_forecast_item_detail.Add(recordDetail);
                        await dbContext.SaveChangesAsync();
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
                await tx.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "EightWeekForecast");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await tx.CommitAsync();
                return "File berhasil di-upload!";
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
        #region Lookup Header

        [HttpGet("BusinessUnitIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ContractorIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.business_unit
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .OrderBy(o => o.business_unit_name)
                    .Select(o => new { Value = o.id, Text = o.business_unit_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("YearIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProductCategoryIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.master_list
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.item_group == "years")
                    .Select(o => new { Value = o.id, Text = o.item_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("UomIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UomIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.uom
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.uom_symbol, Symbol = o.uom_symbol, Search = o.uom_symbol.ToLower() + o.uom_symbol.ToUpper() });

                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        #endregion

    }
}

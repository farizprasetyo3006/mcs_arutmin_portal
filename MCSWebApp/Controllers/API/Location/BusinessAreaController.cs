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
using Microsoft.EntityFrameworkCore;

namespace MCSWebApp.Controllers.API.Location
{
    [Route("api/Location/[controller]")]
    [ApiController]
    public class BusinessAreaController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public BusinessAreaController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_business_area
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataGridByLatest")]
        public async Task<object> DataGridByLatest(DataSourceLoadOptions loadOptions, string latestUpdate)
        {
            DateTime modifiedOn = default;
            if (!string.IsNullOrEmpty(latestUpdate))
            {
                DateTime.TryParse(latestUpdate, out modifiedOn);
            }

            return await DataSourceLoader.LoadAsync(dbContext.vw_business_area
                .Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.modified_on > modifiedOn || (o.modified_on == null && o.created_on > DateTime.Parse(latestUpdate))),
                loadOptions);
        }

        [HttpGet("DataDetail")]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.business_area
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.id == Id),
                loadOptions);
        }

        [HttpGet("GetParent")]
        public async Task<object> GetParent(DataSourceLoadOptions loadOptions, string Id)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.business_area
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Id), loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("InsertData")]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            await using var tx = await dbContext.Database.BeginTransactionAsync();
            try
            {
                if (!await mcsContext.CanCreate(dbContext, nameof(business_area),
                        CurrentUserContext.AppUserId) &&
                    !CurrentUserContext.IsSysAdmin) return BadRequest("User is not authorized.");

                var record = new business_area();
                JsonConvert.PopulateObject(values, record);

                var cekdata = dbContext.business_area
                    .FirstOrDefault(o => o.organization_id == CurrentUserContext.OrganizationId
                                         && o.business_area_code.ToLower().Trim() ==
                                         record.business_area_code.ToLower().Trim()
                                         && o.parent_business_area_id == record.parent_business_area_id);

                if (cekdata != null) return BadRequest("Duplicate Code field.");

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
                record.parent_business_area_id = string.IsNullOrEmpty(record.parent_business_area_id)
                    ? "0"
                    : record.parent_business_area_id;
                record.business_area_code = record.business_area_code.ToUpper();

                dbContext.business_area.Add(record);
                await dbContext.SaveChangesAsync();
                await tx.CommitAsync();
                return Ok(record);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut("UpdateData")]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.business_area
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);

                        var cekdata = dbContext.business_area
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.business_area_code.ToLower().Trim() == record.business_area_code.ToLower().Trim()
                                && o.parent_business_area_id == record.parent_business_area_id
                                && o.id != record.id)
                            .FirstOrDefault();
                        if (cekdata != null) return BadRequest("Duplicate Code field.");

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        if (record.business_unit_id == null) record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

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
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var timesheet_detail = dbContext.timesheet_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.pit_id == key).FirstOrDefault();
                if (timesheet_detail != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var record = dbContext.business_area
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.business_area.Remove(record);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        return BadRequest("User is not authorized.");
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] business_area Record)
        {
            try
            {
                var record = dbContext.business_area
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Record.id)
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
                else if (await mcsContext.CanCreate(dbContext, nameof(business_area),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new business_area();
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

                    dbContext.business_area.Add(record);
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

        //     [HttpGet("ParentBusinessAreaIdLookup")]
        //     public async Task<object> ParentBusinessAreaIdLookup(DataSourceLoadOptions loadOptions)
        //     {
        //         try
        //         {
        //             var lookup = dbContext.vw_business_area
        //                 .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
        //                 .Select(o => new { Value = o.id, Text = o.business_area_name });
        //             return await DataSourceLoader.LoadAsync(lookup, loadOptions);
        //         }
        //catch (Exception ex)
        //{
        //	logger.Error(ex.InnerException ?? ex);
        //             return BadRequest(ex.InnerException?.Message ?? ex.Message);
        //}
        //     }

        [HttpGet("StockLocations/{id}")]
        public async Task<object> StockLocations(string id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.stock_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.business_area_id == id)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("BusinessAreaChild5IdLookup")]
        public async Task<object> BusinessAreaChild5IdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_area_breakdown_structure
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.child_5 != null)
                    .Where(o => o.child_6 == null)
                    .Select(o => new { Value = o.id, Text = o.business_area_name, Search = o.business_area_name.ToLower() + o.business_area_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("BusinessAreaChild6IdLookup")]
        public async Task<object> BusinessAreaChild6IdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_area_breakdown_structure
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.child_6 != null)
                    .Select(o => new { Value = o.id, Text = o.business_area_name, Search = o.business_area_name.ToLower() + o.business_area_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("BusinessAreaIdLookup")]
        public async Task<object> BusinessAreaIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_area_structure
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.business_area_name, Search = o.business_area_name.ToLower() + o.business_area_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("BusinessAreaChild2IdLookup")]
        public async Task<object> BusinessAreaChild2IdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_area_breakdown_structure
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.child_2 != null && o.child_3 == null && o. child_4 == null && o.child_5 == null && o.child_6 == null)
                    .Select(o => new { Value = o.id, Text = o.business_area_name, Search = o.business_area_name.ToLower() + o.business_area_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("BusinessAreaChild4IdLookup")]
        public async Task<object> BusinessAreaChild4IdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_area_breakdown_structure
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.child_4 != null && o.child_5 == null && o.child_6 == null)
                    .Select(o => new { Value = o.id, Text = o.business_area_name, Search = o.business_area_name.ToLower() + o.business_area_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("BusinessAreaIdLookupNoFilter")]
        public async Task<object> BusinessAreaIdLookupNoFilter(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_area_structure
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.business_area_name, Search = o.business_area_name.ToLower() + o.business_area_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("BusinessAreaSubPitIdLookup")]
        public async Task<object> BusinessAreaSubPitIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_area_breakdown_structure
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.child_5 != null && o.child_6 == null)
                    .Select(o => new { Value = o.id, Text = o.business_area_name, Search = o.business_area_name.ToLower() + o.business_area_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("BusinessAreaByIdLookup")]
        public async Task<object> BusinessAreaByIdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                dbContext.vw_business_area_structure.Where(o => o.id == Id &&
                    (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)),
                loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ChildrenBusinessAreaIdLookup")]
        public async Task<object> ChildrenBusinessAreaIdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_business_area_structure
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id_path.Contains(Id))
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.name_path, Search = o.name_path.ToLower() + o.name_path.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("UploadDocument")]
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

                    var parent_business_area_id = "0";
                    var business_area = dbContext.business_area
                        .Where(o => o.business_area_code == PublicFunctions.IsNullCell(row.GetCell(0))
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (business_area != null) parent_business_area_id = business_area.id.ToString();

                    var business_unit_id = "";
                    var BU = dbContext.business_unit.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.business_unit_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                    if (BU != null) business_unit_id = BU.id.ToString();

                    var record = dbContext.business_area
                        .Where(o => o.business_area_code == PublicFunctions.IsNullCell(row.GetCell(1))
                            && o.parent_business_area_id == parent_business_area_id
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                       // record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        record.business_unit_id = business_unit_id;

                        record.parent_business_area_id = parent_business_area_id;
                        record.business_area_code = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.business_area_name = PublicFunctions.IsNullCell(row.GetCell(2));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new business_area();
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
                        record.business_unit_id = business_unit_id;

                        record.parent_business_area_id = parent_business_area_id;
                        record.business_area_code = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.business_area_name = PublicFunctions.IsNullCell(row.GetCell(2));

                        dbContext.business_area.Add(record);
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
                await transaction.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "BusinessArea");
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

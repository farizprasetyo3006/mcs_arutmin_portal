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

namespace MCSWebApp.Controllers.API.General
{
    [Route("api/General/[controller]")]
    [ApiController]
    public class PriceIndexController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public PriceIndexController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
           /* var record = dbContext.vw_price_index
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                .FirstOrDefault();
            if (record != null)
            {
                if ((await mcsContext.CanRead(dbContext, record.id, CurrentUserContext.AppUserId, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                    || CurrentUserContext.IsSysAdmin) == false)
                    return BadRequest("User is not authorized.");
            }*/

            return await DataSourceLoader.LoadAsync(dbContext.vw_price_index
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
               // .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_price_index.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId
                    && CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(price_index),
					CurrentUserContext.AppUserId, HttpContext.Session.GetString("BUSINESS_UNIT_ID")) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new price_index();
                    JsonConvert.PopulateObject(values, record);

                    var cekdata = dbContext.price_index
                        .Where(o => o.price_index_code.ToLower().Trim() == record.price_index_code.ToLower().Trim()
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (cekdata != null) return BadRequest("Duplicate Code field.");

                    if (record.is_base_index == true)
                    {
                        var cekbaseindex = dbContext.price_index
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.is_base_index == true)
                            .FirstOrDefault();
                        if (cekbaseindex != null) return BadRequest("Cannot set more than 1 Index as a Base Index.");
                    }

                    record.id = Guid.NewGuid().ToString("N");
                    record.created_by = CurrentUserContext.AppUserId;
                    record.created_on = DateTime.Now;
                    record.modified_by = null;
                    record.modified_on = null;
                    //record.is_active = true;
                    record.is_default = null;
                    record.is_locked = null;
                    record.entity_id = null;
                    record.owner_id = CurrentUserContext.AppUserId;
                    record.organization_id = CurrentUserContext.OrganizationId;
					//record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    dbContext.price_index.Add(record);
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
                var record = dbContext.price_index
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);

                        if (record.is_base_index == true)
                        {
                            var cekbaseindex = dbContext.price_index
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.id != record.id
                                    && o.is_base_index == true)
                                .FirstOrDefault();
                            if (cekbaseindex != null) return BadRequest("Cannot set more than 1 Index as a Base Index.");
                        }

                        var cekdata = dbContext.price_index
                            .Where(o => o.price_index_code.ToLower().Trim() == record.price_index_code.ToLower().Trim()
                                && o.id != record.id
                                && o.organization_id == CurrentUserContext.OrganizationId)
                            .FirstOrDefault();
                        if (cekdata != null) return BadRequest("Duplicate Code field.");

                        //if (!BaseIndexValidation(record.id))
                        //    throw new Exception("Cannot set more than 1 Index as a Base Index");

                        var is_active = record.is_active;
                       // record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        record.is_active = is_active;

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
                var record = dbContext.price_index
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.price_index.Remove(record);
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
        public async Task<IActionResult> SaveData([FromBody] mine_location Record)
        {
            try
            {
                var record = dbContext.price_index
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

                        //if (!BaseIndexValidation(record.id))
                        //    throw new Exception("Cannot set more than 1 Index as a Base Index");
                        if (record.is_base_index == true)
                        {
                            var cekbaseindex = dbContext.price_index
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.id != record.id
                                    && o.is_base_index == true)
                                .FirstOrDefault();
                            if (cekbaseindex != null) return BadRequest("Cannot set more than 1 Index as a Base Index.");
                        }

                        await dbContext.SaveChangesAsync();
                        return Ok(record);
					}
					else
					{
						return BadRequest("User is not authorized.");
					}
                }
                else if (await mcsContext.CanCreate(dbContext, nameof(price_index),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new price_index();
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

                    if (record.is_base_index == true)
                    {
                        var cekbaseindex = dbContext.price_index
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.is_base_index == true)
                            .FirstOrDefault();
                        if (cekbaseindex != null) return BadRequest("Cannot set more than 1 Index as a Base Index.");
                    }

                    //if (!BaseIndexValidation(record.id))
                    //    throw new Exception("Cannot set more than 1 Index as a Base Index");

                    dbContext.price_index.Add(record);
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
            logger.Trace($"string Id = {Id}");

            try
            {
                var record = dbContext.price_index
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.price_index.Remove(record);
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
            if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);

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

                    var business_unit_id = "";
                    var BU = dbContext.business_unit.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.business_unit_code == PublicFunctions.IsNullCell(row.GetCell(4)).ToUpper()).FirstOrDefault();
                    if (BU != null) business_unit_id = BU.id.ToString();

                    var record = dbContext.price_index
                        .Where(o => o.price_index_code == PublicFunctions.IsNullCell(row.GetCell(0)) 
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        // record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.price_index_name = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.is_active = PublicFunctions.BenarSalah(row.GetCell(2));
                        record.is_base_index = PublicFunctions.BenarSalah(row.GetCell(3));
                        record.business_unit_id = business_unit_id;

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new price_index();
                        record.id = Guid.NewGuid().ToString("N");
                        record.created_by = CurrentUserContext.AppUserId;
                        record.created_on = DateTime.Now;
                        record.modified_by = null;
                        record.modified_on = null;
                        //record.is_active = true;
                        record.is_default = null;
                        record.is_locked = null;
                        record.entity_id = null;
                        record.owner_id = CurrentUserContext.AppUserId;
                        record.organization_id = CurrentUserContext.OrganizationId;
                        record.business_unit_id = business_unit_id;


                        record.price_index_code = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.price_index_name = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.is_active = PublicFunctions.BenarSalah(row.GetCell(2));
                        record.is_base_index = PublicFunctions.BenarSalah(row.GetCell(3));

                        dbContext.price_index.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 1, Line " + (i+1) + " : " + Environment.NewLine;
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
                HttpContext.Session.SetString("filename", "PriceIndex");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpGet("PriceIndexIdLookup")]
        public async Task<object> PriceIndexIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.price_index
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
					//.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.price_index_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("GetPriceIndex")]
        public async Task<object> GetPriceIndex(DataSourceLoadOptions loadOptions)
        {
            var list = dbContext.vw_price_index
                .Where(r => (CustomFunctions.CanRead(r.id, CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin))
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .OrderBy(r => r.price_index_name);
            return await DataSourceLoader.LoadAsync(list, loadOptions);
        }

        [HttpGet("GetPriceIndexHistory")]
        public async Task<object> GetPriceIndexHistory(DataSourceLoadOptions loadOptions)
        {
            var list = dbContext.vw_price_index_history
                .Where(r => (CustomFunctions.CanRead(r.id, CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin))
                .Where(r => (r.index_date.Value.Month > DateTime.Now.Month - 3) && r.index_date.Value.Year == DateTime.Now.Year)
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .OrderBy(r => r.price_index_name)
                .ThenBy(r => r.index_date);
            return await DataSourceLoader.LoadAsync(list, loadOptions);
        }

        [NonAction] // Harus diberi attribute spt ini spy tdk masuk swagger
        public bool BaseIndexValidation(string recordId)
        {
            var result = true;
            var baseIndexRecord = dbContext.price_index
                    .Where(o => o.is_base_index ?? false == true)
                    .FirstOrDefault();
            if (baseIndexRecord != null && baseIndexRecord.id != recordId)
            {
                result = false;
            }
            return result;
        }

    }
}

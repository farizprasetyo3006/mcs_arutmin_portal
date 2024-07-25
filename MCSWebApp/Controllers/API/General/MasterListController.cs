using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DevExtreme.AspNet.Data;
using DataAccess.EFCore;
using NLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataAccess.DTO;
using Omu.ValueInjecter;
using Microsoft.EntityFrameworkCore;
using DataAccess.EFCore.Repository;
using DevExtreme.AspNet.Mvc;
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
    public class MasterListController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public MasterListController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            var record = dbContext.master_list
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .FirstOrDefault();
            if (record != null)
            {
                if ((await mcsContext.CanRead(dbContext, record.id, CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin) == false)
                    return BadRequest("User is not authorized.");
            }

            return await DataSourceLoader.LoadAsync(dbContext.master_list
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.master_list.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(master_list),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new master_list();
                    JsonConvert.PopulateObject(values, record);

                    var cekdata = dbContext.master_list
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.item_in_coding.ToLower() == record.item_in_coding.ToLower()
                            && o.item_group.ToLower().Trim() == record.item_group.ToLower().Trim())
                        .FirstOrDefault();
                    if (cekdata != null)
                    {
                        logger.Debug("MasterList|record.item_in_coding: " + record.item_in_coding.ToLower());
                        logger.Debug("record.item_group: " + record.item_group.ToLower());

                        return BadRequest("Duplicate Item Code field.");
                    }

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

                    record.item_in_coding = record.item_in_coding.Trim().Replace("  ", " ").ToUpper();

                    dbContext.master_list.Add(record);
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
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.master_list
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

                        var cekdata = dbContext.master_list
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.item_in_coding.ToLower() == record.item_in_coding.ToLower()
                                && o.item_group.ToLower().Trim() == record.item_group.ToLower().Trim()
                                && o.id != record.id)
                            .FirstOrDefault();
                        if (cekdata != null)
                        {
                            logger.Debug("MasterList|record.item_in_coding: " + record.item_in_coding.ToLower());
                            logger.Debug("record.item_group: " + record.item_group.ToLower());

                            return BadRequest("Duplicate Item Code field.");
                        }

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.item_in_coding = record.item_in_coding.Trim().Replace("  ", " ").ToUpper();

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
            logger.Trace($"string key = {key}");

            try
            {
                var record = dbContext.master_list
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.master_list.Remove(record);
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

        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.master_list
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                return Ok(record);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("GetByGroup/{Id}")]
        public async Task<IActionResult> GetByGroup(string Id)
        {
            try
            {
                var record = await dbContext.master_list
                    .Where(o => o.item_group == Id && o.organization_id == CurrentUserContext.OrganizationId).ToListAsync();
                return Ok(record);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("MasterListIdLookup")]
        public async Task<object> MasterListIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.master_list
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .OrderBy(o => o.sort_number).ThenBy(o => o.item_name)
                    .Select(o => new { Value = o.id, Text = o.item_name, o.item_group, o.item_in_coding, Search = o.item_name.ToLower() + o.item_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("ItemClass")]
        public async Task<object> ItemClass(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.item_class_view
                    //.Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .Select(o => new {Text = o.item_group });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("MasterListIdStatus")]
        public async Task<object> MasterListIdStatus(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.master_list
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.id == Id)
                    .Select(o => new { Value = o.id, Text = o.item_name, o.item_group, o.item_in_coding, Search = o.item_name.ToLower() + o.item_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

    //    [HttpGet("MasterListIdLookupByLatest")]
    //    public async Task<object> MasterListIdLookupByLatest(DataSourceLoadOptions loadOptions, string latestUpdate)
    //    {
    //        try
    //        {
    //            DateTime modiefiedOn = default;
    //            if (!string.IsNullOrEmpty(latestUpdate))
    //            {
    //                DateTime.TryParse(latestUpdate, out modiefiedOn);
    //            }

    //            var lookup = dbContext.master_list
    //                .Where(o => (o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
    //                    && o.modified_on > modiefiedOn || (o.modified_on == null 
    //                    && o.created_on > DateTime.Parse(latestUpdate)))
    //                .Select(o => new { Value = o.id, Text = o.item_name, o.item_group });
    //            return await DataSourceLoader.LoadAsync(lookup, loadOptions);
    //        }
    //        catch (Exception ex)
    //        {
				//logger.Error(ex.InnerException ?? ex);
				//return BadRequest(ex.InnerException?.Message ?? ex.Message);
    //        }
    //    }

    //    [HttpGet("MasterListIdLookupByItemClass")]
    //    public async Task<object> MasterListIdLookupByItemClass(DataSourceLoadOptions loadOptions)
    //    {
    //        try
    //        {
    //            var lookup = dbContext.master_list
    //                .Where(o => (o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
    //                    && o.item_group.ToLower() == "contract-item-class")
    //                .Select(o => new { Value = o.id, Text = o.item_name, o.item_group });
    //            return await DataSourceLoader.LoadAsync(lookup, loadOptions);
    //        }
    //        catch (Exception ex)
    //        {
				//logger.Error(ex.InnerException ?? ex);
				//return BadRequest(ex.InnerException?.Message ?? ex.Message);
    //        }
    //    }

        [HttpGet("MasterListIdLookupByItemGroup")]
        public async Task<object> MasterListIdLookupByItemGroup(DataSourceLoadOptions loadOptions, string itemGroup)
        {
            try
            {
                var lookup = dbContext.master_list
                    .Where(o => (o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                        && o.item_group.ToLower() == itemGroup)
                    .OrderBy(o => o.sort_number).ThenBy(o => o.item_name)
                    .Select(o => new { Value = o.id, Text = o.item_name, Search = o.item_name.ToLower() + o.item_name.ToUpper() });
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

                    var record = dbContext.master_list
                        .Where(o => o.item_name.ToLower().Trim() == row.GetCell(0).ToString().ToLower().Trim()
                            && o.item_group.ToLower().Trim() == row.GetCell(1).ToString().ToLower().Trim()
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.item_group = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.item_in_coding = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.notes = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.sort_number = PublicFunctions.Bulat(row.GetCell(4));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new master_list();
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

                        record.item_name = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.item_group = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.item_in_coding = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.notes = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.sort_number = PublicFunctions.Bulat(row.GetCell(4));

                        dbContext.master_list.Add(record);
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
                HttpContext.Session.SetString("filename", "MasterList");
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

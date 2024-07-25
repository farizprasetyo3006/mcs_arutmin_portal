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

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("/api/Planning/[controller]")]
    [ApiController]
    public class GeologyController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public GeologyController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                return await DataSourceLoader.LoadAsync(dbContext.mine_plan_geology
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
                dbContext.mine_plan_geology.Where(o => o.id == Id),
                loadOptions);
        }
        [HttpGet("HistoryById")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> HistoryById(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_mineplan_geology_history
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.header_id == Id),
                loadOptions);
        }
        [HttpPost("InsertData")]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(mine_plan_geology),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new mine_plan_geology();
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
                        dbContext.mine_plan_geology.Add(record);
                        await dbContext.SaveChangesAsync();

                        var history = new mineplan_geology_history();
                        JsonConvert.PopulateObject(values, history);
                        history.id = Guid.NewGuid().ToString("N");
                        history.created_by = CurrentUserContext.AppUserId;
                        history.created_on = DateTime.Now;
                        history.modified_by = null;
                        history.modified_on = null;
                        history.is_active = true;
                        history.is_default = null;
                        history.is_locked = null;
                        history.entity_id = null;
                        history.owner_id = CurrentUserContext.AppUserId;
                        history.organization_id = CurrentUserContext.OrganizationId;
                        history.header_id = record.id;
                        dbContext.mineplan_geology_history.Add(history);
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
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.mine_plan_geology
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

                        //record.InjectFrom(values);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        await dbContext.SaveChangesAsync();
                        #region history
                        var history = new mineplan_geology_history();
                        history.id = Guid.NewGuid().ToString("N");
                        history.created_by = CurrentUserContext.AppUserId;
                        history.created_on = DateTime.Now;
                        history.modified_by = null;
                        history.modified_on = null;
                        history.is_active = true;
                        history.is_default = null;
                        history.is_locked = null;
                        history.entity_id = null;
                        history.owner_id = CurrentUserContext.AppUserId;
                        history.organization_id = CurrentUserContext.OrganizationId;
                        history.business_unit_id = record.business_unit_id;
                        history.truethick = record.truethick;
                        history.mass_tonnage = record.mass_tonnage;
                        history.tm_ar = record.tm_ar;
                        history.im_adb = record.im_adb;
                        history.ash_adb = record.ash_adb;
                        history.vm_adb = record.vm_adb;
                        history.fc_adb = record.fc_adb;
                        history.ts_adb = record.ts_adb;
                        history.cv_adb = record.cv_adb;
                        history.cv_arb = record.cv_arb;
                        history.rd = record.rd;
                        history.rdi = record.rdi;
                        history.hgi = record.hgi;
                        history.resource_type_id = record.resource_type_id;
                        history.coal_type_id = record.coal_type_id;
                        history.model_data = record.model_data;
                        history.header_id = record.id;
                        dbContext.mineplan_geology_history.Add(history);
                        await dbContext.SaveChangesAsync();
                        #endregion
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
                var record = dbContext.mine_plan_geology
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.mine_plan_geology.Remove(record);
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
                var record = await dbContext.mine_plan_geology
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
        public async Task<IActionResult> SaveData([FromBody] mine_plan_geology Record)
        {
            try
            {
                var record = dbContext.mine_plan_geology
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
                    record = new mine_plan_geology();
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

                    dbContext.mine_plan_geology.Add(record);
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
                var record = dbContext.mine_plan_geology
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.mine_plan_geology.Remove(record);
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
                            var record = dbContext.mine_plan_geology
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.mine_plan_geology.Remove(record);
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

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var resource_id = "";
                    var materialType = dbContext.master_list
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(16)).ToLower()).FirstOrDefault();
                    if (materialType != null) resource_id = materialType.id.ToString();

                    var business_unit_id = "";
                    var business_unit = dbContext.business_unit.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.business_unit_name.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(19)).ToUpper()).FirstOrDefault();
                    if (business_unit != null) business_unit_id = business_unit.id.ToString();

                    var coal_type_id = "";
                    var reserve_type = dbContext.master_list
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(17)).ToLower()).FirstOrDefault(); 
                    if (reserve_type != null) coal_type_id = reserve_type.id.ToString();

                    string mine = PublicFunctions.IsNullCell(row.GetCell(0));
                    string submine = PublicFunctions.IsNullCell(row.GetCell(1));
                    string seam = PublicFunctions.IsNullCell(row.GetCell(2));
                    var record = dbContext.mine_plan_geology
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.mine_code == mine && o.submine_code == submine && o.seam_code == seam &&
                        o.coal_type_id == coal_type_id && o.resource_type_id == resource_id && o.business_unit_id == business_unit_id)
                        .FirstOrDefault();
                    
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        // record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        //record.name = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.mine_code = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.submine_code = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.seam_code = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.truethick = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(3)));
                        record.mass_tonnage = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(4)));
                        record.tm_ar = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(5)));
                        record.coal_type_id = coal_type_id;
                        record.resource_type_id = resource_id;
                        record.im_adb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(6)));
                        record.ash_adb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(7)));
                        record.vm_adb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(8)));
                        record.fc_adb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(9)));
                        record.ts_adb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(10)));
                        record.cv_adb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(11)));
                        record.cv_arb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(12)));
                        record.rd = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(13)));
                        record.rdi = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(14)));
                        record.hgi = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(15)));
                        record.model_data = PublicFunctions.IsNullCell(row.GetCell(18));
                        record.business_unit_id = business_unit_id;


                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new mine_plan_geology();
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

                        record.mine_code = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.submine_code = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.seam_code = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.truethick = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(3)));
                        record.mass_tonnage = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(4)));
                        record.tm_ar = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(5)));
                        record.coal_type_id = coal_type_id;
                        record.resource_type_id = resource_id;
                        record.im_adb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(6)));
                        record.ash_adb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(7)));
                        record.vm_adb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(8)));
                        record.fc_adb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(9)));
                        record.ts_adb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(10)));
                        record.cv_adb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(11)));
                        record.cv_arb = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(12)));
                        record.rd = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(13)));
                        record.rdi = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(14)));
                        record.hgi = Convert.ToDecimal(PublicFunctions.IsNullCell(row.GetCell(15)));
                        record.model_data = PublicFunctions.IsNullCell(row.GetCell(18));
                        record.business_unit_id = business_unit_id;


                        dbContext.mine_plan_geology.Add(record);
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
                HttpContext.Session.SetString("filename", "Barge");
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

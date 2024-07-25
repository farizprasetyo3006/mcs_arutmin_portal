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
using Microsoft.EntityFrameworkCore;
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
    public class EmployeeController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public EmployeeController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_employee
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataGridByLatest")]
        public async Task<object> DataGridByLatest(DataSourceLoadOptions loadOptions, string latestUpdate)
        {
            DateTime modiefiedOn = default;
            if (!string.IsNullOrEmpty(latestUpdate))
            {
                DateTime.TryParse(latestUpdate, out modiefiedOn);
            }

            return await DataSourceLoader.LoadAsync(dbContext.vw_employee
                .Where(o => CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.modified_on > modiefiedOn || (o.modified_on == null && o.created_on > DateTime.Parse(latestUpdate))),
                loadOptions);
        }

        [HttpGet("DataDetail")]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.employee.Where(o => o.id == Id && o.organization_id == CurrentUserContext.OrganizationId), 
                loadOptions);
        }

        [HttpPost("InsertData")]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(employee),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new employee();
                    JsonConvert.PopulateObject(values, record);

                    var cekdata = dbContext.employee
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.employee_number.ToLower().Trim() == record.employee_number.ToLower().Trim())
                        .FirstOrDefault();
                    if (cekdata != null) return BadRequest("Duplicate Employee Number field.");

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
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    dbContext.employee.Add(record);
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
                var record = dbContext.employee
                    .Where(o => o.id == key && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);

                        var cekdata = dbContext.employee
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.employee_number.ToLower().Trim() == record.employee_number.ToLower().Trim()
                                && o.id != record.id)
                            .FirstOrDefault();
                        if (cekdata != null) return BadRequest("Duplicate Employee Number field.");

                        var is_active = record.is_active;
                        record.InjectFrom(e);
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
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var timesheet = dbContext.timesheet.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.operator_id == key).FirstOrDefault();
                if (timesheet != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var record = dbContext.employee
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.employee.Remove(record);
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
                var record = await dbContext.vw_employee
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
        public async Task<IActionResult> SaveData([FromBody] employee Record)
        {
            try
            {
                var record = dbContext.employee
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
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        record.owner_id = (record.owner_id ?? CurrentUserContext.AppUserId);

                        await dbContext.SaveChangesAsync();
                        return Ok(record);
                    }
                    else
                    {
                        return BadRequest("User is not authorized.");
                    }
                }
                else if (await mcsContext.CanCreate(dbContext, nameof(employee),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new employee();
                    record.InjectFrom(Record);

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
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    dbContext.employee.Add(record);
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
                var record = dbContext.employee
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.employee.Remove(record);
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

                    var record = dbContext.employee
                        .Where(o => o.employee_number == PublicFunctions.IsNullCell(row.GetCell(0))
                             && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.employee_name = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.address = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.join_date = PublicFunctions.Tanggal(row.GetCell(3));
                        record.gender = PublicFunctions.BenarSalah(row.GetCell(4));
                        record.phone = PublicFunctions.IsNullCell(row.GetCell(5));
                        record.is_operator = PublicFunctions.BenarSalah(row.GetCell(6));
                        record.is_supervisor = PublicFunctions.BenarSalah(row.GetCell(7));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new employee();
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

                        record.employee_number = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.employee_name = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.address = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.join_date = PublicFunctions.Tanggal(row.GetCell(3));
                        record.gender = PublicFunctions.BenarSalah(row.GetCell(4));
                        record.phone = PublicFunctions.IsNullCell(row.GetCell(5));
                        record.is_operator = PublicFunctions.BenarSalah(row.GetCell(6));
                        record.is_supervisor = PublicFunctions.BenarSalah(row.GetCell(7));

                        dbContext.employee.Add(record);
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
                HttpContext.Session.SetString("filename", "Employee");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpGet("EmployeeIdLookup")]
        public async Task<object> EmployeeIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.employee
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.employee_name != null)
					.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderByDescending(o => o.is_active).ThenBy(o => o.employee_number)
                    .Select(o => new {
                        Value = o.id,
                        Text = o.employee_number + " - " + o.employee_name + (o.is_active == true ? "" : " ## Not Active"),
                        search = o.employee_number.ToLower() + " - " + o.employee_name.ToLower() + (o.is_active == true ? "" : " ## not active") +
                        o.employee_number.ToUpper() + " - " + o.employee_name.ToUpper() + (o.is_active == true ? "" : " ## NOT ACTIVE")
                    });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("EmployeeOperatorIdLookup")]
        public async Task<object> EmployeeOperatorIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.employee.Where(x => x.organization_id == CurrentUserContext.OrganizationId
                        && x.is_operator == true)
					.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.employee_number + " - " + o.employee_name + (o.is_active == true ? "" : " ## Not Active"),
                        search = o.employee_number.ToLower() + " - " + o.employee_name.ToLower() + (o.is_active == true ? "" : " ## not active")
                        + o.employee_number.ToUpper() + " - " + o.employee_name.ToUpper() + (o.is_active == true ? "" : " ## NOT ACTIVE")
                    });
              return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("EmployeeSupervisorIdLookup")]
        public async Task<object> EmployeeSupervisorIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.employee.Where(x => x.organization_id == CurrentUserContext.OrganizationId
                        && x.is_supervisor == true)
					.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderByDescending(o => o.is_active).ThenBy(o => o.employee_number)
                    .Select(o => new { Value = o.id, Text = o.employee_number + " - " + o.employee_name + (o.is_active == true ? "" : " ## Not Active"), 
                        search = o.employee_number.ToLower() + " - " + o.employee_name.ToLower() + (o.is_active == true ? "" : " ## not active") + o.employee_number.ToUpper() + " - " + o.employee_name.ToUpper() + (o.is_active == true ? "" : " ## NOT ACTIVE") });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("EmployeeOperatorNumberLookup")]
        public async Task<object> EmployeeOperatorNumberLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                //var lookup = dbContext.employee.Where(x => x.organization_id == CurrentUserContext.OrganizationId
                //    && x.is_operator == true).Select(o => new { Value = o.id, Text = o.employee_number });

                var lookup = dbContext.employee.Where(x => x.organization_id == CurrentUserContext.OrganizationId
                        && x.is_operator == true)
					.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderByDescending(o => o.is_active).ThenBy(o => o.employee_number)
                    .Select(o => new { Value = o.id, 
                        Text =  o.employee_number + " - " + o.employee_name + (o.is_active == true ? "" : " ## Not Active") ,
                        search = o.employee_number.ToLower() + " - " + o.employee_name.ToLower() + (o.is_active == true ? "" : " ## not active") + o.employee_number.ToUpper() + " - " + o.employee_name.ToUpper() + (o.is_active == true ? "" : " ## NOT ACTIVE")

                    });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("EmployeeSupervisorNumberLookup")]
        public async Task<object> EmployeeSupervisorNumberLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                //var lookup = dbContext.employee.Where(x => x.organization_id == CurrentUserContext.OrganizationId
                //    && x.is_supervisor == true).Select(o => new { Value = o.id, Text = o.employee_number });

                var lookup = dbContext.employee.Where(x => x.organization_id == CurrentUserContext.OrganizationId
                        && x.is_supervisor == true)
					.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderByDescending(o => o.is_active).ThenBy(o => o.employee_number)
                    .Select(o => new { Value = o.id, Text = o.employee_number + " - " + o.employee_name + (o.is_active == true ? "" : " ## Not Active")
                    , search = o.employee_number.ToLower() + " - " + o.employee_name.ToLower() + (o.is_active == true ? "" : " ## not active") + o.employee_number.ToUpper() + " - " + o.employee_name.ToUpper() + (o.is_active == true ? "" : " ## NOT ACTIVE") });
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

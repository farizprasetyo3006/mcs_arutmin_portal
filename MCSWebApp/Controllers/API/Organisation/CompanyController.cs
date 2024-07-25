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

namespace MCSWebApp.Controllers.API.Organisation
{
    [Route("api/Organisation/[controller]")]
    [ApiController]
    public class CompanyController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public CompanyController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_business_partner
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.business_partner.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(business_partner),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new business_partner();
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
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    dbContext.business_partner.Add(record);
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
                var record = dbContext.business_partner
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
                var record = dbContext.business_partner
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.business_partner.Remove(record);
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
                var record = await dbContext.vw_business_partner
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
        public async Task<IActionResult> SaveData([FromBody] business_partner Record)
        {
            try
            {
                var record = dbContext.business_partner
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
                else if (await mcsContext.CanCreate(dbContext, nameof(business_partner),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new business_partner();
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

                    dbContext.business_partner.Add(record);
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
                var record = dbContext.business_partner
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.business_partner.Remove(record);
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

        [HttpPost("UploadDocument_old")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UploadDocument_old([FromBody] dynamic FileDocument)
        {
            var result = new StandardResult();
            long size = 0;

            if (FileDocument == null)
            {
                return BadRequest("No file uploaded!");
            }

            try
            {
                string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
            if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                };

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

                using var transaction = await dbContext.Database.BeginTransactionAsync();
                try
                {

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;

                        var record = dbContext.business_partner
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                                o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower())
                            .FirstOrDefault();
                        if (record != null)
                        {
                            var e = new entity();
                            e.InjectFrom(record);

                            record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;

                            record.business_partner_name = PublicFunctions.IsNullCell(row.GetCell(0));
                            record.is_vendor = PublicFunctions.BenarSalah(row.GetCell(2));
                            record.is_customer = PublicFunctions.BenarSalah(row.GetCell(3));
                            record.is_government = PublicFunctions.BenarSalah(row.GetCell(4));
                            record.primary_address = PublicFunctions.IsNullCell(row.GetCell(5));
                            record.primary_contact_name = PublicFunctions.IsNullCell(row.GetCell(6));
                            record.primary_contact_email = PublicFunctions.IsNullCell(row.GetCell(7));
                            record.primary_contact_phone = PublicFunctions.IsNullCell(row.GetCell(8));
                            record.tax_registration_number = PublicFunctions.IsNullCell(row.GetCell(9));

                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            record = new business_partner();
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

                            record.business_partner_code = PublicFunctions.IsNullCell(row.GetCell(1));
                            record.business_partner_name = PublicFunctions.IsNullCell(row.GetCell(0));
                            record.is_vendor = PublicFunctions.BenarSalah(row.GetCell(2));
                            record.is_customer = PublicFunctions.BenarSalah(row.GetCell(3));
                            record.is_government = PublicFunctions.BenarSalah(row.GetCell(4));
                            record.primary_address = PublicFunctions.IsNullCell(row.GetCell(5));
                            record.primary_contact_name = PublicFunctions.IsNullCell(row.GetCell(6));
                            record.primary_contact_email = PublicFunctions.IsNullCell(row.GetCell(7));
                            record.primary_contact_phone = PublicFunctions.IsNullCell(row.GetCell(8));
                            record.tax_registration_number = PublicFunctions.IsNullCell(row.GetCell(9));

                            dbContext.business_partner.Add(record);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    await transaction.CommitAsync();

                    sheet.Workbook.Close();

                    System.IO.File.Delete(FilePath);
                }
                catch (Exception ex)
                {
					await transaction.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.Message);
                }

                return "File berhasil di-upload!";
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

            IWorkbook workbook;
            using (FileStream file = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                workbook = WorkbookFactory.Create(file);
            }

            var importer = new Npoi.Mapper.Mapper(workbook);
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var sheet = importer.Take<business_partner>(0);
                foreach (var item in sheet)
                {
                    var row = item.Value;
                    var record = dbContext.business_partner.Where(o => o.organization_id == row.organization_id
                    && o.business_partner_name.ToLower() == row.business_partner_name.ToLower()).FirstOrDefault();
                    if (record == null)
                    {
                        row.id = Guid.NewGuid().ToString("N");
                        dbContext.business_partner.Add(row);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        record.InjectFrom(row);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                }
                await transaction.CommitAsync();

                return "File berhasil di-upload!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.Error(ex.ToString());
                return ex.ToString();
            }
            finally
            {
                System.IO.File.Delete(FilePath);
            }
        }

    }
}

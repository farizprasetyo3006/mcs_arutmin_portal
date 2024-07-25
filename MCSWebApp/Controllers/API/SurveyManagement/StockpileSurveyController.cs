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
using DataAccess.Select2;
using BusinessLogic.Entity;
using Common;
using DocumentFormat.OpenXml.InkML;

namespace MCSWebApp.Controllers.API.SurveyManagement
{
    [Route("api/SurveyManagement/[controller]")]
    [ApiController]
    public class StockpileSurveyController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public StockpileSurveyController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.survey
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.survey.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            survey record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(survey),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new survey();
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

                        dbContext.survey.Add(record);
                        await dbContext.SaveChangesAsync();

                        #region Insert survey analytes

                        if (!string.IsNullOrEmpty(record.sampling_template_id))
                        {
                            var st = dbContext.sampling_template
                                .Where(o => o.id == record.sampling_template_id)
                                .FirstOrDefault();
                            if (st != null)
                            {
                                var details = dbContext.sampling_template_detail
                                    .Where(o => o.sampling_template_id == st.id)
                                    .ToList();
                                if (details != null && details.Count > 0)
                                {
                                    foreach (var d in details)
                                    {
                                        dbContext.survey_analyte.Add(new survey_analyte()
                                        {
                                            id = Guid.NewGuid().ToString("N"),
                                            created_by = CurrentUserContext.AppUserId,
                                            created_on = DateTime.Now,
                                            owner_id = CurrentUserContext.AppUserId,
                                            organization_id = CurrentUserContext.OrganizationId,
                                            survey_id = record.id,
                                            analyte_id = d.analyte_id,
                                            uom_id = d.uom_id
                                        });
                                    }

                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }

                        #endregion

                        await tx.CommitAsync();
                        success = true;
                    }
                    else
                    {
                        logger.Debug("User is not authorized.");
                        return Unauthorized();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }

            if (success && record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.survey();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }

            return Ok(record);
        }

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var success = false;
            survey record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.survey
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

                            if (!string.IsNullOrEmpty(record.sampling_template_id))
                            {
                                var details = dbContext.sampling_template_detail
                                    .FromSqlRaw(" SELECT * FROM sampling_template_detail "
                                        + $" WHERE sampling_template_id = '{record.sampling_template_id}' "
                                        + " AND analyte_id NOT IN ( "
                                        + " SELECT analyte_id FROM survey_analyte "
                                        + $" WHERE survey_id = '{record.id}' "
                                        + " ) ")
                                    .ToList();
                                if (details != null && details.Count > 0)
                                {
                                    foreach (var d in details)
                                    {
                                        var sa = await dbContext.survey_analyte.Where(o => o.analyte_id == d.analyte_id)
                                            .FirstOrDefaultAsync();
                                        if (sa == null)
                                        {
                                            dbContext.survey_analyte.Add(new survey_analyte()
                                            {
                                                id = Guid.NewGuid().ToString("N"),
                                                created_by = CurrentUserContext.AppUserId,
                                                created_on = DateTime.Now,
                                                owner_id = CurrentUserContext.AppUserId,
                                                organization_id = CurrentUserContext.OrganizationId,
                                                survey_id = record.id,
                                                analyte_id = d.analyte_id
                                            });
                                        }
                                    }

                                    await dbContext.SaveChangesAsync();
                                }
                            }

                            await tx.CommitAsync();
                            success = true;
                        }
                        else
                        {
                            logger.Debug("User is not authorized.");
                            return Unauthorized();
                        }
                    }
                    else
                    {
                        logger.Debug("Record is not found.");
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }

            if (success && record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.survey();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }

            return Ok(record);
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            var success = false;
            survey record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.survey
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            dbContext.survey.Remove(record);
                            await dbContext.SaveChangesAsync();

                            await tx.CommitAsync();
                            success = true;
                        }
                        else
                        {
                            logger.Debug("User is not authorized.");
                            return Unauthorized();
                        }
                    }
                    else
                    {
                        logger.Debug("Record is not found.");
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }

            if (success && record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.survey();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }

            return Ok();
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
                            var record = dbContext.survey
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.survey.Remove(record);
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

        [HttpGet("StockLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> StockLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                ////var lookup = dbContext.stock_location
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                //    .Select(o => new { Value = o.id, Text = o.stock_location_name });
                var lookup = dbContext.vw_location.FromSqlRaw(@"SELECT ml.id,
                        concat(ba.business_area_name, ' > ', ml.stock_location_name) AS location_name, 
                        ml.mine_location_code AS code,
                        ml.organization_id, 
                        ml.business_unit_id, 
                        ml.business_area_id, 
                        1 AS grup 
                        FROM mine_location ml 
                        JOIN business_area ba ON ml.business_area_id = ba.id 
                            UNION SELECT sl.id, concat(ba.business_area_name, ' > ', sl.stock_location_name) AS location_name, 
                            sl.stockpile_location_code AS code, 
                            sl.organization_id, 
                            sl.business_unit_id,
                            sl.business_area_id,
                            2 AS grup 
                            FROM stockpile_location sl 
                            JOIN business_area ba ON sl.business_area_id = ba.id 
                            WHERE sl.is_virtual is null OR sl.is_virtual is FALSE 
                                UNION SELECT pl.id,concat(ba.business_area_name, ' > ', pl.stock_location_name) AS location_name,
                                pl.port_location_code AS code, 
                                pl.organization_id,
                                pl.business_unit_id, 
                                pl.business_area_id, 
                                3 AS grup 
                                FROM port_location pl 
                                JOIN business_area ba ON pl.business_area_id = ba.id")
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderBy(o => o.location_name)
                    .Select(o => new { Value = o.id, Text = o.location_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SamplingTemplateIdLookup")]
        public async Task<object> SamplingTemplateIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.sampling_template
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sampling_template_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SurveyorIdLookup")]
        public async Task<object> SurveyorIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.business_partner
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.is_vendor == true)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.business_partner_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SurveyIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SurveyIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.survey
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && (o.is_draft_survey == null || o.is_draft_survey == false))
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.survey_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DraftSurveyIdLookup")]
        public async Task<object> DraftSurveyIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.survey
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.is_draft_survey == true)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.survey_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
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
                var record = await dbContext.vw_survey
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
        public async Task<IActionResult> SaveData([FromBody] survey Record)
        {
            try
            {
                var record = dbContext.survey
                    .Where(o => o.id == Record.id)
                    .FirstOrDefault();
                if (record != null)
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
                    record = new survey();
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

                    dbContext.survey.Add(record);
                    await dbContext.SaveChangesAsync();

                    return Ok(record);
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
                var record = dbContext.survey
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.survey.Remove(record);
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

        [HttpGet("Approve")]
        public async Task<IActionResult> Approve([FromQuery] string Id)
        {
            logger.Debug($"string Id = {Id}");
            var result = new StandardResult();

            try
            {
                var _survey = new BusinessLogic.Entity.Survey(CurrentUserContext);
                result = await _survey.Approve(Id);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
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

                    var surveyor_id = "";
                    var surveyor = dbContext.business_partner
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.business_partner_code == PublicFunctions.IsNullCell(row.GetCell(3))).FirstOrDefault();
                    if (surveyor != null) surveyor_id = surveyor.id.ToString();

                    var stock_location_id = "";
                    var stock_location = dbContext.stockpile_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.stockpile_location_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower()).FirstOrDefault();
                    if (stock_location != null) stock_location_id = stock_location.id.ToString();

                    var product_id = "";
                    var product = dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.product_code == PublicFunctions.IsNullCell(row.GetCell(5))).FirstOrDefault();
                    if (product != null) product_id = product.id.ToString();

                    var uom_id = "";
                    var uom = dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.uom_symbol == PublicFunctions.IsNullCell(row.GetCell(7))).FirstOrDefault();
                    if (uom != null) uom_id = uom.id.ToString();

                    var sampling_template_id = "";
                    var sampling_template = dbContext.sampling_template
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sampling_template_code == PublicFunctions.IsNullCell(row.GetCell(8))).FirstOrDefault();
                    if (sampling_template != null) sampling_template_id = sampling_template.id.ToString();

                    var business_unit_id = "";
                    var busniess_unit = dbContext.business_unit.Where(o => o.business_unit_code == PublicFunctions.IsNullCell(row.GetCell(10))).FirstOrDefault();
                    if (busniess_unit != null) business_unit_id = busniess_unit.id.ToString();

                    var accounting_period_id = "";
                    var accounting_period = dbContext.accounting_period.Where(o => o.accounting_period_name == PublicFunctions.IsNullCell(row.GetCell(9))).FirstOrDefault();
                    if (accounting_period != null) accounting_period_id = accounting_period.id;

                    var record = dbContext.survey
                        .Where(o => o.survey_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.is_draft_survey = PublicFunctions.BenarSalah(row.GetCell(1));
                        record.survey_date = PublicFunctions.Tanggal(row.GetCell(2));
                        record.surveyor_id = surveyor_id;
                        record.stock_location_id = stock_location_id;
                        record.product_id = product_id;
                        record.quantity = PublicFunctions.Desimal(row.GetCell(6));
                        record.uom_id = uom_id;
                        record.sampling_template_id = sampling_template_id;
                        record.business_unit_id = business_unit_id;
                        record.accounting_period_id = accounting_period_id;

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new survey();
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
                        record.accounting_period_id = accounting_period_id;

                        record.survey_number = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.is_draft_survey = PublicFunctions.BenarSalah(row.GetCell(1));
                        record.survey_date = PublicFunctions.Tanggal(row.GetCell(2));
                        record.surveyor_id = surveyor_id;
                        record.stock_location_id = stock_location_id;
                        record.product_id = product_id;
                        record.quantity = PublicFunctions.Desimal(row.GetCell(6));
                        record.uom_id = uom_id;
                        record.sampling_template_id = sampling_template_id;

                        dbContext.survey.Add(record);
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

            sheet = wb.GetSheetAt(1); //*** detail sheet 1
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var survey_id = "";
                    var survey = dbContext.survey
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.survey_number == PublicFunctions.IsNullCell(row.GetCell(0))).FirstOrDefault();
                    if (survey != null) survey_id = survey.id.ToString();

                    var analyte_id = "";
                    var analyte = dbContext.analyte
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.analyte_name == PublicFunctions.IsNullCell(row.GetCell(1))).FirstOrDefault();
                    if (analyte != null) analyte_id = analyte.id.ToString();

                    var uom_id = "";
                    var uom = dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.uom_symbol == PublicFunctions.IsNullCell(row.GetCell(2))).FirstOrDefault();
                    if (uom != null) uom_id = uom.id.ToString();

                    var record = dbContext.survey_analyte
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.survey_id == survey_id &&
                            o.analyte_id == analyte_id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.uom_id = uom_id;
                        record.analyte_value = PublicFunctions.Desimal(row.GetCell(3));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new survey_analyte();
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

                        record.survey_id = survey_id;
                        record.analyte_id = analyte_id;
                        record.uom_id = uom_id;
                        record.analyte_value = PublicFunctions.Desimal(row.GetCell(3));

                        dbContext.survey_analyte.Add(record);
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
            }
            //***********

            sheet = wb.GetSheetAt(2); //*** detail sheet 2
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var survey_id = "";
                    var survey = dbContext.survey
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.survey_number == PublicFunctions.IsNullCell(row.GetCell(0))).FirstOrDefault();
                    if (survey != null) survey_id = survey.id.ToString();

                    var product_id = "";
                    var product = dbContext.product.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                        o.product_code == PublicFunctions.IsNullCell(row.GetCell(1))).FirstOrDefault();
                    if (product != null) product_id = product.id.ToString();

                    var contractor_id = "";
                    var contractor = dbContext.contractor
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.business_partner_code == PublicFunctions.IsNullCell(row.GetCell(2)))
                        .FirstOrDefault();
                    if (contractor != null) contractor_id = contractor.id.ToString();

                    var record = dbContext.survey_detail
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.survey_id == survey_id && 
                            o.product_id == product_id && 
                            o.contractor_id == contractor_id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.quantity = PublicFunctions.Desimal(row.GetCell(3));
                        record.percentage = PublicFunctions.Desimal(row.GetCell(4));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new survey_detail();
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

                        record.survey_id = survey_id;
                        record.product_id = product_id;
                        record.contractor_id = contractor_id;
                        record.quantity = PublicFunctions.Desimal(row.GetCell(3));
                        record.percentage = PublicFunctions.Desimal(row.GetCell(4));

                        dbContext.survey_detail.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 3, Line " + (i + 1) + " : " + Environment.NewLine;
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
                HttpContext.Session.SetString("filename", "Survey");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpGet("select2")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Select2([FromQuery] string q)
        {
            var result = new Select2Response();

            try
            {
                var s2Request = new Select2Request()
                {
                    q = q
                };
                if (s2Request != null)
                {
                    var svc = new Survey(CurrentUserContext);
                    result = await svc.Select2(s2Request, "survey_number");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return new JsonResult(result);
        }
    }
}

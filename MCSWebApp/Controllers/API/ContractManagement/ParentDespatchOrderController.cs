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

namespace MCSWebApp.Controllers.API.ContractManagement
{
    [Route("api/ContractManagement/[controller]")]
    [ApiController]
    public class ParentDespatchOrderController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ParentDespatchOrderController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.parent_despatch_order
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.parent_despatch_order.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            var tx = await dbContext.Database.BeginTransactionAsync();
            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(parent_despatch_order),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new parent_despatch_order();
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

                   
                    
                    dbContext.parent_despatch_order.Add(record);
                    await dbContext.SaveChangesAsync();

                    await tx.CommitAsync();

                    return Ok(record);
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

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.parent_despatch_order
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
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
                        logger.Debug("User is not authorized.");
                        return Unauthorized();
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
                var record = dbContext.parent_despatch_order
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.parent_despatch_order.Remove(record);
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


        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] shipping_cost Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.parent_despatch_order
                        .Where(o => o.id == Record.id)
                        .FirstOrDefaultAsync();
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
                            await tx.CommitAsync();
                            return Ok(record);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else if (await mcsContext.CanCreate(dbContext, nameof(parent_despatch_order),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new parent_despatch_order();
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

                        dbContext.parent_despatch_order.Add(record);
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
                    logger.Error(ex.InnerException ?? ex);
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        //[HttpPost("UploadDocument")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<object> UploadDocument([FromBody] dynamic FileDocument)
        //{
        //    var result = new StandardResult();
        //    long size = 0;

        //    if (FileDocument == null)
        //    {
        //        return BadRequest("No file uploaded!");
        //    }

        //    string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + PublicFunctions.ExcelFolder;
        //    if (!Directory.Exists(FilePath))  Directory.CreateDirectory(FilePath);

        //    var fileName = (string)FileDocument.filename;
        //    FilePath += $@"\{fileName}";

        //    string strfile = (string)FileDocument.data;
        //    byte[] arrfile = Convert.FromBase64String(strfile);

        //    await System.IO.File.WriteAllBytesAsync(FilePath, arrfile);

        //    size = fileName.Length;
        //    string sFileExt = Path.GetExtension(FilePath).ToLower();

        //    ISheet sheet;
        //    dynamic wb;
        //    if (sFileExt == ".xls")
        //    {
        //        FileStream stream = System.IO.File.OpenRead(FilePath);
        //        wb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats
        //        sheet = wb.GetSheetAt(0); //get first sheet from workbook
        //        stream.Close();
        //    }
        //    else
        //    {
        //        wb = new XSSFWorkbook(FilePath); //This will read 2007 Excel format
        //        sheet = wb.GetSheetAt(0); //get first sheet from workbook
        //    }

        //    string teks = "";
        //    bool gagal = false; string errormessage = "";

        //    using var transaction = await dbContext.Database.BeginTransactionAsync();
        //    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
        //    {
        //        try
        //        {
        //            IRow row = sheet.GetRow(i);
        //            if (row == null) continue;

        //            var despatch_order_id = "";
        //            var despatch_order = dbContext.despatch_order
        //                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
        //                    o.despatch_order_number == PublicFunctions.IsNullCell(row.GetCell(0))).FirstOrDefault();
        //            if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();

        //            var record = dbContext.parent_despatch_order
        //                .Where(o => o.shipping_cost_number == PublicFunctions.Bulat(row.GetCell(0))
        //                    && o.organization_id == CurrentUserContext.OrganizationId)
        //                .FirstOrDefault();
        //            if (record != null)
        //            {
        //                var e = new entity();
        //                e.InjectFrom(record);

        //                record.InjectFrom(e);
        //                record.modified_by = CurrentUserContext.AppUserId;
        //                record.modified_on = DateTime.Now;
        //                
        //                record.despatch_order_id = despatch_order_id;
        //                record.freight_rate = PublicFunctions.Desimal(row.GetCell(2));
        //                record.insurance_cost = PublicFunctions.Desimal(row.GetCell(3));
        //                record.quantity = PublicFunctions.Desimal(row.GetCell(4));
        //                record.remark = PublicFunctions.IsNullCell(row.GetCell(5));

        //                await dbContext.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                record = new shipping_cost();
        //                record.id = Guid.NewGuid().ToString("N");
        //                record.created_by = CurrentUserContext.AppUserId;
        //                record.created_on = DateTime.Now;
        //                record.modified_by = null;
        //                record.modified_on = null;
        //                record.is_active = true;
        //                record.is_default = null;
        //                record.is_locked = null;
        //                record.entity_id = null;
        //                record.owner_id = CurrentUserContext.AppUserId;
        //                record.organization_id = CurrentUserContext.OrganizationId;

        //                record.shipping_cost_number = PublicFunctions.Bulat(row.GetCell(0));
        //                record.despatch_order_id = despatch_order_id;
        //                record.freight_rate = PublicFunctions.Desimal(row.GetCell(2));
        //                record.insurance_cost = PublicFunctions.Desimal(row.GetCell(3));
        //                record.quantity = PublicFunctions.Desimal(row.GetCell(4));
        //                record.remark = PublicFunctions.IsNullCell(row.GetCell(5));

        //                dbContext.shipping_cost.Add(record);
        //                await dbContext.SaveChangesAsync();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            if (ex.InnerException != null)
        //            {
        //                errormessage = ex.InnerException.Message;
        //                teks += "==>Error Sheet 1, Line " + (i+1) + " : " + Environment.NewLine;
        //            }
        //            else errormessage = ex.Message;

        //            teks += errormessage + Environment.NewLine + Environment.NewLine;
        //            gagal = true;
        //        }
        //    }

        //    wb.Close();
        //    if (gagal)
        //    {
        //        await transaction.RollbackAsync();
        //        HttpContext.Session.SetString("errormessage", teks);
        //        HttpContext.Session.SetString("filename", "DraftSurvey");
        //        return BadRequest("File gagal di-upload");
        //    }
        //    else
        //    {
        //        await transaction.CommitAsync();
        //        return "File berhasil di-upload!";
        //    }
        //}
        [HttpGet("DespatchOrderIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DespatchOrderIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.despatch_order
                    .Where(o => o.set_parent ==true && o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ByDraftSurveyId/{Id}")]
        public async Task<object> ByDraftSurveyId(string Id, DataSourceLoadOptions loadOptions)
        {
            var record = dbContext.vw_draft_survey.Where(o => o.id == Id).FirstOrDefault();
            var quality_sampling_id = "";
            if (record != null) quality_sampling_id = record.quality_sampling_id;

            return await DataSourceLoader.LoadAsync(dbContext.vw_quality_sampling_analyte
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.quality_sampling_id == quality_sampling_id),
                loadOptions);
        }

    }
}

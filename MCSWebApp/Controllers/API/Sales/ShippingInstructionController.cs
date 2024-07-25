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
using SelectPdf;
using System.Globalization;
using Common;
using Microsoft.AspNetCore.StaticFiles;
using WebApp.Extensions;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/[controller]")]
    [ApiController]
    public class ShippingInstructionController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        string AttachmentFolder = "shipping_instruction_attachment";

        public ShippingInstructionController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                return await DataSourceLoader.LoadAsync(dbContext.vw_shipping_instruction
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderByDescending(o => o.created_on),
                        loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DespatchOrderIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DespatchOrderIdLookup([FromQuery] string SalesOrderId, DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (string.IsNullOrEmpty(SalesOrderId))
                {
                    logger.Debug($"SalesOrderId in DespatchOrderIdLookup is null or empty");
                    var lookup = dbContext.despatch_order
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.despatch_order_number, o.sales_order_id });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    logger.Debug($"SalesOrderId in DespatchOrderIdLookup is {SalesOrderId}");
                    var lookup = dbContext.despatch_order
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.sales_order_id == SalesOrderId)
                        .Select(o => new { Value = o.id, Text = o.despatch_order_number, o.sales_order_id });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DataDetail/{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<StandardResult> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            var result = new StandardResult();
            try
            {
                var record = await dbContext.vw_shipping_instruction
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                result.Data = record;
                result.Success = record != null ? true : false;
                result.Message = result.Success ? "Ok" : "Record not found";

            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                result.Success = false;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        [HttpGet("GetShippingInstruction/{Id}")]
        public object GetCHLS(string Id)
        {
            try
            {
                var result = new shipping_instruction();

                result = dbContext.shipping_instruction.Where(x => x.id == Id).FirstOrDefault();

                if (result == null)
                {
                    result = new shipping_instruction();
                    result.id = Id;
                }

                return result;
            }
            catch (Exception ex)
            {
                //logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DataDetailByDespatchOrder")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Object> DataDetailByDespatchOrder(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                 dbContext.shipping_instruction.Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.despatch_order_id == Id),
                 loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(shipping_instruction),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var record = new shipping_instruction();
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

                    #region Get transaction number
                    var conn = dbContext.Database.GetDbConnection();
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                    }
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            try
                            {
                                
                                cmd.CommandText = $"SELECT nextval('shipping_instruction_number_seq')";
                                var r = await cmd.ExecuteScalarAsync();
                                var s = new string('0', 4 - r.ToString().Length);
                                record.shipping_instruction_number = $"SI-{DateTime.Now:yyyy}{DateTime.Now:MM}{DateTime.Now:dd}-{s}{r}";
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.ToString());
                                return BadRequest(ex.Message);
                            }
                        }
                    }
                    #endregion


                    dbContext.shipping_instruction.Add(record);
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

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] shipping_instruction Record)
        {
            try
            {
                var record = dbContext.shipping_instruction
                    .Where(o => o.id == Record.id)
                    .FirstOrDefault();
                if (record != null)
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
                    record = new shipping_instruction();
                    record.InjectFrom(Record);

                    record.id = Guid.NewGuid().ToString("N");
                    record.created_by = CurrentUserContext.AppUserId;
                    record.created_on = DateTime.Now;
                    record.modified_by = null;
                    record.modified_on = null;
                    record.is_default = null;
                    record.is_locked = null;
                    record.entity_id = null;
                    record.owner_id = CurrentUserContext.AppUserId;
                    record.organization_id = CurrentUserContext.OrganizationId;
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    #region Get transaction number
                    var conn = dbContext.Database.GetDbConnection();
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                    }
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            try
                            {
                                cmd.CommandText = $"SELECT nextval('seq_transaction_number')";
                                var r = await cmd.ExecuteScalarAsync();
                                record.shipping_instruction_number = $"{r}/IC-SI/{DateTime.Now:MM}/{DateTime.Now:yyyy}";
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.ToString());
                                return BadRequest(ex.Message);
                            }
                        }
                    }
                    #endregion

                    dbContext.shipping_instruction.Add(record);
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

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.shipping_instruction
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    var e = new entity();
                    e.InjectFrom(record);

                    JsonConvert.PopulateObject(values, record);

                    //record.InjectFrom(e);
                    record.modified_by = CurrentUserContext.AppUserId;
                    record.modified_on = DateTime.Now;

                    await dbContext.SaveChangesAsync();
                    return Ok(record);
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
                var record = dbContext.shipping_instruction
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.shipping_instruction.Remove(record);
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

        [HttpGet("SamplingTemplateIdLookup")]
        public async Task<object> SamplingTemplateIdLookup(DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                var lookup = dbContext.sampling_template
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Select(o =>
                    new
                    {
                        value = o.id,
                        text = o.sampling_template_name
                    });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("GetLc/{SalesOrder}")]
        public object GetLc(string SalesOrder)
        {
            try
            {
                var result = new shipping_instruction();

                result = dbContext.shipping_instruction.Where(o => o.despatch_order_id == SalesOrder).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DetailSurveyByShippingInstructionId/{id}")]
        public async Task<object> DetailSurveyByShippingInstructionId(string id)
        {
            try
            {
                var lookup = await dbContext.vw_shipping_instruction_detail_survey
                    .Where(o => o.shipping_instruction_id == id).FirstOrDefaultAsync();
                return Ok(lookup);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("CopyData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> CopyData([FromForm] string key, [FromForm] string values)
        {
            var currrentData = dbContext.shipping_instruction.Where(o=>o.id == key).FirstOrDefault();
            if (currrentData != null)
            {
                string jsonData = JsonConvert.SerializeObject(currrentData);
                var newData = new shipping_instruction();
                JsonConvert.PopulateObject(jsonData, newData);

                newData.id = Guid.NewGuid().ToString("N");
                newData.created_by = CurrentUserContext.AppUserId;
                newData.created_on = DateTime.Now;
                newData.modified_by = null;
                newData.modified_on = null;
                newData.is_active = true;
                newData.is_default = null;
                newData.is_locked = null;
                newData.entity_id = null;
                newData.owner_id = CurrentUserContext.AppUserId;
                newData.organization_id = CurrentUserContext.OrganizationId;
                //newData.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                #region Get transaction number
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {

                            cmd.CommandText = $"SELECT nextval('shipping_instruction_number_seq')";
                            var r = await cmd.ExecuteScalarAsync();
                            var s = new string('0', 4 - r.ToString().Length);
                            newData.shipping_instruction_number = $"SI-{DateTime.Now:yyyy}{DateTime.Now:MM}{DateTime.Now:dd}-{s}{r}";
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                            return BadRequest(ex.Message);
                        }
                    }
                }
                #endregion


                dbContext.shipping_instruction.Add(newData);
                await dbContext.SaveChangesAsync();

            }
                return Ok(currrentData);
        }

            #region Attachment method
            [HttpGet("ByHeaderId/{Id}")]
        public async Task<object> ByHeaderId(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.shipping_instruction_attachment.Where(o => o.shipping_instruction_id == Id),
                loadOptions);
        }
        [HttpPost("InsertAttachment")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> InsertAttachment(dynamic FileDocument)
        {
            var result = new StandardResult();

            if (FileDocument.shipping_instruction_id == null)
            {
                throw new Exception("Id is null");
            }

            if (FileDocument.fileName == null)
            {
                throw new Exception("File name is empty");
            }

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(shipping_instruction_attachment),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var mainId = (string)FileDocument.shipping_instruction_id;
                    var fileName = (string)FileDocument.fileName;
                    var data = (string)FileDocument.data;

                    //save to server
                    var uploadPath = Path.Combine(configuration["Path:UploadBasePath"].ToString(), AttachmentFolder, mainId);
                    var filePath = uploadPath;
                    if (!System.IO.Directory.Exists(filePath))
                    {
                        System.IO.Directory.CreateDirectory(filePath);
                    }

                    //save to folder
                    var fullPath = Path.Combine(filePath, fileName);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    byte[] imageBytes = Convert.FromBase64String(data);

                    System.IO.File.WriteAllBytes(fullPath, imageBytes);


                    string savedFileName = fileName;

                    //insert data to customer attachment
                    var attachmentDocument = new shipping_instruction_attachment()
                    {
                        id = Guid.NewGuid().ToString("N"),
                        created_by = CurrentUserContext.AppUserId,
                        created_on = DateTime.Now,
                        modified_by = null,
                        modified_on = null,
                        is_active = true,
                        is_locked = null,
                        is_default = null,
                        owner_id = CurrentUserContext.AppUserId,
                        organization_id = CurrentUserContext.OrganizationId,
                        entity_id = null,
                        filename = savedFileName,
                        shipping_instruction_id = mainId,
                    };
                    dbContext.shipping_instruction_attachment.Add(attachmentDocument);
                    await dbContext.SaveChangesAsync();

                    result.Success = true;
                    result.Message = "File is uploaded";
                }
                else
                {
                    return BadRequest("User is not authorized.");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return result;
        }
        [HttpDelete("DeleteAttachment")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteAttachment([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var record = dbContext.shipping_instruction_attachment
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {

                    dbContext.shipping_instruction_attachment.Remove(record);
                    await dbContext.SaveChangesAsync();

                    var uploadPath = Path.Combine(configuration["Path:UploadBasePath"].ToString(), AttachmentFolder, record.shipping_instruction_id, record.filename);
                    if (System.IO.File.Exists(uploadPath))
                    {
                        System.IO.File.Delete(uploadPath);
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

        [HttpGet("DownloadDocument/{Id}")]
        public async Task<IActionResult> DownloadDocument(string Id)
        {
            logger.Debug($"string Id = {Id}");

            try
            {
                var record = await dbContext.shipping_instruction_attachment
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    //get file 
                    var filePath = Path.Combine(configuration["Path:UploadBasePath"].ToString(), AttachmentFolder, record.shipping_instruction_id, record.filename);

                    if (System.IO.File.Exists(filePath))
                    {
                        var provider = new FileExtensionContentTypeProvider();
                        string mimeType;
                        if (provider.TryGetContentType(filePath, out mimeType))
                        {
                            var byteFile = await System.IO.File.ReadAllBytesAsync(filePath);
                            if (byteFile.Length > 0)
                            {
                                return new InlineFileContentResult(byteFile, mimeType)
                                {
                                    FileDownloadName = Path.GetFileName(filePath)
                                };
                            }
                        }
                    }
                }

                throw new Exception("File not found");
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        #endregion
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

                    string despatch_order_id = "";
                    var despatch_order = dbContext.despatch_order
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.despatch_order_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower())
                        .FirstOrDefault();
                    if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();

                    //var barge_id = "";
                    //var barge = dbContext.barge
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                    //        o.vehicle_name.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(18)).ToLower().Trim()).FirstOrDefault();

                    //var tug_id = "";
                    //var tug = dbContext.tug
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                    //        o.vehicle_name.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(19)).ToLower().Trim()).FirstOrDefault();
                    
                    //var vendor_id = "";
                    //var vendor = dbContext.contractor
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                    //        o.business_partner_name.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(20)).ToLower().Trim()).FirstOrDefault();

                    string sampling_template_id = "";
                    var sampling_template = dbContext.sampling_template
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sampling_template_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower())
                        .FirstOrDefault();
                    if (sampling_template != null) sampling_template_id = sampling_template.id.ToString();

                    string ShippingInstructionNumber = "";
                    if (PublicFunctions.IsNullCell(row.GetCell(0)) == "")
                    {
                        #region Get transaction number
                        var conn = dbContext.Database.GetDbConnection();
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            await conn.OpenAsync();
                        }
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                try
                                {
                                    cmd.CommandText = $"SELECT nextval('shipping_instruction_number_seq')";
                                    var r = await cmd.ExecuteScalarAsync();
                                    var s = new string('0', 4 - r.ToString().Length);
                                    ShippingInstructionNumber = $"SI-{DateTime.Now:yyyy}{DateTime.Now:MM}{DateTime.Now:dd}-{s}{r}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }
                        #endregion
                    }
                    else
                        ShippingInstructionNumber = PublicFunctions.IsNullCell(row.GetCell(0)).Trim();


                    var record = dbContext.shipping_instruction
                        .Where(o => o.shipping_instruction_number == ShippingInstructionNumber
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record == null)
                    {
                        record = new shipping_instruction();
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

                        record.shipping_instruction_number = ShippingInstructionNumber;
                        record.shipping_instruction_date = PublicFunctions.Tanggal(row.GetCell(1));
                        record.despatch_order_id = despatch_order_id;
                        //record.to_other = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.notify_party = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.cargo_description = PublicFunctions.IsNullCell(row.GetCell(4));
                        //record.cc = PublicFunctions.IsNullCell(row.GetCell(6));
                        record.sampling_template_id = sampling_template_id;
                        record.marked = PublicFunctions.IsNullCell(row.GetCell(6));
                        record.issued_date = PublicFunctions.Tanggal(row.GetCell(7));
                        record.placed = PublicFunctions.IsNullCell(row.GetCell(8));
                        record.shipping_instruction_created_by = PublicFunctions.IsNullCell(row.GetCell(9));
                        record.lampiran1 = PublicFunctions.IsNullCell(row.GetCell(10));
                        record.lampiran2 = PublicFunctions.IsNullCell(row.GetCell(11));
                        record.lampiran3 = PublicFunctions.IsNullCell(row.GetCell(12));
                        record.lampiran4 = PublicFunctions.IsNullCell(row.GetCell(13));
                        record.lampiran5 = PublicFunctions.IsNullCell(row.GetCell(14));
                        record.si_number = PublicFunctions.IsNullCell(row.GetCell(15));
                        record.notify_party_address = PublicFunctions.IsNullCell(row.GetCell(16));
                        record.hs_code = PublicFunctions.IsNullCell(row.GetCell(17));
                        //record.barge_id = barge_id;
                        //record.tug_id = tug_id;
                        // record.vendor_id = vendor_id;

                        dbContext.shipping_instruction.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.shipping_instruction_date = PublicFunctions.Tanggal(row.GetCell(1));
                        record.despatch_order_id = despatch_order_id;
                        //record.to_other = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.notify_party = PublicFunctions.IsNullCell(row.GetCell(3));
                        record.cargo_description = PublicFunctions.IsNullCell(row.GetCell(4));
                        //record.cc = PublicFunctions.IsNullCell(row.GetCell(6));
                        record.sampling_template_id = sampling_template_id;
                        record.marked = PublicFunctions.IsNullCell(row.GetCell(6));
                        record.issued_date = PublicFunctions.Tanggal(row.GetCell(7));
                        record.placed = PublicFunctions.IsNullCell(row.GetCell(8));
                        record.shipping_instruction_created_by = PublicFunctions.IsNullCell(row.GetCell(9));
                        record.lampiran1 = PublicFunctions.IsNullCell(row.GetCell(10));
                        record.lampiran2 = PublicFunctions.IsNullCell(row.GetCell(11));
                        record.lampiran3 = PublicFunctions.IsNullCell(row.GetCell(12));
                        record.lampiran4 = PublicFunctions.IsNullCell(row.GetCell(13));
                        record.lampiran5 = PublicFunctions.IsNullCell(row.GetCell(14));
                        record.si_number = PublicFunctions.IsNullCell(row.GetCell(15));
                        record.notify_party_address = PublicFunctions.IsNullCell(row.GetCell(16));
                        record.hs_code = PublicFunctions.IsNullCell(row.GetCell(17));
                        //record.barge_id = barge_id;
                        //record.tug_id = tug_id;

                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 1, Line " + (i+1) + ": " + Environment.NewLine;
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
                HttpContext.Session.SetString("filename", "DespatchOrder");
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

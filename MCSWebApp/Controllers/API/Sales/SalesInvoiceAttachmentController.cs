using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Microsoft.AspNetCore.Http;
using System.IO;
using BusinessLogic;
using Microsoft.AspNetCore.StaticFiles;
using WebApp.Extensions;
using Common;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/[controller]")]
    [ApiController]
    public class SalesInvoiceAttachmentController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        string AttachmentFolder = "salesinvoiceattachment";

        public SalesInvoiceAttachmentController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                return await DataSourceLoader.LoadAsync(dbContext.sales_invoice_attachment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DataGridAttachments")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGridAttachments(DataSourceLoadOptions loadOptions, [FromQuery] string salesInvoiceId)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_sales_invoice_attachment
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.sales_invoice_id == salesInvoiceId),
                    loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("BySalesInvoiceId/{Id}")]
        public async Task<object> ByCustomerId(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.sales_invoice_attachment.Where(o => o.sales_invoice_id == Id),
                loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string custId, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.sales_invoice_attachment.Where(o => o.sales_invoice_id == custId),                    
                loadOptions);
        }

        [HttpPost("InsertAttachment")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> InsertAttachment(dynamic FileDocument)
        {
            logger.Debug($"salesInvoiceId = {FileDocument.salesInvoiceId}");
            logger.Debug($"filename = {FileDocument.fileName}");
            logger.Debug($"filesize = {FileDocument.fileSize}");
            var result = new StandardResult();

            if (FileDocument.salesInvoiceId == null)
            {
                throw new Exception("Sales Invoice Id is null");
            }

            if (FileDocument.fileName == null)
            {
                throw new Exception("File name is empty");
            }

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(sales_invoice_attachment),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    //var salesInvoiceAttachmentId = (string)FileDocument.salesInvoiceId;
                    var salesInvoiceId = (string)FileDocument.salesInvoiceId;
                    var fileName = (string)FileDocument.fileName;
                    var data = (string)FileDocument.data;

                    //save to server
                    //var uploadPath = Path.Combine(configuration["Path:UploadPathSalesInvoiceAttachment"].ToString(), configuration["Path:UploadUrlSalesInvoiceAttachment"].ToString());
                    var uploadPath = Path.Combine(configuration["Path:UploadBasePath"].ToString(), AttachmentFolder, salesInvoiceId);
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


                    //string savedFileName = configuration["Path:UploadUrlSalesInvoiceAttachment"].ToString() + salesInvoiceId + "\\" + fileName;
                    string savedFileName = fileName;

                    var salesInvoiceAttachmentRecord = new sales_invoice_attachment()
                    {
                        id = Guid.NewGuid().ToString("N"),
                        created_by = CurrentUserContext.AppUserId,
                        created_on = System.DateTime.Now,
                        modified_by = null,
                        modified_on = null,
                        is_active = true,
                        is_locked = null,
                        is_default = null,
                        owner_id = CurrentUserContext.AppUserId,
                        organization_id = CurrentUserContext.OrganizationId,
                        entity_id = null,
                        filename = savedFileName,
                        sales_invoice_id = salesInvoiceId
                    };
                    dbContext.sales_invoice_attachment.Add(salesInvoiceAttachmentRecord);
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

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var record = dbContext.sales_invoice_attachment
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {

                    dbContext.sales_invoice_attachment.Remove(record);
                    await dbContext.SaveChangesAsync();

                    //var uploadPath = Path.Combine(configuration["Path:UploadPathSalesInvoice"].ToString(), record.filename);
                    var uploadPath = Path.Combine(configuration["Path:UploadBasePath"].ToString(), AttachmentFolder, record.sales_invoice_id, record.filename);
                    
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
                var record = await dbContext.sales_invoice_attachment
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    //get file 
                    //var filePath = Path.Combine(configuration["Path:UploadPathSalesInvoice"].ToString(), record.filename);
                    var filePath = Path.Combine(configuration["Path:UploadBasePath"].ToString(), AttachmentFolder, record.sales_invoice_id, record.filename);

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

    }
}

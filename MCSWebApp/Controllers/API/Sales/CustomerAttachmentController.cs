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
    public class CustomerAttachmentController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public CustomerAttachmentController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                return await DataSourceLoader.LoadAsync(dbContext.customer_attachment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ByCustomerId/{Id}")]
        public async Task<object> ByCustomerId(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.customer_attachment.Where(o => o.customer_id == Id),
                loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string custId, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.customer_attachment.Where(o => o.customer_id == custId),                    
                loadOptions);
        }

        //[HttpPost("InsertData")]
        //public async Task<IActionResult> InsertData([FromForm] List<IFormFile> values, [FromForm]string customerId)
        //{
        //    logger.Trace($"string values = {values}");
        //    var record = new List<customer_attachment>();

        //    if(string.IsNullOrEmpty(customerId))
        //    {
        //        return Ok(null);
        //    }
        //    try
        //    {
        //        if (values.Count > 0)
        //        {
        //            //save to server
        //            var uploadPath = configuration["Path:UploadPathCustomer"].ToString();
        //            var filePath = uploadPath + customerId;
        //            if (!System.IO.Directory.Exists(filePath))
        //            {
        //                System.IO.Directory.CreateDirectory(filePath);
        //            }


        //            foreach (var formFile in values)
        //            {
        //                if (formFile.Length > 0)
        //                {
        //                    //save to folder
        //                    var fullPath = Path.Combine(filePath, formFile.FileName);  
                            
        //                    if (System.IO.File.Exists(fullPath))
        //                    {
        //                        System.IO.File.Delete(fullPath);
        //                    }
        //                    using (var stream = new FileStream(fullPath, FileMode.Create))
        //                    {
        //                        formFile.CopyTo(stream);
        //                    }

                            
        //                    string savedFileName = customerId + "\\" + formFile.FileName;

        //                    //insert data to customer attachment
        //                    var customerAttachment = new customer_attachment()
        //                    {
        //                        id = Guid.NewGuid().ToString("N"),
        //                        created_by = CurrentUserContext.AppUserId,
        //                        created_on = DateTime.Now,
        //                        modified_by = null,
        //                        modified_on = null,
        //                        is_active = true,
        //                        is_locked = null,
        //                        is_default = null,
        //                        owner_id = CurrentUserContext.AppUserId,
        //                        organization_id = CurrentUserContext.OrganizationId,
        //                        entity_id = null,
        //                        filename = savedFileName,
        //                        customer_id = customerId
        //                    };
        //                    dbContext.customer_attachment.Add(customerAttachment);
        //                    await dbContext.SaveChangesAsync();
        //                    record.Add(customerAttachment);
        //                }
        //            }
        //        }
                              
        //        return Ok(record);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex.ToString());
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> InsertData(dynamic FileDocument)
        {
            logger.Debug($"id = {FileDocument.customerId}");
            logger.Debug($"filename = {FileDocument.fileName}");
            logger.Debug($"filesize = {FileDocument.fileSize}");
            var result = new StandardResult();

            if (FileDocument.customerId == null)
            {
                throw new Exception("Id is null");
            }

            if (FileDocument.fileName == null)
            {
                throw new Exception("File name is empty");
            }

            if (FileDocument.data == null)
            {
                throw new Exception("File content is empty");
            }

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(customer_attachment),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var customerId = (string)FileDocument.customerId;
                    var fileName = (string)FileDocument.fileName;
                    var data = (string)FileDocument.data;
                    //save to server
                    var uploadPath = Path.Combine(configuration["Path:UploadPathCustomer"].ToString(), configuration["Path:UploadUrlCustomer"].ToString()); 
                    var filePath = uploadPath + customerId;
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
                    

                    string savedFileName = configuration["Path:UploadUrlCustomer"].ToString() + customerId + "\\" + fileName;

                    //insert data to customer attachment
                    var customerAttachment = new customer_attachment()
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
                        customer_id = customerId
                    };
                    dbContext.customer_attachment.Add(customerAttachment);
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
                var record = dbContext.customer_attachment
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {

                    dbContext.customer_attachment.Remove(record);
                    await dbContext.SaveChangesAsync();

                    var uploadPath = Path.Combine(configuration["Path:UploadPathCustomer"].ToString(), record.filename);
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
                var record = await dbContext.customer_attachment
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    //get file 
                    var filePath = Path.Combine(configuration["Path:UploadPathCustomer"].ToString(), record.filename);
                    
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

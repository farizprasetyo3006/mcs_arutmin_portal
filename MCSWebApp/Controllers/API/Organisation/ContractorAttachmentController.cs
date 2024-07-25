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

namespace MCSWebApp.Controllers.API.Organisation
{
    [Route("api/Organisation/[controller]")]
    [ApiController]
    public class ContractorAttachmentController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ContractorAttachmentController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
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
                return await DataSourceLoader.LoadAsync(dbContext.contractor_document
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ByContractorId/{Id}")]
        public async Task<object> ByContractorId(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.contractor_document.Where(o => o.contractor_id == Id),
                loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string custId, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.contractor_document.Where(o => o.contractor_id == custId),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> InsertData(dynamic FileDocument)
        {
            logger.Debug($"id = {FileDocument.contractorId}");
            logger.Debug($"filename = {FileDocument.fileName}");
            logger.Debug($"filesize = {FileDocument.fileSize}");
            var result = new StandardResult();

            if (FileDocument.contractorId == null)
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
				if (await mcsContext.CanCreate(dbContext, nameof(contractor_document),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var contractorId = (string)FileDocument.contractorId;
                    var fileName = (string)FileDocument.fileName;
                    var data = (string)FileDocument.data;
                    //save to server
                    var uploadPath = configuration.GetSection("Path").GetSection("UploadBasePath").Value + configuration.GetSection("Path").GetSection("ContractorDocumentPath").Value;
                    var filePath = uploadPath + "/" + contractorId;
                    if (!System.IO.Directory.Exists(filePath))
                    {
                        System.IO.Directory.CreateDirectory(filePath);
                    }
                                    
                    //save to folder
                    var fullPath = Path.Combine(filePath, fileName);

                    //if (System.IO.File.Exists(fullPath))
                    //{
                    //    System.IO.File.Delete(fullPath);
                    //}

                    byte[] imageBytes = Convert.FromBase64String(data);

                    System.IO.File.WriteAllBytes(fullPath, imageBytes);
                    
                    string savedFileName = fullPath + "\\" + fileName;

                    //insert data to contractor attachment
                    var contractorAttachment = new contractor_document()
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
                        file_name = savedFileName,
                        contractor_id = contractorId
                    };
                    dbContext.contractor_document.Add(contractorAttachment);
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
                var record = dbContext.contractor_document
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {

                    dbContext.contractor_document.Remove(record);
                    await dbContext.SaveChangesAsync();

                    var uploadPath = Path.Combine(configuration["Path:UploadBasePath"].ToString(), record.file_name);
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
                var record = await dbContext.contractor_document
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    //get file 
                    var filePath = Path.Combine(configuration["Path:UploadBasePath"].ToString(), record.file_name);
                    
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

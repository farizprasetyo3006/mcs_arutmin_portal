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

namespace MCSWebApp.Controllers.API.Port
{
    [Route("api/Port/[controller]")]
    [ApiController]
    public class StatementOfFactDocumentController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        string AttachmentFolder = "sof_documents";

        public StatementOfFactDocumentController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions, string Id)
        {
            try
            {                
                return await DataSourceLoader.LoadAsync(dbContext.sof_document
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.sof_id == Id),
                        loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ByStatementOfFactId/{Id}")]
        public async Task<object> ByStatementOfFactId(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.sof_document.Where(o => o.sof_id == Id),
                loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.sof_document.Where(o => o.sof_id == Id),                    
                loadOptions);
        }
        
        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> InsertData(dynamic FileDocument)
        {
            var result = new StandardResult();

            if (FileDocument.sof_id == null)
            {
                throw new Exception("Id is null");
            }

            if (FileDocument.fileName == null)
            {
                throw new Exception("File name is empty");
            }

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(sof_document),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var sofId = (string)FileDocument.sof_id;
                    var fileName = (string)FileDocument.fileName;
                    var data = (string)FileDocument.data;
                    var remark = (string)FileDocument.remark;

                    //save to server
                    var uploadPath = Path.Combine(configuration["Path:UploadBasePath"].ToString(), AttachmentFolder, sofId);
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
                    var bargingLoadUnloadDocument = new sof_document()
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
                        remark = remark,
                        filename = savedFileName,
                        sof_id = sofId,
                    };
                    dbContext.sof_document.Add(bargingLoadUnloadDocument);
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
                var record = dbContext.sof_document
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {

                    dbContext.sof_document.Remove(record);
                    await dbContext.SaveChangesAsync();

                    var uploadPath = Path.Combine(configuration["Path:UploadBasePath"].ToString(), AttachmentFolder, record.sof_id, record.filename);
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
                var record = await dbContext.sof_document
                    .Where(o => o.id == Id)
                    .FirstOrDefaultAsync();
                if (record != null)
                {
                    //get file 
                    var filePath = Path.Combine(configuration["Path:UploadBasePath"].ToString(), AttachmentFolder, record.sof_id, record.filename);

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

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

namespace MCSWebApp.Controllers.API.ProcessFlow
{
    [Route("api/ProcessFlow/[controller]")]
    [ApiController]
    public class ProcessFlowController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ProcessFlowController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.process_flow
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.process_flow.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(process_flow),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new process_flow();
                    JsonConvert.PopulateObject(values, record);

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
					//record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    dbContext.process_flow.Add(record);
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
                var record = dbContext.process_flow
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);

                        var is_active = record.is_active;
                        //record.InjectFrom(e);
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
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Trace($"string key = {key}");

            try
            {
                var barging_transaction = dbContext.barging_transaction.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.process_flow_id == key).FirstOrDefault();
                if (barging_transaction != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var shipping_transaction = dbContext.shipping_transaction.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.process_flow_id == key).FirstOrDefault();
                if (shipping_transaction != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var production_transaction = dbContext.production_transaction.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.process_flow_id == key).FirstOrDefault();
                if (production_transaction != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var processing_transaction = dbContext.processing_transaction.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.process_flow_id == key).FirstOrDefault();
                if (processing_transaction != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var blending_plan = dbContext.blending_plan.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.process_flow_id == key).FirstOrDefault();
                if (blending_plan != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var hauling_transaction = dbContext.hauling_transaction.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.process_flow_id == key).FirstOrDefault();
                if (hauling_transaction != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var railing_transaction = dbContext.railing_transaction.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.process_flow_id == key).FirstOrDefault();
                if (railing_transaction != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var waste_removal = dbContext.waste_removal.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.process_flow_id == key).FirstOrDefault();
                if (waste_removal != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var rehandling_transaction = dbContext.rehandling_transaction.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.process_flow_id == key).FirstOrDefault();
                if (rehandling_transaction != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var record = dbContext.process_flow
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.process_flow.Remove(record);
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

        [HttpGet("SourceLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SourceLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                var lookup = dbContext.vw_business_area_structure
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = o.name_path
                        });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("SourceLocationIdByProcessFlowCategoryLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SourceLocationIdByProcessFlowCategoryLookup(DataSourceLoadOptions loadOptions, string processFlowCategory)
        {
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                if (string.IsNullOrEmpty(processFlowCategory))
                {
                    var lookup = dbContext.vw_business_area_structure
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .OrderBy(o => o.name_path)
                                .Select(o =>
                                    new
                                    {
                                        value = o.id,
                                        text = o.name_path
                                    });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var joinTable = "";

                    if (processFlowCategory.ToLower() == "processing")
                    {
                        joinTable = "stockpile_location";
                    }
                    else if (processFlowCategory.ToLower() == "production" ||
                             processFlowCategory.ToLower() == "waste removal")
                    {
                        joinTable = "mine_location";
                    }
                    else if (processFlowCategory.ToLower() == "hauling" ||
                            processFlowCategory.ToLower() == "blending" ||
                            processFlowCategory.ToLower() == "railing" ||
                            processFlowCategory.ToLower() == "rehandling")
                    {
                        joinTable = "stock_location";
                    }
                    else if (processFlowCategory.ToLower() == "barging")
                    {
                        joinTable = "port_location";
                    }
                    else if (processFlowCategory.ToLower() == "shipping")
                    {
                        joinTable = "barge";
                    }
                    else
                    {
                        // Shipping
                    }


                    var lookup = dbContext.vw_business_area_structure.FromSqlRaw(
                                " SELECT distinct b.id, b.name_path "
                                + " FROM vw_business_area_structure AS b "
                                + " INNER JOIN " + joinTable + " AS s ON b.id = s.business_area_id "
                                + " WHERE b.organization_id = {0} ORDER BY b.name_path", CurrentUserContext.OrganizationId)
                                .Select(o =>
                                    new
                                    {
                                        value = o.id,
                                        text = o.name_path,
                                    });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProductInputIdLookup")]
        public async Task<object> ProductInputIdLookup(string Category, DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (string.IsNullOrEmpty(Category) || Category != Common.ProcessFlowCategory.WASTE_REMOVAL)
                {
                    var lookup = dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o =>
                        new
                        {
                            Value = o.id,
                            Text = o.product_name
                        });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.waste
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o =>
                        new
                        {
                            Value = o.id,
                            Text = o.waste_name
                        });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("DestinationLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DestinationLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                var lookup = dbContext.vw_business_area_structure
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {
                            value = o.id,
                            text = o.name_path
                        });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("DestinationLocationIdByProcessFlowCategoryLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DestinationLocationIdByProcessFlowCategoryLookup(DataSourceLoadOptions loadOptions, string processFlowCategory)
        {
            logger.Trace($"loadOptions = {JsonConvert.SerializeObject(loadOptions)}");

            try
            {
                if (string.IsNullOrEmpty(processFlowCategory))
                {
                    var lookup = dbContext.vw_business_area_structure
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Select(o =>
                                    new
                                    {
                                        value = o.id,
                                        text = o.name_path
                                    });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                } 
                else
                {
                    var joinTable = "";

                    if (processFlowCategory.ToLower() == "production" ||
                        processFlowCategory.ToLower() == "processing")
                    {
                        joinTable = "stockpile_location";
                    }
                    else if (processFlowCategory.ToLower() == "waste removal")
                    {
                        joinTable = "waste_location";
                    }
                    else if(processFlowCategory.ToLower() == "hauling" ||
                            processFlowCategory.ToLower() == "blending" ||
                            processFlowCategory.ToLower() == "railing" ||
                            processFlowCategory.ToLower() == "rehandling")
                    {
                        joinTable = "stock_location";
                    } else
                    {
                        //-- Barging, Shipping
                        joinTable = "port_location";
                    }


                    var lookup = dbContext.vw_business_area_structure.FromSqlRaw(
                                " SELECT b.id, b.name_path "
                                + " FROM vw_business_area_structure AS b "
                                + " INNER JOIN " + joinTable + " AS s ON b.id = s.business_area_id "
                                + " WHERE b.organization_id = {0} ", CurrentUserContext.OrganizationId)
                                .Select(o =>
                                    new
                                    {
                                        value = o.id,
                                        text = o.name_path,
                                    });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProductOutputIdLookup")]
        public async Task<object> ProductOutputIdLookup(string Category, DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (string.IsNullOrEmpty(Category) || Category != Common.ProcessFlowCategory.WASTE_REMOVAL)
                {
                    var lookup = dbContext.product
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o =>
                        new
                        {
                            Value = o.id,
                            Text = o.product_name
                        });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.waste.Select(o =>
                        new
                        {
                            Value = o.id,
                            Text = o.waste_name
                        });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("EquipmentIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> EquipmentIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.equipment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.equipment_code });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("ProcessFlowCategoryLookup")]
        public object ProcessFlowCategoryLookup()
        {
            try
            {
                var lookup = Common.ProcessFlowCategory.ProcessFlowCategories
                    .Select(o => new
                    {
                        Value = o,
                        Text = o
                    });
                return lookup;
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

        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.process_flow
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Id).FirstOrDefaultAsync();
                return Ok(record);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] process_flow Record)
        {
            try
            {
                var record = dbContext.process_flow
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Record.id)
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
                    record = new process_flow();
                    record.InjectFrom(Record);

                    record.id = Guid.NewGuid().ToString("N");
                    record.created_by = CurrentUserContext.AppUserId;
                    record.created_on = DateTime.Now;
                    record.modified_by = null;
                    record.modified_on = null;
                   // record.is_active = true;
                    record.is_default = null;
                    record.is_locked = null;
                    record.entity_id = null;
                    record.owner_id = CurrentUserContext.AppUserId;
                    record.organization_id = CurrentUserContext.OrganizationId;
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    dbContext.process_flow.Add(record);
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
                var record = dbContext.process_flow
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.process_flow.Remove(record);
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

                    var source_location_id = "";
                    var source = dbContext.business_area
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.business_area_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                    if (source != null) source_location_id = source.id.ToString();

                    var destination_location_id = "";
                    var destination = dbContext.business_area
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.business_area_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower()).FirstOrDefault();
                    if (destination != null) destination_location_id = destination.id.ToString();

                    var sampling_template_id = "";
                    var sampling_template = dbContext.sampling_template
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.sampling_template_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower()).FirstOrDefault();
                    if (sampling_template != null) sampling_template_id = sampling_template.id.ToString();

                    var record = dbContext.process_flow
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.process_flow_code == PublicFunctions.IsNullCell(row.GetCell(1)))
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.process_flow_name = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.process_flow_category = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.source_location_id = source_location_id;
                        record.destination_location_id = destination_location_id;
                        record.sampling_template_id = sampling_template_id;
                        record.assume_source_quality = PublicFunctions.BenarSalah(row.GetCell(6));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new process_flow();
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

                        record.process_flow_name = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.process_flow_code = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.process_flow_category = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.source_location_id = source_location_id;
                        record.destination_location_id = destination_location_id;
                        record.sampling_template_id = sampling_template_id;
                        record.assume_source_quality = PublicFunctions.BenarSalah(row.GetCell(6));

                        dbContext.process_flow.Add(record);
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
                HttpContext.Session.SetString("filename", "ProcessFlow");
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

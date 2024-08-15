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

namespace MCSWebApp.Controllers.API.Material
{
    [Route("api/Material/[controller]")]
    [ApiController]
    public class ProductController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ProductController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            var record = dbContext.vw_product
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                .FirstOrDefault();
            if (record != null)
            {
                if ((await mcsContext.CanRead(dbContext, record.id, CurrentUserContext.AppUserId, HttpContext.Session.GetString("BUSINESS_UNIT_ID")) 
                    || CurrentUserContext.IsSysAdmin) == false)
                        return BadRequest("User is not authorized.");
            }

            return await DataSourceLoader.LoadAsync(dbContext.vw_product
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.product.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        

        [HttpGet("ProductCategoryIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProductCategoryIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.product_category
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.product_category_name, Search = o.product_category_name.ToLower() + o.product_category_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("ProductIdLookup")]
        public async Task<object> ProductIdLookup(string id,DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (id != null)
                {
                    var lookup = dbContext.vw_product
                       .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                       .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin && o.product_category_id == id)
                       .Select(o => new { Value = o.id, Text = o.product_name, o.product_category_id, Search = o.product_name.ToLower() + o.product_name.ToUpper() })
                       .OrderBy(o=>o.Text);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                    
                }
                else
                {
                    var lookup = dbContext.vw_product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                        .Select(o => new { Value = o.id, Text = o.product_name, o.product_category_id, Search = o.product_name.ToLower() + o.product_name.ToUpper() })
                       .OrderBy(o => o.Text);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("ProductIdFilteredLookup/{id}/{status}")]
        public async Task<object> ProductIdFilteredLookup(string id, bool status, DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (status == true)
                {
                    var lookup1 = dbContext.vw_product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => !dbContext.product_standard.Any(x => x.product_id == o.id))
                        .Select(o => new { Value = o.id, Text = o.product_name, Search = o.product_name.ToLower() + o.product_name.ToUpper(), numb = 1 })
                        .OrderBy(o => o.Text);
                    var lookup2 = dbContext.vw_product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.id == id)
                        .Select(o => new { Value = o.id, Text = o.product_name, Search = o.product_name.ToLower() + o.product_name.ToUpper(),numb = 2 })
                        .OrderBy(o => o.Text);
                    var lookup = lookup1.Union(lookup2).OrderBy(o=>o.Text);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.vw_product
                       .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                       .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                       .Select(o => new { Value = o.id, Text = o.product_name, o.product_category_id, Search = o.product_name.ToLower() + o.product_name.ToUpper() })
                      .OrderBy(o => o.Text);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProductIdNPLCT")]
        public async Task<object> ProductIdNPLCT(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_product
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o=>o.business_unit_name == "NPLCT (NORTH PULAU LAUT COAL TERMINAL)")
                    .Where(o=>o.is_active == true)
                    .Select(o => new { Value = o.id, Text = o.product_name, o.product_category_id, Search = o.product_name.ToLower() + o.product_name.ToUpper() })
                    .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProductIdNoFilterBusinessUnitLookup")]
        public async Task<object> ProductIdNoFilterBusinessUnitLookup(string id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (id != null)
                {
                    var lookup = dbContext.vw_product
                       .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.product_category_name == "COAL BRAND")
                       .Select(o => new { Value = o.id, Text = o.product_name, o.product_category_id, Search = o.product_name.ToLower() + o.product_name.ToUpper() })
                       .OrderBy(o => o.Text);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);

                }
                else
                {
                    var lookup = dbContext.vw_product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.product_category_name == "COAL BRAND")
                        .Select(o => new { Value = o.id, Text = o.product_name, o.product_category_id, Search = o.product_name.ToLower() + o.product_name.ToUpper() })
                       .OrderBy(o => o.Text);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProductIdLookupWhereCategory")]
        public async Task<object> ProductIdLookupWhereCategory(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_product
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.product_category_name == "COAL BRAND")
                    .Select(o => new { Value = o.id, Text = o.product_name, Search = o.product_name.ToLower() + o.product_name.ToUpper() })
                    .OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProductIdLookupB")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ProductIdLookupB(string ProcessFlowId, DataSourceLoadOptions loadOptions)
        {
            //logger.Trace($"ProcessFlowId = {ProcessFlowId}");

            try
            {
                var lookup = dbContext.product
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.product_name, Search = o.product_name.ToLower() + o.product_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);

                /*
                if (string.IsNullOrEmpty(ProcessFlowId))
                {
                    var lookup = dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Select(o => new { Value = o.id, Text = o.product_name });
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup = dbContext.product.FromSqlRaw(
                          " SELECT p.* FROM product p "
                        + " INNER JOIN stockpile_location l ON l.product_id = p.id "
                        + " WHERE p.organization_id = {0} "
                        + " AND l.business_area_id IN ( "
                        + "     SELECT ba.id FROM vw_business_area_structure ba, process_flow pf "
                        + "     WHERE position(pf.source_location_id in ba.id_path) > 0 "
                        + "         AND pf.id = {1} "
                        + " ) ", 
                          CurrentUserContext.OrganizationId, ProcessFlowId
                        )
                        .Select(o =>
                            new
                            {
                                value = o.id,
                                text = o.product_name
                            })
                        .Distinct();
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                */
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        #region commented bcs in portal u cant create,update or delete. just read
        /*[HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(product),
                    CurrentUserContext.AppUserId, HttpContext.Session.GetString("BUSINESS_UNIT_ID")) || CurrentUserContext.IsSysAdmin)
                {
                    var record = new product();
                    JsonConvert.PopulateObject(values, record);

                    var cekdata = dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.product_code.ToLower().Trim() == record.product_code.ToLower().Trim())
                        .FirstOrDefault();
                    if (cekdata != null) return BadRequest("Duplicate Code field.");

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

                    dbContext.product.Add(record);
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
                var record = dbContext.product
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                        || CurrentUserContext.IsSysAdmin)
                    {
                        *//*var e = new entity();
                        e.InjectFrom(record);*//*

                        JsonConvert.PopulateObject(values, record);

                        var cekdata = dbContext.product
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.product_code.ToLower().Trim() == record.product_code.ToLower().Trim()
                                && o.id != record.id)
                            .FirstOrDefault();
                        if (cekdata != null) return BadRequest("Duplicate Code field.");

                        var is_active = record.is_active;
                        record.InjectFrom(values);
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
            logger.Debug($"string key = {key}");

            try
            {
                var timesheet_detail = dbContext.timesheet_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.material_id == key).FirstOrDefault();
                if (timesheet_detail != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var record = dbContext.product
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.product.Remove(record);
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

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] product Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.product
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
                            await tx.CommitAsync();
                            return Ok(record);
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                    }
                    else if (await mcsContext.CanCreate(dbContext, nameof(product),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new product();
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

                        dbContext.product.Add(record);
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
                    if (ex.InnerException != null)
                    {
                        logger.Error(ex.InnerException.Message);
                        return BadRequest(ex.InnerException.Message);
                    }
                    else
                    {
                        logger.Error(ex.ToString());
                        return BadRequest(ex.Message);
                    }
                }
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

                    string product_category_id = "";
                    var product_category = dbContext.product_category
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.product_category_code == PublicFunctions.IsNullCell(row.GetCell(1))).FirstOrDefault();
                    if (product_category != null) product_category_id = product_category.id.ToString();

                    string coa_id = "";
                    var coa = dbContext.coa.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.account_code == PublicFunctions.IsNullCell(row.GetCell(3))).FirstOrDefault();
                    if (coa != null) coa_id = coa.id.ToString();

                    string business_unit_id = "";
                    var business_unit = dbContext.business_unit.Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                                                                && o.business_unit_code == PublicFunctions.IsNullCell(row.GetCell(5))).FirstOrDefault();
                    if(business_unit != null) business_unit_id = business_unit.id.ToString();

                    var record = dbContext.product
                        .Where(o => o.product_code == PublicFunctions.IsNullCell(row.GetCell(0))
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.product_category_id = product_category_id;
                        record.product_name = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.coa_id = coa_id;
                        record.is_active = PublicFunctions.BenarSalah(row.GetCell(4));
                        record.business_unit_id = business_unit_id;

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new product();
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

                        record.product_code = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.product_category_id = product_category_id;
                        record.product_name = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.coa_id = coa_id;
                        record.is_active = PublicFunctions.BenarSalah(row.GetCell(4));
                        record.business_unit_id = business_unit_id;

                        dbContext.product.Add(record);
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

            sheet = wb.GetSheetAt(1); //*** detail sheet
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    string product_id = null;
                    var product = dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.product_code == PublicFunctions.IsNullCell(row.GetCell(0)))
                        .FirstOrDefault();
                    if (product != null) product_id = product.id.ToString();

                    string analyte_id = null;
                    var analyte = dbContext.analyte
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.analyte_name.ToUpper() == row.GetCell(1).ToString().ToUpper())
                        .FirstOrDefault();
                    if (analyte != null) analyte_id = analyte.id.ToString();

                    string uom_id = null;
                    var uom = dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.uom_symbol == PublicFunctions.IsNullCell(row.GetCell(2)))
                        .FirstOrDefault();
                    if (uom != null) uom_id = uom.id.ToString();

                    var record = dbContext.product_specification
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        .Where(o => o.product_id == product_id)
                        .Where(o => o.analyte_id == analyte_id)
                        .FirstOrDefault();

                    if (record != null)
                    {
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.analyte_id = analyte_id;
                        record.uom_id = uom_id;
                        record.minimum_value = PublicFunctions.Desimal(row.GetCell(3));
                        record.target_value = PublicFunctions.Desimal(row.GetCell(4));
                        record.maximum_value = PublicFunctions.Desimal(row.GetCell(5));
                        record.applicable_date = PublicFunctions.Tanggal(row.GetCell(6));
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new product_specification();
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

                        record.product_id = product_id;
                        record.analyte_id = analyte_id;
                        record.uom_id = uom_id;
                        record.minimum_value = PublicFunctions.Desimal(row.GetCell(3));
                        record.target_value = PublicFunctions.Desimal(row.GetCell(4));
                        record.maximum_value = PublicFunctions.Desimal(row.GetCell(5));
                        record.applicable_date = PublicFunctions.Tanggal(row.GetCell(6));

                        dbContext.product_specification.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 2, Line " + (i+1) + " : " + Environment.NewLine;
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
                HttpContext.Session.SetString("filename", "Product");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }*/
        #endregion
    }
}

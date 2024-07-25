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
using NPOI.SS.Formula.Functions;

namespace MCSWebApp.Controllers.API.SurveyManagement
{
    [Route("api/SurveyManagement/[controller]")]
    [ApiController]
    public class COWController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public COWController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_draft_survey
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.draft_survey.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpGet("DataDetailByDespatchOrder")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetailByDespatchOrder(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.draft_survey.Where(o => o.despatch_order_id == Id
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
				if (await mcsContext.CanCreate(dbContext, nameof(draft_survey),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new draft_survey();
                    JsonConvert.PopulateObject(values, record);

                    //********** Validasi
                    if (!string.IsNullOrEmpty(record.despatch_order_id) && record.non_commercial != true)
                    {
                        //var DraftSurvey = dbContext.draft_survey
                        //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        //        && o.despatch_order_id == record.despatch_order_id)
                        //    .FirstOrDefault();
                        //if (DraftSurvey != null)
                        //{
                        //    var SalesInvoice = dbContext.vw_sales_invoice
                        //        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        //            && o.despatch_order_id == record.despatch_order_id
                        //            && o.approve_status == "APPROVED")
                        //        .FirstOrDefault();
                        //    if (SalesInvoice != null)
                        //    {
                        //        return BadRequest($"The Sales Invoice has been approved. Can't use this Shipping Order Number.");
                        //    }
                        //}

                        var DraftSurvey = dbContext.draft_survey
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.despatch_order_id == record.despatch_order_id
                                && o.non_commercial != true)
                            .FirstOrDefault();
                        if (DraftSurvey != null)
                        {
                            return BadRequest($"This Shipping Order Number has been used in other COW.");
                        }
                    }
                    //***************

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

                    dbContext.draft_survey.Add(record);
                    await dbContext.SaveChangesAsync();

                    #region Insert draft_survey analytes

                    if (!string.IsNullOrEmpty(record.sampling_template_id))
                    {
                        var st = dbContext.sampling_template
                            .Where(o => o.id == record.sampling_template_id)
                            .FirstOrDefault();
                        if(st != null)
                        {
                            var details = dbContext.sampling_template_detail
                                .Where(o => o.sampling_template_id == st.id)
                                .ToList();
                            if(details != null && details.Count > 0)
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
                                        analyte_id = d.analyte_id
                                    });
                                }

                                await dbContext.SaveChangesAsync();
                            }
                        }
                    }

                    #endregion

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

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.draft_survey
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (!string.IsNullOrEmpty(record.despatch_order_id) && record.non_commercial != true)
                        {
                            //var SalesInvoice = dbContext.vw_sales_invoice
                            //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            //        && o.despatch_order_id == record.despatch_order_id
                            //        && o.approve_status == "APPROVED")
                            //    .FirstOrDefault();
                            //if (SalesInvoice != null)
                            //{
                            //    return BadRequest($"The Sales Invoice has been approved. Can't use this Shipping Order Number.");
                            //}


                            var DraftSurvey = dbContext.draft_survey
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                    && o.despatch_order_id == record.despatch_order_id
                                    && o.non_commercial != true
                                    && o.id != record.id)
                                .FirstOrDefault();
                            if (DraftSurvey != null)
                            {
                                return BadRequest($"This Shipping Order Number has been used in other COW.");
                            }
                        }

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
        }

        [HttpPost("UpdateSurveyNumber")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateSurveyNumber([FromForm] string key, [FromForm] string values)
        {
            try
            {
                var record = dbContext.draft_survey
                    .Where(o => (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin) && o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    var e = new entity();
                    e.InjectFrom(record);

                    JsonConvert.PopulateObject(values, record);

                    record.InjectFrom(e);

                    record.modified_by = CurrentUserContext.AppUserId;
                    record.modified_on = System.DateTime.Now;

                    await dbContext.SaveChangesAsync();
                    return Ok(record);
                }

                return BadRequest("ID COW not found.");
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("GetSurveyNumber/{Id}")]
        public object GetSurveyNumber(string Id)
        {
            try
            {
                var result = new draft_survey();

                result = dbContext.draft_survey.Where(o => o.id == Id).FirstOrDefault();

                return result;
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

            var success = false;
            draft_survey record;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.draft_survey
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            if (record.approved_by != null)
                            {
                                return BadRequest("Record cannot be updated. Status is Closed.");
                            }

                            dbContext.draft_survey.Remove(record);
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
            /*
            if (success && record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.draft_survey();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.DraftSurvey.UpdateStockState(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
            return Ok();
        }

        [HttpGet("GetQuantity")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetQuantity(string Id,DataSourceLoadOptions loadOptions)
        {
            try { 
              
                    var DO = await dbContext.vw_despatch_order.Where(o => o.id == Id).FirstOrDefaultAsync();
                if (DO.delivery_term_name.Contains("BARGE"))
                {
                    var BT = new barging_transaction();
                    BT = await dbContext.barging_transaction.Where(o => o.despatch_order_id == Id && o.is_loading == true).FirstOrDefaultAsync();
                    return BT;
                   //return await DataSourceLoader.LoadAsync(BT,loadOptions);
                   /* return await DataSourceLoader.LoadAsync(dbContext.barging_transaction.Where(o => o.despatch_order_id == Id && o.is_loading == true),
                loadOptions);*/
                }
                else if (DO.delivery_term_name.Contains("VESSEL"))
                {
                    var ST = new shipping_transaction();
                    ST = dbContext.shipping_transaction.Where(o => o.despatch_order_id == Id && o.is_loading == true).FirstOrDefault();
                    return ST;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpGet("StockLocationIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> StockLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                //var lookup = dbContext.stock_location
                //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                //    .Select(o => new { Value = o.id, Text = o.stock_location_name });

                var barges = dbContext.barge
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { Value = o.id, Text = o.vehicle_name, search = o.vehicle_name.ToLower() + o.vehicle_name.ToUpper() });
                var vessels = dbContext.vessel
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { Value = o.id, Text = o.vehicle_name, search = o.vehicle_name.ToLower() + o.vehicle_name.ToUpper() });
                var bv = barges.Union(vessels);

                return await DataSourceLoader.LoadAsync(bv, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("QualitySamplingIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> QualitySamplingIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.quality_sampling
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sampling_number, search = o.sampling_number.ToLower() + o.sampling_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SurveyorIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> SurveyorIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.business_partner
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.is_vendor == true)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.business_partner_name, search = o.business_partner_name.ToLower() + o.business_partner_name.ToUpper() });
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
                    .Select(o => new { Value = o.id, Text = o.survey_number, search = o.survey_number.ToLower() + o.survey_number.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("DraftSurveyIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DraftSurveyIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.draft_survey
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.survey_number, search = o.survey_number.ToLower() + o.survey_number.ToUpper() });
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
                var record = await dbContext.vw_draft_survey
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
        public async Task<IActionResult> SaveData([FromBody] draft_survey Record)
        {
            try
            {
                var record = dbContext.draft_survey
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
                    record = new draft_survey();
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

                    dbContext.draft_survey.Add(record);
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
                var record = dbContext.draft_survey
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.draft_survey.Remove(record);
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
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Approve([FromQuery]string Id)
        {
            var result = new StandardResult();
            draft_survey record = null;

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    record = dbContext.draft_survey
                        .Where(o => o.id == Id && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, Id, CurrentUserContext.AppUserId)
                                    || CurrentUserContext.IsSysAdmin)
                        {
                            var e = new entity();
                            e.InjectFrom(record);

                            //JsonConvert.PopulateObject(values, record);

                            record.InjectFrom(e);
                            record.approved_by = CurrentUserContext.AppUserId;
                            record.approved_on = DateTime.Now;

                            await dbContext.SaveChangesAsync();
                            result.Success = true;
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
                    result.Message = ex.InnerException?.Message ?? ex.Message;
                }
            }
            /*
            if (result.Success && record != null)
            {
                try
                {
                    var _record = new DataAccess.Repository.draft_survey();
                    _record.InjectFrom(record);
                    var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                    _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.DraftSurvey.UpdateStockState(connectionString, _record));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            */
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

                    var surveyor_id = "";
                    var surveyor = dbContext.business_partner
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.business_partner_code == PublicFunctions.IsNullCell(row.GetCell(2))).FirstOrDefault();
                    if (surveyor != null) surveyor_id = surveyor.id.ToString();

                    var stock_location_id = "";
                    var stock_location = dbContext.stock_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.stock_location_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                    if (stock_location != null) stock_location_id = stock_location.id.ToString();

                    var product_id = "";
                    var product = dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.product_code == PublicFunctions.IsNullCell(row.GetCell(4))).FirstOrDefault();
                    if (product != null) product_id = product.id.ToString();

                    var uom_id = "";
                    var uom = dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.uom_symbol == PublicFunctions.IsNullCell(row.GetCell(6))).FirstOrDefault();
                    if (uom != null) uom_id = uom.id.ToString();

                    var sampling_template_id = "";
                    var sampling_template = dbContext.sampling_template
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.sampling_template_code == PublicFunctions.IsNullCell(row.GetCell(7))).FirstOrDefault();
                    if (sampling_template != null) sampling_template_id = sampling_template.id.ToString();

                    var despatch_order_id = "";
                    var despatch_order = dbContext.despatch_order
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.despatch_order_number == PublicFunctions.IsNullCell(row.GetCell(8))).FirstOrDefault();
                    if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();

                    var record = dbContext.draft_survey
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
                        
                        record.survey_date = Convert.ToDateTime(row.GetCell(1).ToString());
                        record.surveyor_id = surveyor_id;
                        record.stock_location_id = stock_location_id;
                        record.product_id = product_id;
                        record.quantity = PublicFunctions.Desimal(row.GetCell(5));
                        record.uom_id = uom_id;
                        record.sampling_template_id = sampling_template_id;
                        record.despatch_order_id = despatch_order_id;

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new draft_survey();
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

                        record.survey_number = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.survey_date = PublicFunctions.Tanggal(row.GetCell(1));
                        record.surveyor_id = surveyor_id;
                        record.stock_location_id = stock_location_id;
                        record.product_id = product_id;
                        record.quantity = PublicFunctions.Desimal(row.GetCell(5));
                        record.uom_id = uom_id;
                        record.sampling_template_id = sampling_template_id;
                        record.despatch_order_id = despatch_order_id;

                        dbContext.draft_survey.Add(record);
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

            sheet = wb.GetSheetAt(1); //*** detail sheet 1
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var survey_id = "";
                    var draft_survey = dbContext.draft_survey
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.survey_number == PublicFunctions.IsNullCell(row.GetCell(0))).FirstOrDefault();
                    if (draft_survey != null) survey_id = draft_survey.id.ToString();

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
                        teks += "==>Error Sheet 2, Line " + (i+1) + " : " + Environment.NewLine;
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
                    var draft_survey = dbContext.draft_survey
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.survey_number == PublicFunctions.IsNullCell(row.GetCell(0))).FirstOrDefault();
                    if (draft_survey != null) survey_id = draft_survey.id.ToString();

                    var record = dbContext.survey_detail
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.survey_id == survey_id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        // var e = new entity();
                        // e.InjectFrom(record);

                        // record.InjectFrom(e);
                        // record.modified_by = CurrentUserContext.AppUserId;
                        // record.modified_on = DateTime.Now;
                        
                        // record.quantity = PublicFunctions.Desimal(row.GetCell(1));
                        // record.distance = PublicFunctions.Desimal(row.GetCell(2));
                        // record.elevation = PublicFunctions.Desimal(row.GetCell(3));

                        // await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        // record = new survey_detail();
                        // record.id = Guid.NewGuid().ToString("N");
                        // record.created_by = CurrentUserContext.AppUserId;
                        // record.created_on = DateTime.Now;
                        // record.modified_by = null;
                        // record.modified_on = null;
                        // record.is_active = true;
                        // record.is_default = null;
                        // record.is_locked = null;
                        // record.entity_id = null;
                        // record.owner_id = CurrentUserContext.AppUserId;
                        // record.organization_id = CurrentUserContext.OrganizationId;
					    // record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                        // record.survey_id = survey_id;
                        // record.quantity = PublicFunctions.Desimal(row.GetCell(1));
                        // record.distance = PublicFunctions.Desimal(row.GetCell(2));
                        // record.elevation = PublicFunctions.Desimal(row.GetCell(3));

                        // dbContext.survey_detail.Add(record);
                        // await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 3, Line " + (i+1) + " : " + Environment.NewLine;
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
                HttpContext.Session.SetString("filename", "DraftSurvey");
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

        [HttpGet("LookupByDespatchOrderId/{despatchOrderId}")]
        public async Task<StandardResult> LookupByDespatchOrderId(string despatchOrderId)
        {
            var result = new StandardResult();
            result.Success = true;
            try
            {
                result.Data = await dbContext.vw_draft_survey.FirstOrDefaultAsync(o => o.despatch_order_id == despatchOrderId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException.Message ?? ex.Message);
                result.Message = ex.InnerException.Message ?? ex.Message;
            }
            return result;
        }

        [HttpGet("DespatchOrderIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DespatchOrderIdLookup(string id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    var lookup = dbContext.vw_despatch_order
                                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                        .Where(o => !dbContext.vw_draft_survey.Any(x => x.despatch_order_id == o.id))
                                        .Select(o => new { Value = o.id, Text = o.despatch_order_number })
                                        .Distinct();
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
                else
                {
                    var lookup1 = dbContext.vw_despatch_order
                                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                        .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                        .Where(o => !dbContext.vw_draft_survey.Any(x => x.despatch_order_id == o.id))
                                        .Select(o => new { Value = o.id, Text = o.despatch_order_number });
                    var lookup2 = dbContext.vw_despatch_order
                                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                        .Where(o => o.id == id)
                                        .Select(o => new { Value = o.id, Text = o.despatch_order_number });
                    var lookup = lookup1.Union(lookup2);
                    return await DataSourceLoader.LoadAsync(lookup, loadOptions);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("GetBillLadingDate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<StandardResult> GetBillLadingDate(string Id, DataSourceLoadOptions loadOptions)
        {
            var result = new StandardResult();
            Dictionary<string, dynamic> myData = new Dictionary<string, dynamic>();

            try
            {
                dynamic billLadingDate = null;
                string incoterm = "";

                dynamic record = await dbContext.vw_despatch_order
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                if (record != null)
                {
                    incoterm = record.delivery_term_name ?? "";
                }

                if (incoterm == "CIF BARGE" || incoterm == "FOB BARGE")
                {
                    record = await dbContext.barging_transaction
                        .Where(o => o.despatch_order_id == Id)
                        .OrderByDescending(o => o.created_on)
                        .ThenByDescending(o => o.modified_on)
                        .FirstOrDefaultAsync();
                    if (record != null)
                    {
                        billLadingDate = record.end_datetime ?? null;
                        myData.Add("bill_lading_date", billLadingDate);
                        result.Data = myData;
                        result.Success = true;
                        result.Message = "Ok";
                    }
                }
                else if (incoterm == "CIF MV" || incoterm == "FOB VESSEL")
                {
                    record = await dbContext.shipping_transaction
                        .Where(o => o.despatch_order_id == Id).FirstOrDefaultAsync();
                    if (record != null)
                    {
                        billLadingDate = record.end_datetime ?? null;
                        myData.Add("bill_lading_date", billLadingDate);
                        result.Data = myData;
                        result.Success = true;
                        result.Message = "Ok";
                    }
                }
                else
                {
                    result.Data = null;
                    result.Success = false;
                    result.Message = "Error.";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

    }
}

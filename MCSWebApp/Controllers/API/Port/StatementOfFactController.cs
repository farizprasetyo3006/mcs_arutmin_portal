
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

namespace MCSWebApp.Controllers.API.Port
{
    [Route("api/Port/[controller]")]
    [ApiController]
    public class StatementOfFactController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public StatementOfFactController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_sof
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("StatemenfOfFactIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> StatemenfOfIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.sof
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sof_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("StatemenfOfFactByCustomerIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> StatemenfOfFactByCustomerIdLookup(string customerId, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_sof
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.customer_id == customerId)
                    .Select(o => new { Value = o.id, Text = o.sof_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("StatemenfOfFactByDespatchOrderIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> StatemenfOfFactByDespatchOrderIdLookup(string despatchOrderId, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_sof
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.despatch_order_id == despatchOrderId)
                    .Select(o => new { Value = o.id, Text = o.sof_number, /*Issued_date = o.nor_accepted*/});
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        [HttpGet("GetNor/{DO}")]
        public object GetSurveyNumber(string DO)
        {
            try
            {
                var result = new sof();

                result = dbContext.sof.Where(o => o.despatch_order_id == DO).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DesDemTermIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DesDemTermIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.despatch_demurrage_detail
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.term_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DesDemContractById")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DesDemContractById(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var despatch_demurrage_id = "";
                var despatch_demurrage_detail = dbContext.despatch_demurrage_detail.Where(o => o.id == Id).FirstOrDefault();
                if (despatch_demurrage_detail != null) despatch_demurrage_id = despatch_demurrage_detail.despatch_demurrage_id.ToString();

                var lookup = dbContext.despatch_demurrage
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == despatch_demurrage_id);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("GetDetailById/{Id}")]
        public async Task<object> GetDetailById(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(
                    dbContext.vw_sof
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Id),
                    loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetSofDetailById/{Id}")]
        public async Task<StandardResult> GetSofDetailById(string Id)
        {
            var result = new StandardResult();
            result.Success = true;
            Dictionary<string, dynamic> myData = new Dictionary<string, dynamic>();
            try
            {
                var record = await dbContext.vw_sof.FirstOrDefaultAsync(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Id);
                if (record != null)
                {
                    myData.Add("record", record);
                    var sofDetails = await dbContext.vw_sof_detail.Where(r => r.sof_id == record.id).ToListAsync();
                    if (sofDetails.Count > 0)
                    {
                        double totalSeconds = 0;
                        foreach (var item in sofDetails)
                        {
                            if (item.start_datetime > item.end_datetime)
                            {
                                result.Message = "Start Time cannot be greater than End Time";
                                result.Success = false;
                                return result;
                            }

                            var divider = (double)(item.percentage / 100);
                            totalSeconds += (item.end_datetime.Value).Subtract(item.start_datetime.Value).TotalSeconds * divider;
                        }

                        if (totalSeconds > 0)
                        {
                            var laytime_text = secondsToDhms((decimal)totalSeconds);
                            myData.Add("laytime_text", laytime_text);
                            myData.Add("laytime_duration", totalSeconds);
                        }
                    }

                    result.Data = myData;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.sof.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(sof),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new sof();
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
                    record.sof_name = record.sof_number;

                    if (isDespatchOrderExist(record.id, record.despatch_order_id))
                    {
                        return BadRequest("Shipping Order already used.");
                    }

                    var despatchOrderRecord = dbContext.despatch_order
                        .Where(o => o.id == record.despatch_order_id && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();

                    if (despatchOrderRecord != null)
                        record.vessel_id = despatchOrderRecord.vessel_id;

                    dbContext.sof.Add(record);
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
                var record = dbContext.sof
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

                       // record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        if (isDespatchOrderExist(record.id, record.despatch_order_id))
                        {
                            return BadRequest("Shipping Order already used.");
                        }

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
                var record = dbContext.sof
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.sof.Remove(record);
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

                    var desdem_term_id = "";
                    var despatch_demurrage_detail = dbContext.despatch_demurrage_detail
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.term_name.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).Trim().ToLower())
                        .FirstOrDefault();
                    if (despatch_demurrage_detail != null) desdem_term_id = despatch_demurrage_detail.id.ToString();

                    var despatch_order_id = "";
                    var despatch_order = dbContext.despatch_order
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.despatch_order_number.Trim().ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).Trim().ToLower())
                        .FirstOrDefault();
                    if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();

                    var record = dbContext.sof
                        .Where(o => o.sof_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.desdem_term_id = desdem_term_id;
                        record.despatch_order_id = despatch_order_id;
                        record.nor_tendered = PublicFunctions.Tanggal(row.GetCell(3));
                        record.nor_accepted = PublicFunctions.Tanggal(row.GetCell(4));
                        record.laytime_commenced = PublicFunctions.Tanggal(row.GetCell(5));
                        record.commenced_loading = PublicFunctions.Tanggal(row.GetCell(6));
                        record.completed_loading = PublicFunctions.Tanggal(row.GetCell(7));
                        record.laytime_completed = PublicFunctions.Tanggal(row.GetCell(8));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new sof();
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

                        record.sof_number = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.desdem_term_id = desdem_term_id;
                        record.despatch_order_id = despatch_order_id;
                        record.nor_tendered = PublicFunctions.Tanggal(row.GetCell(3));
                        record.nor_accepted = PublicFunctions.Tanggal(row.GetCell(4));
                        record.laytime_commenced = PublicFunctions.Tanggal(row.GetCell(5));
                        record.commenced_loading = PublicFunctions.Tanggal(row.GetCell(6));
                        record.completed_loading = PublicFunctions.Tanggal(row.GetCell(7));
                        record.laytime_completed = PublicFunctions.Tanggal(row.GetCell(8));

                        dbContext.sof.Add(record);
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

                    var railing_transaction_id = "";
                    var railing_transaction = dbContext.railing_transaction
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.transaction_number == PublicFunctions.IsNullCell(row.GetCell(0))).FirstOrDefault();
                    if (railing_transaction != null) railing_transaction_id = railing_transaction.id.ToString();

                    var wagon_id = "";
                    var wagon = dbContext.wagon
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.vehicle_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower()).FirstOrDefault();
                    if (wagon != null) wagon_id = wagon.id.ToString();

                    var uom_id = "";
                    var uom = dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.uom_symbol == PublicFunctions.IsNullCell(row.GetCell(6))).FirstOrDefault();
                    if (uom != null) uom_id = uom.id.ToString();

                    var record = dbContext.sof_detail
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.sof_id.ToLower() == railing_transaction_id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        //record.loading_datetime = PublicFunctions.Tanggal(row.GetCell(2));
                        //record.loading_quantity = PublicFunctions.Desimal(row.GetCell(3));
                        //record.unloading_datetime = PublicFunctions.Tanggal(row.GetCell(4));
                        //record.unloading_quantity = PublicFunctions.Desimal(row.GetCell(5));
                        //record.uom_id = uom_id;

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new sof_detail();
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

                        //record.railing_transaction_id = railing_transaction_id;
                        //record.wagon_id = wagon_id;
                        //record.loading_datetime = PublicFunctions.Tanggal(row.GetCell(2));
                        //record.loading_quantity = PublicFunctions.Desimal(row.GetCell(3));
                        //record.unloading_datetime = PublicFunctions.Tanggal(row.GetCell(4));
                        //record.unloading_quantity = PublicFunctions.Desimal(row.GetCell(5));
                        //record.uom_id = uom_id;

                        dbContext.sof_detail.Add(record);
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
                HttpContext.Session.SetString("filename", "SOF");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string secondsToDhms(decimal seconds)
        {
            var d = Math.Floor(seconds / (3600 * 24));
            var h = Math.Floor(seconds % (3600 * 24) / 3600);
            var m = Math.Floor(seconds % 3600 / 60);
            var s = Math.Floor(seconds % 60);

            var dDisplay = d > 0 ? d + (d == 1 ? " Day " : " Days ") : "";
            var hDisplay = h > 0 ? h + (h == 1 ? " Hour " : " Hours ") : "";
            var mDisplay = m > 0 ? m + (m == 1 ? " Minute " : " Minutes ") : "";
            return dDisplay + hDisplay + mDisplay;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool isDespatchOrderExist(string sofId, string despatchOrderId)
        {
            var sofRecord = dbContext.sof
                        .Where(o => o.id != sofId 
                        && o.despatch_order_id == despatchOrderId 
                        && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
            if (sofRecord != null)
            {
                return true;
            }

            return false;
        }
    }
}


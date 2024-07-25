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
using NLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataAccess.DTO;
using Omu.ValueInjecter;
using Microsoft.EntityFrameworkCore;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using BusinessLogic;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Common;

namespace MCSWebApp.Controllers.API.Transport
{
    [Route("api/Transport/[controller]")]
    [ApiController]
    public class TruckController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public TruckController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            var record = dbContext.vw_truck
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                .FirstOrDefault();
            if (record != null)
            {
                if ((await mcsContext.CanRead(dbContext, record.id, CurrentUserContext.AppUserId, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                    || CurrentUserContext.IsSysAdmin) == false)
                    return BadRequest("User is not authorized.");
            }

            return await DataSourceLoader.LoadAsync(dbContext.vw_truck
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.truck.Where(o => o.id == Id), 
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(truck),
					CurrentUserContext.AppUserId, HttpContext.Session.GetString("BUSINESS_UNIT_ID")) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new truck();
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

                    dbContext.truck.Add(record);
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
                var record = dbContext.truck
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);

                       // record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

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
            try
            {
                #region Validation

                dynamic cekTrans = dbContext.timesheet.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.cn_unit_id == key).Count();
                if (cekTrans > 0) return BadRequest("Can't be deleted, already been used in Timesheet.");

                cekTrans = dbContext.daywork.Where(o => o.equipment_id == key).Count();
                if (cekTrans > 0) return BadRequest("Can't be deleted, already been used in Daywork.");

                cekTrans = dbContext.hauling_transaction.Where(o => o.transport_id == key).Count();
                if (cekTrans > 0) return BadRequest("Can't be deleted, already been used in Hauling.");

                cekTrans = dbContext.production_transaction.Where(o => o.transport_id == key).Count();
                if (cekTrans > 0) return BadRequest("Can't be deleted, already been used in Mining Production.");

                cekTrans = dbContext.rehandling_transaction.Where(o => o.transport_id == key).Count();
                if (cekTrans > 0) return BadRequest("Can't be deleted, already been used in Rehandling.");

                cekTrans = dbContext.waste_removal.Where(o => o.transport_id == key).Count();
                if (cekTrans > 0) return BadRequest("Can't be deleted, already been used in Waste Removal.");

                #endregion

                var record = dbContext.truck
                    .Where(o => o.id == key && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId, HttpContext.Session.GetString("BUSINESS_UNIT_ID"))
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.truck.Remove(record);
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

        [HttpGet("TruckIdLookup")]
        public async Task<object> TruckIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.truck
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.vehicle_id + " - " + o.vehicle_name, search = o.vehicle_id.ToLower() + " - " + o.vehicle_name.ToLower() + o.vehicle_id.ToUpper() + " - "+o.vehicle_name.ToUpper()  });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("TransportIdLookup")]
        public async Task<object> TransportIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var equipments = dbContext.vw_equipment
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    //.Select(o => new { Value = o.id, Text = o.equipment_code, Type = o.equipment_name });
                    .Select(o => new { Value = o.id, Text = o.equipment_code + " - " + o.equipment_name, Type = o.equipment_name, Search = o.equipment_code.ToLower() + " - " + o.equipment_name.ToLower()
                    + o.equipment_code.ToUpper() + " - " + o.equipment_name.ToUpper() });
                var trucks = dbContext.truck
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    //.Select(o => new { Value = o.id, Text = o.vehicle_id, Type = o.vehicle_name });
                    .Select(o => new { Value = o.id, Text = o.vehicle_id + " - " + o.vehicle_name, Type = o.vehicle_name, Search = o.vehicle_id.ToLower() + " - " + o.vehicle_name.ToLower() + o.vehicle_id.ToUpper() + " - " + o.vehicle_name.ToUpper() });

                var lookup = equipments.Union(trucks).OrderBy(o => o.Text);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("VendorIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> VendorIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.contractor
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.is_truck_owner == true)
                    .Select(o => new { Value = o.id, Text = o.business_partner_name, Search = o.business_partner_name.ToLower() + o.business_partner_name.ToUpper() });

                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }


        [HttpGet("DataTruck")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<StandardResult> DataTruck(string Id, DataSourceLoadOptions loadOptions)
        {
            var result = new StandardResult();
            try
            {
                var record = await dbContext.vw_truck
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

        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {
                var record = await dbContext.vw_truck
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
        public async Task<IActionResult> SaveData([FromBody] truck Record)
        {
            try
            {
                var record = dbContext.truck
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
                        return Ok(record);
					}
					else
					{
						return BadRequest("User is not authorized.");
					}
                }
                else if (await mcsContext.CanCreate(dbContext, nameof(truck),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new truck();
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

                    dbContext.truck.Add(record);
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

        [HttpDelete("DeleteById/{Id}")]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Debug($"string Id = {Id}");

            try
            {
                var record = dbContext.truck
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.truck.Remove(record);
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

                    var capacity_uom_id = "";
                    var uom = dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.uom_symbol.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                    if (uom != null) capacity_uom_id = uom.id.ToString();

                    var vendor_id = "";
                    var vendor = dbContext.contractor
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.business_partner_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower()).FirstOrDefault();
                    if (vendor != null) vendor_id = vendor.id.ToString();

                    var business_unit_id = "";
                    var BU = dbContext.business_unit.Where(o => o.business_unit_code == PublicFunctions.IsNullCell(row.GetCell(14)).ToUpper()).FirstOrDefault();
                    if (BU != null) business_unit_id = BU.id.ToString();

                    var record = dbContext.truck
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
							&& o.vehicle_id == PublicFunctions.IsNullCell(row.GetCell(1)))
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.vehicle_name = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.capacity = PublicFunctions.Pecahan(row.GetCell(2));
                        record.capacity_uom_id = capacity_uom_id;
                        record.vendor_id = vendor_id;
                        record.vehicle_make = PublicFunctions.IsNullCell(row.GetCell(5));
                        record.vehicle_model = PublicFunctions.IsNullCell(row.GetCell(6));
                        record.vehicle_model_year = PublicFunctions.Bulat(row.GetCell(7));
                        record.vehicle_manufactured_year = PublicFunctions.Bulat(row.GetCell(8));
                        record.typical_tonnage = PublicFunctions.Desimal(row.GetCell(9));
                        record.typical_volume = PublicFunctions.Desimal(row.GetCell(10));
                        record.tare = PublicFunctions.Desimal(row.GetCell(11));
                        record.average_scale = PublicFunctions.Desimal(row.GetCell(12));
                        record.status = PublicFunctions.BenarSalah(row.GetCell(13));
                        record.business_unit_id = business_unit_id;

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new truck();
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

                        record.vehicle_name = PublicFunctions.IsNullCell(row.GetCell(0));
                        record.vehicle_id = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.capacity = PublicFunctions.Pecahan(row.GetCell(2));
                        record.capacity_uom_id = capacity_uom_id;
                        record.vendor_id = vendor_id;
                        record.vehicle_make = PublicFunctions.IsNullCell(row.GetCell(5));
                        record.vehicle_model = PublicFunctions.IsNullCell(row.GetCell(6));
                        record.vehicle_model_year = PublicFunctions.Bulat(row.GetCell(7));
                        record.vehicle_manufactured_year = PublicFunctions.Bulat(row.GetCell(8));
                        record.typical_tonnage = PublicFunctions.Desimal(row.GetCell(9));
                        record.typical_volume = PublicFunctions.Desimal(row.GetCell(10));
                        record.tare = PublicFunctions.Desimal(row.GetCell(11));
                        record.average_scale = PublicFunctions.Desimal(row.GetCell(12));
                        record.status = PublicFunctions.BenarSalah(row.GetCell(13));
                        record.business_unit_id = business_unit_id;

                        dbContext.truck.Add(record);
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

                    var truck_id = "";
                    var truck = dbContext.truck
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.vehicle_id.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()).FirstOrDefault();
                    if (truck != null) truck_id = truck.id.ToString();

                    var accounting_period_id = "";
                    var accounting_period = dbContext.accounting_period
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.accounting_period_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                    if (accounting_period != null) accounting_period_id = accounting_period.id.ToString();

                    var currency_id = "";
                    var currency = dbContext.currency
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.currency_symbol == PublicFunctions.IsNullCell(row.GetCell(4))).FirstOrDefault();
                    if (currency != null) currency_id = currency.id.ToString();

                    var record = dbContext.truck_cost_rate
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.truck_id == truck_id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.accounting_period_id = accounting_period_id;
                        record.currency_id = currency_id;
                        record.hourly_rate = PublicFunctions.Desimal(row.GetCell(5));
                        record.trip_rate = PublicFunctions.Desimal(row.GetCell(6));
                        record.code = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.name = PublicFunctions.IsNullCell(row.GetCell(2));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new truck_cost_rate();
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

                        record.truck_id = truck_id;
                        record.accounting_period_id = accounting_period_id;
                        record.currency_id = currency_id;
                        record.hourly_rate = PublicFunctions.Desimal(row.GetCell(5));
                        record.trip_rate = PublicFunctions.Desimal(row.GetCell(6));
                        record.code = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.name = PublicFunctions.IsNullCell(row.GetCell(2));

                        dbContext.truck_cost_rate.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 2, Line " + (i+1) + ": " + Environment.NewLine;
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
                HttpContext.Session.SetString("filename", "Truck");
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

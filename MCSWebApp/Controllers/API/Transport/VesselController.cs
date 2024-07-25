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
    public class VesselController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly mcsContext dbLoop;

        public VesselController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
            dbLoop = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            var record = dbContext.vessel
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .FirstOrDefault();
            if (record != null)
            {
                if ((await mcsContext.CanRead(dbContext, record.id, CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin) == false)
                    return BadRequest("User is not authorized.");
            }

            return await DataSourceLoader.LoadAsync(dbContext.vessel
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vessel.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(vessel),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new vessel();
                    JsonConvert.PopulateObject(values, record);

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
					//record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                    
                    record.stock_location_name = record.vehicle_name;
                    dbContext.vessel.Add(record);
                    await dbContext.SaveChangesAsync();

                    //20220812 Loop Copy Data
                    var org = dbLoop.organization
                        .Where(o => o.id != CurrentUserContext.OrganizationId
                        && o.id != "00000000000000000000000000000000");
                    foreach (organization _org in org)
                    {
                        record.id = Guid.NewGuid().ToString("N");
                        record.organization_id = _org.organization_id;
                        dbContext.vessel.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                    //20220812 Loop Copy Data

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
                var record = dbContext.vessel
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    var e = new entity();
                    e.InjectFrom(record);

                    JsonConvert.PopulateObject(values, record);
                    var is_active = record.is_active;
                   // record.InjectFrom(e);
                    record.modified_by = CurrentUserContext.AppUserId;
                    record.modified_on = DateTime.Now;
                    record.is_active = is_active;

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
            #region Validation

            dynamic cekTrans = dbContext.despatch_order.Where(o => o.vessel_id == key).Count();
            if (cekTrans > 0) return BadRequest("Can't be deleted, already been used in Shipping Order.");

            cekTrans = dbContext.blending_plan.Where(o => o.destination_location_id == key).Count();
            if (cekTrans > 0) return BadRequest("Can't be deleted, already been used in Blending Plan.");

            cekTrans = dbContext.shipment_plan.Where(o => o.transport_id == key).Count();
            if (cekTrans > 0) return BadRequest("Can't be deleted, already been used in Shipment Plan.");

            cekTrans = dbContext.shipping_transaction.Where(o => o.ship_location_id == key).Count();
            if (cekTrans > 0) return BadRequest("Can't be deleted, already been used in Shipping.");

            cekTrans = dbContext.quality_sampling.Where(o => o.stock_location_id == key).Count();
            if (cekTrans > 0) return BadRequest("Can't be deleted, already been used in Quality Sampling.");

            #endregion

            try
            {
                var record = dbContext.vessel
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.vessel.Remove(record);
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

        [HttpGet("VendorIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> VendorIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.contractor
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.is_vessel_owner == true )
                    .Select(o => new { Value = o.id, Text = o.business_partner_name });

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
                var record = await dbContext.vw_vessel
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
        public async Task<IActionResult> SaveData([FromBody] vessel Record)
        {
            try
            {
                var record = dbContext.vessel
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
                else if (await mcsContext.CanCreate(dbContext, nameof(vessel),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new vessel();
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
					//record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    dbContext.vessel.Add(record);
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
                var record = dbContext.vessel
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.vessel.Remove(record);
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

                    var uom_id = "";
                    var uom = dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.uom_symbol.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                    if (uom != null) uom_id = uom.id.ToString();

                    /*var vendor_id = "";
                    var vendor = dbContext.business_partner
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.business_partner_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower()).FirstOrDefault();
                    if (vendor != null) vendor_id = vendor.id.ToString();*/

                    var business_area_id = "";
                    var business_area = dbContext.vw_business_area_structure
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.business_area_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()).FirstOrDefault();
                    if (business_area != null) business_area_id = business_area.id.ToString();

                    var business_unit_id = "";
                    var BU = dbContext.business_unit.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.business_unit_code == PublicFunctions.IsNullCell(row.GetCell(9)).ToUpper()).FirstOrDefault();
                    if (BU != null) business_unit_id = BU.id.ToString();

                    var record = dbContext.vessel
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                            && o.vehicle_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower())
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.business_area_id = business_area_id;
                        record.vehicle_name = PublicFunctions.IsNullCell(row.GetCell(1)).Trim();
                        record.capacity = PublicFunctions.Pecahan(row.GetCell(2));
                        record.capacity_uom_id = uom_id;
                        record.imo_number = PublicFunctions.IsNullCell(row.GetCell(4));
                        record.type = PublicFunctions.IsNullCell(row.GetCell(5));
                        record.is_geared = PublicFunctions.BenarSalah(row.GetCell(7));
                        record.is_active = PublicFunctions.BenarSalah(row.GetCell(8));
                        record.business_unit_id = business_unit_id;
                        record.stock_location_name = "";

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new vessel();
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

                        record.business_area_id = business_area_id;
                        record.vehicle_name = PublicFunctions.IsNullCell(row.GetCell(1)).Trim();
                        record.capacity = PublicFunctions.Pecahan(row.GetCell(2));
                        record.capacity_uom_id = uom_id;
                        record.imo_number = PublicFunctions.IsNullCell(row.GetCell(4));
                        record.type = PublicFunctions.IsNullCell(row.GetCell(5));
                        record.is_geared = PublicFunctions.BenarSalah(row.GetCell(7));
                        record.is_active = PublicFunctions.BenarSalah(row.GetCell(8));
                        record.business_unit_id = business_unit_id;
                        record.stock_location_name = "";

                        dbContext.vessel.Add(record);
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
                HttpContext.Session.SetString("filename", "Vessel");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpGet("VesselIdLookup")]
        public async Task<object> VesselIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vessel
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name, search = o.vehicle_name.ToLower() + o.vehicle_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("VesselBargeIdLookup")]
        public async Task<object> VesselBargeIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var barge = dbContext.barge
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name, search = o.vehicle_name.ToLower() + o.vehicle_name.ToUpper() });
                var vessel = dbContext.vessel
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.vehicle_name, search = o.vehicle_name.ToLower() + o.vehicle_name.ToUpper() });
                var lookup = vessel.Union(barge).OrderBy(o => o.Text);

                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}

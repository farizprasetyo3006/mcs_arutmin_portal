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
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Hosting;
using BusinessLogic;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using Common;
using BusinessLogic.Entity;
using NPOI.SS.Formula.Functions;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;

namespace MCSWebApp.Controllers.API.Location
{
    [Route("api/Location/[controller]")]
    [ApiController]
    public class MineLocationController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public MineLocationController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption) : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_mine_location_breakdown_structure
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataGridByLatest")]
        public async Task<object> DataGridByLatest(DataSourceLoadOptions loadOptions, string latestUpdate)
        {
            DateTime modiefiedOn = default;
            if (!string.IsNullOrEmpty(latestUpdate))
            {
                DateTime.TryParse(latestUpdate, out modiefiedOn);
            }

            return await DataSourceLoader.LoadAsync(dbContext.vw_mine_location
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.modified_on > modiefiedOn || (o.modified_on == null && o.created_on > DateTime.Parse(latestUpdate))),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.mine_location.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpPost("InsertData")]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(mine_location),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new mine_location();
                    JsonConvert.PopulateObject(values, record);

                    //var cekdata = dbContext.mine_location
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    //        && (o.mine_location_code.ToLower().Trim() == record.mine_location_code.ToLower().Trim()
                    //        || o.stock_location_name.ToLower().Trim() == record.stock_location_name.ToLower().Trim()))
                    //    .FirstOrDefault();
                    var cekdata = dbContext.mine_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.business_area_id == record.business_area_id
                            && (o.mine_location_code.ToLower().Trim() == record.mine_location_code.ToLower().Trim()
                            || o.stock_location_name.ToLower().Trim() == record.stock_location_name.ToLower().Trim()))
                        .FirstOrDefault();
                    if (cekdata != null) return BadRequest("Duplicate data. Already have the same Business Area, Mine Location Code or/and Mine Location Name in database.");

                    if (record.opening_date > record.closing_date)
                        return BadRequest("Opening Date can not exceed Closing Date.");

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
                    record.mine_location_code = record.mine_location_code.ToUpper();
                    record.ready_to_get = true;

                    dbContext.mine_location.Add(record);
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
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.mine_location
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

                        //var cekdata = dbContext.mine_location
                        //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        //        && (o.mine_location_code.ToLower().Trim() == record.mine_location_code.ToLower().Trim()
                        //        || o.stock_location_name.ToLower().Trim() == record.stock_location_name.ToLower().Trim())
                        //        && o.id != record.id)
                        //    .FirstOrDefault();
                        var cekdata = dbContext.mine_location
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.business_area_id == record.business_area_id
                                && (o.mine_location_code.ToLower().Trim() == record.mine_location_code.ToLower().Trim()
                                || o.stock_location_name.ToLower().Trim() == record.stock_location_name.ToLower().Trim())
                                && o.id != record.id)
                            .FirstOrDefault();
                        if (cekdata != null) return BadRequest("Duplicate data. Already have the same Business Area, Mine Location Code or/and Mine Location Name in database.");

                        if (record.opening_date > record.closing_date)
                            return BadRequest("Opening Date can not exceed Closing Date.");
                        record.ready_to_get = true;
                        //record.InjectFrom(e);
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
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var timesheet_detail = dbContext.timesheet_detail.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.source_id == key).FirstOrDefault();
                if (timesheet_detail != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");
                var coalMined = dbContext.production_transaction.Where(o => o.source_location_id == key).FirstOrDefault();
                if (coalMined != null) { return BadRequest("Can not be Deleted. This Data Already Have a Transaction in Coal Mined"); }
                var wasteRemoval = dbContext.waste_removal.Where(o => o.source_location_id == key).FirstOrDefault();
                if (wasteRemoval != null) { return BadRequest("Can not be Deleted. This Data Already Have a Transaction in Waste Removal"); }
                var record = dbContext.mine_location
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                var parts = record.mine_location_code.Split('_');
                var ltp = dbContext.mine_plan_ltp.Where(o=>o.mine_code == parts[0] && o.submine_code == parts[1] && o.pit_code == parts[2] &&
                          o.subpit_code == parts[3] && o.contractor_code == parts[4] && o.seam_code == parts[5] && o.blok_code == parts[6]);
                if (ltp != null) { return BadRequest("Can not be Deleted. This Data is from Mine Plan LTP"); }
                var stockpileLocation = dbContext.stockpile_location.Where(o => o.stockpile_location_code == record.mine_location_code && o.is_virtual == true).FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        dbContext.mine_location.Remove(record);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        return BadRequest("User is not authorized.");
                    }
                }
                if (stockpileLocation != null) { dbContext.stockpile_location.Remove(stockpileLocation); await dbContext.SaveChangesAsync(); }
                return Ok();
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] mine_location Record)
        {
            try
            {
                var record = dbContext.mine_location
                    .Where(o => o.id == Record.id
                        && o.organization_id == CurrentUserContext.OrganizationId)
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
                else if (await mcsContext.CanCreate(dbContext, nameof(mine_location),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new mine_location();
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

                    dbContext.mine_location.Add(record);
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
            logger.Trace($"string Id = {Id}");

            try
            {
                var record = dbContext.mine_location
                    .Where(o => o.id == Id
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.mine_location.Remove(record);
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

        [HttpGet("MineLocationIdLookup")]
        public async Task<object> MineLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.mine_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
					.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.stock_location_name, Search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ContractorIdLookup")]
        public async Task<object> ContractorIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.contractor
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.is_truck_owner == true)
					//.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.business_partner_name, Search = o.business_partner_name.ToLower() + o.business_partner_name.ToUpper() });

                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("MineLocationCodeLookup")]
        public async Task<object> MineLocationCodeLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.mine_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
					.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.mine_location_code, Search = o.mine_location_code.ToLower() + o.mine_location_code.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
				logger.Error(ex.InnerException ?? ex);
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("FetchToQualitySampling")]    //**** tanpa trigger trf_sales_invoice_approval_sync di table sales_invoice_approval
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> FetchToQualitySampling([FromForm] string key, [FromForm] string values)
        {
            dynamic result;
            bool? isApproved = null;


            var modelGeology = dbContext.model_geology
                            .Where(o => o.id == key)
                            .FirstOrDefault();
            var mineLocation = await dbContext.mine_location.Where(o => o.id == modelGeology.mine_location_id).FirstOrDefaultAsync();
            var samplingTempalte = await dbContext.sampling_template.Where(o=>o.sampling_template_name == "GEOLOGY").FirstOrDefaultAsync();
            var samplingType = await dbContext.sampling_type.Where(o=>o.sampling_type_code == "GEO").FirstOrDefaultAsync();
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    var record = new quality_sampling();
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

                    record.stock_location_id = mineLocation.id;
                    record.sampling_template_id = samplingTempalte.id;
                    record.sampling_type_id = samplingType.id;
                    record.sampling_datetime = (DateTime)modelGeology.created_on;
                    record.business_unit_id = samplingType.business_unit_id;

                    dbContext.quality_sampling.Add(record);
                    await dbContext.SaveChangesAsync();
                    string[] analyteIdList = { "e410ab5fc90544168169dbee2fc08504", "fd7266e329be4311b2a05bf9776d7b75", "98c6162bfa084fcd8378f158ce8b0388", "aea5c4a6a37c4680868c8ce4c815b6b5", "a233c12c7131409c9eb5e320ebad0157",
                                                "7b1153bbb4d14461ae67c4e55fd00944","ad0f6aafe6a14bdbb47522f8d0f15bea","477c28cf7b8248d686eb0a6731210f15","63903fd614e74b7ab71a202fd2120c47","e585181855cd43eb86128a5cddbebea3",
                                                "992744db8de342368047d6c5d8799ad3"};

                    string[] uomId = { "90f4b279a62e4efdb33b8e2d6c292e7d", "90f4b279a62e4efdb33b8e2d6c292e7d", "90f4b279a62e4efdb33b8e2d6c292e7d", "90f4b279a62e4efdb33b8e2d6c292e7d", "90f4b279a62e4efdb33b8e2d6c292e7d"
                                        , "90f4b279a62e4efdb33b8e2d6c292e7d", "457ec80c9b0c4dae87ab149bdbb3266b", "457ec80c9b0c4dae87ab149bdbb3266b", "79b3cd26c1234d44b45534fdf503baa3", "79b3cd26c1234d44b45534fdf503baa3",
                                        "80d855cb81d844108939785c9356a9c2"};

                    decimal?[] value = { modelGeology.tm, modelGeology.ts, modelGeology.ash, modelGeology.im,modelGeology.vm,modelGeology.fc,modelGeology.gcv_ar, modelGeology.gcv_adb,modelGeology.rd,modelGeology.rdi,
                                            modelGeology.hgi};
                    var count = 0;
                    foreach (var analyte in analyteIdList)
                    {

                        var recordDetail = new quality_sampling_analyte();
                        recordDetail.id = Guid.NewGuid().ToString("N");
                        recordDetail.created_by = CurrentUserContext.AppUserId;
                        recordDetail.created_on = DateTime.Now;
                        recordDetail.modified_by = null;
                        recordDetail.modified_on = null;
                        recordDetail.is_active = true;
                        recordDetail.is_default = null;
                        recordDetail.is_locked = null;
                        recordDetail.entity_id = null;
                        recordDetail.owner_id = CurrentUserContext.AppUserId;
                        recordDetail.organization_id = CurrentUserContext.OrganizationId;

                        recordDetail.uom_id = uomId[count];
                        recordDetail.analyte_value = value[count];
                        count++;
                        recordDetail.analyte_id = analyte;
                        recordDetail.quality_sampling_id = record.id;
                        dbContext.quality_sampling_analyte.Add(recordDetail);
                        await dbContext.SaveChangesAsync();

                    }


                    result = record;
                    await tx.CommitAsync();
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.InnerException ?? ex);
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        [HttpGet("ParentLocationIdLookup")]
        public async Task<object> ParentLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.mine_location
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
					.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.stock_location_name, Search = o.stock_location_name.ToLower() + o.stock_location_name.ToUpper() });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
			catch (Exception ex)
			{
				logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
        }

        [HttpGet("MonthIdLookup")]
        public async Task<object> MonthIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.months
                    .Select(o => new { Value = o.id, Text = o.month_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        [HttpPost("UploadExposedCoal")]
        public async Task<object> UploadExposedCoal([FromBody] dynamic FileDocument)
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

            var uom_id = "";
            var uom = dbContext.uom.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                && o.uom_name.ToLower() == "metric ton").FirstOrDefault();
            if (uom != null) uom_id = uom.id.ToString();

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    /*var child4 = "";
                    var business_area = dbContext.vw_business_area_breakdown_structure.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.id == Value.business_area_id).FirstOrDefault();
                    if (business_area != null) child4 = business_area.child_4.ToString();*/

                    var contractor_id = "";
                    var contractor = await dbContext.contractor.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && o.business_partner_code.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower().Trim()).FirstOrDefaultAsync();
                    if (contractor != null) contractor_id = contractor.id.ToString();

                    var cellChild4 = PublicFunctions.IsNullCell(row.GetCell(0)).ToLower().Trim();
                    var records = await dbContext.vw_mine_location_breakdown_structure
                                        .Where(o => o.child_4.ToLower().Trim() == cellChild4)
                                        .Where(o=> o.contractor_id == contractor_id)
                                        .ToListAsync();
                    int totalData = records.Count();
                    var quantity = Math.Round(PublicFunctions.Desimal(row.GetCell(3)) / totalData,3); //input quantity / total data dengan banyak data

                    foreach(var item in records)
                    {
                        var checkData = dbContext.exposed_coal
                                        .Where(o => o.transaction_date == PublicFunctions.TanggalNull(row.GetCell(2)))
                                        .Where(o=>o.mine_location_id == item.id)
                                        .FirstOrDefault();
                        if (checkData != null)
                        {
                            checkData.quantity = quantity;
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            var record = new exposed_coal();
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
                            record.mine_location_id = item.id;

                            record.transaction_date = PublicFunctions.Tanggal(row.GetCell(2));
                            record.quantity = quantity;
                            record.uom_id = uom_id;

                            dbContext.exposed_coal.Add(record);
                            await dbContext.SaveChangesAsync();

                        }
                    }

                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 1, Line " + (i + 1) + " : " + Environment.NewLine;
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
                HttpContext.Session.SetString("filename", "MineLocation");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpPost("UploadDocument")]
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

                    var business_area_id = "";
                    var business_area = dbContext.business_area
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.business_area_code.Trim() == PublicFunctions.IsNullCell(row.GetCell(0)).Trim())
                        .FirstOrDefault();
                    if (business_area != null) business_area_id = business_area.id.ToString();

                    var product_id = "";
                    var product = dbContext.product
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.product_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                    if (product != null) product_id = product.id.ToString();

                    var uom_id = "";
                    var uom = dbContext.uom
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.uom_symbol.ToLower() == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower()).FirstOrDefault();
                    if (uom != null) uom_id = uom.id.ToString();

                    //var record = dbContext.mine_location
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    //        && (o.mine_location_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower()
                    //        || o.stock_location_name.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower().Trim()))
                    //    .FirstOrDefault();
                    var record = dbContext.mine_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.business_area_id == business_area_id
                            && (o.mine_location_code.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower().Trim()
                            || o.stock_location_name.ToLower().Trim() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower().Trim())
                            )
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        //record.business_area_id = business_area_id;
                        record.product_id = product_id;
                        record.uom_id = uom_id;
                        record.opening_date = PublicFunctions.Tanggal(row.GetCell(5));
                        record.closing_date = PublicFunctions.Tanggal(row.GetCell(6));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new mine_location();
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

                        record.business_area_id = business_area_id;
                        record.mine_location_code = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.stock_location_name = PublicFunctions.IsNullCell(row.GetCell(2));
                        record.product_id = product_id;
                        record.uom_id = uom_id;
                        record.opening_date = PublicFunctions.Tanggal(row.GetCell(5));
                        record.closing_date = PublicFunctions.Tanggal(row.GetCell(6));

                        dbContext.mine_location.Add(record);
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

            sheet = wb.GetSheetAt(1); //*** Exposed Coal sheet
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var mine_location_id = "";
                    var mine_location = dbContext.mine_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.mine_location_code == PublicFunctions.IsNullCell(row.GetCell(0))).FirstOrDefault();
                    if (mine_location != null) mine_location_id = mine_location.id.ToString();

                    var uom_id = "";
                    var uom = dbContext.uom.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                        o.uom_symbol == row.GetCell(3).ToString()).FirstOrDefault();
                    if (uom != null) uom_id = uom.id.ToString();

                    var record = dbContext.exposed_coal.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.mine_location_id == mine_location_id &&
                            o.transaction_date == Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(1))))
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.quantity = PublicFunctions.Desimal(row.GetCell(2));
                        record.uom_id = uom_id;

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new exposed_coal();
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

                        record.mine_location_id = mine_location_id;
                        record.transaction_date = PublicFunctions.Tanggal(row.GetCell(1));
                        record.quantity = PublicFunctions.Desimal(row.GetCell(2));
                        record.uom_id = uom_id;

                        dbContext.exposed_coal.Add(record);
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

            sheet = wb.GetSheetAt(2); //*** Ready to Get sheet
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                //var _success = false;

                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var mine_location_id = "";
                    var mine_location = dbContext.mine_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.mine_location_code == PublicFunctions.IsNullCell(row.GetCell(0))).FirstOrDefault();
                    if (mine_location != null) mine_location_id = mine_location.id.ToString();

                    var uom_id = "";
                    var uom = dbContext.uom.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                        o.uom_symbol == row.GetCell(3).ToString()).FirstOrDefault();
                    if (uom != null) uom_id = uom.id.ToString();

                    var record = dbContext.ready_to_get.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.mine_location_id == mine_location_id &&
                            o.transaction_date == Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(1))))
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.quantity = PublicFunctions.Desimal(row.GetCell(2));
                        record.uom_id = uom_id;

                        await dbContext.SaveChangesAsync();
                        //_success = true;
                    }
                    else
                    {
                        record = new ready_to_get();
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

                        record.mine_location_id = mine_location_id;
                        record.transaction_date = Convert.ToDateTime(row.GetCell(1));
                        record.quantity = PublicFunctions.Desimal(row.GetCell(2));
                        record.uom_id = uom_id;

                        dbContext.ready_to_get.Add(record);
                        await dbContext.SaveChangesAsync();
                        //_success = true;
                    }
                    /*
                    #region Update stock state
                    if (_success && record != null)
                    {
                        try
                        {
                            var _record = new DataAccess.Repository.ready_to_get();
                            _record.InjectFrom(record);
                            var connectionString = CurrentUserContext.GetDataContext().Database.ConnectionString;
                            var _jobId = _backgroundJobClient.Enqueue(() => BusinessLogic.Entity.MineLocation.UpdateStockState(connectionString, _record));
                            //_backgroundJobClient.Enqueue(() => BusinessLogic.Entity.MineLocation.UpdateStockStateAnalyte(connectionString, _record));
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                    #endregion
                    */
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

            sheet = wb.GetSheetAt(3); //*** Model Geology sheet
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var mine_location_id = "";
                    var mine_location = dbContext.mine_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.mine_location_code == PublicFunctions.IsNullCell(row.GetCell(0))).FirstOrDefault();
                    if (mine_location != null) mine_location_id = mine_location.id.ToString();

                    var year_id = "";
                    var years = dbContext.master_list
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.item_in_coding.Trim().ToUpper() == PublicFunctions.IsNullCell(row.GetCell(2)).Trim().ToUpper())
                        .FirstOrDefault();
                    if (years != null) year_id = years.id.ToString();

                    var record = dbContext.model_geology.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.mine_location_id == mine_location_id &&
                            o.month_id == PublicFunctions.IsNullCell(row.GetCell(1)) &&
                            o.year_id == year_id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.quantity = PublicFunctions.Desimal(row.GetCell(3));
                        record.tm = PublicFunctions.Desimal(row.GetCell(4));
                        record.ts = PublicFunctions.Desimal(row.GetCell(5));
                        record.ash = PublicFunctions.Desimal(row.GetCell(6));
                        record.im = PublicFunctions.Desimal(row.GetCell(7));
                        record.vm = PublicFunctions.Desimal(row.GetCell(8));
                        record.fc = PublicFunctions.Desimal(row.GetCell(9));
                        record.gcv_ar = PublicFunctions.Desimal(row.GetCell(10));
                        record.gcv_adb = PublicFunctions.Desimal(row.GetCell(11));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new model_geology();
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

                        record.mine_location_id = mine_location_id;
                        record.month_id = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.year_id = year_id;
                        record.quantity = PublicFunctions.Desimal(row.GetCell(3));
                        record.tm = PublicFunctions.Desimal(row.GetCell(4));
                        record.ts = PublicFunctions.Desimal(row.GetCell(5));
                        record.ash = PublicFunctions.Desimal(row.GetCell(6));
                        record.im = PublicFunctions.Desimal(row.GetCell(7));
                        record.vm = PublicFunctions.Desimal(row.GetCell(8));
                        record.fc = PublicFunctions.Desimal(row.GetCell(9));
                        record.gcv_ar = PublicFunctions.Desimal(row.GetCell(10));
                        record.gcv_adb = PublicFunctions.Desimal(row.GetCell(11));

                        dbContext.model_geology.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 4, Line " + (i+1) + " : " + Environment.NewLine;
                    }
                    else errormessage = ex.Message;

                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                    gagal = true;
                    break;
                }
            }

            //sheet = wb.GetSheetAt(4); //*** Quality (Channel Sampling) sheet
            //for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            //{
            //    try
            //    {
            //        IRow row = sheet.GetRow(i);
            //        if (row == null) continue;

            //        var mine_location_id = "";
            //        var mine_location = dbContext.mine_location
            //            .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
            //                        o.mine_location_code == PublicFunctions.IsNullCell(row.GetCell(0))).FirstOrDefault();
            //        if (mine_location != null) mine_location_id = mine_location.id.ToString();

            //        var record = dbContext.mine_location_quality
            //            .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
            //                o.mine_location_id == mine_location_id &&
            //                o.sampling_datetime == Convert.ToDateTime(PublicFunctions.Tanggal(row.GetCell(1))))
            //            .FirstOrDefault();
            //        if (record != null)
            //        {
            //            var e = new entity();
            //            e.InjectFrom(record);

            //            record.InjectFrom(e);
            //            record.modified_by = CurrentUserContext.AppUserId;
            //            record.modified_on = DateTime.Now;
            //            
            //            record.tm = PublicFunctions.Desimal(row.GetCell(2));
            //            record.ts = PublicFunctions.Desimal(row.GetCell(3));
            //            record.ash = PublicFunctions.Desimal(row.GetCell(4));
            //            record.im = PublicFunctions.Desimal(row.GetCell(5));
            //            record.vm = PublicFunctions.Desimal(row.GetCell(6));
            //            record.fc = PublicFunctions.Desimal(row.GetCell(7));
            //            record.gcv_ar = PublicFunctions.Desimal(row.GetCell(8));
            //            record.gcv_adb = PublicFunctions.Desimal(row.GetCell(9));

            //            await dbContext.SaveChangesAsync();
            //        }
            //        else
            //        {
            //            record = new mine_location_quality();
            //            record.id = Guid.NewGuid().ToString("N");
            //            record.created_by = CurrentUserContext.AppUserId;
            //            record.created_on = DateTime.Now;
            //            record.modified_by = null;
            //            record.modified_on = null;
            //            record.is_active = true;
            //            record.is_default = null;
            //            record.is_locked = null;
            //            record.entity_id = null;
            //            record.owner_id = CurrentUserContext.AppUserId;
            //            record.organization_id = CurrentUserContext.OrganizationId;

            //            record.mine_location_id = mine_location_id;
            //            record.sampling_datetime = PublicFunctions.Tanggal(row.GetCell(1));
            //            record.tm = PublicFunctions.Desimal(row.GetCell(2));
            //            record.ts = PublicFunctions.Desimal(row.GetCell(3));
            //            record.ash = PublicFunctions.Desimal(row.GetCell(4));
            //            record.im = PublicFunctions.Desimal(row.GetCell(5));
            //            record.vm = PublicFunctions.Desimal(row.GetCell(6));
            //            record.fc = PublicFunctions.Desimal(row.GetCell(7));
            //            record.gcv_ar = PublicFunctions.Desimal(row.GetCell(8));
            //            record.gcv_adb = PublicFunctions.Desimal(row.GetCell(9));

            //            dbContext.mine_location_quality.Add(record);
            //            await dbContext.SaveChangesAsync();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        if (ex.InnerException != null)
            //        {
            //            errormessage = ex.InnerException.Message;
            //            teks += "==>Error Sheet 5, Line " + (i+1) + " : " + Environment.NewLine;
            //        }
            //        else errormessage = ex.Message;

            //        teks += errormessage + Environment.NewLine + Environment.NewLine;
            //        gagal = true;
            //    }
            //}

            sheet = wb.GetSheetAt(4); //*** Quality Pit sheet
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var mine_location_id = "";
                    var mine_location = dbContext.mine_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.mine_location_code == PublicFunctions.IsNullCell(row.GetCell(0))).FirstOrDefault();
                    if (mine_location != null) mine_location_id = mine_location.id.ToString();

                    var year_id = "";
                    var years = dbContext.master_list
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.item_name.Trim().ToUpper() == PublicFunctions.IsNullCell(row.GetCell(2)).Trim().ToUpper())
                        .FirstOrDefault();
                    if (years != null) year_id = years.id.ToString();

                    var record = dbContext.mine_location_quality_pit
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.month_id == PublicFunctions.IsNullCell(row.GetCell(1)) &&
                            o.year_id == year_id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.quantity = PublicFunctions.Desimal(row.GetCell(3));
                        record.tm = PublicFunctions.Desimal(row.GetCell(4));
                        record.ts = PublicFunctions.Desimal(row.GetCell(5));
                        record.ash = PublicFunctions.Desimal(row.GetCell(6));
                        record.im = PublicFunctions.Desimal(row.GetCell(7));
                        record.vm = PublicFunctions.Desimal(row.GetCell(8));
                        record.fc = PublicFunctions.Desimal(row.GetCell(9));
                        record.gcv_ar = PublicFunctions.Desimal(row.GetCell(10));
                        record.gcv_adb = PublicFunctions.Desimal(row.GetCell(11));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new mine_location_quality_pit();
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

                        record.mine_location_id = mine_location_id;
                        record.month_id = PublicFunctions.IsNullCell(row.GetCell(1));
                        record.year_id = year_id;
                        record.quantity = PublicFunctions.Desimal(row.GetCell(3));
                        record.tm = PublicFunctions.Desimal(row.GetCell(4));
                        record.ts = PublicFunctions.Desimal(row.GetCell(5));
                        record.ash = PublicFunctions.Desimal(row.GetCell(6));
                        record.im = PublicFunctions.Desimal(row.GetCell(7));
                        record.vm = PublicFunctions.Desimal(row.GetCell(8));
                        record.fc = PublicFunctions.Desimal(row.GetCell(9));
                        record.gcv_ar = PublicFunctions.Desimal(row.GetCell(10));
                        record.gcv_adb = PublicFunctions.Desimal(row.GetCell(11));

                        dbContext.mine_location_quality_pit.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 5, Line " + (i+1) + " : " + Environment.NewLine;
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
                HttpContext.Session.SetString("filename", "MineLocation");
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

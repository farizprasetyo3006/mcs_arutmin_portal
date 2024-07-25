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
using BusinessLogic.Formula;
using Common;
using System.Diagnostics;
using MCSWebApp.Models;

namespace MCSWebApp.Controllers.API.Mining
{
    [Route("api/Timesheet/[controller]")]
    [ApiController]
    public class TimesheetController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public TimesheetController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_timesheet
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            logger.Debug($"tanggal1 = {tanggal1}");
            logger.Debug($"tanggal2 = {tanggal2}");

            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_timesheet
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            logger.Debug($"dt1 = {dt1}");
            logger.Debug($"dt2 = {dt2}");

            return await DataSourceLoader.LoadAsync(dbContext.vw_timesheet
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.timesheet_date >= dt1 && o.timesheet_date <= dt2),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_timesheet.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpGet("EGIDataDetail")]
        public async Task<object> EGIDataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            var equipmentRecord = dbContext.vw_equipment.Where(o => o.id == Id
                     && (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin));
            var firstEquipment = equipmentRecord.FirstOrDefault();

            if (firstEquipment != null)
            {
                return await DataSourceLoader.LoadAsync(equipmentRecord, loadOptions);
            }
            else
            {
                var truckRecord = dbContext.vw_truck.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId);
                return await DataSourceLoader.LoadAsync(truckRecord, loadOptions);
            }
        }

        [HttpPost("InsertData")]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            using var tx = await dbContext.Database.BeginTransactionAsync();

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(timesheet),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var record = new timesheet();
                    JsonConvert.PopulateObject(values, record);

                    if (record.timesheet_date > DateTime.Now)
                        return BadRequest("The Timesheet Date should not exceed today's date");

                    var recordExist = dbContext.timesheet
                            .Where(o => o.cn_unit_id == record.cn_unit_id
                                    && o.timesheet_date == record.timesheet_date
                                    && o.operator_id == record.operator_id
                                    && o.shift_id == record.shift_id
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                            .FirstOrDefault();

                    if (recordExist != null)
                    {
                        return BadRequest("Already have same timesheet.");
                    }

                    record.id = Guid.NewGuid().ToString("N");
                    #region Base Record
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
                    #endregion

                    dbContext.timesheet.Add(record);
                    await dbContext.SaveChangesAsync();

                    if (await createTimesheetDetail(record.id, record.shift_id))
                    {
                        await tx.CommitAsync();
                        return Ok(record);
                    }
                    else
                    {
                        return BadRequest("Failed create timesheet details.");
                    }
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
                var record = dbContext.timesheet
                    .Where(o => o.id == key
                        && o.organization_id == CurrentUserContext.OrganizationId)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var shiftId = record.shift_id;
                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);

                        if (record.timesheet_date > DateTime.Now)
                            return BadRequest("The Timesheet Date should not exceed today's date");

                        using var tx = await dbContext.Database.BeginTransactionAsync();
                        //record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        await dbContext.SaveChangesAsync();
                        if (shiftId != record.shift_id)
                        {
                            //delete all timesheet detail if different shift
                            var tsDetails = dbContext.timesheet_detail.Where(x => x.timesheet_id == record.id).ToList();
                            if (tsDetails.Count() > 0)
                            {
                                foreach (var item in tsDetails)
                                {
                                    var recordDetailEVent = dbContext.timesheet_detail_event
                                        .Where(o => o.timesheet_detail_id == item.id).ToList();
                                    if (recordDetailEVent.Count() > 0)
                                    {
                                        dbContext.timesheet_detail_event.RemoveRange(recordDetailEVent);
                                        await dbContext.SaveChangesAsync();
                                    }

                                    var recordDetailProblem = dbContext.timesheet_detail_productivity_problem
                                        .Where(o => o.timesheet_detail_id == item.id).ToList();
                                    if (recordDetailEVent.Count() > 0)
                                    {
                                        dbContext.timesheet_detail_productivity_problem.RemoveRange(recordDetailProblem);
                                        await dbContext.SaveChangesAsync();
                                    }
                                }
                                dbContext.timesheet_detail.RemoveRange(tsDetails);
                                await dbContext.SaveChangesAsync();
                            }

                            //create new timesheet detail
                            if (await createTimesheetDetail(record.id, record.shift_id))
                            {
                                await tx.CommitAsync();
                                return Ok(record);
                            }
                            else
                            {
                                return BadRequest("Failed to create timesheet details.");
                            }

                        }
                        await tx.CommitAsync();
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
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    var record = dbContext.timesheet
                        .Where(o => o.id == key
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            var recordDetail = dbContext.timesheet_detail
                                .Where(o => o.timesheet_id == record.id).ToList();
                            if (recordDetail.Count() > 0)
                            {
                                foreach (var item in recordDetail)
                                {
                                    var recordDetailEVent = dbContext.timesheet_detail_event
                                        .Where(o => o.timesheet_detail_id == item.id).ToList();
                                    if (recordDetail.Count() > 0)
                                    {
                                        dbContext.timesheet_detail_event.RemoveRange(recordDetailEVent);
                                        await dbContext.SaveChangesAsync();

                                    }

                                    var recordDetailProblem = dbContext.timesheet_detail_productivity_problem
                                        .Where(o => o.timesheet_detail_id == item.id).ToList();
                                    if (recordDetail.Count() > 0)
                                    {
                                        dbContext.timesheet_detail_productivity_problem.RemoveRange(recordDetailProblem);
                                        await dbContext.SaveChangesAsync();

                                    }
                                }
                                dbContext.timesheet_detail.RemoveRange(recordDetail);
                                await dbContext.SaveChangesAsync();

                            }

                            dbContext.timesheet.Remove(record);
                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
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

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] timesheet Record)
        {
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = await dbContext.timesheet
                        .Where(o => o.id == Record.id)
                        .FirstOrDefaultAsync();
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
                    else if (await mcsContext.CanCreate(dbContext, nameof(timesheet),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new timesheet();
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

                        dbContext.timesheet.Add(record);
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

        [HttpGet("EquipmentOrTruckIdLookupByLatest")]
        public async Task<object> EquipmentOrTruckIdLookupByLatest(DataSourceLoadOptions loadOptions, string latestUpdate)
        {
            try
            {
                DateTime modiefiedOn = default;
                if (!string.IsNullOrEmpty(latestUpdate))
                {
                    DateTime.TryParse(latestUpdate, out modiefiedOn);
                }

                var equipments = dbContext.equipment
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.modified_on > modiefiedOn || (o.modified_on == null && o.created_on > DateTime.Parse(latestUpdate)))
                                .Select(o => new { Value = o.id, Text = o.equipment_code, Type = o.equipment_name });
                var trucks = dbContext.truck
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.modified_on > modiefiedOn || (o.modified_on == null && o.created_on > DateTime.Parse(latestUpdate)))
                                .Select(o => new { Value = o.id, Text = o.vehicle_id, Type = o.vehicle_name });

                var lookup = equipments.Union(trucks);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("EquipmentOrTruckCapacity")]
        public async Task<object> EquipmentOrTruckCapacity(string id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var equipments = dbContext.equipment
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == id)
                                .Select(o => new { Value = o.id, capacity = (double)o.capacity });
                var trucks = dbContext.truck
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == id)
                                .Select(o => new { Value = o.id, capacity = (double)o.capacity });

                var lookup = equipments.Union(trucks);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProductOrWasteIdLookup")]
        public async Task<object> ProductOrWasteIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var productRecord = dbContext.product
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { Value = o.id, Text = o.product_name });
                var wasteRecord = dbContext.waste
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { Value = o.id, Text = o.waste_name });
                var lookup = productRecord.Union(wasteRecord);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ProductOrWasteIdLookupByLatest")]
        public async Task<object> ProductOrWasteIdLookupByLatest(DataSourceLoadOptions loadOptions, string latestUpdate)
        {
            try
            {
                DateTime modiefiedOn = default;
                if (!string.IsNullOrEmpty(latestUpdate))
                {
                    DateTime.TryParse(latestUpdate, out modiefiedOn);
                }

                var productRecord = dbContext.product
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.modified_on > modiefiedOn || (o.modified_on == null && o.created_on > DateTime.Parse(latestUpdate)))
                                .Select(o => new { Value = o.id, Text = o.product_name });
                var wasteRecord = dbContext.waste
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.modified_on > modiefiedOn || (o.modified_on == null && o.created_on > DateTime.Parse(latestUpdate)))
                                .Select(o => new { Value = o.id, Text = o.waste_name });
                var lookup = productRecord.Union(wasteRecord);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("StockpileOrWasteLocationIdLookup")]
        public async Task<object> StockpileOrWasteLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var stockpileRecord = dbContext.stockpile_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { Value = o.id, Text = o.stock_location_name });
                var wasteLocationRecord = dbContext.waste_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { Value = o.id, Text = o.stock_location_name });
                var lookup = stockpileRecord.Union(wasteLocationRecord);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("StockpileOrWasteLocationOrPortLocationIdLookup")]
        public async Task<object> StockpileOrWasteLocationOrPortLocationIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var stockpileRecord = dbContext.stockpile_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { Value = o.id, Text = o.stock_location_name });
                var wasteLocationRecord = dbContext.waste_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { Value = o.id, Text = o.stock_location_name });
                var portLocationRecord = dbContext.port_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin)
                                //.Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                                .Select(o => new { Value = o.id, Text = o.port_location_code });
                var lookup = stockpileRecord.Union(wasteLocationRecord).Union(portLocationRecord);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("StockpileOrWasteLocationOrPortLocationIdLookupByLatest")]
        public async Task<object> StockpileOrWasteLocationOrPortLocationIdLookupByLatest(DataSourceLoadOptions loadOptions, string latestUpdate)
        {
            try
            {
                DateTime modiefiedOn = default;
                if (!string.IsNullOrEmpty(latestUpdate))
                {
                    DateTime.TryParse(latestUpdate, out modiefiedOn);
                }

                var stockpileRecord = dbContext.stockpile_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.modified_on > modiefiedOn || (o.modified_on == null && o.created_on > DateTime.Parse(latestUpdate)))
                                .Select(o => new { Value = o.id, Text = o.stock_location_name });
                var wasteLocationRecord = dbContext.waste_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.modified_on > modiefiedOn || (o.modified_on == null && o.created_on > DateTime.Parse(latestUpdate)))
                                .Select(o => new { Value = o.id, Text = o.stock_location_name });
                var portLocationRecord = dbContext.port_location
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.modified_on > modiefiedOn || (o.modified_on == null && o.created_on > DateTime.Parse(latestUpdate)))
                                .Select(o => new { Value = o.id, Text = o.port_location_code });
                var lookup = stockpileRecord.Union(wasteLocationRecord).Union(portLocationRecord);
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
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
            //TimeSpan oldDuration = default;
            shift oldShift = new shift();
            TimeSpan tsTime = default;
            string currentCNID = string.Empty;
            string oldCNID = string.Empty;
            string oldTimeSheetId = string.Empty;
            DateTime oldTimesheetDate = default;
            DateTime currentTimesheetDate = default;
            string currentShift = string.Empty;
            string oldShiftText = string.Empty;
            string currentOperatorId = string.Empty;
            string oldOperatorId = string.Empty;

            var equipments = dbContext.equipment
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Select(o => new { Id = o.id, Text = o.equipment_name });
            var trucks = dbContext.truck
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Select(o => new { Id = o.id, Text = o.vehicle_name });
            var lookup = equipments.Union(trucks);

            var shifts = dbContext.shift
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .ToList();
            var employees = dbContext.employee
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .ToList();
            var eventCategories = dbContext.event_category
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .ToList();

            var sw = new Stopwatch();
            sw.Start();

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    string cn_unit_id = null;
                    //var equiptruck = lookup.Where(o => o.Id.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
                    //Bob 20220313 Error Handling CN UNIT
                    string _unitID = PublicFunctions.IsNullCell(row.GetCell(3));
                    var equiptruck = lookup.Where(o => o.Id == _unitID)
                        .FirstOrDefault();
                    if (equiptruck != null)
                    {
                        cn_unit_id = equiptruck.Id.ToString();
                        currentCNID = cn_unit_id;
                    }
                    else
                    {
                        teks += "==>Error Line " + (i + 1) + ", Column cn_unit_id  Not Found : " + _unitID + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        break;

                    }
                    //Bob 20220313 Error Handling CN UNIT

                    DateTime timesheet_date = PublicFunctions.Tanggal(row.GetCell(0));
                    currentTimesheetDate = timesheet_date;

                    //Bob 20220313 Error Handling SHIFT
                    string shift_id = null;
                    string _shift = PublicFunctions.IsNullCell(row.GetCell(1));
                    //var shift = dbContext.shift
                    //  .Where(o => o.id.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower()).FirstOrDefault();
                    var shift = shifts.Where(o => o.id == _shift)
                        .FirstOrDefault();
                    if (shift != null)
                    {
                        shift_id = shift.id.ToString();
                        currentShift = shift_id;
                    }
                    else
                    {
                        teks += "==>Error Line " + (i + 1) + ", Column Shift_id  Not Found : " + _shift + Environment.NewLine;
                        teks += errormessage + Environment.NewLine + Environment.NewLine;
                        gagal = true;
                        break;

                    }
                    //Bob 20220313 Error Handling SHIFT

                    string operator_id = null;
                    //var _operator = dbContext.employee
                    //  .Where(o => o.id == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower()).FirstOrDefault();
                    var _operator = employees.Where(o => o.id == PublicFunctions.IsNullCell(row.GetCell(4)).ToLower())
                        .FirstOrDefault();
                    if (_operator != null)
                    {
                        operator_id = _operator.id.ToString();
                        currentOperatorId = operator_id;
                    }

                    //var record = dbContext.timesheet
                    //    .Where(o => o.cn_unit_id == cn_unit_id && o.timesheet_date == timesheet_date && o.organization_id == CurrentUserContext.OrganizationId)
                    //    .FirstOrDefault();
                    var record = new timesheet();

                    if (currentCNID != oldCNID
                        || currentTimesheetDate != oldTimesheetDate
                        || currentShift != oldShiftText
                        || currentOperatorId != oldOperatorId
                        )
                    {
                        string mine_location_id = null;
                        //var mine_location = dbContext.mine_location
                        //    .Where(o => o.mine_location_code == PublicFunctions.IsNullCell(row.GetCell(1))
                        //        && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                        //if (mine_location != null) mine_location_id = mine_location.id.ToString();

                        string supervisor_id = null;
                        //var _supervisor = dbContext.employee
                        //  .Where(o => o.id == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower()).FirstOrDefault();
                        var _supervisor = employees.Where(o => o.id == PublicFunctions.IsNullCell(row.GetCell(5)).ToLower())
                            .FirstOrDefault();
                        if (_supervisor != null) supervisor_id = _supervisor.id.ToString();

                        string uom_id = null;
                        //var uom = dbContext.uom
                        //    .Where(o => o.uom_symbol == PublicFunctions.IsNullCell(row.GetCell(7))
                        //        && o.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                        //if (uom != null) uom_id = uom.id.ToString();

                        //var productRecord = dbContext.product.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        //                .Select(o => new { Id = o.id, Text = o.product_code });
                        //var wasteRecord = dbContext.waste.Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        //                .Select(o => new { Id = o.id, Text = o.waste_code });
                        //var lookup2 = productRecord.Union(wasteRecord);

                        string material_id = null;
                        //var material = lookup2.Where(o => o.Text.ToLower() == PublicFunctions.IsNullCell(row.GetCell(9)).ToLower())
                        //    .FirstOrDefault();
                        //if (material != null) material_id = material.Id.ToString();

                        if (i != 1)
                        {
                            //create sisa waktunya kalo beda timesheet
                            TimeSpan tsTimeEnd = oldShift.start_time.Add((TimeSpan)oldShift.duration);
                            tsTime = oldShift.start_time;
                            while (tsTime < tsTimeEnd)
                            {
                                var detailExist = dbContext.timesheet_detail
                                    .Where(x => x.timesheet_id == oldTimeSheetId && x.classification == tsTime)
                                    .Select(x => x.id)
                                    .FirstOrDefault();
                                if (detailExist == null)
                                {
                                    var recordDetailEmpty = new timesheet_detail();

                                    recordDetailEmpty.id = Guid.NewGuid().ToString("N");

                                    #region Base Record
                                    recordDetailEmpty.created_by = CurrentUserContext.AppUserId;
                                    recordDetailEmpty.created_on = DateTime.Now;
                                    recordDetailEmpty.modified_by = null;
                                    recordDetailEmpty.modified_on = null;
                                    recordDetailEmpty.is_active = true;
                                    recordDetailEmpty.is_default = null;
                                    recordDetailEmpty.is_locked = null;
                                    recordDetailEmpty.entity_id = null;
                                    recordDetailEmpty.owner_id = CurrentUserContext.AppUserId;
                                    recordDetailEmpty.organization_id = CurrentUserContext.OrganizationId;
                                    recordDetailEmpty.timesheet_id = oldTimeSheetId;
                                    recordDetailEmpty.timesheet_time = TimeSpan.FromHours((double)tsTime.Hours);
                                    recordDetailEmpty.classification = TimeSpan.FromHours((double)tsTime.Hours);
                                    #endregion

                                    dbContext.timesheet_detail.Add(recordDetailEmpty);
                                    await dbContext.SaveChangesAsync();
                                }
                                tsTime = tsTime.Add(TimeSpan.FromHours(1));

                                if (tsTime.TotalDays == 1)
                                {
                                    tsTime = tsTime.Subtract(TimeSpan.FromDays(-1));
                                }
                            }

                        }

                        //record = dbContext.timesheet
                        //.Where(o => o.cn_unit_id == cn_unit_id
                        //        && o.timesheet_date == timesheet_date
                        //        && o.operator_id == operator_id
                        //        && o.shift_id == shift_id
                        //        && o.organization_id == CurrentUserContext.OrganizationId)
                        //.FirstOrDefault();
                        record = dbContext.timesheet
                            .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                                && o.cn_unit_id == cn_unit_id
                                && o.timesheet_date == timesheet_date
                                && o.operator_id == operator_id
                                && o.shift_id == shift_id
                                && o.hour_start == PublicFunctions.Desimal(row.GetCell(6))
                                && o.hour_end == PublicFunctions.Desimal(row.GetCell(7)))
                            .FirstOrDefault();

                        if (record == null)
                        {
                            record = new timesheet
                            {
                                id = Guid.NewGuid().ToString("N"),
                                created_by = CurrentUserContext.AppUserId,
                                created_on = DateTime.Now,
                                modified_by = null,
                                modified_on = null,
                                is_active = true,
                                is_default = null,
                                is_locked = null,
                                entity_id = null,
                                owner_id = CurrentUserContext.AppUserId,
                                organization_id = CurrentUserContext.OrganizationId,
                                cn_unit_id = cn_unit_id,
                                mine_location_id = mine_location_id,
                                hour_start = PublicFunctions.Desimal(row.GetCell(6)),
                                operator_id = operator_id,
                                supervisor_id = supervisor_id,
                                hour_end = PublicFunctions.Desimal(row.GetCell(7)),
                                quantity = null,
                                shift_id = shift_id,
                                uom_id = uom_id,
                                timesheet_date = timesheet_date,
                                material_id = material_id,
                                //activity_id = PublicFunctions.IsNullCell(row.GetCell(2)).ToLower()
                                activity_id = null
                            };

                            dbContext.timesheet.Add(record);
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            //kalo edit
                            record.shift_id = shift_id;
                            record.supervisor_id = supervisor_id;
                            record.operator_id = operator_id;
                            record.cn_unit_id = cn_unit_id;
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;
                            //dbContext.timesheet.Add(record);
                            await dbContext.SaveChangesAsync();

                            //delete all timesheet detail 
                            var tsDetails = dbContext.timesheet_detail.Where(x => x.timesheet_id == record.id).ToList();
                            if (tsDetails.Count() > 0)
                            {
                                foreach (var item in tsDetails)
                                {
                                    var recordDetailEVent = dbContext.timesheet_detail_event
                                        .Where(o => o.timesheet_detail_id == item.id).ToList();
                                    if (recordDetailEVent.Count() > 0)
                                    {
                                        //***** sync to ellipse
                                        var rec2 = dbContext.ell_sync
                                            .Where(o => o.id == item.id && o.module == "BREAKDOWN"
                                                && o.organization_id == CurrentUserContext.OrganizationId)
                                            .FirstOrDefault();
                                        if (rec2 != null)
                                        {
                                            rec2.delete_sync_status = false;
                                        }
                                        //******************

                                        dbContext.timesheet_detail_event.RemoveRange(recordDetailEVent);
                                        await dbContext.SaveChangesAsync();
                                    }

                                    var recordDetailProblem = dbContext.timesheet_detail_productivity_problem
                                        .Where(o => o.timesheet_detail_id == item.id).ToList();
                                    if (recordDetailEVent.Count() > 0)
                                    {
                                        dbContext.timesheet_detail_productivity_problem.RemoveRange(recordDetailProblem);
                                        await dbContext.SaveChangesAsync();
                                    }
                                }
                                dbContext.timesheet_detail.RemoveRange(tsDetails);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                        oldCNID = currentCNID;
                        oldTimeSheetId = record.id;
                        oldShiftText = record.shift_id;
                        oldTimesheetDate = currentTimesheetDate;
                        oldShift = shift;
                        oldOperatorId = currentOperatorId;
                        tsTime = shift.start_time;
                    }

                    #region detail
                    //detail
                    string pit_id = PublicFunctions.IsNullCell(row.GetCell(8)).ToLower();
                    string source_id = PublicFunctions.IsNullCell(row.GetCell(9)).ToLower();
                    string destination_id = PublicFunctions.IsNullCell(row.GetCell(10)).ToLower();
                    string loader_id = PublicFunctions.IsNullCell(row.GetCell(11)).ToLower();
                    string material_id_detail = PublicFunctions.IsNullCell(row.GetCell(12)).ToLower();
                    var distance = PublicFunctions.Desimal(row.GetCell(13));
                    var ritase = PublicFunctions.Desimal(row.GetCell(14));
                    var volume = PublicFunctions.Desimal(row.GetCell(15));
                    var rit_rehandling = PublicFunctions.Desimal(row.GetCell(16));
                    var vol_rehandling = PublicFunctions.Desimal(row.GetCell(17));
                    var productivity = PublicFunctions.Desimal(row.GetCell(18));
                    var vol_x_distance = PublicFunctions.Desimal(row.GetCell(19));
                    var vol_x_density = PublicFunctions.Desimal(row.GetCell(20));

                    var refuelling_quantity = PublicFunctions.Desimal(row.GetCell(21));
                    dynamic classification;
                    if (row.GetCell(22).StringCellValue == null || row.GetCell(22).StringCellValue == "")
                        classification = TimeSpan.Parse("0:00");
                    else
                        classification = TimeSpan.Parse(PublicFunctions.IsNullCell(row.GetCell(22)));

                    var accounting_period_id = PublicFunctions.IsNullCell(row.GetCell(25)).ToLower();
                    var recordDetail = new timesheet_detail();

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
                    recordDetail.timesheet_id = oldTimeSheetId;
                    recordDetail.timesheet_time = classification;
                    recordDetail.classification = classification;
                    recordDetail.pit_id = pit_id;
                    recordDetail.loader_id = loader_id;
                    recordDetail.source_id = source_id;
                    recordDetail.destination_id = destination_id;
                    recordDetail.material_id = material_id_detail;
                    recordDetail.distance = distance;
                    recordDetail.ritase = ritase;
                    recordDetail.volume = volume;
                    recordDetail.rit_rehandling = rit_rehandling;
                    recordDetail.vol_rehandling = vol_rehandling;
                    recordDetail.productivity = productivity;
                    recordDetail.vol_density = vol_x_density;
                    recordDetail.vol_distance = vol_x_distance;
                    recordDetail.refuelling_quantity = refuelling_quantity;
                    recordDetail.accounting_periode_id = accounting_period_id;
                    dbContext.timesheet_detail.Add(recordDetail);
                    await dbContext.SaveChangesAsync();
                    //tsTime = tsTime.Add(TimeSpan.FromHours(1));
                    #endregion

                    #region event and problem
                    //cek ada event dan problem
                    string events = PublicFunctions.IsNullCell(row.GetCell(23)).ToLower();
                    string problems = PublicFunctions.IsNullCell(row.GetCell(24)).ToLower();

                    if (!string.IsNullOrEmpty(events))
                    {
                        string[] eventArray = events.Split("#");
                        foreach (string item in eventArray)
                        {
                            string[] eventItem = item.Split("-");
                            int minute = 0;
                            if (eventItem.Count() == 2 && int.TryParse(eventItem[1], out minute))
                            {
                                var eventCategory = eventCategories //dbContext.event_category
                                    .Where(x => x.id.ToLower() == eventItem[0]
                                        && x.organization_id == CurrentUserContext.OrganizationId)
                                    .FirstOrDefault();

                                if (eventCategory != null)
                                {
                                    var recordEvent = new timesheet_detail_event();

                                    recordEvent.id = Guid.NewGuid().ToString("N");
                                    recordEvent.created_by = CurrentUserContext.AppUserId;
                                    recordEvent.created_on = DateTime.Now;
                                    recordEvent.modified_by = null;
                                    recordEvent.modified_on = null;
                                    recordEvent.is_active = true;
                                    recordEvent.is_default = null;
                                    recordEvent.is_locked = null;
                                    recordEvent.entity_id = null;
                                    recordEvent.owner_id = CurrentUserContext.AppUserId;
                                    recordEvent.organization_id = CurrentUserContext.OrganizationId;
                                    recordEvent.timesheet_detail_id = recordDetail.id;
                                    recordEvent.event_category_id = eventCategory.id;
                                    recordEvent.event_definition_category_id = eventCategory.event_definition_category_id;
                                    recordEvent.minute = minute;

                                    //***** sync to ellipse
                                    var rec2 = new ell_sync();
                                    rec2.id = recordEvent.id;
                                    rec2.organization_id = CurrentUserContext.OrganizationId;
                                    rec2.data = recordEvent.id + "|" + recordEvent.organization_id + "|" + recordEvent.event_category_id +
                                        "|" + recordEvent.event_definition_category_id +
                                        "|" + recordEvent.minute + "|" + recordEvent.timesheet_detail_id;
                                    rec2.new_sync_status = false;
                                    rec2.module = "BREAKDOWN";
                                    dbContext.ell_sync.Add(rec2);
                                    //******************

                                    dbContext.timesheet_detail_event.Add(recordEvent);
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(problems))
                    {
                        string[] problemArray = problems.Split("#");
                        foreach (string item in problemArray)
                        {
                            string[] problemItem = item.Split("-");
                            int frequency = 0;
                            if (problemItem.Count() == 2 && int.TryParse(problemItem[1], out frequency))
                            {
                                var eventCategory = eventCategories //dbContext.event_category
                                    .Where(x => x.id.ToLower() == problemItem[0]
                                        && x.organization_id == CurrentUserContext.OrganizationId)
                                    .FirstOrDefault();

                                if (eventCategory != null)
                                {
                                    var recordProblem = new timesheet_detail_productivity_problem();

                                    recordProblem.id = Guid.NewGuid().ToString("N");
                                    recordProblem.created_by = CurrentUserContext.AppUserId;
                                    recordProblem.created_on = DateTime.Now;
                                    recordProblem.modified_by = null;
                                    recordProblem.modified_on = null;
                                    recordProblem.is_active = true;
                                    recordProblem.is_default = null;
                                    recordProblem.is_locked = null;
                                    recordProblem.entity_id = null;
                                    recordProblem.owner_id = CurrentUserContext.AppUserId;
                                    recordProblem.organization_id = CurrentUserContext.OrganizationId;
                                    recordProblem.timesheet_detail_id = recordDetail.id;
                                    recordProblem.event_category_id = eventCategory.id;
                                    recordProblem.event_definition_category_id = eventCategory.event_definition_category_id;
                                    recordProblem.frequency = frequency;

                                    dbContext.timesheet_detail_productivity_problem.Add(recordProblem);
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    #endregion

                    #region sisa detail
                    //cek kalo selanjutnya keluar dr looping
                    if (i + 1 > sheet.LastRowNum)
                    {
                        TimeSpan tsTimeEnd = oldShift.start_time.Add((TimeSpan)oldShift.duration);
                        tsTime = oldShift.start_time;
                        TimeSpan tsToCompare = tsTime;
                        while (tsTime < tsTimeEnd)
                        {

                            var detailExist = dbContext.timesheet_detail
                                .Where(x => x.timesheet_id == oldTimeSheetId && x.classification == tsToCompare)
                                .Select(x => x.id)
                                .FirstOrDefault();

                            if (detailExist == null)
                            {
                                var recordDetailEmpty = new timesheet_detail();

                                recordDetailEmpty.id = Guid.NewGuid().ToString("N");

                                #region Base Record
                                recordDetailEmpty.created_by = CurrentUserContext.AppUserId;
                                recordDetailEmpty.created_on = DateTime.Now;
                                recordDetailEmpty.modified_by = null;
                                recordDetailEmpty.modified_on = null;
                                recordDetailEmpty.is_active = true;
                                recordDetailEmpty.is_default = null;
                                recordDetailEmpty.is_locked = null;
                                recordDetailEmpty.entity_id = null;
                                recordDetailEmpty.owner_id = CurrentUserContext.AppUserId;
                                recordDetailEmpty.organization_id = CurrentUserContext.OrganizationId;
                                recordDetailEmpty.timesheet_id = oldTimeSheetId;
                                recordDetailEmpty.timesheet_time = TimeSpan.FromHours((double)tsTime.Hours);
                                recordDetailEmpty.classification = TimeSpan.FromHours((double)tsTime.Hours);
                                #endregion

                                dbContext.timesheet_detail.Add(recordDetailEmpty);
                                await dbContext.SaveChangesAsync();

                            }
                            tsTime = tsTime.Add(TimeSpan.FromHours(1));

                            if (tsTime.TotalDays >= 1)
                            {
                                tsToCompare = tsTime.Subtract(TimeSpan.FromDays(1));
                            }
                            else
                            {
                                tsToCompare = tsTime;
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        //teks += "==>Error Line " + (i+1) + " : " + Environment.NewLine;
                        teks += "==>Error Line " + (i + 1) + Environment.NewLine;
                    }
                    else errormessage = ex.Message;

                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                    gagal = true;
                    break;
                }
            }

            sw.Stop();
            logger.Debug($"Processing time = {sw.Elapsed}");

            //sheet = wb.GetSheetAt(1); //*** detail sheet
            //for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            //{
            //    try
            //    {
            //        IRow row = sheet.GetRow(i);
            //        if (row == null) continue;

            //        var equipments = dbContext.equipment
            //                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
            //                        .Select(o => new { Id = o.id, Text = o.equipment_name });
            //        var trucks = dbContext.truck
            //                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
            //                        .Select(o => new { Id = o.id, Text = o.vehicle_name });
            //        var et = equipments.Union(trucks);
            //        var cn_unit_id = "";
            //        var equiptruck = et.Where(o => o.Text.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()).FirstOrDefault();
            //        if (equiptruck != null) cn_unit_id = equiptruck.Id.ToString();

            //        var timesheet_id = "";
            //        var timesheet = dbContext.timesheet.Where(o => o.cn_unit_id == cn_unit_id).FirstOrDefault();
            //        if (timesheet != null) timesheet_id = timesheet.id.ToString();

            //        var event_category_id = "";
            //        var event_category = dbContext.event_category
            //            .Where(o => o.event_category_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower()).FirstOrDefault();
            //        if (event_category != null) event_category_id = event_category.id.ToString();

            //        var mine_location_id = "";
            //        var location = dbContext.mine_location
            //            .Where(o => o.mine_location_code.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower()).FirstOrDefault();
            //        if (location != null) mine_location_id = location.id.ToString();

            //        var stockpileRecord = dbContext.stockpile_location
            //                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
            //                        .Select(o => new { Id = o.id, Text = o.stock_location_name });
            //        var wasteLocationRecord = dbContext.waste_location
            //                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
            //                        .Select(o => new { Id = o.id, Text = o.stock_location_name });
            //        var sw = stockpileRecord.Union(wasteLocationRecord);
            //        var destination_id = "";
            //        var destination_location = sw.Where(o => o.Text.ToLower() == PublicFunctions.IsNullCell(row.GetCell(3)).ToLower()).FirstOrDefault();
            //        if (destination_location != null) destination_id = destination_location.Id.ToString();

            //        string[] HourList = { "00:00 - 01.00", "01:00 - 02.00", "02:00 - 03.00", "03:00 - 04.00", "04:00 - 05.00", "05:00 - 06.00", "06:00 - 07.00", "07:00 - 08.00", "08:00 - 09.00", "09:00 - 10.00", "10:00 - 11.00", "11:00 - 12.00", "12:00 - 13.00", "13:00 - 14.00", "14:00 - 15.00", "15:00 - 16.00", "16:00 - 17.00", "17:00 - 18.00", "18:00 - 19.00", "19:00 - 20.00", "20:00 - 21.00", "21:00 - 22.00", "22:00 - 23.00", "23:00 - 24.00" };
            //        string periode_id = "";
            //        for (int t = 0; t < HourList.Length; t++)
            //        {
            //            if (HourList[t] == PublicFunctions.IsNullCell(row.GetCell(4)))
            //            {
            //                periode_id = t.ToString();
            //                break;
            //            }
            //        }

            //        var uom_id = "";
            //        var uom = dbContext.uom.Where(o => o.uom_symbol == PublicFunctions.IsNullCell(row.GetCell(10))).FirstOrDefault();
            //        if (uom != null) uom_id = uom.id.ToString();

            //        var record = dbContext.timesheet_detail
            //            .Where(o => o.timesheet_id == timesheet_id && o.periode == periode_id)
            //            .FirstOrDefault();
            //        if (record != null)
            //        {
            //            var e = new entity();
            //            e.InjectFrom(record);

            //            record.InjectFrom(e);
            //            record.modified_by = CurrentUserContext.AppUserId;
            //            record.modified_on = DateTime.Now;
            //            
            //            record.event_category_id = event_category_id;
            //            record.mine_location_id = mine_location_id;
            //            record.destination_id = destination_id;
            //            record.periode = periode_id;
            //            record.duration = PublicFunctions.Desimal(row.GetCell(5));
            //            record.distance = PublicFunctions.Desimal(row.GetCell(6));
            //            record.ritase = PublicFunctions.Desimal(row.GetCell(7));
            //            record.quantity = PublicFunctions.Desimal(row.GetCell(8));
            //            record.refuelling_quantity = PublicFunctions.Desimal(row.GetCell(9));
            //            record.uom_id = uom_id;

            //            await dbContext.SaveChangesAsync();
            //        }
            //        else
            //        {
            //            record = new timesheet_detail();
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

            //            record.timesheet_id = timesheet_id;
            //            record.event_category_id = event_category_id;
            //            record.mine_location_id = mine_location_id;
            //            record.destination_id = destination_id;
            //            record.periode = periode_id.ToString();
            //            record.duration = PublicFunctions.Desimal(row.GetCell(5));
            //            record.distance = PublicFunctions.Desimal(row.GetCell(6));
            //            record.ritase = PublicFunctions.Desimal(row.GetCell(7));
            //            record.quantity = PublicFunctions.Desimal(row.GetCell(8));
            //            record.refuelling_quantity = PublicFunctions.Desimal(row.GetCell(9));
            //            record.uom_id = uom_id;

            //            dbContext.timesheet_detail.Add(record);
            //            await dbContext.SaveChangesAsync();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        if (ex.InnerException != null)
            //        {
            //            errormessage = ex.InnerException.Message;
            //            teks += "==>Error Sheet 2, Line " + (i+1) + " : " + Environment.NewLine;
            //        }
            //        else errormessage = ex.Message;

            //        teks += errormessage + Environment.NewLine + Environment.NewLine;
            //        gagal = true;
            //    }
            //}

            wb.Close();
            if (gagal)
            {
                await transaction.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "Timesheet");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        private async Task<bool> createTimesheetDetail(string timeSheetId, string shiftId)
        {
            var objShift = dbContext.shift.Where(x => x.id == shiftId).FirstOrDefault();

            if (objShift != null)
            {
                if (objShift.start_time != null)
                {
                    TimeSpan tsTime = objShift.start_time;
                    TimeSpan tsTimeEnd = objShift.start_time.Add((TimeSpan)objShift.duration);
                    while (tsTime < tsTimeEnd)
                    {
                        var recordDetail = new timesheet_detail();

                        recordDetail.id = Guid.NewGuid().ToString("N");
                        #region Base Record
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
                        recordDetail.timesheet_id = timeSheetId;
                        recordDetail.timesheet_time = TimeSpan.FromHours((double)tsTime.Hours);
                        recordDetail.classification = TimeSpan.FromHours((double)tsTime.Hours);
                        #endregion

                        dbContext.timesheet_detail.Add(recordDetail);
                        await dbContext.SaveChangesAsync();
                        tsTime = tsTime.Add(TimeSpan.FromHours(1));
                        //if(tsTime.TotalDays == 1)
                        //{
                        //    tsTime = tsTime.Subtract(TimeSpan.FromDays(-1));
                        //}
                    }
                    return true;
                }
            }

            return false;
        }

        [HttpPost("UploadTimeSheet")]
        public object UploadTimeSheet(UploadTimesheetModel tsModel)
        {
            return "yang di passing " + tsModel.test;
        }
    }
}


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

namespace MCSWebApp.Controllers.API.Mining
{
    [Route("api/Timesheet/[controller]")]
    [ApiController]
    public class TimesheetPlanController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public TimesheetPlanController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_timesheet_plan
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_timesheet_plan.Where(o => o.id == Id
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }
        

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            using var tx = await dbContext.Database.BeginTransactionAsync();
            
            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(timesheet_plan),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new timesheet_plan();
                    JsonConvert.PopulateObject(values, record);

                    var recordExist = dbContext.timesheet_plan
                            .Where(o => o.cn_unit_id == record.cn_unit_id
                                    && o.timesheet_date == record.timesheet_date
                                    && o.shift_id == record.shift_id
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                            .FirstOrDefault();

                    if (recordExist != null)
                    {
                        return BadRequest("Already have same timesheet plan.");
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
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                    #endregion

                    dbContext.timesheet_plan.Add(record);
                    await dbContext.SaveChangesAsync();

                    
                    if (await createTimesheetDetail(record.id, record.shift_id, record.cn_unit_id))
                    {
                        await tx.CommitAsync();
                        return Ok(record);
                    }
                    else
                    {
                        return BadRequest("failed create timesheet details.");
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
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var record = dbContext.timesheet_plan
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
                        using var tx = await dbContext.Database.BeginTransactionAsync();
                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        await dbContext.SaveChangesAsync();
                        if (shiftId != record.shift_id)
                        {
                            //delete all timesheet detail if different shift
                            var tsDetails = dbContext.timesheet_detail_plan.Where(x => x.timesheet_id == record.id).ToList();
                            if (tsDetails.Count() > 0)
                            {
                                foreach (var item in tsDetails)
                                {
                                    var recordDetailEVent = dbContext.timesheet_detail_event_plan
                                        .Where(o => o.timesheet_detail_id == item.id).ToList();
                                    if (recordDetailEVent.Count() > 0)
                                    {
                                        dbContext.timesheet_detail_event_plan.RemoveRange(recordDetailEVent);
                                        await dbContext.SaveChangesAsync();
                                    }

                                    var recordDetailProblem = dbContext.timesheet_detail_productivity_problem_plan
                                        .Where(o => o.timesheet_detail_id == item.id).ToList();
                                    if (recordDetailEVent.Count() > 0)
                                    {
                                        dbContext.timesheet_detail_productivity_problem_plan.RemoveRange(recordDetailProblem);
                                        await dbContext.SaveChangesAsync();
                                    }
                                }
                                dbContext.timesheet_detail_plan.RemoveRange(tsDetails);
                                await dbContext.SaveChangesAsync();
                            }

                            //create new timesheet detail
                            if (await createTimesheetDetail(record.id, record.shift_id, record.cn_unit_id))
                            {
                                await tx.CommitAsync();
                                return Ok(record);
                            }
                            else
                            {
                                return BadRequest("failed create timesheet details.");
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
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    var record = dbContext.timesheet_plan
                        .Where(o => o.id == key
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();                    
                    if (record != null)
                    {
                        if (await mcsContext.CanDelete(dbContext, key, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            var recordDetail = dbContext.timesheet_detail_plan
                                .Where(o => o.timesheet_id == record.id).ToList();
                            if(recordDetail.Count() > 0)
                            {
                                foreach(var item in recordDetail)
                                {
                                    var recordDetailEVent = dbContext.timesheet_detail_event_plan
                                        .Where(o => o.timesheet_detail_id == item.id).ToList();
                                    if(recordDetail.Count() > 0)
                                    {
                                        dbContext.timesheet_detail_event_plan.RemoveRange(recordDetailEVent);
                                        await dbContext.SaveChangesAsync();

                                    }

                                    var recordDetailProblem = dbContext.timesheet_detail_productivity_problem_plan
                                        .Where(o => o.timesheet_detail_id == item.id).ToList();
                                    if (recordDetail.Count() > 0)
                                    {
                                        dbContext.timesheet_detail_productivity_problem_plan.RemoveRange(recordDetailProblem);
                                        await dbContext.SaveChangesAsync();

                                    }
                                }
                                dbContext.timesheet_detail_plan.RemoveRange(recordDetail);
                                await dbContext.SaveChangesAsync();

                            }

                            dbContext.timesheet_plan.Remove(record);
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
                    var record = await dbContext.timesheet_plan
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
                    else if (await mcsContext.CanCreate(dbContext, nameof(timesheet_plan),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        record = new timesheet_plan();
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

                        dbContext.timesheet_plan.Add(record);
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
					logger.Error(ex.InnerException ?? ex);
					return BadRequest(ex.InnerException?.Message ?? ex.Message);
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
            TimeSpan oldDuration = default;
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


            using var transaction = await dbContext.Database.BeginTransactionAsync();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {

                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var equipments = dbContext.equipment
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .Select(o => new { Id = o.id, Text = o.equipment_name });
                    var trucks = dbContext.truck
                                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                                    .Select(o => new { Id = o.id, Text = o.vehicle_name });
                    var lookup = equipments.Union(trucks);

                    string cn_unit_id = "";
                    var equiptruck = lookup.Where(o => o.Id.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower()).FirstOrDefault();
                    if (equiptruck != null)
                    {
                        cn_unit_id = equiptruck.Id.ToString();
                        currentCNID = cn_unit_id;
                    }
                    DateTime timesheet_date = PublicFunctions.Tanggal(row.GetCell(0));
                    currentTimesheetDate = timesheet_date;

                    string shift_id = "";
                    var shift = dbContext.shift
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.id.ToLower() == PublicFunctions.IsNullCell(row.GetCell(2)).ToLower()).FirstOrDefault();
                    if (shift != null)
                    {
                        shift_id = shift.id.ToString();
                        currentShift = shift_id;
                    }

                    string operator_id = null;
                    
                    var record = new timesheet_plan();

                    //if (currentCNID != oldCNID
                    //    || currentTimesheetDate != oldTimesheetDate
                    //    || currentShift != oldShiftText
                    //    )
                    //{
                        string mine_location_id = null;
                        
                        string supervisor_id = null;
                        
                        string uom_id = null;
                        
                        string material_id = null;                                                

                        record = dbContext.timesheet_plan
                        .Where(o => o.cn_unit_id == cn_unit_id
                                && o.timesheet_date == timesheet_date
                                && o.shift_id == shift_id
                                && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();

                        if (record == null)
                        {
                            record = new timesheet_plan
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
                                operator_id = operator_id,
                                supervisor_id = supervisor_id,
                                quantity = null,
                                shift_id = shift_id,
                                uom_id = uom_id,
                                timesheet_date = timesheet_date,
                                material_id = material_id
                                
                            };

                            dbContext.timesheet_plan.Add(record);
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {                            
                            //delete all timesheet detail 
                            var tsDetails = dbContext.timesheet_detail_plan.Where(x => x.organization_id == CurrentUserContext.OrganizationId && 
                                x.timesheet_id == record.id).ToList();
                            if (tsDetails.Count() > 0)
                            {
                                foreach (var item in tsDetails)
                                {
                                    var recordDetailEVent = dbContext.timesheet_detail_event_plan
                                        .Where(o => o.timesheet_detail_id == item.id).ToList();
                                    if (recordDetailEVent.Count() > 0)
                                    {
                                        dbContext.timesheet_detail_event_plan.RemoveRange(recordDetailEVent);
                                        await dbContext.SaveChangesAsync();
                                    }

                                    var recordDetailProblem = dbContext.timesheet_detail_productivity_problem_plan
                                        .Where(o => o.timesheet_detail_id == item.id).ToList();
                                    if (recordDetailEVent.Count() > 0)
                                    {
                                        dbContext.timesheet_detail_productivity_problem_plan.RemoveRange(recordDetailProblem);
                                        await dbContext.SaveChangesAsync();
                                    }
                                }
                                dbContext.timesheet_detail_plan.RemoveRange(tsDetails);
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


                    //}
                    #region detail
                    //detail
                    string pit_id = PublicFunctions.IsNullCell(row.GetCell(3)).ToLower();
                    string loader_id = null;
                    string source_id = null;
                    string destination_id = null;
                    string material_id_detail = null;
                    var distance = PublicFunctions.Desimal(row.GetCell(5));
                    var ritase = 0;
                    var volume = PublicFunctions.Desimal(row.GetCell(4));
                    var refuelling_quantity = 0;
                    var classification = shift.start_time;
                    var material = PublicFunctions.IsNullCell(row.GetCell(8)).ToLower();
                    var accounting_period_id = PublicFunctions.IsNullCell(row.GetCell(7)).ToLower();
                    var recordDetail = new timesheet_detail_plan();

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
                    recordDetail.refuelling_quantity = refuelling_quantity;
                    recordDetail.material_id = material;
                    recordDetail.accounting_periode_id = accounting_period_id;

                    dbContext.timesheet_detail_plan.Add(recordDetail);
                    await dbContext.SaveChangesAsync();
                    //tsTime = tsTime.Add(TimeSpan.FromHours(1));
                    #endregion

                    #region event and problem
                    //cek ada event dan problem
                    string events = PublicFunctions.IsNullCell(row.GetCell(6)).ToLower();
                    //string problems = PublicFunctions.IsNullCell(row.GetCell(19)).ToLower();

                    if (!string.IsNullOrEmpty(events))
                    {
                        string[] eventArray = events.Split("#");
                        foreach (string item in eventArray)
                        {
                            string[] eventItem = item.Split("-");
                            int minute = 0;
                            if (eventItem.Count() == 2 && int.TryParse(eventItem[1], out minute))
                            {
                                var eventCategory = dbContext.event_category.Where(x => x.id.ToLower() == eventItem[0]
                                && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();

                                if (eventCategory != null)
                                {
                                    var recordEvent = new timesheet_detail_event_plan();

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

                                    dbContext.timesheet_detail_event_plan.Add(recordEvent);
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }
                    }

                    //if (!string.IsNullOrEmpty(problems))
                    //{
                    //    string[] problemArray = problems.Split("#");
                    //    foreach (string item in problemArray)
                    //    {
                    //        string[] problemItem = item.Split("-");
                    //        int frequency = 0;
                    //        if (problemItem.Count() == 2 && int.TryParse(problemItem[1], out frequency))
                    //        {
                    //            var eventCategory = dbContext.event_category.Where(x => x.id.ToLower() == problemItem[0]
                    //            && x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();

                    //            if (eventCategory != null)
                    //            {
                    //                var recordProblem = new timesheet_detail_productivity_problem_plan();

                    //                recordProblem.id = Guid.NewGuid().ToString("N");
                    //                recordProblem.created_by = CurrentUserContext.AppUserId;
                    //                recordProblem.created_on = DateTime.Now;
                    //                recordProblem.modified_by = null;
                    //                recordProblem.modified_on = null;
                    //                recordProblem.is_active = true;
                    //                recordProblem.is_default = null;
                    //                recordProblem.is_locked = null;
                    //                recordProblem.entity_id = null;
                    //                recordProblem.owner_id = CurrentUserContext.AppUserId;
                    //                recordProblem.organization_id = CurrentUserContext.OrganizationId;
                    //                recordProblem.timesheet_detail_id = recordDetail.id;
                    //                recordProblem.event_category_id = eventCategory.id;
                    //                recordProblem.event_definition_category_id = eventCategory.event_definition_category_id;
                    //                recordProblem.frequency = frequency;

                    //                dbContext.timesheet_detail_productivity_problem_plan.Add(recordProblem);
                    //                await dbContext.SaveChangesAsync();
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    //#region sisa detail
                    ////cek kalo selanjutnya keluar dr looping
                    //if (i + 1 > sheet.LastRowNum)
                    //{
                    //    TimeSpan tsTimeEnd = oldShift.start_time.Add((TimeSpan)oldShift.duration);

                    //    while (tsTime < tsTimeEnd)
                    //    {
                    //        var recordDetailEmpty = new timesheet_detail();

                    //        recordDetailEmpty.id = Guid.NewGuid().ToString("N");
                    //        #region Base Record
                    //        recordDetailEmpty.created_by = CurrentUserContext.AppUserId;
                    //        recordDetailEmpty.created_on = DateTime.Now;
                    //        recordDetailEmpty.modified_by = null;
                    //        recordDetailEmpty.modified_on = null;
                    //        recordDetailEmpty.is_active = true;
                    //        recordDetailEmpty.is_default = null;
                    //        recordDetailEmpty.is_locked = null;
                    //        recordDetailEmpty.entity_id = null;
                    //        recordDetailEmpty.owner_id = CurrentUserContext.AppUserId;
                    //        recordDetailEmpty.organization_id = CurrentUserContext.OrganizationId;
                    //        recordDetailEmpty.timesheet_id = oldTimeSheetId;
                    //        recordDetailEmpty.timesheet_time = TimeSpan.FromHours((double)tsTime.Hours);
                    //        recordDetailEmpty.classification = TimeSpan.FromHours((double)tsTime.Hours);
                    //        #endregion

                    //        dbContext.timesheet_detail.Add(recordDetailEmpty);
                    //        await dbContext.SaveChangesAsync();
                    //        tsTime = tsTime.Add(TimeSpan.FromHours(1));
                    //        //if(tsTime.TotalDays == 1)
                    //        //{
                    //        //    tsTime = tsTime.Subtract(TimeSpan.FromDays(-1));
                    //        //}
                    //    }
                    //}
                    //#endregion

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
                HttpContext.Session.SetString("filename", "Timesheet");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        private async Task<bool> createTimesheetDetail(string timeSheetId, string shiftId, string cnId = null)
        {
            var objShift = dbContext.shift.Where(x => x.id == shiftId).FirstOrDefault();

            if (objShift != null)
            {
                if (objShift.start_time != null)
                {

                    if (cnId == null)
                    {
                        TimeSpan tsTime = objShift.start_time;
                        TimeSpan tsTimeEnd = objShift.start_time.Add((TimeSpan)objShift.duration);
                        while (tsTime < tsTimeEnd)
                        {
                            var recordDetail = new timesheet_detail_plan();

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

                            dbContext.timesheet_detail_plan.Add(recordDetail);
                            await dbContext.SaveChangesAsync();
                            tsTime = tsTime.Add(TimeSpan.FromHours(1));
                            //if(tsTime.TotalDays == 1)
                            //{
                            //    tsTime = tsTime.Subtract(TimeSpan.FromDays(-1));
                            //}
                        }
                    }
                    else
                    {
                        var recordDetail = new timesheet_detail_plan();

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
                        recordDetail.timesheet_time = objShift.start_time;
                        recordDetail.classification = objShift.start_time;
                        #endregion

                        dbContext.timesheet_detail_plan.Add(recordDetail);
                        await dbContext.SaveChangesAsync();                        
                    }
                    return true;
                }                
            }

            return false;
        }
    }
}

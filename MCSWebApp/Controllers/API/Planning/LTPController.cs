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
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Net.Http;
using static System.Net.Mime.MediaTypeNames;
using DocumentFormat.OpenXml.Office2010.Excel;
using BusinessLogic.Entity;
using NPOI.SS.Formula.Functions;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.SignalR;
using System.Threading;

namespace MCSWebApp.Controllers.API.Planning
{
    [Route("/api/Planning/[controller]")]
    [ApiController]
    public class LTPController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;
        private readonly IHubContext<ProgressHub> _hubContext;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public LTPController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption, IHubContext<ProgressHub> hubContext)
             : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
            _hubContext = hubContext;
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_mine_plan_ltp
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                        loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.mine_plan_ltp.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    string id;
                    if (await mcsContext.CanCreate(dbContext, nameof(mine_plan_ltp),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var record = new mine_plan_ltp();
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
                        var ltp = record.id;
                        //record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                        //CHECK DATA LTP YANG SAMA
                        var ltpData = dbContext.mine_plan_ltp.Where(o => o.mine_code == record.mine_code && o.submine_code == record.submine_code && o.pit_code == record.pit_code
                            && o.subpit_code == record.subpit_code && o.contractor_code == record.contractor_code && o.seam_code == record.seam_code && o.blok_code == record.blok_code
                            && o.material_type_id == record.material_type_id && o.reserve_type_id == record.reserve_type_id).FirstOrDefault();
                        dbContext.mine_plan_ltp.Add(record);
                        await dbContext.SaveChangesAsync();

                        if (await mcsContext.CanCreate(dbContext, nameof(mine_location),
                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                        {
                            //CONCAT RECORD MINE TO CONTRACTOR TO GET BUSINESS AREA ID
                            var code = record.mine_code.ToUpper() + "_" + record.submine_code.ToUpper() + "_"
                                + record.pit_code.ToUpper() + "_" + record.subpit_code.ToUpper() + "_" + record.contractor_code.ToUpper() + "_" + record.seam_code;
                            var contractor = dbContext.contractor.Where(o => o.business_partner_code == record.contractor_code.ToUpper()).Select(o => o.id).FirstOrDefault();
                            var businessArea = dbContext.vw_business_area
                                                .Where(o => o.business_area_code == code)
                                                .Select(o => o.id).FirstOrDefault();
                            //VALIDATE LTP DATA NYA SUDAH ADA ATAU TIDAK
                            if (ltpData != null)
                            {
                                return BadRequest("You Already Have a Data for This Business Area with the same Matarial Type and Reserve Type. Please Choose Another Material Type or Reserve Type");
                            }
                            //CONCAT CODE DAN BLOK CODE, CEK DATA NYA SUDAH ADA ATAU BELUM DI MINE LOCATION
                            var mineCode = code + "_" + record.blok_code.ToUpper();
                            var mineLocation = dbContext.mine_location.Where(o => o.mine_location_code == mineCode).FirstOrDefault();
                            //KONDISI JIKA SUDAH ADA MAKA HANYA INSERT KE MODEL GEOLOGY, ELSE MAKE A NEW ONE
                            if (mineLocation != null)
                            {
                                // mineLocation.mine_plan_ltp_id = record.id;
                                //var month = Convert.ToString(Convert.ToDateTime(record.created_on).Month);
                                // var year = Convert.ToString(Convert.ToDateTime(record.created_on).Year);
                                // var masterlist = dbContext.master_list.Where(o => o.item_group == "years" && o.item_name == year).Select(o => o.id).FirstOrDefault();
                                mineLocation.contractor_id = contractor;
                                var Geology = dbContext.model_geology.Where(o => o.mine_location_id == record.id && o.material_type_id == record.material_type_id).FirstOrDefault();
                                // if geology sudah ada then modify else make a new one.
                                if (Geology == null)
                                {
                                    var recordGeology = new model_geology();
                                    recordGeology.id = Guid.NewGuid().ToString("N");
                                    recordGeology.created_by = CurrentUserContext.AppUserId;
                                    recordGeology.created_on = record.model_date; //created on used for model date
                                    recordGeology.modified_by = null;
                                    recordGeology.modified_on = null;
                                    recordGeology.is_active = true;
                                    recordGeology.is_default = null;
                                    recordGeology.is_locked = null;
                                    recordGeology.entity_id = null;
                                    recordGeology.owner_id = CurrentUserContext.AppUserId;
                                    recordGeology.organization_id = CurrentUserContext.OrganizationId;
                                    recordGeology.business_unit_id = record.business_unit_id;
                                   // recordGeology.year_id = masterlist;
                                    //recordGeology.month_id = month;
                                    recordGeology.waste_bcm = Convert.ToDecimal(record.waste_bcm);
                                    recordGeology.im = record.im_ar;
                                    recordGeology.tm = record.tm_ar;
                                    recordGeology.ts = record.ts_ar;
                                    recordGeology.fc = record.fc_ar;
                                    recordGeology.ash = record.ash_ar;
                                    recordGeology.vm = record.vm_ar;
                                    recordGeology.gcv_ar = record.gcv_ar_ar;
                                    recordGeology.gcv_adb = record.gcv_adb_ar;
                                    recordGeology.rdi = record.rdi_ar;
                                    recordGeology.hgi = record.hgi_ar;
                                    recordGeology.rd = record.rd_ar;
                                    recordGeology.quantity = record.coal_tonnage == null ? 0 : record.coal_tonnage;
                                    recordGeology.material_type_id = record.material_type_id;
                                    recordGeology.mine_location_id = mineLocation.id;
                                    dbContext.model_geology.Add(recordGeology);
                                    await dbContext.SaveChangesAsync();
                                }
                                else
                                {
                                    Geology.created_on = record.model_date; //created on used for model date 
                                    Geology.modified_by = CurrentUserContext.AppUserId;
                                    Geology.modified_on = DateTime.Now;
                                   // Geology.year_id = masterlist;
                                    // Geology.month_id = month;
                                    Geology.waste_bcm = Convert.ToDecimal(record.waste_bcm);
                                    Geology.im = record.im_ar;
                                    Geology.tm = record.tm_ar;
                                    Geology.ts = record.ts_ar;
                                    Geology.fc = record.fc_ar;
                                    Geology.ash = record.ash_ar;
                                    Geology.vm = record.vm_ar;
                                    Geology.gcv_ar = record.gcv_ar_ar;
                                    Geology.gcv_adb = record.gcv_adb_ar;
                                    Geology.rdi = record.rdi_ar;
                                    Geology.hgi = record.hgi_ar;
                                    Geology.rd = record.rd_ar;
                                    Geology.quantity = record.coal_tonnage == null ? 0 : record.coal_tonnage;
                                    Geology.material_type_id = record.material_type_id;
                                    Geology.mine_location_id = mineLocation.id;
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                var recordMineLocation = new mine_location();
                                //JsonConvert.PopulateObject(values, record);
                                recordMineLocation.id = Guid.NewGuid().ToString("N");
                                recordMineLocation.created_by = CurrentUserContext.AppUserId;
                                recordMineLocation.created_on = DateTime.Now;
                                recordMineLocation.modified_by = null;
                                recordMineLocation.modified_on = null;
                                recordMineLocation.is_active = true;
                                recordMineLocation.is_default = null;
                                recordMineLocation.is_locked = null;
                                recordMineLocation.entity_id = null;
                                recordMineLocation.owner_id = CurrentUserContext.AppUserId;
                                recordMineLocation.organization_id = CurrentUserContext.OrganizationId;
                                recordMineLocation.business_unit_id = record.business_unit_id;

                                recordMineLocation.business_area_id = businessArea;
                                recordMineLocation.mine_location_code = code + "_" + record.blok_code;
                                recordMineLocation.stock_location_name =/* record.seam_code + "-" +*/ record.blok_code;
                                recordMineLocation.opening_date = record.created_on;
                                recordMineLocation.contractor_id = contractor;
                                id = recordMineLocation.id;
                                recordMineLocation.mine_plan_ltp_id = ltp;
                                dbContext.mine_location.Add(recordMineLocation);
                                await dbContext.SaveChangesAsync();

                                if (await mcsContext.CanCreate(dbContext, nameof(model_geology),
                                CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                                {
                                   // var month = Convert.ToString(Convert.ToDateTime(record.created_on).Month);
                                    //var year = Convert.ToString(Convert.ToDateTime(record.created_on).Year);
                                    //var masterlist = dbContext.master_list.Where(o => o.item_group == "years" && o.item_name == year).Select(o => o.id).FirstOrDefault();

                                    var Geology = dbContext.model_geology.Where(o => o.mine_location_id == record.id && o.material_type_id == record.material_type_id).FirstOrDefault();
                                    if (Geology == null)
                                    {
                                        var recordGeology = new model_geology();
                                        recordGeology.id = Guid.NewGuid().ToString("N"); 
                                        recordGeology.created_on = record.model_date; 
                                        recordGeology.created_on = DateTime.Now;
                                        recordGeology.modified_by = null;
                                        recordGeology.modified_on = null;
                                        recordGeology.is_active = true;
                                        recordGeology.is_default = null;
                                        recordGeology.is_locked = null;
                                        recordGeology.entity_id = null;
                                        recordGeology.owner_id = CurrentUserContext.AppUserId;
                                        recordGeology.organization_id = CurrentUserContext.OrganizationId;
                                        recordGeology.business_unit_id = record.business_unit_id;
                                        //recordGeology.year_id = masterlist;
                                        //recordGeology.month_id = month;
                                        recordGeology.waste_bcm = Convert.ToDecimal(record.waste_bcm);
                                        recordGeology.im = record.im_ar;
                                        recordGeology.tm = record.tm_ar;
                                        recordGeology.ts = record.ts_ar;
                                        recordGeology.fc = record.fc_ar;
                                        recordGeology.ash = record.ash_ar;
                                        recordGeology.vm = record.vm_ar;
                                        recordGeology.gcv_ar = record.gcv_ar_ar;
                                        recordGeology.gcv_adb = record.gcv_adb_ar;
                                        recordGeology.rdi = record.rdi_ar;
                                        recordGeology.hgi = record.hgi_ar;
                                        recordGeology.rd = record.rd_ar;
                                        recordGeology.quantity = record.coal_tonnage == null ? 0 : record.coal_tonnage;
                                        recordGeology.material_type_id = record.material_type_id;
                                        recordGeology.mine_location_id = id;
                                        dbContext.model_geology.Add(recordGeology);
                                        await dbContext.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        Geology.modified_by = CurrentUserContext.AppUserId;
                                        Geology.modified_on = DateTime.Now;
                                        // Geology.year_id = masterlist;
                                        //Geology.month_id = month;
                                        Geology.created_on = record.model_date;
                                        Geology.waste_bcm = Convert.ToDecimal(record.waste_bcm);
                                        Geology.im = record.im_ar;
                                        Geology.tm = record.tm_ar;
                                        Geology.ts = record.ts_ar;
                                        Geology.fc = record.fc_ar;
                                        Geology.ash = record.ash_ar;
                                        Geology.vm = record.vm_ar;
                                        Geology.gcv_ar = record.gcv_ar_ar;
                                        Geology.gcv_adb = record.gcv_adb_ar;
                                        Geology.rdi = record.rdi_ar;
                                        Geology.hgi = record.hgi_ar;
                                        Geology.rd = record.rd_ar;
                                        Geology.quantity = record.coal_tonnage == null ? 0 : record.coal_tonnage;
                                        Geology.material_type_id = record.material_type_id;
                                        Geology.mine_location_id = mineLocation.id;
                                        await dbContext.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    return BadRequest("User is not authorized.");
                                }
                            }

                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
                        }
                        var history = new mineplan_ltp_history();
                        JsonConvert.PopulateObject(values, history);
                        history.id = Guid.NewGuid().ToString("N");
                        history.created_by = CurrentUserContext.AppUserId;
                        history.created_on = record.model_date;
                        history.modified_by = null;
                        history.modified_on = null;
                        history.is_active = true;
                        history.is_default = null;
                        history.is_locked = null;
                        history.entity_id = null;
                        history.owner_id = CurrentUserContext.AppUserId;
                        history.organization_id = CurrentUserContext.OrganizationId;
                        history.header_id = record.id;
                        dbContext.mineplan_ltp_history.Add(history);

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
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPost("FetchToQualitySampling")]    //**** tanpa trigger trf_sales_invoice_approval_sync di table sales_invoice_approval
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> FetchToQualitySampling(string Id)
        {
            var result = new StandardResult();

           

            var mineLocations = await dbContext.mine_location.ToListAsync();
            var samplingTempaltes = await dbContext.sampling_template.ToListAsync();
            var samplingTypes = await dbContext.sampling_type.ToListAsync();
            var surveys = await dbContext.survey.ToListAsync();

            List<mine_plan_ltp> ltps;
            switch (Id)
            {
                case "76d138be53ad48d1822a3bd76aacab1b": //AsamAsam
                     ltps = await dbContext.mine_plan_ltp
                                    .Where(o => o.business_unit_id == Id && o.material_type_id == "7c61e2b5f89044a9b152c4d606b97936" && o.reserve_type_id == "520f774d4a524518bdb1e4f24bc75386")
                                    .ToListAsync();
                    break;
                case "cd33ec601e334b8ea03daecc45eed8a0":
                     ltps = await dbContext.mine_plan_ltp
                                    .Where(o => o.business_unit_id == Id && o.material_type_id == "7c61e2b5f89044a9b152c4d606b97936" && o.reserve_type_id == "520f774d4a524518bdb1e4f24bc75386")
                                    .ToListAsync();
                    break;
                case "7d1bf0bd927c43df822f3287461cdde3":
                     ltps = await dbContext.mine_plan_ltp
                                    .Where(o => o.business_unit_id == Id && o.material_type_id == "7c61e2b5f89044a9b152c4d606b97936" && o.reserve_type_id == "520f774d4a524518bdb1e4f24bc75386")
                                    .ToListAsync();
                    break;
                case "ee0b98be20d44a08bb9c00e56c335327":
                     ltps = await dbContext.mine_plan_ltp
                                    .Where(o => o.business_unit_id == Id && o.material_type_id == "7c61e2b5f89044a9b152c4d606b97936" && o.reserve_type_id == "520f774d4a524518bdb1e4f24bc75386")
                                    .ToListAsync();
                    break;
                case "ab743db8c718439a8de1c00216f56b98":
                     ltps = await dbContext.mine_plan_ltp
                                    .Where(o => o.business_unit_id == Id && o.material_type_id == "7c61e2b5f89044a9b152c4d606b97936" && o.reserve_type_id == "520f774d4a524518bdb1e4f24bc75386")
                                    .ToListAsync();
                    break;
                default:
                    ltps = await dbContext.mine_plan_ltp.Where(o=> o.material_type_id == "7c61e2b5f89044a9b152c4d606b97936" && o.reserve_type_id == "520f774d4a524518bdb1e4f24bc75386")
                                  .ToListAsync();
                    break;
            }
            var count = 0;
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var item in ltps)
                    {
                        count++;
                        var code = item.mine_code.ToUpper() + "_" + item.submine_code.ToUpper() + "_"
                                        + item.pit_code.ToUpper() + "_" + item.subpit_code.ToUpper() + "_" + item.contractor_code.ToUpper() + "_" + item.seam_code + "_" + item.blok_code.ToUpper();

                        var mineLocation =  mineLocations.Where(o => o.mine_location_code == code).FirstOrDefault(); //.Select(o => o.id)
                        var samplingTempalte = samplingTempaltes.Where(o => o.sampling_template_name == "GEOLOGY").FirstOrDefault();
                        var samplingType = samplingTypes.Where(o => o.sampling_type_code == "GEO").FirstOrDefault();

                        var samplingNumber = code + "_" + Convert.ToDateTime(item.model_date).ToString("yyyy-MM-dd HH:mm");
                        var current = surveys.Where(o => o.survey_number == samplingNumber).FirstOrDefault();
                        if(current != null) { continue; }

                        if (await mcsContext.CanCreate(dbContext, nameof(survey),
                            CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                        {
                            var record = new survey();

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

                            record.uom_id = "76dd627ffad44d74b9ce022f04677609";
                            record.stock_location_id = mineLocation.id;
                            record.sampling_template_id = samplingTempalte.id;
                            record.survey_date = (DateTime)item.model_date;
                            record.business_unit_id = item.business_unit_id;
                            record.survey_number = samplingNumber;
                            record.quantity = item.coal_tonnage;
                            //record.sampling_type_id = samplingType.id;

                            dbContext.survey.Add(record);
                            await dbContext.SaveChangesAsync();

                            string[] analyteIdList = { "e410ab5fc90544168169dbee2fc08504", "fd7266e329be4311b2a05bf9776d7b75", "98c6162bfa084fcd8378f158ce8b0388", "aea5c4a6a37c4680868c8ce4c815b6b5", "a233c12c7131409c9eb5e320ebad0157",
                                                "7b1153bbb4d14461ae67c4e55fd00944","ad0f6aafe6a14bdbb47522f8d0f15bea","477c28cf7b8248d686eb0a6731210f15","63903fd614e74b7ab71a202fd2120c47","e585181855cd43eb86128a5cddbebea3",
                                                "992744db8de342368047d6c5d8799ad3"};

                            string[] uomId = { "90f4b279a62e4efdb33b8e2d6c292e7d", "90f4b279a62e4efdb33b8e2d6c292e7d", "90f4b279a62e4efdb33b8e2d6c292e7d", "90f4b279a62e4efdb33b8e2d6c292e7d", "90f4b279a62e4efdb33b8e2d6c292e7d"
                                        , "90f4b279a62e4efdb33b8e2d6c292e7d", "457ec80c9b0c4dae87ab149bdbb3266b", "457ec80c9b0c4dae87ab149bdbb3266b", "79b3cd26c1234d44b45534fdf503baa3", "79b3cd26c1234d44b45534fdf503baa3",
                                        "80d855cb81d844108939785c9356a9c2"};

                            decimal?[] value = { item.tm_ar, item.ts_ar, item.ash_ar, item.im_ar,item.vm_ar,item.fc_ar,item.gcv_ar_ar, item.gcv_adb_ar,item.rd_ar,item.rdi_ar,
                                            item.hgi_ar};
                            if (await mcsContext.CanCreate(dbContext, nameof(survey_analyte),
                                 CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                            {
                                for (var i = 0; i < analyteIdList.Length; i++)
                                {
                                    var recordDetail = new survey_analyte();
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

                                    recordDetail.uom_id = uomId[i];
                                    recordDetail.analyte_value = value[i];
                                    recordDetail.analyte_id = analyteIdList[i];
                                    recordDetail.survey_id = record.id;
                                    dbContext.survey_analyte.Add(recordDetail);
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }
                        
                    }


                    result.Success = true;

                    await tx.CommitAsync();
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.InnerException ?? ex);
                    result.Message = ex.Message;
                    result.Success = false;
                    return BadRequest(result);
                }
            }
        }



        [HttpPut("UpdateContractor")]
        public async Task<IActionResult> UpdateContractor([FromForm] string key, [FromForm] string values)
        {
            var result = new StandardResult();

            logger.Trace($"string values = {values}");
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    List<mine_plan_ltp> records = await dbContext.mine_plan_ltp.ToListAsync();
                    List<contractor> contractors = await dbContext.contractor.ToListAsync();
                    List<mine_location> mineLocs = await dbContext.mine_location.ToListAsync();
                    List<stockpile_location> stockLocs = await dbContext.stockpile_location.ToListAsync();  
                    var count = 0;
                    if (records != null)
                    {
                        foreach(var record in records)
                        {
                            var code = record.mine_code.ToUpper() + "_" + record.submine_code.ToUpper() + "_"
                                        + record.pit_code.ToUpper() + "_" + record.subpit_code.ToUpper() + "_" + record.contractor_code.ToUpper() + "_" + record.seam_code + "_" + record.blok_code.ToUpper();

                            var contractor = contractors.Where(o => o.business_partner_code == record.contractor_code.ToUpper()).Select(o => o.id).FirstOrDefault();
                            var mineCode = code + "_" + record.blok_code.ToUpper();
                            var mineLoc = mineLocs.Where(o => o.mine_location_code.ToUpper() == mineCode.ToUpper()).FirstOrDefault();
                            var stockLoc = stockLocs.Where(o => o.stockpile_location_code.ToUpper() == mineCode.ToUpper()).FirstOrDefault();
                            if (mineLoc != null && mineLoc.contractor_id == null && contractor != null)
                            {
                                mineLoc.contractor_id = contractor;
                            }
                        await dbContext.SaveChangesAsync();
                            if (stockLoc != null && stockLoc.contractor_id == null && contractor != null)
                            {
                                stockLoc.contractor_id = contractor;
                            }
                        await dbContext.SaveChangesAsync();
                            count++;
                        }
                        await tx.CommitAsync();
                    }

                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    result.Message = ex.Message;
                    result.Success = false;
                    return BadRequest(result);
                }

            }

            result.Success = true;
            return Ok(result);

        }

        [HttpPut("UpdateData")]
        public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var record = dbContext.mine_plan_ltp
                        .Where(o => o.id == key)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin)
                        {
                            var approvalStatus = record.approved;

                            var e = new entity();
                            e.InjectFrom(record);

                            JsonConvert.PopulateObject(values, record);

                           // record.InjectFrom(e);
                            record.modified_by = CurrentUserContext.AppUserId;
                            record.modified_on = DateTime.Now;
                            await dbContext.SaveChangesAsync();
                            // update mine location
                            var code = record.mine_code.ToUpper() + "_" + record.submine_code.ToUpper() + "_"
                               + record.pit_code.ToUpper() + "_" + record.subpit_code.ToUpper() + "_" + record.contractor_code.ToUpper() + "_" + record.seam_code;
                            var contractor = dbContext.contractor.Where(o => o.business_partner_code == record.contractor_code.ToUpper()).Select(o => o.id).FirstOrDefault();
                            var businessArea = dbContext.vw_business_area
                                                .Where(o => o.business_area_code == code)
                                                .Select(o => o.id).FirstOrDefault();
                            var mineLocation = dbContext.mine_location.Where(o => o.business_area_id == businessArea).FirstOrDefault();
                             if (mineLocation.business_area_id != businessArea && mineLocation != null)
                            {
                                return BadRequest("You Already Have a Data in This Business Area. Please Insert Different Area");
                            }
                            var mineLoc = dbContext.mine_location.Where(o => o.mine_location_code == code + "_" + record.blok_code).FirstOrDefault();

                            mineLoc.modified_by = CurrentUserContext.AppUserId;
                            mineLoc.modified_on = DateTime.Now;
                            mineLoc.business_area_id = businessArea;
                            mineLoc.mine_location_code = code + "_" + record.blok_code;
                            mineLoc.stock_location_name =/* record.seam_code + "-" +*/ record.blok_code;
                            mineLoc.opening_date = record.created_on;
                            mineLoc.contractor_id = contractor;
                            string id = mineLoc.id;
                            // record.contractor_code = ltp;
                            await dbContext.SaveChangesAsync();
                            //update model geology
                            var recordGeology = dbContext.model_geology.Where(o=>o.mine_location_id == id && o.material_type_id == record.material_type_id).FirstOrDefault();
                            //var month = Convert.ToString(Convert.ToDateTime(record.created_on).Month);
                           // var year = Convert.ToString(Convert.ToDateTime(record.created_on).Year);
                           // var masterlist = dbContext.master_list.Where(o => o.item_group == "years" && o.item_name == year).Select(o => o.id).FirstOrDefault();

                            recordGeology.modified_by = CurrentUserContext.AppUserId;
                            recordGeology.modified_on = DateTime.Now;
                            recordGeology.created_on = record.model_date;
                            //recordGeology.year_id = masterlist;
                            //recordGeology.month_id = month;
                            recordGeology.waste_bcm = Convert.ToDecimal(record.waste_bcm);
                            recordGeology.im = record.im_ar;
                            recordGeology.tm = record.tm_ar;
                            recordGeology.ts = record.ts_ar;
                            recordGeology.fc = record.fc_ar;
                            recordGeology.ash = record.ash_ar;
                            recordGeology.vm = record.vm_ar;
                            recordGeology.gcv_ar = record.gcv_ar_ar;
                            recordGeology.gcv_adb = record.gcv_adb_ar;
                            recordGeology.rdi = record.rdi_ar;
                            recordGeology.hgi = record.gcv_adb_ar;
                            recordGeology.rd = record.rd_ar;
                            recordGeology.quantity = record.coal_tonnage == null ? 0 : record.coal_tonnage;
                            await dbContext.SaveChangesAsync();
                            if (approvalStatus == true)
                                if (approvalStatus != record.approved)
                                    return BadRequest("Approval Cannot be Undone.");

                            var history = new mineplan_ltp_history();
                            history.id = Guid.NewGuid().ToString("N");
                            history.created_by = CurrentUserContext.AppUserId;
                            history.created_on = record.model_date;
                            history.modified_by = null;
                            history.modified_on = null;
                            history.is_active = true;
                            history.is_default = null;
                            history.is_locked = null;
                            history.entity_id = null;
                            history.owner_id = CurrentUserContext.AppUserId;
                            history.organization_id = CurrentUserContext.OrganizationId;
                            history.header_id = record.id;

                            history.business_unit_id = record.business_unit_id;
                            history.waste_bcm = record.waste_bcm;
                            history.material_type_id = record.material_type_id;
                            history.reserve_type_id = record.reserve_type_id;
                            history.coal_tonnage = record.coal_tonnage;
                            history.tm_ar = record.tm_ar;
                            history.im_ar = record.im_ar;
                            history.ash_ar = record.ash_ar;
                            history.vm_ar = record.vm_ar;
                            history.fc_ar = record.fc_ar;
                            history.ts_ar = record.ts_ar;
                            history.gcv_adb_ar = record.gcv_adb_ar;
                            history.gcv_ar_ar = record.gcv_ar_ar;
                            history.hgi_ar = record.hgi_ar;
                            history.rdi_ar = record.rdi_ar;
                            history.rd_ar = record.rd_ar;
                            dbContext.mineplan_ltp_history.Add(history);
                            if (record.approved == true)
                            {
                                var result = await LTPMineLocationCreate(key);
                                await dbContext.SaveChangesAsync();
                                await tx.CommitAsync();
                            }
                            else
                            {
                                await dbContext.SaveChangesAsync();
                                await tx.CommitAsync();
                            }
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
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    return BadRequest(ex.Message);
                }

            }
            return Ok();
        }

        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var record = dbContext.mine_plan_ltp
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                var code = record.mine_code.ToUpper() + "_" + 
                    record.submine_code.ToUpper() + "_" + 
                    record.pit_code.ToUpper() + "_" + 
                    record.subpit_code.ToUpper() + "_" + 
                    record.contractor_code.ToUpper() + "_" + 
                    record.seam_code+"_"+
                    record.blok_code;
                var mineLocation = dbContext.mine_location.Where(o => o.mine_location_code == code).FirstOrDefault();
                var stockpileLocation = dbContext.stockpile_location.Where(o => o.stockpile_location_code == code && o.is_virtual == true).FirstOrDefault();
                /*
                var coalHauling = dbContext.hauling_transaction.Where(o => o.source_location_id == mineLocation.id).FirstOrDefault();
                if (coalMined != null) { return BadRequest("This Data Already Have a Transaction in Coal Hauling"); }*/
                var coalMined = dbContext.production_transaction.Where(o => o.source_location_id == mineLocation.id).FirstOrDefault();
                if (coalMined != null) { return BadRequest("Can not be Deleted. This Data Already Have a Transaction in Coal Mined"); }
                var wasteRemoval = dbContext.waste_removal.Where(o => o.source_location_id == mineLocation.id).FirstOrDefault();
                if (wasteRemoval != null) { return BadRequest("Can not be Deleted. This Data Already Have a Transaction in Waste Removal"); }
                if (record != null)
                {
                    dbContext.mine_plan_ltp.Remove(record);
                    await dbContext.SaveChangesAsync();
                }
                if(mineLocation != null) { dbContext.mine_location.Remove(mineLocation); await dbContext.SaveChangesAsync(); }
                if(stockpileLocation != null) { dbContext.stockpile_location.Remove(stockpileLocation); await dbContext.SaveChangesAsync(); }
                return Ok();
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
                var record = await dbContext.mine_plan_ltp
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                return Ok(record);
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
                var record = dbContext.mine_plan_ltp
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.mine_plan_ltp.Remove(record);
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

        [HttpPost("LTPMineLocationCreate")]
        public async Task<IActionResult> LTPMineLocationCreate(string key)
        {
            //Preparing Data to be Added on Mine Location and Model Geology.

            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await mcsContext.CanCreate(dbContext, nameof(mine_location),
                            CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                    {
                        var dataToBeAdded = dbContext.mine_plan_ltp
                            .Where(x => x.id == key)
                            .Where(x => x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();

                        if (dataToBeAdded == null)
                        {
                            return BadRequest("No LTP Data Found.");
                        }

                        if (dataToBeAdded.approved == true)
                        {
                            return BadRequest("LTP Mine Plan Data Already Approved");
                        }

                        var record = new mine_location();
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

                        var stockLocation = dataToBeAdded.blok_code + "-" + dataToBeAdded.strip_code;
                        var productId = string.Empty;
                        var uomId = "76dd627ffad44d74b9ce022f04677609"; // --> Permenently Added as MT.
                        var opening_date = dataToBeAdded.created_on.Value.Date;
                        var closing_date = dataToBeAdded.created_on.Value.Date.AddYears(30);
                        var mine_location_code = dataToBeAdded.mine_code + "_" + dataToBeAdded.submine_code + "_" + dataToBeAdded.pit_code + "_"
                                               + dataToBeAdded.subpit_code + "_" + dataToBeAdded.contractor_code + "_" + dataToBeAdded.seam_code;

                        record.stock_location_name = stockLocation;
                        record.product_id = productId;
                        record.uom_id = uomId;
                        record.opening_date = opening_date;
                        record.closing_date = closing_date;
                        record.mine_location_code = mine_location_code;

                        dbContext.mine_location.Add(record);
                        await dbContext.SaveChangesAsync();

                        if (await mcsContext.CanCreate(dbContext, nameof(model_geology),
                            CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                        {
                            var newRecord = new model_geology();
                            newRecord.id = Guid.NewGuid().ToString("N");
                            newRecord.created_by = CurrentUserContext.AppUserId;
                            newRecord.created_on = DateTime.Now;
                            newRecord.modified_by = null;
                            newRecord.modified_on = null;
                            newRecord.is_active = true;
                            newRecord.is_default = null;
                            newRecord.is_locked = null;
                            newRecord.entity_id = null;
                            newRecord.owner_id = CurrentUserContext.AppUserId;
                            newRecord.organization_id = CurrentUserContext.OrganizationId;
                            newRecord.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
                            newRecord.mine_location_id = record.id;
                            newRecord.quantity = dataToBeAdded.coal_tonnage;
                            newRecord.tm = dataToBeAdded.tm_ar;
                            newRecord.ts = dataToBeAdded.ts_ar;
                            newRecord.ash = dataToBeAdded.ash_ar;
                            newRecord.im = dataToBeAdded.im_ar;
                            newRecord.vm = dataToBeAdded.vm_ar;
                            newRecord.fc = dataToBeAdded.fc_ar;
                            newRecord.gcv_ar = dataToBeAdded.gcv_ar_ar;
                            newRecord.gcv_adb = dataToBeAdded.gcv_adb_ar;
                            newRecord.rd = dataToBeAdded.rd_ar;
                            newRecord.rdi = dataToBeAdded.rdi_ar;
                            newRecord.hgi = dataToBeAdded.hgi_ar;
                            newRecord.month_id = dataToBeAdded.created_on.Value.Month.ToString();
                            newRecord.year_id = dataToBeAdded.created_on.Value.Year.ToString();

                            dbContext.model_geology.Add(newRecord);
                            await dbContext.SaveChangesAsync();

                            dataToBeAdded.modified_by = CurrentUserContext.AppUserId;
                            dataToBeAdded.modified_on = DateTime.Now;
                            dataToBeAdded.approved = true;
                            dataToBeAdded.approved_by = CurrentUserContext.AppUserId;

                            await dbContext.SaveChangesAsync();
                            await tx.CommitAsync();
                        }
                        else
                        {
                            return BadRequest("User is not authorized.");
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
                return Ok();
            }
        }

        [HttpPost("DeleteSelectedRows")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteSelectedRows([FromBody] dynamic Data)
        {
            var result = new StandardResult();
            using (var tx = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (Data != null && Data.selectedIds != null)
                    {
                        var selectedIds = ((string)Data.selectedIds)
                            .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .ToList();

                        foreach (string key in selectedIds)
                        {
                            var record = dbContext.mine_plan_ltp
                                .Where(o => o.id == key
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                                .FirstOrDefault();
                            if (record != null)
                            {
                                dbContext.mine_plan_ltp.Remove(record);
                                await dbContext.SaveChangesAsync();
                            }
                        }

                        await tx.CommitAsync();

                        result.Success = true;
                        return Ok(result);
                    }
                    else
                    {
                        result.Message = "Invalid data.";
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    logger.Error(ex.ToString());
                    result.Message = ex.Message;
                }
            }

            return new JsonResult(result);
        }

        [HttpPost("UploadDocument")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> UploadDocument([FromBody] dynamic FileDocument)
        {
            var operationId = (string)FileDocument.operationId;
            using (await GlobalUploadQueue.EnterQueueAsync(operationId, _hubContext))
            {
                await _semaphore.WaitAsync();
                try
                {
                    await _hubContext.Clients.Group(operationId).SendAsync("QueueUpdate", -1);
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
                    int totalRows = sheet.LastRowNum - sheet.FirstRowNum;

                    var master_list = await dbContext.master_list.ToListAsync();
                    var business_unit = await dbContext.business_unit.ToListAsync();
                    var reserve_type = await dbContext.master_list.ToListAsync();
                    var vw_business_area = await dbContext.vw_business_area.ToListAsync();
                    var vw_contractor = await dbContext.vw_contractor.ToListAsync();
                    var mine_location = await dbContext.mine_location.ToListAsync();
                    var model_geology = await dbContext.model_geology.ToListAsync();
                    var mine_plan_ltp = await dbContext.mine_plan_ltp.ToListAsync();

                    using var transaction = await dbContext.Database.BeginTransactionAsync();
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        try
                        {
                            await _hubContext.Clients.Group(operationId).SendAsync("UpdateUploaderProgress", i - sheet.FirstRowNum, totalRows);
                            IRow row = sheet.GetRow(i);
                            if (row == null) continue;
                            if (row.Cells.Count() < 14) continue;

                            var material_type_id = "";
                            var a = master_list.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(7)).ToLower()).FirstOrDefault();
                            if (a != null) material_type_id = a.id.ToString();

                            var business_unit_id = "";
                            var b = business_unit.Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.business_unit_name.ToUpper() == PublicFunctions.IsNullCell(row.GetCell(22)).ToUpper()).FirstOrDefault();
                            if (b != null) business_unit_id = b.id.ToString();

                            var reserve_type_id = "";
                            var c = master_list
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                                    o.item_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(8)).ToLower()).FirstOrDefault();
                            if (c != null) reserve_type_id = c.id.ToString();

                            string mine = PublicFunctions.IsNullCell(row.GetCell(0));
                            string submine = PublicFunctions.IsNullCell(row.GetCell(1));
                            string pit = PublicFunctions.IsNullCell(row.GetCell(2));
                            string subpit = PublicFunctions.IsNullCell(row.GetCell(3));
                            string contractor = PublicFunctions.IsNullCell(row.GetCell(4));
                            string seam = PublicFunctions.IsNullCell(row.GetCell(5));
                            string blok = PublicFunctions.IsNullCell(row.GetCell(6));
                            var record = mine_plan_ltp
                                .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.mine_code == mine && o.submine_code == submine &&
                                o.pit_code == pit && o.subpit_code == subpit &&
                                o.contractor_code == contractor && o.seam_code == seam && o.blok_code == blok && o.material_type_id == material_type_id && o.reserve_type_id == reserve_type_id)
                                .FirstOrDefault();
                            // jika mine_plan_ltp != null maka update
                            if (record != null)
                            {
                                #region modify mine_plan_ltp
                                var e = new entity();
                                e.InjectFrom(record);

                                // record.InjectFrom(e);
                                record.modified_by = CurrentUserContext.AppUserId;
                                record.modified_on = DateTime.Now;

                                //record.name = PublicFunctions.IsNullCell(row.GetCell(0));
                                record.mine_code = PublicFunctions.IsNullCell(row.GetCell(0));
                                record.submine_code = PublicFunctions.IsNullCell(row.GetCell(1));
                                record.pit_code = PublicFunctions.IsNullCell(row.GetCell(2));
                                record.subpit_code = PublicFunctions.IsNullCell(row.GetCell(3));
                                record.contractor_code = PublicFunctions.IsNullCell(row.GetCell(4));
                                record.seam_code = PublicFunctions.IsNullCell(row.GetCell(5));
                                record.blok_code = PublicFunctions.IsNullCell(row.GetCell(6));
                                record.material_type_id = material_type_id;
                                record.reserve_type_id = reserve_type_id;
                                record.waste_bcm = PublicFunctions.Desimal(row.GetCell(9));
                                record.coal_tonnage = PublicFunctions.Desimal(row.GetCell(10));
                                record.tm_ar = PublicFunctions.Desimal(row.GetCell(11));
                                record.im_ar = PublicFunctions.Desimal(row.GetCell(12));
                                record.ash_ar = PublicFunctions.Desimal(row.GetCell(13));
                                record.vm_ar = PublicFunctions.Desimal(row.GetCell(14));
                                record.fc_ar = PublicFunctions.Desimal(row.GetCell(15));
                                record.ts_ar = PublicFunctions.Desimal(row.GetCell(16));
                                record.gcv_adb_ar = PublicFunctions.Desimal(row.GetCell(17));
                                record.gcv_ar_ar = PublicFunctions.Desimal(row.GetCell(18));
                                record.rd_ar = PublicFunctions.Desimal(row.GetCell(19));
                                record.rdi_ar = PublicFunctions.Desimal(row.GetCell(20));
                                record.hgi_ar = PublicFunctions.Desimal(row.GetCell(21));
                                record.business_unit_id = business_unit_id;
                                record.model_date = PublicFunctions.Tanggal(row.GetCell(23));
                                await dbContext.SaveChangesAsync();
                                #endregion
                                #region insert ltp history
                                var history = new mineplan_ltp_history();
                                history.id = Guid.NewGuid().ToString("N");
                                history.created_by = CurrentUserContext.AppUserId;
                                history.created_on = record.model_date;
                                history.modified_by = null;
                                history.modified_on = null;
                                history.is_active = true;
                                history.is_default = null;
                                history.is_locked = null;
                                history.entity_id = null;
                                history.owner_id = CurrentUserContext.AppUserId;
                                history.organization_id = CurrentUserContext.OrganizationId;
                                history.header_id = record.id;

                                history.business_unit_id = record.business_unit_id;
                                history.waste_bcm = record.waste_bcm;
                                history.material_type_id = record.material_type_id;
                                history.reserve_type_id = record.reserve_type_id;
                                history.coal_tonnage = record.coal_tonnage;
                                history.tm_ar = record.tm_ar;
                                history.im_ar = record.im_ar;
                                history.ash_ar = record.ash_ar;
                                history.vm_ar = record.vm_ar;
                                history.fc_ar = record.fc_ar;
                                history.ts_ar = record.ts_ar;
                                history.gcv_adb_ar = record.gcv_adb_ar;
                                history.gcv_ar_ar = record.gcv_ar_ar;
                                history.hgi_ar = record.hgi_ar;
                                history.rdi_ar = record.rdi_ar;
                                history.rd_ar = record.rd_ar;
                                dbContext.mineplan_ltp_history.Add(history);
                                await dbContext.SaveChangesAsync();
                                #endregion
                                var code3 = record.mine_code.ToUpper() + "_" + record.submine_code.ToUpper() + "_"
                               + record.pit_code.ToUpper() + "_" + record.subpit_code.ToUpper() + "_" + record.contractor_code.ToUpper() + "_" + record.seam_code.ToUpper();

                                var businessArea = vw_business_area
                                                .Where(o => o.business_area_code == code3)
                                                .Select(o => o.id).FirstOrDefault();
                                var mineCode1 = code3 + "_" + record.blok_code.ToUpper();
                                var contractorId = vw_contractor.Where(o => o.business_partner_code == record.contractor_code).FirstOrDefault();
                                var mineLocation1 = mine_location.Where(o => o.mine_location_code == mineCode1).FirstOrDefault();
                                if (mineLocation1 != null)
                                {
                                    #region modify MineLocation
                                    //mineLocation1.business_area_id = businessArea;
                                    // mineLocation1.mine_location_code = code3 + "_" + record.blok_code;
                                    //mineLocation1.stock_location_name = record.blok_code;
                                    //mineLocation1.opening_date = record.created_on;
                                    //id = mineLocation1.id;
                                    mineLocation1.mine_plan_ltp_id = record.id;
                                    if (contractorId != null)
                                    {
                                        mineLocation1.contractor_id = contractorId.id;
                                    }
                                    await dbContext.SaveChangesAsync();
                                    #endregion
                                    var recordGeology1 = model_geology.Where(o => o.mine_location_id == mineLocation1.id && o.material_type_id == material_type_id).FirstOrDefault();
                                    //var month1 = Convert.ToString(Convert.ToDateTime(record.created_on).Month);
                                    // var year1 = Convert.ToString(Convert.ToDateTime(record.created_on).Year);
                                    //var masterlist1 = dbContext.master_list.Where(o => o.item_group == "years" && o.item_name == year1).Select(o => o.id).FirstOrDefault();
                                    if (recordGeology1 != null)
                                    {
                                        #region modify geology mine location
                                        // recordGeology1.year_id = masterlist1;
                                        // recordGeology1.month_id = month1;
                                        recordGeology1.created_on = PublicFunctions.Tanggal(row.GetCell(23));
                                        recordGeology1.waste_bcm = Convert.ToDecimal(record.waste_bcm);
                                        recordGeology1.im = record.im_ar;
                                        recordGeology1.tm = record.tm_ar;
                                        recordGeology1.ts = record.ts_ar;
                                        recordGeology1.fc = record.fc_ar;
                                        recordGeology1.ash = record.ash_ar;
                                        recordGeology1.vm = record.vm_ar;
                                        recordGeology1.gcv_ar = record.gcv_ar_ar;
                                        recordGeology1.gcv_adb = record.gcv_adb_ar;
                                        recordGeology1.rdi = record.rdi_ar;
                                        recordGeology1.hgi = record.hgi_ar;
                                        recordGeology1.rd = record.rd_ar;
                                        recordGeology1.quantity = record.coal_tonnage == null ? 0 : record.coal_tonnage;
                                        recordGeology1.material_type_id = record.material_type_id;
                                        recordGeology1.mine_location_id = mineLocation1.id;
                                        await dbContext.SaveChangesAsync();
                                        #endregion
                                    }
                                    else
                                    {
                                        #region insert new model geology mine location
                                        var recordGeology = new model_geology();
                                        recordGeology.id = Guid.NewGuid().ToString("N");
                                        recordGeology.created_by = CurrentUserContext.AppUserId;
                                        recordGeology.created_on = PublicFunctions.Tanggal(row.GetCell(23));
                                        recordGeology.modified_by = null;
                                        recordGeology.modified_on = null;
                                        recordGeology.is_active = true;
                                        recordGeology.is_default = null;
                                        recordGeology.is_locked = null;
                                        recordGeology.entity_id = null;
                                        recordGeology.owner_id = CurrentUserContext.AppUserId;
                                        recordGeology.organization_id = CurrentUserContext.OrganizationId;
                                        recordGeology.business_unit_id = record.business_unit_id;
                                        // recordGeology.year_id = masterlist1;
                                        // recordGeology.month_id = month1;
                                        recordGeology.waste_bcm = Convert.ToDecimal(record.waste_bcm);
                                        recordGeology.im = record.im_ar;
                                        recordGeology.tm = record.tm_ar;
                                        recordGeology.ts = record.ts_ar;
                                        recordGeology.fc = record.fc_ar;
                                        recordGeology.ash = record.ash_ar;
                                        recordGeology.vm = record.vm_ar;
                                        recordGeology.gcv_ar = record.gcv_ar_ar;
                                        recordGeology.gcv_adb = record.gcv_adb_ar;
                                        recordGeology.rdi = record.rdi_ar;
                                        recordGeology.hgi = record.hgi_ar;
                                        recordGeology.rd = record.rd_ar;
                                        recordGeology.quantity = record.coal_tonnage == null ? 0 : record.coal_tonnage;
                                        recordGeology.material_type_id = record.material_type_id;
                                        recordGeology.mine_location_id = mineLocation1.id;
                                        dbContext.model_geology.Add(recordGeology);
                                        await dbContext.SaveChangesAsync();
                                        #endregion
                                    }
                                }
                            }
                            else
                            {
                                #region create new mine plan ltp record
                                record = new mine_plan_ltp();
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

                                record.mine_code = PublicFunctions.IsNullCell(row.GetCell(0));
                                record.submine_code = PublicFunctions.IsNullCell(row.GetCell(1));
                                record.pit_code = PublicFunctions.IsNullCell(row.GetCell(2));
                                record.subpit_code = PublicFunctions.IsNullCell(row.GetCell(3));
                                record.contractor_code = PublicFunctions.IsNullCell(row.GetCell(4));
                                record.seam_code = PublicFunctions.IsNullCell(row.GetCell(5));
                                record.blok_code = PublicFunctions.IsNullCell(row.GetCell(6));
                                record.material_type_id = material_type_id;
                                record.reserve_type_id = reserve_type_id;
                                record.waste_bcm = PublicFunctions.Desimal(row.GetCell(9));
                                record.coal_tonnage = PublicFunctions.Desimal(row.GetCell(10));
                                record.tm_ar = PublicFunctions.Desimal(row.GetCell(11));
                                record.im_ar = PublicFunctions.Desimal(row.GetCell(12));
                                record.ash_ar = PublicFunctions.Desimal(row.GetCell(13));
                                record.vm_ar = PublicFunctions.Desimal(row.GetCell(14));
                                record.fc_ar = PublicFunctions.Desimal(row.GetCell(15));
                                record.ts_ar = PublicFunctions.Desimal(row.GetCell(16));
                                record.gcv_adb_ar = PublicFunctions.Desimal(row.GetCell(17));
                                record.gcv_ar_ar = PublicFunctions.Desimal(row.GetCell(18));
                                record.rd_ar = PublicFunctions.Desimal(row.GetCell(19));
                                record.rdi_ar = PublicFunctions.Desimal(row.GetCell(20));
                                record.hgi_ar = PublicFunctions.Desimal(row.GetCell(21));
                                record.business_unit_id = business_unit_id;
                                record.model_date = PublicFunctions.Tanggal(row.GetCell(23));

                                dbContext.mine_plan_ltp.Add(record);
                                await dbContext.SaveChangesAsync();
                                #endregion
                                #region create new history for mineplan ltp
                                //add history to mineplan ltp
                                var history = new mineplan_ltp_history();
                                history.id = Guid.NewGuid().ToString("N");
                                history.created_by = CurrentUserContext.AppUserId;
                                history.created_on = record.model_date;
                                history.modified_by = null;
                                history.modified_on = null;
                                history.is_active = true;
                                history.is_default = null;
                                history.is_locked = null;
                                history.entity_id = null;
                                history.owner_id = CurrentUserContext.AppUserId;
                                history.organization_id = CurrentUserContext.OrganizationId;
                                history.header_id = record.id;

                                history.business_unit_id = record.business_unit_id;
                                history.waste_bcm = record.waste_bcm;
                                history.material_type_id = record.material_type_id;
                                history.reserve_type_id = record.reserve_type_id;
                                history.coal_tonnage = record.coal_tonnage;
                                history.tm_ar = record.tm_ar;
                                history.im_ar = record.im_ar;
                                history.ash_ar = record.ash_ar;
                                history.vm_ar = record.vm_ar;
                                history.fc_ar = record.fc_ar;
                                history.ts_ar = record.ts_ar;
                                history.gcv_adb_ar = record.gcv_adb_ar;
                                history.gcv_ar_ar = record.gcv_ar_ar;
                                history.hgi_ar = record.hgi_ar;
                                history.rdi_ar = record.rdi_ar;
                                history.rd_ar = record.rd_ar;
                                dbContext.mineplan_ltp_history.Add(history);
                                await dbContext.SaveChangesAsync();
                                #endregion
                                var code1 = record.mine_code.ToUpper() + "_" + record.submine_code.ToUpper() + "_"
                               + record.pit_code.ToUpper() + "_" + record.subpit_code.ToUpper() + "_" + record.contractor_code.ToUpper() + "_" + record.seam_code.ToUpper();
                                var mineCode2 = code1 + "_" + record.blok_code.ToUpper();
                                var businessArea = vw_business_area
                                                            .Where(o => o.business_area_code == code1)
                                                            .Select(o => o.id).FirstOrDefault();
                                var contractorId = vw_contractor.Where(o => o.business_partner_code == record.contractor_code).FirstOrDefault();
                                var mineLocation2 = mine_location.Where(o => o.mine_location_code == mineCode2).FirstOrDefault();
                                if (mineLocation2 != null)
                                {
                                    #region modify mineloc
                                    // mineLocation2.business_area_id = businessArea;
                                    // mineLocation2.mine_location_code = code1 + "_" + record.blok_code;
                                    // mineLocation2.stock_location_name = record.blok_code;
                                    // mineLocation2.opening_date = record.created_on;
                                    if (contractorId != null)
                                    {
                                        mineLocation2.contractor_id = contractorId.id;
                                    }

                                    mineLocation2.mine_plan_ltp_id = record.id;
                                    await dbContext.SaveChangesAsync();
                                    #endregion
                                    var recordGeology1 = model_geology.Where(o => o.mine_location_id == mineLocation2.id && o.material_type_id == material_type_id).FirstOrDefault();
                                    //var month1 = Convert.ToString(Convert.ToDateTime(record.created_on).Month);
                                    //var year1 = Convert.ToString(Convert.ToDateTime(record.created_on).Year);
                                    //var masterlist1 = dbContext.master_list.Where(o => o.item_group == "years" && o.item_name == year1).Select(o => o.id).FirstOrDefault();

                                    if (recordGeology1 != null)
                                    {
                                        #region update new model geology mine loc
                                        //recordGeology1.year_id = masterlist1;
                                        //recordGeology1.month_id = month1;
                                        recordGeology1.created_on = PublicFunctions.Tanggal(row.GetCell(23));
                                        recordGeology1.waste_bcm = Convert.ToDecimal(record.waste_bcm);
                                        recordGeology1.im = record.im_ar;
                                        recordGeology1.tm = record.tm_ar;
                                        recordGeology1.ts = record.ts_ar;
                                        recordGeology1.fc = record.fc_ar;
                                        recordGeology1.ash = record.ash_ar;
                                        recordGeology1.vm = record.vm_ar;
                                        recordGeology1.gcv_ar = record.gcv_ar_ar;
                                        recordGeology1.gcv_adb = record.gcv_adb_ar;
                                        recordGeology1.rdi = record.rdi_ar;
                                        recordGeology1.hgi = record.hgi_ar;
                                        recordGeology1.rd = record.rd_ar;
                                        recordGeology1.quantity = record.coal_tonnage == null ? 0 : record.coal_tonnage;
                                        recordGeology1.material_type_id = record.material_type_id;
                                        recordGeology1.mine_location_id = mineLocation2.id;
                                        await dbContext.SaveChangesAsync();
                                        #endregion
                                    }
                                    else
                                    {
                                        #region create new model geology mine loc
                                        var recordGeology = new model_geology();
                                        recordGeology.id = Guid.NewGuid().ToString("N");
                                        recordGeology.created_by = CurrentUserContext.AppUserId;
                                        recordGeology.created_on = PublicFunctions.Tanggal(row.GetCell(23));
                                        recordGeology.modified_by = null;
                                        recordGeology.modified_on = null;
                                        recordGeology.is_active = true;
                                        recordGeology.is_default = null;
                                        recordGeology.is_locked = null;
                                        recordGeology.entity_id = null;
                                        recordGeology.owner_id = CurrentUserContext.AppUserId;
                                        recordGeology.organization_id = CurrentUserContext.OrganizationId;
                                        recordGeology.business_unit_id = record.business_unit_id;
                                        // recordGeology.year_id = masterlist1;
                                        // recordGeology.month_id = month1;
                                        recordGeology.waste_bcm = Convert.ToDecimal(record.waste_bcm);
                                        recordGeology.im = record.im_ar;
                                        recordGeology.tm = record.tm_ar;
                                        recordGeology.ts = record.ts_ar;
                                        recordGeology.fc = record.fc_ar;
                                        recordGeology.ash = record.ash_ar;
                                        recordGeology.vm = record.vm_ar;
                                        recordGeology.gcv_ar = record.gcv_ar_ar;
                                        recordGeology.gcv_adb = record.gcv_adb_ar;
                                        recordGeology.rdi = record.rdi_ar;
                                        recordGeology.hgi = record.hgi_ar;
                                        recordGeology.rd = record.rd_ar;
                                        recordGeology.quantity = record.coal_tonnage == null ? 0 : record.coal_tonnage;
                                        recordGeology.material_type_id = record.material_type_id;
                                        recordGeology.mine_location_id = mineLocation2.id;
                                        dbContext.model_geology.Add(recordGeology);
                                        await dbContext.SaveChangesAsync();
                                        #endregion
                                    }

                                }
                                else
                                {
                                    string id;
                                    #region create new mine location
                                    var recordMineLocation = new mine_location();
                                    //JsonConvert.PopulateObject(values, record);
                                    recordMineLocation.id = Guid.NewGuid().ToString("N");
                                    recordMineLocation.created_by = CurrentUserContext.AppUserId;
                                    recordMineLocation.created_on = DateTime.Now;
                                    recordMineLocation.modified_by = null;
                                    recordMineLocation.modified_on = null;
                                    recordMineLocation.is_active = true;
                                    recordMineLocation.is_default = null;
                                    recordMineLocation.is_locked = null;
                                    recordMineLocation.entity_id = null;
                                    recordMineLocation.owner_id = CurrentUserContext.AppUserId;
                                    recordMineLocation.organization_id = CurrentUserContext.OrganizationId;
                                    recordMineLocation.business_unit_id = record.business_unit_id;
                                    recordMineLocation.ready_to_get = true;
                                    if (contractorId != null)
                                    {
                                        recordMineLocation.contractor_id = contractorId.id;
                                    }
                                    recordMineLocation.business_area_id = businessArea;
                                    recordMineLocation.mine_location_code = code1 + "_" + record.blok_code;
                                    recordMineLocation.stock_location_name = record.blok_code;
                                    recordMineLocation.opening_date = record.created_on;
                                    recordMineLocation.mine_plan_ltp_id = record.id;
                                    id = recordMineLocation.id;
                                    recordMineLocation.mine_plan_ltp_id = record.id;
                                    if (contractorId != null)
                                    {
                                        recordMineLocation.contractor_id = contractorId.id;
                                    }
                                    dbContext.mine_location.Add(recordMineLocation);
                                    #endregion
                                    #region create new model geology mine loc
                                    var recordGeology = new model_geology();
                                    //  var month = Convert.ToString(Convert.ToDateTime(record.created_on).Month);
                                    // var year = Convert.ToString(Convert.ToDateTime(record.created_on).Year);
                                    // var masterlist = dbContext.master_list.Where(o => o.item_group == "years" && o.item_name == year).Select(o => o.id).FirstOrDefault();

                                    recordGeology.id = Guid.NewGuid().ToString("N");
                                    recordGeology.created_by = CurrentUserContext.AppUserId;
                                    recordGeology.created_on = PublicFunctions.Tanggal(row.GetCell(23));
                                    recordGeology.modified_by = null;
                                    recordGeology.modified_on = null;
                                    recordGeology.is_active = true;
                                    recordGeology.is_default = null;
                                    recordGeology.is_locked = null;
                                    recordGeology.entity_id = null;
                                    recordGeology.owner_id = CurrentUserContext.AppUserId;
                                    recordGeology.organization_id = CurrentUserContext.OrganizationId;
                                    recordGeology.business_unit_id = record.business_unit_id;

                                    // recordGeology.year_id = masterlist;
                                    //  recordGeology.month_id = month;
                                    recordGeology.waste_bcm = Convert.ToDecimal(record.waste_bcm);
                                    recordGeology.im = record.im_ar;
                                    recordGeology.tm = record.tm_ar;
                                    recordGeology.ts = record.ts_ar;
                                    recordGeology.fc = record.fc_ar;
                                    recordGeology.ash = record.ash_ar;
                                    recordGeology.vm = record.vm_ar;
                                    recordGeology.gcv_ar = record.gcv_ar_ar;
                                    recordGeology.gcv_adb = record.gcv_adb_ar;
                                    recordGeology.quantity = record.coal_tonnage;
                                    recordGeology.material_type_id = material_type_id;
                                    recordGeology.mine_location_id = id;
                                    dbContext.model_geology.Add(recordGeology);
                                    await dbContext.SaveChangesAsync();
                                    #endregion
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
                        HttpContext.Session.SetString("filename", "MinePlanLTP");
                        return BadRequest("File gagal di-upload");
                    }
                    else
                    {
                        await transaction.CommitAsync();
                        return "File berhasil di-upload!";
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        [HttpGet("HistoryById")]
        public async Task<object> HistoryById(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_mineplan_ltp_history
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.header_id == Id),
               // .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }
        //[HttpPost("LTPMineLocationUpdate")]
        //public async Task<IActionResult> LTPMineLocationUpdate(string key, string values)
        //{
        //    //Preparing Data to be Updated on Mine Location and Model Geology.

        //    logger.Trace($"string values = {values}");

        //    using (var tx = await dbContext.Database.BeginTransactionAsync())
        //    {
        //        var record = dbContext.mine_location
        //            .Where(o => o.mine_location_code == key)
        //            .FirstOrDefault();

        //        if (record != null) 
        //        {
        //            try
        //            {
        //                if (await mcsContext.CanUpdate(dbContext, nameof(mine_location),
        //                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
        //                {
        //                    var dataToBeAdded = new mine_plan_ltp();
        //                    JsonConvert.PopulateObject(values, dataToBeAdded);

        //                    var e = new entity();
        //                    e.InjectFrom(record);

        //                    JsonConvert.PopulateObject(values, record);

        //                    record.InjectFrom(e);
        //                    record.modified_by = CurrentUserContext.AppUserId;
        //                    record.modified_on = DateTime.Now;

        //                    var stockLocation = dataToBeAdded.blok_code + "-" + dataToBeAdded.strip_code;
        //                    var productId = string.Empty;
        //                    var uomId = "76dd627ffad44d74b9ce022f04677609"; // --> Permenently Added as MT.
        //                    var opening_date = dataToBeAdded.created_on.Value.Date;
        //                    var closing_date = dataToBeAdded.created_on.Value.Date.AddYears(30);
        //                    var mine_location_code = dataToBeAdded.mine_code + "_" + dataToBeAdded.submine_code + "_" + dataToBeAdded.pit_code + "_"
        //                                           + dataToBeAdded.subpit_code + "_" + dataToBeAdded.contractor_code + "_" + dataToBeAdded.seam_code;

        //                    record.stock_location_name = stockLocation;
        //                    record.product_id = productId;
        //                    record.uom_id = uomId;
        //                    record.opening_date = opening_date;
        //                    record.closing_date = closing_date;
        //                    record.mine_location_code = mine_location_code;

        //                    dbContext.mine_location.Add(record);
        //                    await dbContext.SaveChangesAsync();

        //                    if (await mcsContext.CanUpdate(dbContext, nameof(model_geology),
        //                        CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
        //                    {
        //                        var newRecord = new model_geology();
        //                        newRecord.id = Guid.NewGuid().ToString("N");
        //                        newRecord.created_by = CurrentUserContext.AppUserId;
        //                        newRecord.created_on = DateTime.Now;
        //                        newRecord.modified_by = null;
        //                        newRecord.modified_on = null;
        //                        newRecord.is_active = true;
        //                        newRecord.is_default = null;
        //                        newRecord.is_locked = null;
        //                        newRecord.entity_id = null;
        //                        newRecord.owner_id = CurrentUserContext.AppUserId;
        //                        newRecord.organization_id = CurrentUserContext.OrganizationId;
        //                        newRecord.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");
        //                        newRecord.mine_location_id = record.id;
        //                        newRecord.quantity = dataToBeAdded.coal_tonnage;
        //                        newRecord.tm = dataToBeAdded.tm_ar;
        //                        newRecord.ts = dataToBeAdded.ts_ar;
        //                        newRecord.ash = dataToBeAdded.ash_ar;
        //                        newRecord.im = dataToBeAdded.im_ar;
        //                        newRecord.vm = dataToBeAdded.vm_ar;
        //                        newRecord.fc = dataToBeAdded.fc_ar;
        //                        newRecord.gcv_ar = dataToBeAdded.gcv_ar_ar;
        //                        newRecord.gcv_adb = dataToBeAdded.gcv_adb_ar;
        //                        newRecord.rd = dataToBeAdded.rd_ar;
        //                        newRecord.rdi = dataToBeAdded.rdi_ar;
        //                        newRecord.hgi = dataToBeAdded.hgi_ar;
        //                        newRecord.month_id = dataToBeAdded.created_on.Value.Month.ToString();
        //                        newRecord.year_id = dataToBeAdded.created_on.Value.Year.ToString();

        //                        dbContext.model_geology.Add(newRecord);
        //                        await dbContext.SaveChangesAsync();
        //                        await tx.CommitAsync();
        //                    }
        //                    else
        //                    {
        //                        return BadRequest("User is not authorized.");
        //                    }
        //                }
        //                else
        //                {
        //                    return BadRequest("User is not authorized.");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                logger.Error(ex.InnerException ?? ex);
        //                return BadRequest(ex.InnerException?.Message ?? ex.Message);
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest("No default organization");
        //        }
        //        return Ok(values);
        //    }
        //}

    }
}

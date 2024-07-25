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
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.EntityFrameworkCore;
using Common;

namespace MCSWebApp.Controllers.API.DailyRecord
{
    [Route("api/DailyRecord/[controller]")]
    [ApiController]
    public class DailyStockPortController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public DailyStockPortController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_daily_stock_port
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId),
                    loadOptions);
            }

            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.vw_daily_stock_port
                .Where(o =>
                    o.cargo_date >= dt1
                    && o.cargo_date <= dt2
                    && o.organization_id == CurrentUserContext.OrganizationId),
                loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_daily_stock_port.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
				if (await mcsContext.CanCreate(dbContext, nameof(daily_stock_port),
					CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
				{
                    var record = new daily_stock_port();
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
					record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    dbContext.daily_stock_port.Add(record);
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
                var record = dbContext.daily_stock_port
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        JsonConvert.PopulateObject(values, record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        await dbContext.SaveChangesAsync();
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
                var record = dbContext.daily_stock_port
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.daily_stock_port.Remove(record);
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

        [HttpGet("GetLastEndingByDate/{cargoDate}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetLastEndingByDate(string cargoDate)
        {
            try
            {
                dynamic dt = Convert.ToDateTime(cargoDate);
                if (dt.Day == 1)
                {
                    dt = dt.AddDays(-1);

                    dt = Convert.ToDateTime(dt).ToString("yyyy-MM-dd");

                    string sql = string.Format("select coalesce(sum(js.quantity), 0) as quantity from joint_survey js join vw_port_location pl " +
                        "on js.location_id = pl.id where js.organization_id = '{0}' and upper(trim(pl.business_area_name)) like 'PORT STOCKPILE' " +
                        "and js.join_survey_date::date = '{1}'",
                            CurrentUserContext.OrganizationId, dt);

                    var record = await dbContext.barging_transaction.FromSqlRaw(sql)
                        .Select(o => new { ending = o.quantity }).FirstOrDefaultAsync();

                    return Ok(record);
                }
                else
                {
                    var record = await dbContext.vw_daily_stock_port
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
                            o.cargo_date < Convert.ToDateTime(cargoDate))
                        .OrderByDescending(o => o.cargo_date)
                        .Select(o => new { ending = o.ending })
                        .FirstOrDefaultAsync();

                    return Ok(record);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("GetHaulingByDate/{tanggal1}/{tanggal2}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetHaulingByDate(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            return await DataSourceLoader.LoadAsync(dbContext.vw_hauling_transaction
                .Where(o => o.loading_datetime >= dt1
                    && o.loading_datetime <= dt2
                    && o.organization_id == CurrentUserContext.OrganizationId)
                .GroupBy(o => o.organization_id)
                .Select(o =>
                    new
                    {
                        loading_quantity = o.Sum(p => p.loading_quantity ?? 0)
                    }),
                loadOptions);
        }

        [HttpGet("GetBargingByDate/{tanggal1}/{tanggal2}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetBargingByDate(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            var dt1 = Convert.ToDateTime(tanggal1).ToString("yyyy-MM-dd HH:mm").Replace('.', ':');
            var dt2 = Convert.ToDateTime(tanggal2).ToString("yyyy-MM-dd HH:mm").Replace('.', ':');

            string sql = string.Format("select coalesce(sum(quantity), 0) quantity from (select sum(bt.quantity) quantity from " +
                "barging_transaction bt left join despatch_order ds on bt.despatch_order_id = ds.id left join master_list ml on " +
                "ds.delivery_term_id = ml.id where bt.end_datetime >= '{1}' and bt.end_datetime < '{2}' and bt.is_loading is true " +
                "and ml.item_name like '%BARGE%' and bt.organization_id = '{0}' union select sum(st.quantity) quantity from " +
                "shipping_transaction_detail st where st.end_datetime >= '{1}' and st.end_datetime < '{2}' and st.organization_id = '{0}') as tbl",
                    CurrentUserContext.OrganizationId, dt1, dt2);

            var lookup = dbContext.barging_transaction.FromSqlRaw(sql)
                .Select(o => new { quantity = o.quantity });
            return await DataSourceLoader.LoadAsync(lookup, loadOptions);
        }

        [HttpGet("GetReturnCargoByDate/{tanggal1}/{tanggal2}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetReturnCargoByDate(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            string dt1 = Convert.ToDateTime(tanggal1).ToString("yyyy-MM-dd HH:mm").Replace('.', ':');
            string dt2 = Convert.ToDateTime(tanggal2).ToString("yyyy-MM-dd HH:mm").Replace('.', ':');

            string sql = string.Format("select coalesce(sum(std.final_quantity), 0) final_quantity from shipping_transaction st left join " +
                "shipping_transaction_detail std on st.id =  std.shipping_transaction_id where st.end_datetime >= '{1}' and " +
                "st.end_datetime < '{2}' and st.is_loading is true and st.organization_id = '{0}' and COALESCE(std.final_quantity, 0) > 0",
                    CurrentUserContext.OrganizationId, dt1, dt2);

            var lookup = dbContext.shipping_transaction_detail.FromSqlRaw(sql)
                .Select(o => new { quantity = o.final_quantity });
            return await DataSourceLoader.LoadAsync(lookup, loadOptions);
        }

        [HttpGet("GetAdjustmentByDate/{tanggal1}/{tanggal2}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetAdjustmentByDate(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            string dt1 = Convert.ToDateTime(tanggal1).ToString("yyyy-MM-dd");
            string dt2 = Convert.ToDateTime(tanggal2).ToString("yyyy-MM-dd");

            string sql = string.Concat("with cteBarge as ( select DISTINCT st.despatch_order_id, des.required_quantity ",
                "from shipping_transaction_detail std left join shipping_transaction st on ",
                "std.shipping_transaction_id = st.id left join despatch_order des on st.despatch_order_id = des.id ",
                "where std.organization_id = '",  CurrentUserContext.OrganizationId, 
                "' and std.end_datetime::date >= '", dt1, "' and std.end_datetime::date < '", dt2, "'),", 
                "cteBargeDetail as (",
                "select bg.despatch_order_id, std.quantity, COALESCE(std.final_quantity,0) qty_rc, ",
                "bg.required_quantity from cteBarge bg ",
                "left join shipping_transaction st on bg.despatch_order_id = st.despatch_order_id ",
                "left join shipping_transaction_detail std on st.id = std.shipping_transaction_id ",
                "where std.end_datetime::date >= '", dt1, "'), ",
                "cteBargeAdj as (",
                "select despatch_order_id, sum(quantity) qty_barge, sum(qty_rc) qty_rc, ",
                "avg(required_quantity) qty_vessel ",
                "from cteBargeDetail ",
                "group by despatch_order_id),",
                "cteadj as (",
                "select despatch_order_id, qty_barge, qty_rc, qty_vessel,",
                "qty_vessel-qty_barge-qty_rc adj ",
                "from cteBargeAdj) ",
                "select sum(adj) quantity from cteadj"); 

            var lookup = dbContext.shipping_transaction_detail.FromSqlRaw(sql)
                .Select(o => new { quantity = o.quantity });
            return await DataSourceLoader.LoadAsync(lookup, loadOptions);
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
                    if (row.Cells.Count() < 6) continue;

                    var record = dbContext.daily_stock_port
                        .Where(o => o.cargo_date == Convert.ToDateTime(row.GetCell(0).ToString()).Date
                                    && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        
                        record.beginning = PublicFunctions.Pecahan(row.GetCell(1));
                        record.hauling = PublicFunctions.Pecahan(row.GetCell(2));
                        record.processing = PublicFunctions.Pecahan(row.GetCell(3));
                        record.return_cargo = PublicFunctions.Pecahan(row.GetCell(4));
                        record.loading_to_barge = PublicFunctions.Pecahan(row.GetCell(5));
                        record.adjustment = PublicFunctions.Pecahan(row.GetCell(6));
                        record.ending = PublicFunctions.Pecahan(row.GetCell(7));
                        record.hauling_from_date = PublicFunctions.Tanggal(row.GetCell(8));
                        record.hauling_to_date = PublicFunctions.Tanggal(row.GetCell(9));
                        record.barging_from_date = PublicFunctions.Tanggal(row.GetCell(10));
                        record.barging_to_date = PublicFunctions.Tanggal(row.GetCell(11));

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new daily_stock_port();
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

                        record.cargo_date = PublicFunctions.Tanggal(row.GetCell(0));
                        record.beginning = PublicFunctions.Pecahan(row.GetCell(1));
                        record.hauling = PublicFunctions.Pecahan(row.GetCell(2));
                        record.processing = PublicFunctions.Pecahan(row.GetCell(3));
                        record.return_cargo = PublicFunctions.Pecahan(row.GetCell(4));
                        record.loading_to_barge = PublicFunctions.Pecahan(row.GetCell(5));
                        record.adjustment = PublicFunctions.Pecahan(row.GetCell(6));
                        record.ending = PublicFunctions.Pecahan(row.GetCell(7));
                        record.hauling_from_date = PublicFunctions.Tanggal(row.GetCell(8));
                        record.hauling_to_date = PublicFunctions.Tanggal(row.GetCell(9));
                        record.barging_from_date = PublicFunctions.Tanggal(row.GetCell(10));
                        record.barging_to_date = PublicFunctions.Tanggal(row.GetCell(11));

                        dbContext.daily_stock_port.Add(record);
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
                HttpContext.Session.SetString("filename", "DailyCargo");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpGet("Detail/{Id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Detail(string Id)
        {
            if (await mcsContext.CanRead(dbContext, Id, CurrentUserContext.AppUserId)
                || CurrentUserContext.IsSysAdmin)
            {
                try
                {
                    var record = await dbContext.vw_daily_stock_port
                        .Where(o => o.id == Id
                            && (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                                || CurrentUserContext.IsSysAdmin))
                        .FirstOrDefaultAsync();
                    return Ok(record);
                }
				catch (Exception ex)
				{
					logger.Error(ex.InnerException ?? ex);
					return BadRequest(ex.InnerException?.Message ?? ex.Message);
				}
            }
            else
            {
                return BadRequest("User is not authorized.");
            }
        }

        [HttpDelete("DeleteById/{Id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Trace($"string Id = {Id}");

            if (await mcsContext.CanDelete(dbContext, Id, CurrentUserContext.AppUserId)
                || CurrentUserContext.IsSysAdmin)
            {
                try
                {
                    var record = dbContext.daily_stock_port
                        .Where(o => o.id == Id)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        dbContext.daily_stock_port.Remove(record);
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
            else
            {
                return BadRequest("User is not authorized.");
            }
        }
    }
}

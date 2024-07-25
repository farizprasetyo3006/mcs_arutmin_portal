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

namespace MCSWebApp.Controllers.API.StockpileManagement
{
    [Route("api/StockpileManagement/[controller]")]
    [ApiController]
    public class StockpileMutationController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public StockpileMutationController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            //return await DataSourceLoader.LoadAsync(dbContext.mv_stockpile_mutation
            //    .Where(o =>
            //        o.trans_date >= dt1
            //        && o.trans_date <= dt2
            //        && o.organization_id == CurrentUserContext.OrganizationId
            //        && (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
            //            || CurrentUserContext.IsSysAdmin)),
            //    loadOptions);

            return await DataSourceLoader.LoadAsync(dbContext.mv_stockpile_mutation_kmia
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.trans_date >= dt1 && o.trans_date <= dt2),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string StockLocationId, string tanggal, long rowNumber, DataSourceLoadOptions loadOptions)
        {
            var dt = Convert.ToDateTime(tanggal).Date;

            return await DataSourceLoader.LoadAsync(
                dbContext.mv_stockpile_mutation_quality_kmia
                    .Where(o => o.trans_date.Value.Date == dt
                        && o.stock_location_id == StockLocationId
                        && o.quality_row_number == rowNumber),
                    loadOptions);

            //tanggal = tanggal.Substring(0, 10);

            //string sql = string.Format("select * from mv_stockpile_mutation_quality where stock_location_id = '{0}' " +
            //    "and trans_date::date = '{1}' and quality_row_number = {2}",
            //        StockLocationId, tanggal, rowNumber);

            //return await DataSourceLoader.LoadAsync(
            //    dbContext.mv_stockpile_mutation_quality.FromSqlRaw(sql), loadOptions);
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

            string teks = "==>Sheet 1" + Environment.NewLine;
            bool gagal = false; string errormessage = "";

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var stock_location_id = "";
                    var stock_location = dbContext.stock_location
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && 
                            o.stock_location_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()).FirstOrDefault();
                    if (stock_location != null) stock_location_id = stock_location.id.ToString();

                    var record = dbContext.stockpile_state
                        .Where(o => o.stockpile_location_id == stock_location_id
							&& o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.transaction_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                        record.qty_opening = PublicFunctions.Bulat(row.GetCell(2));
                        record.qty_in = PublicFunctions.Bulat(row.GetCell(3));
                        record.qty_out = PublicFunctions.Bulat(row.GetCell(4));
                        record.qty_adjustment = PublicFunctions.Bulat(row.GetCell(5));
                        record.qty_closing = PublicFunctions.Bulat(row.GetCell(6));
                        record.transaction_id = "0";

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new stockpile_state();
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

                        record.stockpile_location_id = stock_location_id;
                        record.transaction_datetime = PublicFunctions.Tanggal(row.GetCell(1));
                        record.qty_opening = PublicFunctions.Bulat(row.GetCell(2));
                        record.qty_in = PublicFunctions.Bulat(row.GetCell(3));
                        record.qty_out = PublicFunctions.Bulat(row.GetCell(4));
                        record.qty_adjustment = PublicFunctions.Bulat(row.GetCell(5));
                        record.qty_closing = PublicFunctions.Bulat(row.GetCell(6));
                        record.transaction_id = "0";

                        dbContext.stockpile_state.Add(record);
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
                HttpContext.Session.SetString("filename", "StockpileState");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [HttpPut("RefreshView")]
        public async Task<object> RefreshView()
        {
            try
            {
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    await using var cmd = conn.CreateCommand();
                    try
                    {
                        cmd.CommandText = string.Format(@" REFRESH MATERIALIZED VIEW CONCURRENTLY mv_stockpile_mutation_kmia;");

                        //var record = dbContext.organization
                        //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                        //    .FirstOrDefault();
                        //if (record != null && record.organization_code == "KM01")
                        //    cmd.CommandText = string.Format(@" REFRESH MATERIALIZED VIEW CONCURRENTLY mv_stockpile_mutation_kmia;");
                        //else
                        //    cmd.CommandText = string.Format(@" REFRESH MATERIALIZED VIEW CONCURRENTLY mv_stockpile_mutation;");

                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
                return "Refresh Material Views have been done successfully!";
            }
            catch (Exception ex)
            {
                return "Update failed";
            }
        }

        [HttpPut("RefreshQuality")]
        public async Task<object> RefreshQuality()
        {
            try
            {
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {
                            //var record = dbContext.organization
                            //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            //    .FirstOrDefault();
                            //if (record != null && record.organization_code == "KM01")
                            //    cmd.CommandText = string.Format(@" REFRESH MATERIALIZED VIEW CONCURRENTLY mv_stockpile_mutation_quality_kmia;");
                            //else
                            //    cmd.CommandText = string.Format(@" REFRESH MATERIALIZED VIEW CONCURRENTLY mv_stockpile_mutation_quality;");

                            cmd.CommandText = string.Format(@" REFRESH MATERIALIZED VIEW CONCURRENTLY mv_stockpile_mutation_quality_kmia;");

                            await cmd.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                    }
                }

                return "Refresh Material Views have been done successfully!";

            }
            catch (Exception ex)
            {
                return "Update failed";
            }
        }

        [HttpPut("RefreshView2")]
        public async Task<object> RefreshView2() //ini versi yang pake popoup dan informasi loading
        {
            var result = new StandardResult(); 
            try
            {
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    await using var cmd = conn.CreateCommand();
                    try
                    {
                        cmd.CommandText = string.Format(@" REFRESH MATERIALIZED VIEW CONCURRENTLY mv_stockpile_mutation_kmia;");
                        await cmd.ExecuteNonQueryAsync();

                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.InnerException ?? ex);
                        result.Message = ex.Message;
                        result.Success = false;
                        return BadRequest(result);
                    }

                    try
                    {
                        // run query historical mas yosho
                        cmd.CommandText = string.Format(@" with a as (
                                                            select
                                                            concat(dates,'#',stock_location_id) id
                                                            ,stock_location_name
                                                            ,stock_location_id
                                                            ,dates
                                                            ,value_partition
                                                            ,closing_before
                                                            ,closing
                                                            from vw_stockpile_summary_daily
                                                            )
                                                            ,updates as (
                                                                update historical_stock_mutation 
                                                                set closing = a.closing
                                                                from a
                                                                where a.id = historical_stock_mutation.id
                                                                and a.closing != historical_stock_mutation.closing
                                                                and extract (year from date(a.dates))  = extract (year from date(now()) - interval '1 day' ) --between extract (year from date(a.dates) ) and extract (year from date(now()) )
			                                                    and  extract (month from date(a.dates) ) between extract (month from date(now()) - interval '2 month' ) and extract (month from date(now()) )
			                                                    returning 
			                                                    'UPDATED' Process
			                                                    ,historical_stock_mutation.id
			                                                    ,historical_stock_mutation.stock_location_name
			                                                    ,historical_stock_mutation.stock_location_id
			                                                    ,historical_stock_mutation.dates
			                                                    ,historical_stock_mutation.value_partition
			                                                    ,historical_stock_mutation.closing_before
			                                                    ,historical_stock_mutation.closing
                                                            )
                                                            ,inserts as (
                                                                insert into historical_stock_mutation (
                                                                id
                                                                ,stock_location_name
                                                                ,stock_location_id
                                                                ,dates
                                                                ,value_partition
                                                                ,closing_before
                                                                ,closing
                                                            )			
                                                            select
                                                                id
                                                                ,stock_location_name
                                                                ,stock_location_id
                                                                ,dates
                                                                ,value_partition
                                                                ,closing_before
                                                                ,closing
                                                                from a 
                                                                where  a.id not in (select id from historical_stock_mutation)
                                                                returning 
			                                                            'INSERT' Process
			                                                            ,historical_stock_mutation.id
			                                                            ,historical_stock_mutation.stock_location_name
			                                                            ,historical_stock_mutation.stock_location_id
			                                                            ,historical_stock_mutation.dates
			                                                            ,historical_stock_mutation.value_partition
			                                                            ,historical_stock_mutation.closing_before
			                                                            ,historical_stock_mutation.closing
                                                            )
                                                            select*from updates
                                                            union
                                                            select*from inserts
                                                            ;");
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
                result.Success = true;

                return Ok(result);

            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                result.Message = ex.Message;
                result.Success = false;
                return BadRequest(result);
            }
        }

        [HttpPut("RefreshQuality2")] //ini versi yang pake popoup dan informasi loading
        public async Task<object> RefreshQuality2()
        {
            var result = new StandardResult();
            try
            {
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {
                            //var record = dbContext.organization
                            //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                            //    .FirstOrDefault();
                            //if (record != null && record.organization_code == "KM01")
                            //    cmd.CommandText = string.Format(@" REFRESH MATERIALIZED VIEW CONCURRENTLY mv_stockpile_mutation_quality_kmia;");
                            //else
                            //    cmd.CommandText = string.Format(@" REFRESH MATERIALIZED VIEW CONCURRENTLY mv_stockpile_mutation_quality;");

                            cmd.CommandText = string.Format(@" REFRESH MATERIALIZED VIEW CONCURRENTLY mv_stockpile_mutation_quality_kmia;");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.InnerException ?? ex);
                            result.Message = ex.Message;
                            result.Success = false;
                            return BadRequest(result);
                        }
                    }
                }
                result.Success = true;

                return Ok(result);

            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                result.Message = ex.Message;
                result.Success = false;
                return BadRequest(result);
            }
        }

    }
}

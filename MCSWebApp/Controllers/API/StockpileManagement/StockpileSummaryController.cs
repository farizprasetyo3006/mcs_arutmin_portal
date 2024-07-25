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
using Common;
using Npgsql;
using Microsoft.EntityFrameworkCore;

namespace MCSWebApp.Controllers.API.StockpileManagement
{
    [Route("api/StockpileManagement/[controller]")]
    [ApiController]
    public class StockpileSummaryController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public StockpileSummaryController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_stockpile_summary
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("getStockpile")]
        public async Task<IActionResult> GetStockpile(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var query = dbContext.vw_stockpile_summary
        .FromSqlRaw(@"SELECT vw.*, sl.business_unit_id
                      FROM vw_stockpile_summary AS vw
                      JOIN stockpile_location AS sl ON vw.stock_location_id = sl.id
                      WHERE (vw.organization_id = {0} OR {1})
                            AND ({2} = '' OR sl.business_unit_id = {2})",
                      CurrentUserContext.OrganizationId,
                      CurrentUserContext.IsSysAdmin,
                      CurrentUserContext.BusinessUnitId);
                // Add the parameter
                var stockpileDescriptions = await query.Select(q => q.stock_location_description).Distinct().ToListAsync();

                var data = await DataSourceLoader.LoadAsync(query, loadOptions);

                return new JsonResult(new
                {
                    data,
                    stockpileDescriptions
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.vw_stockpile_summary.Where(o => o.id == Id),
                loadOptions);
        }

    //    [HttpGet("Detail/{Id}")]
    //    public async Task<IActionResult> Detail(string Id)
    //    {
    //        if (await mcsContext.CanRead(dbContext, Id, CurrentUserContext.AppUserId)
    //            || CurrentUserContext.IsSysAdmin)
    //        {
    //            try
    //            {
    //                var record = await dbContext.vw_rainfall
    //                    .Where(o => o.id == Id
    //                        && (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
    //                            || CurrentUserContext.IsSysAdmin))
    //                    .FirstOrDefaultAsync();
    //                return Ok(record);
    //            }
				//catch (Exception ex)
				//{
				//	logger.Error(ex.InnerException ?? ex);
				//	return BadRequest(ex.InnerException?.Message ?? ex.Message);
				//}
    //        }
    //        else
    //        {
    //            return BadRequest("User is not authorized.");
    //        }
    //    }

    }
}

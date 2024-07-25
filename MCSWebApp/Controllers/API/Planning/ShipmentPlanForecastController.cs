using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using BusinessLogic;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Common;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Planning/[controller]")]
    [ApiController]
    public class ShipmentPlanForecastController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public ShipmentPlanForecastController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("GenerateReport")]
        public object GenerateReport()
        {
            try
            {                
                return dbContext.shipment_forecast.Where(x => x.year == DateTime.Now.Year); 
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
                dbContext.shipment_forecast.Where(o => o.id == Id),
                loadOptions);
        }

        [HttpPut("UpdateData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {

                var temp = dbContext.shipment_forecast
                    .Where(o => o.id == key)
                    .FirstOrDefault();

                if (temp == null)
                {
                    var record = new shipment_forecast();
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

                    dbContext.shipment_forecast.Add(record);
                    await dbContext.SaveChangesAsync();
                    return Ok(record);
                }
                else
                {
                    JsonConvert.PopulateObject(values, temp);
                    var recordUpdate = dbContext.shipment_forecast
                    .Where(o => o.id == temp.id)
                    .FirstOrDefault();
                    if (recordUpdate != null)
                    {
                        recordUpdate.InjectFrom(temp);
                        recordUpdate.modified_by = CurrentUserContext.AppUserId;
                        recordUpdate.modified_on = DateTime.Now;

                        await dbContext.SaveChangesAsync();
                        return Ok(recordUpdate);
                    }
                    else
                    {
                        return BadRequest("No default organization");
                    }
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
            logger.Debug($"string Id = {key}");

            try
            {
                var record = dbContext.shipment_forecast
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.shipment_forecast.Remove(record);
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
    }
}

﻿using System;
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
using Microsoft.EntityFrameworkCore;

namespace MCSWebApp.Controllers.API.Location
{
    [Route("api/Location/[controller]")]
    [ApiController]
    public class MineLocationQualityController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public MineLocationQualityController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption) : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("ByMineLocationId/{Id}")]
        public async Task<object> ByMineLocationId(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_mine_location_quality
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.mine_location_id == Id),
                loadOptions);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(dbContext.vw_mine_location_quality
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin),
                    loadOptions);
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(
                dbContext.mine_location_quality.Where(o => o.id == Id),
                loadOptions);
        }

   //     [HttpPost("InsertData")]
   //     [ApiExplorerSettings(IgnoreApi = true)]
   //     public async Task<IActionResult> InsertData([FromForm] string values)
   //     {
   //         logger.Trace($"string values = {values}");

   //         mine_location_quality record;

   //         try
   //         {
			//	if (await mcsContext.CanCreate(dbContext, nameof(mine_location_quality),
			//		CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
			//	{
   //                 record = new mine_location_quality();
   //                 JsonConvert.PopulateObject(values, record);

   //                 record.id = Guid.NewGuid().ToString("N");
   //                 record.created_by = CurrentUserContext.AppUserId;
   //                 record.created_on = DateTime.Now;
   //                 record.modified_by = null;
   //                 record.modified_on = null;
   //                 record.is_active = true;
   //                 record.is_default = null;
   //                 record.is_locked = null;
   //                 record.entity_id = null;
   //                 record.owner_id = CurrentUserContext.AppUserId;
   //                 record.organization_id = CurrentUserContext.OrganizationId;

   //                 dbContext.mine_location_quality.Add(record);
   //                 await dbContext.SaveChangesAsync();
			//	}
			//	else
			//	{
			//		return BadRequest("User is not authorized.");
			//	}
   //         }
			//catch (Exception ex)
			//{
			//	logger.Error(ex.InnerException ?? ex);
   //             return BadRequest(ex.InnerException?.Message ?? ex.Message);
			//}

   //         return Ok(record);
   //     }

   //     [HttpPut("UpdateData")]
   //     [ApiExplorerSettings(IgnoreApi = true)]
   //     public async Task<IActionResult> UpdateData([FromForm] string key, [FromForm] string values)
   //     {
   //         logger.Trace($"string values = {values}");

   //         try
   //         {
   //             var record = dbContext.mine_location_quality
   //                 .Where(o => o.id == key)
   //                 .FirstOrDefault();
   //             if (record != null)
   //             {
			//		if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
			//			|| CurrentUserContext.IsSysAdmin)
			//		{
   //                     var e = new entity();
   //                     e.InjectFrom(record);

   //                     JsonConvert.PopulateObject(values, record);

   //                     record.InjectFrom(e);
   //                     record.modified_by = CurrentUserContext.AppUserId;
   //                     record.modified_on = DateTime.Now;

   //                     await dbContext.SaveChangesAsync();
   //                     return Ok(record);
			//		}
			//		else
			//		{
			//			return BadRequest("User is not authorized.");
			//		}
   //             }
   //             else
   //             {
   //                 return BadRequest("No default organization");
   //             }
   //         }
			//catch (Exception ex)
			//{
			//	logger.Error(ex.InnerException ?? ex);
   //             return BadRequest(ex.InnerException?.Message ?? ex.Message);
			//}
   //     }

   //     [HttpDelete("DeleteData")]
   //     [ApiExplorerSettings(IgnoreApi = true)]
   //     public async Task<IActionResult> DeleteData([FromForm] string key)
   //     {
   //         logger.Debug($"string key = {key}");

   //         try
   //         {
   //             var record = dbContext.mine_location_quality
   //                 .Where(o => o.id == key)
   //                 .FirstOrDefault();
   //             if (record != null)
   //             {
   //                 dbContext.mine_location_quality.Remove(record);
   //                 await dbContext.SaveChangesAsync();
   //             }

   //             return Ok();
   //         }
			//catch (Exception ex)
			//{
			//	logger.Error(ex.InnerException ?? ex);
   //             return BadRequest(ex.InnerException?.Message ?? ex.Message);
			//}
   //     }

   //     [HttpPost("SaveData")]
   //     public async Task<IActionResult> SaveData([FromBody] mine_location_quality Record)
   //     {
   //         try
   //         {
   //             var record = dbContext.mine_location_quality
   //                 .Where(o => o.id == Record.id)
   //                 .FirstOrDefault();
   //             if (record != null)
   //             {
			//		if (await mcsContext.CanUpdate(dbContext, record.id, CurrentUserContext.AppUserId)
			//			|| CurrentUserContext.IsSysAdmin)
			//		{
   //                     var e = new entity();
   //                     e.InjectFrom(record);
   //                     record.InjectFrom(Record);
   //                     record.InjectFrom(e);
   //                     record.modified_by = CurrentUserContext.AppUserId;
   //                     record.modified_on = DateTime.Now;

   //                     await dbContext.SaveChangesAsync();
   //                     return Ok(record);
			//		}
			//		else
			//		{
			//			return BadRequest("User is not authorized.");
			//		}
   //             }
   //             else if (await mcsContext.CanCreate(dbContext, nameof(mine_location_quality),
   //                 CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
   //             {
   //                 record = new mine_location_quality();
   //                 record.InjectFrom(Record);

   //                 record.id = Guid.NewGuid().ToString("N");
   //                 record.created_by = CurrentUserContext.AppUserId;
   //                 record.created_on = DateTime.Now;
   //                 record.modified_by = null;
   //                 record.modified_on = null;
   //                 record.is_active = true;
   //                 record.is_default = null;
   //                 record.is_locked = null;
   //                 record.entity_id = null;
   //                 record.owner_id = CurrentUserContext.AppUserId;
   //                 record.organization_id = CurrentUserContext.OrganizationId;

   //                 dbContext.mine_location_quality.Add(record);
   //                 await dbContext.SaveChangesAsync();

   //                 return Ok(record);
   //             }
   //             else
   //             {
   //                 return BadRequest("User is not authorized.");
   //             }
   //         }
			//catch (Exception ex)
			//{
			//	logger.Error(ex.InnerException ?? ex);
   //             return BadRequest(ex.InnerException?.Message ?? ex.Message);
			//}
   //     }

        [HttpDelete("DeleteById/{Id}")]
        public async Task<IActionResult> DeleteById(string Id)
        {
            logger.Trace($"string Id = {Id}");

            try
            {
                var record = dbContext.mine_location_quality
                    .Where(o => o.id == Id)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.mine_location_quality.Remove(record);
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

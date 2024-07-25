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
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office.Word;
using Microsoft.EntityFrameworkCore.Storage;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/[controller]")]
    [ApiController]
    public class DespatchOrderController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public DespatchOrderController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("DataGrid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DataGrid(DataSourceLoadOptions loadOptions)
        {
            try
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .OrderByDescending(o => o.created_on),
                        loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DataGrid/{tanggal1}/{tanggal2}")]
        public async Task<object> DataGrid(string tanggal1, string tanggal2, DataSourceLoadOptions loadOptions)
        {
            logger.Debug($"tanggal1 = {tanggal1}");
            logger.Debug($"tanggal2 = {tanggal2}");

            if (tanggal1 == null || tanggal2 == null)
            {
                return await DataSourceLoader.LoadAsync(dbContext.vw_despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                        && CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                            || CurrentUserContext.IsSysAdmin),
                    loadOptions);
            }

            //var dt1 = DateTime.Parse(tanggal1);
            //var dt2 = DateTime.Parse(tanggal2);
            var dt1 = Convert.ToDateTime(tanggal1);
            var dt2 = Convert.ToDateTime(tanggal2);

            logger.Debug($"dt1 = {dt1}");
            logger.Debug($"dt2 = {dt2}");

            return await DataSourceLoader.LoadAsync(dbContext.vw_despatch_order
                .Where(o =>
                    o.despatch_order_date >= dt1
                    && o.despatch_order_date <= dt2
                    && o.organization_id == CurrentUserContext.OrganizationId
                    && (CustomFunctions.CanRead(o.id, CurrentUserContext.AppUserId)
                        || CurrentUserContext.IsSysAdmin)),
                loadOptions);
        }

        [HttpGet("ParentDespatchOrderIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> ParentDespatchOrderIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.despatch_order
                    .Where(o => o.set_parent == true && o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("DataDetail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<StandardResult> DataDetail(string Id, DataSourceLoadOptions loadOptions)
        {
            Dictionary<string, dynamic> myData = new Dictionary<string, dynamic>();
            var result = new StandardResult();
            try
            {
                var record = await dbContext.vw_despatch_order
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                result.Success = record != null ? true : false;
                result.Message = result.Success ? "Ok" : "Record not found";
                result.Data = record;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                result.Success = false;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpGet("DataDetailForDesDem")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<StandardResult> DataDetailForDesDem(string Id, DataSourceLoadOptions loadOptions)
        {
            Dictionary<string, dynamic> myData = new Dictionary<string, dynamic>();
            var result = new StandardResult();
            try
            {
                var record = await dbContext.vw_despatch_order
                    .Where(o => o.id == Id).FirstOrDefaultAsync();
                result.Success = record != null ? true : false;
                result.Message = result.Success ? "Ok" : "Record not found";
                var laytime_text = "";
                var draftSurvey = await dbContext.draft_survey.Where(o=>o.despatch_order_id == Id).FirstOrDefaultAsync();
                if (draftSurvey != null)
                {
                    decimal draftSruveyQuantity = ((decimal)draftSurvey.quantity / (decimal)record.loading_rate) * 86400;
                    laytime_text = secondsToDhms(draftSruveyQuantity);
                }
                else
                {

                    if (record.laytime_duration != null)
                    {
                        laytime_text = secondsToDhms((decimal)record.laytime_duration);
                    }
                }
                myData.Add("Data", record);
                myData.Add("laytime_text", laytime_text);
                myData.Add("success", true);
                result.Data = myData;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                result.Success = false;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                if (await mcsContext.CanCreate(dbContext, nameof(despatch_order),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var record = new despatch_order();
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

                    #region Get transaction number
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
                                string organizationCode = dbContext.organization
                                    .Where(o => o.id == CurrentUserContext.OrganizationId)
                                    .Select(o => o.organization_code)
                                    .FirstOrDefault() ?? "";
                                cmd.CommandText = $"SELECT nextval('seq_despatch_order_number')";

                                var r = await cmd.ExecuteScalarAsync();
                                r = Convert.ToInt32(r).ToString("D3");

                                var result = $"";

                                var szYear = record.created_on.Value.Year.ToString();
                                var szMonth = record.created_on.Value.Month.ToString();
                                var szDay = record.created_on.Value.Day.ToString();
                                var szNamaKapal = string.Empty;
                                var szNamaKustomer = string.Empty;
                                var szInvoiceNo = record.invoice_number ?? string.Empty;
                                var discharge_port = record.discharge_port.ToString() ?? string.Empty;

                                if (record.vessel_id != null)
                                {
                                    var resultBarge = await dbContext.barge
                                        .Where(x => x.id == record.vessel_id).SingleOrDefaultAsync();
                                    if (resultBarge == null)
                                    {
                                        var resultVessel = await dbContext.vessel
                                            .Where(x => x.id == record.vessel_id).SingleOrDefaultAsync();
                                        if (resultVessel == null)
                                            throw new Exception("Vessel or Barge Not Found.");
                                        else
                                        {
                                            szNamaKapal = resultVessel.vehicle_name;
                                        }
                                    }
                                    else
                                    {
                                        szNamaKapal = resultBarge.vehicle_name;
                                    }
                                }

                                if (string.IsNullOrEmpty(szNamaKustomer))
                                {
                                    var resultCustomer = await dbContext.customer
                                        .Where(x => x.id == record.customer_id).SingleOrDefaultAsync();

                                    szNamaKustomer = resultCustomer.business_partner_name;
                                }

                                record.despatch_order_number = $"SO-{szYear}/{szMonth}/{szDay}_{r}_{szNamaKapal}_({szNamaKustomer})_{discharge_port}_{szInvoiceNo}_{record.royalty_number}";
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.ToString());
                                return BadRequest(ex.Message);
                            }
                        }
                    }
                    #endregion

                    if (record.loading_rate != null && record.required_quantity != null &&
                        record.loading_rate > 0 && record.required_quantity > 0)
                    {
                        record.laytime_duration = (record.required_quantity / record.loading_rate) * 86400;
                    }

                    dbContext.despatch_order.Add(record);
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

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] despatch_order Record)
        {
            try
            {
                var record = dbContext.despatch_order
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
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;
                        record.owner_id = (record.owner_id ?? CurrentUserContext.AppUserId);

                        await dbContext.SaveChangesAsync();
                        return Ok(record);
                    }
                    else
                    {
                        return BadRequest("User is not authorized.");
                    }
                }
                else if (await mcsContext.CanCreate(dbContext, nameof(despatch_order),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    record = new despatch_order();
                    record.InjectFrom(Record);

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
                    record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

                    #region Get transaction number
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
                                string organizationCode = dbContext.organization
                                    .Where(o => o.id == CurrentUserContext.OrganizationId)
                                    .Select(o => o.organization_code)
                                    .FirstOrDefault() ?? "";
                                switch (organizationCode.ToUpper())
                                {
                                    case "IN01":
                                        cmd.CommandText = $"SELECT nextval('seq_despatch_order_number_ic')";
                                        break;
                                    case "KM01":
                                        cmd.CommandText = $"SELECT nextval('seq_despatch_order_number_kmia')";
                                        break;
                                    case "UD01":
                                        cmd.CommandText = $"SELECT nextval('seq_despatch_order_number_udu')";
                                        break;
                                    default:
                                        cmd.CommandText = $"SELECT nextval('seq_despatch_order_number')";
                                        break;
                                }

                                //cmd.CommandText = $"SELECT nextval('seq_despatch_order_number')";
                                var r = await cmd.ExecuteScalarAsync();
                                r = Convert.ToInt32(r).ToString("D3");
                                record.despatch_order_number = $"DO-{DateTime.Now:yyyyMMdd}-{r}";
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.ToString());
                                return BadRequest(ex.Message);
                            }
                        }
                    }
                    #endregion

                    if (record.loading_rate != null && record.required_quantity != null &&
                        record.loading_rate > 0 && record.required_quantity > 0)
                    {
                        record.laytime_duration = (record.required_quantity / record.loading_rate) * 86400;
                    }

                    dbContext.despatch_order.Add(record);
                    await dbContext.SaveChangesAsync();

                    return Ok(record);
                }
                else
                {
                    logger.Debug("User is not authorized.");
                    return Unauthorized();
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
                var record = dbContext.despatch_order
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

                        if (record.loading_rate != null && record.required_quantity != null &&
                            record.loading_rate > 0 && record.required_quantity > 0)
                        {
                            record.laytime_duration = (record.required_quantity / record.loading_rate) * 86400;
                        }

                        #region Update Despatch Order Number

                        var lastShipmentNumber = record.despatch_order_number;
                        var szYear = record.created_on.Value.Year.ToString();
                        var szMonth = record.created_on.Value.Month.ToString("D2");
                        var szDay = record.created_on.Value.Day.ToString("D2");
                        var szIndex = string.Empty;
                        var szNamaKapal = string.Empty;
                        var szNamaKustomer = string.Empty;
                        var szInvoiceNo = record.invoice_number;
                        var discharge_port = record.discharge_port.ToString();

                        if (lastShipmentNumber != null)
                        {
                            var szSplit = lastShipmentNumber.Split('_');
                            if (szSplit.Length > 4)
                            {
                                szIndex = szSplit[1];
                            }
                            else
                            {
                                szSplit = lastShipmentNumber.Split('-');
                                szIndex = szSplit[2];
                            }
                            var dtmDate = record.created_on.Value;
                            var szDate = $"{szYear}/{szMonth}/{szDay}";

                            if (string.IsNullOrEmpty(szNamaKapal))
                            {
                                var resultBarge = await dbContext.barge
                                    .Where(x => x.id == record.vessel_id).SingleOrDefaultAsync();
                                if (resultBarge == null)
                                {
                                    var resultVessel = await dbContext.vessel
                                        .Where(x => x.id == record.vessel_id).SingleOrDefaultAsync();
                                    if (resultVessel == null)
                                        throw new Exception("Vessel or Barge Not Found.");
                                    else
                                    {
                                        szNamaKapal = resultVessel.vehicle_name;
                                    }
                                }
                                else
                                {
                                    szNamaKapal = resultBarge.vehicle_name;
                                }
                            }

                            if (string.IsNullOrEmpty(szNamaKustomer))
                            {
                                var resultCustomer = await dbContext.customer
                                    .Where(x => x.id == record.customer_id).SingleOrDefaultAsync();

                                szNamaKustomer = resultCustomer.business_partner_name;
                            }
                        }
                        else
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
                                    cmd.CommandText = $"SELECT nextval('seq_despatch_order_number')";
                                    var r = await cmd.ExecuteScalarAsync();
                                    r = Convert.ToInt32(r).ToString("D3");
                                    szIndex = (string)r;

                                    if (string.IsNullOrEmpty(szNamaKapal))
                                    {
                                        var resultBarge = await dbContext.barge
                                            .Where(x => x.id == record.vessel_id).SingleOrDefaultAsync();
                                        if (resultBarge == null)
                                        {
                                            var resultVessel = await dbContext.vessel
                                                .Where(x => x.id == record.vessel_id).SingleOrDefaultAsync();
                                            if (resultVessel == null)
                                                throw new Exception("Vessel or Barge Not Found.");
                                            else
                                            {
                                                szNamaKapal = resultVessel.vehicle_name;
                                            }
                                        }
                                        else
                                        {
                                            szNamaKapal = resultBarge.vehicle_name;
                                        }
                                    }

                                    if (string.IsNullOrEmpty(szNamaKustomer))
                                    {
                                        var resultCustomer = await dbContext.customer
                                            .Where(x => x.id == record.customer_id).SingleOrDefaultAsync();

                                        szNamaKustomer = resultCustomer.business_partner_name;
                                    }
                                }
                            }
                        }

                        record.despatch_order_number = $"SO-{szYear}/{szMonth}/{szDay}_{szIndex}_{szNamaKapal}_({szNamaKustomer})_{discharge_port}_{szInvoiceNo}_{record.royalty_number}";


                        #endregion

                        await dbContext.SaveChangesAsync();

                        //string WONumber = null;

                        //dynamic recDOE = dbContext.despatch_order_ell
                        //   .Where(o => o.id == key && o.sync_status == "SUCCESS" && (o.wo_number != null || o.wo_number.Trim() != ""))
                        //   .FirstOrDefault();
                        //if (recDOE == null)
                        //{
                        //    dbContext.despatch_order_ell.Remove(recDOE);
                        //    await dbContext.SaveChangesAsync();

                        //    var recEll = new despatch_order_ell()
                        //    {
                        //        id = record.id,
                        //        created_by = CurrentUserContext.AppUserId,
                        //        created_on = DateTime.Now,
                        //        modified_by = null,
                        //        modified_on = null,
                        //        is_active = record.is_active,
                        //        is_locked = record.is_locked,
                        //        is_default = record.is_default,
                        //        owner_id = record.owner_id,
                        //        organization_id = record.organization_id,
                        //        entity_id = record.entity_id,

                        //        despatch_order_number = record.despatch_order_number,
                        //        sales_order_id = record.sales_order_id,
                        //        product_id = record.product_id,
                        //        quantity = record.quantity,
                        //        uom_id = record.uom_id,
                        //        despatch_order_date = record.despatch_order_date,
                        //        planned_despatch_date = record.planned_despatch_date,
                        //        contract_term_id = record.contract_term_id,
                        //        customer_id = record.customer_id,
                        //        seller_id = record.seller_id,
                        //        ship_to = record.ship_to,
                        //        contract_product_id = record.contract_product_id,
                        //        required_quantity = record.required_quantity,
                        //        final_quantity = record.final_quantity,
                        //        fulfilment_type_id = record.fulfilment_type_id,
                        //        delivery_term_id = record.delivery_term_id,
                        //        notes = record.notes,
                        //        despatch_plan_id = record.despatch_plan_id,
                        //        bill_of_lading_date = record.bill_of_lading_date,
                        //        vessel_id = record.vessel_id,
                        //        laycan_date = record.laycan_date,
                        //        shipment_number = record.shipment_number,
                        //        order_reference_date = record.order_reference_date,
                        //        eta_plan = record.eta_plan,
                        //        laycan_start = record.laycan_start,
                        //        laycan_end = record.laycan_end,
                        //        laycan_committed = record.laycan_committed,
                        //        vessel_committed = record.vessel_committed,
                        //        eta_committed = record.eta_committed,
                        //        loading_port = record.loading_port,
                        //        discharge_port = record.discharge_port,
                        //        quantity_actual = record.quantity_actual,
                        //        bill_lading_date = record.bill_lading_date,
                        //        document_reference_id = record.document_reference_id,
                        //        letter_of_credit = record.letter_of_credit,
                        //        port_location_id = record.port_location_id,
                        //        surveyor_id = record.surveyor_id,
                        //        shipping_agent = record.shipping_agent,
                        //        laytime_duration = record.laytime_duration,
                        //        laytime_text = record.laytime_text,
                        //        loading_rate = record.loading_rate,
                        //        despatch_demurrage_rate = record.despatch_demurrage_rate,
                        //        wo_number = null,

                        //        sync_id = Guid.NewGuid().ToString("N"),
                        //        sync_type = "INSERT",
                        //        sync_status = null,
                        //        error_msg = null
                        //    };

                        //    dbContext.despatch_order_ell.Add(recEll);
                        //    await dbContext.SaveChangesAsync();

                        //    return Ok(record);
                        //}
                        //else
                        //{
                        //    WONumber = recDOE.wo_number;

                        //    recDOE = dbContext.despatch_order_ell
                        //       .Where(o => o.id == key && o.sync_type == "UPDATE")
                        //       .FirstOrDefault();
                        //    if (recDOE != null)
                        //    {
                        //        recDOE.modified_by = CurrentUserContext.AppUserId;
                        //        recDOE.modified_on = System.DateTime.Now;
                        //        recDOE.is_active = record.is_active;
                        //        recDOE.is_locked = record.is_locked;
                        //        recDOE.is_default = record.is_default;
                        //        recDOE.owner_id = record.owner_id;
                        //        recDOE.organization_id = record.organization_id;
                        //        recDOE.entity_id = record.entity_id;

                        //        recDOE.despatch_order_number = record.despatch_order_number;
                        //        recDOE.sales_order_id = record.sales_order_id;
                        //        recDOE.product_id = record.product_id;
                        //        recDOE.quantity = record.quantity;
                        //        recDOE.uom_id = record.uom_id;
                        //        recDOE.despatch_order_date = record.despatch_order_date;
                        //        recDOE.planned_despatch_date = record.planned_despatch_date;
                        //        recDOE.contract_term_id = record.contract_term_id;
                        //        recDOE.customer_id = record.customer_id;
                        //        recDOE.seller_id = record.seller_id;
                        //        recDOE.ship_to = record.ship_to;
                        //        recDOE.contract_product_id = record.contract_product_id;
                        //        recDOE.required_quantity = record.required_quantity;
                        //        recDOE.final_quantity = record.final_quantity;
                        //        recDOE.fulfilment_type_id = record.fulfilment_type_id;
                        //        recDOE.delivery_term_id = record.delivery_term_id;
                        //        recDOE.notes = record.notes;
                        //        recDOE.despatch_plan_id = record.despatch_plan_id;
                        //        recDOE.bill_of_lading_date = record.bill_of_lading_date;
                        //        recDOE.vessel_id = record.vessel_id;
                        //        recDOE.laycan_date = record.laycan_date;
                        //        recDOE.shipment_number = record.shipment_number;
                        //        recDOE.order_reference_date = record.order_reference_date;
                        //        recDOE.eta_plan = record.eta_plan;
                        //        recDOE.laycan_start = record.laycan_start;
                        //        recDOE.laycan_end = record.laycan_end;
                        //        recDOE.laycan_committed = record.laycan_committed;
                        //        recDOE.vessel_committed = record.vessel_committed;
                        //        recDOE.eta_committed = record.eta_committed;
                        //        recDOE.loading_port = record.loading_port;
                        //        recDOE.discharge_port = record.discharge_port;
                        //        recDOE.quantity_actual = record.quantity_actual;
                        //        recDOE.bill_lading_date = record.bill_lading_date;
                        //        recDOE.document_reference_id = record.document_reference_id;
                        //        recDOE.letter_of_credit = record.letter_of_credit;
                        //        recDOE.port_location_id = record.port_location_id;
                        //        recDOE.surveyor_id = record.surveyor_id;
                        //        recDOE.shipping_agent = record.shipping_agent;
                        //        recDOE.laytime_duration = record.laytime_duration;
                        //        recDOE.laytime_text = record.laytime_text;
                        //        recDOE.loading_rate = record.loading_rate;
                        //        recDOE.despatch_demurrage_rate = record.despatch_demurrage_rate;
                        //        recDOE.wo_number = WONumber;

                        //        recDOE.sync_status = null;
                        //        recDOE.error_msg = null;
                        //    }
                        //    else
                        //    {

                        //        var recEll = new despatch_order_ell()
                        //        {
                        //            id = record.id,
                        //            created_by = CurrentUserContext.AppUserId,
                        //            created_on = DateTime.Now,
                        //            modified_by = null,
                        //            modified_on = null,
                        //            is_active = record.is_active,
                        //            is_locked = record.is_locked,
                        //            is_default = record.is_default,
                        //            owner_id = record.owner_id,
                        //            organization_id = record.organization_id,
                        //            entity_id = record.entity_id,

                        //            despatch_order_number = record.despatch_order_number,
                        //            sales_order_id = record.sales_order_id,
                        //            product_id = record.product_id,
                        //            quantity = record.quantity,
                        //            uom_id = record.uom_id,
                        //            despatch_order_date = record.despatch_order_date,
                        //            planned_despatch_date = record.planned_despatch_date,
                        //            contract_term_id = record.contract_term_id,
                        //            customer_id = record.customer_id,
                        //            seller_id = record.seller_id,
                        //            ship_to = record.ship_to,
                        //            contract_product_id = record.contract_product_id,
                        //            required_quantity = record.required_quantity,
                        //            final_quantity = record.final_quantity,
                        //            fulfilment_type_id = record.fulfilment_type_id,
                        //            delivery_term_id = record.delivery_term_id,
                        //            notes = record.notes,
                        //            despatch_plan_id = record.despatch_plan_id,
                        //            bill_of_lading_date = record.bill_of_lading_date,
                        //            vessel_id = record.vessel_id,
                        //            laycan_date = record.laycan_date,
                        //            shipment_number = record.shipment_number,
                        //            order_reference_date = record.order_reference_date,
                        //            eta_plan = record.eta_plan,
                        //            laycan_start = record.laycan_start,
                        //            laycan_end = record.laycan_end,
                        //            laycan_committed = record.laycan_committed,
                        //            vessel_committed = record.vessel_committed,
                        //            eta_committed = record.eta_committed,
                        //            loading_port = record.loading_port,
                        //            discharge_port = record.discharge_port,
                        //            quantity_actual = record.quantity_actual,
                        //            bill_lading_date = record.bill_lading_date,
                        //            document_reference_id = record.document_reference_id,
                        //            letter_of_credit = record.letter_of_credit,
                        //            port_location_id = record.port_location_id,
                        //            surveyor_id = record.surveyor_id,
                        //            shipping_agent = record.shipping_agent,
                        //            laytime_duration = record.laytime_duration,
                        //            laytime_text = record.laytime_text,
                        //            loading_rate = record.loading_rate,
                        //            despatch_demurrage_rate = record.despatch_demurrage_rate,
                        //            wo_number = null,

                        //            sync_id = Guid.NewGuid().ToString("N"),
                        //            sync_type = "INSERT",
                        //            sync_status = null,
                        //            error_msg = null
                        //        };

                        //        dbContext.despatch_order_ell.Add(recEll);
                        //    }

                        //    await dbContext.SaveChangesAsync();
                        //}

                        return Ok(record);
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
                var sales_invoice = dbContext.sales_invoice.Where(o => o.organization_id == CurrentUserContext.OrganizationId
                    && o.despatch_order_id == key).FirstOrDefault();
                if (sales_invoice != null) return BadRequest("Can not be deleted since it is already have one or more transactions.");

                var record = dbContext.despatch_order
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    string WONumber = dbContext.despatch_order_ell
                       .Where(o => o.id == key && o.sync_type == "INSERT")
                       .FirstOrDefault()?.wo_number ?? "";

                    var recDOE = dbContext.despatch_order_ell
                       .Where(o => o.id == key && o.sync_type == "DELETE")
                       .FirstOrDefault();
                    if (recDOE == null)
                    {
                        var recEll = new despatch_order_ell()
                        {
                            id = record.id,
                            created_by = CurrentUserContext.AppUserId,
                            created_on = DateTime.Now,
                            modified_by = null,
                            modified_on = null,
                            is_active = record.is_active,
                            is_locked = record.is_locked,
                            is_default = record.is_default,
                            owner_id = record.owner_id,
                            organization_id = record.organization_id,
                            entity_id = record.entity_id,

                            despatch_order_number = record.despatch_order_number,
                            sales_order_id = record.sales_order_id,
                            product_id = record.product_id,
                            quantity = record.quantity,
                            uom_id = record.uom_id,
                            despatch_order_date = record.despatch_order_date,
                            planned_despatch_date = record.planned_despatch_date,
                            contract_term_id = record.contract_term_id,
                            customer_id = record.customer_id,
                            seller_id = record.seller_id,
                            ship_to = record.ship_to,
                            contract_product_id = record.contract_product_id,
                            required_quantity = record.required_quantity,
                            final_quantity = record.final_quantity,
                            fulfilment_type_id = record.fulfilment_type_id,
                            delivery_term_id = record.delivery_term_id,
                            notes = record.notes,
                            despatch_plan_id = record.despatch_plan_id,
                            bill_of_lading_date = record.bill_of_lading_date,
                            vessel_id = record.vessel_id,
                            laycan_date = record.laycan_date,
                            shipment_number = record.shipment_number,
                            order_reference_date = record.order_reference_date,
                            eta_plan = record.eta_plan,
                            laycan_start = record.laycan_start,
                            laycan_end = record.laycan_end,
                            laycan_committed = record.laycan_committed,
                            vessel_committed = record.vessel_committed,
                            eta_committed = record.eta_committed,
                            loading_port = record.loading_port,
                            discharge_port = record.discharge_port,
                            quantity_actual = record.quantity_actual,
                            bill_lading_date = record.bill_lading_date,
                            document_reference_id = record.document_reference_id,
                            letter_of_credit = record.letter_of_credit,
                            port_location_id = record.port_location_id,
                            surveyor_id = record.surveyor_id,
                            shipping_agent = record.shipping_agent,
                            laytime_duration = record.laytime_duration,
                            laytime_text = record.laytime_text,
                            loading_rate = record.loading_rate,
                            despatch_demurrage_rate = record.despatch_demurrage_rate,
                            wo_number = WONumber,

                            sync_id = Guid.NewGuid().ToString("N"),
                            sync_type = "DELETE",
                            sync_status = null,
                            error_msg = null
                        };

                        dbContext.despatch_order_ell.Add(recEll);
                    }
                    else
                    {
                        recDOE.modified_by = CurrentUserContext.AppUserId;
                        recDOE.modified_on = System.DateTime.Now;
                        recDOE.is_active = record.is_active;
                        recDOE.is_locked = record.is_locked;
                        recDOE.is_default = record.is_default;
                        recDOE.owner_id = record.owner_id;
                        recDOE.organization_id = record.organization_id;
                        recDOE.entity_id = record.entity_id;

                        recDOE.despatch_order_number = record.despatch_order_number;
                        recDOE.sales_order_id = record.sales_order_id;
                        recDOE.product_id = record.product_id;
                        recDOE.quantity = record.quantity;
                        recDOE.uom_id = record.uom_id;
                        recDOE.despatch_order_date = record.despatch_order_date;
                        recDOE.planned_despatch_date = record.planned_despatch_date;
                        recDOE.contract_term_id = record.contract_term_id;
                        recDOE.customer_id = record.customer_id;
                        recDOE.seller_id = record.seller_id;
                        recDOE.ship_to = record.ship_to;
                        recDOE.contract_product_id = record.contract_product_id;
                        recDOE.required_quantity = record.required_quantity;
                        recDOE.final_quantity = record.final_quantity;
                        recDOE.fulfilment_type_id = record.fulfilment_type_id;
                        recDOE.delivery_term_id = record.delivery_term_id;
                        recDOE.notes = record.notes;
                        recDOE.despatch_plan_id = record.despatch_plan_id;
                        recDOE.bill_of_lading_date = record.bill_of_lading_date;
                        recDOE.vessel_id = record.vessel_id;
                        recDOE.laycan_date = record.laycan_date;
                        recDOE.shipment_number = record.shipment_number;
                        recDOE.order_reference_date = record.order_reference_date;
                        recDOE.eta_plan = record.eta_plan;
                        recDOE.laycan_start = record.laycan_start;
                        recDOE.laycan_end = record.laycan_end;
                        recDOE.laycan_committed = record.laycan_committed;
                        recDOE.vessel_committed = record.vessel_committed;
                        recDOE.eta_committed = record.eta_committed;
                        recDOE.loading_port = record.loading_port;
                        recDOE.discharge_port = record.discharge_port;
                        recDOE.quantity_actual = record.quantity_actual;
                        recDOE.bill_lading_date = record.bill_lading_date;
                        recDOE.document_reference_id = record.document_reference_id;
                        recDOE.letter_of_credit = record.letter_of_credit;
                        recDOE.port_location_id = record.port_location_id;
                        recDOE.surveyor_id = record.surveyor_id;
                        recDOE.shipping_agent = record.shipping_agent;
                        recDOE.laytime_duration = record.laytime_duration;
                        recDOE.laytime_text = record.laytime_text;
                        recDOE.loading_rate = record.loading_rate;
                        recDOE.despatch_demurrage_rate = record.despatch_demurrage_rate;
                        recDOE.wo_number = WONumber;

                        recDOE.sync_status = null;
                        recDOE.error_msg = null;
                    }
                    await dbContext.SaveChangesAsync();

                    dbContext.despatch_order.Remove(record);
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

        [HttpGet("DespatchOrderIdLookup")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> DespatchOrderIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_despatch_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.despatch_order_number })
                    .Distinct();
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SalesOrderIdLookup")]
        public async Task<object> SalesOrderIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.sales_order
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Select(o => new { Value = o.id, Text = o.sales_order_number });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
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

            string FilePath = configuration.GetSection("Path").GetSection("UploadBasePath")
                .Value + PublicFunctions.ExcelFolder;
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

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    DateTime? shipping_order_date = DateTime.MinValue;
                    shipping_order_date = PublicFunctions.Tanggal(row.GetCell(1));

                    var shipment_plan_id = string.Empty;
                    var shipment_plan = dbContext.shipment_plan
                        .Where(x => x.lineup_number == PublicFunctions.IsNullCell(row.GetCell(2)))
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (shipment_plan != null) shipment_plan_id = shipment_plan.id;

                    var delivery_term_id = string.Empty;
                    var delivery_term = dbContext.master_list
                        .Where(x => x.item_name == PublicFunctions.IsNullCell(row.GetCell(3)))
                        .Where(x => x.item_group == "delivery-term")
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (delivery_term != null) delivery_term_id = delivery_term.id;

                    DateTime? laycan_start = DateTime.MinValue;
                    laycan_start = PublicFunctions.Tanggal(row.GetCell(4));

                    DateTime? laycan_end = DateTime.MinValue;
                    laycan_end = PublicFunctions.Tanggal(row.GetCell(5));

                    var vessel_barge_id = string.Empty;
                    var vessel = dbContext.vessel
                        .Where(x => x.vehicle_name == PublicFunctions.IsNullCell(row.GetCell(6)))
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (vessel != null) vessel_barge_id = vessel.id;
                    else
                    {
                        var barge = dbContext.barge
                            .Where(x => x.vehicle_name == PublicFunctions.IsNullCell(row.GetCell(6)))
                            .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                            .FirstOrDefault();
                        if (barge != null) vessel_barge_id = barge.id;
                    }

                    var contract_term_id = string.Empty;
                    var contract_term = dbContext.sales_contract_term
                        .Where(x => x.contract_term_name == PublicFunctions.IsNullCell(row.GetCell(7)))
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (contract_term != null) contract_term_id = contract_term.id;

                    decimal? cargo_qty = 0;
                    cargo_qty = PublicFunctions.Desimal(row.GetCell(8));

                    var uom_id = string.Empty;
                    var uom = dbContext.uom
                        .Where(x => x.uom_name == PublicFunctions.IsNullCell(row.GetCell(9)))
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (uom != null) uom_id = uom.id;

                    decimal? turn_time = 0;
                    turn_time = PublicFunctions.Desimal(row.GetCell(10));

                    decimal? loading_rate = 0;
                    loading_rate = PublicFunctions.Desimal(row.GetCell(11));

                    decimal? despatch_demurrage_rate = 0;
                    despatch_demurrage_rate = PublicFunctions.Desimal(row.GetCell(12));

                    decimal? despatch_percentage = 0;
                    despatch_percentage = PublicFunctions.Desimal(row.GetCell(13));

                    var seller_id = string.Empty;
                    var seller = dbContext.organization
                        .Where(x => x.organization_name == PublicFunctions.IsNullCell(row.GetCell(14)))
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId).
                        FirstOrDefault();
                    if (seller != null) seller_id = seller.id;

                    var buyer_id = string.Empty;
                    var buyer = dbContext.customer
                        .Where(x => x.business_partner_name == PublicFunctions.IsNullCell(row.GetCell(15)))
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (buyer != null) buyer_id = buyer.id;

                    var loading_port_id = string.Empty;
                    var loading_port = dbContext.port_location
                        .Where(x => x.stock_location_name == PublicFunctions.IsNullCell(row.GetCell(16)))
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (loading_port_id != null) loading_port_id = loading_port.id;

                    var discharge_port_name = string.Empty;
                    discharge_port_name = PublicFunctions.IsNullCell(row.GetCell(17));

                    var lc_number = string.Empty;
                    lc_number = PublicFunctions.IsNullCell(row.GetCell(18));

                    var surveyor_id = string.Empty;
                    var surveyor = dbContext.contractor
                        .Where(x => x.business_partner_name == PublicFunctions.IsNullCell(row.GetCell(19)))
                        .Where(x => x.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();
                    if (surveyor != null) surveyor_id = surveyor.id;

                    var shipping_agent = string.Empty;
                    shipping_agent = PublicFunctions.IsNullCell(row.GetCell(20));

                    var notes = string.Empty;
                    notes = PublicFunctions.IsNullCell(row.GetCell(21));

                    var royalty_number = string.Empty;
                    royalty_number = PublicFunctions.IsNullCell(row.GetCell(22));

                    var invoice_number = string.Empty;
                    invoice_number = PublicFunctions.IsNullCell(row.GetCell(23));

                    var TransactionNumber = PublicFunctions.IsNullCell(row.GetCell(0));

                    var record = dbContext.despatch_order
                        .Where(o => o.despatch_order_number.ToLower() == TransactionNumber.ToLower()
                            && o.organization_id == CurrentUserContext.OrganizationId)
                        .FirstOrDefault();

                    if (record != null)
                    {
                        var e = new entity();
                        e.InjectFrom(record);

                        record.InjectFrom(e);
                        record.modified_by = CurrentUserContext.AppUserId;
                        record.modified_on = DateTime.Now;

                        record.despatch_order_date = shipping_order_date;
                        record.despatch_plan_id = shipment_plan_id;
                        record.delivery_term_id = delivery_term_id;
                        record.laycan_start = laycan_start;
                        record.laycan_end = laycan_end;
                        record.vessel_id = vessel_barge_id;
                        record.contract_term_id = contract_term_id;
                        record.required_quantity = cargo_qty;
                        record.uom_id = uom_id;
                        record.turn_time = turn_time;
                        record.loading_rate = loading_rate;
                        record.despatch_demurrage_rate = despatch_demurrage_rate;
                        record.seller_id = seller_id;
                        record.customer_id = buyer_id;
                        record.loading_port = loading_port_id;
                        record.discharge_port = discharge_port_name;
                        record.letter_of_credit = lc_number;
                        record.surveyor_id = surveyor_id;
                        record.notes = notes; ;
                        record.royalty_number = royalty_number;
                        record.invoice_number = invoice_number;
                        #region Get transaction number
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
                                    string organizationCode = dbContext.organization
                                        .Where(o => o.id == CurrentUserContext.OrganizationId)
                                        .Select(o => o.organization_code)
                                        .FirstOrDefault() ?? "";
                                    cmd.CommandText = $"SELECT nextval('seq_despatch_order_number')";

                                    var r = await cmd.ExecuteScalarAsync();
                                    r = Convert.ToInt32(r).ToString("D3");

                                    var szYear = record.created_on.Value.Year.ToString();
                                    var szMonth = record.created_on.Value.Month.ToString();
                                    var szDay = record.created_on.Value.Day.ToString();
                                    var szNamaKapal = string.Empty;
                                    var szNamaKustomer = string.Empty;
                                    var szInvoiceNo = record.invoice_number ?? string.Empty;
                                    var discharge_port = record.discharge_port.ToString() ?? string.Empty;

                                    if (record.vessel_id != null)
                                    {
                                        var resultBarge = await dbContext.barge
                                            .Where(x => x.id == record.vessel_id).SingleOrDefaultAsync();
                                        if (resultBarge == null)
                                        {
                                            var resultVessel = await dbContext.vessel
                                                .Where(x => x.id == record.vessel_id).SingleOrDefaultAsync();
                                            if (resultVessel == null)
                                                throw new Exception("Vessel or Barge Not Found.");
                                            else
                                            {
                                                szNamaKapal = resultVessel.vehicle_name;
                                            }
                                        }
                                        else
                                        {
                                            szNamaKapal = resultBarge.vehicle_name;
                                        }
                                    }

                                    if (string.IsNullOrEmpty(szNamaKustomer))
                                    {
                                        var resultCustomer = await dbContext.customer
                                            .Where(x => x.id == record.customer_id).SingleOrDefaultAsync();

                                        szNamaKustomer = resultCustomer.business_partner_name;
                                    }

                                    record.despatch_order_number = $"SO-{szYear}/{szMonth}/{szDay}_{r}_{szNamaKapal}_({szNamaKustomer})_{discharge_port}_{szInvoiceNo}_{record.royalty_number}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }
                        #endregion
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        record = new despatch_order();
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

                        record.despatch_order_date = shipping_order_date;
                        record.despatch_plan_id = shipment_plan_id;
                        record.delivery_term_id = delivery_term_id;
                        record.laycan_start = laycan_start;
                        record.laycan_end = laycan_end;
                        record.vessel_id = vessel_barge_id;
                        record.contract_term_id = contract_term_id;
                        record.required_quantity = cargo_qty;
                        record.uom_id = uom_id;
                        record.turn_time = turn_time;
                        record.loading_rate = loading_rate;
                        record.despatch_demurrage_rate = despatch_demurrage_rate;
                        record.seller_id = seller_id;
                        record.customer_id = buyer_id;
                        record.loading_port = loading_port_id;
                        record.discharge_port = discharge_port_name;
                        record.letter_of_credit = lc_number;
                        record.surveyor_id = surveyor_id;
                        record.notes = notes; ;
                        record.royalty_number = royalty_number;
                        record.invoice_number = invoice_number;
                        
                        #region Get transaction number
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
                                    string organizationCode = dbContext.organization
                                        .Where(o => o.id == CurrentUserContext.OrganizationId)
                                        .Select(o => o.organization_code)
                                        .FirstOrDefault() ?? "";
                                    cmd.CommandText = $"SELECT nextval('seq_despatch_order_number')";

                                    var r = await cmd.ExecuteScalarAsync();
                                    r = Convert.ToInt32(r).ToString("D3");

                                    var szYear = record.created_on.Value.Year.ToString();
                                    var szMonth = record.created_on.Value.Month.ToString();
                                    var szDay = record.created_on.Value.Day.ToString();
                                    var szNamaKapal = string.Empty;
                                    var szNamaKustomer = string.Empty;
                                    var szInvoiceNo = record.invoice_number ?? string.Empty;
                                    var discharge_port = record.discharge_port.ToString() ?? string.Empty;

                                    if (record.vessel_id != null)
                                    {
                                        var resultBarge = await dbContext.barge
                                            .Where(x => x.id == record.vessel_id).SingleOrDefaultAsync();
                                        if (resultBarge == null)
                                        {
                                            var resultVessel = await dbContext.vessel
                                                .Where(x => x.id == record.vessel_id).SingleOrDefaultAsync();
                                            if (resultVessel == null)
                                                throw new Exception("Vessel or Barge Not Found.");
                                            else
                                            {
                                                szNamaKapal = resultVessel.vehicle_name;
                                            }
                                        }
                                        else
                                        {
                                            szNamaKapal = resultBarge.vehicle_name;
                                        }
                                    }

                                    if (string.IsNullOrEmpty(szNamaKustomer))
                                    {
                                        var resultCustomer = await dbContext.customer
                                            .Where(x => x.id == record.customer_id).SingleOrDefaultAsync();

                                        szNamaKustomer = resultCustomer.business_partner_name;
                                    }

                                    record.despatch_order_number = $"SO-{szYear}/{szMonth}/{szDay}_{r}_{szNamaKapal}_({szNamaKustomer})_{discharge_port}_{szInvoiceNo}_{record.royalty_number}";
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                    return BadRequest(ex.Message);
                                }
                            }
                        }
                        #endregion
                        dbContext.despatch_order.Add(record);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                        teks += "==>Error Sheet 1, Line " + (i + 1) + ": " + Environment.NewLine;
                    }
                    else errormessage = ex.Message;

                    teks += errormessage + Environment.NewLine + Environment.NewLine;
                    gagal = true;
                    break;
                }
            }

            //sheet = wb.GetSheetAt(1); //*** detail sheet
            //for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            //{
            //    try
            //    {
            //        IRow row = sheet.GetRow(i);
            //        if (row == null) continue;

            //        string despatch_order_id = null;
            //        var despatch_order = dbContext.despatch_order
            //            .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
            //                o.despatch_order_number.ToLower() == PublicFunctions.IsNullCell(row.GetCell(0)).ToLower()).FirstOrDefault();
            //        if (despatch_order != null) despatch_order_id = despatch_order.id.ToString();

            //        string delay_category_id = null;
            //        var delay_category = dbContext.delay_category
            //            .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
            //                o.delay_category_name.ToLower() == PublicFunctions.IsNullCell(row.GetCell(1)).ToLower()).FirstOrDefault();
            //        if (delay_category != null) delay_category_id = delay_category.id.ToString();

            //        string uom_id = null;
            //        var uom = dbContext.uom
            //            .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
            //                o.uom_symbol == PublicFunctions.IsNullCell(row.GetCell(6))).FirstOrDefault();
            //        if (uom != null) uom_id = uom.id.ToString();

            //        var record = dbContext.despatch_order_delay
            //            .Where(o => o.organization_id == CurrentUserContext.OrganizationId &&
            //                o.delay_category_id == delay_category_id)
            //            .FirstOrDefault();
            //        if (record != null)
            //        {
            //            var e = new entity();
            //            e.InjectFrom(record);

            //            record.InjectFrom(e);
            //            record.modified_by = CurrentUserContext.AppUserId;
            //            record.modified_on = DateTime.Now;

            //            record.demurrage_percent = PublicFunctions.Desimal(row.GetCell(2));
            //            record.despatch_percent = PublicFunctions.Desimal(row.GetCell(3));

            //            await dbContext.SaveChangesAsync();
            //        }
            //        else
            //        {
            //            record = new despatch_order_delay();
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
            //            record.business_unit_id = HttpContext.Session.GetString("BUSINESS_UNIT_ID");

            //            record.despatch_order_id = despatch_order_id;
            //            record.delay_category_id = delay_category_id;
            //            record.demurrage_percent = PublicFunctions.Desimal(row.GetCell(2));
            //            record.despatch_percent = PublicFunctions.Desimal(row.GetCell(3));

            //            dbContext.despatch_order_delay.Add(record);
            //            await dbContext.SaveChangesAsync();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        if (ex.InnerException != null)
            //        {
            //            errormessage = ex.InnerException.Message;
            //            teks += "==>Error Sheet 2, Line " + (i + 1) + ": " + Environment.NewLine;
            //        }
            //        else errormessage = ex.Message;

            //        teks += errormessage + Environment.NewLine + Environment.NewLine;
            //        gagal = true;
            //        break;
            //    }
            //}

            wb.Close();
            if (gagal)
            {
                await transaction.RollbackAsync();
                HttpContext.Session.SetString("errormessage", teks);
                HttpContext.Session.SetString("filename", "DespatchOrder");
                return BadRequest("File gagal di-upload");
            }
            else
            {
                await transaction.CommitAsync();
                return "File berhasil di-upload!";
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string secondsToDhms(decimal seconds)
        {
            var d = Math.Floor(seconds / (3600 * 24));
            var h = Math.Floor(seconds % (3600 * 24) / 3600);
            var m = Math.Floor(seconds % 3600 / 60);
            var s = Math.Floor(seconds % 60);

            var dDisplay = d > 0 ? d + (d == 1 ? " Day " : " Days ") : "";
            var hDisplay = h > 0 ? h + (h == 1 ? " Hour " : " Hours ") : "";
            var mDisplay = m > 0 ? m + (m == 1 ? " Minute " : " Minutes ") : "";
            return dDisplay + hDisplay + mDisplay;
        }

        [HttpGet("EndUserIdLookup")]
        public async Task<object> EndUserIdLookup(DataSourceLoadOptions loadOptions, string SalesContractId)
        {
            try
            {
                var lookup = dbContext.vw_sales_contract_end_user
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Where(o => o.business_unit_id == HttpContext.Session.GetString("BUSINESS_UNIT_ID") || CurrentUserContext.IsSysAdmin)
                    .Where(o => o.sales_contract_id == SalesContractId)
                    .Select(o => new { Value = o.id, Text = o.business_partner_name });
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ShipmentPlanIdLookup")]
        public async Task<object> ShipmentPlanIdLookup(string Id, DataSourceLoadOptions loadOptions)
        {
            try
            {
                //.Where(o => !dbContext.despatch_order.Select(x => x.despatch_plan_id).Contains(o.id))
                var current = dbContext.despatch_order.Select(o => o.despatch_plan_id).ToList();
                var lookup = dbContext.vw_shipment_plan
                    .Where(o => o.certain == true)
                    .Where(o => !current.Contains(o.id))
                    .Select(o => new { Value = o.id, Text = o.lineup_number });
                if (!string.IsNullOrEmpty(Id))
                {
                    var lookupId = dbContext.vw_shipment_plan
                    .Where(o => o.id == Id)
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.lineup_number });
                    lookup = lookup.Union(lookupId);
                }
                return await DataSourceLoader.LoadAsync(lookup, loadOptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("ViewShipmentPlanIdLookup")]
        public async Task<object> ViewShipmentPlanIdLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = dbContext.vw_shipment_plan
                    .Where(o => o.organization_id == CurrentUserContext.OrganizationId)
                    .Select(o => new { Value = o.id, Text = o.lineup_number });
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

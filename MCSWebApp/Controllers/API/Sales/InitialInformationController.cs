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
using SelectPdf;
using Microsoft.AspNetCore.Http.Extensions;
using Common;

namespace MCSWebApp.Controllers.API.Sales
{
    [Route("api/Sales/[controller]")]
    [ApiController]
    public class InitialInformationController : ApiBaseController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly mcsContext dbContext;

        public InitialInformationController(IConfiguration Configuration, IOptions<SysAdmin> SysAdminOption)
            : base(Configuration, SysAdminOption)
        {
            dbContext = new mcsContext(DbOptionBuilder.Options);
        }

        [HttpGet("GetByDespatchOrderId/{Id}")]
        public object GetByDespatchOrderId(string Id)
        {
            try
            {
                //var result = new initial_information();
                var result = new vw_initial_information();

                //result = dbContext.initial_information.Where(x => x.despatch_order_id == Id).FirstOrDefault();
                result = dbContext.vw_initial_information.Where(x => x.despatch_order_id == Id).FirstOrDefault();

                if (result == null)
                {
                    //result = new initial_information();
                    result = new vw_initial_information();

                    //var lookup = dbContext.despatch_order
                    //    .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Id)
                    //    .Include(x => x.customer_)
                    //    .Include(x => x.contract_product_)
                    //    .Include(x => x.seller_)
                    //    //.Include(x => x.vessel_)
                    //    .FirstOrDefault();
                    var lookup = dbContext.vw_despatch_order
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == Id)
                        .FirstOrDefault();

                    //if (lookup.contract_product_ != null)
                    //{
                    //    result.contract_product_name = lookup.contract_product_.contract_product_name;
                    //}

                    //if (lookup.seller_ != null)
                    //{
                    //    result.seller = lookup.seller_.organization_name;
                    //}

                    if (lookup != null)
                    {
                        result.contract_product_name = lookup.sales_contract_product_name;
                        result.seller = lookup.seller_name;
                        result.vessel_name = lookup.vessel_name ?? lookup.barge_name;
                        result.royalty_number = lookup.royalty_number;
                        var customer = dbContext.customer
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId && o.id == lookup.customer_id)
                        .FirstOrDefault();
                        if (customer != null)
                        {
                            result.customer_additional_info = customer.additional_information;
                            result.customer_name = customer.business_partner_name;
                            result.customer_address = customer.primary_address;
                        }

                        //if (lookup.customer_ != null)
                        //{
                        //    result.customer_additional_info = lookup.customer_.additional_information;
                        //    result.customer_name = lookup.customer_.business_partner_name;
                        //    result.customer_address = lookup.customer_.primary_address;

                        //}
                        //if(lookup.vessel_ != null)
                        //{
                        //    result.vessel_name = lookup.vessel_.vehicle_name;
                        //}

                        result.loading_port = lookup.loading_port;
                        var portLocation = dbContext.port_location
                            .Where(x => x.id == result.loading_port)
                            .Where(x => x.organization_id == CurrentUserContext.OrganizationId).FirstOrDefault();
                        if (portLocation != null) result.stock_location_name = portLocation.stock_location_name;
                        result.despatch_order_id = lookup.id;
                        result.eta_plan = lookup.eta_plan;
                        //result.status = "not sent";
                    }
                }

                var email_notification = dbContext.vw_email_notification
                .Where(o => o.organization_id == CurrentUserContext.OrganizationId 
                    && o.criteria == "despatch_order_id='" + Id + "'"
                    && o.delivered_on != null)
                .FirstOrDefault();
                if (email_notification != null)
                {
                    result.status = "Sent";
                }
                else
                    result.status = "Not sent";

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex.InnerException ?? ex);
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("InsertData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertData([FromForm] string key, [FromForm] string values)
        {
            logger.Trace($"string values = {values}");

            try
            {
                var recordID = "";
                if (await mcsContext.CanCreate(dbContext, nameof(initial_information),
                    CurrentUserContext.AppUserId) || CurrentUserContext.IsSysAdmin)
                {
                    var temp = dbContext.initial_information
                        .Where(o => o.organization_id == CurrentUserContext.OrganizationId
                            && o.despatch_order_id == key)
                        .FirstOrDefault();

                    if (temp == null)
                    {
                        var record = new initial_information();
                        JsonConvert.PopulateObject(values, record);
                        record.id = Guid.NewGuid().ToString("N");
                        record.despatch_order_id = key;
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

                        dbContext.initial_information.Add(record);
                        recordID = record.id;

                        await dbContext.SaveChangesAsync();
                        //return Ok(record);
                    }
                    else
                    {
                        JsonConvert.PopulateObject(values, temp);
                        var recordUpdate = dbContext.initial_information
                        .Where(o => o.id == temp.id)
                        .FirstOrDefault();
                        if (recordUpdate != null)
                        {
                            recordUpdate.InjectFrom(temp);
                            recordUpdate.modified_by = CurrentUserContext.AppUserId;
                            recordUpdate.modified_on = DateTime.Now;

                            recordID = temp.id;

                            await dbContext.SaveChangesAsync();
                            //return Ok(recordUpdate);
                        }
                        else
                        {
                            return BadRequest("No default organization");
                        }
                    }

                    var recEmail = new email_notification();

                    var lookup = dbContext.vw_initial_information
                        .Where(o => o.id == recordID)
                        .FirstOrDefault();
                    if (lookup != null)
                    {
                        //*** send email
                        recEmail.id = Guid.NewGuid().ToString("N");
                        recEmail.created_by = CurrentUserContext.AppUserId;
                        recEmail.created_on = DateTime.Now;
                        recEmail.modified_by = null;
                        recEmail.modified_on = null;
                        recEmail.is_active = true;
                        recEmail.is_default = null;
                        recEmail.is_locked = null;
                        recEmail.entity_id = null;
                        recEmail.owner_id = CurrentUserContext.AppUserId;
                        recEmail.organization_id = CurrentUserContext.OrganizationId;

                        recEmail.email_subject = "SHIPPING ORDER (INITIAL INFORMATION) #" + lookup.despatch_order_number;
                        var url = HttpContext.Request.GetEncodedUrl();
                        url = url.Substring(0, url.IndexOf("/api"));
                        
                        var responseText = lookup.response_text;
                        var WONumber = "";

                        //if (String.IsNullOrEmpty(responseText))
                        //    return BadRequest("WO Number is in progress.");

                        if (responseText.ToUpper().Contains("SUCCESS"))
                            WONumber = responseText.Substring(responseText.IndexOf("WO No : ") + 8, 8);
                        else
                            WONumber = lookup.response_text;

                        string teks = string.Concat("<p><strong style='style=width: 100%; font-size: 14pt; font-family: Tahoma; text-align: center'>",
                            "SHIPPING ORDER (INITIAL INFORMATION) #", lookup.despatch_order_number, "</strong>",
                            "<div> <a href=", url, "/Sales/DespatchOrder/Index?Id=", lookup.despatch_order_id, "&openEditingForm=false", 
                            ">See Shipping Order</a> </div>",
                            "<div class='container'>",
                            "  <table style='width: 100%'>",
                            "    <tbody>",
                            "      <tr>",
                            "        <td style='width: 10px'></td>",
                            "        <td>Shipping Order Date</td>",
                            "        <td>:</td>",
                            "        <td>", Convert.ToDateTime(lookup.despatch_order_date).ToString("dd/MM/yyyy"), "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Delivery Term</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.delivery_term_name, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Laycan</td>",
                            "        <td>:</td>",
                            "        <td>", Convert.ToDateTime(lookup.laycan_start).ToString("dd/MM/yyyy"), " - ", 
                                            Convert.ToDateTime(lookup.laycan_end).ToString("dd/MM/yyyy"), "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>ETA</td>",
                            "        <td>:</td>",
                            "        <td>", Convert.ToDateTime(lookup.eta_plan).ToString("dd/MM/yyyy"), "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Vessel</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.vessel_name, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Sales Contract</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.sales_contract_name, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Contract Term</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.contract_term_name, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Cargo Quantity</td>",
                            "        <td>:</td>",
                            "        <td>", string.Format("{0:#,0}", lookup.required_quantity), " MT</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Loading Rate</td>",
                            "        <td>:</td>",
                            "        <td>", string.Format("{0:#,0}", lookup.loading_rate), " MT/Day</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Despatch Demurrage Rate</td>",
                            "        <td>:</td>",
                            "        <td>", "USD. ", string.Format("{0:#,0}", lookup.despatch_demurrage_rate), "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Allowed Time</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.allowed_time, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Seller</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.seller_name, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Buyer</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.buyer_name, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Ship To</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.ship_to_name, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Contract Product</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.contract_product_name, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Product Name</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.product_name, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Loading Port</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.stock_location_name, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Discharge Port</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.discharge_port, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Surveyor</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.surveyor_name, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Shipping Agent</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.shipping_agent, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Royalty Number</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.royalty_number, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>SOA</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.record_created_by, "</td>",
                            "      </tr>",
                            "      <tr>",
                            "        <td></td>",
                            "        <td>Notes</td>",
                            "        <td>:</td>",
                            "        <td>", lookup.notes, "</td>",
                            "      </tr>",
                            "    </tbody>",
                            "  </table>",
                            "</div>");

                        recEmail.email_content = teks;

                        //recEmail.recipients = "m.darwin@adiraja-integrasi.com, ahmad.labib@adiraja-integrasi.com";
                        recEmail.delivery_schedule = DateTime.Now;
                        recEmail.table_name = "vw_initial_information";
                        recEmail.fields = "customer_name, customer_address";
                        recEmail.criteria = string.Format("despatch_order_id='{0}'", key);
                        recEmail.email_code = "Initial Information-" + lookup.despatch_order_number;
                        recEmail.recipients = "AIMS-ShippingOrder@arutmin.com";

                        string AttachmentFolder = "do_documents";
                        var uploadPath = configuration["Path:UploadPath"].ToString();
                        string filename = "Initial Information-" + lookup.despatch_order_number + ".pdf";
                        filename = uploadPath + AttachmentFolder + "\\" + filename;
                        recEmail.attachment_file = filename;

                        HtmlToPdf converter = new HtmlToPdf();
                        PdfDocument doc = converter.ConvertHtmlString(teks);
                        doc.Save(filename);
                        doc.Close();

                        dbContext.email_notification.Add(recEmail);

                        //*** end send email
                    }
                    else
                    {
                        return BadRequest("There is no new Initial Information.");
                    }

                    await dbContext.SaveChangesAsync();

                    return Ok(recEmail);
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


        [HttpDelete("DeleteData")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteData([FromForm] string key)
        {
            logger.Debug($"string key = {key}");

            try
            {
                var record = dbContext.initial_information
                    .Where(o => o.id == key)
                    .FirstOrDefault();
                if (record != null)
                {
                    dbContext.initial_information.Remove(record);
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

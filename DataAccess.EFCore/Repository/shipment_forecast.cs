using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class shipment_forecast
    {
        public string id { get; set; }
        public string created_by { get; set; }
        public DateTime? created_on { get; set; }
        public string modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool? is_active { get; set; }
        public bool? is_locked { get; set; }
        public bool? is_default { get; set; }
        public string owner_id { get; set; }
        public string organization_id { get; set; }
        public string entity_id { get; set; }
        public string business_unit_id { get; set; }
        public string despatch_order_id { get; set; }
        public decimal? year { get; set; }
        public decimal? month { get; set; }
        public string no { get; set; }
        public string customer_id { get; set; }
        public string business_partner_name { get; set; }
        public string country_name { get; set; }
        public string shipment_no { get; set; }
        public decimal? total_shipment { get; set; }
        public string delivery_term { get; set; }
        public string vessel_id { get; set; }
        public string vessel { get; set; }
        public DateTime? laycan_start { get; set; }
        public DateTime? laycan_end { get; set; }
        public DateTime? order_reference_date { get; set; }
        public decimal? quantity_plan { get; set; }
        public decimal? quantity_actual { get; set; }
        public DateTime? eta { get; set; }
        public DateTime? comm_date { get; set; }
        public DateTime? bl_date { get; set; }
        public string remark { get; set; }
        public string traffic_officer { get; set; }
        public string payment_method { get; set; }
        public string destination_bank { get; set; }
        public string invoice_ref { get; set; }
        public decimal? invoice_amount { get; set; }
        public DateTime? invoice_date { get; set; }
        public decimal? invoice_price { get; set; }
        public decimal? exchange_rate { get; set; }
        public string si_currency_id { get; set; }
        public string scpt_currency_id { get; set; }
        public DateTime? invoice_due_date { get; set; }
        public string tax_invoice_ref_no { get; set; }
        public DateTime? payment_receiving_date { get; set; }
        public decimal? amount_received { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

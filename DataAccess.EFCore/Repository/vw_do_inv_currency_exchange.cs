using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_do_inv_currency_exchange
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
        public string despatch_order_number { get; set; }
        public string sales_order_id { get; set; }
        public decimal? quantity { get; set; }
        public string uom_id { get; set; }
        public DateTime? despatch_order_date { get; set; }
        public DateTime? planned_despatch_date { get; set; }
        public string contract_term_id { get; set; }
        public string customer_id { get; set; }
        public string seller_id { get; set; }
        public string ship_to { get; set; }
        public string contract_product_id { get; set; }
        public decimal? required_quantity { get; set; }
        public decimal? final_quantity { get; set; }
        public string fulfilment_type_id { get; set; }
        public string delivery_term_id { get; set; }
        public string notes { get; set; }
        public string despatch_plan_id { get; set; }
        public DateTime? bill_of_lading_date { get; set; }
        public string vessel_id { get; set; }
        public DateTime? eta_plan { get; set; }
        public DateTime? laycan_start { get; set; }
        public DateTime? laycan_end { get; set; }
        public bool? laycan_committed { get; set; }
        public bool? vessel_committed { get; set; }
        public bool? eta_committed { get; set; }
        public bool? edit_loading_rate { get; set; }
        public bool? edit_despatch_rate { get; set; }
        public string loading_port { get; set; }
        public string discharge_port { get; set; }
        public DateTime? order_reference_date { get; set; }
        public string document_reference_id { get; set; }
        public string letter_of_credit { get; set; }
        public string port_location_id { get; set; }
        public string surveyor_id { get; set; }
        public string shipping_agent { get; set; }
        public decimal? laytime_duration { get; set; }
        public string laytime_text { get; set; }
        public decimal? loading_rate { get; set; }
        public decimal? despatch_demurrage_rate { get; set; }
        public string currency_id { get; set; }
        public string currency_code { get; set; }
        public string currency_name { get; set; }
        public DateTime? exchange_date { get; set; }
    }
}

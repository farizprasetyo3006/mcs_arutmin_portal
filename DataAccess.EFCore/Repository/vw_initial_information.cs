using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_initial_information
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
        public string despatch_order_id { get; set; }
        public DateTime? despatch_order_date { get; set; }
        public string despatch_order_number { get; set; }
        public string delivery_term_id { get; set; }
        public string delivery_term_name { get; set; }
        public DateTime? laycan_start { get; set; }
        public DateTime? laycan_end { get; set; }
        public DateTime? eta_plan { get; set; }
        public decimal? required_quantity { get; set; }
        public string uom_name { get; set; }
        public string shipping_agent { get; set; }
        public string contract_product_name { get; set; }
        public string product_name { get; set; }
        public string loading_port { get; set; }
        public string stock_location_name { get; set; }
        public string discharge_port { get; set; }
        public string vessel_name { get; set; }
        public string sales_contract_name { get; set; }
        public string contract_term_name { get; set; }
        public string sales_contract_id { get; set; }
        public decimal? loading_rate { get; set; }
        public string allowed_time { get; set; }
        public string seller_name { get; set; }
        public string buyer_name { get; set; }
        public string ship_to_name { get; set; }
        public string surveyor_name { get; set; }
        public string status { get; set; }
        public string seller { get; set; }
        public string customer_name { get; set; }
        public string customer_address { get; set; }
        public string customer_additional_info { get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
        public string subject { get; set; }
        public string cc { get; set; }
        public string notes { get; set; }
        public decimal? despatch_demurrage_rate { get; set; }
        public string response_text { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string royalty_number { get; set; }
    }
}

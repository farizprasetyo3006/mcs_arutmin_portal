using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_despatch_demurrage_invoice
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
        public string invoice_number { get; set; }
        public DateTime? invoice_date { get; set; }
        public string despatch_order_id { get; set; }
        public string despatch_order_number { get; set; }
        public string vessel_id { get; set; }
        public string customer_name { get; set; }
        public string valuation_target_type { get; set; }
        public string ship_to_name { get; set; }
        public string vessel_name { get; set; }
        public string sof_id { get; set; }
        public string sof_name { get; set; }
        public string sof_number { get; set; }
        public decimal? laytime_used_duration { get; set; }
        public string laytime_used_text { get; set; }
        public decimal? laytime_allowed_duration { get; set; }
        public string laytime_allowed_text { get; set; }
        public decimal? rate { get; set; }
        public string notes { get; set; }
        public string currency_id { get; set; }
        public string currency_name { get; set; }
        public string currency_code { get; set; }
        public string total_time { get; set; }
        public decimal? total_price { get; set; }
        public decimal? total_price_final { get; set; }
        public string laytime_used_final { get; set; }
        public string invoice_type { get; set; }
        public string invoice_status { get; set; }
        public string invoice_status_name { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

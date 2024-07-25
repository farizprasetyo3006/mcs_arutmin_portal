using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_royalty_cost
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
        public string currency_exchange_id { get; set; }
        public string port_load_id { get; set; }
        public string discharge_port_id { get; set; }
        public decimal? dist_barge_to_anchorage { get; set; }
        public decimal? barging_cost { get; set; }
        public decimal? freight_cost { get; set; }
        public decimal? transhipment_cost { get; set; }
        public decimal? total_join_cost { get; set; }
        public string barge_name { get; set; }
        public string barge_size { get; set; }
        public string despatch_order_id { get; set; }
        public string despatch_order_number { get; set; }
        public string status_code { get; set; }
        public string status_name { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

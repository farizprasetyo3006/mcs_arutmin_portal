using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_royalty_pricing
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
        public string destination_id { get; set; }
        public string currency_exchange_id { get; set; }
        public string destination_code { get; set; }
        public string despatch_order_id { get; set; }
        public string despatch_order_number { get; set; }
        public DateTime? bl_date { get; set; }
        public decimal? volume_loading { get; set; }
        public DateTime? royalty_date { get; set; }
        public string status_code { get; set; }
        public decimal? fob_price { get; set; }
        public decimal? freight_cost { get; set; }
        public decimal? total_selling_price { get; set; }
        public decimal? total_amount { get; set; }
        public decimal? hba_0 { get; set; }
        public string hba_type { get; set; }
        public decimal? hba_value { get; set; }
        public string formula { get; set; }
        public decimal? hpb_vessel { get; set; }
        public decimal? hpb_barge { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

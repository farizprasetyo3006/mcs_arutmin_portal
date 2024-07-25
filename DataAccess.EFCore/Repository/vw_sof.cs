using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sof
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
        public string sof_name { get; set; }
        public string sof_number { get; set; }
        public string despatch_order_id { get; set; }
        public string despatch_order_number { get; set; }
        public DateTime? despatch_order_date { get; set; }
        public string customer_id { get; set; }
        public string business_partner_name { get; set; }
        public string vessel_id { get; set; }
        public string desdem_term_id { get; set; }
        public string despatch_demurrage_id { get; set; }
        public string contract_name { get; set; }
        public string vessel_name { get; set; }
        public decimal? maximum_capacity { get; set; }
        public bool? is_geared { get; set; }
        public DateTime? nor_tendered { get; set; }
        public DateTime? nor_accepted { get; set; }
        public DateTime? laytime_commenced { get; set; }
        public DateTime? commenced_loading { get; set; }
        public DateTime? completed_loading { get; set; }
        public DateTime? laytime_completed { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string wlcr { get; set; }
        public DateTime? arrival_datetime { get; set; }
    }
}

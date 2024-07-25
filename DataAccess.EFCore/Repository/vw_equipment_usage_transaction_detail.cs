using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_equipment_usage_transaction_detail
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
        public string equipment_usage_transaction_id { get; set; }
        public string equipment_usage_number { get; set; }
        public string accounting_period_id { get; set; }
        public DateTime? start_datetime { get; set; }
        public DateTime? end_datetime { get; set; }
        public string equipment_usage_transaction_note { get; set; }
        public string cn_unit_id { get; set; }
        public string equipment_name { get; set; }
        public string equipment_code { get; set; }
        public string vehicle_name { get; set; }
        public string event_category_id { get; set; }
        public string event_category_name { get; set; }
        public string event_category_code { get; set; }
        public DateTime? date { get; set; }
        public decimal? duration { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

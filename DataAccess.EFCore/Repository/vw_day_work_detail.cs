using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_day_work_detail
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
        public string equipment_id { get; set; }
        public string accounting_period_id { get; set; }
        public DateTime? date_time { get; set; }
        public decimal? hm_start { get; set; }
        public decimal? hm_end { get; set; }
        public decimal? hm_duration { get; set; }
        public decimal? hm_rate { get; set; }
        public decimal? hm_value { get; set; }
        public string note { get; set; }
        public string day_work_id { get; set; }
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public DateTime? transaction_date { get; set; }
        public string advance_contract_reference_id { get; set; }
        public string advance_contract_id { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_advance_contract_reference
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
        public string advance_contract_id { get; set; }
        public string advance_contract_number { get; set; }
        public string progress_claim_name { get; set; }
        public string reference_number { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string accounting_period_id { get; set; }
        public string name { get; set; }
        public string accounting_period_name { get; set; }
        public bool? accounting_period_is_closed { get; set; }
        public string note { get; set; }
        public string advance_contract_item_id { get; set; }
        public string item_name { get; set; }
        public decimal? target_quantity { get; set; }
        public string target_uom_id { get; set; }
        public string target_uom_name { get; set; }
        public string target_uom_symbol { get; set; }
        public decimal? actual_quantity { get; set; }
        public string actual_uom_id { get; set; }
        public string actual_uom_name { get; set; }
        public string actual_uom_symbol { get; set; }
        public string join_survey_id { get; set; }
        public string join_survey_number { get; set; }
        public DateTime? join_survey_date { get; set; }
        public string approved_by { get; set; }
        public string approver_name { get; set; }
        public DateTime? approved_on { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

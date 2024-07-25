using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_advance_contract_valuation
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
        public string advance_contract_reference_id { get; set; }
        public string advance_contract_id { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string progress_claim_name { get; set; }
        public string accounting_period_id { get; set; }
        public string reference_number { get; set; }
        public string advance_contract_reference_note { get; set; }
        public decimal? target_quantity { get; set; }
        public decimal? actual_quantity { get; set; }
        public string advance_contract_valuation_number { get; set; }
        public decimal? total_value { get; set; }
        public decimal? total_valuation { get; set; }
        public decimal? total { get; set; }
        public string notes { get; set; }
        public string employee_id { get; set; }
        public string employee_name { get; set; }
        public string employee_number { get; set; }
        public DateTime? valuation_date { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

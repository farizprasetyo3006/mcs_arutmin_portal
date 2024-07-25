using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_progress_claim_charge
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
        public string progress_claim_id { get; set; }
        public string progress_claim_name { get; set; }
        public string advance_contract_id { get; set; }
        public string advance_contract_number { get; set; }
        public string reference_number { get; set; }
        public string charge_name { get; set; }
        public decimal? charge_amount { get; set; }
        public string charge_currency_id { get; set; }
        public string currency_name { get; set; }
        public string currency_symbol { get; set; }
        public string note { get; set; }
        public bool? is_calculated { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

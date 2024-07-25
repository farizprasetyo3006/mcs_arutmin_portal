using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_joint_survey
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
        public string join_survey_number { get; set; }
        public DateTime? join_survey_date { get; set; }
        public string accounting_period_id { get; set; }
        public string accounting_period_name { get; set; }
        public bool? accounting_period_is_closed { get; set; }
        public string progress_claim_id { get; set; }
        public string progress_claim_name { get; set; }
        public string advance_contract_number { get; set; }
        public string advance_contract_id { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string sampling_template_id { get; set; }
        public string sampling_template_name { get; set; }
        public string surveyor_id { get; set; }
        public string surveyor_name { get; set; }
        public string surveyor_code { get; set; }
        public string approved_by { get; set; }
        public decimal? quantity { get; set; }
        public decimal? quantity_carry_over { get; set; }
        public decimal? distance { get; set; }
        public decimal? distance_carry_over { get; set; }
        public decimal? elevation { get; set; }
        public decimal? elevation_carry_over { get; set; }
        public string uom_id { get; set; }
        public string note { get; set; }
        public string advance_contract_reference_id { get; set; }
        public string transport_model { get; set; }
        public string mine_location_id { get; set; }
        public string mine_location_code { get; set; }
        public string mine_location_name { get; set; }
        public string stockpile_location_id { get; set; }
        public string stockpile_location_code { get; set; }
        public string stockpile_location_name { get; set; }
        public string port_location_id { get; set; }
        public string port_location_code { get; set; }
        public string port_location_name { get; set; }
        public string waste_location_id { get; set; }
        public string waste_location_code { get; set; }
        public string waste_location_name { get; set; }
        public string approver_name { get; set; }
        public DateTime? approved_on { get; set; }
        public string business_process_id { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string location_id { get; set; }
    }
}

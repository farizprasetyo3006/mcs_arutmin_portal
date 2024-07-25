using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_advance_contract_charge_detail
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
        public string advance_contract_charge_id { get; set; }
        public string advance_contract_detail_id { get; set; }
        public string advance_contract_charge_name { get; set; }
        public decimal? advance_contract_detail_amount { get; set; }
        public string advance_contract_detail_variable { get; set; }
        public string advance_contract_detail_currency_id { get; set; }
        public string currency_code { get; set; }
        public string currency_symbol { get; set; }
        public string currency_name { get; set; }
        public string price_index_id { get; set; }
        public string price_index_name { get; set; }
        public string price_index_code { get; set; }
        public string joint_survey_id { get; set; }
        public string joint_survey_number { get; set; }
        public decimal? joint_survey_quantity { get; set; }
        public string formula { get; set; }
        public string variable { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

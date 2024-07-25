using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_contract_quotation_price
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
        public string sales_contract_term_id { get; set; }
        public string contract_term_name { get; set; }
        public string quotation_type_id { get; set; }
        public string quotation_type_name { get; set; }
        public string pricing_method_id { get; set; }
        public string pricing_method_name { get; set; }
        public string pricing_method_notes { get; set; }
        public string pricing_method_in_coding { get; set; }
        public string price_index_id { get; set; }
        public string price_index_name { get; set; }
        public string frequency_id { get; set; }
        public string frequency_name { get; set; }
        public string quotation_period_freq_id { get; set; }
        public string quotation_period { get; set; }
        public string frequency_period { get; set; }
        public string quotation_period_desc { get; set; }
        public string index_calculation_id { get; set; }
        public string index_calculation_name { get; set; }
        public string index_calculation_code { get; set; }
        public decimal? price_value { get; set; }
        public decimal? decimal_places { get; set; }
        public string currency_id { get; set; }
        public string currency_code { get; set; }
        public string currency_name { get; set; }
        public decimal? weightening_value { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string quotation_uom_id { get; set; }
        public string quotation_uom_name { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

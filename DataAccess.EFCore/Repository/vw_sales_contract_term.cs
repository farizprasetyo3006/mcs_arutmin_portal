using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_contract_term
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
        public string sales_contract_id { get; set; }
        public string contract_name { get; set; }
        public string contract_term_name { get; set; }
        public decimal? quantity { get; set; }
        public decimal? minimum_order { get; set; }
        public decimal? maximum_order { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string sales_charge_id { get; set; }
        public string sales_charge_name { get; set; }
        public string notes { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string uom_symbol { get; set; }
        public string charge_formula { get; set; }
        public string currency_id { get; set; }
        public string currency_code { get; set; }
        public string currency_name { get; set; }
        public string organization_name { get; set; }
        public string sales_contract_product_id { get; set; }
        public string sales_contract_despatch_demurrage_term_id { get; set; }
        public decimal? turn_time { get; set; }
        public decimal? loading_rate_geared { get; set; }
        public decimal? loading_rate_gearless { get; set; }
        public decimal? despatch_demurrage_term_rate { get; set; }
        public string despatch_demurrage_term_curency_id { get; set; }
        public decimal? despatch_percentage { get; set; }
        public string rounding_type_name { get; set; }
        public string index_calculation_id { get; set; }
        public string index_calculation_type_name { get; set; }
        public decimal? decimal_places { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string product_id { get; set; }
    }
}

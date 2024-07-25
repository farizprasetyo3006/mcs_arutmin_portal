using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_invoice_test1
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
        public string despatch_order_id { get; set; }
        public decimal? quantity { get; set; }
        public string uom_id { get; set; }
        public decimal? unit_price { get; set; }
        public decimal? amount { get; set; }
        public decimal? final_price { get; set; }
        public decimal? final_amount { get; set; }
        public string currency_id { get; set; }
        public DateTime? invoice_date { get; set; }
        public string invoice_number { get; set; }
        public string accounting_period_id { get; set; }
        public string sales_type_id { get; set; }
        public string invoice_type_id { get; set; }
        public string customer_id { get; set; }
        public string bank_account_id { get; set; }
        public string account_number { get; set; }
        public string account_holder { get; set; }
        public string branch_information { get; set; }
        public string bank_id { get; set; }
        public string bank_name { get; set; }
        public string bank_code { get; set; }
        public string despatch_order_number { get; set; }
        public string sales_order_id { get; set; }
        public string sales_order_number { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string uom_name { get; set; }
        public string uom_symbol { get; set; }
        public string currency_name { get; set; }
        public string currency_code { get; set; }
        public string currency_symbol { get; set; }
        public string accounting_period_name { get; set; }
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string sales_type_name { get; set; }
        public string invoice_type_name { get; set; }
        public string customer_name { get; set; }
        public string seller_name { get; set; }
        public string seller_id { get; set; }
        public string bill_to_name { get; set; }
        public string bill_to { get; set; }
        public string contract_product_name { get; set; }
        public string contract_product_id { get; set; }
        public string analyte_name { get; set; }
        public string analyte_standard_name { get; set; }
        public decimal? sips_value { get; set; }
        public decimal? sips_target { get; set; }
        public string sales_contract_name { get; set; }
        public string sales_contract_id { get; set; }
        public string sales_contract_term_name { get; set; }
        public string sales_contract_term_id { get; set; }
        public string notes { get; set; }
        public int? adjustment { get; set; }
        public int? ash_price_adj { get; set; }
        public int? ts_price_adj { get; set; }
        public int? cv_price_adj { get; set; }
        public string sales_charge_code { get; set; }
        public decimal? price { get; set; }
    }
}

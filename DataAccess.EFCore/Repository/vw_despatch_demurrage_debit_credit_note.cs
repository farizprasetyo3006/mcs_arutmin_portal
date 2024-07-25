using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_despatch_demurrage_debit_credit_note
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
        public string debit_credit_number { get; set; }
        public string despatch_demurrage_invoice_id { get; set; }
        public string valuation_number { get; set; }
        public string desdem_valuation_type { get; set; }
        public string total_time { get; set; }
        public DateTime? debit_credit_date { get; set; }
        public string valuation_target_type { get; set; }
        public string valuation_target_id { get; set; }
        public string despatch_order_id { get; set; }
        public string despatch_order_number { get; set; }
        public string vessel_id { get; set; }
        public string vessel_name { get; set; }
        public string cow_id { get; set; }
        public decimal? cow_quantity { get; set; }
        public DateTime? cow_bill_lading_date { get; set; }
        public string customer_id { get; set; }
        public string contractor_id { get; set; }
        public string valuation_target_name { get; set; }
        public string attn { get; set; }
        public string currency_id { get; set; }
        public string currency_name { get; set; }
        public string currency_code { get; set; }
        public decimal? rate { get; set; }
        public decimal? total_price { get; set; }
        public string total_price_text { get; set; }
        public string bank_account_id { get; set; }
        public string bank_account_number { get; set; }
        public string bank_account_holder { get; set; }
        public string bank_branch_information { get; set; }
        public string bank_swift_code { get; set; }
        public string correspondent_bank_id { get; set; }
        public string correspondent_account_number { get; set; }
        public string correspondent_account_holder { get; set; }
        public string correspondent_branch_information { get; set; }
        public string correspondent_swift_code { get; set; }
        public string currency_exchange_id { get; set; }
        public DateTime? currency_exchange_start_date { get; set; }
        public DateTime? currency_exchange_end_date { get; set; }
        public decimal? currency_exchange_rate { get; set; }
        public string notes { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string re { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

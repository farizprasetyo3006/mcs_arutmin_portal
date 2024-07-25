using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_invoice_overdue
    {
        public string invoice_number { get; set; }
        public string sales_invoice_payment { get; set; }
        public DateTime? invoice_date { get; set; }
        public decimal? number_of_days { get; set; }
        public DateTime? overdue_date { get; set; }
        public DateTime? current_date { get; set; }
        public double? overdue_day { get; set; }
        public decimal? credit_limit { get; set; }
        public decimal? total_price { get; set; }
        public DateTime? created_on { get; set; }
        public string created_by { get; set; }
        public decimal? payment_value { get; set; }
        public DateTime? payment_date { get; set; }
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
        public string contract_product_id { get; set; }
        public string sales_contract_term_id { get; set; }
        public string sales_contract_id { get; set; }
        public string customer_id { get; set; }
        public string currency_id { get; set; }
        public string currency_code { get; set; }
        public string currency_name { get; set; }
        public string currency_symbol { get; set; }
    }
}

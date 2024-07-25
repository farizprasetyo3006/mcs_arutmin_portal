using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_customer_transaction_history
    {
        public string business_partner_name { get; set; }
        public decimal? credit_limit { get; set; }
        public string sales_contract_name { get; set; }
        public string contract_term_name { get; set; }
        public string despatch_plan_name { get; set; }
        public string despatch_order_number { get; set; }
        public string invoice_number { get; set; }
        public DateTime? invoice_date { get; set; }
        public decimal? quantity { get; set; }
        public decimal? unit_price { get; set; }
        public decimal? outstanding { get; set; }
        public decimal? receipt { get; set; }
        public DateTime? receipt_date { get; set; }
        public string id { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
    }
}

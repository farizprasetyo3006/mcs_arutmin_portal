using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_invoice_ell
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
        public string business_unit_id { get; set; }
        public string despatch_order_id { get; set; }
        public decimal? quantity { get; set; }
        public string uom_id { get; set; }
        public decimal? unit_price { get; set; }
        public string currency_id { get; set; }
        public DateTime? invoice_date { get; set; }
        public string accounting_period_id { get; set; }
        public string invoice_number { get; set; }
        public string sales_type_id { get; set; }
        public string invoice_type_id { get; set; }
        public string customer_id { get; set; }
        public string seller_id { get; set; }
        public string bill_to { get; set; }
        public string contract_product_id { get; set; }
        public string notes { get; set; }
        public string bank_account_id { get; set; }
        public decimal? downpayment { get; set; }
        public decimal? total_price { get; set; }
        public string quotation_type_id { get; set; }
        public string currency_exchange_id { get; set; }
        public string lc_status { get; set; }
        public string lc_date_issue { get; set; }
        public string lc_issuing_bank { get; set; }
        public decimal? freight_cost { get; set; }
        public string correspondent_bank_id { get; set; }
        public string sync_id { get; set; }
        public string sync_type { get; set; }
        public string sync_status { get; set; }
        public string error_msg { get; set; }
        public string response_code { get; set; }
        public string response_text { get; set; }
        public bool? canceled { get; set; }
    }
}

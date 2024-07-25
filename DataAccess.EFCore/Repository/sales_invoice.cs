using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_invoice
    {
        public sales_invoice()
        {
            sales_invoice_attachment = new HashSet<sales_invoice_attachment>();
            sales_invoice_detail = new HashSet<sales_invoice_detail>();
            sales_invoice_html_content = new HashSet<sales_invoice_html_content>();
            sales_invoice_trace_element = new HashSet<sales_invoice_trace_element>();
            sales_invoice_transhipment = new HashSet<sales_invoice_transhipment>();
        }

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
        public decimal quantity { get; set; }
        public string uom_id { get; set; }
        public decimal unit_price { get; set; }
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
        public string transaction_code { get; set; }
        public string print_status { get; set; }
        public string description_of_goods { get; set; }
        public DateTime? alias1 { get; set; }
        public DateTime? alias2 { get; set; }
        public DateTime? alias3 { get; set; }
        public DateTime? alias4 { get; set; }
        public decimal? alias5 { get; set; }
        public decimal? alias6 { get; set; }
        public decimal? alias7 { get; set; }
        public decimal? alias8 { get; set; }
        public decimal? alias9 { get; set; }
        public string alias10 { get; set; }
        public string alias11 { get; set; }
        public string alias12 { get; set; }
        public string alias13 { get; set; }
        public string alias14 { get; set; }
        public string alias15 { get; set; }
        public decimal? insurance_cost { get; set; }
        public decimal? subtotal { get; set; }
        public string consignee { get; set; }
        public string approve_status { get; set; }
        public string approve_by_id { get; set; }
        public string disapprove_by_id { get; set; }
        public string alias16 { get; set; }
        public string alias17 { get; set; }
        public string alias18 { get; set; }
        public DateTime? alias19 { get; set; }
        public string alias20 { get; set; }
        public string alias21 { get; set; }
        public string response_code { get; set; }
        public string inco_term { get; set; }
        public string szfreightinvoiceno { get; set; }
        public decimal? decfreightpriceton { get; set; }
        public decimal? decfreightamount { get; set; }
        public DateTime? dtmfreightinvoicedatereceive { get; set; }
        public string szfreightcontractno { get; set; }
        public string hba_type { get; set; }
        public decimal? hba_value { get; set; }
        public string formula { get; set; }
        public decimal? hpb { get; set; }
        public decimal? hpb_value { get; set; }
        public bool? is_fobprice { get; set; }
        public decimal? total_price_hpb { get; set; }
        public decimal? fob_price { get; set; }
        public decimal? gcv_arb { get; set; }
        public decimal? gcv_adb { get; set; }
        public decimal? ts_arb { get; set; }
        public decimal? ts_adb { get; set; }
        public decimal? ash_arb { get; set; }
        public decimal? ash_adb { get; set; }
        public decimal? tm_arb { get; set; }
        public decimal? im_adb { get; set; }
        public string loading_port_id { get; set; }

        public virtual bank_account bank_account_ { get; set; }
        public virtual despatch_order despatch_order_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<sales_invoice_attachment> sales_invoice_attachment { get; set; }
        public virtual ICollection<sales_invoice_detail> sales_invoice_detail { get; set; }
        public virtual ICollection<sales_invoice_html_content> sales_invoice_html_content { get; set; }
        public virtual ICollection<sales_invoice_trace_element> sales_invoice_trace_element { get; set; }
        public virtual ICollection<sales_invoice_transhipment> sales_invoice_transhipment { get; set; }
    }
}

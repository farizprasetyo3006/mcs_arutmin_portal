using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class currency
    {
        public currency()
        {
            advance_contract_detail = new HashSet<advance_contract_detail>();
            benchmark_price_series_detail = new HashSet<benchmark_price_series_detail>();
            currency_exchangesource_currency_ = new HashSet<currency_exchange>();
            currency_exchangetarget_currency_ = new HashSet<currency_exchange>();
            despatch_demurrage_debit_credit_note = new HashSet<despatch_demurrage_debit_credit_note>();
            despatch_demurrage_invoice = new HashSet<despatch_demurrage_invoice>();
            sales_contract_despatch_demurrage_term = new HashSet<sales_contract_despatch_demurrage_term>();
            sales_contract_quotation_price = new HashSet<sales_contract_quotation_price>();
            sales_contract_term = new HashSet<sales_contract_term>();
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
        public string currency_code { get; set; }
        public string currency_symbol { get; set; }
        public string currency_name { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<advance_contract_detail> advance_contract_detail { get; set; }
        public virtual ICollection<benchmark_price_series_detail> benchmark_price_series_detail { get; set; }
        public virtual ICollection<currency_exchange> currency_exchangesource_currency_ { get; set; }
        public virtual ICollection<currency_exchange> currency_exchangetarget_currency_ { get; set; }
        public virtual ICollection<despatch_demurrage_debit_credit_note> despatch_demurrage_debit_credit_note { get; set; }
        public virtual ICollection<despatch_demurrage_invoice> despatch_demurrage_invoice { get; set; }
        public virtual ICollection<sales_contract_despatch_demurrage_term> sales_contract_despatch_demurrage_term { get; set; }
        public virtual ICollection<sales_contract_quotation_price> sales_contract_quotation_price { get; set; }
        public virtual ICollection<sales_contract_term> sales_contract_term { get; set; }
    }
}

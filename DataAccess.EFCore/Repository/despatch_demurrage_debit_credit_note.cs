using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class despatch_demurrage_debit_credit_note
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
        public string debit_credit_number { get; set; }
        public string despatch_demurrage_invoice_id { get; set; }
        public DateTime? debit_credit_date { get; set; }
        public string valuation_target_type { get; set; }
        public string valuation_target_id { get; set; }
        public string attn { get; set; }
        public decimal rate { get; set; }
        public string currency_id { get; set; }
        public decimal? total_price { get; set; }
        public string total_price_text { get; set; }
        public string bank_account_id { get; set; }
        public string correspondent_bank_id { get; set; }
        public string currency_exchange_id { get; set; }
        public string notes { get; set; }
        public string re { get; set; }

        public virtual bank_account bank_account_ { get; set; }
        public virtual bank_account correspondent_bank_ { get; set; }
        public virtual currency currency_ { get; set; }
        public virtual currency_exchange currency_exchange_ { get; set; }
        public virtual despatch_demurrage_invoice despatch_demurrage_invoice_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

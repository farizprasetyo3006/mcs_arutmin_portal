using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class currency_exchange
    {
        public currency_exchange()
        {
            despatch_demurrage_debit_credit_note = new HashSet<despatch_demurrage_debit_credit_note>();
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
        public string source_currency_id { get; set; }
        public string target_currency_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public decimal exchange_rate { get; set; }
        public decimal selling_rate { get; set; }
        public decimal buying_rate { get; set; }
        public string exchange_type_id { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual currency source_currency_ { get; set; }
        public virtual currency target_currency_ { get; set; }
        public virtual ICollection<despatch_demurrage_debit_credit_note> despatch_demurrage_debit_credit_note { get; set; }
    }
}

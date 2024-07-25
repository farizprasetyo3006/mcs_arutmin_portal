using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class bank_account
    {
        public bank_account()
        {
            despatch_demurrage_debit_credit_notebank_account_ = new HashSet<despatch_demurrage_debit_credit_note>();
            despatch_demurrage_debit_credit_notecorrespondent_bank_ = new HashSet<despatch_demurrage_debit_credit_note>();
            sales_invoice = new HashSet<sales_invoice>();
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
        public string bank_id { get; set; }
        public string account_number { get; set; }
        public string account_holder { get; set; }
        public string swift_code { get; set; }
        public string branch_information { get; set; }
        public string currency_id { get; set; }

        public virtual bank bank_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<despatch_demurrage_debit_credit_note> despatch_demurrage_debit_credit_notebank_account_ { get; set; }
        public virtual ICollection<despatch_demurrage_debit_credit_note> despatch_demurrage_debit_credit_notecorrespondent_bank_ { get; set; }
        public virtual ICollection<sales_invoice> sales_invoice { get; set; }
    }
}

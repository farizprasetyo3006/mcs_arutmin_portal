using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_contract_payment_term
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
        public string sales_contract_term_id { get; set; }
        public string invoice_type_id { get; set; }
        public string currency_id { get; set; }
        public decimal? exchange_rate { get; set; }
        public string payment_method_id { get; set; }
        public string reference_date_id { get; set; }
        public decimal? number_of_days { get; set; }
        public string days_type_id { get; set; }
        public string calendar_id { get; set; }
        public decimal? downpayment_value { get; set; }
        public string exchange_date_id { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

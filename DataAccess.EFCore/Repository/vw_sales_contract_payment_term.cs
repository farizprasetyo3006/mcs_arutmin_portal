using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_contract_payment_term
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
        public string sales_contract_term_id { get; set; }
        public string contract_term_name { get; set; }
        public string invoice_type_id { get; set; }
        public string invoice_type_name { get; set; }
        public string invoice_in_coding { get; set; }
        public string currency_id { get; set; }
        public string currency_code { get; set; }
        public string currency_name { get; set; }
        public decimal? exchange_rate { get; set; }
        public decimal? downpayment_value { get; set; }
        public string payment_method_id { get; set; }
        public string payment_method_name { get; set; }
        public string reference_date_id { get; set; }
        public string reference_date_name { get; set; }
        public string exchange_date_id { get; set; }
        public string exchange_date_name { get; set; }
        public decimal? number_of_days { get; set; }
        public string days_type_id { get; set; }
        public string days_type_name { get; set; }
        public string calendar_id { get; set; }
        public string calendar_name { get; set; }
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

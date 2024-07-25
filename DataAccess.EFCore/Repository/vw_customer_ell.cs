using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_customer_ell
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
        public string organization_code { get; set; }
        public string entity_id { get; set; }
        public string business_partner_name { get; set; }
        public string business_partner_code { get; set; }
        public bool? is_vendor { get; set; }
        public bool? is_customer { get; set; }
        public bool? is_company { get; set; }
        public bool? is_government { get; set; }
        public string primary_address { get; set; }
        public string primary_contact_name { get; set; }
        public string primary_contact_email { get; set; }
        public string primary_contact_phone { get; set; }
        public string secondary_contact_name { get; set; }
        public string secondary_contact_email { get; set; }
        public string secondary_contact_phone { get; set; }
        public string tax_registration_number { get; set; }
        public string bank_account_id { get; set; }
        public string account_number { get; set; }
        public string account_holder { get; set; }
        public string bank_id { get; set; }
        public string bank_name { get; set; }
        public string bank_code { get; set; }
        public string currency_id { get; set; }
        public string currency_code { get; set; }
        public string currency_symbol { get; set; }
        public string currency_name { get; set; }
        public string country_id { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string customer_type_id { get; set; }
        public string customer_type_name { get; set; }
        public bool? is_taxable { get; set; }
        public string additional_information { get; set; }
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public decimal? credit_limit { get; set; }
        public decimal? remained_credit_limit { get; set; }
        public string sync_id { get; set; }
        public string sync_type { get; set; }
        public string sync_status { get; set; }
        public string error_msg { get; set; }
    }
}

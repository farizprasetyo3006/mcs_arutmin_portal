using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class customer
    {
        public customer()
        {
            customer_attachment = new HashSet<customer_attachment>();
            despatch_order = new HashSet<despatch_order>();
            sales_contractcustomer_ = new HashSet<sales_contract>();
            sales_contractend_user_ = new HashSet<sales_contract>();
            sales_contractinvoice_target_ = new HashSet<sales_contract>();
            sales_plan_customer = new HashSet<sales_plan_customer>();
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
        public string tax_registration_number { get; set; }
        public string bank_account_id { get; set; }
        public string currency_id { get; set; }
        public string customer_type_id { get; set; }
        public string country_id { get; set; }
        public bool? is_taxable { get; set; }
        public string additional_information { get; set; }
        public string secondary_contact_email { get; set; }
        public string secondary_contact_name { get; set; }
        public string secondary_contact_phone { get; set; }
        public decimal? credit_limit { get; set; }
        public decimal? remained_credit_limit { get; set; }
        public bool? credit_limit_activation { get; set; }
        public string reporting_type_id { get; set; }
        public string customer_category_id { get; set; }
        public string customer_status_id { get; set; }
        public string industry_type_id { get; set; }
        public string alias_name { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<customer_attachment> customer_attachment { get; set; }
        public virtual ICollection<despatch_order> despatch_order { get; set; }
        public virtual ICollection<sales_contract> sales_contractcustomer_ { get; set; }
        public virtual ICollection<sales_contract> sales_contractend_user_ { get; set; }
        public virtual ICollection<sales_contract> sales_contractinvoice_target_ { get; set; }
        public virtual ICollection<sales_plan_customer> sales_plan_customer { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;


namespace DataAccess.DTO
{
    public partial class CustomerDTO
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
        public string country_id { get; set; }
        public string customer_type_id { get; set; }
        public bool is_taxable { get; set; }
        public List<IFormFile> customer_Attachments { get; set; }

    }
}

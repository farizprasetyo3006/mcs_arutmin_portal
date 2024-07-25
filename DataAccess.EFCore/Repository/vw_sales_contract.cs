using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_contract
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
        public string sales_contract_name { get; set; }
        public string document_reference { get; set; }
        public string customer_id { get; set; }
        public string buyer_agent { get; set; }
        public string customer_name { get; set; }
        public string end_user_id { get; set; }
        public string end_user_name { get; set; }
        public string seller_id { get; set; }
        public string seller_agent { get; set; }
        public string seller_name { get; set; }
        public string contract_basis_id { get; set; }
        public string contract_basis_name { get; set; }
        public string commitment_id { get; set; }
        public string commitment_name { get; set; }
        public string contract_status_id { get; set; }
        public string contract_status_name { get; set; }
        public string invoice_target_id { get; set; }
        public string invoice_target_name { get; set; }
        public string description { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string notes { get; set; }
        public string surveyor_id { get; set; }
        public string surveyor_name { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public bool? credit_limit_activation { get; set; }
        public decimal? credit_limit { get; set; }
    }
}

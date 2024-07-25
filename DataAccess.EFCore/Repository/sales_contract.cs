using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_contract
    {
        public sales_contract()
        {
            sales_contract_attachment = new HashSet<sales_contract_attachment>();
            sales_contract_detail = new HashSet<sales_contract_detail>();
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
        public string sales_contract_name { get; set; }
        public string document_reference { get; set; }
        public string customer_id { get; set; }
        public string end_user_id { get; set; }
        public string seller_id { get; set; }
        public string contract_basis_id { get; set; }
        public string commitment_id { get; set; }
        public string contract_status_id { get; set; }
        public string invoice_target_id { get; set; }
        public string description { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string notes { get; set; }
        public bool? credit_limit_activation { get; set; }
        public string buyer_agent_id { get; set; }
        public string seller_agent_id { get; set; }
        public string surveyor_id { get; set; }

        public virtual master_list commitment_ { get; set; }
        public virtual master_list contract_basis_ { get; set; }
        public virtual master_list contract_status_ { get; set; }
        public virtual customer customer_ { get; set; }
        public virtual customer end_user_ { get; set; }
        public virtual customer invoice_target_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual organization seller_ { get; set; }
        public virtual ICollection<sales_contract_attachment> sales_contract_attachment { get; set; }
        public virtual ICollection<sales_contract_detail> sales_contract_detail { get; set; }
        public virtual ICollection<sales_contract_term> sales_contract_term { get; set; }
    }
}

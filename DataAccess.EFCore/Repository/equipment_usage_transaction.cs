using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class equipment_usage_transaction
    {
        public equipment_usage_transaction()
        {
            equipment_usage_transaction_detail = new HashSet<equipment_usage_transaction_detail>();
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
        public string equipment_usage_number { get; set; }
        public string accounting_period_id { get; set; }
        public string advance_contract_reference_id { get; set; }
        public DateTime? start_datetime { get; set; }
        public DateTime? end_datetime { get; set; }
        public string note { get; set; }

        public virtual accounting_period accounting_period_ { get; set; }
        public virtual advance_contract_reference advance_contract_reference_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<equipment_usage_transaction_detail> equipment_usage_transaction_detail { get; set; }
    }
}

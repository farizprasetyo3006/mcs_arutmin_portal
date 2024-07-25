using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class advance_contract_item_detail
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
        public string advance_contract_item_id { get; set; }
        public decimal amount { get; set; }
        public string currency_id { get; set; }
        public string variable { get; set; }

        public virtual advance_contract_item advance_contract_item_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

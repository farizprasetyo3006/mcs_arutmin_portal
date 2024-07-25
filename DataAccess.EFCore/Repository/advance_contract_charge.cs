using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class advance_contract_charge
    {
        public advance_contract_charge()
        {
            advance_contract_charge_detail = new HashSet<advance_contract_charge_detail>();
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
        public string advance_contract_id { get; set; }
        public string charge_name { get; set; }
        public string advance_contract_detail_id { get; set; }
        public string price_index_id { get; set; }
        public string formula { get; set; }
        public string variable { get; set; }
        public string equipment_usage_transaction_id { get; set; }
        public decimal? decimal_places { get; set; }
        public string rounding_type_id { get; set; }
        public bool? is_base_formula { get; set; }
        public string advance_contract_item_id { get; set; }

        public virtual advance_contract advance_contract_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<advance_contract_charge_detail> advance_contract_charge_detail { get; set; }
    }
}

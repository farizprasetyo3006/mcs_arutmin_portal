using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class advance_contract
    {
        public advance_contract()
        {
            advance_contract_charge = new HashSet<advance_contract_charge>();
            advance_contract_detail = new HashSet<advance_contract_detail>();
            advance_contract_item = new HashSet<advance_contract_item>();
            progress_claim = new HashSet<progress_claim>();
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
        public string advance_contract_number { get; set; }
        public string contractor_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public decimal? quantity { get; set; }
        public string quantity_uom_id { get; set; }
        public string reference_number { get; set; }
        public string note { get; set; }
        public string accounting_period_id { get; set; }
        public decimal? contract_value { get; set; }
        public string contract_currency_id { get; set; }
        public string contract_type { get; set; }
        public int? version { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<advance_contract_charge> advance_contract_charge { get; set; }
        public virtual ICollection<advance_contract_detail> advance_contract_detail { get; set; }
        public virtual ICollection<advance_contract_item> advance_contract_item { get; set; }
        public virtual ICollection<progress_claim> progress_claim { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class advance_contract_valuation
    {
        public advance_contract_valuation()
        {
            advance_contract_valuation_detail = new HashSet<advance_contract_valuation_detail>();
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
        public string approved_by { get; set; }
        public DateTime? approved_on { get; set; }
        public string advance_contract_reference_id { get; set; }
        public string advance_contract_valuation_number { get; set; }
        public decimal? total_value { get; set; }
        public string notes { get; set; }
        public DateTime? valuation_date { get; set; }
        public string employee_id { get; set; }

        public virtual advance_contract_reference advance_contract_reference_ { get; set; }
        public virtual employee employee_ { get; set; }
        public virtual ICollection<advance_contract_valuation_detail> advance_contract_valuation_detail { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class advance_contract_valuation_detail
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
        public string approved_by { get; set; }
        public DateTime? approved_on { get; set; }
        public string advance_contract_valuation_id { get; set; }
        public string advance_contract_reference_detail_id { get; set; }
        public decimal? value { get; set; }
        public decimal? convertion_amount { get; set; }
        public string convertion_currency_id { get; set; }
        public string formula_trace { get; set; }

        public virtual advance_contract_reference_detail advance_contract_reference_detail_ { get; set; }
        public virtual advance_contract_valuation advance_contract_valuation_ { get; set; }
    }
}

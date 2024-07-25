using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_contract_taxes
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
        public string sales_contract_term_id { get; set; }
        public string tax_id { get; set; }
        public decimal? tax_rate { get; set; }
        public string tax_name { get; set; }
        public decimal? calculation_sign { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual sales_contract_term sales_contract_term_ { get; set; }
        public virtual tax tax_ { get; set; }
    }
}

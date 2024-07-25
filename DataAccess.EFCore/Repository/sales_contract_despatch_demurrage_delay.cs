using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_contract_despatch_demurrage_delay
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
        public string sales_contract_despatch_demurrage_id { get; set; }
        public string incident_id { get; set; }
        public decimal? demurrage_applicable_percentage { get; set; }
        public decimal? despatch_applicable_percentage { get; set; }

        public virtual event_category incident_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual sales_contract_despatch_demurrage_term sales_contract_despatch_demurrage_ { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class advance_contract_term
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
        public string advance_contract_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string term_name { get; set; }
        public string accounting_period_id { get; set; }

        public virtual advance_contract advance_contract_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

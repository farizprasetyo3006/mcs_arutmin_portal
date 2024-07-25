using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class standard_cost_mining
    {
        public standard_cost_mining()
        {
            standard_cost_mining_detail = new HashSet<standard_cost_mining_detail>();
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
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public bool? is_overburden { get; set; }
        public string mine_location_id { get; set; }
        public string business_partner_id { get; set; }
        public string standard_cost_name { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<standard_cost_mining_detail> standard_cost_mining_detail { get; set; }
    }
}

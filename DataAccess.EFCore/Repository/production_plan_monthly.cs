using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class production_plan_monthly
    {
        public production_plan_monthly()
        {
            production_plan_daily = new HashSet<production_plan_daily>();
            production_plan_monthly_history = new HashSet<production_plan_monthly_history>();
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
        public string production_plan_id { get; set; }
        public decimal? quantity { get; set; }
        public int? month_id { get; set; }

        public virtual months month_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual production_plan production_plan_ { get; set; }
        public virtual ICollection<production_plan_daily> production_plan_daily { get; set; }
        public virtual ICollection<production_plan_monthly_history> production_plan_monthly_history { get; set; }
    }
}

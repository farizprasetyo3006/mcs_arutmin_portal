using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class barging_plan_monthly
    {
        public barging_plan_monthly()
        {
            barging_plan_daily = new HashSet<barging_plan_daily>();
            barging_plan_monthly_history = new HashSet<barging_plan_monthly_history>();
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
        public string barging_plan_id { get; set; }
        public decimal? quantity { get; set; }
        public int? month_id { get; set; }

        public virtual barging_plan barging_plan_ { get; set; }
        public virtual months month_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<barging_plan_daily> barging_plan_daily { get; set; }
        public virtual ICollection<barging_plan_monthly_history> barging_plan_monthly_history { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class hauling_plan_monthly_history
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
        public string hauling_plan_monthly_id { get; set; }
        public string hauling_plan_id { get; set; }
        public decimal? quantity { get; set; }
        public int? month_id { get; set; }

        public virtual hauling_plan hauling_plan_ { get; set; }
        public virtual hauling_plan_monthly hauling_plan_monthly_ { get; set; }
        public virtual months month_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

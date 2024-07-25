using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_plan_detail
    {
        public sales_plan_detail()
        {
            sales_plan_customer = new HashSet<sales_plan_customer>();
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
        public string sales_plan_id { get; set; }
        public decimal? quantity { get; set; }
        public int? month_id { get; set; }

        public virtual months month_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual sales_plan sales_plan_ { get; set; }
        public virtual ICollection<sales_plan_customer> sales_plan_customer { get; set; }
    }
}

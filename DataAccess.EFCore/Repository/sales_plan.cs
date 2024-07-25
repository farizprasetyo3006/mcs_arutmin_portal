using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_plan
    {
        public sales_plan()
        {
            sales_plan_detail = new HashSet<sales_plan_detail>();
            sales_plan_snapshot = new HashSet<sales_plan_snapshot>();
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
        public decimal? quantity { get; set; }
        public string uom_id { get; set; }
        public string site_id { get; set; }
        public string plan_year_id { get; set; }
        public decimal? revision_number { get; set; }
        public bool? is_baseline { get; set; }
        public bool? is_lock { get; set; }
        public string notes { get; set; }
        public string sales_plan_number { get; set; }
        public decimal? rkab_quantity { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<sales_plan_detail> sales_plan_detail { get; set; }
        public virtual ICollection<sales_plan_snapshot> sales_plan_snapshot { get; set; }
    }
}

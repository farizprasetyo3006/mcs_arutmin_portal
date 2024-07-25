using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class production_plan
    {
        public production_plan()
        {
            production_plan_monthly = new HashSet<production_plan_monthly>();
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
        public string production_plan_number { get; set; }
        public string location_id { get; set; }
        public string pit_id { get; set; }
        public string mine_location_id { get; set; }
        public string plan_type { get; set; }
        public string product_id { get; set; }
        public string master_list_id { get; set; }
        public string contractor_id { get; set; }
        public string stock_location_id { get; set; }
        public string activity_plan { get; set; }
        public string product_category_id { get; set; }

        public virtual business_area location_ { get; set; }
        public virtual master_list master_list_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual business_area pit_ { get; set; }
        public virtual ICollection<production_plan_monthly> production_plan_monthly { get; set; }
        public virtual ICollection<production_plan_monthly_history> production_plan_monthly_history { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class mine_location
    {
        public mine_location()
        {
            barging_plan = new HashSet<barging_plan>();
            exposed_coal = new HashSet<exposed_coal>();
            hauling_plan = new HashSet<hauling_plan>();
            hauling_plan_history = new HashSet<hauling_plan_history>();
            mine_location_quality_pit = new HashSet<mine_location_quality_pit>();
            model_geology = new HashSet<model_geology>();
            ready_to_getNavigation = new HashSet<ready_to_get>();
            timesheet = new HashSet<timesheet>();
            timesheet_plan = new HashSet<timesheet_plan>();
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
        public string stock_location_name { get; set; }
        public float? latitude { get; set; }
        public float? longitude { get; set; }
        public string product_id { get; set; }
        public string uom_id { get; set; }
        public decimal? minimum_capacity { get; set; }
        public decimal? maximum_capacity { get; set; }
        public decimal? target_capacity { get; set; }
        public DateTime? opening_date { get; set; }
        public DateTime? closing_date { get; set; }
        public string business_area_id { get; set; }
        public string parent_stock_location_id { get; set; }
        public decimal? current_stock { get; set; }
        public decimal? proved_reserve { get; set; }
        public string mine_location_code { get; set; }
        public bool? ready_to_get { get; set; }
        public string contractor_id { get; set; }
        public string mine_plan_ltp_id { get; set; }

        public virtual business_area business_area_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<barging_plan> barging_plan { get; set; }
        public virtual ICollection<exposed_coal> exposed_coal { get; set; }
        public virtual ICollection<hauling_plan> hauling_plan { get; set; }
        public virtual ICollection<hauling_plan_history> hauling_plan_history { get; set; }
        public virtual ICollection<mine_location_quality_pit> mine_location_quality_pit { get; set; }
        public virtual ICollection<model_geology> model_geology { get; set; }
        public virtual ICollection<ready_to_get> ready_to_getNavigation { get; set; }
        public virtual ICollection<timesheet> timesheet { get; set; }
        public virtual ICollection<timesheet_plan> timesheet_plan { get; set; }
    }
}

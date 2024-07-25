using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class business_area
    {
        public business_area()
        {
            barging_planlocation_ = new HashSet<barging_plan>();
            barging_planpit_ = new HashSet<barging_plan>();
            cpp_location = new HashSet<cpp_location>();
            drill_blast_plan = new HashSet<drill_blast_plan>();
            eight_week_forecast_item = new HashSet<eight_week_forecast_item>();
            hauling_plan_historylocation_ = new HashSet<hauling_plan_history>();
            hauling_plan_historypit_ = new HashSet<hauling_plan_history>();
            hauling_planlocation_ = new HashSet<hauling_plan>();
            hauling_planpit_ = new HashSet<hauling_plan>();
            haze = new HashSet<haze>();
            mine_location = new HashSet<mine_location>();
            port_location = new HashSet<port_location>();
            process_flowdestination_location_ = new HashSet<process_flow>();
            process_flowsource_location_ = new HashSet<process_flow>();
            production_planlocation_ = new HashSet<production_plan>();
            production_planpit_ = new HashSet<production_plan>();
            rainfall = new HashSet<rainfall>();
            slippery = new HashSet<slippery>();
            stockpile_location = new HashSet<stockpile_location>();
            tidalwave = new HashSet<tidalwave>();
            waste_location = new HashSet<waste_location>();
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
        public string business_area_name { get; set; }
        public string parent_business_area_id { get; set; }
        public string business_area_code { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<barging_plan> barging_planlocation_ { get; set; }
        public virtual ICollection<barging_plan> barging_planpit_ { get; set; }
        public virtual ICollection<cpp_location> cpp_location { get; set; }
        public virtual ICollection<drill_blast_plan> drill_blast_plan { get; set; }
        public virtual ICollection<eight_week_forecast_item> eight_week_forecast_item { get; set; }
        public virtual ICollection<hauling_plan_history> hauling_plan_historylocation_ { get; set; }
        public virtual ICollection<hauling_plan_history> hauling_plan_historypit_ { get; set; }
        public virtual ICollection<hauling_plan> hauling_planlocation_ { get; set; }
        public virtual ICollection<hauling_plan> hauling_planpit_ { get; set; }
        public virtual ICollection<haze> haze { get; set; }
        public virtual ICollection<mine_location> mine_location { get; set; }
        public virtual ICollection<port_location> port_location { get; set; }
        public virtual ICollection<process_flow> process_flowdestination_location_ { get; set; }
        public virtual ICollection<process_flow> process_flowsource_location_ { get; set; }
        public virtual ICollection<production_plan> production_planlocation_ { get; set; }
        public virtual ICollection<production_plan> production_planpit_ { get; set; }
        public virtual ICollection<rainfall> rainfall { get; set; }
        public virtual ICollection<slippery> slippery { get; set; }
        public virtual ICollection<stockpile_location> stockpile_location { get; set; }
        public virtual ICollection<tidalwave> tidalwave { get; set; }
        public virtual ICollection<waste_location> waste_location { get; set; }
    }
}

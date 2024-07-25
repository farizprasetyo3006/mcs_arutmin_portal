using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class eight_week_forecast
    {
        public eight_week_forecast()
        {
            eight_week_forecast_item = new HashSet<eight_week_forecast_item>();
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
        public string planning_number { get; set; }
        public int? version { get; set; }
        public string year_id { get; set; }
        public decimal? total { get; set; }
        public string uom_id { get; set; }

        public virtual business_unit business_unit_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual uom uom_ { get; set; }
        public virtual master_list year_ { get; set; }
        public virtual ICollection<eight_week_forecast_item> eight_week_forecast_item { get; set; }
    }
}

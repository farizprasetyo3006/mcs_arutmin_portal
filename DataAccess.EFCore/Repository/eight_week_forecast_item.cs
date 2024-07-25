using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class eight_week_forecast_item
    {
        public eight_week_forecast_item()
        {
            eight_week_forecast_item_detail = new HashSet<eight_week_forecast_item_detail>();
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
        public string header_id { get; set; }
        public string activity_plan_id { get; set; }
        public string location_id { get; set; }
        public string business_area_pit_id { get; set; }
        public string product_category_id { get; set; }
        public string product_id { get; set; }
        public string contractor_id { get; set; }
        public decimal? total { get; set; }
        public string uom_id { get; set; }

        public virtual master_list activity_plan_ { get; set; }
        public virtual eight_week_forecast header_ { get; set; }
        public virtual business_area location_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<eight_week_forecast_item_detail> eight_week_forecast_item_detail { get; set; }
    }
}

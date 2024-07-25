using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_eight_week_forecast_item
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
        public string business_unit_name { get; set; }
        public string header_id { get; set; }
        public string activity_plan_id { get; set; }
        public string planning_number { get; set; }
        public string version { get; set; }
        public string year_id { get; set; }
        public string item_name { get; set; }
        public string location_id { get; set; }
        public string location_name { get; set; }
        public string business_area_pit_id { get; set; }
        public string pit_name { get; set; }
        public string product_category_id { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string contractor_id { get; set; }
        public decimal? total { get; set; }
        public string uom_id { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public decimal? total_quantity { get; set; }
        public decimal? sum { get; set; }
    }
}

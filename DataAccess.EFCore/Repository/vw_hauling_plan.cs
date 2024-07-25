using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_hauling_plan
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
        public string hauling_plan_number { get; set; }
        public string location_id { get; set; }
        public string location_business_area_name { get; set; }
        public string pit_id { get; set; }
        public string pit_business_area_name { get; set; }
        public string mine_location_id { get; set; }
        public string seam_name { get; set; }
        public string plan_type { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string master_list_id { get; set; }
        public string plan_year { get; set; }
        public decimal? total_quantity { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string activity_plan { get; set; }
    }
}

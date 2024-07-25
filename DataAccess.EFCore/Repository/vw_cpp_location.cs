using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_cpp_location
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
        public string business_area_id { get; set; }
        public string business_area_name { get; set; }
        public string parent_stock_location_id { get; set; }
        public string parent_stock_location_name { get; set; }
        public string stock_location_name { get; set; }
        public string cpp_location_code { get; set; }
        public float? latitude { get; set; }
        public float? longitude { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public decimal? minimum_capacity { get; set; }
        public decimal? target_capacity { get; set; }
        public decimal? maximum_capacity { get; set; }
        public decimal? current_stock { get; set; }
        public DateTime? opening_date { get; set; }
        public DateTime? closing_date { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string contractor_id { get; set; }
    }
}

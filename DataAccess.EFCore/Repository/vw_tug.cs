using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_tug
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
        public string vehicle_name { get; set; }
        public string vehicle_id { get; set; }
        public double? capacity { get; set; }
        public string capacity_uom_id { get; set; }
        public string capacity_uom_name { get; set; }
        public double? length { get; set; }
        public string length_uom_id { get; set; }
        public string length_uom_name { get; set; }
        public double? width { get; set; }
        public string width_uom_id { get; set; }
        public string width_uom_name { get; set; }
        public double? height { get; set; }
        public string height_uom_id { get; set; }
        public string height_uom_name { get; set; }
        public string vehicle_make { get; set; }
        public string vehicle_model { get; set; }
        public int? vehicle_model_year { get; set; }
        public int? vehicle_manufactured_year { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string vendor_id { get; set; }
        public string tug_flag { get; set; }
    }
}

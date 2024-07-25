using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_equipment
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
        public string equipment_type_id { get; set; }
        public string equipment_type_name { get; set; }
        public string equipment_type_code { get; set; }
        public string equipment_name { get; set; }
        public string equipment_code { get; set; }
        public string asset_number { get; set; }
        public decimal? capacity { get; set; }
        public string capacity_uom_id { get; set; }
        public string capacity_uom_name { get; set; }
        public string vendor_id { get; set; }
        public bool? status { get; set; }
        public string vehicle_model { get; set; }
        public string vehicle_model_year { get; set; }
        public decimal? plan { get; set; }
        public string vehicle_manufactured_year { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

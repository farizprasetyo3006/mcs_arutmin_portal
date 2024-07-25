using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_timesheet_plan
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
        public string cn_unit_id { get; set; }
        public string equipment_id { get; set; }
        public string truck_id { get; set; }
        public string egi_unit { get; set; }
        public string cn_name { get; set; }
        public string mine_location_id { get; set; }
        public string mine_location_code { get; set; }
        public string operator_id { get; set; }
        public string operator_name { get; set; }
        public string operator_nik { get; set; }
        public string operator_email { get; set; }
        public string operator_phone { get; set; }
        public decimal? hour_start { get; set; }
        public decimal? hour_end { get; set; }
        public decimal? quantity { get; set; }
        public string shift_id { get; set; }
        public string shift_name { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string uom_symbol { get; set; }
        public DateTime? timesheet_date { get; set; }
        public string timesheet_date_show { get; set; }
        public string material_id { get; set; }
        public string masterial_name { get; set; }
        public string material_code { get; set; }
        public string activity_id { get; set; }
        public string activity_name { get; set; }
        public string supervisor_id { get; set; }
        public string supervisor_name { get; set; }
        public string supervisor_nik { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string plan_type { get; set; }
        public string activity_plan { get; set; }
    }
}

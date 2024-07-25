using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_timesheet_detail_plan
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
        public string timesheet_id { get; set; }
        public string event_category_id { get; set; }
        public string event_definition_category_id { get; set; }
        public string event_definition_category_name { get; set; }
        public string event_category_name { get; set; }
        public string event_category_code { get; set; }
        public string mine_location_id { get; set; }
        public string mine_location_code { get; set; }
        public string destination_id { get; set; }
        public string destination_name { get; set; }
        public string stockpile_location_id { get; set; }
        public string waste_location_id { get; set; }
        public decimal? refuelling_quantity { get; set; }
        public decimal? quantity { get; set; }
        public decimal? distance { get; set; }
        public decimal? ritase { get; set; }
        public TimeSpan? timesheet_time { get; set; }
        public decimal? duration { get; set; }
        public string periode { get; set; }
        public string periode_detail { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string uom_symbol { get; set; }
        public string pit_id { get; set; }
        public string pit_name { get; set; }
        public string loader_id { get; set; }
        public string loader_name { get; set; }
        public string source_id { get; set; }
        public string source_name { get; set; }
        public string material_id { get; set; }
        public string material_name { get; set; }
        public decimal? volume { get; set; }
        public int? mohh { get; set; }
        public int? wh { get; set; }
        public int? idle { get; set; }
        public int? delay { get; set; }
        public int? breakdown { get; set; }
        public string accounting_periode_id { get; set; }
        public string accounting_period_name { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public TimeSpan? classification { get; set; }
    }
}

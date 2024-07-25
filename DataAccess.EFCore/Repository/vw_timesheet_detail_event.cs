using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_timesheet_detail_event
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
        public string timesheet_detail_id { get; set; }
        public TimeSpan? timesheet_detail_name { get; set; }
        public string event_category_id { get; set; }
        public string category { get; set; }
        public string event_definition_category_id { get; set; }
        public string activity { get; set; }
        public string activity_code { get; set; }
        public decimal? minute { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string category_code { get; set; }
    }
}

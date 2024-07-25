using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_equipment_incident
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
        public string equipment_id { get; set; }
        public string equipment_name { get; set; }
        public string equipment_code { get; set; }
        public string event_category_id { get; set; }
        public string event_category_name { get; set; }
        public DateTime? incident_start { get; set; }
        public DateTime? incident_end { get; set; }
        public decimal? hour_duration { get; set; }
        public string incident_description { get; set; }
        public string accounting_period_id { get; set; }
        public string accounting_period_name { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_drill_blast_plan
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
        public string plan_number { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string vendor_id { get; set; }
        public string vendor { get; set; }
        public decimal? request_level { get; set; }
        public decimal? quantity { get; set; }
        public string pit_id { get; set; }
        public string business_area_name { get; set; }
        public string business_area_code { get; set; }
        public decimal? blast_volume { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string uom_symbol { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

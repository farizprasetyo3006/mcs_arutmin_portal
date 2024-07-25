using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_blending_plan_source
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
        public string blending_plan_id { get; set; }
        public string transaction_number { get; set; }
        public string accounting_period_id { get; set; }
        public string process_flow_id { get; set; }
        public string survey_id { get; set; }
        public string survey_number { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string uom_symbol { get; set; }
        public string source_location_id { get; set; }
        public string source_location_name { get; set; }
        public DateTime? loading_datetime { get; set; }
        public decimal? loading_quantity { get; set; }
        public string transport_id { get; set; }
        public int? trip_count { get; set; }
        public string equipment_id { get; set; }
        public decimal? hour_usage { get; set; }
        public decimal? spec_ts { get; set; }
        public decimal? volume { get; set; }
        public decimal? analyte_1 { get; set; }
        public decimal? analyte_2 { get; set; }
        public decimal? analyte_3 { get; set; }
        public decimal? analyte_4 { get; set; }
        public decimal? analyte_5 { get; set; }
        public decimal? analyte_6 { get; set; }
        public decimal? analyte_7 { get; set; }
        public decimal? analyte_8 { get; set; }
        public decimal? analyte_9 { get; set; }
        public decimal? analyte_10 { get; set; }
        public decimal? analyte_11 { get; set; }
        public string note { get; set; }
        public string sampling_number { get; set; }
        public string ikh_pit_id { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

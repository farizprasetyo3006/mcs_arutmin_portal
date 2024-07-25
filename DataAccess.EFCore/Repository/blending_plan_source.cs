using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class blending_plan_source
    {
        public blending_plan_source()
        {
            blending_plan_value = new HashSet<blending_plan_value>();
        }

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
        public string business_unit_id { get; set; }
        public string blending_plan_id { get; set; }
        public string accounting_period_id { get; set; }
        public string process_flow_id { get; set; }
        public string survey_id { get; set; }
        public string source_shift_id { get; set; }
        public string source_location_id { get; set; }
        public DateTime? loading_datetime { get; set; }
        public decimal? loading_quantity { get; set; }
        public string product_id { get; set; }
        public string uom_id { get; set; }
        public string transport_id { get; set; }
        public int? trip_count { get; set; }
        public string equipment_id { get; set; }
        public decimal? hour_usage { get; set; }
        public string despatch_order_id { get; set; }
        public string quality_sampling_id { get; set; }
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
        public string note { get; set; }
        public string sampling_number { get; set; }
        public string ikh_pit_id { get; set; }
        public decimal? analyte_11 { get; set; }

        public virtual blending_plan blending_plan_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<blending_plan_value> blending_plan_value { get; set; }
    }
}

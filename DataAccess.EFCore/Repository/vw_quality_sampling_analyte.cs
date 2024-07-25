using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_quality_sampling_analyte
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
        public string quality_sampling_id { get; set; }
        public string sampling_number { get; set; }
        public DateTime? sampling_datetime { get; set; }
        public string despatch_order_id { get; set; }
        public bool? non_commercial { get; set; }
        public string analyte_id { get; set; }
        public string analyte_name { get; set; }
        public string analyte_symbol { get; set; }
        public decimal? analyte_value { get; set; }
        public decimal? value { get; set; }
        public decimal? target { get; set; }
        public decimal? minimum { get; set; }
        public decimal? maximum { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string alias { get; set; }
        public string alias_formula { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public bool? coa_display { get; set; }
        public bool? trace_elements { get; set; }
        public int? order { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

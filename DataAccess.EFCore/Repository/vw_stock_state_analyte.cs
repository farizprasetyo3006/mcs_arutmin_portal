using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_stock_state_analyte
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
        public string stock_state_id { get; set; }
        public string stock_location_id { get; set; }
        public string stock_location_name { get; set; }
        public string product_in_id { get; set; }
        public string product_in_name { get; set; }
        public string product_out_id { get; set; }
        public string product_out_name { get; set; }
        public string quality_sampling_id { get; set; }
        public string sampling_number { get; set; }
        public DateTime? sampling_datetime { get; set; }
        public string analyte_id { get; set; }
        public string analyte_name { get; set; }
        public string analyte_symbol { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string uom_symbol { get; set; }
        public decimal? analyte_value { get; set; }
        public decimal? weighted_value { get; set; }
        public decimal? moving_avg_value { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

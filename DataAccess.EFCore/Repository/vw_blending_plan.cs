using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_blending_plan
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
        public string transaction_number { get; set; }
        public string reference_number { get; set; }
        public string accounting_period_id { get; set; }
        public string process_flow_id { get; set; }
        public string survey_id { get; set; }
        public string survey_number { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string uom_symbol { get; set; }
        public string destination_location_id { get; set; }
        public string stock_location_name { get; set; }
        public DateTime? unloading_datetime { get; set; }
        public decimal? unloading_quantity { get; set; }
        public string transport_id { get; set; }
        public int? trip_count { get; set; }
        public string equipment_id { get; set; }
        public decimal? hour_usage { get; set; }
        public string despatch_order_id { get; set; }
        public string planning_category { get; set; }
        public string source_shift_id { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string source_shift_name { get; set; }
        public decimal? sumprod_volume_product { get; set; }
        public decimal? sumprod_analyte_product_1 { get; set; }
        public decimal? sumprod_analyte_product_2 { get; set; }
        public decimal? sumprod_analyte_product_3 { get; set; }
        public decimal? sumprod_analyte_product_4 { get; set; }
        public decimal? sumprod_analyte_product_5 { get; set; }
        public decimal? sumprod_analyte_product_6 { get; set; }
        public decimal? sumprod_analyte_product_7 { get; set; }
        public decimal? sumprod_analyte_product_8 { get; set; }
        public decimal? sumprod_analyte_product_9 { get; set; }
        public decimal? sumprod_analyte_product_10 { get; set; }
        public decimal? sumprod_analyte_product_11 { get; set; }
        public decimal? sumprod_volume { get; set; }
        public decimal? sumprod_analyte_1 { get; set; }
        public decimal? sumprod_analyte_2 { get; set; }
        public decimal? sumprod_analyte_3 { get; set; }
        public decimal? sumprod_analyte_4 { get; set; }
        public decimal? sumprod_analyte_5 { get; set; }
        public decimal? sumprod_analyte_6 { get; set; }
        public decimal? sumprod_analyte_7 { get; set; }
        public decimal? sumprod_analyte_8 { get; set; }
        public decimal? sumprod_analyte_9 { get; set; }
        public decimal? sumprod_analyte_10 { get; set; }
        public decimal? sumprod_analyte_11 { get; set; }
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string sales_plan_id { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_blending_plan_value
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
        public string blending_plan_source_id { get; set; }
        public string blending_plan_id { get; set; }
        public string transaction_number { get; set; }
        public string source_location_id { get; set; }
        public string source_location_name { get; set; }
        public decimal? quantity { get; set; }
        public string uom_quantity_id { get; set; }
        public string uom_quantity_name { get; set; }
        public string uom_quantity_symbol { get; set; }
        public string analyte_id { get; set; }
        public string analyte_name { get; set; }
        public string analyte_symbol { get; set; }
        public decimal? analyte_value { get; set; }
        public string uom_analyte_id { get; set; }
        public string uom_analyte_name { get; set; }
        public string uom_analyte_symbol { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_process_flow
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
        public string process_flow_name { get; set; }
        public string process_flow_code { get; set; }
        public string process_flow_category { get; set; }
        public string source_location_id { get; set; }
        public string source_location_name { get; set; }
        public string destination_location_id { get; set; }
        public string destination_location_name { get; set; }
        public string product_input_id { get; set; }
        public string product_input_name { get; set; }
        public string uom_input_id { get; set; }
        public string uom_input_name { get; set; }
        public string product_output_id { get; set; }
        public string product_output_name { get; set; }
        public string uom_output_id { get; set; }
        public string uom_output_name { get; set; }
        public string sampling_template_id { get; set; }
        public string sampling_template_name { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

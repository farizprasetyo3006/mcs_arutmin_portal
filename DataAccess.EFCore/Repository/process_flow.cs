using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class process_flow
    {
        public process_flow()
        {
            barging_transaction = new HashSet<barging_transaction>();
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
        public string process_flow_name { get; set; }
        public string source_location_id { get; set; }
        public string destination_location_id { get; set; }
        public string product_input_id { get; set; }
        public string product_output_id { get; set; }
        public bool? assume_source_quality { get; set; }
        public string uom_input_id { get; set; }
        public string uom_output_id { get; set; }
        public string process_flow_category { get; set; }
        public string sampling_template_id { get; set; }
        public string process_flow_code { get; set; }

        public virtual business_area destination_location_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual business_area source_location_ { get; set; }
        public virtual ICollection<barging_transaction> barging_transaction { get; set; }
    }
}

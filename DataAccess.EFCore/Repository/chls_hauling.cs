using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class chls_hauling
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
        public string business_unit_id { get; set; }
        public string formula { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public decimal? duration { get; set; }
        public string event_definition_category { get; set; }
        public string process_flow_id { get; set; }
        public string equipment_id { get; set; }
        public string source_location_id { get; set; }
        public string destination_location_id { get; set; }
        public decimal? quantity { get; set; }
        public string uom { get; set; }
        public string header_id { get; set; }
        public bool? approved { get; set; }
        public string approved_by { get; set; }
        public bool? second_approved { get; set; }
        public decimal? nett_loading_rate { get; set; }
        public string description { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

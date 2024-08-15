using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_coal_transfer
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
        public bool? accounting_period_is_closed { get; set; }
        public string process_flow_id { get; set; }
        public string survey_id { get; set; }
        public string survey_number { get; set; }
        public DateTime? survey_date { get; set; }
        public string source_shift_id { get; set; }
        public string source_location_id { get; set; }
        public string source_location_name { get; set; }
        public DateTime? loading_datetime { get; set; }
        public decimal? loading_quantity { get; set; }
        public string product_id { get; set; }
        public string uom_id { get; set; }
        public string destination_shift_id { get; set; }
        public string destination_location_id { get; set; }
        public string destination_location_name { get; set; }
        public DateTime? unloading_datetime { get; set; }
        public decimal? unloading_quantity { get; set; }
        public string transport_id { get; set; }
        public string vehicle_name { get; set; }
        public int? trip_count { get; set; }
        public string equipment_id { get; set; }
        public string equipment_name { get; set; }
        public decimal? hour_usage { get; set; }
        public string note { get; set; }
        public string progress_claim_id { get; set; }
        public string progress_claim_name { get; set; }
        public string advance_contract_id { get; set; }
        public string advance_contract_number { get; set; }
        public string quality_sampling_id { get; set; }
        public string sampling_number { get; set; }
        public DateTime? sampling_datetime { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public decimal? tare { get; set; }
        public decimal? gross { get; set; }
        public decimal? distance { get; set; }
        public string pic { get; set; }
        public string contractor_id { get; set; }
        public string contractor_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string integration_status { get; set; }
    }
}

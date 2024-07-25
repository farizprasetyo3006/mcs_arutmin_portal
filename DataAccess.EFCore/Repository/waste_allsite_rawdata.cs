using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class waste_allsite_rawdata
    {
        public string id { get; set; }
        public string transaction_number { get; set; }
        public string reference_number { get; set; }
        public string accounting_period_name { get; set; }
        public bool? accounting_period_is_closed { get; set; }
        public string process_flow_name { get; set; }
        public string source_shift_name { get; set; }
        public string sub_mine { get; set; }
        public string pit { get; set; }
        public string sub_pit { get; set; }
        public string contractor_name { get; set; }
        public string pit_contractor { get; set; }
        public string source_location_name { get; set; }
        public DateTime? loading_datetime { get; set; }
        public decimal? loading_quantity { get; set; }
        public string waste_name { get; set; }
        public string uom_name { get; set; }
        public string destination_location_name { get; set; }
        public DateTime? unloading_datetime { get; set; }
        public decimal? unloading_quantity { get; set; }
        public string vehicle_name { get; set; }
        public int? trip_count { get; set; }
        public decimal? hour_usage { get; set; }
        public decimal? distance { get; set; }
        public decimal? elevation { get; set; }
        public string note { get; set; }
        public string progress_claim_name { get; set; }
        public string advance_contract_number { get; set; }
        public string survey_number { get; set; }
        public decimal? capacity { get; set; }
        public decimal? tare { get; set; }
        public string sampling_number { get; set; }
        public string organization_name { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string shift_category_code { get; set; }
        public DateTime? created_on { get; set; }
        public DateTime? modified_on { get; set; }
        public bool? is_active { get; set; }
        public bool? is_locked { get; set; }
        public bool? is_default { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class coal_barging_rawdata
    {
        public string id { get; set; }
        public string transaction_number { get; set; }
        public string voyage_number { get; set; }
        public string mine_origin { get; set; }
        public string barging_destination { get; set; }
        public string reference_number { get; set; }
        public bool? is_loading { get; set; }
        public string accounting_period_name { get; set; }
        public bool? accounting_period_is_closed { get; set; }
        public string process_flow_name { get; set; }
        public string survey_number { get; set; }
        public DateTime? survey_date { get; set; }
        public string source_shift_name { get; set; }
        public string source_location_name { get; set; }
        public DateTime? arrival_datetime { get; set; }
        public DateTime? berth_datetime { get; set; }
        public DateTime? start_datetime { get; set; }
        public DateTime? end_datetime { get; set; }
        public DateTime? unberth_datetime { get; set; }
        public DateTime? departure_datetime { get; set; }
        public decimal? draftsurvey_qty { get; set; }
        public decimal? beltscale_quantity { get; set; }
        public string uom_name { get; set; }
        public string uom_symbol { get; set; }
        public string destination_shift_name { get; set; }
        public string destination_location_name { get; set; }
        public string vehicle_name { get; set; }
        public int? trip_count { get; set; }
        public decimal? return_cargo { get; set; }
        public string equipment_name { get; set; }
        public decimal? hour_usage { get; set; }
        public string note { get; set; }
        public string ref_work_order { get; set; }
        public string delivery_term_name { get; set; }
        public string tug_name { get; set; }
        public string owner_name { get; set; }
        public string tug_name1 { get; set; }
        public string owner_name1 { get; set; }
        public DateTime? draft_survey_date { get; set; }
        public string draft_survey_number { get; set; }
        public string brand { get; set; }
        public string product_category_name { get; set; }
        public string split_cargo { get; set; }
        public string contractor_name { get; set; }
        public decimal? split_qty { get; set; }
        public string organization_name { get; set; }
        public string business_unit_name { get; set; }
        public string si_number { get; set; }
        public DateTime? si_date { get; set; }
        public decimal? distance { get; set; }
        public DateTime? initial_draft_survey { get; set; }
        public DateTime? final_draft_survey { get; set; }
        public string transport_name { get; set; }
        public decimal? intermediate_quantity { get; set; }
        public DateTime? intermediate_time { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public bool? is_finish { get; set; }
        public DateTime? created_on { get; set; }
        public DateTime? modified_on { get; set; }
        public bool? is_active { get; set; }
        public bool? is_locked { get; set; }
        public bool? is_default { get; set; }
    }
}

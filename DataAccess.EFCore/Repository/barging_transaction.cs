using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class barging_transaction
    {
        public barging_transaction()
        {
            barging_load_unload_document = new HashSet<barging_load_unload_document>();
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
        public string transaction_number { get; set; }
        public string reference_number { get; set; }
        public string accounting_period_id { get; set; }
        public string process_flow_id { get; set; }
        public string survey_id { get; set; }
        public bool is_loading { get; set; }
        public string source_shift_id { get; set; }
        public string source_location_id { get; set; }
        public string product_id { get; set; }
        public DateTime start_datetime { get; set; }
        public DateTime? end_datetime { get; set; }
        public decimal quantity { get; set; }
        public string uom_id { get; set; }
        public string destination_shift_id { get; set; }
        public string destination_location_id { get; set; }
        public string transport_id { get; set; }
        public int? trip_count { get; set; }
        public string equipment_id { get; set; }
        public decimal? hour_usage { get; set; }
        public string despatch_order_id { get; set; }
        public string note { get; set; }
        public string quality_sampling_id { get; set; }
        public DateTime? arrival_datetime { get; set; }
        public DateTime? berth_datetime { get; set; }
        public DateTime? unberth_datetime { get; set; }
        public DateTime? departure_datetime { get; set; }
        public string draft_survey_id { get; set; }
        public DateTime? initial_draft_survey { get; set; }
        public DateTime? final_draft_survey { get; set; }
        public decimal? return_cargo { get; set; }
        public string customer_id { get; set; }
        public string sales_contract_id { get; set; }
        public string si_number { get; set; }
        public DateTime? si_date { get; set; }
        public decimal? distance { get; set; }
        public string tug_id { get; set; }
        public string ref_work_order { get; set; }
        public decimal? intermediate_quantity { get; set; }
        public DateTime? intermediate_time { get; set; }
        public string voyage_number { get; set; }
        public string sils_id { get; set; }
        public decimal? beltscale_quantity { get; set; }
        public bool? is_final { get; set; }
        public bool? is_finish { get; set; }
        public string product_category_id { get; set; }
        public string contractor_id { get; set; }
        public string integration_status { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<barging_load_unload_document> barging_load_unload_document { get; set; }
    }
}

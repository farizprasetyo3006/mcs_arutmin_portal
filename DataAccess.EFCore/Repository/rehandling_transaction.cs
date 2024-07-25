using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rehandling_transaction
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
        public string transaction_number { get; set; }
        public string reference_number { get; set; }
        public string accounting_period_id { get; set; }
        public string process_flow_id { get; set; }
        public string survey_id { get; set; }
        public string source_shift_id { get; set; }
        public string source_location_id { get; set; }
        public DateTime loading_datetime { get; set; }
        public decimal loading_quantity { get; set; }
        public string product_id { get; set; }
        public string uom_id { get; set; }
        public string destination_shift_id { get; set; }
        public string destination_location_id { get; set; }
        public DateTime? unloading_datetime { get; set; }
        public decimal? unloading_quantity { get; set; }
        public string transport_id { get; set; }
        public int? trip_count { get; set; }
        public string equipment_id { get; set; }
        public decimal? hour_usage { get; set; }
        public string despatch_order_id { get; set; }
        public string note { get; set; }
        public string progress_claim_id { get; set; }
        public string quality_sampling_id { get; set; }
        public string pic { get; set; }
        public decimal? distance { get; set; }
        public string advance_contract_id1 { get; set; }
        public string chls_id { get; set; }
        public string contractor_id { get; set; }

        public virtual accounting_period accounting_period_ { get; set; }
        public virtual shift destination_shift_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual advance_contract progress_claim_ { get; set; }
        public virtual quality_sampling survey_ { get; set; }
    }
}

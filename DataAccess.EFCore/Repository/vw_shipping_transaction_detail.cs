using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_shipping_transaction_detail
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
        public string shipping_transaction_id { get; set; }
        public string shipping_transaction_number { get; set; }
        public bool? is_loading { get; set; }
        public string accounting_period_id { get; set; }
        public string accounting_period_name { get; set; }
        public bool? accounting_period_is_closed { get; set; }
        public string transaction_number { get; set; }
        public string reference_number { get; set; }
        public string survey_id { get; set; }
        public string survey_number { get; set; }
        public string detail_shift_id { get; set; }
        public string detail_shift_name { get; set; }
        public string detail_location_id { get; set; }
        public string detail_location_name { get; set; }
        public DateTime? start_datetime { get; set; }
        public DateTime? end_datetime { get; set; }
        public decimal? quantity { get; set; }
        public decimal? final_quantity { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string uom_symbol { get; set; }
        public string ship_shift_id { get; set; }
        public string ship_shift_name { get; set; }
        public string transport_id { get; set; }
        public string vehicle_name { get; set; }
        public string equipment_id { get; set; }
        public string equipment_name { get; set; }
        public decimal? hour_usage { get; set; }
        public string note { get; set; }
        public string barging_transaction_id { get; set; }
        public string barging_transaction_number { get; set; }
        public string reason_id { get; set; }
        public DateTime? arrival_datetime { get; set; }
        public DateTime? departure_datetime { get; set; }
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class shipping_transaction_detail
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
        public string shipping_transaction_id { get; set; }
        public string transaction_number { get; set; }
        public string reference_number { get; set; }
        public string detail_location_id { get; set; }
        public string detail_shift_id { get; set; }
        public DateTime? start_datetime { get; set; }
        public DateTime? end_datetime { get; set; }
        public string ship_shift_id { get; set; }
        public string survey_id { get; set; }
        public decimal quantity { get; set; }
        public decimal? final_quantity { get; set; }
        public string uom_id { get; set; }
        public string transport_id { get; set; }
        public string equipment_id { get; set; }
        public decimal? hour_usage { get; set; }
        public string note { get; set; }
        public string barging_transaction_id { get; set; }
        public string reason_id { get; set; }
        public DateTime? arrival_datetime { get; set; }
        public DateTime? departure_datetime { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual shipping_transaction shipping_transaction_ { get; set; }
    }
}

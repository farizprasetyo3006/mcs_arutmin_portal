using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class shipment_plan_item
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
        public string shipment_plan_id { get; set; }
        public string barge_id { get; set; }
        public string tug_id { get; set; }
        public decimal? quantity { get; set; }
        public string load_at { get; set; }
        public DateTime? laycan_start_date { get; set; }
        public DateTime? laycan_end_date { get; set; }
        public DateTime? eta_isp { get; set; }
        public DateTime? commence_loading { get; set; }
        public DateTime? complete_loading { get; set; }
        public DateTime? cash_off_isp { get; set; }
        public DateTime? eta_anchorage { get; set; }
        public DateTime? commence_discharge { get; set; }
        public DateTime? complete_discharge { get; set; }
        public DateTime? bl_date { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class eight_week_forecast_item_detail
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
        public string header_id { get; set; }
        public string item_id { get; set; }
        public string week_id { get; set; }
        public DateTime? from_date { get; set; }
        public DateTime? to_date { get; set; }
        public decimal? quantity { get; set; }
        public decimal? ash_adb { get; set; }
        public decimal? ts_adb { get; set; }
        public decimal? im_adb { get; set; }
        public decimal? tm_arb { get; set; }
        public decimal? gcv_gad { get; set; }
        public decimal? gcv_gar { get; set; }
        public bool? is_using { get; set; }
        public string product_id { get; set; }

        public virtual eight_week_forecast_item item_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

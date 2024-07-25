using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_demurrage_rate
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
        public string sales_order_id { get; set; }
        public string stock_location_id { get; set; }
        public string demurrage_name { get; set; }
        public decimal? loading_rate { get; set; }
        public decimal? turn_time { get; set; }
        public decimal? demurrage_rate { get; set; }
        public decimal? despatch_percent { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual sales_order sales_order_ { get; set; }
    }
}

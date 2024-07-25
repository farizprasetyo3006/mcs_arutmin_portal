using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class reference_price_series
    {
        public reference_price_series()
        {
            benchmark_price_series = new HashSet<benchmark_price_series>();
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
        public string price_name { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string notes { get; set; }
        public decimal? price { get; set; }
        public string currency_id { get; set; }
        public string currency_uom_id { get; set; }
        public decimal? calori { get; set; }
        public string calori_uom_id { get; set; }
        public decimal? total_moisture { get; set; }
        public string total_moisture_uom_id { get; set; }
        public decimal? total_sulphur { get; set; }
        public string total_sulphur_uom_id { get; set; }
        public decimal? ash { get; set; }
        public string ash_uom_id { get; set; }

        public virtual ICollection<benchmark_price_series> benchmark_price_series { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class benchmark_price_series
    {
        public benchmark_price_series()
        {
            benchmark_price_series_detail = new HashSet<benchmark_price_series_detail>();
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
        public string reference_price_id { get; set; }

        public virtual reference_price_series reference_price_ { get; set; }
        public virtual ICollection<benchmark_price_series_detail> benchmark_price_series_detail { get; set; }
    }
}

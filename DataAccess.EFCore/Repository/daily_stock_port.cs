using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class daily_stock_port
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
        public DateTime? cargo_date { get; set; }
        public double? beginning { get; set; }
        public double? hauling { get; set; }
        public double? processing { get; set; }
        public double? return_cargo { get; set; }
        public double? loading_to_barge { get; set; }
        public double? adjustment { get; set; }
        public double? ending { get; set; }
        public DateTime? hauling_from_date { get; set; }
        public DateTime? hauling_to_date { get; set; }
        public DateTime? barging_from_date { get; set; }
        public DateTime? barging_to_date { get; set; }
    }
}

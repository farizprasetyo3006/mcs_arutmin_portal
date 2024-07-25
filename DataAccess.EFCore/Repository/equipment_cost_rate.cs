using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class equipment_cost_rate
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
        public string accounting_period_id { get; set; }
        public decimal? hourly_rate { get; set; }
        public decimal? trip_rate { get; set; }
        public string currency_id { get; set; }
        public string equipment_id { get; set; }
        public decimal? fuel_per_hour { get; set; }
        public decimal? monthly_rate { get; set; }
        public string code { get; set; }
        public string name { get; set; }

        public virtual equipment equipment_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

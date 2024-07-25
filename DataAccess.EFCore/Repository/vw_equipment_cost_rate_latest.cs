using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_equipment_cost_rate_latest
    {
        public string organization_id { get; set; }
        public string equipment_id { get; set; }
        public string equipment_name { get; set; }
        public decimal? monthly_rate { get; set; }
        public decimal? hourly_rate { get; set; }
        public decimal? trip_rate { get; set; }
        public decimal? fuel_per_hour { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_production_actual_plan
    {
        public DateTime? daily_date { get; set; }
        public decimal? quantity { get; set; }
        public string business_unit_id { get; set; }
        public string activity_plan { get; set; }
        public string plan_type { get; set; }
    }
}

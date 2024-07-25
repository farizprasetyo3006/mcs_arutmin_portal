using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_barging_plan_nplct
    {
        public DateTime? daily_date { get; set; }
        public decimal? quantity { get; set; }
        public string product_name { get; set; }
        public string plan_type { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class raindelay_graph_ecoob_aims
    {
        public string details { get; set; }
        public string business_unit_name { get; set; }
        public decimal? val { get; set; }
        public decimal? avg_val { get; set; }
        public decimal? avg_eco { get; set; }
        public decimal? avg_budget { get; set; }
        public decimal? avg_forecast { get; set; }
        public DateTime? dates { get; set; }
        public string months { get; set; }
    }
}

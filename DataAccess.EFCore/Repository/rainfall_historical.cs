using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rainfall_historical
    {
        public string dates { get; set; }
        public string months { get; set; }
        public string business_area_name { get; set; }
        public decimal? rainfall_value { get; set; }
        public decimal? avg_5_year { get; set; }
        public decimal? avg_10_year { get; set; }
        public decimal? duration { get; set; }
    }
}

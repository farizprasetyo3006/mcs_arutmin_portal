using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_eight_week_forecast_SP
    {
        public string site { get; set; }
        public string week { get; set; }
        public string product_name { get; set; }
        public string pit { get; set; }
        public decimal? sum { get; set; }
    }
}

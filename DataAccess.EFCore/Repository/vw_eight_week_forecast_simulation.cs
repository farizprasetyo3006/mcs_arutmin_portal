using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_eight_week_forecast_simulation
    {
        public string site { get; set; }
        public string week { get; set; }
        public string pit { get; set; }
        public decimal? sum { get; set; }
        public string activity { get; set; }
        public string product { get; set; }
        public DateTime? start_week { get; set; }
        public DateTime? end_week { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rainfall_sliperry_schema
    {
        public string bu_code { get; set; }
        public string bu_name { get; set; }
        public DateTime? tanggal { get; set; }
        public string pit { get; set; }
        public decimal? duration { get; set; }
        public decimal? rainfall { get; set; }
        public decimal? frequency { get; set; }
        public bool? is_plan { get; set; }
        public bool? is_forecast { get; set; }
        public decimal? slippery_duration { get; set; }
        public bool? sliperry_plan { get; set; }
        public bool? slippery_forecast { get; set; }
    }
}

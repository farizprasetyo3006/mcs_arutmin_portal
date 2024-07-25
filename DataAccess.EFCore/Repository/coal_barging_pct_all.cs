using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class coal_barging_pct_all
    {
        public int? orders { get; set; }
        public string details { get; set; }
        public string business_unit_name { get; set; }
        public decimal? bit_coal { get; set; }
        public decimal? eco_coal { get; set; }
        public decimal? sarongga { get; set; }
        public decimal? total { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class stock_nplct_discharge
    {
        public string contractor { get; set; }
        public DateTime? last_in_date { get; set; }
        public DateTime? last_out_date { get; set; }
        public string product { get; set; }
        public decimal? stock_in { get; set; }
        public decimal? stock_out { get; set; }
        public decimal? daily_stock { get; set; }
    }
}

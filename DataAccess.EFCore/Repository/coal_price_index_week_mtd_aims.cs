using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class coal_price_index_week_mtd_aims
    {
        public int? orders { get; set; }
        public string week_details { get; set; }
        public string mtd_details { get; set; }
        public string week_price_index_code { get; set; }
        public string mtd_price_index_code { get; set; }
        public string week_gar { get; set; }
        public string mtd_gar { get; set; }
        public DateTime? week_max_date { get; set; }
        public DateTime? mtd_max_date { get; set; }
        public DateTime? week_min_date { get; set; }
        public DateTime? mtd_min_date { get; set; }
        public decimal? week_max_val { get; set; }
        public decimal? week_min_val { get; set; }
        public decimal? mtd_max_val { get; set; }
        public decimal? mtd_min_val { get; set; }
        public decimal? week_chg_val { get; set; }
        public decimal? mtd_chg_val { get; set; }
    }
}

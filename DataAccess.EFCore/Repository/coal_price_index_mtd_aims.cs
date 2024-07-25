using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class coal_price_index_mtd_aims
    {
        public string details { get; set; }
        public string price_index_code { get; set; }
        public string gar { get; set; }
        public DateTime? max_date { get; set; }
        public decimal? max_val { get; set; }
        public DateTime? min_date { get; set; }
        public decimal? min_val { get; set; }
        public decimal? chg_val { get; set; }
    }
}

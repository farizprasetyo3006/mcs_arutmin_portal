using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_month_summary_aims
    {
        public string details { get; set; }
        public decimal? years { get; set; }
        public string months { get; set; }
        public decimal? orders { get; set; }
        public decimal? sales { get; set; }
        public decimal? revenue { get; set; }
        public decimal? price_fob { get; set; }
        public decimal? cv_gar { get; set; }
    }
}

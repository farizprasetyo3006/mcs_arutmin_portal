using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_summary_aims_invoice
    {
        public string product { get; set; }
        public decimal? sales { get; set; }
        public decimal? revenue { get; set; }
        public decimal? price_fob { get; set; }
        public decimal? cv_gar { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_stockpile_summary_monthly
    {
        public string stock_location_name { get; set; }
        public string stock_location_id { get; set; }
        public DateTime? dates { get; set; }
        public long? value_partition { get; set; }
        public decimal? closing_before { get; set; }
        public decimal? closing { get; set; }
    }
}

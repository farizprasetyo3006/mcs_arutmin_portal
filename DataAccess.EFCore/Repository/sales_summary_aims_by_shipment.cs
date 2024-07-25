using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_summary_aims_by_shipment
    {
        public int? quarters { get; set; }
        public string certain_group { get; set; }
        public decimal? mtd { get; set; }
        public decimal? m1 { get; set; }
        public decimal? m2 { get; set; }
        public decimal? m3 { get; set; }
        public decimal? q1 { get; set; }
        public decimal? q2 { get; set; }
        public decimal? q3 { get; set; }
        public decimal? q4 { get; set; }
        public decimal? total_year { get; set; }
    }
}

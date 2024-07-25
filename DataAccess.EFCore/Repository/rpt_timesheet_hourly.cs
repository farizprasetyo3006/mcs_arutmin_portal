using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_hourly
    {
        public DateTime? timesheet_date { get; set; }
        public TimeSpan? classification { get; set; }
        public string timesheet_date_view { get; set; }
        public string shift_code { get; set; }
        public string detail_item { get; set; }
        public string detail_item1 { get; set; }
        public string detail_items { get; set; }
        public int? plan { get; set; }
        public decimal? volume { get; set; }
        public string event_category_name { get; set; }
        public string event_definition_category_name { get; set; }
        public decimal? minute { get; set; }
        public string product { get; set; }
        public string product_name { get; set; }
        public string loader_name { get; set; }
        public string cn_unit { get; set; }
    }
}

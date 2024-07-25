using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_progressive_plan
    {
        public string product { get; set; }
        public string digger { get; set; }
        public DateTime? timesheet_date { get; set; }
        public long? volume { get; set; }
        public decimal? commulative { get; set; }
    }
}

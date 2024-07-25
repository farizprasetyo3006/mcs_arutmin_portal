using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_plan_detail
    {
        public string id { get; set; }
        public decimal? volum { get; set; }
        public string timesheet_time { get; set; }
    }
}

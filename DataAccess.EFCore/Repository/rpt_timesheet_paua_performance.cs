using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_paua_performance
    {
        public string cn_unit { get; set; }
        public DateTime? timesheet_date { get; set; }
        public string timesheet_date_week { get; set; }
        public decimal? pa_plan { get; set; }
        public decimal? pa { get; set; }
        public decimal? pa_achive { get; set; }
        public decimal? pa_dev { get; set; }
        public decimal? ua_plan { get; set; }
        public decimal? ua { get; set; }
        public decimal? ua_achive { get; set; }
        public decimal? ua_dev { get; set; }
        public decimal? totalhr { get; set; }
        public decimal? breakdownhr { get; set; }
        public decimal? delayhr { get; set; }
        public decimal? nonproductionhr { get; set; }
        public decimal? idlehr { get; set; }
        public decimal? productionhr { get; set; }
        public decimal? losstimehr { get; set; }
    }
}

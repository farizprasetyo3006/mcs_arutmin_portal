using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_hr_only_bycnunit
    {
        public string cn_unit_id { get; set; }
        public string cn_unit { get; set; }
        public string product { get; set; }
        public DateTime? timesheet_date { get; set; }
        public TimeSpan? classification { get; set; }
        public decimal? volume { get; set; }
        public decimal? vol_distance { get; set; }
    }
}

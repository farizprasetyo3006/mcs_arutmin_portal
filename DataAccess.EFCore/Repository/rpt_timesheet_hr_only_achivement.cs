using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_hr_only_achivement
    {
        public DateTime? timesheet_date { get; set; }
        public string timesheet_date_show { get; set; }
        public string product { get; set; }
        public string shift_code { get; set; }
        public string classification { get; set; }
        public decimal? volume_plan { get; set; }
        public decimal? volume { get; set; }
        public decimal? achivement { get; set; }
    }
}

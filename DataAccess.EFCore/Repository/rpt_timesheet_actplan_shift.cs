using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_actplan_shift
    {
        public DateTime? timesheet_date { get; set; }
        public string timesheet_date_show { get; set; }
        public string timesheet_date_week { get; set; }
        public string product { get; set; }
        public string digger { get; set; }
        public string loader_name { get; set; }
        public decimal? volume { get; set; }
        public decimal? volume_plan { get; set; }
        public int? achivement { get; set; }
        public decimal? s1_vol { get; set; }
        public decimal? s2_vol { get; set; }
        public decimal? vol_distance { get; set; }
        public decimal? vol_distance_plan { get; set; }
        public decimal? s1_vol_distance { get; set; }
        public decimal? s2_vol_distance { get; set; }
    }
}

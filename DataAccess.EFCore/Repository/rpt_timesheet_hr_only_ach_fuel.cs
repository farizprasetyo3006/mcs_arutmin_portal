using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_hr_only_ach_fuel
    {
        public DateTime? timesheet_date { get; set; }
        public TimeSpan? hr_start { get; set; }
        public string shift_code { get; set; }
        public string product { get; set; }
        public decimal? volume_plans { get; set; }
        public decimal? volumes { get; set; }
        public decimal? achivements { get; set; }
        public decimal? fuel_coal_value { get; set; }
        public decimal? fuel_ob_value { get; set; }
        public decimal? fuel_coal_ratio { get; set; }
        public decimal? fuel_ob_ratio { get; set; }
    }
}

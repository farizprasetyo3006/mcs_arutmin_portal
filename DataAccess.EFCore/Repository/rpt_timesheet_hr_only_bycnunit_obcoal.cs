using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_hr_only_bycnunit_obcoal
    {
        public string cn_unit_id { get; set; }
        public string cn_unit { get; set; }
        public string product { get; set; }
        public DateTime? timesheet_date { get; set; }
        public TimeSpan? classification { get; set; }
        public decimal? ob_volume { get; set; }
        public decimal? ob_vol_distance { get; set; }
        public decimal? ob_distance { get; set; }
        public decimal? coal_volume { get; set; }
        public decimal? coal_vol_distance { get; set; }
        public decimal? coal_distance { get; set; }
    }
}

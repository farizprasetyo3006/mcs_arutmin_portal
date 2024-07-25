using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_join_survey_prorate
    {
        public string accounting_period_name { get; set; }
        public DateTime? timesheet_date { get; set; }
        public string cn_unit { get; set; }
        public decimal? qtysurvey { get; set; }
        public decimal? volume { get; set; }
        public decimal? survey { get; set; }
        public decimal? vol_distance { get; set; }
        public decimal? distance { get; set; }
        public decimal? jsdistance { get; set; }
        public decimal? prorate_distance { get; set; }
    }
}

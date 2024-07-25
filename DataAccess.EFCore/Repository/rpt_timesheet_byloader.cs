using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_byloader
    {
        public string accounting_period_name { get; set; }
        public string equipment_id { get; set; }
        public string equipment_code { get; set; }
        public decimal? minutes { get; set; }
        public decimal? hours { get; set; }
        public decimal? hourly_rate { get; set; }
        public decimal? total { get; set; }
    }
}

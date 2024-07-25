using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_hmkaryawan
    {
        public DateTime? timesheet_date { get; set; }
        public decimal? hour_start { get; set; }
        public decimal? hour_end { get; set; }
        public decimal? hm { get; set; }
        public string employee_number { get; set; }
        public string employee_name { get; set; }
        public string cn_unit { get; set; }
    }
}

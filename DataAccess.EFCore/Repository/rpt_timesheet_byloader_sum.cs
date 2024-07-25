using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_byloader_sum
    {
        public string accounting_period_name { get; set; }
        public decimal? grandtotal { get; set; }
    }
}

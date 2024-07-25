using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_timesheet_ell
    {
        public string id { get; set; }
        public string egi_unit { get; set; }
        public string event_date { get; set; }
        public string shift_code { get; set; }
        public string event_category_code { get; set; }
        public string mulai { get; set; }
        public decimal? minute { get; set; }
        public decimal? hour_start { get; set; }
        public decimal? hour_end { get; set; }
        public string uom_symbol { get; set; }
        public string timesheet_date_show { get; set; }
        public string material_code { get; set; }
        public string organization_id { get; set; }
        public string organization_code { get; set; }
    }
}

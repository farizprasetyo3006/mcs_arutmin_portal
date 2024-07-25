using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_plan_hourly
    {
        public string cn_unit_id { get; set; }
        public string cn_unit { get; set; }
        public string product { get; set; }
        public string product_name { get; set; }
        public DateTime? timesheet_date { get; set; }
        public string timesheet_date_show { get; set; }
        public string timesheet_date_day { get; set; }
        public string shift_code { get; set; }
        public string shift_name { get; set; }
        public string business_area_code { get; set; }
        public string loader_name { get; set; }
        public decimal? volume_plan { get; set; }
        public decimal? distance { get; set; }
        public string stock_location_name { get; set; }
        public string event_category_name { get; set; }
        public string event_definition_category_name { get; set; }
        public decimal? minute { get; set; }
    }
}

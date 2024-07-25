using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class bi_timesheet_event
    {
        public string id { get; set; }
        public DateTime? timesheet_date { get; set; }
        public string timesheet_date_show { get; set; }
        public string timesheet_date_day { get; set; }
        public string business_area_code { get; set; }
        public string equipment_name { get; set; }
        public string product_categ { get; set; }
        public string product { get; set; }
        public string loader_name { get; set; }
        public string event_definition_category_code { get; set; }
        public string event_definition_category_name { get; set; }
        public string event_category_code { get; set; }
        public string event_category_name { get; set; }
        public TimeSpan? timesheet_time { get; set; }
        public decimal? minute { get; set; }
        public decimal? timesheet_hour { get; set; }
    }
}

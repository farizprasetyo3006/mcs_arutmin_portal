using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class bi_timesheet_detail
    {
        public string id { get; set; }
        public string ellproductcode { get; set; }
        public decimal? coalplan { get; set; }
        public decimal? obplan { get; set; }
        public decimal? coal { get; set; }
        public decimal? ob { get; set; }
        public string cn_unit_id { get; set; }
        public string digger { get; set; }
        public string product { get; set; }
        public string product_name { get; set; }
        public DateTime? timesheet_date { get; set; }
        public string timesheet_date_show { get; set; }
        public string timesheet_elldate_show { get; set; }
        public string timesheet_date_day { get; set; }
        public string timesheet_date_week { get; set; }
        public string shift_code { get; set; }
        public string shift_name { get; set; }
        public string hr_start { get; set; }
        public string hr_end { get; set; }
        public string business_area_code { get; set; }
        public string loader_name { get; set; }
        public decimal? volume_plan { get; set; }
        public decimal? volume { get; set; }
        public decimal? achievement { get; set; }
        public decimal? vol_distance_plan { get; set; }
        public decimal? vol_distance { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_ob_day_plan
    {
        public string cn_unit_id { get; set; }
        public string equipment_name { get; set; }
        public string product_name { get; set; }
        public DateTime? timesheet_date { get; set; }
        public string timesheet_date_show { get; set; }
        public string timesheet_date_day { get; set; }
        public string business_area_code { get; set; }
        public string loader_name { get; set; }
        public decimal? volume_plan { get; set; }
        public int? volume { get; set; }
        public int? achievement { get; set; }
        public decimal? vol_distance_plan { get; set; }
        public int? vol_distance { get; set; }
        public decimal? vols1_plan { get; set; }
        public int? vols1 { get; set; }
        public decimal? vols2_plan { get; set; }
        public int? vols2 { get; set; }
        public decimal? distances1_plan { get; set; }
        public int? distances1 { get; set; }
        public decimal? distances2_plan { get; set; }
        public int? distances2 { get; set; }
    }
}

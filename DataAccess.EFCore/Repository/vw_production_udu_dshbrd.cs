using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_production_udu_dshbrd
    {
        public string id { get; set; }
        public string accounting_period_name { get; set; }
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
        public string loader { get; set; }
        public int? volume_plan { get; set; }
        public decimal? volume { get; set; }
        public int? vol_distance_plan { get; set; }
        public decimal? vol_distance { get; set; }
    }
}

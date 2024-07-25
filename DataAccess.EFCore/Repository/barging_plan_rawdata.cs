using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class barging_plan_rawdata
    {
        public string id { get; set; }
        public string barging_plan_number { get; set; }
        public string mine_area { get; set; }
        public string pit_name { get; set; }
        public string mine_location_name { get; set; }
        public string plan_year { get; set; }
        public int? plan_month { get; set; }
        public decimal? monthly_qty { get; set; }
        public DateTime? plan_date { get; set; }
        public decimal? daily_qty { get; set; }
        public string plan_type { get; set; }
        public string activity_plan { get; set; }
        public string product_category_name { get; set; }
        public string product_name { get; set; }
        public string contractor_name { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public DateTime? created_on { get; set; }
        public DateTime? modified_on { get; set; }
        public bool? is_active { get; set; }
        public bool? is_locked { get; set; }
        public bool? is_default { get; set; }
    }
}

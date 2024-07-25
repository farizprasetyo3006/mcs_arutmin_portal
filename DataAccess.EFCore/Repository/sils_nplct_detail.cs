using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sils_nplct_detail
    {
        public string id { get; set; }
        public string created_by { get; set; }
        public DateTime? created_on { get; set; }
        public string modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool? is_active { get; set; }
        public bool? is_locked { get; set; }
        public bool? is_default { get; set; }
        public string owner_id { get; set; }
        public string organization_id { get; set; }
        public string entity_id { get; set; }
        public string business_unit_id { get; set; }
        public decimal? total_flow_time { get; set; }
        public string crew_id { get; set; }
        public decimal? total_down_time { get; set; }
        public decimal? gross_loading_time { get; set; }
        public decimal? nett_loading_time { get; set; }
        public decimal? gross_loading_rate { get; set; }
        public decimal? nett_loading_rate { get; set; }
        public string shift_id { get; set; }
        public string operator1_id { get; set; }
        public string operator2_id { get; set; }
        public string foreman_id { get; set; }
        public string sils_nplct_id { get; set; }
        public DateTime? date { get; set; }
        public decimal? total_on_board { get; set; }
        public decimal? total_shift { get; set; }
    }
}

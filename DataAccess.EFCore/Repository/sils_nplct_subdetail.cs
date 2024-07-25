using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sils_nplct_subdetail
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
        public string stockpile_1 { get; set; }
        public decimal? total_time { get; set; }
        public string category_id { get; set; }
        public string type_id { get; set; }
        public string location_id { get; set; }
        public string cv05 { get; set; }
        public string hatch_no { get; set; }
        public string sils_nplct_detail_id { get; set; }
        public DateTime? start_datetime { get; set; }
        public DateTime? stop_datetime { get; set; }
        public string cv06 { get; set; }
        public decimal? hatch_this_run { get; set; }
        public decimal? hatch_total { get; set; }
        public decimal? progressive_total { get; set; }
        public string description { get; set; }
        public string operator_id { get; set; }
        public string product_id { get; set; }
    }
}

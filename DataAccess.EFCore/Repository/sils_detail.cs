using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sils_detail
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
        public string crew_id { get; set; }
        public string shift_id { get; set; }
        public string source_id { get; set; }
        public string loader_id { get; set; }
        public decimal? total { get; set; }
        public decimal? total_down_time { get; set; }
        public decimal? progressive_total { get; set; }
        public decimal? progress { get; set; }
        public string location_id { get; set; }
        public string category_id { get; set; }
        public string type_id { get; set; }
        public decimal? flow_meter { get; set; }
        public string description { get; set; }
        public string sils_id { get; set; }
        public DateTime? start_flow_time { get; set; }
        public DateTime? stop_flow_time { get; set; }
        public DateTime? down_time_from { get; set; }
        public DateTime? down_time_to { get; set; }
    }
}

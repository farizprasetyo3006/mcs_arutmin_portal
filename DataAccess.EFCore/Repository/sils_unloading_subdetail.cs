using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sils_unloading_subdetail
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
        public string stockpile_2 { get; set; }
        public string stockpile_3 { get; set; }
        public decimal? total { get; set; }
        public string category_id { get; set; }
        public string type_id { get; set; }
        public string location_id { get; set; }
        public decimal? quantity { get; set; }
        public string description { get; set; }
        public string operator_id { get; set; }
        public string sils_unload_detail_id { get; set; }
        public DateTime? start_datetime { get; set; }
        public DateTime? stop_datetime { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sils_unloading
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
        public DateTime? date_arrived { get; set; }
        public DateTime? date_berthed { get; set; }
        public DateTime? start_loading { get; set; }
        public DateTime? finish_loading { get; set; }
        public DateTime? unberthed_time { get; set; }
        public DateTime? departed_time { get; set; }
        public string barge_id { get; set; }
        public string barge_rotation_id { get; set; }
        public string simultan_with { get; set; }
        public string mine_of_origin { get; set; }
        public string barge_destination { get; set; }
        public string product_id { get; set; }
        public decimal? analyte_1 { get; set; }
        public decimal? analyte_2 { get; set; }
        public decimal? analyte_3 { get; set; }
        public decimal? draft_scale { get; set; }
        public decimal? belt_scale { get; set; }
        public string supervisor { get; set; }
        public DateTime? approve_date { get; set; }
        public DateTime? unapprove_date { get; set; }
        public string approve_by_id { get; set; }
        public string disapprove_by_id { get; set; }
        public bool? approve_status { get; set; }
        public string master_list_id { get; set; }
        public decimal? tonnage { get; set; }
        public DateTime? flow_time { get; set; }
        public DateTime? down_time { get; set; }
        public decimal? site_scale { get; set; }
        public string contractor_id { get; set; }
    }
}

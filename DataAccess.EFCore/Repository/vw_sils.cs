using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sils
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
        public string barge_loading_number { get; set; }
        public string despatch_order_id { get; set; }
        public string barge_rotation_id { get; set; }
        public string master_list_id { get; set; }
        public string barge_id { get; set; }
        public string barge_name { get; set; }
        public string business_partner_name { get; set; }
        public string tug_name { get; set; }
        public string product_name { get; set; }
        public string tug_id { get; set; }
        public string destination_location { get; set; }
        public DateTime? date_arrived { get; set; }
        public DateTime? date_berthed { get; set; }
        public DateTime? start_loading { get; set; }
        public DateTime? finish_loading { get; set; }
        public DateTime? unberthed_time { get; set; }
        public DateTime? departed_time { get; set; }
        public string product_id { get; set; }
        public decimal? analyte_1 { get; set; }
        public decimal? analyte_2 { get; set; }
        public decimal? water_consumption { get; set; }
        public decimal? chemical_consumption { get; set; }
        public decimal? draft_scale { get; set; }
        public decimal? belt_scale { get; set; }
        public string operator_id { get; set; }
        public string foreman_id { get; set; }
        public string captain_id { get; set; }
        public string approve_status { get; set; }
        public string approve_by_id { get; set; }
        public string disapprove_by_id { get; set; }
        public decimal? sum_progressive { get; set; }
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string contractor_id { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sils_nplct
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
        public string transaction_number { get; set; }
        public string despatch_order_id { get; set; }
        public string process_flow_id { get; set; }
        public string vessel_id { get; set; }
        public string product_brand { get; set; }
        public string draft_survey_number { get; set; }
        public decimal? bw_start { get; set; }
        public decimal? bw_end { get; set; }
        public decimal? tonnage_scale { get; set; }
        public decimal? tonnage_draft { get; set; }
        public decimal? total_on_board { get; set; }
        public decimal? gross_loading_time { get; set; }
        public decimal? nett_loading_time { get; set; }
        public decimal? gross_loading_rate { get; set; }
        public decimal? nett_loading_rate { get; set; }
        public decimal? supervisor { get; set; }
        public string at_anchorage { get; set; }
        public string description { get; set; }
        public string despatch_demmurage { get; set; }
        public string source_location_id { get; set; }
        public string contractor_id { get; set; }
    }
}

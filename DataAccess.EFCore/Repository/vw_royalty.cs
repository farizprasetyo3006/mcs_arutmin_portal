using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_royalty
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
        public string royalty_code { get; set; }
        public string despatch_order_id { get; set; }
        public string royalty_reference { get; set; }
        public DateTime? royalty_date { get; set; }
        public string nama_pemegang_et { get; set; }
        public string status_id { get; set; }
        public string status_code { get; set; }
        public string status_name { get; set; }
        public string nomor_sk_iupk { get; set; }
        public string nomor_et { get; set; }
        public string destination_id { get; set; }
        public string destination_name { get; set; }
        public string nomor_invoice { get; set; }
        public DateTime? bl_date { get; set; }
        public string delivery_term { get; set; }
        public string currency_exchange_id { get; set; }
        public string destination_country { get; set; }
        public string status_buyer_id { get; set; }
        public string status_buyer { get; set; }
        public string buyer { get; set; }
        public string address { get; set; }
        public string loading_port { get; set; }
        public string stock_location_name { get; set; }
        public string discharge_port { get; set; }
        public string vessel { get; set; }
        public string barge_id { get; set; }
        public string imo_number { get; set; }
        public string tpk_barge { get; set; }
        public string vessel_flag { get; set; }
        public string barge_flag { get; set; }
        public string coal_origin_id { get; set; }
        public string coal_origin_name { get; set; }
        public string tug_id { get; set; }
        public string permit_location { get; set; }
        public string tpk_tug { get; set; }
        public string notes { get; set; }
        public string tug_flag { get; set; }
        public decimal? volume_loading { get; set; }
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

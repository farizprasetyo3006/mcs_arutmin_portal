using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_despatch_demurrage_detail
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
        public string despatch_demurrage_id { get; set; }
        public string contract_name { get; set; }
        public string port_location { get; set; }
        public decimal? despatch_percent { get; set; }
        public decimal? loading_rate { get; set; }
        public string loading_rate_unit { get; set; }
        public decimal? turn_time { get; set; }
        public string turn_time_unit { get; set; }
        public DateTime? laytime_commenced { get; set; }
        public DateTime? laytime_completed { get; set; }
        public DateTime? actual_commenced { get; set; }
        public DateTime? actual_completed { get; set; }
        public string sof_id { get; set; }
        public string sof_name { get; set; }
        public string sof_number { get; set; }
        public string currency_id { get; set; }
        public string currency_code { get; set; }
        public string currency_symbol { get; set; }
        public string currency_name { get; set; }
        public decimal? rate { get; set; }
        public string term_name { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

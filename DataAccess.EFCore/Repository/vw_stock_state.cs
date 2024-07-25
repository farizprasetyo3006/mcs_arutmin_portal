using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_stock_state
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
        public string transaction_id { get; set; }
        public DateTime? transaction_datetime { get; set; }
        public string stock_location_id { get; set; }
        public string stock_location_name { get; set; }
        public string product_in_id { get; set; }
        public string product_in_name { get; set; }
        public string product_out_id { get; set; }
        public string product_out_name { get; set; }
        public decimal? qty_opening { get; set; }
        public decimal? qty_in { get; set; }
        public decimal? qty_out { get; set; }
        public decimal? qty_adjustment { get; set; }
        public decimal? qty_survey { get; set; }
        public decimal? qty_closing { get; set; }
        public decimal? qty_in_provisional { get; set; }
        public decimal? qty_out_provisional { get; set; }
        public decimal? qty_adjustment_provisional { get; set; }
        public decimal? qty_survey_provisional { get; set; }
        public decimal? qty_closing_provisional { get; set; }
        public string survey_id { get; set; }
        public string survey_number { get; set; }
        public DateTime? survey_date { get; set; }
        public string draft_survey_id { get; set; }
        public string draft_survey_number { get; set; }
        public DateTime? draft_survey_date { get; set; }
        public string joint_survey_id { get; set; }
        public string joint_survey_number { get; set; }
        public DateTime? joint_survey_date { get; set; }
        public string quality_sampling_id { get; set; }
        public string sampling_number { get; set; }
        public DateTime? sampling_datetime { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

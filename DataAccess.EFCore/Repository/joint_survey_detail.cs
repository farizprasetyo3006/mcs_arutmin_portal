using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class joint_survey_detail
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
        public string accounting_period_id { get; set; }
        public string progress_claim_id { get; set; }
        public string product_id { get; set; }
        public string sampling_template_id { get; set; }
        public decimal? quantity { get; set; }
        public string uom_id { get; set; }
        public DateTime start_period_date { get; set; }
        public DateTime end_period_date { get; set; }
        public decimal? distance { get; set; }
        public decimal? elevation { get; set; }
        public string transport_model { get; set; }
        public string mine_location_id { get; set; }
        public string stockpile_location_id { get; set; }
        public string port_location_id { get; set; }
        public string waste_location_id { get; set; }
        public decimal? quantity_carry_over { get; set; }
        public string location_id { get; set; }
        public decimal? distance_carry_over { get; set; }
        public decimal? elevation_carry_over { get; set; }
        public string business_process_id { get; set; }
        public string joint_survey_id { get; set; }
    }
}

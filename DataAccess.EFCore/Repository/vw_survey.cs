using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_survey
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
        public string survey_number { get; set; }
        public DateTime? survey_date { get; set; }
        public string stock_location_id { get; set; }
        public string stock_location_name { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string sampling_template_id { get; set; }
        public string sampling_template_name { get; set; }
        public string surveyor_id { get; set; }
        public string surveyor_name { get; set; }
        public string surveyor_code { get; set; }
        public bool? is_draft_survey { get; set; }
        public string approved_by { get; set; }
        public string approver_name { get; set; }
        public DateTime? approved_on { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class survey
    {
        public survey()
        {
            hauling_transaction = new HashSet<hauling_transaction>();
            production_transaction = new HashSet<production_transaction>();
            survey_detail = new HashSet<survey_detail>();
            waste_removal = new HashSet<waste_removal>();
        }

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
        public string survey_number { get; set; }
        public DateTime survey_date { get; set; }
        public string stock_location_id { get; set; }
        public string product_id { get; set; }
        public string sampling_template_id { get; set; }
        public decimal? quantity { get; set; }
        public string uom_id { get; set; }
        public string surveyor_id { get; set; }
        public bool? is_draft_survey { get; set; }
        public string approved_by { get; set; }
        public DateTime? approved_on { get; set; }
        public string accounting_period_id { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<hauling_transaction> hauling_transaction { get; set; }
        public virtual ICollection<production_transaction> production_transaction { get; set; }
        public virtual ICollection<survey_detail> survey_detail { get; set; }
        public virtual ICollection<waste_removal> waste_removal { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class advance_contract_charge_detail
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
        public string advance_contract_charge_id { get; set; }
        public string advance_contract_detail_id { get; set; }
        public string price_index_id { get; set; }
        public string joint_survey_id { get; set; }
        public string formula { get; set; }
        public string variable { get; set; }

        public virtual advance_contract_charge advance_contract_charge_ { get; set; }
        public virtual joint_survey joint_survey_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual price_index price_index_ { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_advance_contract_reference_detail
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
        public string advance_contract_charge_id { get; set; }
        public string advance_contract_reference_id { get; set; }
        public string advance_contract_id { get; set; }
        public string advance_contract_detail_id { get; set; }
        public string charge_name { get; set; }
        public string formula { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public decimal? decimal_places { get; set; }
        public string rounding_type_id { get; set; }
        public string rounding_type { get; set; }
        public bool? is_base_formula { get; set; }
    }
}

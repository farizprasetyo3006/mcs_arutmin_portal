using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class quality_sampling_analyte
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
        public string quality_sampling_id { get; set; }
        public string analyte_id { get; set; }
        public string uom_id { get; set; }
        public decimal? analyte_value { get; set; }
        public string alias { get; set; }
        public string alias_formula { get; set; }
        public bool? coa_display { get; set; }
        public int? order { get; set; }
        public bool? trace_elements { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual quality_sampling quality_sampling_ { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class quality_sampling_approval
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
        public bool? is_approved { get; set; }
        public string approved_by_id { get; set; }
        public DateTime? approved_on { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

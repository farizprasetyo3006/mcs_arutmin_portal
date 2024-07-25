using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class uom_conversion
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
        public string source_uom_id { get; set; }
        public string target_uom_id { get; set; }
        public decimal conversion_value { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual uom source_uom_ { get; set; }
        public virtual uom target_uom_ { get; set; }
    }
}

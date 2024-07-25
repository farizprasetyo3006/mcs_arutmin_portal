using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class material_type
    {
        public material_type()
        {
            material_type_analyte = new HashSet<material_type_analyte>();
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
        public string material_type_name { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<material_type_analyte> material_type_analyte { get; set; }
    }
}

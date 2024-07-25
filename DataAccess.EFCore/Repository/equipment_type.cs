using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class equipment_type
    {
        public equipment_type()
        {
            delay = new HashSet<delay>();
            equipment = new HashSet<equipment>();
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
        public string equipment_type_name { get; set; }
        public string equipment_type_code { get; set; }
        public string item_class { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<delay> delay { get; set; }
        public virtual ICollection<equipment> equipment { get; set; }
    }
}

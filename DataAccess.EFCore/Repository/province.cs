using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class province
    {
        public province()
        {
            city = new HashSet<city>();
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
        public string country_id { get; set; }
        public string province_name { get; set; }
        public string province_code { get; set; }

        public virtual country country_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<city> city { get; set; }
    }
}

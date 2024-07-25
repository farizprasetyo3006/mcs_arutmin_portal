using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class price_index_map
    {
        public price_index_map()
        {
            price_index_map_detail = new HashSet<price_index_map_detail>();
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
        public string price_index_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual price_index price_index_ { get; set; }
        public virtual ICollection<price_index_map_detail> price_index_map_detail { get; set; }
    }
}

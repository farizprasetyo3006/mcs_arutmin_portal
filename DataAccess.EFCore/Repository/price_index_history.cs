using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class price_index_history
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
        public string price_index_id { get; set; }
        public DateTime index_date { get; set; }
        public decimal index_value { get; set; }
        public string note { get; set; }
        public string master_list_id { get; set; }
        public bool? is_forecast { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual price_index price_index_ { get; set; }
    }
}

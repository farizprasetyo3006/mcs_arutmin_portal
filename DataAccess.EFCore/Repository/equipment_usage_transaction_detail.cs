using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class equipment_usage_transaction_detail
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
        public string cn_unit_id { get; set; }
        public string event_category_id { get; set; }
        public DateTime date { get; set; }
        public decimal? duration { get; set; }
        public string equipment_usage_transaction_id { get; set; }

        public virtual equipment_usage_transaction equipment_usage_transaction_ { get; set; }
        public virtual event_category event_category_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

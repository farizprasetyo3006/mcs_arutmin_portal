using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class chls_additional_info
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
        public string chls_id { get; set; }
        public string group_id { get; set; }
        public string description_id { get; set; }
        public decimal? start_count { get; set; }
        public decimal? stop_count { get; set; }
        public decimal? total_count { get; set; }
        public string uom_id { get; set; }

        public virtual chls chls_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

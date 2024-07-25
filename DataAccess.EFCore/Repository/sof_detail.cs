using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sof_detail
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
        public string sof_id { get; set; }
        public DateTime start_datetime { get; set; }
        public DateTime end_datetime { get; set; }
        public string event_category_id { get; set; }
        public string remark { get; set; }
        public decimal? percentage { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual sof sof_ { get; set; }
    }
}

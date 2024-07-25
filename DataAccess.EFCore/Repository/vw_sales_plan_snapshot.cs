using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_plan_snapshot
    {
        public string organization_name { get; set; }
        public string owner_name { get; set; }
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
        public string sales_plan_id { get; set; }
        public string plan_name { get; set; }
        public string snapshot_name { get; set; }
        public string notes { get; set; }
        public string data { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class day_work_detail
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
        public string equipment_id { get; set; }
        public string accounting_period_id { get; set; }
        public DateTime? date_time { get; set; }
        public decimal? hm_start { get; set; }
        public decimal? hm_end { get; set; }
        public decimal? hm_duration { get; set; }
        public string note { get; set; }
        public string day_work_id { get; set; }

        public virtual accounting_period accounting_period_ { get; set; }
        public virtual day_work day_work_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

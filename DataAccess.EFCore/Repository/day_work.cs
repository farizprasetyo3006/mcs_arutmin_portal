using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class day_work
    {
        public day_work()
        {
            day_work_detail = new HashSet<day_work_detail>();
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
        public string daywork_number { get; set; }
        public string advance_contract_id { get; set; }
        public string advance_contract_reference_id { get; set; }
        public string accounting_period_id { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string operator_id { get; set; }
        public string supervisor_id { get; set; }
        public string shift_id { get; set; }
        public string reference_number { get; set; }
        public string note { get; set; }
        public DateTime? transaction_date { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<day_work_detail> day_work_detail { get; set; }
    }
}

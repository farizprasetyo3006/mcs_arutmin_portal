using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class accounting_period
    {
        public accounting_period()
        {
            equipment_incident = new HashSet<equipment_incident>();
            equipment_usage_transaction = new HashSet<equipment_usage_transaction>();
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
        public string accounting_period_name { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public bool? is_closed { get; set; }
        public bool? aktif { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<equipment_incident> equipment_incident { get; set; }
        public virtual ICollection<equipment_usage_transaction> equipment_usage_transaction { get; set; }
    }
}

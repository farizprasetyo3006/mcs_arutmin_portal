using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class accounting_period
    {
        public accounting_period()
        {
            barging_transaction = new HashSet<barging_transaction>();
            coal_transfer = new HashSet<coal_transfer>();
            equipment_incident = new HashSet<equipment_incident>();
            equipment_usage_transaction = new HashSet<equipment_usage_transaction>();
            hauling_transaction = new HashSet<hauling_transaction>();
            processing_transaction = new HashSet<processing_transaction>();
            production_transaction = new HashSet<production_transaction>();
            rehandling_transaction = new HashSet<rehandling_transaction>();
            waste_removal = new HashSet<waste_removal>();
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
        public virtual ICollection<barging_transaction> barging_transaction { get; set; }
        public virtual ICollection<coal_transfer> coal_transfer { get; set; }
        public virtual ICollection<equipment_incident> equipment_incident { get; set; }
        public virtual ICollection<equipment_usage_transaction> equipment_usage_transaction { get; set; }
        public virtual ICollection<hauling_transaction> hauling_transaction { get; set; }
        public virtual ICollection<processing_transaction> processing_transaction { get; set; }
        public virtual ICollection<production_transaction> production_transaction { get; set; }
        public virtual ICollection<rehandling_transaction> rehandling_transaction { get; set; }
        public virtual ICollection<waste_removal> waste_removal { get; set; }
    }
}

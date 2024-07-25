using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class shift
    {
        public shift()
        {
            coal_transfer = new HashSet<coal_transfer>();
            hauling_transaction = new HashSet<hauling_transaction>();
            processing_transaction = new HashSet<processing_transaction>();
            production_transaction = new HashSet<production_transaction>();
            rehandling_transaction = new HashSet<rehandling_transaction>();
            timesheet = new HashSet<timesheet>();
            timesheet_plan = new HashSet<timesheet_plan>();
            waste_removaldestination_shift_ = new HashSet<waste_removal>();
            waste_removalsource_shift_ = new HashSet<waste_removal>();
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
        public string shift_category_id { get; set; }
        public string shift_name { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan? duration { get; set; }
        public TimeSpan? end_time { get; set; }
        public string shift_code { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual shift_category shift_category_ { get; set; }
        public virtual ICollection<coal_transfer> coal_transfer { get; set; }
        public virtual ICollection<hauling_transaction> hauling_transaction { get; set; }
        public virtual ICollection<processing_transaction> processing_transaction { get; set; }
        public virtual ICollection<production_transaction> production_transaction { get; set; }
        public virtual ICollection<rehandling_transaction> rehandling_transaction { get; set; }
        public virtual ICollection<timesheet> timesheet { get; set; }
        public virtual ICollection<timesheet_plan> timesheet_plan { get; set; }
        public virtual ICollection<waste_removal> waste_removaldestination_shift_ { get; set; }
        public virtual ICollection<waste_removal> waste_removalsource_shift_ { get; set; }
    }
}

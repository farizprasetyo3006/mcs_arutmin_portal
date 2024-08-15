using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class shift
    {
        public shift()
        {
            timesheet = new HashSet<timesheet>();
            timesheet_plan = new HashSet<timesheet_plan>();
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
        public virtual ICollection<timesheet> timesheet { get; set; }
        public virtual ICollection<timesheet_plan> timesheet_plan { get; set; }
    }
}

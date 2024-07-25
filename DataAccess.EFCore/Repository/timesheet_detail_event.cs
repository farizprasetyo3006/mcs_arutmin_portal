using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class timesheet_detail_event
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
        public string event_category_id { get; set; }
        public string event_definition_category_id { get; set; }
        public decimal? minute { get; set; }
        public string timesheet_detail_id { get; set; }

        public virtual event_category event_category_ { get; set; }
        public virtual event_definition_category event_definition_category_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual timesheet_detail timesheet_detail_ { get; set; }
    }
}

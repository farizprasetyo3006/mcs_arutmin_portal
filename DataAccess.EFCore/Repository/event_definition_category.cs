using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class event_definition_category
    {
        public event_definition_category()
        {
            delay_details = new HashSet<delay_details>();
            event_category = new HashSet<event_category>();
            timesheet_detail_event = new HashSet<timesheet_detail_event>();
            timesheet_detail_event_plan = new HashSet<timesheet_detail_event_plan>();
            timesheet_detail_productivity_problem = new HashSet<timesheet_detail_productivity_problem>();
            timesheet_detail_productivity_problem_plan = new HashSet<timesheet_detail_productivity_problem_plan>();
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
        public string event_definition_category_name { get; set; }
        public string event_definition_category_code { get; set; }
        public bool? is_problem_productivity { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<delay_details> delay_details { get; set; }
        public virtual ICollection<event_category> event_category { get; set; }
        public virtual ICollection<timesheet_detail_event> timesheet_detail_event { get; set; }
        public virtual ICollection<timesheet_detail_event_plan> timesheet_detail_event_plan { get; set; }
        public virtual ICollection<timesheet_detail_productivity_problem> timesheet_detail_productivity_problem { get; set; }
        public virtual ICollection<timesheet_detail_productivity_problem_plan> timesheet_detail_productivity_problem_plan { get; set; }
    }
}

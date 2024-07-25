using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class event_category
    {
        public event_category()
        {
            delay_details = new HashSet<delay_details>();
            equipment_usage_transaction_detail = new HashSet<equipment_usage_transaction_detail>();
            sales_contract_despatch_demurrage_delay = new HashSet<sales_contract_despatch_demurrage_delay>();
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
        public string event_category_name { get; set; }
        public string event_category_code { get; set; }
        public string event_definition_category_id { get; set; }

        public virtual event_definition_category event_definition_category_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<delay_details> delay_details { get; set; }
        public virtual ICollection<equipment_usage_transaction_detail> equipment_usage_transaction_detail { get; set; }
        public virtual ICollection<sales_contract_despatch_demurrage_delay> sales_contract_despatch_demurrage_delay { get; set; }
        public virtual ICollection<timesheet_detail_event> timesheet_detail_event { get; set; }
        public virtual ICollection<timesheet_detail_event_plan> timesheet_detail_event_plan { get; set; }
        public virtual ICollection<timesheet_detail_productivity_problem> timesheet_detail_productivity_problem { get; set; }
        public virtual ICollection<timesheet_detail_productivity_problem_plan> timesheet_detail_productivity_problem_plan { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class employee
    {
        public employee()
        {
            advance_contract_valuation = new HashSet<advance_contract_valuation>();
            timesheet_planoperator_ = new HashSet<timesheet_plan>();
            timesheet_plansupervisor_ = new HashSet<timesheet_plan>();
            timesheetoperator_ = new HashSet<timesheet>();
            timesheetsupervisor_ = new HashSet<timesheet>();
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
        public string employee_number { get; set; }
        public string employee_name { get; set; }
        public string address { get; set; }
        public DateTime? join_date { get; set; }
        public bool? gender { get; set; }
        public string phone { get; set; }
        public bool? is_supervisor { get; set; }
        public bool? is_operator { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<advance_contract_valuation> advance_contract_valuation { get; set; }
        public virtual ICollection<timesheet_plan> timesheet_planoperator_ { get; set; }
        public virtual ICollection<timesheet_plan> timesheet_plansupervisor_ { get; set; }
        public virtual ICollection<timesheet> timesheetoperator_ { get; set; }
        public virtual ICollection<timesheet> timesheetsupervisor_ { get; set; }
    }
}

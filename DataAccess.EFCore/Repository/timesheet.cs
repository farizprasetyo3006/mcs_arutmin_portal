using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class timesheet
    {
        public timesheet()
        {
            timesheet_detail = new HashSet<timesheet_detail>();
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
        public string cn_unit_id { get; set; }
        public string mine_location_id { get; set; }
        public string operator_id { get; set; }
        public decimal? hour_start { get; set; }
        public decimal? hour_end { get; set; }
        public decimal? quantity { get; set; }
        public string shift_id { get; set; }
        public string uom_id { get; set; }
        public DateTime? timesheet_date { get; set; }
        public string material_id { get; set; }
        public string activity_id { get; set; }
        public string supervisor_id { get; set; }
        public string accounting_period_id { get; set; }

        public virtual master_list activity_ { get; set; }
        public virtual mine_location mine_location_ { get; set; }
        public virtual employee operator_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual shift shift_ { get; set; }
        public virtual employee supervisor_ { get; set; }
        public virtual uom uom_ { get; set; }
        public virtual ICollection<timesheet_detail> timesheet_detail { get; set; }
    }
}

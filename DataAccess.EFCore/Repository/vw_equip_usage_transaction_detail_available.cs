using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_equip_usage_transaction_detail_available
    {
        public DateTime? timesheet_date { get; set; }
        public string equipment_id { get; set; }
        public string equipment_name { get; set; }
        public string organization_id { get; set; }
        public string event_category_name { get; set; }
        public decimal? duration { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class land_clearing_detail
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
        public string land_clearing_id { get; set; }
        public string product_id { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan end_time { get; set; }
        public string contractor_id { get; set; }
        public decimal? luasan_area { get; set; }
        public string metode { get; set; }
        public string pic { get; set; }
        public string notes { get; set; }
        public DateTime? land_clearing_date { get; set; }
        public string shift_id { get; set; }

        public virtual land_clearing land_clearing_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class haze
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
        public DateTime date_time { get; set; }
        public decimal haze_value { get; set; }
        public string business_area_id { get; set; }
        public string shift_id { get; set; }
        public string contractor_id { get; set; }

        public virtual business_area business_area_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

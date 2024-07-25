using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class arutmin_waste_removal
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
        public string transaction_id { get; set; }
        public string fleet_id { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public string shift_id { get; set; }
        public string process_flow_id { get; set; }
        public string waste_id { get; set; }
        public decimal? density { get; set; }
        public decimal ritase { get; set; }
        public decimal tonnage { get; set; }
        public decimal? volume_bcm { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

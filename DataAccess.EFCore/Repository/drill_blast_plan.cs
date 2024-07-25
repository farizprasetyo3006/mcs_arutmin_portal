using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class drill_blast_plan
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
        public string plan_number { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string vendor_id { get; set; }
        public decimal? request_level { get; set; }
        public decimal? quantity { get; set; }
        public string pit_id { get; set; }
        public decimal? blast_volume { get; set; }
        public string uom_id { get; set; }
        public string material_type_id { get; set; }
        public string blasting_method_id { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual business_area pit_ { get; set; }
        public virtual uom uom_ { get; set; }
        public virtual contractor vendor_ { get; set; }
    }
}

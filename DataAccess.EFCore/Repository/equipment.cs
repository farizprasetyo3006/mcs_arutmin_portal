using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class equipment
    {
        public equipment()
        {
            delay = new HashSet<delay>();
            equipment_cost_rate = new HashSet<equipment_cost_rate>();
            equipment_incident = new HashSet<equipment_incident>();
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
        public string equipment_type_id { get; set; }
        public string equipment_name { get; set; }
        public string asset_number { get; set; }
        public decimal? capacity { get; set; }
        public string capacity_uom_id { get; set; }
        public string vendor_id { get; set; }
        public string equipment_code { get; set; }
        public bool? status { get; set; }
        public string vehicle_model { get; set; }
        public string vehicle_model_year { get; set; }
        public string vehicle_manufactured_year { get; set; }
        public decimal? plan { get; set; }

        public virtual equipment_type equipment_type_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<delay> delay { get; set; }
        public virtual ICollection<equipment_cost_rate> equipment_cost_rate { get; set; }
        public virtual ICollection<equipment_incident> equipment_incident { get; set; }
    }
}

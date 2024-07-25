using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class arutmin_waste_removal_item
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
        public string transport_id { get; set; }
        public decimal? ritase { get; set; }
        public decimal? truck_factor { get; set; }
        public decimal? tonnage { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

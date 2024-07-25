using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class shipping_instruction_stevedoring
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
        public string shipping_instruction_id { get; set; }
        public string barge_id { get; set; }
        public string port_location_id { get; set; }
        public decimal? cargo { get; set; }

        public virtual barge barge_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual port_location port_location_ { get; set; }
        public virtual shipping_instruction shipping_instruction_ { get; set; }
    }
}

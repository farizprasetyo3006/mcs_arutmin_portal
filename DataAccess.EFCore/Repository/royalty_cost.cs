using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class royalty_cost
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
        public string royalty_id { get; set; }
        public string port_load_id { get; set; }
        public string discharge_port_id { get; set; }
        public decimal? dist_barge_to_anchorage { get; set; }
        public decimal? barging_cost { get; set; }
        public decimal? freight_cost { get; set; }
        public decimal? transhipment_cost { get; set; }
        public decimal? total_join_cost { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

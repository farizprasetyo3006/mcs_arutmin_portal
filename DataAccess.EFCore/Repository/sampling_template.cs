using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sampling_template
    {
        public sampling_template()
        {
            sampling_template_detail = new HashSet<sampling_template_detail>();
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
        public string sampling_template_name { get; set; }
        public string sampling_template_code { get; set; }
        public bool? is_despatch_order_required { get; set; }
        public bool? is_stock_state { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<sampling_template_detail> sampling_template_detail { get; set; }
    }
}

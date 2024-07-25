using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class despatch_demurrage
    {
        public despatch_demurrage()
        {
            despatch_demurrage_delay = new HashSet<despatch_demurrage_delay>();
            despatch_demurrage_detail = new HashSet<despatch_demurrage_detail>();
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
        public string contract_name { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string contractor_id { get; set; }
        public string invoice_target { get; set; }
        public string despatch_order_id { get; set; }
        public string vessel_id { get; set; }

        public virtual despatch_order despatch_order_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<despatch_demurrage_delay> despatch_demurrage_delay { get; set; }
        public virtual ICollection<despatch_demurrage_detail> despatch_demurrage_detail { get; set; }
    }
}

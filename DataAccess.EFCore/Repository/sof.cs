using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sof
    {
        public sof()
        {
            despatch_demurrage_invoice = new HashSet<despatch_demurrage_invoice>();
            sales_contract_despatch_demurrage_term = new HashSet<sales_contract_despatch_demurrage_term>();
            sof_detail = new HashSet<sof_detail>();
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
        public string sof_name { get; set; }
        public string sof_number { get; set; }
        public string despatch_order_id { get; set; }
        public string vessel_id { get; set; }
        public string ops_name { get; set; }
        public string desdem_term_id { get; set; }
        public string despatch_demurrage_id { get; set; }
        public DateTime? nor_tendered { get; set; }
        public DateTime? nor_accepted { get; set; }
        public DateTime? laytime_commenced { get; set; }
        public DateTime? commenced_loading { get; set; }
        public DateTime? completed_loading { get; set; }
        public DateTime? laytime_completed { get; set; }
        public string wlcr { get; set; }
        public DateTime? arrival_datetime { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual vessel vessel_ { get; set; }
        public virtual ICollection<despatch_demurrage_invoice> despatch_demurrage_invoice { get; set; }
        public virtual ICollection<sales_contract_despatch_demurrage_term> sales_contract_despatch_demurrage_term { get; set; }
        public virtual ICollection<sof_detail> sof_detail { get; set; }
    }
}

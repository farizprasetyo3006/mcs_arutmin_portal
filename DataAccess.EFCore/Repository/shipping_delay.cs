using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class shipping_delay
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
        public string shipping_transaction_id { get; set; }
        public string despatch_order_delay_id { get; set; }
        public DateTime start_datetime { get; set; }
        public DateTime end_datetime { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual shipping_transaction shipping_transaction_ { get; set; }
    }
}

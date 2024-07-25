using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class railing_transaction_detail
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
        public string railing_transaction_id { get; set; }
        public string wagon_id { get; set; }
        public DateTime? loading_datetime { get; set; }
        public decimal? loading_quantity { get; set; }
        public string uom_id { get; set; }
        public DateTime? unloading_datetime { get; set; }
        public decimal? unloading_quantity { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual railing_transaction railing_transaction_ { get; set; }
    }
}

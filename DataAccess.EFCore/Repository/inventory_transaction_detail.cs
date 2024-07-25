using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class inventory_transaction_detail
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
        public string inventory_transaction_id { get; set; }
        public string transaction_datetime { get; set; }
        public string reference_number { get; set; }
        public decimal quantity { get; set; }
        public string uom_id { get; set; }
    }
}

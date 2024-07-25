using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class adjustment_sales_invoice
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
        public string sales_invoice_id { get; set; }
        public string adjustment_master_list_id { get; set; }
        public decimal? adjustment_price { get; set; }
        public string item_name { get; set; }
        public string notes { get; set; }
        public string item_group { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

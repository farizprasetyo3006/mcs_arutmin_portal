using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_invoice_attachment
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
        public string filename { get; set; }
        public string sales_invoice_id { get; set; }
        public string invoice_number { get; set; }
        public string despatch_order_id { get; set; }
        public decimal? quantity { get; set; }
        public string contract_product_id { get; set; }
    }
}

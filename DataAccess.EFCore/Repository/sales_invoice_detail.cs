using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_invoice_detail
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
        public string sales_invoice_id { get; set; }
        public string item { get; set; }
        public decimal? item_value { get; set; }
        public decimal? adjustment_value { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual sales_invoice sales_invoice_ { get; set; }
    }
}

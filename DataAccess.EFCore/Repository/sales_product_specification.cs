using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_product_specification
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
        public string sales_order_detail_id { get; set; }
        public string analyte_id { get; set; }
        public string uom_id { get; set; }
        public decimal? minimum_value { get; set; }
        public decimal? maximum_value { get; set; }
        public int? order_index { get; set; }

        public virtual analyte analyte_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual sales_order_detail sales_order_detail_ { get; set; }
    }
}

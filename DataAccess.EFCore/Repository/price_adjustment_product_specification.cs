using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class price_adjustment_product_specification
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
        public string price_adjustment_id { get; set; }
        public string analyte_id { get; set; }
        public string uom_id { get; set; }
        public string analyte_standard_id { get; set; }
        public decimal? value { get; set; }
        public decimal? target { get; set; }
        public decimal? minimum { get; set; }
        public decimal? maximum { get; set; }
        public decimal? adjust_value { get; set; }
        public decimal? fine_value { get; set; }

        public virtual analyte analyte_ { get; set; }
        public virtual master_list analyte_standard_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual sales_charge price_adjustment_ { get; set; }
        public virtual uom uom_ { get; set; }
    }
}

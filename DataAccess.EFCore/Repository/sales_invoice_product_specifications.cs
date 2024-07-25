using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_invoice_product_specifications
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
        public string analyte_id { get; set; }
        public string analyte_standard_id { get; set; }
        public decimal? value { get; set; }
        public decimal? target { get; set; }
        public string nilai_penyesuaian { get; set; }
        public decimal? nilai_denda_penolakan { get; set; }
    }
}

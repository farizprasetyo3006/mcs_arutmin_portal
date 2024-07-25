using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_invoice_product_specification
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
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string business_unit_name { get; set; }
        public string sales_invoice_id { get; set; }
        public string despatch_order_id { get; set; }
        public string analyte_id { get; set; }
        public string analyte_name { get; set; }
        public string analyte_symbol { get; set; }
        public string nilai_penyesuaian { get; set; }
        public decimal? nilai_denda_penolakan { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string uom_symbol { get; set; }
        public decimal? analyte_value { get; set; }
        public decimal? target { get; set; }
        public bool? coa_display { get; set; }
        public bool? trace_elements { get; set; }
        public bool? non_commercial { get; set; }
        public string sampling_analyte_id { get; set; }
        public string spec_id { get; set; }
        public decimal? orderanalyte { get; set; }
        public string invoice_number { get; set; }
    }
}

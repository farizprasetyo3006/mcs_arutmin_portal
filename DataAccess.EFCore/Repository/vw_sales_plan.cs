using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_plan
    {
        public string organization_name { get; set; }
        public string uom_name { get; set; }
        public string owner_name { get; set; }
        public string id { get; set; }
        public string created_by { get; set; }
        public string created_by_name { get; set; }
        public DateTime? created_on { get; set; }
        public string modified_by { get; set; }
        public string modified_by_name { get; set; }
        public DateTime? modified_on { get; set; }
        public bool? is_active { get; set; }
        public bool? is_locked { get; set; }
        public bool? is_default { get; set; }
        public string owner_id { get; set; }
        public string organization_id { get; set; }
        public string entity_id { get; set; }
        public string plan_year_id { get; set; }
        public string plan_year { get; set; }
        public decimal? revision_number { get; set; }
        public bool? is_baseline { get; set; }
        public bool? is_lock { get; set; }
        public decimal? quantity { get; set; }
        public string uom_id { get; set; }
        public string notes { get; set; }
        public decimal? rkab_quantity { get; set; }
        public string site_id { get; set; }
        public string site_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
    }
}

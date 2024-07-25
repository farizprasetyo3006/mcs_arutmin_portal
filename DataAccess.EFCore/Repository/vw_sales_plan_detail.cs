using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_plan_detail
    {
        public string organization_name { get; set; }
        public string owner_name { get; set; }
        public string plan_name { get; set; }
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
        public string sales_plan_id { get; set; }
        public int? month_id { get; set; }
        public string month_name { get; set; }
        public string nama_bulan { get; set; }
        public decimal? quantity { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_plan_customer
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
        public string customer_id { get; set; }
        public decimal? quantity { get; set; }
        public string sales_plan_detail_id { get; set; }
        public string sales_contract_id { get; set; }
        public string dmo { get; set; }
        public string electricity { get; set; }
        public int? monthly_sales_id { get; set; }
        public int? declared_month_id { get; set; }
        public string schedule_id { get; set; }
        public string area_id { get; set; }
        public string country_id { get; set; }
        public string brand_id { get; set; }
        public string product_category_id { get; set; }
        public string agents_id { get; set; }
        public string commitment_id { get; set; }
        public string pricing_id { get; set; }
        public string index_link_id { get; set; }
        public string status_id { get; set; }
        public string incoterm_id { get; set; }
        public string ship_no_id { get; set; }
        public string vessel_id { get; set; }
        public string tipe_penjualan_id { get; set; }
        public string source_coal_id { get; set; }
        public DateTime? laycan_start { get; set; }
        public DateTime? laycan_end { get; set; }
        public DateTime? eta_date { get; set; }

        public virtual customer customer_ { get; set; }
        public virtual sales_plan_detail sales_plan_detail_ { get; set; }
    }
}

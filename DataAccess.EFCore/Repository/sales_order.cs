using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_order
    {
        public sales_order()
        {
            sales_demurrage_rate = new HashSet<sales_demurrage_rate>();
            sales_order_detail = new HashSet<sales_order_detail>();
        }

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
        public string sales_order_number { get; set; }
        public DateTime sales_date { get; set; }
        public string accounting_period_id { get; set; }
        public string business_partner_id { get; set; }
        public string product_id { get; set; }
        public decimal? quantity { get; set; }
        public string uom_id { get; set; }
        public DateTime? shipping_start_date { get; set; }
        public DateTime? shipping_end_date { get; set; }
        public string sales_plan_id { get; set; }
        public decimal? unit_price { get; set; }
        public string currency_id { get; set; }
        public string reference_number { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<sales_demurrage_rate> sales_demurrage_rate { get; set; }
        public virtual ICollection<sales_order_detail> sales_order_detail { get; set; }
    }
}

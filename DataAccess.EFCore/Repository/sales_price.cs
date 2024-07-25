using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_price
    {
        public sales_price()
        {
            sales_contract_detail = new HashSet<sales_contract_detail>();
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
        public string price_type_id { get; set; }
        public decimal? price { get; set; }
        public string uom_id { get; set; }
        public string notes { get; set; }
        public string product_id { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual sales_price_type price_type_ { get; set; }
        public virtual product product_ { get; set; }
        public virtual uom uom_ { get; set; }
        public virtual ICollection<sales_contract_detail> sales_contract_detail { get; set; }
    }
}

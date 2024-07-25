using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_contract_detail
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
        public string sales_contract_id { get; set; }
        public string product_id { get; set; }
        public decimal? quantity { get; set; }
        public string sales_price_id { get; set; }
        public string sales_charge_id { get; set; }
        public string tax_id { get; set; }
        public string notes { get; set; }
        public decimal? price { get; set; }
        public string uom_id { get; set; }
        public string charge_formula { get; set; }
        public decimal? tax_rate { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual product product_ { get; set; }
        public virtual sales_charge sales_charge_ { get; set; }
        public virtual sales_contract sales_contract_ { get; set; }
        public virtual sales_price sales_price_ { get; set; }
        public virtual tax tax_ { get; set; }
        public virtual uom uom_ { get; set; }
    }
}

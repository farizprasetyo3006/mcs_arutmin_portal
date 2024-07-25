using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_charge
    {
        public sales_charge()
        {
            price_adjustment_product_specification = new HashSet<price_adjustment_product_specification>();
            sales_contract_charges = new HashSet<sales_contract_charges>();
            sales_contract_detail = new HashSet<sales_contract_detail>();
            sales_invoice_charges = new HashSet<sales_invoice_charges>();
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
        public string sales_charge_name { get; set; }
        public string charge_formula { get; set; }
        public string notes { get; set; }
        public string charge_type_id { get; set; }
        public string prerequisite { get; set; }
        public string sales_charge_code { get; set; }
        public string description { get; set; }
        public string industry_type_id { get; set; }

        public virtual master_list charge_type_ { get; set; }
        public virtual ICollection<price_adjustment_product_specification> price_adjustment_product_specification { get; set; }
        public virtual ICollection<sales_contract_charges> sales_contract_charges { get; set; }
        public virtual ICollection<sales_contract_detail> sales_contract_detail { get; set; }
        public virtual ICollection<sales_invoice_charges> sales_invoice_charges { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_contract_product
    {
        public sales_contract_product()
        {
            despatch_order = new HashSet<despatch_order>();
            sales_contract_product_specifications = new HashSet<sales_contract_product_specifications>();
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
        public string sales_contract_term_id { get; set; }
        public string contract_product_name { get; set; }
        public string product_id { get; set; }
        public decimal? maximum_order { get; set; }
        public decimal? mass_required { get; set; }
        public decimal? minimum_order { get; set; }
        public string uom_id { get; set; }
        public string analyte_standard_id { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual product product_ { get; set; }
        public virtual uom uom_ { get; set; }
        public virtual ICollection<despatch_order> despatch_order { get; set; }
        public virtual ICollection<sales_contract_product_specifications> sales_contract_product_specifications { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class price_index
    {
        public price_index()
        {
            advance_contract_charge_detail = new HashSet<advance_contract_charge_detail>();
            price_index_history = new HashSet<price_index_history>();
            price_index_map = new HashSet<price_index_map>();
            price_index_product_specification = new HashSet<price_index_product_specification>();
            sales_contract_quotation_price = new HashSet<sales_contract_quotation_price>();
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
        public string price_index_name { get; set; }
        public string price_index_code { get; set; }
        public bool? is_base_index { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<advance_contract_charge_detail> advance_contract_charge_detail { get; set; }
        public virtual ICollection<price_index_history> price_index_history { get; set; }
        public virtual ICollection<price_index_map> price_index_map { get; set; }
        public virtual ICollection<price_index_product_specification> price_index_product_specification { get; set; }
        public virtual ICollection<sales_contract_quotation_price> sales_contract_quotation_price { get; set; }
    }
}

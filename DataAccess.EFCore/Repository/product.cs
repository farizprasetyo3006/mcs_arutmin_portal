using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class product
    {
        public product()
        {
            processing_transaction = new HashSet<processing_transaction>();
            product_specification = new HashSet<product_specification>();
            sales_contract_detail = new HashSet<sales_contract_detail>();
            sales_contract_product = new HashSet<sales_contract_product>();
            sales_price = new HashSet<sales_price>();
            stock_location = new HashSet<stock_location>();
            waste_specification = new HashSet<waste_specification>();
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
        public string product_category_id { get; set; }
        public string product_name { get; set; }
        public string product_code { get; set; }
        public string coa_id { get; set; }
        public decimal? density { get; set; }
        public bool? is_bitcoal { get; set; }
        public bool? is_ecocoal { get; set; }
        public bool? is_mulia { get; set; }
        public bool? is_perintis { get; set; }
        public bool? is_satui { get; set; }
        public bool? is_jumbang { get; set; }
        public string master_list_id { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual product_category product_category_ { get; set; }
        public virtual ICollection<processing_transaction> processing_transaction { get; set; }
        public virtual ICollection<product_specification> product_specification { get; set; }
        public virtual ICollection<sales_contract_detail> sales_contract_detail { get; set; }
        public virtual ICollection<sales_contract_product> sales_contract_product { get; set; }
        public virtual ICollection<sales_price> sales_price { get; set; }
        public virtual ICollection<stock_location> stock_location { get; set; }
        public virtual ICollection<waste_specification> waste_specification { get; set; }
    }
}

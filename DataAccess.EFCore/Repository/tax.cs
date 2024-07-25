using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class tax
    {
        public tax()
        {
            sales_contract_detail = new HashSet<sales_contract_detail>();
            sales_contract_taxes = new HashSet<sales_contract_taxes>();
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
        public string tax_name { get; set; }
        public decimal? rate { get; set; }
        public decimal calculation_sign { get; set; }

        public virtual ICollection<sales_contract_detail> sales_contract_detail { get; set; }
        public virtual ICollection<sales_contract_taxes> sales_contract_taxes { get; set; }
    }
}

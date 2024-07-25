using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_price_type
    {
        public sales_price_type()
        {
            sales_price = new HashSet<sales_price>();
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
        public string sales_price_type_name { get; set; }
        public decimal? fix_value { get; set; }
        public decimal? average { get; set; }
        public string notes { get; set; }
        public string uom_id { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual uom uom_ { get; set; }
        public virtual ICollection<sales_price> sales_price { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class processing_closing_item
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
        public string processing_closing_id { get; set; }
        public DateTime? transaction_item_date { get; set; }
        public string business_area_id { get; set; }
        public string mine_location_id { get; set; }
        public decimal? quantity_item { get; set; }
        public string business_area_pit_id { get; set; }
        public string product_category_id { get; set; }
        public string contractor_id { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual processing_closing processing_closing_ { get; set; }
    }
}

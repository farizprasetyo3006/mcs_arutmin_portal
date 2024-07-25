using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class production_transaction_item
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
        public string production_transaction_id { get; set; }
        public string truck_id { get; set; }
        public decimal? truck_factor { get; set; }
        public decimal? ritase { get; set; }
        public string shift { get; set; }
        public decimal? jam01 { get; set; }
        public decimal? jam02 { get; set; }
        public decimal? jam03 { get; set; }
        public decimal? jam04 { get; set; }
        public decimal? jam05 { get; set; }
        public decimal? jam06 { get; set; }
        public decimal? jam07 { get; set; }
        public decimal? jam08 { get; set; }
        public decimal? jam09 { get; set; }
        public decimal? jam10 { get; set; }
        public decimal? jam11 { get; set; }
        public decimal? jam12 { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class processing_closing
    {
        public processing_closing()
        {
            processing_closing_item = new HashSet<processing_closing_item>();
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
        public string transaction_number { get; set; }
        public DateTime? transaction_date { get; set; }
        public string accounting_period_id { get; set; }
        public string advance_contract_id { get; set; }
        public string advance_contract_reference_id { get; set; }
        public DateTime? from_date { get; set; }
        public DateTime? to_date { get; set; }
        public decimal? volume { get; set; }
        public decimal? distance { get; set; }
        public string note { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<processing_closing_item> processing_closing_item { get; set; }
    }
}

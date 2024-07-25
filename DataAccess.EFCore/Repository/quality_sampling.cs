using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class quality_sampling
    {
        public quality_sampling()
        {
            barging_transaction = new HashSet<barging_transaction>();
            coal_transfer = new HashSet<coal_transfer>();
            processing_transaction = new HashSet<processing_transaction>();
            quality_sampling_analyte = new HashSet<quality_sampling_analyte>();
            quality_sampling_document = new HashSet<quality_sampling_document>();
            rehandling_transaction = new HashSet<rehandling_transaction>();
            waste_removal = new HashSet<waste_removal>();
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
        public string sampling_number { get; set; }
        public DateTime sampling_datetime { get; set; }
        public string stock_location_id { get; set; }
        public string product_id { get; set; }
        public string sampling_template_id { get; set; }
        public string surveyor_id { get; set; }
        public string despatch_order_id { get; set; }
        public bool? is_adjust { get; set; }
        public string sampling_type_id { get; set; }
        public bool? non_commercial { get; set; }
        public string shift_id { get; set; }
        public bool? is_draft { get; set; }
        public string barging_transaction_id { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<barging_transaction> barging_transaction { get; set; }
        public virtual ICollection<coal_transfer> coal_transfer { get; set; }
        public virtual ICollection<processing_transaction> processing_transaction { get; set; }
        public virtual ICollection<quality_sampling_analyte> quality_sampling_analyte { get; set; }
        public virtual ICollection<quality_sampling_document> quality_sampling_document { get; set; }
        public virtual ICollection<rehandling_transaction> rehandling_transaction { get; set; }
        public virtual ICollection<waste_removal> waste_removal { get; set; }
    }
}

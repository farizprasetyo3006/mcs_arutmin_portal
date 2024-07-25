using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class chls
    {
        public chls()
        {
            chls_additional_info = new HashSet<chls_additional_info>();
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
        public DateTime date { get; set; }
        public string shift_id { get; set; }
        public string product_id { get; set; }
        public string operator_id { get; set; }
        public string foreman_id { get; set; }
        public bool? approved { get; set; }
        public string approved_by { get; set; }
        public string disapprove_by_id { get; set; }
        public decimal? gross_loading_olc { get; set; }
        public decimal? net_loading_olc { get; set; }
        public decimal? gross_loading_cpp { get; set; }
        public decimal? net_loading_cpp { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<chls_additional_info> chls_additional_info { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class land_clearing
    {
        public land_clearing()
        {
            land_clearing_detail = new HashSet<land_clearing_detail>();
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
        public string land_clearing_number { get; set; }
        public DateTime? land_clearing_date { get; set; }
        public string clearing_approval { get; set; }
        public string clearing_approval_date { get; set; }
        public string business_area_id { get; set; }
        public string pit_code { get; set; }
        public string seam_code { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public decimal? total_luasan_area { get; set; }
        public decimal? actual_luasan_area { get; set; }
        public string target { get; set; }

        public virtual ICollection<land_clearing_detail> land_clearing_detail { get; set; }
    }
}

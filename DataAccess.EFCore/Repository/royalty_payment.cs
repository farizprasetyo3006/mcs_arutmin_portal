using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class royalty_payment
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
        public string royalty_id { get; set; }
        public string billing_code { get; set; }
        public string ntb_ntp { get; set; }
        public string ntpn { get; set; }
        public DateTime? payment_date { get; set; }
        public decimal? royalty_paid_off { get; set; }
        public decimal? bmn_paid_off { get; set; }
        public decimal? pht_paid_off { get; set; }
        public decimal? dhpb_paid_off { get; set; }
        public decimal? royalty_outstanding { get; set; }
        public decimal? bmn_outstanding { get; set; }
        public decimal? pht_outstanding { get; set; }
        public decimal? dhpb_outstanding { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

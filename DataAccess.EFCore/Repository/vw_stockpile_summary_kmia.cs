using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_stockpile_summary_kmia
    {
        public string id { get; set; }
        public string organization_id { get; set; }
        public DateTime? trans_date { get; set; }
        public string stock_location_id { get; set; }
        public string stock_location_name { get; set; }
        public string stock_location_description { get; set; }
        public decimal? qty_closing { get; set; }
        public float? a_tm { get; set; }
        public decimal? a_im { get; set; }
        public float? a_ash { get; set; }
        public float? a_vm { get; set; }
        public float? a_fc { get; set; }
        public float? a_sulfur { get; set; }
        public float? a_gcv1 { get; set; }
        public float? a_gcv2 { get; set; }
    }
}

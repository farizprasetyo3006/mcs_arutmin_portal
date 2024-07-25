using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_benchmark_price_series_detail
    {
        public string benchmark_price_id { get; set; }
        public string brand_id { get; set; }
        public decimal? calori { get; set; }
        public string calori_uom_id { get; set; }
        public decimal? total_moisture { get; set; }
        public string total_moisture_uom_id { get; set; }
        public decimal? total_sulphur { get; set; }
        public string total_sulphur_uom_id { get; set; }
        public decimal? ash { get; set; }
        public string ash_uom_id { get; set; }
        public decimal? price { get; set; }
        public string currency_id { get; set; }
        public string price_uom_id { get; set; }
        public string id { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string brand_name { get; set; }
        public string notes { get; set; }
        public string brand_owner_id { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_benchmark_price_series
    {
        public string price_name { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string notes { get; set; }
        public string id { get; set; }
        public string reference_price_id { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string reference_price_name { get; set; }
        public DateTime? reference_start_date { get; set; }
        public DateTime? reference_end_date { get; set; }
        public string reference_notes { get; set; }
        public decimal? reference_price { get; set; }
        public string reference_currency_id { get; set; }
        public string reference_currency_uom { get; set; }
        public decimal? reference_calori { get; set; }
        public string reference_calori_uom_id { get; set; }
        public decimal? reference_total_moisture { get; set; }
        public string reference_total_moisture_uom_id { get; set; }
        public decimal? reference_total_sulphur { get; set; }
        public string reference_total_sulphur_uom_id { get; set; }
        public decimal? reference_ash { get; set; }
        public string reference_ash_uom_id { get; set; }
    }
}

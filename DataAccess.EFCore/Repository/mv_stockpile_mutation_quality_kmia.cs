using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class mv_stockpile_mutation_quality_kmia
    {
        public long? row_num { get; set; }
        public DateTime? trans_date { get; set; }
        public string stock_location_id { get; set; }
        public string quality_sampling_id { get; set; }
        public decimal? opening { get; set; }
        public decimal? mutation { get; set; }
        public decimal? survey { get; set; }
        public decimal? closing { get; set; }
        public string analyte_id { get; set; }
        public string analyte_symbol { get; set; }
        public string uom_name { get; set; }
        public decimal? analyte_opening { get; set; }
        public decimal? analyte_in { get; set; }
        public decimal? analyte_closing { get; set; }
        public long? quality_row_number { get; set; }
    }
}

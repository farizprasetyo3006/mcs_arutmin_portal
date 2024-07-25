using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class mv_mutation
    {
        public long? mutation_row_num { get; set; }
        public int? analyte_order { get; set; }
        public long? row_num { get; set; }
        public DateTime? trans_date { get; set; }
        public string ref_location_id { get; set; }
        public string stock_location_id { get; set; }
        public string quality_sampling_id { get; set; }
        public decimal? opening { get; set; }
        public decimal? mutation { get; set; }
        public decimal? survey { get; set; }
        public decimal? closing { get; set; }
        public string analyte_id { get; set; }
        public decimal? analyte_in { get; set; }
        public int? analyte_closed { get; set; }
    }
}

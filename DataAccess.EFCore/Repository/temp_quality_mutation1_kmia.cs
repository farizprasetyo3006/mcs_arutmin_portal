using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class temp_quality_mutation1_kmia
    {
        public DateTime? trans_date { get; set; }
        public string ref_location_id { get; set; }
        public string stock_location_id { get; set; }
        public string quality_sampling_id { get; set; }
        public double? opening { get; set; }
        public double? mutation { get; set; }
        public double? survey { get; set; }
        public double? closing { get; set; }
        public string analyte_id { get; set; }
        public double? analyte_opening { get; set; }
        public double? analyte_in { get; set; }
        public double? analyte_closing { get; set; }
        public long? row_num { get; set; }
    }
}

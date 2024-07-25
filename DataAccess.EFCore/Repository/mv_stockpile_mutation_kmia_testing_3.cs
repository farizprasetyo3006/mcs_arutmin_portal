using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class mv_stockpile_mutation_kmia_testing_3
    {
        public string entity_id { get; set; }
        public string entity { get; set; }
        public string ref_location_id { get; set; }
        public string stock_location_id { get; set; }
        public string stock_location_name { get; set; }
        public string stock_location_description { get; set; }
        public string trans_no { get; set; }
        public string survey_no { get; set; }
        public DateTime? trans_date { get; set; }
        public string quality_sampling_id { get; set; }
        public string sampling_number { get; set; }
        public DateTime? sampling_datetime { get; set; }
        public decimal? opening { get; set; }
        public decimal? trans_in { get; set; }
        public decimal? trans_out { get; set; }
        public decimal? mutation { get; set; }
        public int? adjusment { get; set; }
        public decimal? survey { get; set; }
        public decimal? closing { get; set; }
        public string id { get; set; }
        public string created_by { get; set; }
        public DateTime? created_on { get; set; }
        public string modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string organization_id { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public long? row_num { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class mv_business_area_index
    {
        public long? row_num { get; set; }
        public string id { get; set; }
        public string business_area_code { get; set; }
        public string business_area_name { get; set; }
        public string idx { get; set; }
    }
}

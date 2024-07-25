using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rainfall_schema
    {
        public string business_unit_name { get; set; }
        public string child_2 { get; set; }
        public string child_3 { get; set; }
        public string child_4 { get; set; }
        public decimal? rainfall_value { get; set; }
        public decimal? duration { get; set; }
        public string is_forecast { get; set; }
        public string is_plan { get; set; }
        public DateTime? dates { get; set; }
    }
}

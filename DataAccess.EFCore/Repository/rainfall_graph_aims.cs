using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rainfall_graph_aims
    {
        public string details { get; set; }
        public string business_unit_name { get; set; }
        public decimal? rainfall_value { get; set; }
        public DateTime? dates { get; set; }
        public string months { get; set; }
    }
}

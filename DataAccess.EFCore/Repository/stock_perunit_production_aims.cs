using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class stock_perunit_production_aims
    {
        public string details { get; set; }
        public string business_unit_name { get; set; }
        public DateTime? dates { get; set; }
        public string months { get; set; }
        public decimal? stock { get; set; }
        public decimal? avg_stock { get; set; }
    }
}

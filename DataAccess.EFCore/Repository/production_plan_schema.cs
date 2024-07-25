using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class production_plan_schema
    {
        public string product_category_name { get; set; }
        public string business_unit_name { get; set; }
        public string child_2 { get; set; }
        public string child_3 { get; set; }
        public string child_4 { get; set; }
        public string activity_plan { get; set; }
        public string plan_type { get; set; }
        public decimal? quantity { get; set; }
        public DateTime? dates { get; set; }
    }
}

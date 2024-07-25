using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class closing_schema
    {
        public string details { get; set; }
        public string business_unit_name { get; set; }
        public string business_partner_name { get; set; }
        public string product_category_name { get; set; }
        public string child_2 { get; set; }
        public string child_4 { get; set; }
        public string child_5 { get; set; }
        public decimal? qty { get; set; }
        public DateTime? from_date { get; set; }
        public DateTime? to_date { get; set; }
        public decimal? month_closing { get; set; }
        public decimal? year_closing { get; set; }
    }
}

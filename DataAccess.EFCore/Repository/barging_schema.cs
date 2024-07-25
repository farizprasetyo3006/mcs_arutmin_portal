using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class barging_schema
    {
        public string id { get; set; }
        public string voyage_number { get; set; }
        public string destination { get; set; }
        public string details { get; set; }
        public string product_category_name { get; set; }
        public string product_name { get; set; }
        public string business_unit_name { get; set; }
        public string stock_location_name { get; set; }
        public string business_partner_name { get; set; }
        public string barge { get; set; }
        public DateTime? dates { get; set; }
        public decimal? qty { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class stock_nplct_all_transaction
    {
        public string selected_date { get; set; }
        public string product_category_name { get; set; }
        public string product_code { get; set; }
        public string business_partner_code { get; set; }
        public string contractor_name { get; set; }
        public string sources { get; set; }
        public decimal? nplct_inventory { get; set; }
    }
}

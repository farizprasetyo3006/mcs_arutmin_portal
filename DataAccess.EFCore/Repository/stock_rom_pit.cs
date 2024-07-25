using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class stock_rom_pit
    {
        public string product_category_name { get; set; }
        public string item_name { get; set; }
        public string business_unit_name { get; set; }
        public decimal? qty { get; set; }
    }
}

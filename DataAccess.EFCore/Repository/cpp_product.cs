﻿using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class cpp_product
    {
        public string product_category_name { get; set; }
        public string item_name { get; set; }
        public string business_unit_name { get; set; }
        public decimal? qty { get; set; }
    }
}

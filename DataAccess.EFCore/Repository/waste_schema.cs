﻿using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class waste_schema
    {
        public string id { get; set; }
        public string details { get; set; }
        public string product_category_name { get; set; }
        public string product_name { get; set; }
        public string business_unit_name { get; set; }
        public string business_area_name { get; set; }
        public string pit { get; set; }
        public string business_partner_name { get; set; }
        public string pit_contractor { get; set; }
        public string destination { get; set; }
        public string waste_name { get; set; }
        public DateTime? dates { get; set; }
        public DateTime? date { get; set; }
        public decimal? qty { get; set; }
    }
}

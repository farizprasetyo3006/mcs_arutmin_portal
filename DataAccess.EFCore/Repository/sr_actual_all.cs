using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sr_actual_all
    {
        public string details { get; set; }
        public string business_unit_name { get; set; }
        public decimal? bit_mine { get; set; }
        public decimal? bit_waste { get; set; }
        public decimal? eco_mine { get; set; }
        public decimal? eco_waste { get; set; }
        public decimal? sarongga_mine { get; set; }
        public decimal? sarongga_waste { get; set; }
        public decimal? total_mine { get; set; }
        public decimal? total_waste { get; set; }
    }
}

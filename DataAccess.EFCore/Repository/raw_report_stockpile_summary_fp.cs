using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class raw_report_stockpile_summary_fp
    {
        public string business_unit_code { get; set; }
        public string product_name { get; set; }
        public decimal? rom_pit { get; set; }
        public decimal? rom_cpp { get; set; }
        public decimal? cpp_product { get; set; }
        public decimal? port_product { get; set; }
        public DateTime? dates { get; set; }
    }
}

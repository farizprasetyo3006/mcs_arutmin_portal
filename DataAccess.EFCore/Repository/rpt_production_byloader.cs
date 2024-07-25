using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_production_byloader
    {
        public string accounting_period_name { get; set; }
        public string equipment_id { get; set; }
        public string equipment_code { get; set; }
        public decimal? volume { get; set; }
        public decimal? rate { get; set; }
        public decimal? total { get; set; }
    }
}

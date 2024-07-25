using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class historical_dpr
    {
        public string id { get; set; }
        public DateTime tanggal { get; set; }
        public string entity { get; set; }
        public string business_unit_name { get; set; }
        public string area_name { get; set; }
        public string contractor_code { get; set; }
        public string pit_contractor { get; set; }
        public decimal? daily_actual { get; set; }
        public decimal? mtd_actual { get; set; }
        public decimal? ytd_actual { get; set; }
        public decimal? daily_budget { get; set; }
        public decimal? mtd_budget { get; set; }
        public decimal? mtd_forecast { get; set; }
        public decimal? ytd_budget { get; set; }
        public string process { get; set; }
        public DateTime? process_at { get; set; }
    }
}

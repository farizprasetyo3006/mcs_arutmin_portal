using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_hm_convertion
    {
        public string accounting_period_name { get; set; }
        public string advance_contract_number { get; set; }
        public string advance_contract_valuation_number { get; set; }
        public string charge_name { get; set; }
        public decimal? value { get; set; }
        public decimal? convertion_amount { get; set; }
        public decimal? exchangerate { get; set; }
        public decimal? grandtotal { get; set; }
        public decimal? ratio { get; set; }
        public string equipment_code { get; set; }
        public decimal? hours { get; set; }
        public decimal? hourly_rate { get; set; }
        public decimal? total { get; set; }
        public decimal? convertion_usd { get; set; }
        public decimal? convertion_idr { get; set; }
        public decimal? final_hour { get; set; }
    }
}

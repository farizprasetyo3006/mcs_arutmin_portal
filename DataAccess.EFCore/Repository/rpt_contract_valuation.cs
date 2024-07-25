using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_contract_valuation
    {
        public string accounting_period_name { get; set; }
        public string advance_contract_number { get; set; }
        public string advance_contract_valuation_number { get; set; }
        public string charge_name { get; set; }
        public decimal? value { get; set; }
        public decimal? convertion_amount { get; set; }
        public decimal? exchangerate { get; set; }
    }
}

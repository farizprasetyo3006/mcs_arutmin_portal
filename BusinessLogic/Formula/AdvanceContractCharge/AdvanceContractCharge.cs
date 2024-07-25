using System;

namespace BusinessLogic.Formula
{
    public class AdvanceContractChargeResult
    {
        public double value { get; set; }
        public double average_of_price_index { get; set; }
        public double result { get; set; }
        public double convertion_amount { get; set; }
        public string formula { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
    }
}


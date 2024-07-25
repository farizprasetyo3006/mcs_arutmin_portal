using System;

namespace BusinessLogic.Formula
{
    public class DespatchDemurrage
    {
        public double AllowedTime { get; set; }
        public double ActualTime { get; set; }
        public double Difference { get; set; }
        public bool IsDespatch { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
        public string StatementofFactNumber { get; set; }
    }
}


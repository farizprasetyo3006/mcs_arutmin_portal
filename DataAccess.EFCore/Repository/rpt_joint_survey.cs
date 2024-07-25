using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_joint_survey
    {
        public string id { get; set; }
        public string accounting_period_name { get; set; }
        public string note { get; set; }
        public decimal? quantity { get; set; }
        public string uom_symbol { get; set; }
        public decimal? distance { get; set; }
    }
}

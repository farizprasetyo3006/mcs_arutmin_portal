using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class price_analyst_invoice_arm
    {
        public string details { get; set; }
        public string customer_type_name { get; set; }
        public decimal? qty { get; set; }
        public decimal? cv_gar { get; set; }
        public decimal? fob_price { get; set; }
        public decimal? hpb_price { get; set; }
    }
}

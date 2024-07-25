using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_lookup_despatch_order_for_invoice
    {
        public string invoice_name { get; set; }
        public string invoice_master_id { get; set; }
        public string despatch_order_id { get; set; }
    }
}

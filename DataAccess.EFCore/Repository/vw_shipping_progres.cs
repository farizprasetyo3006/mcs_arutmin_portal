using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_shipping_progres
    {
        public string id { get; set; }
        public string customer_id { get; set; }
        public string shipment_plan_id { get; set; }
        public string despatch_order_id { get; set; }
        public string royalty_id { get; set; }
        public string shipping_instruction_id { get; set; }
        public string barging_transaction_id { get; set; }
        public string sales_invoice_id { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_production
    {
        public string id { get; set; }
        public string transaction_number { get; set; }
        public string process_flow_name { get; set; }
        public string accounting_period_name { get; set; }
        public DateTime? unloading_datetime { get; set; }
        public string unloading_datetime_view { get; set; }
        public string shift_name { get; set; }
        public string mine_location_code { get; set; }
        public string stockpile_location_code { get; set; }
        public string product_name { get; set; }
        public decimal? unloading_quantity { get; set; }
        public string uom_symbol { get; set; }
        public string vehicle_id { get; set; }
        public string equipment_code { get; set; }
    }
}

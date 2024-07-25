using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class raw_data_soa_export
    {
        public decimal? year { get; set; }
        public string month { get; set; }
        public string parcel { get; set; }
        public string vessel_name { get; set; }
        public string invoice_number { get; set; }
        public DateTime? bl_date { get; set; }
        public string loading_port { get; set; }
        public string country_of_destination { get; set; }
        public string sales_contract_number { get; set; }
        public string delivery_term { get; set; }
        public decimal? total_tons { get; set; }
        public decimal? hpb_price { get; set; }
        public decimal? price_adj_per_ton { get; set; }
        public decimal? sales_invoice { get; set; }
        public DateTime? est_date_received { get; set; }
        public string brand { get; set; }
        public decimal? gcv_arb { get; set; }
        public decimal? tm_arb { get; set; }
        public decimal? im_adb { get; set; }
        public decimal? ts_adb { get; set; }
        public decimal? ts_arb { get; set; }
        public decimal? ash_adb { get; set; }
        public decimal? ash_arb { get; set; }
    }
}

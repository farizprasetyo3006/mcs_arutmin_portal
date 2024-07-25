using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class raw_data_soa_pln
    {
        public decimal? year { get; set; }
        public string month { get; set; }
        public string monthly_sales { get; set; }
        public string customer { get; set; }
        public string parcel { get; set; }
        public string vessel { get; set; }
        public string royalty_number { get; set; }
        public string delivery_term { get; set; }
        public string moda_transport { get; set; }
        public string mkt_agent { get; set; }
        public string source { get; set; }
        public DateTime? bl_date { get; set; }
        public DateTime? arrival_date { get; set; }
        public DateTime? berthing_date { get; set; }
        public DateTime? discharge_date { get; set; }
        public decimal? loading_tonnage { get; set; }
        public decimal? discharge_tonnage { get; set; }
        public string invoice_number { get; set; }
        public decimal? freight { get; set; }
        public decimal? basic_fob { get; set; }
        public decimal? penyesuaian { get; set; }
        public decimal? denda_penolakan { get; set; }
        public decimal? cif_adj_price_per_ton { get; set; }
        public decimal? fob_adj_price_per_ton { get; set; }
        public decimal? sales_invoice_tanpa_denda_telat { get; set; }
        public decimal? denda_telat { get; set; }
        public decimal? sales_invoice { get; set; }
        public decimal? fob_value { get; set; }
        public string contract { get; set; }
        public DateTime? est_date_received { get; set; }
        public DateTime? payment_date { get; set; }
        public decimal? payment_received { get; set; }
        public string month_payment { get; set; }
    }
}

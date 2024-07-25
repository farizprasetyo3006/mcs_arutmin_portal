﻿using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_invoice_ell
    {
        public long? grup { get; set; }
        public string business_partner_code { get; set; }
        public string commercial_number { get; set; }
        public string sales_person { get; set; }
        public string position_id { get; set; }
        public string invoice_date { get; set; }
        public string due_date { get; set; }
        public string delivery_location { get; set; }
        public string full_period { get; set; }
        public string reference { get; set; }
        public string dunning_code { get; set; }
        public string invoice_class1 { get; set; }
        public string invoice_class2 { get; set; }
        public string invoice_class3 { get; set; }
        public string invoice_class4 { get; set; }
        public string tax_no { get; set; }
        public string invoice_description { get; set; }
        public string account_group_code { get; set; }
        public string item_description { get; set; }
        public string revenue_code { get; set; }
        public string vat { get; set; }
        public string price_code { get; set; }
        public string unit { get; set; }
        public decimal? invoice_quantity { get; set; }
        public decimal? unit_price { get; set; }
        public decimal? item_value { get; set; }
        public string account_code { get; set; }
        public string work_order { get; set; }
        public string project { get; set; }
        public string process_invoice { get; set; }
        public string upload_status { get; set; }
        public string organization_code { get; set; }
        public string organization_id { get; set; }
        public string sync_id { get; set; }
        public string sync_type { get; set; }
        public string sync_status { get; set; }
        public string response_code { get; set; }
        public string customer_name { get; set; }
        public string response_text { get; set; }
        public string id { get; set; }
        public bool? canceled { get; set; }
        public string approve_status { get; set; }
        public decimal? invoice_amount { get; set; }
        public string invoice_type_name { get; set; }
        public decimal? dp { get; set; }
        public decimal? tax { get; set; }
    }
}

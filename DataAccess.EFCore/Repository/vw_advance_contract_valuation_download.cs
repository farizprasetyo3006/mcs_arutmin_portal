using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_advance_contract_valuation_download
    {
        public string invoice_group { get; set; }
        public string customer_no { get; set; }
        public string sales_person { get; set; }
        public string position_id { get; set; }
        public string invoice_date { get; set; }
        public string due_date { get; set; }
        public string delivery_location { get; set; }
        public string full_period { get; set; }
        public string reference { get; set; }
        public string dunning_code { get; set; }
        public string invoice_class_1 { get; set; }
        public string invoice_class_2 { get; set; }
        public string invoice_class_3 { get; set; }
        public string invoice_class_4 { get; set; }
        public string tax_no { get; set; }
        public string invoice_description { get; set; }
        public string account_group_code { get; set; }
        public string item_description { get; set; }
        public string revenue_code { get; set; }
        public string vat { get; set; }
        public string price_code { get; set; }
        public string uom { get; set; }
        public string invoice_quantity { get; set; }
        public string unit_price { get; set; }
        public decimal? item_value { get; set; }
        public decimal? final_value { get; set; }
        public string account_code { get; set; }
        public string work_order { get; set; }
        public string project { get; set; }
        public string upload_status { get; set; }
        public string id { get; set; }
        public string organization_id { get; set; }
        public string advance_contract_valuation_id { get; set; }
    }
}

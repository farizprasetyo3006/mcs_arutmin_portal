using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_invoice_transhipment
    {
        public string id { get; set; }
        public string created_by { get; set; }
        public DateTime? created_on { get; set; }
        public string modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool? is_active { get; set; }
        public bool? is_locked { get; set; }
        public bool? is_default { get; set; }
        public string owner_id { get; set; }
        public string organization_id { get; set; }
        public string entity_id { get; set; }
        public string sales_invoice_id { get; set; }
        public string transaction_number { get; set; }
        public string transaction_number_shipping { get; set; }
        public string transaction_number_barging { get; set; }
        public DateTime? start_datetime_shipping { get; set; }
        public DateTime? end_datetime_shipping { get; set; }
        public DateTime? start_datetime_barging { get; set; }
        public DateTime? end_datetime_barging { get; set; }
        public string organization_name { get; set; }
        public string despatch_order_id { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

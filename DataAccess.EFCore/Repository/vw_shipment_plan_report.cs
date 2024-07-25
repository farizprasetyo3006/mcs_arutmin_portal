using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_shipment_plan_report
    {
        public string id { get; set; }
        public string sales_contract_id { get; set; }
        public string sales_contract_name { get; set; }
        public DateTime? sales_contract_start_date { get; set; }
        public DateTime? sales_contract_end_date { get; set; }
        public string sales_contract_term_id { get; set; }
        public string sales_contract_term_name { get; set; }
        public DateTime? sales_contract_term_start_date { get; set; }
        public DateTime? sales_contract_term_end_date { get; set; }
        public decimal? sales_contract_term_quantity { get; set; }
        public string buyer { get; set; }
        public string seller { get; set; }
        public string despatch_order_id { get; set; }
        public string despatch_order_number { get; set; }
        public decimal? despatch_order_quantity { get; set; }
        public DateTime? despatch_order_date { get; set; }
        public DateTime? planned_despatch_date { get; set; }
        public string shipment_plan_id { get; set; }
        public string shipment_plan_no { get; set; }
        public decimal? shipment_plan_amount_received { get; set; }
        public DateTime? created_on { get; set; }
        public DateTime? modified_on { get; set; }
        public string laycan_date { get; set; }
        public DateTime? laycan_start { get; set; }
        public DateTime? laycan_end { get; set; }
        public DateTime? eta_plan { get; set; }
        public DateTime? bill_of_lading_date { get; set; }
        public string vessel_id { get; set; }
        public string vessel { get; set; }
        public string despatch_plan_id { get; set; }
        public string despatch_plan_name { get; set; }
        public decimal? despatch_plan_quantity { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class draft_survey
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
        public string business_unit_id { get; set; }
        public string survey_number { get; set; }
        public DateTime? survey_date { get; set; }
        public string stock_location_id { get; set; }
        public string product_id { get; set; }
        public string sampling_template_id { get; set; }
        public decimal? quantity { get; set; }
        public string uom_id { get; set; }
        public string surveyor_id { get; set; }
        public string approved_by { get; set; }
        public DateTime? approved_on { get; set; }
        public string quality_sampling_id { get; set; }
        public string despatch_order_id { get; set; }
        public DateTime? bill_lading_date { get; set; }
        public string bill_lading_number { get; set; }
        public string transport_id { get; set; }
        public string customer_id { get; set; }
        public bool? non_commercial { get; set; }
        public DateTime? wlcr { get; set; }
        public string coa_id { get; set; }
        public DateTime? ds_issued_date { get; set; }
        public string sof_id { get; set; }
        public DateTime? sof_issued_date { get; set; }
        public string peb { get; set; }
        public string royalty { get; set; }
        public string draught_survey { get; set; }
        public string coo_goverment { get; set; }
        public string invoice { get; set; }
        public DateTime? draught_survey_issued { get; set; }
        public decimal? lc_price { get; set; }
        public decimal? lc_amount { get; set; }
        public string issuing_bank { get; set; }
        public string lc_number { get; set; }
        public DateTime? lc_date { get; set; }
        public string advising_bank { get; set; }
        public string peb_request_number { get; set; }
        public string peb_number { get; set; }
        public DateTime? peb_date { get; set; }
        public string pod_on_peb { get; set; }
        public string country_id { get; set; }
        public string customs_office { get; set; }
        public string cohc { get; set; }
        public string packing_list { get; set; }
        public DateTime? nor_tendered { get; set; }
        public DateTime? nor_accepted { get; set; }
        public string description { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_daywork
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
        public string daywork_number { get; set; }
        public DateTime? transaction_date { get; set; }
        public string customer_id { get; set; }
        public string equipment_id { get; set; }
        public string equipment_code { get; set; }
        public string accounting_period_id { get; set; }
        public string reference_number { get; set; }
        public decimal? hm_end { get; set; }
        public decimal? hm_start { get; set; }
        public decimal? hm_duration { get; set; }
        public string shift_id { get; set; }
        public string operator_id { get; set; }
        public string supervisor_id { get; set; }
        public string note { get; set; }
        public string daywork_type { get; set; }
        public string equipment_status { get; set; }
        public string job_description_id { get; set; }
        public string event_category_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public decimal? hm_rate { get; set; }
        public decimal? hm_value { get; set; }
    }
}

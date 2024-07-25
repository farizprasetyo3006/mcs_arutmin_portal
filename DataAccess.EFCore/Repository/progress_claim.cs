using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class progress_claim
    {
        public progress_claim()
        {
            joint_survey = new HashSet<joint_survey>();
        }

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
        public string advance_contract_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string progress_claim_name { get; set; }
        public string accounting_period_id { get; set; }
        public string reference_number { get; set; }
        public string note { get; set; }
        public decimal? target_quantity { get; set; }
        public decimal? actual_quantity { get; set; }
        public string target_uom_id { get; set; }
        public string approved_by { get; set; }
        public DateTime? approved_on { get; set; }
        public decimal? base_unit_price { get; set; }
        public string base_unit_price_currency_id { get; set; }
        public decimal? base_fuel_price { get; set; }
        public string base_fuel_price_currency_id { get; set; }
        public decimal? base_overdistance_price { get; set; }
        public string base_overdistance_price_currency_id { get; set; }
        public string actual_uom_id { get; set; }

        public virtual advance_contract advance_contract_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<joint_survey> joint_survey { get; set; }
    }
}

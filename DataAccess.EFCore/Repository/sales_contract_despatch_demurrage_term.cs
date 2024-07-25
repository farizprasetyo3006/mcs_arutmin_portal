using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_contract_despatch_demurrage_term
    {
        public sales_contract_despatch_demurrage_term()
        {
            sales_contract_despatch_demurrage_delay = new HashSet<sales_contract_despatch_demurrage_delay>();
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
        public string sales_contract_term_id { get; set; }
        public string despatch_demurrage_id { get; set; }
        public string location_id { get; set; }
        public decimal? loading_rate { get; set; }
        public string loading_rate_uom_id { get; set; }
        public decimal? turn_time { get; set; }
        public string turn_time_uom_id { get; set; }
        public decimal? despatch_percentage { get; set; }
        public decimal? rate { get; set; }
        public string currency_id { get; set; }
        public string sof_id { get; set; }
        public decimal? loading_rate_geared { get; set; }
        public decimal? loading_rate_gearless { get; set; }
        public decimal? turn_time_ending { get; set; }
        public string location2_id { get; set; }
        public string location3_id { get; set; }
        public decimal? loading_rate_nplct { get; set; }

        public virtual currency currency_ { get; set; }
        public virtual uom loading_rate_uom_ { get; set; }
        public virtual port_location location_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual sales_contract_term sales_contract_term_ { get; set; }
        public virtual sof sof_ { get; set; }
        public virtual uom turn_time_uom_ { get; set; }
        public virtual ICollection<sales_contract_despatch_demurrage_delay> sales_contract_despatch_demurrage_delay { get; set; }
    }
}

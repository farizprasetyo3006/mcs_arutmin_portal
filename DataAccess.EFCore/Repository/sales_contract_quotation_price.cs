using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_contract_quotation_price
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
        public string sales_contract_term_id { get; set; }
        public string quotation_type_id { get; set; }
        public string pricing_method_id { get; set; }
        public string frequency_id { get; set; }
        public decimal? price_value { get; set; }
        public decimal? decimal_places { get; set; }
        public string currency_id { get; set; }
        public string uom_id { get; set; }
        public decimal? weightening_value { get; set; }
        public string quotation_uom_id { get; set; }
        public string price_index_id { get; set; }
        public string quotation_period_freq_id { get; set; }
        public string index_calculation_id { get; set; }

        public virtual currency currency_ { get; set; }
        public virtual master_list frequency_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual price_index price_index_ { get; set; }
        public virtual master_list pricing_method_ { get; set; }
        public virtual master_list quotation_type_ { get; set; }
        public virtual uom quotation_uom_ { get; set; }
        public virtual sales_contract_term sales_contract_term_ { get; set; }
        public virtual uom uom_ { get; set; }
    }
}

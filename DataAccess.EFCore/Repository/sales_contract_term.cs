using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_contract_term
    {
        public sales_contract_term()
        {
            despatch_order = new HashSet<despatch_order>();
            sales_contract_charges = new HashSet<sales_contract_charges>();
            sales_contract_despatch_demurrage_term = new HashSet<sales_contract_despatch_demurrage_term>();
            sales_contract_despatch_plan = new HashSet<sales_contract_despatch_plan>();
            sales_contract_quotation_price = new HashSet<sales_contract_quotation_price>();
            sales_contract_taxes = new HashSet<sales_contract_taxes>();
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
        public string sales_contract_id { get; set; }
        public decimal? quantity { get; set; }
        public string notes { get; set; }
        public string uom_id { get; set; }
        public string charge_formula { get; set; }
        public string contract_term_name { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string delivery_term_id { get; set; }
        public string currency_id { get; set; }
        public string sales_charge_id { get; set; }
        public string quotation_period_freq_id { get; set; }
        public decimal? decimal_places { get; set; }
        public string rounding_type_id { get; set; }
        public string index_calculation_id { get; set; }
        public string index_calculation { get; set; }
        public string incoterm_id { get; set; }

        public virtual currency currency_ { get; set; }
        public virtual master_list delivery_term_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual sales_contract sales_contract_ { get; set; }
        public virtual uom uom_ { get; set; }
        public virtual ICollection<despatch_order> despatch_order { get; set; }
        public virtual ICollection<sales_contract_charges> sales_contract_charges { get; set; }
        public virtual ICollection<sales_contract_despatch_demurrage_term> sales_contract_despatch_demurrage_term { get; set; }
        public virtual ICollection<sales_contract_despatch_plan> sales_contract_despatch_plan { get; set; }
        public virtual ICollection<sales_contract_quotation_price> sales_contract_quotation_price { get; set; }
        public virtual ICollection<sales_contract_taxes> sales_contract_taxes { get; set; }
    }
}

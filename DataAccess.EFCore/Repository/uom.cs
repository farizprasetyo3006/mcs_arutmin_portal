using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class uom
    {
        public uom()
        {
            analyte = new HashSet<analyte>();
            drill_blast_plan = new HashSet<drill_blast_plan>();
            eight_week_forecast = new HashSet<eight_week_forecast>();
            explosive_usage_plan = new HashSet<explosive_usage_plan>();
            explosive_usage_plan_detail = new HashSet<explosive_usage_plan_detail>();
            price_adjustment_product_specification = new HashSet<price_adjustment_product_specification>();
            price_index_product_specification = new HashSet<price_index_product_specification>();
            product_specification = new HashSet<product_specification>();
            sales_contract_despatch_demurrage_termloading_rate_uom_ = new HashSet<sales_contract_despatch_demurrage_term>();
            sales_contract_despatch_demurrage_termturn_time_uom_ = new HashSet<sales_contract_despatch_demurrage_term>();
            sales_contract_despatch_plan = new HashSet<sales_contract_despatch_plan>();
            sales_contract_detail = new HashSet<sales_contract_detail>();
            sales_contract_product = new HashSet<sales_contract_product>();
            sales_contract_product_specifications = new HashSet<sales_contract_product_specifications>();
            sales_contract_quotation_pricequotation_uom_ = new HashSet<sales_contract_quotation_price>();
            sales_contract_quotation_priceuom_ = new HashSet<sales_contract_quotation_price>();
            sales_contract_term = new HashSet<sales_contract_term>();
            sales_price = new HashSet<sales_price>();
            sales_price_type = new HashSet<sales_price_type>();
            sampling_template_detail = new HashSet<sampling_template_detail>();
            stock_location = new HashSet<stock_location>();
            timesheet = new HashSet<timesheet>();
            timesheet_plan = new HashSet<timesheet_plan>();
            uom_conversionsource_uom_ = new HashSet<uom_conversion>();
            uom_conversiontarget_uom_ = new HashSet<uom_conversion>();
            waste_specification = new HashSet<waste_specification>();
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
        public string uom_category_id { get; set; }
        public string uom_name { get; set; }
        public string numerator_id { get; set; }
        public string denominator_id { get; set; }
        public string uom_symbol { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual uom_category uom_category_ { get; set; }
        public virtual ICollection<analyte> analyte { get; set; }
        public virtual ICollection<drill_blast_plan> drill_blast_plan { get; set; }
        public virtual ICollection<eight_week_forecast> eight_week_forecast { get; set; }
        public virtual ICollection<explosive_usage_plan> explosive_usage_plan { get; set; }
        public virtual ICollection<explosive_usage_plan_detail> explosive_usage_plan_detail { get; set; }
        public virtual ICollection<price_adjustment_product_specification> price_adjustment_product_specification { get; set; }
        public virtual ICollection<price_index_product_specification> price_index_product_specification { get; set; }
        public virtual ICollection<product_specification> product_specification { get; set; }
        public virtual ICollection<sales_contract_despatch_demurrage_term> sales_contract_despatch_demurrage_termloading_rate_uom_ { get; set; }
        public virtual ICollection<sales_contract_despatch_demurrage_term> sales_contract_despatch_demurrage_termturn_time_uom_ { get; set; }
        public virtual ICollection<sales_contract_despatch_plan> sales_contract_despatch_plan { get; set; }
        public virtual ICollection<sales_contract_detail> sales_contract_detail { get; set; }
        public virtual ICollection<sales_contract_product> sales_contract_product { get; set; }
        public virtual ICollection<sales_contract_product_specifications> sales_contract_product_specifications { get; set; }
        public virtual ICollection<sales_contract_quotation_price> sales_contract_quotation_pricequotation_uom_ { get; set; }
        public virtual ICollection<sales_contract_quotation_price> sales_contract_quotation_priceuom_ { get; set; }
        public virtual ICollection<sales_contract_term> sales_contract_term { get; set; }
        public virtual ICollection<sales_price> sales_price { get; set; }
        public virtual ICollection<sales_price_type> sales_price_type { get; set; }
        public virtual ICollection<sampling_template_detail> sampling_template_detail { get; set; }
        public virtual ICollection<stock_location> stock_location { get; set; }
        public virtual ICollection<timesheet> timesheet { get; set; }
        public virtual ICollection<timesheet_plan> timesheet_plan { get; set; }
        public virtual ICollection<uom_conversion> uom_conversionsource_uom_ { get; set; }
        public virtual ICollection<uom_conversion> uom_conversiontarget_uom_ { get; set; }
        public virtual ICollection<waste_specification> waste_specification { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class master_list
    {
        public master_list()
        {
            barging_plan = new HashSet<barging_plan>();
            despatch_order = new HashSet<despatch_order>();
            eight_week_forecast = new HashSet<eight_week_forecast>();
            eight_week_forecast_item = new HashSet<eight_week_forecast_item>();
            explosive_usage_plan = new HashSet<explosive_usage_plan>();
            explosive_usage_plan_detail = new HashSet<explosive_usage_plan_detail>();
            hauling_plan = new HashSet<hauling_plan>();
            hauling_plan_history = new HashSet<hauling_plan_history>();
            price_adjustment_product_specification = new HashSet<price_adjustment_product_specification>();
            price_index_product_specification = new HashSet<price_index_product_specification>();
            production_plan = new HashSet<production_plan>();
            sales_charge = new HashSet<sales_charge>();
            sales_contract_charges = new HashSet<sales_contract_charges>();
            sales_contract_despatch_plandelivery_term_ = new HashSet<sales_contract_despatch_plan>();
            sales_contract_despatch_planfulfilment_type_ = new HashSet<sales_contract_despatch_plan>();
            sales_contract_product_specifications = new HashSet<sales_contract_product_specifications>();
            sales_contract_quotation_pricefrequency_ = new HashSet<sales_contract_quotation_price>();
            sales_contract_quotation_pricepricing_method_ = new HashSet<sales_contract_quotation_price>();
            sales_contract_quotation_pricequotation_type_ = new HashSet<sales_contract_quotation_price>();
            sales_contract_term = new HashSet<sales_contract_term>();
            sales_contractcommitment_ = new HashSet<sales_contract>();
            sales_contractcontract_basis_ = new HashSet<sales_contract>();
            sales_contractcontract_status_ = new HashSet<sales_contract>();
            sales_invoice_charges = new HashSet<sales_invoice_charges>();
            shipping_instruction_detail_survey = new HashSet<shipping_instruction_detail_survey>();
            shipping_instruction_detail_survey_document = new HashSet<shipping_instruction_detail_survey_document>();
            shipping_instruction_document_agent = new HashSet<shipping_instruction_document_agent>();
            shipping_instruction_pekerjaan_agent = new HashSet<shipping_instruction_pekerjaan_agent>();
            shipping_instruction_to_company = new HashSet<shipping_instruction_to_company>();
            timesheet = new HashSet<timesheet>();
            timesheet_plan = new HashSet<timesheet_plan>();
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
        public string item_name { get; set; }
        public string notes { get; set; }
        public string item_group { get; set; }
        public string item_in_coding { get; set; }
        public decimal? sort_number { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<barging_plan> barging_plan { get; set; }
        public virtual ICollection<despatch_order> despatch_order { get; set; }
        public virtual ICollection<eight_week_forecast> eight_week_forecast { get; set; }
        public virtual ICollection<eight_week_forecast_item> eight_week_forecast_item { get; set; }
        public virtual ICollection<explosive_usage_plan> explosive_usage_plan { get; set; }
        public virtual ICollection<explosive_usage_plan_detail> explosive_usage_plan_detail { get; set; }
        public virtual ICollection<hauling_plan> hauling_plan { get; set; }
        public virtual ICollection<hauling_plan_history> hauling_plan_history { get; set; }
        public virtual ICollection<price_adjustment_product_specification> price_adjustment_product_specification { get; set; }
        public virtual ICollection<price_index_product_specification> price_index_product_specification { get; set; }
        public virtual ICollection<production_plan> production_plan { get; set; }
        public virtual ICollection<sales_charge> sales_charge { get; set; }
        public virtual ICollection<sales_contract_charges> sales_contract_charges { get; set; }
        public virtual ICollection<sales_contract_despatch_plan> sales_contract_despatch_plandelivery_term_ { get; set; }
        public virtual ICollection<sales_contract_despatch_plan> sales_contract_despatch_planfulfilment_type_ { get; set; }
        public virtual ICollection<sales_contract_product_specifications> sales_contract_product_specifications { get; set; }
        public virtual ICollection<sales_contract_quotation_price> sales_contract_quotation_pricefrequency_ { get; set; }
        public virtual ICollection<sales_contract_quotation_price> sales_contract_quotation_pricepricing_method_ { get; set; }
        public virtual ICollection<sales_contract_quotation_price> sales_contract_quotation_pricequotation_type_ { get; set; }
        public virtual ICollection<sales_contract_term> sales_contract_term { get; set; }
        public virtual ICollection<sales_contract> sales_contractcommitment_ { get; set; }
        public virtual ICollection<sales_contract> sales_contractcontract_basis_ { get; set; }
        public virtual ICollection<sales_contract> sales_contractcontract_status_ { get; set; }
        public virtual ICollection<sales_invoice_charges> sales_invoice_charges { get; set; }
        public virtual ICollection<shipping_instruction_detail_survey> shipping_instruction_detail_survey { get; set; }
        public virtual ICollection<shipping_instruction_detail_survey_document> shipping_instruction_detail_survey_document { get; set; }
        public virtual ICollection<shipping_instruction_document_agent> shipping_instruction_document_agent { get; set; }
        public virtual ICollection<shipping_instruction_pekerjaan_agent> shipping_instruction_pekerjaan_agent { get; set; }
        public virtual ICollection<shipping_instruction_to_company> shipping_instruction_to_company { get; set; }
        public virtual ICollection<timesheet> timesheet { get; set; }
        public virtual ICollection<timesheet_plan> timesheet_plan { get; set; }
    }
}

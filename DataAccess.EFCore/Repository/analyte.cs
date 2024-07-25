using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class analyte
    {
        public analyte()
        {
            joint_survey_analyte = new HashSet<joint_survey_analyte>();
            material_type_analyte = new HashSet<material_type_analyte>();
            price_adjustment_product_specification = new HashSet<price_adjustment_product_specification>();
            price_index_product_specification = new HashSet<price_index_product_specification>();
            product_specification = new HashSet<product_specification>();
            sales_contract_product_specifications = new HashSet<sales_contract_product_specifications>();
            sales_product_specification = new HashSet<sales_product_specification>();
            sampling_template_detail = new HashSet<sampling_template_detail>();
            stock_state_analyte = new HashSet<stock_state_analyte>();
            survey_analyte = new HashSet<survey_analyte>();
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
        public string analyte_name { get; set; }
        public string analyte_symbol { get; set; }
        public string uom_id { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual uom uom_ { get; set; }
        public virtual ICollection<joint_survey_analyte> joint_survey_analyte { get; set; }
        public virtual ICollection<material_type_analyte> material_type_analyte { get; set; }
        public virtual ICollection<price_adjustment_product_specification> price_adjustment_product_specification { get; set; }
        public virtual ICollection<price_index_product_specification> price_index_product_specification { get; set; }
        public virtual ICollection<product_specification> product_specification { get; set; }
        public virtual ICollection<sales_contract_product_specifications> sales_contract_product_specifications { get; set; }
        public virtual ICollection<sales_product_specification> sales_product_specification { get; set; }
        public virtual ICollection<sampling_template_detail> sampling_template_detail { get; set; }
        public virtual ICollection<stock_state_analyte> stock_state_analyte { get; set; }
        public virtual ICollection<survey_analyte> survey_analyte { get; set; }
        public virtual ICollection<waste_specification> waste_specification { get; set; }
    }
}

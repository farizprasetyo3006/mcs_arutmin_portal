using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class initial_information
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
        public string despatch_order_id { get; set; }
        public string status { get; set; }
        public string vessel_name { get; set; }
        public DateTime? eta_plan { get; set; }
        public string loading_port { get; set; }
        public string seller { get; set; }
        public string customer_name { get; set; }
        public string customer_address { get; set; }
        public string customer_additional_info { get; set; }
        public string contract_product_name { get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
        public string subject { get; set; }
        public string cc { get; set; }
        public string royalty_number { get; set; }

        public virtual despatch_order despatch_order_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

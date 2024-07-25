using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_plan_customer_list
    {
        public string id { get; set; }
        public string sales_contract_id { get; set; }
        public string sales_contract_name { get; set; }
        public string plan_name { get; set; }
        public string customer_id { get; set; }
        public string business_partner_code { get; set; }
        public string business_partner_name { get; set; }
        public int? month_id { get; set; }
        public string month_name { get; set; }
        public decimal? quantity { get; set; }
        public string dmo { get; set; }
        public string electricity { get; set; }
        public string owner_id { get; set; }
        public string organization_id { get; set; }
        public string entity_id { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

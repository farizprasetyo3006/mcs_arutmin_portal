﻿using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sales_contract_despatch_plan
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
        public string sales_contract_term_id { get; set; }
        public string contract_term_name { get; set; }
        public DateTime? despatch_date { get; set; }
        public decimal? quantity { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string fulfilment_type_id { get; set; }
        public string fulfilment_type_name { get; set; }
        public string delivery_term_id { get; set; }
        public string delivery_term_name { get; set; }
        public string notes { get; set; }
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

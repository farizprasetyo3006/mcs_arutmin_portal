using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_sampling_template_detail
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
        public string sampling_template_id { get; set; }
        public string sampling_template_name { get; set; }
        public string analyte_id { get; set; }
        public string analyte_name { get; set; }
        public string uom_id { get; set; }
        public string uom_name { get; set; }
        public string remark { get; set; }
        public int? order { get; set; }
        public string sampling_template_code { get; set; }
        public bool? is_stock_state { get; set; }
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_shipping_load_unload_document
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
        public string shipping_transaction_id { get; set; }
        public string transaction_number { get; set; }
        public string reference_number { get; set; }
        public string activity_id { get; set; }
        public string activity_name { get; set; }
        public string document_type_id { get; set; }
        public string document_type_name { get; set; }
        public string remark { get; set; }
        public bool? quantity { get; set; }
        public bool? quality { get; set; }
        public string filename { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

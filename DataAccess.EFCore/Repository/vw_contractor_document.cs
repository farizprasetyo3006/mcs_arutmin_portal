using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_contractor_document
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
        public string contractor_id { get; set; }
        public string contractor_name { get; set; }
        public string contractor_code { get; set; }
        public string document_type_id { get; set; }
        public string document_type_name { get; set; }
        public string document_type_code { get; set; }
        public string document_name { get; set; }
        public string document_number { get; set; }
        public string verified_by { get; set; }
        public string verified_by_name { get; set; }
        public DateTime? verified_on { get; set; }
        public DateTime? valid_from { get; set; }
        public DateTime? valid_to { get; set; }
        public string mime_type { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class contractor_document
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
        public string contractor_id { get; set; }
        public string document_type_id { get; set; }
        public string document_name { get; set; }
        public string document_number { get; set; }
        public string verified_by { get; set; }
        public DateTime? verified_on { get; set; }
        public DateTime? valid_from { get; set; }
        public DateTime? valid_to { get; set; }
        public string mime_type { get; set; }
        public string file_content { get; set; }
        public string file_name { get; set; }

        public virtual contractor contractor_ { get; set; }
        public virtual document_type document_type_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

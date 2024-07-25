using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class role_access
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
        public string application_role_id { get; set; }
        public string application_entity_id { get; set; }
        public long? access_create { get; set; }
        public long? access_read { get; set; }
        public long? access_update { get; set; }
        public long? access_delete { get; set; }
        public long? access_append { get; set; }

        public virtual application_entity application_entity_ { get; set; }
        public virtual organization organization_ { get; set; }
    }
}

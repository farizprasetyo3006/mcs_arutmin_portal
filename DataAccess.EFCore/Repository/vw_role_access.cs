using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_role_access
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
        public string application_role_id { get; set; }
        public string role_name { get; set; }
        public string application_entity_id { get; set; }
        public string entity_name { get; set; }
        public string display_name { get; set; }
        public long? access_create { get; set; }
        public long? access_read { get; set; }
        public long? access_update { get; set; }
        public long? access_delete { get; set; }
        public long? access_append { get; set; }
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

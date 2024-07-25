using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class team_member
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
        public string team_id { get; set; }
        public string application_user_id { get; set; }
        public bool? is_team_leader { get; set; }

        public virtual application_user application_user_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual team team_ { get; set; }
    }
}

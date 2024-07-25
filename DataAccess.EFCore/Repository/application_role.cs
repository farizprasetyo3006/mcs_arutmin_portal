using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class application_role
    {
        public application_role()
        {
            team_role = new HashSet<team_role>();
            user_role = new HashSet<user_role>();
        }

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
        public string role_name { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<team_role> team_role { get; set; }
        public virtual ICollection<user_role> user_role { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class team
    {
        public team()
        {
            application_user = new HashSet<application_user>();
            team_member = new HashSet<team_member>();
            team_role = new HashSet<team_role>();
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
        public string team_name { get; set; }
        public string team_code { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<application_user> application_user { get; set; }
        public virtual ICollection<team_member> team_member { get; set; }
        public virtual ICollection<team_role> team_role { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class app_user
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
        public string application_username { get; set; }
        public string application_password { get; set; }
        public string fullname { get; set; }
        public string primary_team_id { get; set; }
        public bool? is_sysadmin { get; set; }
        public string access_token { get; set; }
        public DateTime? token_expiry { get; set; }
        public string email { get; set; }
        public string manager_id { get; set; }
        public string api_key { get; set; }
        public DateTime? expired_date { get; set; }
        public bool? use_ldap { get; set; }
    }
}

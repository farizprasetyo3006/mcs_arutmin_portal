using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.DTO
{
    public partial class SetupOrganization
    {
        public string id { get; set; }
        public string organization_name { get; set; }
        public string organization_code { get; set; }
        public string parent_organization_id { get; set; }
        public string sysadmin_username { get; set; }
        public string sysadmin_password { get; set; }
    }
}

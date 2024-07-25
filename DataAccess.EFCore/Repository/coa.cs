using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class coa
    {
        public coa()
        {
            coa_subaccount = new HashSet<coa_subaccount>();
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
        public string account_code { get; set; }
        public string account_name { get; set; }
        public string parent_account_id { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<coa_subaccount> coa_subaccount { get; set; }
    }
}

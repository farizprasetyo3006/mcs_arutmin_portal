using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class bank
    {
        public bank()
        {
            bank_account = new HashSet<bank_account>();
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
        public string bank_name { get; set; }
        public string bank_code { get; set; }
        public string address { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<bank_account> bank_account { get; set; }
    }
}

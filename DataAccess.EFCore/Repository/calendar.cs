using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class calendar
    {
        public calendar()
        {
            business_days = new HashSet<business_days>();
            national_holiday = new HashSet<national_holiday>();
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
        public string calendar_name { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string days { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<business_days> business_days { get; set; }
        public virtual ICollection<national_holiday> national_holiday { get; set; }
    }
}

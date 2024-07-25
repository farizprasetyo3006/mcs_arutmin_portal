using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_email_notification
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
        public string email_subject { get; set; }
        public string email_content { get; set; }
        public DateTime? delivery_schedule { get; set; }
        public DateTime? delivered_on { get; set; }
        public int? attempt_count { get; set; }
        public string variable { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string recipients { get; set; }
        public string table_name { get; set; }
        public string fields { get; set; }
        public string criteria { get; set; }
        public string email_code { get; set; }
        public string attachment_file { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sync_log
    {
        public string id { get; set; }
        public string organization_id { get; set; }
        public string sync_id { get; set; }
        public DateTime date_time { get; set; }
        public string form_name { get; set; }
        public string sync_type { get; set; }
        public string sync_status { get; set; }
        public string response_text { get; set; }
    }
}

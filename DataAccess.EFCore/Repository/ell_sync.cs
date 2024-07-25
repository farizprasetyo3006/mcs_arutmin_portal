using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class ell_sync
    {
        public string id { get; set; }
        public string organization_id { get; set; }
        public string data { get; set; }
        public bool? new_sync_status { get; set; }
        public DateTime? new_sync_date { get; set; }
        public bool? update_sync_status { get; set; }
        public DateTime? update_sync_date { get; set; }
        public string module { get; set; }
        public string row_id { get; set; }
        public bool? delete_sync_status { get; set; }
        public DateTime? delete_sync_date { get; set; }
        public string data_sent { get; set; }
        public string error_msg { get; set; }
    }
}

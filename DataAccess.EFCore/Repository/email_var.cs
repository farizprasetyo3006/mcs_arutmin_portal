using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class email_var
    {
        public string id { get; set; }
        public string email_notification_id { get; set; }
        public string table_view_name { get; set; }
        public string field_name { get; set; }
        public string filter { get; set; }
        public string var_name { get; set; }
    }
}

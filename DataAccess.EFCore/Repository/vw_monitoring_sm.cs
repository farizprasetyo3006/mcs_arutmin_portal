using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_monitoring_sm
    {
        public string business_unit { get; set; }
        public string created_date { get; set; }
        public string modified_date { get; set; }
        public string transaction_date { get; set; }
        public string module_name { get; set; }
        public long? vall { get; set; }
    }
}

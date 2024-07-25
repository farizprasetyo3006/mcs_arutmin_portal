using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_monitoring_aims
    {
        public string business_unit_name { get; set; }
        public DateTime? created_date { get; set; }
        public DateTime? modified_date { get; set; }
        public DateTime? transaction_date { get; set; }
        public string module_name { get; set; }
    }
}

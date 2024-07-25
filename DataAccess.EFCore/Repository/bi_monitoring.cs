using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class bi_monitoring
    {
        public string tbl { get; set; }
        public DateTime? trans_date { get; set; }
        public string transaction_date { get; set; }
        public long? totalrec { get; set; }
        public decimal? sumrec { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_coal_mined_sp
    {
        public DateTime? date { get; set; }
        public string site { get; set; }
        public string product { get; set; }
        public decimal? quantity { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_coal_movement_sp
    {
        public string activity { get; set; }
        public DateTime? date { get; set; }
        public decimal? quantity { get; set; }
        public string product { get; set; }
        public string site { get; set; }
    }
}

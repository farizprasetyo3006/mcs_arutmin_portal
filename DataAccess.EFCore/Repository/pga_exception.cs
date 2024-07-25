using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class pga_exception
    {
        public int jexid { get; set; }
        public int jexscid { get; set; }
        public DateTime? jexdate { get; set; }
        public TimeSpan? jextime { get; set; }

        public virtual pga_schedule jexsc { get; set; }
    }
}

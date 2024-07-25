using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class pga_jobclass
    {
        public pga_jobclass()
        {
            pga_job = new HashSet<pga_job>();
        }

        public int jclid { get; set; }
        public string jclname { get; set; }

        public virtual ICollection<pga_job> pga_job { get; set; }
    }
}

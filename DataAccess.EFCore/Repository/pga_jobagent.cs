using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class pga_jobagent
    {
        public pga_jobagent()
        {
            pga_job = new HashSet<pga_job>();
        }

        public int jagpid { get; set; }
        public DateTime jaglogintime { get; set; }
        public string jagstation { get; set; }

        public virtual ICollection<pga_job> pga_job { get; set; }
    }
}

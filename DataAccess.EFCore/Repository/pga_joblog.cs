using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class pga_joblog
    {
        public pga_joblog()
        {
            pga_jobsteplog = new HashSet<pga_jobsteplog>();
        }

        public int jlgid { get; set; }
        public int jlgjobid { get; set; }
        public char jlgstatus { get; set; }
        public DateTime jlgstart { get; set; }
        public TimeSpan? jlgduration { get; set; }

        public virtual pga_job jlgjob { get; set; }
        public virtual ICollection<pga_jobsteplog> pga_jobsteplog { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class pga_jobstep
    {
        public pga_jobstep()
        {
            pga_jobsteplog = new HashSet<pga_jobsteplog>();
        }

        public int jstid { get; set; }
        public int jstjobid { get; set; }
        public string jstname { get; set; }
        public string jstdesc { get; set; }
        public bool? jstenabled { get; set; }
        public char jstkind { get; set; }
        public string jstcode { get; set; }
        public string jstconnstr { get; set; }
        public char jstonerror { get; set; }
        public DateTime? jscnextrun { get; set; }

        public virtual pga_job jstjob { get; set; }
        public virtual ICollection<pga_jobsteplog> pga_jobsteplog { get; set; }
    }
}

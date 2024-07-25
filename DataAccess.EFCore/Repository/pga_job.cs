using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class pga_job
    {
        public pga_job()
        {
            pga_joblog = new HashSet<pga_joblog>();
            pga_jobstep = new HashSet<pga_jobstep>();
            pga_schedule = new HashSet<pga_schedule>();
        }

        public int jobid { get; set; }
        public int jobjclid { get; set; }
        public string jobname { get; set; }
        public string jobdesc { get; set; }
        public string jobhostagent { get; set; }
        public bool? jobenabled { get; set; }
        public DateTime jobcreated { get; set; }
        public DateTime jobchanged { get; set; }
        public int? jobagentid { get; set; }
        public DateTime? jobnextrun { get; set; }
        public DateTime? joblastrun { get; set; }

        public virtual pga_jobagent jobagent { get; set; }
        public virtual pga_jobclass jobjcl { get; set; }
        public virtual ICollection<pga_joblog> pga_joblog { get; set; }
        public virtual ICollection<pga_jobstep> pga_jobstep { get; set; }
        public virtual ICollection<pga_schedule> pga_schedule { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class pga_jobsteplog
    {
        public int jslid { get; set; }
        public int jsljlgid { get; set; }
        public int jsljstid { get; set; }
        public char jslstatus { get; set; }
        public int? jslresult { get; set; }
        public DateTime jslstart { get; set; }
        public TimeSpan? jslduration { get; set; }
        public string jsloutput { get; set; }

        public virtual pga_joblog jsljlg { get; set; }
        public virtual pga_jobstep jsljst { get; set; }
    }
}

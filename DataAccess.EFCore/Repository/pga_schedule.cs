using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class pga_schedule
    {
        public pga_schedule()
        {
            pga_exception = new HashSet<pga_exception>();
        }

        public int jscid { get; set; }
        public int jscjobid { get; set; }
        public string jscname { get; set; }
        public string jscdesc { get; set; }
        public bool? jscenabled { get; set; }
        public DateTime jscstart { get; set; }
        public DateTime? jscend { get; set; }
        public bool[] jscminutes { get; set; }
        public bool[] jschours { get; set; }
        public bool[] jscweekdays { get; set; }
        public bool[] jscmonthdays { get; set; }
        public bool[] jscmonths { get; set; }

        public virtual pga_job jscjob { get; set; }
        public virtual ICollection<pga_exception> pga_exception { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class rpt_timesheet_paua_old
    {
        public string cn_unit_id { get; set; }
        public string cn_unit { get; set; }
        public string product { get; set; }
        public DateTime? timesheet_date { get; set; }
        public string timesheet_date_show { get; set; }
        public decimal? total { get; set; }
        public decimal? breakdown { get; set; }
        public decimal? delay { get; set; }
        public decimal? nonproduction { get; set; }
        public decimal? idle { get; set; }
        public decimal? production { get; set; }
        public decimal? totalhr { get; set; }
        public decimal? breakdownhr { get; set; }
        public decimal? delayhr { get; set; }
        public decimal? nonproductionhr { get; set; }
        public decimal? idlehr { get; set; }
        public decimal? productionhr { get; set; }
        public decimal? losstime { get; set; }
        public decimal? losstimehr { get; set; }
        public decimal? pa { get; set; }
        public decimal? ua { get; set; }
    }
}

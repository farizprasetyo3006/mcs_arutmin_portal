using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MCSWebApp.Models
{
    public class FastReportModel
    {
        public WebReport WebReport { get; set; }
        public string[] ReportsList { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}

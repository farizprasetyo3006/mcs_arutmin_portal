using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MCSWebApp.Models
{
    public class ReportViewerModel
    {
        public WebReport WebReport { get; set; }
        public string ReportName { get; set; }
        public List<ReportParameterModel> Parameters { get; set; }
    }
}

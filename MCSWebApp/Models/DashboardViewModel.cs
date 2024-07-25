using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MCSWebApp.Models
{
    public class DashboardViewModel
    {
        public decimal? quantity { get ; set; }
        public string module_name { get; set; }
        public decimal? quantity_waste_removal { get; set; }
        public decimal? quantity_processing { get; set; }
        public decimal? quantity_hauling { get; set; }
        public decimal? quantity_production { get; set; }
        /*      public string[] ReportsList { get; set; }
              public Dictionary<string, string> Parameters { get; set; }*/
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MCSWebApp.Models
{
    public class ReportParameterModel
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LookupValues { get; set; }
    }
}

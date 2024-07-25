using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Select2
{
    public partial class Select2Request
    {
        public string term { get; set; }
        public string _type { get; set; }
        public string q { get; set; }
        public int? page { get; set; }
        public Dictionary<string, string> keyValues { get; set; }

        public Select2Request()
        {
            keyValues = new Dictionary<string, string>();
        }
    }
}
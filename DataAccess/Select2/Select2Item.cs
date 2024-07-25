using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Select2
{
    public partial class Select2Item
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool selected { get; set; }
        public bool disabled { get; set; }
        public object data { get; set; }
    }
}
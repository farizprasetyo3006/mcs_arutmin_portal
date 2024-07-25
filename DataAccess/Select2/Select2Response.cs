using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Select2
{
    public partial class Select2Response
    {
        public List<Select2Item> results { get; set; }
        public Select2Pagination pagination { get; set; }
        public int? count { get; set; }
    }
}
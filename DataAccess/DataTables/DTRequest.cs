using System.Collections.Generic;

namespace DataAccess.DataTables
{
    public partial class DTRequest
    {
        public int? Draw { get; set; }
        public int? Start { get; set; }
        public int? Length { get; set; }
        public List<DTColumn> Columns { get; set; }
        public List<DTOrder> Orders { get; set; }
        public DTSearch Search { get; set; }
    }
}
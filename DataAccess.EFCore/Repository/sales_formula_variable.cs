using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class sales_formula_variable
    {
        public string id { get; set; }
        public string variable_name { get; set; }
        public string statement { get; set; }
        public string notes { get; set; }
    }
}

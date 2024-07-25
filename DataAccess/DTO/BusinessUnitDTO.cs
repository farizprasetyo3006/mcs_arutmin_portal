using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.DTO
{
    public partial class BusinessUnitDTO
    {
        public string id { get; set; }
        public string parent_business_unit_id { get; set; }
        public string parent_business_unit_name { get; set; }
        public string business_unit_name { get; set; }
        public string manager_id { get; set; }
        public string manager_name { get; set; }
    }
}

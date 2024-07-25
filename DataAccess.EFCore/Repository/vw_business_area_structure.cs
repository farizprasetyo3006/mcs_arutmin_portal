using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_business_area_structure
    {
        public string id { get; set; }
        public string organization_id { get; set; }
        public string business_unit_id { get; set; }
        public int? level { get; set; }
        public string business_area_code { get; set; }
        public string business_area_name { get; set; }
        public string parent_business_area_id { get; set; }
        public string id_path { get; set; }
        public string name_path { get; set; }
    }
}

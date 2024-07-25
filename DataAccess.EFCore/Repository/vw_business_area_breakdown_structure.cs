using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_business_area_breakdown_structure
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
        public string parent { get; set; }
        public string child_1 { get; set; }
        public string child_2 { get; set; }
        public string child_3 { get; set; }
        public string child_4 { get; set; }
        public string child_5 { get; set; }
        public string child_6 { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_location
    {
        public string id { get; set; }
        public string location_name { get; set; }
        public string code { get; set; }
        public string organization_id { get; set; }
        public string business_unit_id { get; set; }
        public string business_area_id { get; set; }
        public int? grup { get; set; }
    }
}

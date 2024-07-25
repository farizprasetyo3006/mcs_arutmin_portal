using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_waste_removal_item
    {
        public string id { get; set; }
        public string created_by { get; set; }
        public DateTime? created_on { get; set; }
        public string modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool? is_active { get; set; }
        public bool? is_locked { get; set; }
        public bool? is_default { get; set; }
        public string owner_id { get; set; }
        public string organization_id { get; set; }
        public string entity_id { get; set; }
        public string waste_removal_id { get; set; }
        public string truck_id { get; set; }
        public string vehicle_name { get; set; }
        public string truck_factor { get; set; }
        public string shift { get; set; }
        public decimal? ritase { get; set; }
        public decimal? jam07 { get; set; }
        public decimal? jam08 { get; set; }
        public decimal? jam09 { get; set; }
        public decimal? jam10 { get; set; }
        public decimal? jam11 { get; set; }
        public decimal? jam12 { get; set; }
        public decimal? jam13 { get; set; }
        public decimal? jam14 { get; set; }
        public decimal? jam15 { get; set; }
        public decimal? jam16 { get; set; }
        public decimal? jam17 { get; set; }
        public decimal? jam18 { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

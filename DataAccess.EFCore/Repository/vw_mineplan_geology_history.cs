using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_mineplan_geology_history
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
        public decimal? truethick { get; set; }
        public decimal? mass_tonnage { get; set; }
        public decimal? tm_ar { get; set; }
        public decimal? im_adb { get; set; }
        public decimal? ash_adb { get; set; }
        public decimal? vm_adb { get; set; }
        public decimal? fc_adb { get; set; }
        public decimal? ts_adb { get; set; }
        public decimal? cv_adb { get; set; }
        public decimal? cv_arb { get; set; }
        public decimal? rd { get; set; }
        public decimal? rdi { get; set; }
        public decimal? hgi { get; set; }
        public string resource_type_id { get; set; }
        public string coal_type_id { get; set; }
        public string model_data { get; set; }
        public string header_id { get; set; }
        public string organization_name { get; set; }
        public string business_unit_id { get; set; }
        public string business_unit_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
    }
}

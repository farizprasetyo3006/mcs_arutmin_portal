using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class mine_plan_ltp
    {
        public mine_plan_ltp()
        {
            mineplan_ltp_history = new HashSet<mineplan_ltp_history>();
        }

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
        public string business_unit_id { get; set; }
        public string name { get; set; }
        public string subname { get; set; }
        public string mine_code { get; set; }
        public string submine_code { get; set; }
        public string pit_code { get; set; }
        public string subpit_code { get; set; }
        public string contractor_code { get; set; }
        public string seam_code { get; set; }
        public string blok_code { get; set; }
        public string strip_code { get; set; }
        public string material_type_id { get; set; }
        public string reserve_type_id { get; set; }
        public decimal? int_truethick { get; set; }
        public decimal? waste_bcm { get; set; }
        public decimal? coal_tonnage { get; set; }
        public decimal? tm_ar { get; set; }
        public decimal? im_ar { get; set; }
        public decimal? ash_ar { get; set; }
        public decimal? vm_ar { get; set; }
        public decimal? fc_ar { get; set; }
        public decimal? ts_ar { get; set; }
        public decimal? gcv_adb_ar { get; set; }
        public decimal? gcv_ar_ar { get; set; }
        public decimal? rd_ar { get; set; }
        public decimal? rdi_ar { get; set; }
        public decimal? hgi_ar { get; set; }
        public string mine_location_reference_id { get; set; }
        public bool? approved { get; set; }
        public string approved_by { get; set; }
        public DateTime? model_date { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<mineplan_ltp_history> mineplan_ltp_history { get; set; }
    }
}

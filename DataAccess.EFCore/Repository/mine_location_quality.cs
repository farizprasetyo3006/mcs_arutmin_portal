﻿using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class mine_location_quality
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
        public string business_unit_id { get; set; }
        public string mine_location_id { get; set; }
        public DateTime? sampling_datetime { get; set; }
        public decimal? tm { get; set; }
        public decimal? ts { get; set; }
        public decimal? ash { get; set; }
        public decimal? im { get; set; }
        public decimal? vm { get; set; }
        public decimal? fc { get; set; }
        public decimal? gcv_ar { get; set; }
        public decimal? gcv_adb { get; set; }
        public decimal? rd { get; set; }
        public decimal? rdi { get; set; }
        public decimal? hgi { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

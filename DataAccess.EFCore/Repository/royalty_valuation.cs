﻿using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class royalty_valuation
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
        public string royalty_id { get; set; }
        public decimal? base_price_royalty { get; set; }
        public double? dhpb { get; set; }
        public double? bmn { get; set; }
        public double? royalty { get; set; }
        public double? pht { get; set; }
        public decimal? royalty_calc { get; set; }
        public decimal? royalty_awal { get; set; }
        public decimal? royalty_value { get; set; }
        public decimal? bmn_calc { get; set; }
        public decimal? bmn_awal { get; set; }
        public decimal? bmn_value { get; set; }
        public decimal? pht_calc { get; set; }
        public decimal? pht_awal { get; set; }
        public decimal? pht_value { get; set; }
        public decimal? dhpb_final_calc { get; set; }
        public decimal? dhpb_final_awal { get; set; }
        public decimal? dhpb_final_value { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

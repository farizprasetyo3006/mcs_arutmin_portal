﻿using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class vw_barge_rotation
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
        public string entity_id { get; set; }
        public string business_unit_id { get; set; }
        public string transaction_id { get; set; }
        public string voyage_number { get; set; }
        public string source_location { get; set; }
        public string destination_location { get; set; }
        public decimal? net_quantity { get; set; }
        public string transport_id { get; set; }
        public string product_id { get; set; }
        public DateTime? eta_loading_port { get; set; }
        public string quality_sampling_id { get; set; }
        public string eta_discharge_port { get; set; }
        public string remark { get; set; }
        public string organization_id { get; set; }
        public string organization_name { get; set; }
        public string record_created_by { get; set; }
        public string record_modified_by { get; set; }
        public string record_owning_user { get; set; }
        public string record_owning_team { get; set; }
        public string tug_id { get; set; }
        public string business_unit_name { get; set; }
        public string product_name { get; set; }
        public decimal? gcv_arb { get; set; }
        public decimal? ash_adb { get; set; }
        public decimal? ts_adb { get; set; }
        public decimal? quantity { get; set; }
        public string product_category_id { get; set; }
        public string business_partner_name { get; set; }
        public string contractor_id { get; set; }
    }
}

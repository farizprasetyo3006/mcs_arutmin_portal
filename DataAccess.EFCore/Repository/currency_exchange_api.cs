using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class currency_exchange_api
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
        public string id_curr { get; set; }
        public DateTime? id_date { get; set; }
        public decimal? f_middle_rate { get; set; }
        public decimal? f_sell_rate { get; set; }
        public decimal? f_buy_rate { get; set; }
        public string response_data { get; set; }
        public string remarks { get; set; }
        public bool? is_error { get; set; }
    }
}

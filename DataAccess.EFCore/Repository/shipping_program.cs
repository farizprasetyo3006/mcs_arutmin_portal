using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class shipping_program
    {
        public shipping_program()
        {
            shipping_program_detail = new HashSet<shipping_program_detail>();
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
        public string plan_year_id { get; set; }
        public int? month_id { get; set; }
        public string declared_month_id { get; set; }
        public string product_category_id { get; set; }
        public string commitment_id { get; set; }
        public string incoterm_id { get; set; }
        public string customer_id { get; set; }
        public string sales_contract_id { get; set; }
        public string tipe_penjualan_id { get; set; }
        public string source_coal_id { get; set; }
        public decimal? quantity { get; set; }
        public string shipping_program_number { get; set; }
        public decimal? product_1 { get; set; }
        public decimal? product_2 { get; set; }
        public decimal? product_3 { get; set; }
        public decimal? product_4 { get; set; }
        public decimal? product_5 { get; set; }
        public decimal? product_6 { get; set; }
        public decimal? product_7 { get; set; }
        public decimal? product_8 { get; set; }
        public decimal? product_9 { get; set; }
        public decimal? product_10 { get; set; }
        public decimal? product_11 { get; set; }
        public decimal? product_12 { get; set; }
        public decimal? product_13 { get; set; }
        public decimal? product_14 { get; set; }
        public decimal? product_15 { get; set; }
        public decimal? product_16 { get; set; }
        public decimal? product_17 { get; set; }
        public decimal? product_18 { get; set; }
        public decimal? product_19 { get; set; }
        public decimal? product_20 { get; set; }
        public decimal? product_21 { get; set; }
        public decimal? product_22 { get; set; }
        public decimal? product_23 { get; set; }
        public decimal? product_24 { get; set; }
        public decimal? product_25 { get; set; }
        public decimal? product_26 { get; set; }
        public decimal? product_27 { get; set; }
        public decimal? product_28 { get; set; }
        public decimal? product_29 { get; set; }
        public decimal? product_30 { get; set; }
        public decimal? product_31 { get; set; }
        public decimal? product_32 { get; set; }
        public decimal? product_33 { get; set; }
        public decimal? product_34 { get; set; }
        public decimal? product_35 { get; set; }
        public string end_user_id { get; set; }
        public string master_list_ids { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<shipping_program_detail> shipping_program_detail { get; set; }
    }
}

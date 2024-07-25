using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class shipment_plan
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
        public string sales_contract_id { get; set; }
        public int? month_id { get; set; }
        public string destination { get; set; }
        public string customer_id { get; set; }
        public string shipment_number { get; set; }
        public string incoterm { get; set; }
        public string transport_id { get; set; }
        public string laycan { get; set; }
        public DateTime? eta { get; set; }
        public decimal? qty_sp { get; set; }
        public string remarks { get; set; }
        public string traffic_officer_id { get; set; }
        public string sales_plan_customer_id { get; set; }
        public string shipment_year { get; set; }
        public decimal? loading_rate { get; set; }
        public decimal? despatch_demurrage_rate { get; set; }
        public DateTime? laycan_start { get; set; }
        public DateTime? laycan_end { get; set; }
        public string laycan_status { get; set; }
        public bool? is_geared { get; set; }
        public string eta_status { get; set; }
        public string invoice_no { get; set; }
        public string royalti { get; set; }
        public string contract_product_id { get; set; }
        public string shipping_agent { get; set; }
        public string surveyor { get; set; }
        public int? sp_month_id { get; set; }
        public DateTime? nora { get; set; }
        public DateTime? etb { get; set; }
        public DateTime? etc { get; set; }
        public string pln_schedule { get; set; }
        public string original_schedule { get; set; }
        public string loading_port { get; set; }
        public DateTime? eta_disc { get; set; }
        public DateTime? etb_disc { get; set; }
        public DateTime? etcommence_disc { get; set; }
        public DateTime? etcompleted_disc { get; set; }
        public decimal? stow_plan { get; set; }
        public decimal? loading_standart { get; set; }
        public string product_id { get; set; }
        public string shipping_program_id { get; set; }
        public string shipment_plan_number { get; set; }
        public string lineup_number { get; set; }
        public string declared_month_id { get; set; }
        public string vessel_id { get; set; }
        public bool? certain { get; set; }
        public string end_user { get; set; }
        public string fc_provider_id { get; set; }
        public string transport_provider_id { get; set; }
        public string loadport_agent { get; set; }
        public decimal? hpb_forecast { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

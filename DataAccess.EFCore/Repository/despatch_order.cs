using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class despatch_order
    {
        public despatch_order()
        {
            coal_transfer = new HashSet<coal_transfer>();
            despatch_demurrage = new HashSet<despatch_demurrage>();
            despatch_demurrage_delay = new HashSet<despatch_demurrage_delay>();
            despatch_demurrage_invoice = new HashSet<despatch_demurrage_invoice>();
            despatch_order_delay = new HashSet<despatch_order_delay>();
            hauling_transaction = new HashSet<hauling_transaction>();
            initial_information = new HashSet<initial_information>();
            processing_transaction = new HashSet<processing_transaction>();
            production_transaction = new HashSet<production_transaction>();
            sales_invoice = new HashSet<sales_invoice>();
            sales_invoice_transhipment = new HashSet<sales_invoice_transhipment>();
            shipping_instruction = new HashSet<shipping_instruction>();
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
        public string despatch_order_number { get; set; }
        public string sales_order_id { get; set; }
        public string product_id { get; set; }
        public decimal? quantity { get; set; }
        public string uom_id { get; set; }
        public DateTime? despatch_order_date { get; set; }
        public DateTime? planned_despatch_date { get; set; }
        public string contract_term_id { get; set; }
        public string customer_id { get; set; }
        public string seller_id { get; set; }
        public string ship_to { get; set; }
        public string contract_product_id { get; set; }
        public decimal? required_quantity { get; set; }
        public decimal? final_quantity { get; set; }
        public string fulfilment_type_id { get; set; }
        public string delivery_term_id { get; set; }
        public string notes { get; set; }
        public string despatch_plan_id { get; set; }
        public DateTime? bill_of_lading_date { get; set; }
        public string vessel_id { get; set; }
        public string laycan_date { get; set; }
        public string shipment_number { get; set; }
        public DateTime? order_reference_date { get; set; }
        public DateTime? eta_plan { get; set; }
        public DateTime? laycan_start { get; set; }
        public DateTime? laycan_end { get; set; }
        public bool? laycan_committed { get; set; }
        public bool? vessel_committed { get; set; }
        public bool? eta_committed { get; set; }
        public string loading_port { get; set; }
        public string discharge_port { get; set; }
        public decimal? quantity_actual { get; set; }
        public DateTime? bill_lading_date { get; set; }
        public string document_reference_id { get; set; }
        public string letter_of_credit { get; set; }
        public string port_location_id { get; set; }
        public string surveyor_id { get; set; }
        public string shipping_agent { get; set; }
        public decimal? laytime_duration { get; set; }
        public string laytime_text { get; set; }
        public decimal? loading_rate { get; set; }
        public decimal? despatch_demurrage_rate { get; set; }
        public bool? edit_loading_rate { get; set; }
        public bool? edit_despatch_rate { get; set; }
        public bool? multiple_barge { get; set; }
        public string parent_despatch_order_id { get; set; }
        public bool? set_parent { get; set; }
        public string wo_number { get; set; }
        public string customer_address { get; set; }
        public DateTime? shipment_plan_eta { get; set; }
        public string vehicle_make { get; set; }
        public string dwt_gt { get; set; }
        public string royalty_number { get; set; }
        public string invoice_number { get; set; }
        public decimal? turn_time { get; set; }
        public decimal? despatch_percentage { get; set; }
        public string loadport_agent { get; set; }

        public virtual sales_contract_product contract_product_ { get; set; }
        public virtual sales_contract_term contract_term_ { get; set; }
        public virtual customer customer_ { get; set; }
        public virtual master_list document_reference_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual organization seller_ { get; set; }
        public virtual ICollection<coal_transfer> coal_transfer { get; set; }
        public virtual ICollection<despatch_demurrage> despatch_demurrage { get; set; }
        public virtual ICollection<despatch_demurrage_delay> despatch_demurrage_delay { get; set; }
        public virtual ICollection<despatch_demurrage_invoice> despatch_demurrage_invoice { get; set; }
        public virtual ICollection<despatch_order_delay> despatch_order_delay { get; set; }
        public virtual ICollection<hauling_transaction> hauling_transaction { get; set; }
        public virtual ICollection<initial_information> initial_information { get; set; }
        public virtual ICollection<processing_transaction> processing_transaction { get; set; }
        public virtual ICollection<production_transaction> production_transaction { get; set; }
        public virtual ICollection<sales_invoice> sales_invoice { get; set; }
        public virtual ICollection<sales_invoice_transhipment> sales_invoice_transhipment { get; set; }
        public virtual ICollection<shipping_instruction> shipping_instruction { get; set; }
    }
}

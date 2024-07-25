using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class contractor
    {
        public contractor()
        {
            barging_transaction = new HashSet<barging_transaction>();
            contractor_document = new HashSet<contractor_document>();
            drill_blast_plan = new HashSet<drill_blast_plan>();
            shipping_instruction_to_company = new HashSet<shipping_instruction_to_company>();
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
        public string business_partner_name { get; set; }
        public string business_partner_code { get; set; }
        public bool? is_vendor { get; set; }
        public bool? is_customer { get; set; }
        public bool? is_company { get; set; }
        public bool? is_government { get; set; }
        public string primary_address { get; set; }
        public string primary_contact_name { get; set; }
        public string primary_contact_email { get; set; }
        public string primary_contact_phone { get; set; }
        public string tax_registration_number { get; set; }
        public bool? is_contractor { get; set; }
        public string bank_account_id { get; set; }
        public string currency_id { get; set; }
        public string contractor_type_id { get; set; }
        public string country_id { get; set; }
        public bool? is_taxable { get; set; }
        public bool? is_barge_owner { get; set; }
        public bool? is_tug_owner { get; set; }
        public bool? is_truck_owner { get; set; }
        public bool? is_vessel_owner { get; set; }
        public bool? is_train_owner { get; set; }
        public bool? is_equipment_owner { get; set; }
        public bool? is_surveyor { get; set; }
        public bool? is_other { get; set; }
        public decimal? barge_trip_plan { get; set; }

        public virtual organization organization_ { get; set; }
        public virtual ICollection<barging_transaction> barging_transaction { get; set; }
        public virtual ICollection<contractor_document> contractor_document { get; set; }
        public virtual ICollection<drill_blast_plan> drill_blast_plan { get; set; }
        public virtual ICollection<shipping_instruction_to_company> shipping_instruction_to_company { get; set; }
    }
}

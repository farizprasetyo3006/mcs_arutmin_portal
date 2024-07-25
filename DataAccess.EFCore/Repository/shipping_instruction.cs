using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class shipping_instruction
    {
        public shipping_instruction()
        {
            shipping_instruction_asuransi = new HashSet<shipping_instruction_asuransi>();
            shipping_instruction_attachment = new HashSet<shipping_instruction_attachment>();
            shipping_instruction_detail_survey = new HashSet<shipping_instruction_detail_survey>();
            shipping_instruction_detail_survey_document = new HashSet<shipping_instruction_detail_survey_document>();
            shipping_instruction_document_agent = new HashSet<shipping_instruction_document_agent>();
            shipping_instruction_pekerjaan_agent = new HashSet<shipping_instruction_pekerjaan_agent>();
            shipping_instruction_stevedoring = new HashSet<shipping_instruction_stevedoring>();
            shipping_instruction_to_company = new HashSet<shipping_instruction_to_company>();
            shipping_instruction_tug_boat = new HashSet<shipping_instruction_tug_boat>();
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
        public string shipping_instruction_number { get; set; }
        public DateTime? shipping_instruction_date { get; set; }
        public string despatch_order_id { get; set; }
        public string to_other { get; set; }
        public string notify_party { get; set; }
        public string cargo_description { get; set; }
        public string cc { get; set; }
        public string sampling_template_id { get; set; }
        public string approved_by_id { get; set; }
        public string marked { get; set; }
        public DateTime? issued_date { get; set; }
        public string placed { get; set; }
        public string shipping_instruction_created_by { get; set; }
        public string lampiran1 { get; set; }
        public string lampiran2 { get; set; }
        public string lampiran3 { get; set; }
        public string lampiran4 { get; set; }
        public string lampiran5 { get; set; }
        public DateTime? released_date { get; set; }
        public string si_number { get; set; }
        public string barge_id { get; set; }
        public string tug_id { get; set; }
        public string hs_code { get; set; }
        public string notify_party_address { get; set; }
        public string analyte_standard { get; set; }
        public string consignee { get; set; }
        public string imo { get; set; }
        public string dwt { get; set; }
        public string voy { get; set; }
        public string place_delivery { get; set; }
        public decimal? required_quantity { get; set; }
        public string vessel_id { get; set; }
        public string type { get; set; }
        public string flag { get; set; }
        public string royalty_number { get; set; }
        public string invoice_number { get; set; }
        public decimal? lc_price { get; set; }
        public decimal? lc_amount { get; set; }
        public string issuing_bank { get; set; }
        public string advising_bank { get; set; }
        public string lc_number { get; set; }
        public DateTime? lc_date { get; set; }
        public string applicant { get; set; }
        public string quality_contract { get; set; }
        public string country_of_discharge { get; set; }
        public decimal? royalty_amount { get; set; }
        public decimal? advance_royalty_amount { get; set; }
        public string buyer_address { get; set; }

        public virtual despatch_order despatch_order_ { get; set; }
        public virtual organization organization_ { get; set; }
        public virtual ICollection<shipping_instruction_asuransi> shipping_instruction_asuransi { get; set; }
        public virtual ICollection<shipping_instruction_attachment> shipping_instruction_attachment { get; set; }
        public virtual ICollection<shipping_instruction_detail_survey> shipping_instruction_detail_survey { get; set; }
        public virtual ICollection<shipping_instruction_detail_survey_document> shipping_instruction_detail_survey_document { get; set; }
        public virtual ICollection<shipping_instruction_document_agent> shipping_instruction_document_agent { get; set; }
        public virtual ICollection<shipping_instruction_pekerjaan_agent> shipping_instruction_pekerjaan_agent { get; set; }
        public virtual ICollection<shipping_instruction_stevedoring> shipping_instruction_stevedoring { get; set; }
        public virtual ICollection<shipping_instruction_to_company> shipping_instruction_to_company { get; set; }
        public virtual ICollection<shipping_instruction_tug_boat> shipping_instruction_tug_boat { get; set; }
    }
}

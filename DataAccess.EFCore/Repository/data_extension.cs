using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class data_extension
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
        public string application_entity_id { get; set; }
        public string record_id { get; set; }
        public string text0 { get; set; }
        public string text1 { get; set; }
        public string text2 { get; set; }
        public string text3 { get; set; }
        public string text4 { get; set; }
        public string text5 { get; set; }
        public string text6 { get; set; }
        public string text7 { get; set; }
        public string text8 { get; set; }
        public string text9 { get; set; }
        public decimal? number0 { get; set; }
        public decimal? number1 { get; set; }
        public decimal? number2 { get; set; }
        public decimal? number3 { get; set; }
        public decimal? number4 { get; set; }
        public decimal? number5 { get; set; }
        public decimal? number6 { get; set; }
        public decimal? number7 { get; set; }
        public decimal? number8 { get; set; }
        public decimal? number9 { get; set; }
        public DateTime? datetime0 { get; set; }
        public DateTime? datetime1 { get; set; }
        public DateTime? datetime2 { get; set; }
        public DateTime? datetime3 { get; set; }
        public DateTime? datetime4 { get; set; }
        public DateTime? datetime5 { get; set; }
        public DateTime? datetime6 { get; set; }
        public DateTime? datetime7 { get; set; }
        public DateTime? datetime8 { get; set; }
        public DateTime? datetime9 { get; set; }
        public bool? boolean0 { get; set; }
        public bool? boolean1 { get; set; }
        public bool? boolean2 { get; set; }
        public bool? boolean3 { get; set; }
        public bool? boolean4 { get; set; }
        public bool? boolean5 { get; set; }
        public bool? boolean6 { get; set; }
        public bool? boolean7 { get; set; }
        public bool? boolean8 { get; set; }
        public bool? boolean9 { get; set; }
        public string object0 { get; set; }
        public string object1 { get; set; }
        public string object2 { get; set; }
        public string object3 { get; set; }
        public string object4 { get; set; }
        public string object5 { get; set; }
        public string object6 { get; set; }
        public string object7 { get; set; }
        public string object8 { get; set; }
        public string object9 { get; set; }

        public virtual organization organization_ { get; set; }
    }
}

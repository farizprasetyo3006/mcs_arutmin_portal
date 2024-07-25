using System;
using System.Collections.Generic;

namespace DataAccess.EFCore.Repository
{
    public partial class ViewModel_equipment_usage_transaction : equipment_usage_transaction
    {
        public string[] event_definition_category_id { get; set; }
    }
}

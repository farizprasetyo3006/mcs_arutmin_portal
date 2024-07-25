using System;
using System.Collections.Generic;

namespace DataAccess.Interfaces
{
    public interface IEntity
    {
        string id { get; set; }
        string created_by { get; set; }
        DateTime? created_on { get; set; }
        string modified_by { get; set; }
        DateTime? modified_on { get; set; }
        bool? is_active { get; set; }
        bool? is_locked { get; set; }
        bool? is_default { get; set; }
        string owner_id { get; set; }
        string organization_id { get; set; }
        string entity_id { get; set; }

        string GetEntityName();

        string GetViewName();

        string GetDefaultView();

        Dictionary<string, string> GetDefaultViewColumns();

        Dictionary<string, string> GetDefaultViewOrders();
    }
}
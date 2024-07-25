using DataAccess.Interfaces;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DataAccess.Repository
{
    public partial class barging_transaction : IEntity
    {
        public static readonly string EntityId;

        public static readonly string EntityName;

        public static readonly string EntityDisplayName;

        public static readonly string ViewName;

        public static readonly string DefaultView;

        public static readonly Type DefaultViewType;

        public static readonly Dictionary<string, string> DefaultViewColumns;

        public static readonly Dictionary<string, string> DefaultViewOrders;

        static barging_transaction()
        {
            EntityName = GetTableName();
            EntityDisplayName = GetTableName(true);

            var ti = CultureInfo.InvariantCulture.TextInfo;

            DefaultViewColumns = new Dictionary<string, string>();
            DefaultViewType = typeof(vw_barging_transaction);

            ViewName = ((TableNameAttribute)Attribute.GetCustomAttributes(DefaultViewType)
                            .Where(o => o is TableNameAttribute).FirstOrDefault())?.Value;

            var columns = DefaultViewType.GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(ColumnAttribute)));
            foreach (var column in columns)
            {
                DefaultViewColumns.Add(column.Name, ti.ToTitleCase(column.Name.Replace("_", " ")));
            }

            DefaultViewOrders = new Dictionary<string, string>
            {
                { "organization_name", "ASC" },
                { "transaction_datetime", "DESC" }
            };

            DefaultView = " SELECT ";
            DefaultView += string.Join(", ", DefaultViewColumns.Keys.ToArray());
            DefaultView += $" FROM {ViewName} ";
        }

        public string GetEntityName()
        {
            return EntityName;
        }

        public string GetViewName()
        {
            return ViewName;
        }

        public string GetDefaultView()
        {
            return DefaultView;
        }

        public Dictionary<string, string> GetDefaultViewColumns()
        {
            return DefaultViewColumns;
        }

        public Dictionary<string, string> GetDefaultViewOrders()
        {
            return DefaultViewOrders;
        }
    }
}
using DataAccess.Interfaces;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DataAccess.Repository
{
    public partial class application_user : IEntity
    {
        public static readonly string EntityId;

        public static readonly string EntityName;

        public static readonly string EntityDisplayName;

        public static readonly string ViewName;

        public static readonly string DefaultView;

        public static readonly Type DefaultViewType;

        public static readonly Dictionary<string, string> DefaultViewColumns;

        public static readonly Dictionary<string, string> DefaultViewOrders;

        static application_user()
        {
            EntityName = GetTableName();
            EntityDisplayName = GetTableName(true);

            var ti = CultureInfo.InvariantCulture.TextInfo;

            DefaultViewColumns = new Dictionary<string, string>();
            DefaultViewType = typeof(vw_application_user);

            ViewName = ((TableNameAttribute)Attribute.GetCustomAttributes(DefaultViewType)
                            .Where(o => o is TableNameAttribute).FirstOrDefault())?.Value;

            var columns = DefaultViewType.GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(ColumnAttribute)));
            foreach (var column in columns)
            {
                if (column.Name == "application_password"
                    || column.Name == "access_token"
                    || column.Name == "token_expiry")
                    continue;

                DefaultViewColumns.Add(column.Name, ti.ToTitleCase(column.Name.Replace("_", " ")));
            }

            DefaultViewOrders = new Dictionary<string, string>
            {
                { "organization_name", "ASC" },
                { "fullname", "ASC" },
                { "application_username", "ASC" }
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
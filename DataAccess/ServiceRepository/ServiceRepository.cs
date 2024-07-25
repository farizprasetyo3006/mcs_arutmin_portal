using DataAccess.Interfaces;
using NLog;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace DataAccess
{
    public partial class ServiceRepository<TEntity, TEntityView> : IRepository<TEntity, TEntityView>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly DataContext context;
        protected readonly TEntity appEntity;

        private string GetTableName<T>(bool DisplayName = false)
        {
            string result = null;
            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(typeof(T));

            foreach (System.Attribute attr in attrs)
            {
                if (attr is TableNameAttribute)
                {
                    TableNameAttribute a = (TableNameAttribute)attr;
                    result = a.Value;

                    if (!string.IsNullOrEmpty(result) && DisplayName)
                    {
                        char[] delim = { '.' };
                        string[] strs = result.Split(delim);
                        if (strs != null)
                        {
                            if (strs.Length > 1) result = strs[1];
                            else result = strs[0];

                            TextInfo ti = new CultureInfo("en-US", false).TextInfo;
                            result = ti.ToTitleCase(result.Replace("_", " "));
                        }
                    }
                }
            }

            return result;
        }

        public ServiceRepository(DataContext dataContext)
        {
            context = dataContext;

            try
            {
                appEntity = Activator.CreateInstance<TEntity>();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        public virtual async Task<bool?> RemoveAsync(string id)
        {
            try
            {
                return await context.DeleteEntity<TEntity>(id, null);
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex);
            }

            return null;
        }

        public virtual async Task<bool?> RemoveAllAsync(List<string> ids)
        {
            using (var scope = context.Database.GetTransaction())
            {
                try
                {
                    var success = false;

                    foreach (var id in ids)
                    {
                        success = await context.DeleteEntity<TEntity>(id, null);
                        if (!success) break;
                    }

                    if (success)
                    {
                        scope.Complete();
                    }

                    return success;
                }
                catch (Exception ex)
                {
                    logger.Debug(context.Database.LastCommand);
                    logger.Error(ex);
                }
            }

            return null;
        }
    }
}
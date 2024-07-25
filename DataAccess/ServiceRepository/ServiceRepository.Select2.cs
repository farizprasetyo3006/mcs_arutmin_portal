using DataAccess.Select2;
using DataAccess.Interfaces;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public partial class ServiceRepository<TEntity, TEntityView> : IRepository<TEntity, TEntityView>
    {
        public virtual async Task<Select2Response> Select2(Select2Request select2Request, 
            string TextField, List<string> SearchFields = null, Dictionary<string, object> KeyFields = null)
        {
            var result = new Select2Response();
            TEntity appEntity = Activator.CreateInstance<TEntity>();
            TEntityView viewEntity = Activator.CreateInstance<TEntityView>();
            var defaultViewColumns = ((IEntity)appEntity).GetDefaultViewColumns();

            try
            {
                var sql = Sql.Builder.Append($" SELECT id AS select2_id_field ");
                sql.Append($", \"{TextField}\" AS select2_text_field");
                sql.Append($" FROM {appEntity.GetViewName()} ");
                sql.Append($" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);
                }                

                if (KeyFields?.Count > 0)
                {
                    foreach(var kf in KeyFields)
                    {
                        if(kf.Value == null)
                        {
                            sql.Append($" AND \"{kf.Key}\" IS NULL ");
                        }
                        else
                        {
                            sql.Append($" AND \"{kf.Key}\" = @0 ", kf.Value);
                        }                        
                    }                    
                }

                sql.Append(" AND ( 1=0 ");
                sql.Append($" OR \"{TextField}\" LIKE @0 ", $"%{select2Request.q}%");
                if (SearchFields?.Count > 0)
                {                    
                    foreach(var searchField in SearchFields)
                    {
                        sql.Append($" OR {searchField} LIKE @0 ", $"%{select2Request.q}%");
                    }
                }
                sql.Append(" ) ");

                var data = await context.Database.FetchAsync<dynamic>(sql);
                logger.Trace(context.Database.LastCommand);

                if (data?.Count > 0)
                {
                    result.results = new List<Select2Item>();

                    foreach (dynamic d in data)
                    {
                        if (d == null) continue;

                        result.results.Add(new Select2Item()
                        {
                            id = d.select2_id_field,
                            text = d.select2_text_field
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex.ToString());                
            }

            return result;
        }
    }
}

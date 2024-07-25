using DataAccess.Interfaces;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess
{
    public partial class ServiceRepository<TEntity, TEntityView> : IRepository<TEntity, TEntityView>
    {
        public virtual async Task<TEntityView> GetViewByIdAsync(string id)
        {
            TEntityView result = default;

            try
            {
                var sql = Sql.Builder.Append(((IEntity)appEntity).GetDefaultView());
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);
                    // Check user access
                    //sql.Append("AND ufn_can_read( @0, @1 ) = TRUE",
                    //  ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                sql.Append(" AND id = @0 ", id);

                result = await context.Database.FirstOrDefaultAsync<TEntityView>(sql);
                logger.Trace(context.Database.LastCommand);
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex.ToString());
            }

            return result;
        }

        public virtual async Task<TEntityView> GetViewFirstOrDefaultAsync(Sql sqlFilter)
        {
            TEntityView result = default;

            try
            {
                var sql = Sql.Builder.Append(((IEntity)appEntity).GetDefaultView());
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);
                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                if (!string.IsNullOrEmpty(sqlFilter?.SQL))
                {
                    sql.Append($" AND ( {sqlFilter?.SQL} ) ", sqlFilter?.Arguments);
                }

                result = await context.Database.FirstOrDefaultAsync<TEntityView>(sql);
                logger.Trace(context.Database.LastCommand);
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex.ToString());
            }

            return result;
        }

        public virtual async Task<TEntityView> GetViewFirstOrDefaultAsync(string sqlFilter, params object[] args)
        {
            TEntityView result = default;

            try
            {
                var sql = Sql.Builder.Append(((IEntity)appEntity).GetDefaultView());
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);
                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                if (!string.IsNullOrEmpty(sqlFilter))
                {
                    sql.Append($" AND ( {sqlFilter} ) ", args);
                }

                result = await context.Database.FirstOrDefaultAsync<TEntityView>(sql);
                logger.Trace(context.Database.LastCommand);
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex.ToString());
            }

            return result;
        }

        public virtual async Task<List<TEntityView>> GetViewAllAsync()
        {
            try
            {
                var sql = Sql.Builder.Append(((IEntity)appEntity).GetDefaultView());
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                var result = await context.Database.FetchAsync<TEntityView>(sql);
                logger.Trace(context.Database.LastCommand);

                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex);
            }

            return null;
        }

        public virtual async Task<List<TEntityView>> GetViewAllAsync(Sql sqlFilter)
        {
            try
            {
                var sql = Sql.Builder.Append(((IEntity)appEntity).GetDefaultView());
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                if (!string.IsNullOrEmpty(sqlFilter?.SQL))
                {
                    sql.Append($" AND ( {sqlFilter?.SQL} ) ", sqlFilter?.Arguments);
                }

                var result = await context.Database.FetchAsync<TEntityView>(sql);
                logger.Trace(context.Database.LastCommand);

                return result; 
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex);
            }

            return null;
        }

        public virtual async Task<List<TEntityView>> GetViewAllAsync(string sqlFilter, params object[] args)
        {
            try
            {
                var sql = Sql.Builder.Append(((IEntity)appEntity).GetDefaultView());
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                if (!string.IsNullOrEmpty(sqlFilter))
                {
                    sql.Append($" AND ( {sqlFilter} ) ", args);
                }

                var result = await context.Database.FetchAsync<TEntityView>(sql);
                logger.Trace(context.Database.LastCommand);

                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex);
            }

            return null;
        }

        public virtual async Task<Page<TEntityView>> GetViewAllAsync(long page, long itemsPerPage)
        {
            try
            {
                var sql = Sql.Builder.Append(((IEntity)appEntity).GetDefaultView());
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                var result = await context.Database.PageAsync<TEntityView>(page, itemsPerPage, sql);
                logger.Trace(context.Database.LastCommand);

                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex.ToString());
            }

            return null;
        }

        public virtual async Task<Page<TEntityView>> GetViewAllAsync(long page, long itemsPerPage, Sql sqlFilter)
        {
            try
            {
                var sql = Sql.Builder.Append(((IEntity)appEntity).GetDefaultView());
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                if (!string.IsNullOrEmpty(sqlFilter?.SQL))
                {
                    sql.Append($" AND ( {sqlFilter?.SQL} ) ", sqlFilter?.Arguments);
                }

                var result = await context.Database.PageAsync<TEntityView>(page, itemsPerPage, sql);
                logger.Trace(context.Database.LastCommand);

                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex.ToString());
            }

            return null;
        }

        public virtual async Task<Page<TEntityView>> GetViewAllAsync(long page, long itemsPerPage,
            string sqlFilter, params object[] args)
        {
            try
            {
                var sql = Sql.Builder.Append(((IEntity)appEntity).GetDefaultView());
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                if (!string.IsNullOrEmpty(sqlFilter))
                {
                    sql.Append($" AND ( {sqlFilter} ) ", args);
                }

                var result = await context.Database.PageAsync<TEntityView>(page, itemsPerPage, sql);
                logger.Trace(context.Database.LastCommand);

                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex.ToString());
            }

            return null;
        }

        public virtual async Task<long?> GetViewCountAsync()
        {
            try
            {
                var sql = Sql.Builder.Append(" SELECT COUNT(id) ");
                sql.Append($" FROM {GetTableName<TEntityView>(false)} ");
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                var result = await context.Database.ExecuteScalarAsync<long>(sql);
                logger.Trace(context.Database.LastCommand);

                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex);
            }

            return null;
        }

        public virtual async Task<long?> GetViewCountAsync(Sql sqlFilter)
        {
            try
            {
                var sql = Sql.Builder.Append(" SELECT COUNT(id) ");
                sql.Append($" FROM {GetTableName<TEntityView>(false)} ");
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                if (!string.IsNullOrEmpty(sqlFilter?.SQL))
                {
                    sql.Append($" AND ( {sqlFilter?.SQL} ) ", sqlFilter?.Arguments);
                }

                var result = await context.Database.ExecuteScalarAsync<long>(sql);
                logger.Trace(context.Database.LastCommand);

                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex);
            }

            return null;
        }

        public virtual async Task<long?> GetViewCountAsync(string sqlFilter, params object[] args)
        {
            try
            {
                var sql = Sql.Builder.Append(" SELECT COUNT(id) ");
                sql.Append($" FROM {GetTableName<TEntityView>(false)} ");
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                if (!string.IsNullOrEmpty(sqlFilter))
                {
                    sql.Append($"AND ( {sqlFilter} )", args);
                }

                var result = await context.Database.ExecuteScalarAsync<long>(sql);
                logger.Trace(context.Database.LastCommand);

                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex);
            }

            return null;
        }
    }
}
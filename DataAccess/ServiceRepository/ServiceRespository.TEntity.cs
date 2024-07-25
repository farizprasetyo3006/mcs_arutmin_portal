using DataAccess.Interfaces;
using NLog.Time;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace DataAccess
{
    public partial class ServiceRepository<TEntity, TEntityView> : IRepository<TEntity, TEntityView> where TEntity:IEntity
    {
        public virtual async Task<TEntity> GetByIdAsync(string id
            //, Action<bool, string> continueWith
            )
        {
            TEntity result = default;
            //bool success = false;
            //string message = null;

            try
            {
                var sql = Sql.Builder.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //    GetTableName<TEntity>(false), context.AppUserId);
                }

                sql.Append(" AND id = @0 ", id);

                result = await context.Database.FirstOrDefaultAsync<TEntity>(sql);
                logger.Trace(context.Database.LastCommand);
                //success = true;
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex.ToString());
                //message = ex.Message;
            }
            /*
            finally
            {
                continueWith?.Invoke(success, message);
            }
            */

            return result;
        }

        public virtual async Task<TEntity> GetFirstOrDefaultAsync(Sql sqlFilter)
        {
            TEntity result = default;

            try
            {
                var sql = Sql.Builder.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    /*
                    sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                        GetTableName<TEntity>(false), context.AppUserId);

                    if (!string.IsNullOrEmpty(sqlFilter?.SQL))
                    {
                        sql.Append($" AND ( {sqlFilter?.SQL} ) ", sqlFilter?.Arguments);
                    }
                    */
                }

                if (sqlFilter != null)
                {
                    sql.Append(" AND ( ");
                    sql.Append(sqlFilter);
                    sql.Append(" ) ");
                }

                result = await context.Database.FirstOrDefaultAsync<TEntity>(sql);
                logger.Trace(context.Database.LastCommand);
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex.ToString());
            }

            return result;
        }

        public virtual async Task<TEntity> GetFirstOrDefaultAsync(string sqlFilter, params object[] args)
        {
            TEntity result = default;

            try
            {
                var sql = Sql.Builder.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    /*
                    sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                        GetTableName<TEntity>(false), context.AppUserId);
                    */
                }

                if (!string.IsNullOrEmpty(sqlFilter))
                {
                    sql.Append($"AND ( {sqlFilter} )", args);
                }

                result = await context.Database.FirstOrDefaultAsync<TEntity>(sql);
                logger.Trace(context.Database.LastCommand);
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex.ToString());
            }

            return result;
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            try
            {
                var sql = Sql.Builder.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    /*
                    sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                        GetTableName<TEntity>(false), context.AppUserId);
                    */
                }

                var result = await context.Database.FetchAsync<TEntity>(sql);
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

        public virtual async Task<List<TEntity>> GetAllAsync(Sql sqlFilter)
        {
            try
            {
                var sql = Sql.Builder.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    /*
                    sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                        GetTableName<TEntity>(false), context.AppUserId);
                    */
                }

                if (!string.IsNullOrEmpty(sqlFilter?.SQL))
                {
                    sql.Append($" AND ( {sqlFilter.SQL} ) ", sqlFilter.Arguments);
                }

                var result = await context.Database.FetchAsync<TEntity>(sql);
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

        public virtual async Task<List<TEntity>> GetAllAsync(string sqlFilter, params object[] args)
        {
            try
            {
                var sql = Sql.Builder.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    /*
                    sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                        GetTableName<TEntity>(false), context.AppUserId);
                    */
                }

                if (!string.IsNullOrEmpty(sqlFilter))
                {
                    sql.Append($" AND ( {sqlFilter} ) ", args);
                }

                var result = await context.Database.FetchAsync<TEntity>(sql);
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

        public virtual async Task<Page<TEntity>> GetAllAsync(long page, long itemsPerPage)
        {
            try
            {
                var sql = Sql.Builder.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  GetTableName<TEntity>(false), context.AppUserId);
                }

                var result = await context.Database.PageAsync<TEntity>(page, itemsPerPage, sql);
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

        public virtual async Task<Page<TEntity>> GetAllAsync(long page, long itemsPerPage, Sql sqlFilter)
        {
            try
            {
                var sql = Sql.Builder.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  GetTableName<TEntity>(false), context.AppUserId);
                }

                if (!string.IsNullOrEmpty(sqlFilter?.SQL))
                {
                    sql.Append($" AND ( {sqlFilter?.SQL} ) ", sqlFilter?.Arguments);
                }

                var result = await context.Database.PageAsync<TEntity>(page, itemsPerPage, sql);
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

        public virtual async Task<Page<TEntity>> GetAllAsync(long page, long itemsPerPage,
            string sqlFilter, params object[] args)
        {
            try
            {
                var sql = Sql.Builder.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                    //  GetTableName<TEntity>(false), context.AppUserId);
                }

                if (!string.IsNullOrEmpty(sqlFilter))
                {
                    sql.Append($" AND ( {sqlFilter} ) ", args);
                }

                var result = await context.Database.PageAsync<TEntity>(page, itemsPerPage, sql);
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

        public virtual async Task<long?> GetCountAsync()
        {
            try
            {
                var sql = Sql.Builder.Append(" SELECT COUNT(id) ");
                sql.Append($" FROM {GetTableName<TEntity>(false)} ");
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

        public virtual async Task<long?> GetCountAsync(Sql sqlFilter)
        {
            try
            {
                var sql = Sql.Builder.Append(" SELECT COUNT(id) ");
                sql.Append($" FROM {GetTableName<TEntity>(false)} ");
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

        public virtual async Task<long?> GetCountAsync(string sqlFilter, params object[] args)
        {
            try
            {
                var sql = Sql.Builder.Append(" SELECT COUNT(id) ");
                sql.Append($" FROM {GetTableName<TEntity>(false)} ");
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

        /// <summary>
        ///     Save Entity with compare to existing data by id
        /// </summary>
        /// <param name="record">New record or modified record</param>
        /// <param name="continueWith">The value to assign to the parameter output (isNew,success)</param>
        public virtual async Task<TEntity> SaveWithMapEntity(TEntity record, Action<bool,bool> continueWith)
        {
            TEntity result = record;
            var success = false;
            var isNew = false;            

            try
            {
                result = context.MapToEntity(record, ref isNew);
                success = await context.SaveEntity<TEntity>(result);
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex);
            }
            finally
            {
                continueWith?.Invoke(isNew, success);
            }
            
            return result;
        }

        public virtual async Task<TEntity> SaveValues(string values, Action<bool, bool> continueWith)
        {
            TEntity result = default(TEntity);
            var success = false;
            var isNew = false;

            logger.Trace($"values = {values}");

            try
            {
                result = context.MapValues<TEntity>(values, ref isNew);
                success = await context.SaveEntity<TEntity>(result);
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex);
            }
            finally
            {
                continueWith?.Invoke(isNew, success);
            }

            return result;
        }
    }
}
using DataAccess.DataTables;
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
        public virtual async Task<DTResponse> GetDataTablesResponseAsync(DTRequest request, Sql additionalFilters = null)
        {
            TEntity appEntity = Activator.CreateInstance<TEntity>();
            TEntityView viewEntity = Activator.CreateInstance<TEntityView>();
            var defaultViewColumns = ((IEntity)appEntity).GetDefaultViewColumns();

            DTResponse result = new DTResponse()
            {
                Draw = request.Draw ?? 0,
                RecordsFiltered = 0,
                RecordsTotal = 0,
                Data = new List<object>()
            };

            try
            {
                #region Unfiltered count

                var sql = Sql.Builder.Append($" SELECT COUNT(*) ");
                sql.Append($" FROM {GetTableName<TEntityView>(false)} ");
                sql.Append(" WHERE COALESCE(is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND organization_id = @0 ", context.OrganizationId);

                    // Check user access
                    //sql.Append("AND ufn_can_read( @0, @1 ) = TRUE",
                    //    ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                }

                var unfilteredCount = await context.Database.ExecuteScalarAsync<long>(sql);
                logger.Trace(context.Database.LastCommand);

                result.RecordsTotal = (int)unfilteredCount;

                #endregion Unfiltered count

                if (unfilteredCount > 0)
                {
                    #region Filtered query

                    sql = Sql.Builder.Append(((IEntity)appEntity).GetDefaultView());
                    sql.Append($" WHERE COALESCE(is_active, FALSE) = TRUE ");
                    if (!context.IsSysAdmin)
                    {
                        sql.Append(" AND organization_id = @0 ", context.OrganizationId);
                        // Check user access
                        // Debug ufn_can_read
                        //sql.Append(" AND ufn_can_read( @0, @1 ) = TRUE ",
                        //    ((IEntity)appEntity).GetEntityName(), context.AppUserId);
                    }

                    if (additionalFilters != null)
                    {
                        sql.Append(additionalFilters);
                    }

                    #endregion Filtered query

                    #region Filters

                    // DataTables global search
                    if (!string.IsNullOrEmpty(request.Search?.SearchValue))
                    {
                        if (request.Search?.SearchRegex ?? false
                            && request.Columns != null)
                        {
                            foreach (var column in request.Columns)
                            {
                                if (!defaultViewColumns.ContainsKey(column?.Name)
                                    || !(column.Searchable ?? false))
                                    continue;

                                sql.Append($" AND {column.Name} LIKE @0 ",
                                    request.Search?.SearchValue);
                            }
                        }
                        else if (request.Columns != null) // Plain text search
                        {
                            foreach (var column in request.Columns)
                            {
                                if (!defaultViewColumns.ContainsKey(column?.Name)
                                    || !(column.Searchable ?? false))
                                    continue;

                                sql.Append($" AND {column.Name} LIKE @0 ",
                                    $"%{request.Search?.SearchValue}%");
                            }
                        }
                    }
                    else if(request.Columns != null) // DataTables column search
                    {
                        foreach (var column in request.Columns)
                        {
                            if (!defaultViewColumns.ContainsKey(column?.Name)
                                || !(column.Searchable ?? false))
                                continue;

                            if (column.SearchRegex ?? false)
                            {
                                sql.Append($" AND {column.Name} LIKE @0 ",
                                    request.Search?.SearchValue);
                            }
                            else // Plain text search
                            {
                                sql.Append($" AND {column.Name} LIKE @0 ",
                                    $"%{request.Search?.SearchValue}%");
                            }
                        }
                    }

                    #endregion Filters

                    #region Orders

                    var columnOrders = new List<string>();

                    if (request?.Orders?.Count > 0)
                    {
                        foreach (var order in request?.Orders.OrderBy(o => o.Index))
                        {
                            if (order.ColumnIndex < 1 || order.ColumnIndex >= request?.Columns.Count)
                                continue;

                            var column = request?.Columns[order.ColumnIndex];
                            if (column != null)
                            {
                                if (!defaultViewColumns.ContainsKey(column?.Name)
                                    || !(column.Orderable ?? false))
                                    continue;

                                var dir = order.Direction?.ToUpper() == "ASC" ?
                                    "ASC" : "DESC";
                                columnOrders.Add($" {column.Name} {dir} ");
                            }
                        }
                    }
                    else
                    {
                        foreach (var columnOrder in ((IEntity)appEntity).GetDefaultViewOrders())
                        {
                            columnOrders.Add($" {columnOrder.Key} {columnOrder.Value} ");
                        }
                    }

                    if (columnOrders.Count > 0)
                    {
                        sql.Append(" ORDER BY ");
                        sql.Append(string.Join(", ", columnOrders.ToArray()));
                    }

                    #endregion Orders

                    #region Page setting

                    var page = 1;
                    var length = 1000;
                    if (request.Start.HasValue && request.Length.HasValue)
                    {
                        page = (request.Start.Value / request.Length.Value) + 1;
                        length = request.Length.Value;
                    }

                    page = page < 1 ? 1 : page;
                    length = length < 1 ? 1 : length;
                    length = length > 1000 ? 1000 : length;

                    #endregion Page setting

                    var pagedData = await context.Database.PageAsync<TEntityView>(page, length, sql);
                    logger.Trace(context.Database.LastCommand);

                    if (pagedData != null && pagedData.Items != null)
                    {
                        result.RecordsFiltered = pagedData.Items.Count;

                        foreach (var item in pagedData.Items)
                        {
                            result.Data.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(context.Database.LastCommand);
                logger.Error(ex.ToString());
                result.Error = ex.Message;
            }

            return result;
        }
    }
}
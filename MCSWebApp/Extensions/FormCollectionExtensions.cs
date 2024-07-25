using DataAccess.DataTables;
using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Linq;

namespace MCSWebApp.Extensions
{
    public static class FormCollectionExtensions
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static DTRequest ToDataTablesRequest(this IFormCollection formCollection)
        {
            DTRequest result = null;

            try
            {
                result = new DTRequest();
                foreach (var key in formCollection.Keys)
                {
                    if (key.StartsWith("draw"))
                    {
                        result.Draw = int.Parse(formCollection[key]);
                    }
                    else if (key.StartsWith("start"))
                    {
                        // Start page
                        result.Start = int.Parse(formCollection[key]);
                    }
                    else if (key.StartsWith("length"))
                    {
                        // Row per page
                        result.Length = int.Parse(formCollection[key]);
                    }
                    else if (key.StartsWith("columns["))
                    {
                        #region DTColumn

                        DTColumn dtColumn = null;
                        var columnIndex = key.Replace("columns[", "");
                        columnIndex = columnIndex.Substring(0, columnIndex.IndexOf("]["));

                        if (int.TryParse(columnIndex, out int i))
                        {
                            dtColumn = result.Columns.FirstOrDefault(o => o.Index == i);
                            if (dtColumn == null)
                            {
                                dtColumn = new DTColumn()
                                {
                                    Index = i
                                };

                                result.Columns.Add(dtColumn);
                            }
                        }

                        if (dtColumn != null)
                        {
                            if (key.EndsWith("][name]"))
                            {
                                dtColumn.Name = formCollection[key];
                            }
                            else if (key.EndsWith("][data]"))
                            {
                                dtColumn.Data = formCollection[key];
                            }
                            else if (key.EndsWith("][searchable]"))
                            {
                                if (bool.TryParse(formCollection[key], out bool b))
                                {
                                    dtColumn.Searchable = b;
                                }
                            }
                            else if (key.EndsWith("][orderable]"))
                            {
                                if (bool.TryParse(formCollection[key], out bool b))
                                {
                                    dtColumn.Orderable = b;
                                }
                            }
                            else if (key.EndsWith("][search][value]"))
                            {
                                dtColumn.SearchValue = formCollection[key];
                            }
                            else if (key.EndsWith("][search][regex]"))
                            {
                                if (bool.TryParse(formCollection[key], out bool b))
                                {
                                    dtColumn.SearchRegex = b;
                                }
                            }
                        }

                        #endregion DTColumn
                    }
                    else if (key.StartsWith("order["))
                    {
                        #region DTOrder

                        DTOrder dtOrder = null;
                        var orderIndex = key.Replace("order[", "");
                        orderIndex = orderIndex.Substring(0, orderIndex.IndexOf("]["));

                        if (int.TryParse(orderIndex, out int i))
                        {
                            dtOrder = result.Orders.FirstOrDefault(o => o.Index == i);
                            if (dtOrder == null)
                            {
                                dtOrder = new DTOrder()
                                {
                                    Index = i
                                };

                                result.Orders.Add(dtOrder);
                            }
                        }

                        if (dtOrder != null)
                        {
                            if (key.EndsWith("][column]"))
                            {
                                if (int.TryParse(formCollection[key], out i))
                                {
                                    dtOrder.ColumnIndex = i;
                                }
                            }
                            else if (key.EndsWith("][dir]"))
                            {
                                dtOrder.Direction = formCollection[key];
                            }
                        }

                        #endregion DTOrder
                    }
                    else if (key.StartsWith("search["))
                    {
                        #region DTSearch

                        if (result.Search == null)
                        {
                            result.Search = new DTSearch();
                        }

                        if (key.EndsWith("value]"))
                        {
                            result.Search.SearchValue = formCollection[key];
                        }
                        else if (key.EndsWith("regex]"))
                        {
                            if (bool.TryParse(formCollection[key], out bool b))
                            {
                                result.Search.SearchRegex = b;
                            }
                        }

                        #endregion DTSearch
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return result;
        }
    }
}
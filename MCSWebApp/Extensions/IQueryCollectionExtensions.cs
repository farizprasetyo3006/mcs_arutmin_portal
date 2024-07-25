using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using DataAccess.Select2;

namespace MCSWebApp.Extensions
{
    public static class IQueryCollectionExtensions
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static Select2Request ToSelect2Request(this IQueryCollection qryCollection)
        {
            Select2Request result = new Select2Request();

            try
            {
                foreach (var key in qryCollection.Keys)
                {
                    if (key.ToLower() == "term")
                    {
                        result.term = qryCollection[key];
                    }
                    else if (key.ToLower() == "_type")
                    {
                        result._type = qryCollection[key];
                    }
                    else if (key.ToLower() == "q")
                    {
                        result._type = qryCollection[key];
                    }
                    else if (key.ToLower() == "page")
                    {
                        if (int.TryParse(qryCollection[key], out int page))
                        {
                            result.page = page;
                        }
                    }
                    else
                    {
                        result.keyValues.Add(key, qryCollection[key]);
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
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Utilities;
using DataAccess;
using DataAccess.Repository;
using DataAccess.Select2;
using NLog;
using Omu.ValueInjecter;
using PetaPoco;

namespace BusinessLogic.Entity
{
    public partial class Team: ServiceRepository<team, vw_team>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public Team(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public async Task<Select2Response> Select2PrimaryTeam(Select2Request select2Request,
                    string TextField, List<string> SearchFields = null, Dictionary<string, object> KeyFields = null)
        {
            var result = new Select2Response();
            var appEntity = Activator.CreateInstance<team>();
            //var viewEntity = Activator.CreateInstance<vw_team>();
            //var defaultViewColumns = appEntity.GetDefaultViewColumns();

            try
            {
                var sql = Sql.Builder.Append($" SELECT t1.id AS select2_id_field ");
                //sql.Append($", t1.\"{TextField}\" AS select2_text_field");
                sql.Append($", vbus.name_path AS select2_text_field");
                sql.Append($" FROM {appEntity.GetViewName()} t1 ");
                sql.Append($" INNER JOIN vw_business_unit_structure vbus ON vbus.default_team_id = t1.id ");
                sql.Append($" WHERE COALESCE(t1.is_active, FALSE) = TRUE ");
                if (!context.IsSysAdmin)
                {
                    sql.Append(" AND t1.organization_id = @0 ", context.OrganizationId);
                }

                if (KeyFields?.Count > 0)
                {
                    foreach (var kf in KeyFields)
                    {
                        if (kf.Value == null)
                        {
                            sql.Append($" AND t1.\"{kf.Key}\" IS NULL ");
                        }
                        else
                        {
                            sql.Append($" AND t1.\"{kf.Key}\" = @0 ", kf.Value);
                        }
                    }
                }

                sql.Append(" AND ( 1=0 ");
                sql.Append($" OR t1.\"{TextField}\" LIKE @0 ", $"%{select2Request.q}%");
                if (SearchFields?.Count > 0)
                {
                    foreach (var searchField in SearchFields)
                    {
                        sql.Append($" OR t1.{searchField} LIKE @0 ", $"%{select2Request.q}%");
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

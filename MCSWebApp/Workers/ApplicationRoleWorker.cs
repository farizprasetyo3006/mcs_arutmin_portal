using DataAccess.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using PetaPoco;
using PetaPoco.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Omu.ValueInjecter;

namespace MCSWebApp.Workers
{
    public class ApplicationRoleWorker : BackgroundService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        public ApplicationRoleWorker(IConfiguration configuration)
        {
            this.configuration = configuration;            
            connectionString = configuration.GetConnectionString("MCS");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.Info("ApplicationRoleWorker running at: {time}", DateTimeOffset.Now);

            var t = Task.Factory.StartNew(() => 
            {
                using (var db = DatabaseConfiguration
                    .Build()
                    .UsingConnectionString(connectionString)
                    .UsingProvider<PostgreSQLDatabaseProvider>()
                    .Create())
                {
                    try
                    {
                        using (var tx = db.GetTransaction())
                        {
                            logger.Info("ApplicationRoleWorker running at: {time}", DateTimeOffset.Now);

                            #region Get tables from information schema

                            var sql = Sql.Builder.Append("SELECT table_name, INITCAP(REPLACE(table_name, '_', ' ')) AS display_name");
                            sql.Append("FROM information_schema.tables");
                            sql.Append("WHERE table_schema = 'public' AND table_type = 'BASE TABLE'");
                            sql.Append("ORDER BY table_name;");

                            List<dynamic> _tables = db.Fetch<dynamic>(sql);

                            #endregion

                            sql = Sql.Builder.Append("SELECT * FROM organization WHERE is_active = TRUE");
                            var organizations = db.Fetch<organization>(sql);
                            logger.Debug(db.LastCommand);

                            if ((organizations?.Count ?? 0) > 0)
                            {
                                foreach (var org in organizations)
                                {
                                    if (stoppingToken.IsCancellationRequested) break;

                                    var e = new entity();
                                    e.InjectFrom(org);

                                    sql = Sql.Builder.Append("SELECT * FROM application_entity");
                                    sql.Append("WHERE organization_id = @0", org.id);
                                    var currentEntities = db.Fetch<application_entity>(sql);

                                    for (var i = _tables.Count - 1; i >= 0; i--)
                                    {
                                        if (currentEntities.Any(o => o.entity_name == (string)_tables[i].table_name))
                                        {
                                            _tables.RemoveAt(i);
                                        }
                                    }

                                    sql = Sql.Builder.Append("SELECT * FROM application_entity");
                                    sql.Append("WHERE organization_id = @0", org.id);
                                    sql.Append("AND entity_name = @0", "role_access");
                                    var aeRoleAccess = db.FirstOrDefault<application_entity>(sql);

                                    var appEntities = new List<application_entity>();
                                    //foreach (var c in Constants.ApplicationEntities)
                                    foreach (var c in _tables)
                                    {
                                        if (stoppingToken.IsCancellationRequested) break;

                                        sql = Sql.Builder.Append("SELECT * FROM application_entity");
                                        sql.Append("WHERE organization_id = @0", org.id);
                                        sql.Append("AND entity_name = @0", (string)c.table_name);
                                        var ae = db.FirstOrDefault<application_entity>(sql);
                                        logger.Debug(db.LastCommand);

                                        if (ae == null)
                                        {
                                            ae = new application_entity();
                                            ae.InjectFrom(e);

                                            ae.id = Guid.NewGuid().ToString("N");
                                            ae.created_on = DateTime.Now;
                                            ae.modified_on = null;
                                            ae.modified_by = null;

                                            ae.entity_id = ae.id;
                                            ae.entity_name = (string)c.table_name;
                                            ae.display_name = (string)c.display_name;
                                            db.Insert(ae);
                                            logger.Debug(db.LastCommand);
                                        }

                                        appEntities.Add(ae);
                                    }

                                    sql = Sql.Builder.Append("SELECT * FROM application_role");
                                    sql.Append("WHERE organization_id = @0", org.id);
                                    var appRoles = db.Fetch<application_role>(sql);
                                    if ((appRoles?.Count ?? 0) > 0)
                                    {
                                        foreach (var appRole in appRoles)
                                        {
                                            if (stoppingToken.IsCancellationRequested) break;

                                            foreach (var appEntity in appEntities)
                                            {
                                                if (stoppingToken.IsCancellationRequested) break;

                                                sql = Sql.Builder.Append("SELECT * FROM role_access");
                                                sql.Append("WHERE application_role_id = @0", appRole.id);
                                                sql.Append("AND application_entity_id = @0", appEntity.id);

                                                var roleAccess = db.FirstOrDefault<role_access>(sql);
                                                if (roleAccess == null)
                                                {
                                                    roleAccess = new role_access();
                                                    roleAccess.InjectFrom(e);

                                                    roleAccess.id = Guid.NewGuid().ToString("N");
                                                    roleAccess.created_on = DateTime.Now;
                                                    roleAccess.modified_on = null;
                                                    roleAccess.modified_by = null;
                                                    roleAccess.entity_id = aeRoleAccess?.id;
                                                    roleAccess.application_role_id = appRole.id;
                                                    roleAccess.application_entity_id = appEntity.id;
                                                    roleAccess.access_create = 0;
                                                    roleAccess.access_read = 0;
                                                    roleAccess.access_update = 0;
                                                    roleAccess.access_delete = 0;
                                                    roleAccess.access_append = 0;

                                                    db.Insert(roleAccess);
                                                    logger.Debug(db.LastCommand);
                                                }
                                            }
                                        }
                                    }
                                }

                                tx.Complete();
                            }
                        }
                    }
                    catch (TaskCanceledException tce)
                    {
                        logger.Error(tce.Message);
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(db.LastCommand);
                        logger.Error(ex.ToString());
                    }
                }
            });

            await t;
        }

    }
}

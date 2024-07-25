using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Utilities;
using DataAccess;
using DataAccess.Repository;
using NLog;
using Omu.ValueInjecter;
using PetaPoco;
using Common;

namespace BusinessLogic.Entity
{
    public partial class Organization : ServiceRepository<organization, vw_organization>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public Organization(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public async Task<StandardResult> SetupOrganization(string OrganizationName, string OrganizationCode, 
            string ParentOrganizationId, string AdministratorUsername, string AdministratorPassword)
        {
            StandardResult result = new StandardResult();

            try
            {
                var db = context.Database;
                var sql = Sql.Builder.Append("SELECT ufn_setup_organization(@0, @1, @2, @3, @4, @5)",
                    OrganizationName, OrganizationCode, ParentOrganizationId, AdministratorUsername,
                    StringHash.CreateHash(AdministratorPassword),
                    db.ConnectionString);
                var _result = await db.ExecuteScalarAsync<bool>(sql);
                logger.Trace(db.LastCommand);

                if (_result)
                {
                    using (var tx = db.GetTransaction())
                    {
                        try
                        {
                            sql = Sql.Builder.Append("SELECT * FROM organization WHERE organization_name = @0", OrganizationName);
                            var org = await db.FirstOrDefaultAsync<organization>(sql);
                            if (org != null)
                            {
                                var e = new entity();
                                e.InjectFrom(org);

                                sql = Sql.Builder.Append("SELECT * FROM application_entity");
                                sql.Append("WHERE organization_id = @0", org.id);
                                sql.Append("AND entity_name = @0", "role_access");
                                var aeRoleAccess = await db.FirstOrDefaultAsync<application_entity>(sql);

                                var appEntities = new List<application_entity>();
                                foreach (var c in Constants.ApplicationEntities)
                                {
                                    sql = Sql.Builder.Append("SELECT * FROM application_entity");
                                    sql.Append("WHERE organization_id = @0", org.id);
                                    sql.Append("AND entity_name = @0", c.Key);
                                    var ae = await db.FirstOrDefaultAsync<application_entity>(sql);
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
                                        ae.entity_name = c.Key;
                                        ae.display_name = c.Value;
                                        await db.InsertAsync(ae);
                                        logger.Debug(db.LastCommand);
                                    }

                                    appEntities.Add(ae);
                                }

                                sql = Sql.Builder.Append("SELECT * FROM application_role");
                                sql.Append("WHERE organization_id = @0", org.id);
                                var appRoles = await db.FetchAsync<application_role>(sql);
                                if ((appRoles?.Count ?? 0) > 0)
                                {
                                    foreach (var appRole in appRoles)
                                    {
                                        foreach (var appEntity in appEntities)
                                        {
                                            sql = Sql.Builder.Append("SELECT * FROM role_access");
                                            sql.Append("WHERE application_role_id = @0", appRole.id);
                                            sql.Append("AND application_entity_id = @0", appEntity.id);

                                            var roleAccess = await db.FirstOrDefaultAsync<role_access>(sql);
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

                                                await db.InsertAsync(roleAccess);
                                                logger.Debug(db.LastCommand);
                                            }
                                        }
                                    }
                                }
                            }

                            tx.Complete();
                            result.Success = _result;
                        }
                        catch (Exception ex)
                        {
                            logger.Debug(db.LastCommand);
                            logger.Error(ex.ToString());
                            result.Message = ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return result;
        }
    }
}

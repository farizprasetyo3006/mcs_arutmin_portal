using DataAccess.Interfaces;
using DataAccess.Repository;
using PetaPoco;
using System;
using System.Threading.Tasks;

namespace DataAccess
{
    public partial class DataContext
    {
        public async Task<bool> SaveEntity<T>(T record)
        {
            bool result = false;

            try
            {
                //var appEntity = Activator.CreateInstance<T>();
                if (!typeof(IEntity).IsAssignableFrom(typeof(T)))
                {
                    throw new Exception("Record is not IEntity");
                }

                var isNew = false;
                var sql = Sql.Builder.Append(
                    " WHERE is_active = TRUE AND organization_id = @0 AND id = @1 ",
                    OrganizationId, ((IEntity)record).id);
                var oldRecord = await Database.FirstOrDefaultAsync<T>(sql);
                logger.Trace(Database.LastCommand);

                isNew = oldRecord == null;

                if (isNew)
                {
                    #region Insert

                    logger.Debug("Insert record ...");

                    #region Set common values

                    if (string.IsNullOrEmpty(((IEntity)record).id)
                        || string.IsNullOrWhiteSpace(((IEntity)record).id))
                    {
                        ((IEntity)record).id = Guid.NewGuid().ToString("N").ToLower();
                    }

                    ((IEntity)record).created_by = AppUserId;
                    ((IEntity)record).created_on = DateTime.Now;
                    ((IEntity)record).modified_by = null;
                    ((IEntity)record).modified_on = null;
                    ((IEntity)record).is_active = true;
                    ((IEntity)record).is_locked = null;
                    ((IEntity)record).is_default = null;
                    ((IEntity)record).owner_id = AppUserId;
                    ((IEntity)record).organization_id = OrganizationId;

                    #endregion Set common values

                    #region Check role access

                    if (!IsSysAdmin)
                    {
                        sql = Sql.Builder.Append(" SELECT ufn_can_create( @0, @1 ) ",
                            ((IEntity)record).GetEntityName(), AppUserId);
                        var hasAccess = await Database.ExecuteScalarAsync<bool>(sql);
                        logger.Trace(Database.LastCommand);

                        if (!hasAccess)
                        {
                            throw new Exception("User does not have permission to create this entity");
                        }
                    }

                    #endregion Check role access

                    #region Special rules

                    if (((IEntity)record).GetEntityName() == application_entity.EntityName)
                    {
                        throw new Exception("Application Entity can not be added using this method");
                    }

                    if (((IEntity)record).GetEntityName() == organization.EntityName)
                    {
                        throw new Exception("Organization can not be added using this method");
                    }

                    #endregion Special rules

                    #region Insert record

                    using (var scope = Database.GetTransaction())
                    {
                        try
                        {
                            await Database.InsertAsync(record);
                            logger.Trace(Database.LastCommand);

                            scope.Complete();
                            result = true;
                        }
                        catch (Exception ex)
                        {
                            logger.Debug(Database.LastCommand);
                            logger.Error(ex.ToString());
                        }
                    }

                    logger.Debug($"Insert record = {result}");

                    #endregion Insert record

                    #endregion Insert
                }
                else
                {
                    #region Update

                    logger.Debug("Update record ...");

                    #region Set common values

                    if (string.IsNullOrEmpty(((IEntity)record).id)
                        || string.IsNullOrWhiteSpace(((IEntity)record).id))
                    {
                        throw new Exception("Record id is empty");
                    }

                    ((IEntity)record).created_by = ((IEntity)oldRecord).created_by;
                    ((IEntity)record).created_on = ((IEntity)oldRecord).created_on;
                    ((IEntity)record).modified_by = AppUserId;
                    ((IEntity)record).modified_on = DateTime.Now;
                    ((IEntity)record).is_active = ((IEntity)oldRecord).is_active;
                    ((IEntity)record).is_locked = ((IEntity)oldRecord).is_locked;
                    ((IEntity)record).is_default = ((IEntity)oldRecord).is_default;
                    ((IEntity)record).owner_id = ((IEntity)oldRecord).owner_id;
                    ((IEntity)record).organization_id = ((IEntity)oldRecord).organization_id;
                    ((IEntity)record).entity_id = ((IEntity)oldRecord).entity_id;

                    #endregion Set common values

                    #region Check role access

                    if (!IsSysAdmin)
                    {
                        sql = Sql.Builder.Append(" SELECT ufn_can_update( @0, @1 ) ",
                            ((IEntity)record).id, AppUserId);
                        var hasAccess = await Database.ExecuteScalarAsync<bool>(sql);
                        logger.Trace(Database.LastCommand);

                        if (!hasAccess)
                        {
                            throw new Exception("User does not have permission to update this entity");
                        }
                    }

                    #endregion Check role access

                    #region Special rules

                    if (((IEntity)record).GetEntityName() == application_entity.EntityName)
                    {
                        (record as application_entity).entity_id =
                            (oldRecord as application_entity).entity_id;
                        (record as application_entity).entity_name =
                            (oldRecord as application_entity).entity_name;
                    }

                    if (((IEntity)record).GetEntityName() == application_user.EntityName)
                    {
                        (record as application_user).application_password =
                            (oldRecord as application_user).application_password;
                        (record as application_user).is_sysadmin =
                            (oldRecord as application_user).is_sysadmin;
                        (record as application_user).access_token =
                            (oldRecord as application_user).access_token;
                        (record as application_user).token_expiry =
                            (oldRecord as application_user).token_expiry;
                    }

                    if (((IEntity)record).GetEntityName() == organization.EntityName)
                    {
                        (record as organization).parent_organization_id =
                            (oldRecord as organization).parent_organization_id;

                        (record as organization).connection_string =
                            (oldRecord as organization).connection_string;
                    }

                    #endregion Special rules

                    #region Update record

                    using (var scope = Database.GetTransaction())
                    {
                        try
                        {
                            await Database.UpdateAsync(record);
                            logger.Trace(Database.LastCommand);

                            scope.Complete();
                            result = true;
                        }
                        catch (Exception ex)
                        {
                            logger.Debug(Database.LastCommand);
                            logger.Error(ex.ToString());
                        }
                    }

                    logger.Debug($"Update record = {result}");

                    #endregion Update record

                    #endregion Update
                }
            }
            catch (Exception ex)
            {
                logger.Debug(Database.LastCommand);
                logger.Error(ex.ToString());
            }

            return result;
        }
    }
}
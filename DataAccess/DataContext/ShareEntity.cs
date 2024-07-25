using DataAccess.Interfaces;
using DataAccess.Repository;
using PetaPoco;
using System;
using System.Threading.Tasks;

namespace DataAccess
{
    public partial class DataContext
    {
        public async Task<bool> ShareEntity<T>(T record, string ToId, bool Read, bool Write)
        {
            bool result = false;
            string log;

            try
            {
                var appEntity = Activator.CreateInstance<T>();
                if (!typeof(IEntity).IsAssignableFrom(typeof(T)))
                {
                    throw new Exception("Record is not IEntity");
                }

                var sharedTo = await Database.FirstOrDefaultAsync<application_user>(
                    " WHERE is_active = TRUE AND organization_id = @0 AND id = @1 ",
                    OrganizationId, ToId);
                logger.Trace(Database.LastCommand);

                if (sharedTo == null)
                {
                    throw new Exception("Shared-to user is not found");
                }

                var sql = Sql.Builder.Append(
                    " WHERE is_active = TRUE AND organization_id = @0 AND record_id = @1 AND shared_to_id = @2 ",
                    OrganizationId, ((IEntity)record).id, sharedTo.id);
                var sharedRecord = await Database.FirstOrDefaultAsync<shared_record>(sql);
                logger.Trace(Database.LastCommand);

                if (sharedRecord != null)
                {
                    #region Insert

                    log = "Insert shared record ...";
                    logger.Debug(log);

                    #region Set common values

                    sharedRecord = new shared_record()
                    {
                        id = Guid.NewGuid().ToString("N").ToLower(),
                        created_by = AppUserId,
                        created_on = DateTime.Now,
                        is_active = true,
                        owner_id = AppUserId,
                        organization_id = OrganizationId
                    };

                    #endregion Set common values

                    #region Check role access
                    /*
                    if (!IsSysAdmin)
                    {
                        sql = Sql.Builder.Append("SELECT ufn_can_create( @0, @1 )",
                            shared_record.EntityName, AppUserId);
                        var hasAccess = await Database.ExecuteScalarAsync<bool>(sql);
                        if (!hasAccess)
                        {
                            throw new Exception("User doesn't have permission to share entity");
                        }
                    }
                    */
                    #endregion Check role access

                    #region Insert record

                    sharedRecord.record_id = ((IEntity)record).id;
                    sharedRecord.shared_to_id = sharedTo.id;
                    sharedRecord.can_read = Read;
                    sharedRecord.can_write = Write;

                    using (var scope = Database.GetTransaction())
                    {
                        try
                        {
                            await Database.InsertAsync(sharedRecord);
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

                    log = String.Format("Insert record = {0}", result);
                    logger.Debug(log);

                    #endregion Insert record

                    #endregion Insert
                }
                else
                {
                    #region Update

                    log = "Update shared record ...";
                    logger.Debug(log);

                    #region Set common values

                    ((IEntity)record).created_by = sharedRecord.created_by;
                    ((IEntity)record).created_on = sharedRecord.created_on;
                    ((IEntity)record).modified_by = AppUserId;
                    ((IEntity)record).modified_on = DateTime.Now;
                    ((IEntity)record).is_active = sharedRecord.is_active;
                    ((IEntity)record).owner_id = sharedRecord.owner_id;
                    ((IEntity)record).organization_id = sharedRecord.organization_id;

                    #endregion Set common values

                    #region Check role access
                    /*
                    if (!IsSysAdmin)
                    {
                        sql = Sql.Builder.Append("SELECT ufn_can_update( @0, @1 )",
                            sharedRecord.id, AppUserId);
                        var hasAccess = await Database.ExecuteScalarAsync<bool>(sql);
                        if (!hasAccess)
                        {
                            throw new Exception("User doesn't have permission to update this entity");
                        }
                    }
                    */
                    #endregion Check role access

                    #region Update record

                    sharedRecord.can_read = Read;
                    sharedRecord.can_write = Write;

                    using (var scope = Database.GetTransaction())
                    {
                        try
                        {
                            await Database.UpdateAsync(sharedRecord);
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

                    log = String.Format("Update record = {0}", result);
                    logger.Debug(log);

                    #endregion Update record

                    #endregion Update
                }
            }
            catch (Exception ex)
            {
                logger.Debug(Database.LastCommand);
                logger.Error(ex);
            }

            return result;
        }
    }
}
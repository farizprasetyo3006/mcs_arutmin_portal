using DataAccess.Interfaces;
using PetaPoco;
using System;
using System.Threading.Tasks;

namespace DataAccess
{
    public partial class DataContext
    {
        public async Task<bool> DeleteEntity<T>(string Id, Action<bool> continueWith)
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

                var sql = Sql.Builder.Append(" SELECT id FROM entity ");
                sql.Append(" WHERE is_active = TRUE AND organization_id = @0 AND id = @1 ",
                    OrganizationId, Id);
                var id = await Database.ExecuteScalarAsync<string>(sql);
                logger.Trace(Database.LastCommand);

                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("Record is not found");
                }

                #region Delete

                log = "Delete record ...";
                logger.Debug(log);

                #region Check role access

                if (!IsSysAdmin)
                {
                    sql = Sql.Builder.Append(" SELECT ufn_can_delete( @0, @1 ) ", id, AppUserId);
                    var hasAccess = await Database.ExecuteScalarAsync<bool>(sql);
                    logger.Trace(Database.LastCommand);

                    if (!hasAccess)
                    {
                        throw new Exception("User does not have permission to delete this entity");
                    }
                }

                #endregion Check role access

                #region Delete record

                using (var scope = Database.GetTransaction())
                {
                    try
                    {
                        await Database.DeleteAsync<T>("WHERE id = @0", id);
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

                log = String.Format("Delete record = {0}", result);
                logger.Debug(log);

                #endregion Delete record

                #endregion Delete
            }
            catch (Exception ex)
            {
                logger.Debug(Database.LastCommand);
                logger.Error(ex.ToString());
            }
            finally
            {
                continueWith?.Invoke(result);
            }

            return result;
        }
    }
}
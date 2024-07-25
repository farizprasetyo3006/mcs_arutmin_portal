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

namespace BusinessLogic.Entity
{
    public partial class ApplicationUser: ServiceRepository<application_user, vw_application_user>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public ApplicationUser(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public async Task<StandardResult> ResetPassword(string Id, string NewPassword)
        {
            var result = new StandardResult();
            logger.Debug($"IsSysAdmin = {userContext.IsSysAdmin}");

            if (!userContext.IsSysAdmin && userContext.AppUserId != Id) 
                return result;

            try
            {
                var dc = userContext.GetDataContext();
                var db = dc.Database;
                using(var tx = db.GetTransaction())
                {
                    try
                    {
                        var record = await db.FirstOrDefaultAsync<application_user>("WHERE id = @0", Id);
                        logger.Debug(db.LastCommand);

                        if (record != null)
                        {
                            logger.Debug($"New password for {Id} = {NewPassword}");

                            var sql = Sql.Builder.Append(" UPDATE application_user "
                                + " SET modified_by = @0, modified_on = @1 "
                                + " , application_password = @2 WHERE id = @3 ",
                                userContext.AppUserId, DateTime.Now,
                                StringHash.CreateHash(NewPassword), record.id);
                            var ar = await db.ExecuteAsync(sql);
                            tx.Complete();

                            logger.Debug(db.LastCommand);
                            result.Success = ar > 0;
                        }
                        else
                        {
                            result.Message = "User does not exist";
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(db.LastCommand);
                        logger.Error(ex.ToString());
                        result.Message = ex.Message;
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

        public async Task<StandardResult> GetApiKey(string Id)
        {
            var result = new StandardResult();
            logger.Debug($"IsSysAdmin = {userContext.IsSysAdmin}");

            if (!userContext.IsSysAdmin && userContext.AppUserId != Id)
                return result;

            try
            {
                var dc = userContext.GetDataContext();
                var db = dc.Database;
                try
                {
                    var record = await db.FirstOrDefaultAsync<application_user>("WHERE id = @0", Id);
                    logger.Debug(db.LastCommand);

                    if (record != null)
                    {
                        result.Data = new { record.api_key, record.expired_date };
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = "User does not exist";
                    }
                }
                catch (Exception ex)
                {
                    logger.Debug(db.LastCommand);
                    logger.Error(ex.ToString());
                    result.Message = ex.Message;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<StandardResult> RefreshApiKey(string Id, DateTime ExpiredDate)
        {
            var result = new StandardResult();
            logger.Debug($"IsSysAdmin = {userContext.IsSysAdmin}");

            if (!userContext.IsSysAdmin && userContext.AppUserId != Id)
                return result;

            try
            {
                var dc = userContext.GetDataContext();
                var db = dc.Database;
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        var record = await db.FirstOrDefaultAsync<application_user>("WHERE id = @0", Id);
                        logger.Debug(db.LastCommand);

                        if (record != null)
                        {
                            var newApiKey = Guid.NewGuid().ToString("N");
                            logger.Debug($"New API key for {Id} = {newApiKey}");

                            var sql = Sql.Builder.Append(" UPDATE application_user "
                                + " SET modified_by = @0, modified_on = @1 "
                                + " , api_key = @2, expired_date = @3 WHERE id = @4 ",
                                userContext.AppUserId, DateTime.Now,
                                newApiKey, ExpiredDate.Date, record.id);
                            var ar = await db.ExecuteAsync(sql);
                            tx.Complete();

                            logger.Debug(db.LastCommand);
                            result.Success = ar > 0;
                            result.Data = new { api_key = newApiKey, expired_date = ExpiredDate.Date };
                        }
                        else
                        {
                            result.Message = "User does not exist";
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(db.LastCommand);
                        logger.Error(ex.ToString());
                        result.Message = ex.Message;
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

        public async Task<StandardResult> DisableUser(string Id)
        {
            var result = new StandardResult();
            if (!userContext.IsSysAdmin) return result;
            else if (userContext.AppUserId == Id) return result;

            try
            {
                var dc = userContext.GetDataContext();
                var db = dc.Database;
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        var record = await db.FirstOrDefaultAsync<application_user>("WHERE id = @0", Id);
                        if (record != null)
                        {
                            var sql = Sql.Builder.Append(" UPDATE application_user "
                                + " SET modified_by = @0, modified_on = @1 "
                                + " , is_active = @2 WHERE id = @3 ",
                                userContext.AppUserId, DateTime.Now,
                                false, record.id);
                            var ar = await db.ExecuteAsync(sql);
                            tx.Complete();

                            logger.Debug(db.LastCommand);
                            result.Success = ar > 0;
                        }
                        else
                        {
                            result.Message = "User does not exist";
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(db.LastCommand);
                        logger.Error(ex.ToString());
                        result.Message = ex.Message;
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

        public async Task<StandardResult> EnableUser(string Id)
        {
            var result = new StandardResult();
            if (!userContext.IsSysAdmin) return result;

            try
            {
                var dc = userContext.GetDataContext();
                var db = dc.Database;
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        var record = await db.FirstOrDefaultAsync<application_user>("WHERE id = @0", Id);
                        if (record != null)
                        {
                            var sql = Sql.Builder.Append(" UPDATE application_user "
                                + " SET modified_by = @0, modified_on = @1 "
                                + " , is_active = @2 WHERE id = @3 ",
                                userContext.AppUserId, DateTime.Now,
                                true, record.id);
                            var ar = await db.ExecuteAsync(sql);
                            tx.Complete();

                            logger.Debug(db.LastCommand);
                            result.Success = ar > 0;
                        }
                        else
                        {
                            result.Message = "User does not exist";
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(db.LastCommand);
                        logger.Error(ex.ToString());
                        result.Message = ex.Message;
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

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
    public partial class BusinessUnit : ServiceRepository<business_unit, vw_business_unit>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public BusinessUnit(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public async Task<StandardResult> SaveBusinessUnit(business_unit businessUnit)
        {
            var result = new StandardResult();

            try
            {
                var db = context.Database;
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        var success = false;
                        var record = await GetByIdAsync(businessUnit.id);
                        logger.Trace(db.LastCommand);

                        if (record == null)
                        {
                            
                            #region Create or update team

                            var defaultTeam = new team()
                            {
                                team_name = businessUnit.business_unit_name,
                                created_by = userContext.AppUserId,
                                created_on = DateTime.Now                               
                            };
                            success = await context.SaveEntity(defaultTeam);
                            logger.Trace(db.LastCommand);

                            #endregion

                            #region Create or update business unit

                            if (success)
                            {
                                record = new business_unit();
                                record.InjectFrom(businessUnit);
                                record.default_team_id = defaultTeam.id;
                                success = await context.SaveEntity(record);
                                logger.Trace(db.LastCommand);
                            }

                            #endregion

                            if(success)
                            {
                                tx.Complete();
                            }
                        }
                        else
                        {
                            var r = await SaveWithMapEntity(record, (isNew, success) => 
                            {
                                result.Success = success;
                            });

                            if (result.Success)
                            {
                                result.Data = r;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
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

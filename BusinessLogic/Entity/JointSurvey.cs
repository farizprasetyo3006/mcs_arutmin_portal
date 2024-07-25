using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Utilities;
using DataAccess;
using DataAccess.Repository;
using NLog;
using Omu.ValueInjecter;
using PetaPoco;
using PetaPoco.Providers;

namespace BusinessLogic.Entity
{
    public partial class JointSurvey: ServiceRepository<joint_survey, vw_joint_survey>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public JointSurvey(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public static async Task<StandardResult> UpdateStockState(string ConnectionString, joint_survey TransactionRecord)
        {
            var result = new StandardResult();
            logger.Trace($"UpdateStockState; TransactionRecord.id = {0}", TransactionRecord.id);

            var source_location_id = TransactionRecord.mine_location_id
                ?? TransactionRecord.stockpile_location_id
                ?? TransactionRecord.port_location_id
                ?? TransactionRecord.waste_location_id;
            logger.Debug($"UpdateStockState; source_location_id = {source_location_id}");

            try
            {
                var db = new Database(ConnectionString, new PostgreSQLDatabaseProvider());
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        if (TransactionRecord != null && TransactionRecord.join_survey_date != null
                            && !string.IsNullOrEmpty(source_location_id))
                        {
                            var record = await db.FirstOrDefaultAsync<joint_survey>(
                                "WHERE id = @0", TransactionRecord.id);

                            if (record != null)
                            {                             
                                logger.Debug(db.LastCommand);

                                #region Latest quality sampling

                                var qualitySampling = await db.FirstOrDefaultAsync<quality_sampling>(
                                    " WHERE stock_location_id = @0 "
                                    + " AND sampling_datetime <= @1 "
                                    + " ORDER BY sampling_datetime DESC ",
                                    source_location_id,
                                    record.join_survey_date);

                                logger.Debug(db.LastCommand);

                                #endregion

                                #region Source stock state

                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, source_location_id);
                                if (sourceStockState == null)
                                {
                                    sourceStockState = new stock_state()
                                    {
                                        created_by = record.modified_by ?? record.created_by,
                                        created_on = DateTime.Now,
                                        is_active = true,
                                        owner_id = record.owner_id,
                                        organization_id = record.organization_id,
                                        transaction_id = TransactionRecord.id,
                                        transaction_datetime = TransactionRecord.join_survey_date,
                                        stock_location_id = source_location_id,
                                        product_out_id = TransactionRecord.product_id,
                                        quality_sampling_id = qualitySampling?.id
                                    };
                                }
                                else
                                {
                                    sourceStockState.transaction_datetime = TransactionRecord.join_survey_date;
                                    sourceStockState.quality_sampling_id = qualitySampling?.id;
                                }
                                logger.Debug(db.LastCommand);

                                // Get previous source stock state
                                var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                    source_location_id, TransactionRecord.join_survey_date);

                                logger.Debug(db.LastCommand);

                                sourceStockState.qty_opening = prevSourceStockState?.qty_closing ?? 0;
                                sourceStockState.qty_in = 0;
                                sourceStockState.qty_out = 0;
                                sourceStockState.qty_survey = record.quantity ?? 0;
                                sourceStockState.qty_closing = sourceStockState.qty_survey;

                                if (string.IsNullOrEmpty(sourceStockState.id))
                                {
                                    sourceStockState.id = Guid.NewGuid().ToString("N").ToLower();
                                    await db.InsertAsync(sourceStockState);
                                }
                                else
                                {
                                    sourceStockState.modified_by = record.modified_by ?? record.created_by;
                                    sourceStockState.modified_on = DateTime.Now;
                                    await db.UpdateAsync(sourceStockState);
                                }

                                // Modify all subsequent stock state
                                var nextSourceStockStates = await db.FetchAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime > @1",
                                    source_location_id, TransactionRecord.join_survey_date);
                                if (nextSourceStockStates != null && nextSourceStockStates.Count > 0)
                                {
                                    var qty_opening = sourceStockState.qty_closing ?? 0;
                                    foreach (var nextSourceStockState in nextSourceStockStates.OrderBy(o => o.transaction_datetime))
                                    {
                                        nextSourceStockState.qty_opening = qty_opening;
                                        nextSourceStockState.qty_closing = nextSourceStockState.qty_opening +
                                            (nextSourceStockState.qty_in ?? 0) - (nextSourceStockState.qty_out ?? 0);
                                        nextSourceStockState.modified_by = record.modified_by ?? record.created_by;
                                        nextSourceStockState.modified_on = DateTime.Now;

                                        if (nextSourceStockState.qty_survey != null)
                                        {
                                            nextSourceStockState.qty_closing = nextSourceStockState.qty_survey;
                                            await db.UpdateAsync(nextSourceStockState);
                                            break;
                                        }
                                        else
                                        {
                                            await db.UpdateAsync(nextSourceStockState);
                                            qty_opening = nextSourceStockState.qty_closing ?? 0;
                                        }
                                    }
                                }
                                logger.Debug(db.LastCommand);

                                #endregion
                            }
                            else // Handle deleted transaction
                            {
                                #region Source stock state

                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, source_location_id);
                                if (sourceStockState != null)
                                {
                                    sourceStockState.transaction_datetime = TransactionRecord.join_survey_date;

                                    // Get previous source stock state
                                    var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        source_location_id, TransactionRecord.join_survey_date);

                                    sourceStockState.qty_opening = prevSourceStockState?.qty_closing ?? 0;
                                    sourceStockState.qty_in = 0;
                                    sourceStockState.qty_out = 0;
                                    sourceStockState.qty_closing = sourceStockState.qty_opening;
                                    sourceStockState.modified_by = record.modified_by ?? record.created_by;
                                    sourceStockState.modified_on = DateTime.Now;
                                    await db.UpdateAsync(sourceStockState);

                                    // Modify all subsequent stock state
                                    var nextSourceStockStates = await db.FetchAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime > @1",
                                        source_location_id, TransactionRecord.join_survey_date);
                                    if (nextSourceStockStates != null && nextSourceStockStates.Count > 0)
                                    {
                                        var qty_opening = sourceStockState.qty_closing ?? 0;
                                        foreach (var nextSourceStockState in nextSourceStockStates.OrderBy(o => o.transaction_datetime))
                                        {
                                            nextSourceStockState.qty_opening = qty_opening;
                                            nextSourceStockState.qty_closing = nextSourceStockState.qty_opening +
                                                (nextSourceStockState.qty_in ?? 0) - (nextSourceStockState.qty_out ?? 0);
                                            nextSourceStockState.modified_by = record.modified_by ?? record.created_by;
                                            nextSourceStockState.modified_on = DateTime.Now;

                                            if (nextSourceStockState.qty_survey != null)
                                            {
                                                nextSourceStockState.qty_closing = nextSourceStockState.qty_survey;
                                                await db.UpdateAsync(nextSourceStockState);
                                                break;
                                            }
                                            else
                                            {
                                                await db.UpdateAsync(nextSourceStockState);
                                                qty_opening = nextSourceStockState.qty_closing ?? 0;
                                            }
                                        }
                                    }
                                }
                                logger.Debug(db.LastCommand);

                                #endregion
                            }

                            tx.Complete();
                            result.Success = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(db.LastCommand);
                        logger.Error(ex);
                        result.Message = ex.InnerException?.Message ?? ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        public async Task<StandardResult> Approve(string JointSurveyId)
        {
            var result = new StandardResult();

            var dc = userContext.GetDataContext();
            var db = dc.Database;
            using (var tx = db.GetTransaction())
            {
                try
                {
                    var record = await db.FirstOrDefaultAsync<joint_survey>("WHERE id = @0", JointSurveyId);
                    logger.Debug(db.LastCommand);

                    if(record != null)
                    {
                        record.approved_by = userContext.AppUserId;
                        record.approved_on = DateTime.Now;
                        await db.UpdateAsync(record);
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = $"Survey {JointSurveyId} is not found";
                    }
                }
                catch (Exception ex)
                {
                    logger.Debug(db.LastCommand);
                    logger.Error(ex.ToString());
                    result.Message = ex.InnerException?.Message ?? ex.Message;
                }
            }

            return result;
        }
    }
}

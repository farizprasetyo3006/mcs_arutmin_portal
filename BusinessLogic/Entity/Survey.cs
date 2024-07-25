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
    public partial class Survey: ServiceRepository<survey, vw_survey>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public Survey(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public static async Task<StandardResult> UpdateStockState(string ConnectionString, survey TransactionRecord)
        {
            var result = new StandardResult();
            logger.Trace($"UpdateStockState; TransactionRecord.id = {0}", TransactionRecord.id);

            try
            {
                var db = new Database(ConnectionString, new PostgreSQLDatabaseProvider());
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        if (TransactionRecord != null && TransactionRecord.survey_date != null)
                        {
                            var record = await db.FirstOrDefaultAsync<survey>(
                                "WHERE id = @0", TransactionRecord.id);

                            if (record != null)
                            {
                                #region Latest quality sampling

                                var qualitySampling = await db.FirstOrDefaultAsync<quality_sampling>(
                                    " WHERE stock_location_id = @0 "
                                    + " AND sampling_datetime <= @1 "
                                    + " ORDER BY sampling_datetime DESC ",
                                    TransactionRecord.stock_location_id,
                                    TransactionRecord.survey_date);

                                logger.Debug(db.LastCommand);

                                #endregion

                                #region Source stock state

                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, TransactionRecord.stock_location_id);
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
                                        transaction_datetime = TransactionRecord.survey_date,
                                        stock_location_id = TransactionRecord.stock_location_id,
                                        product_out_id = TransactionRecord.product_id,
                                        quality_sampling_id = qualitySampling?.id
                                    };
                                }
                                else
                                {
                                    sourceStockState.transaction_datetime = TransactionRecord.survey_date;
                                    sourceStockState.quality_sampling_id = qualitySampling?.id;
                                }
                                logger.Debug(db.LastCommand);

                                // Get previous source stock state
                                var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                    TransactionRecord.stock_location_id, TransactionRecord.survey_date);

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
                                    TransactionRecord.stock_location_id, TransactionRecord.survey_date);
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
                                    TransactionRecord.id, TransactionRecord.stock_location_id);
                                if (sourceStockState != null)
                                {
                                    sourceStockState.transaction_datetime = TransactionRecord.survey_date;

                                    // Get previous source stock state
                                    var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        TransactionRecord.stock_location_id, TransactionRecord.survey_date);

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
                                        TransactionRecord.stock_location_id, TransactionRecord.survey_date);
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

        public static async Task<StandardResult> UpdateStockState(string ConnectionString, survey_detail SurveyDetail)
        {
            var result = new StandardResult();
            logger.Trace($"UpdateStockState; SurveyDetail.id = {0}", SurveyDetail.id);

            try
            {
                var db = new Database(ConnectionString, new PostgreSQLDatabaseProvider());
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        var TransactionRecord = await db.FirstOrDefaultAsync<survey>(
                            "WHERE id = @0", SurveyDetail.survey_id);
                        if (TransactionRecord != null && TransactionRecord.survey_date != null)
                        {
                            var record = await db.FirstOrDefaultAsync<survey>(
                                "WHERE id = @0", TransactionRecord.id);

                            if (record != null)
                            {
                                #region Stock state

                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, TransactionRecord.stock_location_id);
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
                                        transaction_datetime = TransactionRecord.survey_date,
                                        stock_location_id = TransactionRecord.stock_location_id,
                                        survey_id = TransactionRecord.id
                                    };
                                }
                                else
                                {
                                    sourceStockState.transaction_id = TransactionRecord.id;
                                    sourceStockState.transaction_datetime = TransactionRecord.survey_date;
                                    sourceStockState.stock_location_id = TransactionRecord.stock_location_id;
                                    sourceStockState.survey_id = TransactionRecord.id;
                                }
                                logger.Debug(db.LastCommand);

                                // Get previous source stock state
                                var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                    TransactionRecord.stock_location_id, TransactionRecord.survey_date);

                                sourceStockState.qty_opening = prevSourceStockState?.qty_closing ?? 0;
                                sourceStockState.qty_in = 0;
                                sourceStockState.qty_out = 0;
                                sourceStockState.qty_survey = TransactionRecord.quantity ?? 0;
                                sourceStockState.qty_closing = sourceStockState.qty_survey ?? 0;

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
                                logger.Debug(db.LastCommand);

                                // Modify all subsequent stock state
                                var nextSourceStockStates = await db.FetchAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime > @1",
                                    TransactionRecord.stock_location_id, TransactionRecord.survey_date);
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

        public async Task<StandardResult> Approve(string SurveyId)
        {
            var result = new StandardResult();

            var dc = userContext.GetDataContext();
            var db = dc.Database;
            using (var tx = db.GetTransaction())
            {
                try
                {
                    var record = await db.FirstOrDefaultAsync<survey>("WHERE id = @0", SurveyId);
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
                        result.Message = $"Survey {SurveyId} is not found";
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

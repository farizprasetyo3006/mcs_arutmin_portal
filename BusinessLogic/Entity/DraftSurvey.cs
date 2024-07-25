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
    public partial class DraftSurvey: ServiceRepository<draft_survey, vw_draft_survey>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public DraftSurvey(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public static async Task<StandardResult> UpdateStockState(string ConnectionString, draft_survey TransactionRecord)
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
                            var record = await db.FirstOrDefaultAsync<draft_survey>(
                                "WHERE id = @0", TransactionRecord.id);
                            logger.Debug(db.LastCommand);

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
                                        transaction_datetime = TransactionRecord.survey_date.Value,
                                        stock_location_id = TransactionRecord.stock_location_id,
                                        survey_id = TransactionRecord.id
                                    };
                                }
                                else
                                {
                                    sourceStockState.transaction_id = TransactionRecord.id;
                                    sourceStockState.transaction_datetime = TransactionRecord.survey_date.Value;
                                    sourceStockState.stock_location_id = TransactionRecord.stock_location_id;
                                    sourceStockState.survey_id = TransactionRecord.id;
                                }
                                logger.Debug(db.LastCommand);

                                // Get previous source stock state
                                var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                    TransactionRecord.stock_location_id, TransactionRecord.survey_date);
                                logger.Debug(db.LastCommand);

                                sourceStockState.qty_opening = prevSourceStockState?.qty_closing ?? 0;
                                sourceStockState.qty_survey = TransactionRecord.quantity ?? 0;
                                sourceStockState.qty_in = 0;
                                sourceStockState.qty_out = 0;
                                sourceStockState.qty_adjustment = sourceStockState.qty_survey - sourceStockState.qty_opening;
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
                                            nextSourceStockState.qty_adjustment = nextSourceStockState.qty_survey - nextSourceStockState.qty_opening;
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
                                #region Stock state

                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, TransactionRecord.stock_location_id);
                                if (sourceStockState != null)
                                {
                                    sourceStockState.transaction_datetime = TransactionRecord.survey_date.Value;

                                    // Get previous source stock state
                                    var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        TransactionRecord.stock_location_id, TransactionRecord.survey_date);

                                    sourceStockState.qty_opening = prevSourceStockState?.qty_closing ?? 0;
                                    sourceStockState.qty_in = 0;
                                    sourceStockState.qty_out = 0;
                                    sourceStockState.qty_survey = 0;
                                    sourceStockState.qty_adjustment = 0;
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

                                    var sql = Sql.Builder.Append("DELETE FROM stock_state");
                                    sql.Append("WHERE transaction_id = @0", TransactionRecord.id);
                                    sql.Append("AND stock_location_id = @0", TransactionRecord.stock_location_id);
                                    await db.ExecuteAsync(sql);
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

        public async Task<StandardResult> Approve(string DraftSurveyId)
        {
            var result = new StandardResult();

            var dc = userContext.GetDataContext();
            var db = dc.Database;
            using (var tx = db.GetTransaction())
            {
                try
                {
                    var record = await db.FirstOrDefaultAsync<draft_survey>("WHERE id = @0", DraftSurveyId);
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
                        result.Message = $"Draft survey {DraftSurveyId} is not found";
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

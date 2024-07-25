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
using Newtonsoft.Json;

namespace BusinessLogic.Entity
{
    public partial class HaulingTransaction: ServiceRepository<hauling_transaction, vw_hauling_transaction>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public HaulingTransaction(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public static async Task<StandardResult> UpdateStockState(string ConnectionString, hauling_transaction TransactionRecord)
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
                        if (TransactionRecord != null && TransactionRecord.unloading_datetime != null)
                        {
                            var record = await db.FirstOrDefaultAsync<hauling_transaction>(
                                "WHERE id = @0", TransactionRecord.id);

                            if(record != null)
                            {
                                logger.Debug(db.LastCommand);

                                #region Source stock state

                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, TransactionRecord.source_location_id);
                                logger.Debug(db.LastCommand);

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
                                        transaction_datetime = TransactionRecord.loading_datetime,
                                        stock_location_id = TransactionRecord.source_location_id,
                                        product_out_id = TransactionRecord.product_id
                                    };
                                }
                                else
                                {
                                    sourceStockState.transaction_datetime = TransactionRecord.loading_datetime;
                                }
                                logger.Debug(db.LastCommand);

                                // Get previous source stock state
                                var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                    TransactionRecord.source_location_id, TransactionRecord.loading_datetime);
                                logger.Debug(db.LastCommand);

                                sourceStockState.qty_opening = prevSourceStockState?.qty_closing ?? 0;
                                sourceStockState.qty_in = 0;
                                sourceStockState.qty_out = record.loading_quantity;
                                sourceStockState.qty_closing = (sourceStockState.qty_opening ?? 0)
                                    - (sourceStockState.qty_out ?? 0);

                                if (string.IsNullOrEmpty(sourceStockState.id))
                                {
                                    sourceStockState.id = Guid.NewGuid().ToString("N").ToLower();
                                    await db.InsertAsync(sourceStockState);
                                    logger.Debug(db.LastCommand);
                                }
                                else
                                {
                                    sourceStockState.modified_by = record.modified_by ?? record.created_by;
                                    sourceStockState.modified_on = DateTime.Now;
                                    await db.UpdateAsync(sourceStockState);
                                    logger.Debug(db.LastCommand);
                                }

                                // Modify all subsequent stock state
                                var nextSourceStockStates = await db.FetchAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime > @1",
                                    TransactionRecord.source_location_id, TransactionRecord.loading_datetime);
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

                                #region Destination stock state

                                var destinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, TransactionRecord.destination_location_id);
                                logger.Debug(db.LastCommand);

                                if (destinationStockState == null)
                                {
                                    destinationStockState = new stock_state()
                                    {
                                        created_by = record.modified_by ?? record.created_by,
                                        created_on = DateTime.Now,
                                        is_active = true,
                                        owner_id = record.owner_id,
                                        organization_id = record.organization_id,
                                        transaction_id = TransactionRecord.id,
                                        transaction_datetime = TransactionRecord.unloading_datetime.Value,
                                        stock_location_id = TransactionRecord.destination_location_id,
                                        product_in_id = TransactionRecord.product_id
                                    };
                                }
                                else
                                {
                                    destinationStockState.transaction_datetime = TransactionRecord.unloading_datetime.Value;
                                }
                                logger.Debug(db.LastCommand);

                                // Get previous destination stock state
                                var prevDestinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                    TransactionRecord.destination_location_id, TransactionRecord.unloading_datetime.Value); //cek juga
                                logger.Debug(db.LastCommand);

                                destinationStockState.qty_opening = prevDestinationStockState?.qty_closing ?? 0;
                                //destinationStockState.qty_in = record?.unloading_quantity ?? 0;
                                destinationStockState.qty_in = record?.loading_quantity ?? 0; // cek ambil dari mana?
                                destinationStockState.qty_out = 0;
                                destinationStockState.qty_closing = (destinationStockState.qty_opening ?? 0)
                                    + (destinationStockState.qty_in ?? 0);

                                if (string.IsNullOrEmpty(destinationStockState.id))
                                {
                                    destinationStockState.id = Guid.NewGuid().ToString("N").ToLower();
                                    await db.InsertAsync(destinationStockState);
                                    logger.Debug(db.LastCommand);
                                }
                                else
                                {
                                    destinationStockState.modified_by = record.modified_by ?? record.created_by;
                                    destinationStockState.modified_on = DateTime.Now;
                                    await db.UpdateAsync(destinationStockState);
                                    logger.Debug(db.LastCommand);
                                }

                                // Modify all subsequent stock state
                                var nextDestinationStockStates = await db.FetchAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime > @1",
                                    TransactionRecord.destination_location_id, TransactionRecord.unloading_datetime.Value);
                                if (nextDestinationStockStates != null && nextDestinationStockStates.Count > 0)
                                {
                                    var qty_opening = destinationStockState.qty_closing ?? 0;
                                    foreach (var nextDestinationStockState in nextDestinationStockStates.OrderBy(o => o.transaction_datetime))
                                    {
                                        nextDestinationStockState.qty_opening = qty_opening;
                                        nextDestinationStockState.qty_closing = nextDestinationStockState.qty_opening +
                                            (nextDestinationStockState.qty_in ?? 0) - (nextDestinationStockState.qty_out ?? 0);
                                        nextDestinationStockState.modified_by = record.modified_by ?? record.created_by;
                                        nextDestinationStockState.modified_on = DateTime.Now;

                                        if (nextDestinationStockState.qty_survey != null)
                                        {
                                            nextDestinationStockState.qty_closing = nextDestinationStockState.qty_survey;
                                            await db.UpdateAsync(nextDestinationStockState);
                                            logger.Debug(db.LastCommand);
                                            break;
                                        }
                                        else
                                        {
                                            await db.UpdateAsync(nextDestinationStockState);
                                            logger.Debug(db.LastCommand);
                                            qty_opening = nextDestinationStockState.qty_closing ?? 0;
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
                                    TransactionRecord.id, TransactionRecord.source_location_id);
                                if (sourceStockState != null)
                                {
                                    sourceStockState.transaction_datetime = TransactionRecord.loading_datetime;

                                    // Get previous source stock state
                                    var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        TransactionRecord.source_location_id, TransactionRecord.loading_datetime);

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
                                        TransactionRecord.source_location_id, TransactionRecord.loading_datetime);
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

                                #region Destination stock state

                                var destinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, TransactionRecord.destination_location_id);
                                if (destinationStockState != null)
                                {
                                    destinationStockState.transaction_datetime = TransactionRecord.unloading_datetime.Value;

                                    // Get previous destination stock state
                                    var prevDestinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        TransactionRecord.destination_location_id, TransactionRecord.unloading_datetime.Value);

                                    destinationStockState.qty_opening = prevDestinationStockState?.qty_closing ?? 0;
                                    destinationStockState.qty_in = 0;
                                    destinationStockState.qty_out = 0;
                                    destinationStockState.qty_closing = destinationStockState.qty_opening;
                                    destinationStockState.modified_by = record.modified_by ?? record.created_by;
                                    destinationStockState.modified_on = DateTime.Now;
                                    await db.UpdateAsync(destinationStockState);

                                    // Modify all subsequent stock state
                                    var nextDestinationStockStates = await db.FetchAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime > @1",
                                        TransactionRecord.destination_location_id, TransactionRecord.unloading_datetime.Value);
                                    if (nextDestinationStockStates != null && nextDestinationStockStates.Count > 0)
                                    {
                                        var qty_opening = destinationStockState.qty_closing ?? 0;
                                        foreach (var nextDestinationStockState in nextDestinationStockStates.OrderBy(o => o.transaction_datetime))
                                        {
                                            nextDestinationStockState.qty_opening = qty_opening;
                                            nextDestinationStockState.qty_closing = nextDestinationStockState.qty_opening +
                                                (nextDestinationStockState.qty_in ?? 0) - (nextDestinationStockState.qty_out ?? 0);
                                            nextDestinationStockState.modified_by = record.modified_by ?? record.created_by;
                                            nextDestinationStockState.modified_on = DateTime.Now;

                                            if (nextDestinationStockState.qty_survey != null)
                                            {
                                                nextDestinationStockState.qty_closing = nextDestinationStockState.qty_survey;
                                                await db.UpdateAsync(nextDestinationStockState);
                                                break;
                                            }
                                            else
                                            {
                                                await db.UpdateAsync(nextDestinationStockState);
                                                qty_opening = nextDestinationStockState.qty_closing ?? 0;
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

        public static async Task<StandardResult> UpdateStockStateAnalyte(string ConnectionString, hauling_transaction TransactionRecord)
        {
            var result = new StandardResult();
            logger.Trace($"UpdateStockStateAnalyte; TransactionRecord.id = {0}", TransactionRecord.id);

            try
            {
                var db = new Database(ConnectionString, new PostgreSQLDatabaseProvider());
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        if (TransactionRecord != null && TransactionRecord.loading_datetime != null)
                        {
                            var record = await db.FirstOrDefaultAsync<hauling_transaction>(
                                "WHERE id = @0", TransactionRecord.id);
                            logger.Debug(db.LastCommand);

                            if (record != null)
                            {
                                #region Get quality sampling

                                var sourceQualitySampling = await db.FirstOrDefaultAsync<quality_sampling>(
                                    " WHERE stock_location_id = @0 "
                                    + " AND sampling_datetime <= @1 "
                                    + " ORDER BY sampling_datetime DESC ",
                                    TransactionRecord.source_location_id,
                                    TransactionRecord.loading_datetime);
                                logger.Debug(db.LastCommand);

                                var srcQSAnalytes = new List<quality_sampling_analyte>();
                                if (sourceQualitySampling != null)
                                {
                                    srcQSAnalytes = await db.FetchAsync<quality_sampling_analyte>(
                                        "WHERE quality_sampling_id = @0", sourceQualitySampling.id
                                        );
                                    logger.Debug(db.LastCommand);
                                }

                                var destinationQualitySampling = await db.FirstOrDefaultAsync<quality_sampling>(
                                    " WHERE stock_location_id = @0 "
                                    + " AND sampling_datetime <= @1 "
                                    + " ORDER BY sampling_datetime DESC ",
                                    TransactionRecord.destination_location_id,
                                    TransactionRecord.loading_datetime);
                                logger.Debug(db.LastCommand);

                                var destQSAnalytes = new List<quality_sampling_analyte>();
                                if (destinationQualitySampling != null)
                                {
                                    destQSAnalytes = await db.FetchAsync<quality_sampling_analyte>(
                                        "WHERE quality_sampling_id = @0", destinationQualitySampling.id
                                        );
                                    logger.Debug(db.LastCommand);
                                }

                                #endregion

                                #region Get latest quality data

                                // Source stock state
                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    " WHERE transaction_datetime < @0 AND stock_location_id = @1 " +
                                    " ORDER BY transaction_datetime DESC ",
                                    TransactionRecord.loading_datetime, TransactionRecord.source_location_id);
                                logger.Debug(db.LastCommand);

                                var srcStockStateAnalytes = new List<stock_state_analyte>();
                                if (sourceStockState != null)
                                {
                                    srcStockStateAnalytes = await db.FetchAsync<stock_state_analyte>(
                                            "WHERE stock_state_id = @0", sourceStockState.id);
                                    logger.Debug(db.LastCommand);
                                }

                                // Destination stock state
                                var destinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    " WHERE transaction_datetime < @0 AND stock_location_id = @1 " +
                                    " ORDER BY transaction_datetime DESC ",
                                    TransactionRecord.loading_datetime, TransactionRecord.destination_location_id);
                                logger.Debug(db.LastCommand);

                                var destStockStateAnalytes = new List<stock_state_analyte>();
                                if (destinationStockState != null)
                                {
                                    destStockStateAnalytes = await db.FetchAsync<stock_state_analyte>(
                                            "WHERE stock_state_id = @0", destinationStockState.id);
                                    logger.Debug(db.LastCommand);
                                }

                                // Get from current source stock state
                                var currentSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    " WHERE transaction_id = @0 AND stock_location_id = @1 ",
                                    TransactionRecord.id, TransactionRecord.source_location_id);
                                logger.Debug(db.LastCommand);

                                // Get from current destination stock state
                                var currentDestinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    " WHERE transaction_id = @0 AND stock_location_id = @1 ", 
                                    TransactionRecord.id, TransactionRecord.destination_location_id);
                                logger.Debug(db.LastCommand);

                                #endregion

                                #region Calculate destination quality sampling

                                logger.Debug($"srcStockStateAnalytes = {JsonConvert.SerializeObject(srcStockStateAnalytes)}");
                                logger.Debug($"destStockStateAnalytes = {JsonConvert.SerializeObject(destStockStateAnalytes)}");
                                logger.Debug($"currentStockState = {JsonConvert.SerializeObject(currentDestinationStockState)}");

                                if (srcStockStateAnalytes != null && srcStockStateAnalytes.Count > 0
                                    && currentDestinationStockState != null)
                                {
                                    if(destStockStateAnalytes != null && destStockStateAnalytes.Count > 0)
                                    {
                                        foreach (var destStockStateAnalyte in destStockStateAnalytes)
                                        {
                                            logger.Debug($"destStockStateAnalyte = {JsonConvert.SerializeObject(destStockStateAnalyte)}");

                                            decimal wa_value = -1;
                                            decimal analyte_value = -1;
                                            var srcStockStateAnalyte = srcStockStateAnalytes.FirstOrDefault(
                                                    o => o.analyte_id == destStockStateAnalyte.analyte_id
                                                );
                                            logger.Debug($"srcStockStateAnalyte = {JsonConvert.SerializeObject(srcStockStateAnalyte)}");

                                            if (srcStockStateAnalyte != null && (currentDestinationStockState.qty_closing ?? 0) > 0)
                                            {
                                                // weighted-average
                                                analyte_value = srcStockStateAnalyte.analyte_value ?? 0;
                                                wa_value =
                                                    ((currentDestinationStockState.qty_opening ?? 0) * (destStockStateAnalyte.weighted_value ?? destStockStateAnalyte.analyte_value)
                                                    + (currentDestinationStockState.qty_in ?? 0) * (srcStockStateAnalyte.weighted_value ?? srcStockStateAnalyte.analyte_value))
                                                    / currentDestinationStockState.qty_closing ?? 0;
                                            }

                                            logger.Debug($"analyte_value = {analyte_value}, wa_value = {wa_value}");
                                            if (analyte_value >= 0 && wa_value >= 0)
                                            {
                                                var ssa = await db.FirstOrDefaultAsync<stock_state_analyte>(
                                                    "WHERE stock_state_id = @0 AND analyte_id = @1",
                                                    currentDestinationStockState.id, destStockStateAnalyte.analyte_id);
                                                if (ssa == null)
                                                {
                                                    ssa = new stock_state_analyte()
                                                    {
                                                        id = Guid.NewGuid().ToString("N").ToLower(),
                                                        created_by = currentDestinationStockState.created_by,
                                                        created_on = currentDestinationStockState.created_on,
                                                        organization_id = currentDestinationStockState.organization_id,
                                                        owner_id = currentDestinationStockState.owner_id,
                                                        is_active = true,
                                                        stock_state_id = currentDestinationStockState.id,
                                                        analyte_id = destStockStateAnalyte.analyte_id,
                                                        analyte_value = analyte_value,
                                                        weighted_value = wa_value
                                                    };

                                                    await db.InsertAsync(ssa);
                                                }
                                                else
                                                {
                                                    ssa.modified_by = currentDestinationStockState.modified_by;
                                                    ssa.modified_on = currentDestinationStockState.modified_on;
                                                    ssa.analyte_value = analyte_value;
                                                    ssa.weighted_value = wa_value;

                                                    await db.UpdateAsync(ssa);
                                                }

                                                logger.Debug($"stock_state_analyte = {JsonConvert.SerializeObject(ssa)}");
                                                logger.Debug(db.LastCommand);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var srcStockStateAnalyte in srcStockStateAnalytes)
                                        {
                                            logger.Debug($"srcStockStateAnalyte = {JsonConvert.SerializeObject(srcStockStateAnalyte)}");

                                            var ssa = await db.FirstOrDefaultAsync<stock_state_analyte>(
                                                "WHERE stock_state_id = @0 AND analyte_id = @1",
                                                currentDestinationStockState.id, srcStockStateAnalyte.analyte_id);
                                            if (ssa == null)
                                            {
                                                ssa = new stock_state_analyte()
                                                {
                                                    id = Guid.NewGuid().ToString("N").ToLower(),
                                                    created_by = currentDestinationStockState.created_by,
                                                    created_on = currentDestinationStockState.created_on,
                                                    organization_id = currentDestinationStockState.organization_id,
                                                    owner_id = currentDestinationStockState.owner_id,
                                                    is_active = true,
                                                    stock_state_id = currentDestinationStockState.id,
                                                    analyte_id = srcStockStateAnalyte.analyte_id,
                                                    analyte_value = srcStockStateAnalyte.analyte_value,
                                                    weighted_value = srcStockStateAnalyte.analyte_value
                                                };

                                                await db.InsertAsync(ssa);

                                                logger.Debug($"stock_state_analyte = {JsonConvert.SerializeObject(ssa)}");
                                                logger.Debug(db.LastCommand);
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #region Continue source quality sampling

                                if (srcStockStateAnalytes != null && srcStockStateAnalytes.Count > 0
                                    && currentSourceStockState != null)
                                {
                                    foreach (var srcStockStateAnalyte in srcStockStateAnalytes)
                                    {
                                        var ssa = await db.FirstOrDefaultAsync<stock_state_analyte>(
                                            "WHERE stock_state_id = @0 AND analyte_id = @1",
                                            currentSourceStockState.id, srcStockStateAnalyte.analyte_id);
                                        if (ssa == null)
                                        {
                                            ssa = new stock_state_analyte()
                                            {
                                                id = Guid.NewGuid().ToString("N").ToLower(),
                                                created_by = currentSourceStockState.created_by,
                                                created_on = currentSourceStockState.created_on,
                                                organization_id = currentSourceStockState.organization_id,
                                                owner_id = currentSourceStockState.owner_id,
                                                is_active = true,
                                                stock_state_id = currentSourceStockState.id,
                                                analyte_id = srcStockStateAnalyte.analyte_id,
                                                analyte_value = srcStockStateAnalyte.analyte_value,
                                                weighted_value = srcStockStateAnalyte.weighted_value
                                            };

                                            await db.InsertAsync(ssa);
                                            logger.Debug($"Source stock_state_analyte = {JsonConvert.SerializeObject(ssa)}");
                                            logger.Debug(db.LastCommand);
                                        }
                                    }
                                }

                                #endregion
                            }
                            else // Handle deleted transaction
                            {
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
    }
}

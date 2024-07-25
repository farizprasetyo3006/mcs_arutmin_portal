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
    public partial class ProcessingTransaction: ServiceRepository<processing_transaction, vw_processing_transaction>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public ProcessingTransaction(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public static async Task<StandardResult> UpdateStockState(string ConnectionString, processing_transaction TransactionRecord)
        {
            var result = new StandardResult();
            logger.Trace($"UpdateStockState; ProcessingTransaction; TransactionRecord.id = {0}", TransactionRecord.id);

            try
            {
                var db = new Database(ConnectionString, new PostgreSQLDatabaseProvider());
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        if (TransactionRecord != null && TransactionRecord.loading_datetime != null)
                        {
                            var record = await db.FirstOrDefaultAsync<processing_transaction>(
                                "WHERE id = @0", TransactionRecord.id);
                            logger.Debug(db.LastCommand);

                            if(record != null)
                            {
                                #region Latest quality sampling

                                var qs1 = await db.FirstOrDefaultAsync<quality_sampling>(
                                    " WHERE stock_location_id = @0 "
                                    + " AND sampling_datetime <= @1 "
                                    + " ORDER BY sampling_datetime DESC ",
                                    record.source_location_id,
                                    record.loading_datetime.Value);
                                logger.Debug(db.LastCommand);

                                var qs2 = await db.FirstOrDefaultAsync<quality_sampling>(
                                    " WHERE stock_location_id = @0 "
                                    + " AND sampling_datetime <= @1 "
                                    + " ORDER BY sampling_datetime DESC ",
                                    record.destination_location_id,
                                    record.loading_datetime.Value);
                                logger.Debug(db.LastCommand);

                                #endregion

                                #region Source stock state

                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1", record.id, record.source_location_id);
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
                                        transaction_id = record.id,
                                        transaction_datetime = record.loading_datetime.Value,
                                        stock_location_id = record.source_location_id,
                                        quality_sampling_id = record.quality_sampling_id ?? qs1?.id
                                    };
                                }
                                else
                                {
                                    sourceStockState.transaction_datetime = record.loading_datetime.Value;
                                    sourceStockState.quality_sampling_id = record.quality_sampling_id ?? qs1?.id;
                                }

                                // Get previous source stock state
                                var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                    TransactionRecord.source_location_id, TransactionRecord.loading_datetime.Value);
                                logger.Debug(db.LastCommand);

                                sourceStockState.qty_opening = prevSourceStockState?.qty_closing ?? 0;
                                sourceStockState.qty_in = 0;
                                sourceStockState.qty_out = record?.loading_quantity ?? 0;
                                sourceStockState.qty_closing = (sourceStockState.qty_opening ?? 0)
                                    - (sourceStockState.qty_out ?? 0);

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
                                    record.source_location_id, record.loading_datetime.Value);
                                logger.Debug(db.LastCommand);

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

                                        logger.Debug(db.LastCommand);
                                    }
                                }

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
                                        transaction_id = record.id,
                                        transaction_datetime = record.loading_datetime.Value,
                                        stock_location_id = record.destination_location_id,
                                        quality_sampling_id = record.quality_sampling_id ?? qs2?.id
                                    };
                                }
                                else
                                {
                                    destinationStockState.transaction_datetime = record.loading_datetime.Value;
                                    destinationStockState.quality_sampling_id = record.quality_sampling_id ?? qs2?.id;
                                }

                                // Get previous destination stock state
                                var prevDestinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                    TransactionRecord.destination_location_id, TransactionRecord.loading_datetime.Value);
                                logger.Debug(db.LastCommand);

                                destinationStockState.qty_opening = prevDestinationStockState?.qty_closing ?? 0;
                                destinationStockState.qty_in = record?.loading_quantity ?? 0;
                                destinationStockState.qty_out = 0;
                                destinationStockState.qty_closing = (destinationStockState.qty_opening ?? 0)
                                    + (destinationStockState.qty_in ?? 0);

                                if (string.IsNullOrEmpty(destinationStockState.id))
                                {
                                    destinationStockState.id = Guid.NewGuid().ToString("N").ToLower();
                                    await db.InsertAsync(destinationStockState);
                                }
                                else
                                {
                                    destinationStockState.modified_by = record.modified_by ?? record.created_by;
                                    destinationStockState.modified_on = DateTime.Now;
                                    await db.UpdateAsync(destinationStockState);
                                }
                                logger.Debug(db.LastCommand);

                                // Modify all subsequent stock state
                                var nextDestinationStockStates = await db.FetchAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime > @1",
                                    record.destination_location_id, record.loading_datetime.Value);
                                logger.Debug(db.LastCommand);

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
                                            qty_opening = nextDestinationStockState.qty_closing ?? 0;
                                            logger.Debug(db.LastCommand);
                                        }
                                    }
                                }

                                #endregion
                            }
                            else // Handle deleted transaction
                            {
                                #region Source stock state

                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1", TransactionRecord.id, TransactionRecord.source_location_id);
                                logger.Debug(db.LastCommand);

                                if (sourceStockState != null)
                                {
                                    sourceStockState.transaction_datetime = TransactionRecord.loading_datetime.Value;

                                    // Get previous source stock state
                                    var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        record.source_location_id, record.loading_datetime.Value);
                                    logger.Debug(db.LastCommand);

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
                                        record.source_location_id, record.loading_datetime.Value);
                                    logger.Debug(db.LastCommand);

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

                                #endregion

                                #region Destination stock state

                                var destinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, TransactionRecord.destination_location_id);
                                logger.Debug(db.LastCommand);

                                if (destinationStockState != null)
                                {
                                    destinationStockState.transaction_datetime = record.loading_datetime.Value;

                                    // Get previous destination stock state
                                    var prevDestinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        TransactionRecord.destination_location_id, TransactionRecord.loading_datetime.Value);
                                    logger.Debug(db.LastCommand);

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
                                        record.destination_location_id, record.loading_datetime.Value);
                                    logger.Debug(db.LastCommand);

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
                                                qty_opening = nextDestinationStockState.qty_closing ?? 0;
                                                logger.Debug(db.LastCommand);
                                            }
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

        public static async Task<StandardResult> UpdateStockStateAnalyte(string ConnectionString, processing_transaction TransactionRecord)
        {
            var result = new StandardResult();
            logger.Trace($"UpdateStockStateAnalyte; ProcessingTransaction; TransactionRecord.id = {0}", TransactionRecord.id);

            try
            {
                var db = new Database(ConnectionString, new PostgreSQLDatabaseProvider());
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        if (TransactionRecord != null && TransactionRecord.loading_datetime != null)
                        {
                            var record = await db.FirstOrDefaultAsync<processing_transaction>(
                                "WHERE id = @0", TransactionRecord.id);
                            logger.Debug(db.LastCommand);

                            if (record != null)
                            {
                                #region Get quality sampling

                                var sourceQualitySampling = await db.FirstOrDefaultAsync<quality_sampling>(
                                    " WHERE stock_location_id = @0 "
                                    + " AND sampling_datetime <= @1 "
                                    + " ORDER BY sampling_datetime DESC ",
                                    record.source_location_id,
                                    record.loading_datetime);
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
                                    + " AND sampling_datetime < @1 "
                                    + " ORDER BY sampling_datetime DESC ",
                                    record.destination_location_id,
                                    record.loading_datetime);
                                logger.Debug(db.LastCommand);

                                var destQSAnalytes = new List<quality_sampling_analyte>();
                                if (destinationQualitySampling != null)
                                {
                                    destQSAnalytes = await db.FetchAsync<quality_sampling_analyte>(
                                        "WHERE quality_sampling_id = @0", 
                                        destinationQualitySampling.id);
                                }

                                #endregion

                                #region Get latest quality data

                                // Source stock state
                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    " WHERE transaction_datetime < @0 AND stock_location_id = @1 " +
                                    " ORDER BY transaction_datetime DESC ",
                                    record.loading_datetime, record.source_location_id);
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
                                    record.loading_datetime, record.destination_location_id);
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
                                    record.id, record.source_location_id);
                                logger.Debug(db.LastCommand);

                                // Get from current destination stock state
                                var currentDestinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    " WHERE transaction_id = @0 AND stock_location_id = @1 ",
                                    record.id, record.destination_location_id);
                                logger.Debug(db.LastCommand);

                                #endregion

                                logger.Debug($"sourceStockState = {JsonConvert.SerializeObject(sourceStockState)}");
                                logger.Debug($"srcQSAnalytes = {JsonConvert.SerializeObject(srcQSAnalytes)}");
                                logger.Debug($"srcStockStateAnalytes = {JsonConvert.SerializeObject(srcStockStateAnalytes)}");

                                logger.Debug($"destinationStockState = {JsonConvert.SerializeObject(destinationStockState)}");
                                logger.Debug($"destQSAnalytes = {JsonConvert.SerializeObject(destQSAnalytes)}");
                                logger.Debug($"destStockStateAnalytes = {JsonConvert.SerializeObject(destStockStateAnalytes)}");

                                #region Calculate destination quality sampling

                                if (srcStockStateAnalytes != null && srcStockStateAnalytes.Count > 0
                                    && currentDestinationStockState != null)
                                {
                                    if (destStockStateAnalytes != null && destStockStateAnalytes.Count > 0)
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
                                                //analyte_value = srcStockStateAnalyte.analyte_value ?? 0;
                                                analyte_value = srcStockStateAnalyte.weighted_value ?? (srcStockStateAnalyte.analyte_value ?? 0);
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
                                                decimal wa_value = srcStockStateAnalyte.weighted_value ?? (srcStockStateAnalyte.analyte_value ?? 0);
                                                decimal analyte_value = srcStockStateAnalyte.weighted_value ?? (srcStockStateAnalyte.analyte_value ?? 0);

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
                                                    analyte_value = analyte_value,
                                                    weighted_value = wa_value
                                                };

                                                await db.InsertAsync(ssa);

                                                logger.Debug($"Destination stock_state_analyte = {JsonConvert.SerializeObject(ssa)}");
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

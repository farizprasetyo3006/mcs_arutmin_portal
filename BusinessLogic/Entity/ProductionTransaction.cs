using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class ProductionTransaction: ServiceRepository<production_transaction, vw_production_transaction>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public ProductionTransaction(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public static async Task<StandardResult> UpdateStockState(string ConnectionString, production_transaction TransactionRecord)
        {
            var result = new StandardResult();
            logger.Trace($"UpdateStockState; ProductionTransaction; TransactionRecord.id = {0}", TransactionRecord.id);

            try
            {
                await Task.Delay(1000);

                var db = new Database(ConnectionString, new PostgreSQLDatabaseProvider());
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        if (TransactionRecord != null && TransactionRecord.loading_datetime != null)
                        {
                            var transactionDateTime = TransactionRecord.loading_datetime ?? 
                                TransactionRecord.unloading_datetime;

                            var record = await db.FirstOrDefaultAsync<production_transaction>(
                                "WHERE id = @0", TransactionRecord.id);

                            if(record != null)
                            {
                                #region Latest quality sampling

                                var qualitySampling = await db.FirstOrDefaultAsync<quality_sampling>(
                                    " WHERE stock_location_id = @0 "
                                    + " AND sampling_datetime <= @1 "
                                    + " ORDER BY sampling_datetime DESC ",
                                    TransactionRecord.source_location_id,
                                    transactionDateTime);

                                logger.Debug(db.LastCommand);
                                #endregion

                                #region Source stock state

                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, TransactionRecord.source_location_id);
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
                                        transaction_datetime = transactionDateTime,
                                        stock_location_id = TransactionRecord.source_location_id,
                                        product_out_id = TransactionRecord.product_id,
                                        quality_sampling_id = TransactionRecord.quality_sampling_id ?? qualitySampling?.id
                                    };
                                }
                                else
                                {
                                    sourceStockState.transaction_datetime = transactionDateTime;
                                    sourceStockState.quality_sampling_id = TransactionRecord.quality_sampling_id ?? qualitySampling?.id;
                                }
                                logger.Debug(db.LastCommand);

                                // Get previous source stock state
                                var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                    TransactionRecord.source_location_id, transactionDateTime);

                                sourceStockState.qty_opening = prevSourceStockState?.qty_closing ?? 0;
                                sourceStockState.qty_in = 0;
                                sourceStockState.qty_out = record.loading_quantity;
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
                                    TransactionRecord.source_location_id, transactionDateTime);
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
                                        transaction_datetime = transactionDateTime,
                                        stock_location_id = TransactionRecord.destination_location_id,
                                        product_in_id = TransactionRecord.product_id,
                                        quality_sampling_id = TransactionRecord.quality_sampling_id ?? qualitySampling?.id
                                    };
                                }
                                else
                                {
                                    destinationStockState.transaction_datetime = transactionDateTime;
                                    destinationStockState.quality_sampling_id = TransactionRecord.quality_sampling_id ?? qualitySampling?.id;
                                }
                                logger.Debug(db.LastCommand);

                                // Get previous destination stock state
                                var prevDestinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                    TransactionRecord.destination_location_id, transactionDateTime);

                                destinationStockState.qty_opening = prevDestinationStockState?.qty_closing ?? 0;
                                destinationStockState.qty_in = record.loading_quantity;
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
                                    TransactionRecord.destination_location_id, transactionDateTime);
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
                                    sourceStockState.transaction_datetime = transactionDateTime;

                                    // Get previous source stock state
                                    var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        TransactionRecord.source_location_id, TransactionRecord.unloading_datetime);

                                    sourceStockState.qty_opening = prevSourceStockState?.qty_closing ?? 0;
                                    sourceStockState.qty_in = 0;
                                    sourceStockState.qty_out = 0;
                                    sourceStockState.qty_closing = sourceStockState.qty_opening;
                                    sourceStockState.modified_by = record.modified_by ?? record.created_by;
                                    sourceStockState.modified_on = DateTime.Now;
                                    await db.UpdateAsync(sourceStockState);

                                    logger.Debug(db.LastCommand);

                                    // Modify all subsequent stock state
                                    var nextSourceStockStates = await db.FetchAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime > @1",
                                        TransactionRecord.source_location_id, transactionDateTime);
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
                                    destinationStockState.transaction_datetime = transactionDateTime;

                                    // Get previous destination stock state
                                    var prevDestinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        TransactionRecord.destination_location_id, TransactionRecord.unloading_datetime);

                                    destinationStockState.qty_opening = prevDestinationStockState?.qty_closing ?? 0;
                                    destinationStockState.qty_in = 0;
                                    destinationStockState.qty_out = 0;
                                    destinationStockState.qty_closing = destinationStockState.qty_opening;
                                    destinationStockState.modified_by = record.modified_by ?? record.created_by;
                                    destinationStockState.modified_on = DateTime.Now;
                                    await db.UpdateAsync(destinationStockState);
                                    logger.Debug(db.LastCommand);

                                    // Modify all subsequent stock state
                                    var nextDestinationStockStates = await db.FetchAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime > @1",
                                        TransactionRecord.destination_location_id, transactionDateTime);
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
                                    logger.Debug(db.LastCommand);
                                }

                                #endregion

                                var sql = Sql.Builder.Append(" DELETE FROM stock_state ");
                                sql.Append(" WHERE transaction_id = @0 ", TransactionRecord.id);
                                await db.ExecuteAsync(sql);
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

        public static async Task<StandardResult> UpdateStockStateAnalyte(string ConnectionString, production_transaction TransactionRecord)
        {
            var result = new StandardResult();
            logger.Trace($"UpdateStockStateAnalyte; ProductionTransaction; TransactionRecord.id = {0}", TransactionRecord.id);

            try
            {
                await Task.Delay(1000);

                var db = new Database(ConnectionString, new PostgreSQLDatabaseProvider());
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        if (TransactionRecord != null && TransactionRecord.loading_datetime != null)
                        {
                            var transactionDateTime = TransactionRecord.loading_datetime ??
                                TransactionRecord.unloading_datetime;

                            var record = await db.FirstOrDefaultAsync<production_transaction>(
                                "WHERE id = @0", TransactionRecord.id);
                            logger.Debug(db.LastCommand);

                            if (record != null)
                            {
                                #region Source stock state

                                // Get from quality sampling
                                var sourceQualitySampling = await db.FirstOrDefaultAsync<quality_sampling>(
                                    " WHERE stock_location_id = @0 "
                                    + " AND sampling_datetime <= @1 "
                                    + " ORDER BY sampling_datetime DESC ",
                                    TransactionRecord.source_location_id,
                                    transactionDateTime);
                                var srcQSAnalytes = new List<quality_sampling_analyte>();
                                if (sourceQualitySampling != null)
                                {
                                    srcQSAnalytes = await db.FetchAsync<quality_sampling_analyte>(
                                        "WHERE quality_sampling_id = @0", sourceQualitySampling.id
                                        );
                                }

                                #endregion

                                #region Get destination quality sampling

                                var destinationQualitySampling = await db.FirstOrDefaultAsync<quality_sampling>(
                                    " WHERE stock_location_id = @0 "
                                    + " AND sampling_datetime <= @1 "
                                    + " ORDER BY sampling_datetime DESC ",
                                    TransactionRecord.destination_location_id,
                                    transactionDateTime);
                                logger.Debug(db.LastCommand);

                                var destQSAnalytes = new List<quality_sampling_analyte>();
                                if (destinationQualitySampling != null)
                                {
                                    destQSAnalytes = await db.FetchAsync<quality_sampling_analyte>(
                                        "WHERE quality_sampling_id = @0", destinationQualitySampling.id
                                        );
                                }

                                #endregion

                                #region Get latest quality data

                                // Get from source stock state
                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    " WHERE transaction_datetime < @0 AND stock_location_id = @1 " +
                                    " ORDER BY transaction_datetime DESC ",
                                    transactionDateTime, TransactionRecord.source_location_id);
                                var srcStockStateAnalytes = new List<stock_state_analyte>();
                                if (sourceStockState != null)
                                {
                                    srcStockStateAnalytes = await db.FetchAsync<stock_state_analyte>(
                                            "WHERE stock_state_id = @0", sourceStockState.id);
                                }

                                // Get from destination stock state
                                var destinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    " WHERE transaction_datetime < @0 AND stock_location_id = @1 " +
                                    " ORDER BY transaction_datetime DESC ",
                                    transactionDateTime, TransactionRecord.destination_location_id);
                                var destStockStateAnalytes = new List<stock_state_analyte>();
                                if (destinationStockState != null)
                                {
                                    destStockStateAnalytes = await db.FetchAsync<stock_state_analyte>(
                                            "WHERE stock_state_id = @0", destinationStockState.id);
                                }

                                // Get from current destination stock state
                                var currentStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    " WHERE transaction_id = @0 AND stock_location_id = @1 ",
                                    TransactionRecord.id, TransactionRecord.destination_location_id);

                                #endregion

                                logger.Debug($"sourceStockState = {JsonConvert.SerializeObject(sourceStockState)}");
                                logger.Debug($"srcQSAnalytes = {JsonConvert.SerializeObject(srcQSAnalytes)}");
                                logger.Debug($"srcStockStateAnalytes = {JsonConvert.SerializeObject(srcStockStateAnalytes)}");

                                logger.Debug($"destinationStockState = {JsonConvert.SerializeObject(destinationStockState)}");
                                logger.Debug($"destQSAnalytes = {JsonConvert.SerializeObject(destQSAnalytes)}");
                                logger.Debug($"destStockStateAnalytes = {JsonConvert.SerializeObject(destStockStateAnalytes)}");

                                #region Calculate destination quality sampling

                                if(destinationStockState != null)
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
                                            if (srcStockStateAnalyte != null && (currentStockState.qty_closing ?? 0) > 0)
                                            {
                                                logger.Debug($"srcStockStateAnalyte = {JsonConvert.SerializeObject(srcStockStateAnalyte)}");

                                                // weighted-average
                                                analyte_value = srcStockStateAnalyte.analyte_value ?? -1;
                                                wa_value =
                                                    ((currentStockState.qty_opening ?? 0) * (destStockStateAnalyte.weighted_value ?? 0)
                                                    + (currentStockState.qty_in ?? 0) * srcStockStateAnalyte.analyte_value)
                                                    / currentStockState.qty_closing ?? 0;
                                            }
                                            else
                                            {
                                                var srcQSAnalyte = srcQSAnalytes.FirstOrDefault(
                                                        o => o.analyte_id == destStockStateAnalyte.analyte_id
                                                    );
                                                if (srcQSAnalyte != null && (destinationStockState.qty_closing ?? 0) > 0)
                                                {
                                                    logger.Debug($"srcQSAnalyte = {JsonConvert.SerializeObject(srcQSAnalyte)}");

                                                    // weighted-average
                                                    analyte_value = srcQSAnalyte.analyte_value ?? -1;
                                                    wa_value =
                                                        ((currentStockState.qty_opening ?? 0) * (destStockStateAnalyte.weighted_value ?? 0)
                                                        + (currentStockState.qty_in ?? 0) * srcQSAnalyte.analyte_value)
                                                        / currentStockState.qty_closing ?? 0;
                                                }
                                            }

                                            if (analyte_value >= 0 && wa_value >= 0)
                                            {
                                                var ssa = await db.FirstOrDefaultAsync<stock_state_analyte>(
                                                    "WHERE stock_state_id = @0 AND analyte_id = @1",
                                                    currentStockState.id, destStockStateAnalyte.analyte_id);
                                                if (ssa == null)
                                                {
                                                    ssa = new stock_state_analyte()
                                                    {
                                                        id = Guid.NewGuid().ToString("N").ToLower(),
                                                        created_by = currentStockState.created_by,
                                                        created_on = currentStockState.created_on,
                                                        organization_id = currentStockState.organization_id,
                                                        owner_id = currentStockState.owner_id,
                                                        is_active = true,
                                                        stock_state_id = currentStockState.id,
                                                        analyte_id = destStockStateAnalyte.analyte_id,
                                                        analyte_value = analyte_value,
                                                        weighted_value = wa_value
                                                    };

                                                    await db.InsertAsync(ssa);
                                                }
                                                else
                                                {
                                                    ssa.modified_by = currentStockState.modified_by;
                                                    ssa.modified_on = currentStockState.modified_on;
                                                    ssa.analyte_value = analyte_value;
                                                    ssa.weighted_value = wa_value;

                                                    await db.UpdateAsync(ssa);
                                                }

                                                logger.Debug($"stock_state_analyte = {JsonConvert.SerializeObject(ssa)}");
                                                logger.Debug(db.LastCommand);
                                            }
                                        }
                                    }
                                    else if (destQSAnalytes != null && destQSAnalytes.Count > 0)
                                    {
                                        foreach (var destQSAnalyte in destQSAnalytes)
                                        {
                                            logger.Debug($"FOR destQSAnalyte = {JsonConvert.SerializeObject(destQSAnalyte)}");

                                            decimal wa_value = -1;
                                            decimal analyte_value = -1;
                                            var srcStockStateAnalyte = srcStockStateAnalytes.FirstOrDefault(
                                                    o => o.analyte_id == destQSAnalyte.analyte_id
                                                );
                                            if (srcStockStateAnalyte != null && (currentStockState.qty_closing ?? 0) > 0)
                                            {
                                                logger.Debug($"srcStockStateAnalyte = {JsonConvert.SerializeObject(srcStockStateAnalyte)}");

                                                // weighted-average
                                                analyte_value = srcStockStateAnalyte.analyte_value ?? -1;
                                                wa_value =
                                                    ((currentStockState.qty_opening ?? 0) * destQSAnalyte.analyte_value
                                                    + (currentStockState.qty_in ?? 0) * srcStockStateAnalyte.analyte_value)
                                                    / currentStockState.qty_closing ?? 0;
                                            }
                                            else
                                            {
                                                var srcQSAnalyte = srcQSAnalytes.FirstOrDefault(
                                                        o => o.analyte_id == destQSAnalyte.analyte_id
                                                    );
                                                if (srcQSAnalyte != null && (currentStockState.qty_closing ?? 0) > 0)
                                                {
                                                    logger.Debug($"srcQSAnalyte = {JsonConvert.SerializeObject(srcQSAnalyte)}");

                                                    // weighted-average
                                                    analyte_value = srcQSAnalyte.analyte_value ?? -1;
                                                    wa_value =
                                                        ((currentStockState.qty_opening ?? 0) * destQSAnalyte.analyte_value
                                                        + (currentStockState.qty_in ?? 0) * srcQSAnalyte.analyte_value)
                                                        / currentStockState.qty_closing ?? 0;
                                                }
                                            }

                                            if (analyte_value >= 0 && wa_value >= 0)
                                            {
                                                var ssa = await db.FirstOrDefaultAsync<stock_state_analyte>(
                                                    "WHERE stock_state_id = @0 AND analyte_id = @1",
                                                    currentStockState.id, destQSAnalyte.analyte_id);
                                                if (ssa == null)
                                                {
                                                    ssa = new stock_state_analyte()
                                                    {
                                                        id = Guid.NewGuid().ToString("N").ToLower(),
                                                        created_by = currentStockState.created_by,
                                                        created_on = currentStockState.created_on,
                                                        organization_id = currentStockState.organization_id,
                                                        owner_id = currentStockState.owner_id,
                                                        is_active = true,
                                                        stock_state_id = currentStockState.id,
                                                        analyte_id = destQSAnalyte.analyte_id,
                                                        analyte_value = analyte_value,
                                                        weighted_value = wa_value
                                                    };

                                                    await db.InsertAsync(ssa);
                                                }
                                                else
                                                {
                                                    ssa.modified_by = currentStockState.modified_by;
                                                    ssa.modified_on = currentStockState.modified_on;
                                                    ssa.analyte_value = analyte_value;
                                                    ssa.weighted_value = wa_value;

                                                    await db.UpdateAsync(ssa);
                                                }

                                                logger.Debug($"stock_state_analyte = {JsonConvert.SerializeObject(ssa)}");
                                                logger.Debug(db.LastCommand);
                                            }
                                        }
                                    }
                                    else if(srcStockStateAnalytes != null && srcStockStateAnalytes.Count > 0)
                                    {
                                        foreach (var srcStockStateAnalyte in srcStockStateAnalytes)
                                        {
                                            logger.Debug($"srcStockStateAnalyte = {JsonConvert.SerializeObject(srcStockStateAnalyte)}");

                                            var ssa = await db.FirstOrDefaultAsync<stock_state_analyte>(
                                                "WHERE stock_state_id = @0 AND analyte_id = @1",
                                                currentStockState.id, srcStockStateAnalyte.analyte_id);
                                            if (ssa == null)
                                            {
                                                ssa = new stock_state_analyte()
                                                {
                                                    id = Guid.NewGuid().ToString("N").ToLower(),
                                                    created_by = currentStockState.created_by,
                                                    created_on = currentStockState.created_on,
                                                    organization_id = currentStockState.organization_id,
                                                    owner_id = currentStockState.owner_id,
                                                    is_active = true,
                                                    stock_state_id = currentStockState.id,
                                                    analyte_id = srcStockStateAnalyte.analyte_id,
                                                    analyte_value = srcStockStateAnalyte.analyte_value,
                                                    weighted_value = srcStockStateAnalyte.analyte_value
                                                };

                                                await db.InsertAsync(ssa);
                                            }

                                            logger.Debug($"stock_state_analyte = {JsonConvert.SerializeObject(ssa)}");
                                            logger.Debug(db.LastCommand);
                                        }
                                    }
                                }
                                else
                                {
                                    logger.Error("destinationStockState is null");
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

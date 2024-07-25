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
    public partial class ShippingTransaction: ServiceRepository<shipping_transaction, vw_shipping_transaction>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public ShippingTransaction(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public static async Task<StandardResult> UpdateStockStateLoading(string ConnectionString, shipping_transaction_detail TransactionRecord)
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
                        if (TransactionRecord != null)
                        {
                            var shippingTransaction = await db.FirstOrDefaultAsync<shipping_transaction>(
                                "WHERE id = @0", TransactionRecord.shipping_transaction_id);
                            if(shippingTransaction != null)
                            {
                                var record = await db.FirstOrDefaultAsync<shipping_transaction_detail>(
                                    "WHERE id = @0", TransactionRecord.id);

                                if (record != null)
                                {
                                    var source_location_id = shippingTransaction.is_loading ?
                                            TransactionRecord.detail_location_id : shippingTransaction.ship_location_id;

                                    #region Latest quality sampling

                                    var qualitySampling = await db.FirstOrDefaultAsync<quality_sampling>(
                                        " WHERE stock_location_id = @0 "
                                        + " AND sampling_datetime <= @1 "
                                        + " ORDER BY sampling_datetime DESC ",
                                        source_location_id,
                                        TransactionRecord.end_datetime.Value);
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
                                            transaction_datetime = TransactionRecord.end_datetime.Value,
                                            stock_location_id = source_location_id,
                                            product_out_id = shippingTransaction.product_id,
                                            quality_sampling_id = shippingTransaction.quality_sampling_id ?? qualitySampling?.id
                                        };
                                    }
                                    else
                                    {
                                        sourceStockState.transaction_datetime = TransactionRecord.end_datetime.Value;
                                        sourceStockState.quality_sampling_id = shippingTransaction.quality_sampling_id ?? qualitySampling?.id;
                                    }
                                    logger.Debug(db.LastCommand);

                                    // Get previous source stock state
                                    var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        source_location_id, TransactionRecord.end_datetime.Value);

                                    sourceStockState.qty_opening = prevSourceStockState?.qty_closing ?? 0;
                                    sourceStockState.qty_in = 0;
                                    sourceStockState.qty_out = record?.quantity ?? 0;
                                    sourceStockState.qty_closing = (sourceStockState.qty_opening ?? 0)
                                        - (sourceStockState.qty_out ?? 0);
                                    sourceStockState.qty_adjustment = sourceStockState.qty_closing
                                        - (record.final_quantity ?? 0);
                                    sourceStockState.qty_closing = record.final_quantity ?? 0;

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
                                        source_location_id, TransactionRecord.end_datetime.Value);
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

                                    var destination_location_id = shippingTransaction.is_loading ?
                                            shippingTransaction.ship_location_id : TransactionRecord.detail_location_id;
                                    var destinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE transaction_id = @0 AND stock_location_id = @1",
                                        TransactionRecord.id, destination_location_id);
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
                                            transaction_datetime = TransactionRecord.end_datetime.Value,
                                            stock_location_id = destination_location_id,
                                            product_in_id = shippingTransaction.product_id,
                                            quality_sampling_id = shippingTransaction.quality_sampling_id ?? qualitySampling?.id
                                        };
                                    }
                                    else
                                    {
                                        destinationStockState.transaction_datetime = TransactionRecord.end_datetime.Value;
                                        destinationStockState.quality_sampling_id = shippingTransaction.quality_sampling_id ?? qualitySampling?.id;
                                    }
                                    logger.Debug(db.LastCommand);

                                    // Get previous destination stock state
                                    var prevDestinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        destination_location_id, TransactionRecord.end_datetime.Value);

                                    destinationStockState.qty_opening = prevDestinationStockState?.qty_closing ?? 0;
                                    destinationStockState.qty_in = record?.quantity ?? 0;
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
                                        destination_location_id, TransactionRecord.end_datetime.Value);
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

                                    var source_location_id = shippingTransaction.is_loading ?
                                            TransactionRecord.detail_location_id : shippingTransaction.ship_location_id;
                                    var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE transaction_id = @0 AND stock_location_id = @1",
                                        TransactionRecord.id, source_location_id);
                                    if (sourceStockState != null)
                                    {
                                        sourceStockState.transaction_datetime = TransactionRecord.end_datetime.Value;

                                        // Get previous source stock state
                                        var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                            "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                            source_location_id, TransactionRecord.end_datetime.Value);

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
                                            source_location_id, TransactionRecord.end_datetime.Value);
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

                                    var destination_location_id = shippingTransaction.is_loading ?
                                            shippingTransaction.ship_location_id : TransactionRecord.detail_location_id;

                                    var destinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE transaction_id = @0 AND stock_location_id = @1",
                                        TransactionRecord.id, destination_location_id);
                                    if (destinationStockState != null)
                                    {
                                        destinationStockState.transaction_datetime = TransactionRecord.end_datetime.Value;

                                        // Get previous destination stock state
                                        var prevDestinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                            "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                            destination_location_id, TransactionRecord.end_datetime.Value);

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
                                            destination_location_id, TransactionRecord.end_datetime.Value);
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

                                    #endregion
                                }

                                tx.Complete();
                                result.Success = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Message = ex.InnerException?.Message ?? ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        public static async Task<StandardResult> UpdateStockStateUnloading(string ConnectionString, shipping_transaction TransactionRecord)
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
                        if (TransactionRecord != null)
                        {
                            var record = await db.FirstOrDefaultAsync<shipping_transaction>(
                                "WHERE id = @0", TransactionRecord.id);

                            if (record != null)
                            {
                                var source_location_id = record.ship_location_id;

                                #region Latest quality sampling

                                var qualitySampling = await db.FirstOrDefaultAsync<quality_sampling>(
                                    " WHERE stock_location_id = @0 "
                                    + " AND sampling_datetime <= @1 "
                                    + " ORDER BY sampling_datetime DESC ",
                                    source_location_id,
                                    TransactionRecord.end_datetime.Value);
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
                                        transaction_datetime = TransactionRecord.end_datetime.Value,
                                        stock_location_id = source_location_id,
                                        product_out_id = record.product_id,
                                        quality_sampling_id = record.quality_sampling_id ?? qualitySampling?.id
                                    };
                                }
                                else
                                {
                                    sourceStockState.transaction_datetime = TransactionRecord.end_datetime.Value;
                                    sourceStockState.quality_sampling_id = record.quality_sampling_id ?? qualitySampling?.id;
                                }
                                logger.Debug(db.LastCommand);

                                // Get previous source stock state
                                var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                    source_location_id, TransactionRecord.end_datetime.Value);

                                sourceStockState.qty_opening = prevSourceStockState?.qty_closing ?? 0;
                                sourceStockState.qty_in = 0;
                                sourceStockState.qty_out = record?.original_quantity ?? 0;
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
                                    source_location_id, TransactionRecord.end_datetime.Value);
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

                                var destination_location_id = record.destination_location_id;
                                var destinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, destination_location_id);
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
                                        transaction_datetime = TransactionRecord.end_datetime.Value,
                                        stock_location_id = destination_location_id,
                                        product_in_id = record.product_id,
                                        quality_sampling_id = record.quality_sampling_id ?? qualitySampling?.id
                                    };
                                }
                                else
                                {
                                    destinationStockState.transaction_datetime = TransactionRecord.end_datetime.Value;
                                    destinationStockState.quality_sampling_id = record.quality_sampling_id ?? qualitySampling?.id;
                                }
                                logger.Debug(db.LastCommand);

                                // Get previous destination stock state
                                var prevDestinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                    destination_location_id, TransactionRecord.end_datetime.Value);

                                destinationStockState.qty_opening = prevDestinationStockState?.qty_closing ?? 0;
                                destinationStockState.qty_in = record?.original_quantity ?? 0;
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
                                    destination_location_id, TransactionRecord.end_datetime.Value);
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

                                var source_location_id = record.ship_location_id;
                                var sourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, source_location_id);
                                if (sourceStockState != null)
                                {
                                    sourceStockState.transaction_datetime = TransactionRecord.end_datetime.Value;

                                    // Get previous source stock state
                                    var prevSourceStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        source_location_id, TransactionRecord.end_datetime.Value);

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
                                        source_location_id, TransactionRecord.end_datetime.Value);
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

                                var destination_location_id = record.destination_location_id;
                                var destinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                    "WHERE transaction_id = @0 AND stock_location_id = @1",
                                    TransactionRecord.id, destination_location_id);
                                if (destinationStockState != null)
                                {
                                    destinationStockState.transaction_datetime = TransactionRecord.end_datetime.Value;

                                    // Get previous destination stock state
                                    var prevDestinationStockState = await db.FirstOrDefaultAsync<stock_state>(
                                        "WHERE stock_location_id = @0 AND transaction_datetime < @1 ORDER BY transaction_datetime DESC",
                                        destination_location_id, TransactionRecord.end_datetime.Value);

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
                                        destination_location_id, TransactionRecord.end_datetime.Value);
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

                                #endregion
                            }

                            tx.Complete();
                            result.Success = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Message = ex.InnerException?.Message ?? ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }
    }
}

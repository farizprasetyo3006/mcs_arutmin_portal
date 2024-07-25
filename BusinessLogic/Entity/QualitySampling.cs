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
    public partial class QualitySampling: ServiceRepository<quality_sampling, vw_quality_sampling>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public QualitySampling(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public static async Task<StandardResult> UpdateStockStateAnalyte(string ConnectionString, quality_sampling TransactionRecord)
        {
            var result = new StandardResult();
            var updateClosestStockState = true;
            quality_sampling record = null;
            logger.Trace($"UpdateStockStateAnalyte; TransactionRecord.id = {0}", TransactionRecord.id);

            try
            {
                var db = new Database(ConnectionString, new PostgreSQLDatabaseProvider());
                using (var tx = db.GetTransaction())
                {
                    try
                    {
                        if (TransactionRecord != null)
                        {
                            record = await db.FirstOrDefaultAsync<quality_sampling>(
                                "WHERE id = @0", TransactionRecord.id);
                            logger.Debug(db.LastCommand);

                            if(record != null)
                            {
                                var samplingType = await db.FirstOrDefaultAsync<master_list>(
                                    "WHERE id = @0", record.sampling_type_id);
                                logger.Debug(db.LastCommand);

                                var _samplingType = samplingType?.item_name?.ToLower() ?? "";
                                if (_samplingType.Contains("crushed") || _samplingType.Contains("channel"))
                                    updateClosestStockState = false;

                                if (updateClosestStockState)
                                {
                                    // Get closest stock state
                                    var sql = Sql.Builder.Append($"SELECT * FROM stock_state");
                                    sql.Append("WHERE stock_location_id = @0", record.stock_location_id);
                                    sql.Append("AND transaction_datetime >= @0", record.sampling_datetime);
                                    sql.Append("ORDER BY transaction_datetime ASC");
                                    var firstStockState = await db.FirstOrDefaultAsync<stock_state>(sql);
                                    logger.Debug(db.LastCommand);

                                    if (firstStockState != null)
                                    {
                                        logger.Debug("Update next first stock state");

                                        firstStockState.quality_sampling_id = record.id;
                                        await db.UpdateAsync(firstStockState);
                                        logger.Debug(db.LastCommand);

                                        var qsAnalytes = await db.FetchAsync<quality_sampling_analyte>(
                                            "WHERE quality_sampling_id = @0", record.id);
                                        logger.Debug(db.LastCommand);
                                        foreach (var qsAnalyte in qsAnalytes)
                                        {
                                            var ssa = await db.FirstOrDefaultAsync<stock_state_analyte>(
                                                "WHERE stock_state_id = @0 AND analyte_id = @1",
                                                    firstStockState.id, qsAnalyte.analyte_id
                                                );
                                            logger.Debug(db.LastCommand);

                                            if (ssa == null)
                                            {
                                                ssa = new stock_state_analyte();
                                                ssa.InjectFrom(firstStockState);

                                                ssa.id = Guid.NewGuid().ToString("N").ToLower();
                                                ssa.entity_id = null;
                                                ssa.stock_state_id = firstStockState.id;
                                                ssa.analyte_id = qsAnalyte.analyte_id;
                                                ssa.uom_id = qsAnalyte.uom_id;
                                                ssa.analyte_value = qsAnalyte.analyte_value;
                                                ssa.weighted_value = qsAnalyte.analyte_value;

                                                await db.InsertAsync(ssa);
                                                logger.Debug(db.LastCommand);
                                            }
                                            else
                                            {
                                                ssa.uom_id = qsAnalyte.uom_id;
                                                ssa.analyte_value = qsAnalyte.analyte_value;
                                                ssa.weighted_value = qsAnalyte.analyte_value;

                                                await db.UpdateAsync(ssa);
                                                logger.Debug(db.LastCommand);
                                            }
                                        }
                                    }
                                }
                            }

                            tx.Complete();
                            result.Success = true;
                        }
                        else
                        {
                            result.Message = "Transaction record is null";
                            logger.Debug(result.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        logger.Debug(db.LastCommand);
                        result.Message = ex.InnerException?.Message ?? ex.Message;
                    }
                }

                if(result.Success && record != null && !updateClosestStockState)
                {
                    logger.Debug("Update next stock states");

                    try
                    {
                        var sql = Sql.Builder.Append("SELECT * FROM quality_sampling");
                        sql.Append("WHERE stock_location_id = @0", record.stock_location_id);
                        sql.Append("AND sampling_datetime > @0", record.sampling_datetime);
                        sql.Append("ORDER BY sampling_datetime ASC");
                        var newQS = await db.FirstOrDefaultAsync<quality_sampling>(sql);
                        logger.Debug(db.LastCommand);

                        var endDate = new DateTime(9999, 1, 1);
                        if (newQS != null)
                        {
                            endDate = newQS.sampling_datetime;
                        }

                        var productionTransactions = await db.FetchAsync<production_transaction>(
                            "WHERE source_location_id = @0 AND transaction_datetime >= @1 AND transaction_datetime < @2",
                            record.stock_location_id, record.sampling_datetime, endDate);
                        if (productionTransactions != null)
                        {
                            foreach (var productionTransaction in productionTransactions.OrderBy(o => o.loading_datetime))
                            {
                                await ProductionTransaction.UpdateStockStateAnalyte(ConnectionString, productionTransaction);
                            }
                        }

                        var haulingTransactions = await db.FetchAsync<hauling_transaction>(
                            "WHERE source_location_id = @0 AND transaction_datetime >= @1 AND transaction_datetime < @2",
                            record.stock_location_id, record.sampling_datetime, endDate);
                        if (haulingTransactions != null)
                        {
                            foreach (var haulingTransaction in haulingTransactions.OrderBy(o => o.loading_datetime))
                            {
                                await HaulingTransaction.UpdateStockStateAnalyte(ConnectionString, haulingTransaction);
                            }
                        }

                        var rehandlingTransactions = await db.FetchAsync<rehandling_transaction>(
                            "WHERE source_location_id = @0 AND transaction_datetime >= @1 AND transaction_datetime < @2",
                            record.stock_location_id, record.sampling_datetime, endDate);
                        if (rehandlingTransactions != null)
                        {
                            foreach (var rehandlingTransaction in rehandlingTransactions.OrderBy(o => o.loading_datetime))
                            {
                                await RehandlingTransaction.UpdateStockStateAnalyte(ConnectionString, rehandlingTransaction);
                            }
                        }

                        var processingTransactions = await db.FetchAsync<processing_transaction>(
                            "WHERE source_location_id = @0 AND transaction_datetime >= @1 AND transaction_datetime < @2",
                            record.stock_location_id, record.sampling_datetime, endDate);
                        if (processingTransactions != null)
                        {
                            foreach (var processingTransaction in processingTransactions.OrderBy(o => o.loading_datetime))
                            {
                                await ProcessingTransaction.UpdateStockStateAnalyte(ConnectionString, processingTransaction);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
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

        public async Task<StandardResult> ApplyToTransactions(string TransactionCategory, string QualitySamplingId,
            List<string> TransactionIds)
        {
            var result = new StandardResult();

            #region Validation

            if(string.IsNullOrEmpty(TransactionCategory) || string.IsNullOrEmpty(QualitySamplingId)
                || TransactionIds?.Count < 0)
            {
                return result;
            }

            #endregion

            var dc = userContext.GetDataContext();
            var db = dc.Database;
            using (var tx = db.GetTransaction())
            {
                try
                {
                    var qs = await db.FirstOrDefaultAsync<quality_sampling>("WHERE id = @0", QualitySamplingId);
                    logger.Debug(db.LastCommand);

                    if (qs != null)
                    {
                        switch (TransactionCategory.ToLower())
                        {
                            case "hauling":
                                #region Hauling

                                foreach (var id in TransactionIds)
                                {
                                    var record = await db.FirstOrDefaultAsync<hauling_transaction>("WHERE id = @0", id);
                                    logger.Debug(db.LastCommand);
                                    if (record != null)
                                    {
                                        record.quality_sampling_id = qs.id;
                                        record.modified_by = userContext.AppUserId;
                                        record.modified_on = DateTime.Now;

                                        await db.UpdateAsync(record);
                                        logger.Debug(db.LastCommand);
                                    }
                                }

                                tx.Complete();
                                result.Success = true;

                                #endregion
                                break;

                            case "processing":
                                #region Processing

                                foreach (var id in TransactionIds)
                                {
                                    var record = await db.FirstOrDefaultAsync<processing_transaction>("WHERE id = @0", id);
                                    logger.Debug(db.LastCommand);
                                    if (record != null)
                                    {
                                        record.quality_sampling_id = qs.id;
                                        record.modified_by = userContext.AppUserId;
                                        record.modified_on = DateTime.Now;

                                        await db.UpdateAsync(record);
                                        logger.Debug(db.LastCommand);
                                    }
                                }

                                tx.Complete();
                                result.Success = true;

                                #endregion
                                break;

                            case "production":
                                #region Production

                                foreach (var id in TransactionIds)
                                {
                                    var record = await db.FirstOrDefaultAsync<production_transaction>("WHERE id = @0", id);
                                    logger.Debug(db.LastCommand);
                                    if (record != null)
                                    {
                                        record.quality_sampling_id = qs.id;
                                        record.modified_by = userContext.AppUserId;
                                        record.modified_on = DateTime.Now;

                                        await db.UpdateAsync(record);
                                        logger.Debug(db.LastCommand);
                                    }
                                }

                                tx.Complete();
                                result.Success = true;

                                #endregion
                                break;

                            case "rehandling":
                                #region Rehandling
                                foreach (var id in TransactionIds)
                                {
                                    var record = await db.FirstOrDefaultAsync<rehandling_transaction>("WHERE id = @0", id);
                                    logger.Debug(db.LastCommand);
                                    if (record != null)
                                    {
                                        record.quality_sampling_id = qs.id;
                                        record.modified_by = userContext.AppUserId;
                                        record.modified_on = DateTime.Now;

                                        await db.UpdateAsync(record);
                                        logger.Debug(db.LastCommand);
                                    }
                                }

                                tx.Complete();
                                result.Success = true;
                                #endregion
                                break;
                        }
                    }
                    else
                    {
                        result.Message = "Quality sampling record is not found.";
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    result.Message = ex.Message;
                }
            }

            return result;
        }

        public async Task<StandardResult> ApplyToProductionTransactions(string QualitySamplingId, 
            List<string> TransactionIds)
        {
            var result = new StandardResult();

            var dc = userContext.GetDataContext();
            var db = dc.Database;
            using(var tx = db.GetTransaction())
            {
                try
                {
                    var qs = await db.FirstOrDefaultAsync<quality_sampling>("WHERE id = @0", QualitySamplingId);
                    logger.Debug(db.LastCommand);

                    if(qs != null)
                    {
                        foreach (var id in TransactionIds)
                        {
                            var record = await db.FirstOrDefaultAsync<production_transaction>("WHERE id = @0", id);
                            logger.Debug(db.LastCommand);
                            if (record != null)
                            {
                                record.quality_sampling_id = qs.id;
                                record.modified_by = userContext.AppUserId;
                                record.modified_on = DateTime.Now;

                                await db.UpdateAsync(record);
                                logger.Debug(db.LastCommand);
                            }
                        }

                        tx.Complete();
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = "Quality sampling record is not found.";
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    result.Message = ex.Message;
                }
            }

            return result;
        }       
    }
}

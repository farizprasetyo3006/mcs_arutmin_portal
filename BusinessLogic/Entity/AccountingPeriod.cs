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
    public partial class AccountingPeriod: ServiceRepository<accounting_period, vw_accounting_period>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public AccountingPeriod(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public async Task<StandardResult> ApplyToTransactions(string Category, string AccountingPeriodId,
            List<string> TransactionIds)
        {
            var result = new StandardResult();

            var dc = userContext.GetDataContext();
            var db = dc.Database;
            using (var tx = db.GetTransaction())
            {
                try
                {
                    var ap1 = await db.FirstOrDefaultAsync<accounting_period>("WHERE id = @0", AccountingPeriodId);
                    logger.Debug(db.LastCommand);

                    if (ap1 != null)
                    {
                        switch (Category.ToLower())
                        {
                            case "hauling":
                                #region Hauling

                                foreach (var id in TransactionIds)
                                {
                                    var record = await db.FirstOrDefaultAsync<hauling_transaction>("WHERE id = @0", id);
                                    logger.Debug(db.LastCommand);
                                    if (record != null)
                                    {
                                        record.accounting_period_id = ap1.id;
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
                                        record.accounting_period_id = ap1.id;
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
                                        record.accounting_period_id = ap1.id;
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
                                        record.accounting_period_id = ap1.id;
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
                        result.Message = "Accounting period record is not found.";
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

        public async Task<StandardResult> ApplyToProductionTransactions(string AccountingPeriodId,
            List<string> TransactionIds)
        {
            var result = new StandardResult();

            var dc = userContext.GetDataContext();
            var db = dc.Database;
            using (var tx = db.GetTransaction())
            {
                try
                {
                    var ap1 = await db.FirstOrDefaultAsync<accounting_period>("WHERE id = @0", AccountingPeriodId);
                    logger.Debug(db.LastCommand);

                    if (ap1 != null)
                    {
                        foreach (var id in TransactionIds)
                        {
                            var record = await db.FirstOrDefaultAsync<production_transaction>("WHERE id = @0", id);
                            logger.Debug(db.LastCommand);
                            if (record != null)
                            {
                                record.accounting_period_id = ap1.id;
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
                        result.Message = "Accounting period record is not found.";
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

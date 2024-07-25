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
    public partial class StockpileState: ServiceRepository<stockpile_state, vw_stockpile_state>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UserContext userContext;

        public StockpileState(UserContext userContext)
            : base(userContext.GetDataContext())
        {
            this.userContext = userContext;
        }

        public StandardResult Update(string StockpileLocationId, string TransactionId, IDatabase _db = null)
        {
            var result = new StandardResult();

            var dc = userContext.GetDataContext();
            var db = _db ?? dc.Database;

            using (var tx = db.GetTransaction())
            {
                try
                {
                    var sql = Sql.Builder.Append("SELECT ufn_update_stockpile_state(@0, @1)",
                        StockpileLocationId, TransactionId);
                    result.Success = db.ExecuteScalar<bool>(sql);
                    logger.Debug(db.LastCommand);

                    tx.Complete();
                    logger.Debug($"Result = {result.Success}");
                }
                catch (Exception ex)
                {
                    logger.Debug(db.LastCommand);
                    logger.Error(ex.ToString());
                    result.Message = ex.Message;
                }
            }

            return result;
        }
    }
}

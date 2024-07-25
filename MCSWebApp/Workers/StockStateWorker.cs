using DataAccess.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using PetaPoco;
using PetaPoco.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Omu.ValueInjecter;

namespace MCSWebApp.Workers
{
    public class StockStateWorker : BackgroundService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration configuration;
        private readonly string connectionString;
        private readonly string stockpileLocationId;
        private readonly string transactionId;

        public StockStateWorker(IConfiguration configuration, 
            string StockpileLocationId, string TransactionId)
        {
            this.configuration = configuration;            
            connectionString = configuration.GetConnectionString("MCS");
            stockpileLocationId = StockpileLocationId;
            transactionId = TransactionId;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.Info("StockStateWorker running at: {time}", DateTimeOffset.Now);

            var t = Task.Factory.StartNew(() => 
            {
                using (var db = DatabaseConfiguration
                    .Build()
                    .UsingConnectionString(connectionString)
                    .UsingProvider<PostgreSQLDatabaseProvider>()
                    .Create())
                {
                    using (var tx = db.GetTransaction())
                    {
                        try
                        {
                            var sql = Sql.Builder.Append("SELECT ufn_update_stockpile_state(@0, @1)",
                                stockpileLocationId, transactionId);
                            var result = db.ExecuteScalar<bool>(sql);
                            logger.Debug(db.LastCommand);

                            tx.Complete();
                            logger.Debug($"Result = {result}");
                        }
                        catch (TaskCanceledException tce)
                        {
                            logger.Error(tce.Message);
                        }
                        catch (Exception ex)
                        {
                            logger.Debug(db.LastCommand);
                            logger.Error(ex.ToString());
                        }
                    }
                }
            });

            await t;
        }
    }
}

using DataAccess.DTO;
using DataAccess.EFCore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MCSWebApp.Scheduler
{
    public class SchedulerService : BackgroundService 
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<SchedulerService> _logger;
        private readonly IConfiguration _configuration;

        public SchedulerService(IServiceScopeFactory serviceScopeFactory, ILogger<SchedulerService> logger, IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _configuration = configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await FetchAndPostData();
                await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
            }
        }
        private async Task FetchAndPostData()
        {
            try
            {
                var data = await FetchData();
                if (data != null)
                {
                    await ProcessData(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in fetching and processing data");
            }
        }

        private List<DateTime> GenerateDateList()
        {
            List<DateTime> dates = new List<DateTime>();
            DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime endDate = DateTime.Now.Date;

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                dates.Add(date);
            }

            return dates;
        }
        private async Task<List<historical_dpr>> FetchData()
        {
            string query = $"select * from get_dpr_data(current_date) where business_unit_name is not null";
            List<historical_dpr> rows = new List<historical_dpr>();
            string sqlDataSource = _configuration.GetConnectionString("MCS");

            using (NpgsqlConnection connection = new NpgsqlConnection(sqlDataSource))
            {
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            historical_dpr row = new historical_dpr
                            {
                                tanggal = reader.GetDateTime(reader.GetOrdinal("tanggal")),
                                entity = reader.GetString(reader.GetOrdinal("entity")),
                                business_unit_name = reader.GetString(reader.GetOrdinal("business_unit_name")),
                                area_name = reader.GetString(reader.GetOrdinal("area_name")),
                                contractor_code = reader.GetString(reader.GetOrdinal("contractor_code")),
                                pit_contractor = reader.GetString(reader.GetOrdinal("pit_contractor")),
                                daily_actual = reader.IsDBNull(reader.GetOrdinal("daily_actual")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("daily_actual")),
                                mtd_actual = reader.IsDBNull(reader.GetOrdinal("mtd_actual")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("mtd_actual")),
                                ytd_actual = reader.IsDBNull(reader.GetOrdinal("ytd_actual")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("ytd_actual")),
                                daily_budget = reader.IsDBNull(reader.GetOrdinal("daily_budget")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("daily_budget")),
                                mtd_budget = reader.IsDBNull(reader.GetOrdinal("mtd_budget")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("mtd_budget")),
                                mtd_forecast = reader.IsDBNull(reader.GetOrdinal("mtd_forecast")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("mtd_forecast")),
                                ytd_budget = reader.IsDBNull(reader.GetOrdinal("ytd_budget")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("ytd_budget")),
                            };
                            rows.Add(row);
                        }
                    }
                    await connection.CloseAsync();
                }
            }
            return rows;
        }
        public async Task ProcessData(List<historical_dpr> datas)
        {
            string sqlDataSource = _configuration.GetConnectionString("MCS");
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlDataSource))
            {
                try
                {
                    foreach (var data in datas)
                    {
                        string id = $"{data.tanggal.ToString("yyyy-MM-dd")}#{data.area_name}#{data.entity}#{data.pit_contractor}";
                        data.id = id;
                        string lookupQuery = $"select id from historical_dpr where id = '{data.id}'";
                        object checkData;
                        using (NpgsqlCommand command = new NpgsqlCommand(lookupQuery, connection))
                        {
                             connection.Open();
                             checkData =  command.ExecuteScalar();
                             connection.Close();
                        }

                        if (checkData == null)
                        {
                            var insertQuery = @"INSERT INTO public.historical_dpr (
                                    id, tanggal, entity, business_unit_name, area_name, contractor_code, pit_contractor, 
                                    daily_actual, mtd_actual, ytd_actual, daily_budget, mtd_budget, mtd_forecast, ytd_budget,process,process_at
                                ) 
                                VALUES (
                                    @id, @tanggal, @entity, @business_unit_name, @area_name, @contractor_code, @pit_contractor, 
                                    @daily_actual, @mtd_actual, @ytd_actual, @daily_budget, @mtd_budget, @mtd_forecast, @ytd_budget,@process,@process_at
                            );";

                            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@id", data.id);
                                command.Parameters.AddWithValue("@tanggal", data.tanggal);
                                command.Parameters.AddWithValue("@entity", data.entity);
                                command.Parameters.AddWithValue("@business_unit_name", data.business_unit_name);
                                command.Parameters.AddWithValue("@area_name", data.area_name);
                                command.Parameters.AddWithValue("@contractor_code", data.contractor_code);
                                command.Parameters.AddWithValue("@pit_contractor", data.pit_contractor);
                                command.Parameters.AddWithValue("@daily_actual", (object)data.daily_actual ?? DBNull.Value);
                                command.Parameters.AddWithValue("@mtd_actual", (object)data.mtd_actual ?? DBNull.Value);
                                command.Parameters.AddWithValue("@ytd_actual", (object)data.ytd_actual ?? DBNull.Value);
                                command.Parameters.AddWithValue("@daily_budget", (object)data.daily_budget ?? DBNull.Value);
                                command.Parameters.AddWithValue("@mtd_budget", (object)data.mtd_budget ?? DBNull.Value);
                                command.Parameters.AddWithValue("@mtd_forecast", (object)data.mtd_forecast ?? DBNull.Value);
                                command.Parameters.AddWithValue("@ytd_budget", (object)data.ytd_budget ?? DBNull.Value);
                                command.Parameters.AddWithValue("@process", "INSERT");
                                command.Parameters.AddWithValue("@process_at", DateTime.Now);

                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();
                            }
                        }
                        else
                        {
                            string updateQuery = @$"UPDATE public.historical_dpr 
                                    SET 
                                        tanggal=@tanggal,
                                        entity=@entity,
                                        business_unit_name=@business_unit_name,
                                        area_name=@area_name,
                                        contractor_code=@contractor_code,
                                        pit_contractor=@pit_contractor,
                                        daily_actual=@daily_actual,
                                        mtd_actual=@mtd_actual,
                                        ytd_actual=@ytd_actual,
                                        daily_budget=@daily_budget,
                                        mtd_budget=@mtd_budget,
                                        mtd_forecast=@mtd_forecast,
                                        ytd_budget=@ytd_budget,
                                        process=@process,
                                        process_at=@process_at
                                    WHERE id=@id;";

                            using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@tanggal", data.tanggal);
                                command.Parameters.AddWithValue("@id", data.id);
                                command.Parameters.AddWithValue("@entity", data.entity);
                                command.Parameters.AddWithValue("@business_unit_name", data.business_unit_name);
                                command.Parameters.AddWithValue("@area_name", data.area_name);
                                command.Parameters.AddWithValue("@contractor_code", data.contractor_code);
                                command.Parameters.AddWithValue("@pit_contractor", data.pit_contractor);
                                command.Parameters.AddWithValue("@daily_actual", (object)data.daily_actual ?? DBNull.Value);
                                command.Parameters.AddWithValue("@mtd_actual", (object)data.mtd_actual ?? DBNull.Value);
                                command.Parameters.AddWithValue("@ytd_actual", (object)data.ytd_actual ?? DBNull.Value);
                                command.Parameters.AddWithValue("@daily_budget", (object)data.daily_budget ?? DBNull.Value);
                                command.Parameters.AddWithValue("@mtd_budget", (object)data.mtd_budget ?? DBNull.Value);
                                command.Parameters.AddWithValue("@mtd_forecast", (object)data.mtd_forecast ?? DBNull.Value);
                                command.Parameters.AddWithValue("@ytd_budget", (object)data.ytd_budget ?? DBNull.Value);
                                command.Parameters.AddWithValue("@process", "UPDATE");
                                command.Parameters.AddWithValue("@process_at", DateTime.Now);

                                connection.Open();
                                 command.ExecuteNonQuery();
                                 connection.Close();
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }
        /*protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await RefreshView();
                await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
            }
        }

        public async Task RefreshView()
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<mcsContext>();
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    await using var cmd = conn.CreateCommand();
                    try
                    {
                        cmd.CommandText = @"select * from get_dpr_data(current_date)";
                        await using var reader = await cmd.ExecuteReaderAsync();

                        var historical_dprs = new List<historical_dpr>();

                        while (await reader.ReadAsync())
                        {
                            var historical_dpr = new historical_dpr
                            {
                                tanggal = reader.GetDateTime(reader.GetOrdinal("tanggal")),
                                entity = reader.GetString(reader.GetOrdinal("entity")),
                                business_unit_name = reader.GetString(reader.GetOrdinal("business_unit_name")),
                                area_name = reader.GetString(reader.GetOrdinal("area_name")),
                                contractor_code = reader.GetString(reader.GetOrdinal("contractor_code")),
                                pit_contractor = reader.GetString(reader.GetOrdinal("pit_contractor")),
                                daily_actual = reader.GetDecimal(reader.GetOrdinal("daily_actual")),
                                mtd_actual = reader.GetDecimal(reader.GetOrdinal("mtd_actual")),
                                ytd_actual = reader.GetDecimal(reader.GetOrdinal("ytd_actual")),
                                daily_budget = reader.GetDecimal(reader.GetOrdinal("daily_budget")),
                                mtd_budget = reader.GetDecimal(reader.GetOrdinal("mtd_budget")),
                                mtd_forecast = reader.GetDecimal(reader.GetOrdinal("mtd_forecast")),
                                ytd_budget = reader.GetDecimal(reader.GetOrdinal("ytd_budget")),
                            };

                            historical_dprs.Add(historical_dpr);
                        }

                        // Insert or update in the database
                        foreach (var dpr in historical_dprs)
                        {
                            string id = $"{dpr.tanggal}#{dpr.area_name}#{dpr.pit_contractor}";
                            dpr.id = id;
                            var existingDpr = await dbContext.historical_dpr
                                .FirstOrDefaultAsync(x => x.id == dpr.id);

                            if (existingDpr == null)
                            {
                                dpr.process = "INSERT";
                                dpr.process_at = DateTime.Now;
                                dbContext.historical_dpr.Add(dpr);
                                await dbContext.SaveChangesAsync();
                            }
                            else
                            {
                                existingDpr.process = "UPDATE";
                                existingDpr.process_at = DateTime.Now;
                                existingDpr.daily_budget = dpr.daily_budget;
                                existingDpr.mtd_actual = dpr.mtd_actual;
                                existingDpr.ytd_actual = dpr.ytd_actual;
                                existingDpr.daily_budget = dpr.daily_budget;
                                existingDpr.mtd_budget = dpr.mtd_budget;
                                existingDpr.mtd_forecast = dpr.mtd_forecast;
                                existingDpr.ytd_budget = dpr.ytd_budget;
                                await dbContext.SaveChangesAsync();
                            }
                        }

                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error refreshing materialized view");
                    }
                }
                _logger.LogInformation("Refresh Material Views have been done successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update failed");
            }
        }*/
    }
}

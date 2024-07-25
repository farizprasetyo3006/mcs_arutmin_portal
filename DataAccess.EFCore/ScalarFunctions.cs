using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NLog;

namespace DataAccess.EFCore.Repository
{
    public partial class mcsContext
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static async Task<bool> CanCreate(DbContext dbContext, string entityName, string applicationUserId)
        {
            var result = false;

            try
            {
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {
                            entityName = entityName.Substring(0, 1).Replace("_", "") + entityName.Substring(1);
                            cmd.CommandText = $"SELECT ufn_can_create('{entityName}', '{applicationUserId}');";

                            var r = await cmd.ExecuteScalarAsync();
                            if (r != null)
                            {
                                result = (bool)r;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public static async Task<bool> CanCreate(DbContext dbContext, string entityName, string applicationUserId, string businessUnitId)
        {
            var result = false;

            try
            {
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {
                            entityName = entityName.Substring(0, 1).Replace("_", "") + entityName.Substring(1);
                            cmd.CommandText = $"SELECT ufn_can_create('{entityName}', '{applicationUserId}', '{businessUnitId}');";

                            var r = await cmd.ExecuteScalarAsync();
                            if (r != null)
                            {
                                result = (bool)r;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public static async Task<bool> CanRead(DbContext dbContext, string entityId, string applicationUserId)
        {
            var result = false;

            try
            {
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = $"SELECT ufn_can_read('{entityId}', '{applicationUserId}');";
                            var r = await cmd.ExecuteScalarAsync();
                            if (r != null)
                            {
                                result = (bool)r;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public static async Task<bool> CanRead(DbContext dbContext, string entityId, string applicationUserId, string businessUnitId)
        {
            var result = false;

            try
            {
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = $"SELECT ufn_can_read('{entityId}', '{applicationUserId}', '{businessUnitId}');";
                            var r = await cmd.ExecuteScalarAsync();
                            if (r != null)
                            {
                                result = (bool)r;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public static async Task<bool> CanUpdate(DbContext dbContext, string entityId, string applicationUserId)
        {
            var result = false;

            try
            {
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = $"SELECT ufn_can_update('{entityId}', '{applicationUserId}');";
                            var r = await cmd.ExecuteScalarAsync();
                            if (r != null)
                            {
                                result = (bool)r;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public static async Task<bool> CanUpdate(DbContext dbContext, string entityId, string applicationUserId, string businessUnitId)
        {
            var result = false;

            try
            {
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = $"SELECT ufn_can_update('{entityId}', '{applicationUserId}', '{businessUnitId}');";
                            var r = await cmd.ExecuteScalarAsync();
                            if (r != null)
                            {
                                result = (bool)r;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public static async Task<bool> CanDelete(DbContext dbContext, string entityId, string applicationUserId)
        {
            var result = false;

            try
            {
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = $"SELECT ufn_can_delete('{entityId}', '{applicationUserId}');";
                            var r = await cmd.ExecuteScalarAsync();
                            if (r != null)
                            {
                                result = (bool)r;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public static async Task<bool> CanDelete(DbContext dbContext, string entityId, string applicationUserId, string businessUnitId)
        {
            var result = false;

            try
            {
                var conn = dbContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = $"SELECT ufn_can_delete('{entityId}', '{applicationUserId}', '{businessUnitId}');";
                            var r = await cmd.ExecuteScalarAsync();
                            if (r != null)
                            {
                                result = (bool)r;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

    }
}

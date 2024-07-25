using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using NLog;

namespace DataAccess.EFCore.Repository
{
    public static class CustomFunctions
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static bool CanCreate(string entityName, string applicationUserId)
            => throw new Exception($"{nameof(CanCreate)} cannot be called client side");

        public static bool CanRead(string recordId, string applicationUserId)
            => throw new Exception($"{nameof(CanRead)} cannot be called client side");

        public static bool CanRead(string recordId, string applicationUserId, string businessUnitId)
            => throw new Exception($"{nameof(CanRead)} cannot be called client side");

        public static bool CanUpdate(string recordId, string applicationUserId)
            => throw new Exception($"{nameof(CanUpdate)} cannot be called client side");

        public static bool CanDelete(string recordId, string applicationUserId)
            => throw new Exception($"{nameof(CanDelete)} cannot be called client side");

        public static void RegisterFunctions(ModelBuilder modelBuilder)
        {
            try
            {
                modelBuilder
                    .HasDbFunction(typeof(CustomFunctions)
                        .GetRuntimeMethod(nameof(CustomFunctions.CanCreate),
                            new[] { typeof(string), typeof(string) }))
                    .HasTranslation(args => SqlFunctionExpression.Create(
                        modelBuilder.Model.GetDefaultSchema(), "ufn_can_create",
                        args, typeof(bool), null));

                modelBuilder
                    .HasDbFunction(typeof(CustomFunctions)
                        .GetRuntimeMethod(nameof(CustomFunctions.CanRead),
                            new[] { typeof(string), typeof(string) }))
                    .HasTranslation(args => SqlFunctionExpression.Create(
                        modelBuilder.Model.GetDefaultSchema(), "ufn_can_read",
                        args, typeof(bool), null));

                modelBuilder
                    .HasDbFunction(typeof(CustomFunctions)
                        .GetRuntimeMethod(nameof(CustomFunctions.CanRead),
                            new[] { typeof(string), typeof(string), typeof(string) }))
                    .HasTranslation(args => SqlFunctionExpression.Create(
                        modelBuilder.Model.GetDefaultSchema(), "ufn_can_read",
                        args, typeof(bool), null));

                modelBuilder
                    .HasDbFunction(typeof(CustomFunctions)
                        .GetRuntimeMethod(nameof(CustomFunctions.CanUpdate),
                            new[] { typeof(string), typeof(string) }))
                    .HasTranslation(args => SqlFunctionExpression.Create(
                        modelBuilder.Model.GetDefaultSchema(), "ufn_can_update",
                        args, typeof(bool), null));

                modelBuilder
                    .HasDbFunction(typeof(CustomFunctions)
                        .GetRuntimeMethod(nameof(CustomFunctions.CanDelete),
                            new[] { typeof(string), typeof(string) }))
                    .HasTranslation(args => SqlFunctionExpression.Create(
                        modelBuilder.Model.GetDefaultSchema(), "ufn_can_delete",
                        args, typeof(bool), null));

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}

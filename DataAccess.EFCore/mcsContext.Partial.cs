using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataAccess.EFCore.Repository
{
    public partial class mcsContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            CustomFunctions.RegisterFunctions(modelBuilder);
        }
    }
}

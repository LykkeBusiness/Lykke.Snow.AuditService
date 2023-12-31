// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Data.Common;
using Lykke.Common.MsSql;
using Lykke.Snow.AuditService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Snow.AuditService.SqlRepositories
{
    public class AuditDbContext : MsSqlContext
    {
        private const string Schema = "audit";

        public AuditDbContext() : base(Schema)
        {
        }

        public AuditDbContext(string connectionString, bool isTracingEnabled) : base(Schema, connectionString,
            isTracingEnabled)
        {
        }

        public AuditDbContext(DbContextOptions contextOptions) : base(Schema, contextOptions)
        {
        }

        public AuditDbContext(DbConnection dbConnection) : base(Schema, dbConnection)
        {
        }

        public DbSet<AuditEventEntity> Events => Set<AuditEventEntity>();
        public DbSet<AuditObjectStateEntity> AuditObjectStates => Set<AuditObjectStateEntity>();


        protected override void OnLykkeModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditDbContext).Assembly);
        }
    }
}

// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Lykke.Snow.AuditService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lykke.Snow.AuditService.SqlRepositories.EntityConfigurations
{
    public class AuditEventEntityConfiguration : IEntityTypeConfiguration<AuditEventEntity>
    {
        public void Configure(EntityTypeBuilder<AuditEventEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Timestamp).HasColumnType("datetime2").IsRequired();
            builder.Property(x => x.CorrelationId).IsRequired(false);
            builder.Property(x => x.UserName).IsRequired(false);
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.AuditEventTypeDetails).HasMaxLength(200).IsRequired();
            builder.Property(x => x.DataType).HasConversion<string>().HasMaxLength(100).IsRequired();
            builder.Property(x => x.DataReference).HasMaxLength(200).IsRequired();
            builder.Property(x => x.DataDiff).IsRequired(false);
        }
    }
}
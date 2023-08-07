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
            builder.Property(x => x.Timestamp).HasColumnType("datetime2").IsRequired();
            builder.Property(x => x.CorrelationId).IsRequired();
            builder.Property(x => x.CorporateActionsId).IsRequired(false);
            builder.Property(x => x.Username).IsRequired(false);
            builder.Property(x => x.ActionType).HasMaxLength(200).IsRequired();
            builder.Property(x => x.ActionTypeDetails).HasMaxLength(200).IsRequired();
            builder.Property(x => x.DataType).HasMaxLength(200).IsRequired();
            builder.Property(x => x.DataReference).HasMaxLength(200).IsRequired();
            builder.Property(x => x.DataDiff).IsRequired(false);
        }
    }
}
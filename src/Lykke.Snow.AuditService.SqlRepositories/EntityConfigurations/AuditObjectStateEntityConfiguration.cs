using Lykke.Snow.AuditService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lykke.Snow.AuditService.SqlRepositories.EntityConfigurations
{
    public class AuditObjectStateEntityConfiguration : IEntityTypeConfiguration<AuditObjectStateEntity>
    {
        public void Configure(EntityTypeBuilder<AuditObjectStateEntity> builder)
        {
            builder.HasKey(x => x.Oid);
            builder.HasIndex(x => new { x.DataType, x.DataReference }).IsUnique();
            builder.Property(x => x.DataType).HasConversion<string>();
            builder.Property(x => x.Oid).ValueGeneratedOnAdd();
            builder.Property(x => x.LastModified).HasColumnType("datetime2");
        }
    }
}
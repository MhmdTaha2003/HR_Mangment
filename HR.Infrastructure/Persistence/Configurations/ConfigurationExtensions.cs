using HR.Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

internal static class ConfigurationExtensions
{
    public static void ConfigureAuditableEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseAuditableEntity
    {
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.CreatedBy).HasMaxLength(100);
        builder.Property(entity => entity.UpdatedBy).HasMaxLength(100);
    }
}

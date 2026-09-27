using HR.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
{
    public void Configure(EntityTypeBuilder<DocumentType> builder)
    {
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(20);
        builder.Property(x => x.NameEn).IsRequired().HasMaxLength(150);
        builder.Property(x => x.NameAr).IsRequired().HasMaxLength(150);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

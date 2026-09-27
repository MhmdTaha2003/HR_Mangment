using HR.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(30);
        builder.Property(x => x.NameEn).IsRequired().HasMaxLength(150);
        builder.Property(x => x.NameAr).IsRequired().HasMaxLength(150);
        builder.Property(x => x.AddressEn)
        .HasMaxLength(300);
        builder.Property(x => x.AddressAr)
            .HasMaxLength(300);
        builder.Property(x => x.Phone)
            .HasMaxLength(30);
        builder.Property(x => x.Email)
            .HasMaxLength(150);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

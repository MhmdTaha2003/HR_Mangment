using HR.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(20);
        builder.Property(x => x.NameEn).IsRequired().HasMaxLength(150);
        builder.Property(x => x.NameAr).IsRequired().HasMaxLength(150);
        builder.Property(x => x.DefaultDays).HasPrecision(5, 2);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

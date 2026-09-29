using HR.Domain.Entity;
using HR.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace HR.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.EmployeeNumber).IsRequired().HasMaxLength(30);
        builder.Property(x => x.FirstNameEn).IsRequired().HasMaxLength(100);
        builder.Property(x => x.MiddleNameEn).HasMaxLength(100);
        builder.Property(x => x.LastNameEn).IsRequired().HasMaxLength(100);
        builder.Property(x => x.FirstNameAr).IsRequired().HasMaxLength(100);
        builder.Property(x => x.MiddleNameAr).HasMaxLength(100);
        builder.Property(x => x.LastNameAr).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.PhoneNumber).HasMaxLength(30);
        builder.Property(x => x.TerminationDate).HasColumnType("date");
        builder.Property(x => x.Gender).HasConversion<int>();
        builder.Property(x => x.EmploymentStatus).HasConversion<int>();
        builder.HasIndex(x => x.EmployeeNumber).IsUnique();
    }
}

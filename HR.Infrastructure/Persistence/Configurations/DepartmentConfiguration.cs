using HR.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(20);
        builder.Property(x => x.NameEn).IsRequired().HasMaxLength(150);
        builder.Property(x => x.NameAr).IsRequired().HasMaxLength(150);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasOne(x => x.Branch).WithMany(x => x.Departments)
            .HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ParentDepartment).WithMany(x => x.ChildDepartments)
            .HasForeignKey(x => x.ParentDepartmentId).OnDelete(DeleteBehavior.Restrict);
    }
}

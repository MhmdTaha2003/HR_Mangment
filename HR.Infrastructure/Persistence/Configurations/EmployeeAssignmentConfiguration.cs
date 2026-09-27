using HR.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public class EmployeeAssignmentConfiguration : IEntityTypeConfiguration<EmployeeAssignment>
{
    public void Configure(EntityTypeBuilder<EmployeeAssignment> builder)
    {
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.AssignmentType).HasConversion<int>();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => x.EmployeeId).IsUnique().HasFilter("[IsCurrent] = 1");
        builder.HasOne(x => x.Employee).WithMany(x => x.Assignments)
            .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Branch).WithMany(x => x.EmployeeAssignments)
            .HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Department).WithMany(x => x.EmployeeAssignments)
            .HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.JobTitle).WithMany(x => x.EmployeeAssignments)
            .HasForeignKey(x => x.JobTitleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ManagerEmployee).WithMany(x => x.ManagedAssignments)
            .HasForeignKey(x => x.ManagerEmployeeId).OnDelete(DeleteBehavior.Restrict);
    }
}

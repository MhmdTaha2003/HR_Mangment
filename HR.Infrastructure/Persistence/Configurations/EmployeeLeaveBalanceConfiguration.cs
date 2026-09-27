using HR.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public class EmployeeLeaveBalanceConfiguration : IEntityTypeConfiguration<EmployeeLeaveBalance>
{
    public void Configure(EntityTypeBuilder<EmployeeLeaveBalance> builder)
    {
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.EntitledDays).HasPrecision(5, 2);
        builder.Property(x => x.CarriedForwardDays).HasPrecision(5, 2);
        builder.Property(x => x.UsedDays).HasPrecision(5, 2);
        builder.Property(x => x.AdjustedDays).HasPrecision(5, 2);
        builder.Ignore(x => x.RemainingBalance);
        builder.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.Year }).IsUnique();
        builder.HasOne(x => x.Employee).WithMany(x => x.LeaveBalances)
            .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.LeaveType).WithMany(x => x.EmployeeLeaveBalances)
            .HasForeignKey(x => x.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}

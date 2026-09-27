using HR.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.RequestedDays).HasPrecision(5, 2);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.AttachmentPath).HasMaxLength(500);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.HasOne(x => x.Employee).WithMany(x => x.LeaveRequests)
            .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.LeaveType).WithMany(x => x.LeaveRequests)
            .HasForeignKey(x => x.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}

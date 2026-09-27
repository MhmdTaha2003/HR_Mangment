using HR.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public class LeaveRequestApprovalConfiguration : IEntityTypeConfiguration<LeaveRequestApproval>
{
    public void Configure(EntityTypeBuilder<LeaveRequestApproval> builder)
    {
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.Action).HasConversion<int>();
        builder.Property(x => x.Comments).HasMaxLength(1000);
        builder.HasIndex(x => new { x.LeaveRequestId, x.ApprovalLevel }).IsUnique();
        builder.HasOne(x => x.LeaveRequest).WithMany(x => x.Approvals)
            .HasForeignKey(x => x.LeaveRequestId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ApproverEmployee).WithMany(x => x.ApprovalsGiven)
            .HasForeignKey(x => x.ApproverEmployeeId).OnDelete(DeleteBehavior.Restrict);
    }
}

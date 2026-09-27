using HR.Domain.Common;
using HR.Domain.Enums;

namespace HR.Domain.Entity;

public class LeaveRequest : BaseAuditableEntity
{
    public long EmployeeId { get; set; }
    public long LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal RequestedDays { get; set; }
    public string? Reason { get; set; }
    public string? AttachmentPath { get; set; }
    public LeaveRequestStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public Employee Employee { get; set; } = null!;
    public LeaveType LeaveType { get; set; } = null!;
    public ICollection<LeaveRequestApproval> Approvals { get; set; } = new List<LeaveRequestApproval>();
}

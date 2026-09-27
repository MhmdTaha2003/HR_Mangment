using HR.Domain.Common;
using HR.Domain.Enums;

namespace HR.Domain.Entity;

public class LeaveRequestApproval : BaseAuditableEntity
{
    public long LeaveRequestId { get; set; }
    public int ApprovalLevel { get; set; }
    public long ApproverEmployeeId { get; set; }
    public ApprovalAction Action { get; set; } = ApprovalAction.Pending;
    public DateTime? ActionDate { get; set; }
    public string? Comments { get; set; }
    public LeaveRequest LeaveRequest { get; set; } = null!;
    public Employee ApproverEmployee { get; set; } = null!;
}

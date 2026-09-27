using HR.Domain.Enums;

namespace HR.Application.DTOs.LeaveRequestApprovals;
public record LeaveRequestApprovalDto(long Id, long LeaveRequestId, int ApprovalLevel, long ApproverEmployeeId, ApprovalAction Action, DateTime? ActionDate, string? Comments);

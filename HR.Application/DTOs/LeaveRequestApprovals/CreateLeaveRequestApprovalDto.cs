using HR.Domain.Enums;

namespace HR.Application.DTOs.LeaveRequestApprovals;
public record CreateLeaveRequestApprovalDto(long LeaveRequestId, int ApprovalLevel, long ApproverEmployeeId, ApprovalAction Action, string? Comments);

using HR.Domain.Enums;

namespace HR.Application.DTOs.LeaveRequestApprovals;
public record UpdateLeaveRequestApprovalDto(ApprovalAction Action, string? Comments);

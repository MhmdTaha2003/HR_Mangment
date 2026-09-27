using HR.Domain.Enums;

namespace HR.Application.DTOs.LeaveRequests;
public record UpdateLeaveRequestDto(DateTime StartDate, DateTime EndDate, decimal RequestedDays, string? Reason, string? AttachmentPath, LeaveRequestStatus Status);

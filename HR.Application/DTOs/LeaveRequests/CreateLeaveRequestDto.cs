using HR.Domain.Enums;

namespace HR.Application.DTOs.LeaveRequests;
public record CreateLeaveRequestDto(long EmployeeId, long LeaveTypeId, DateTime StartDate, DateTime EndDate, decimal RequestedDays, string? Reason, string? AttachmentPath);

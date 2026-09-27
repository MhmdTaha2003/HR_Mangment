namespace HR.Application.DTOs.LeaveTypes;
public record UpdateLeaveTypeDto(string NameEn, string NameAr, decimal DefaultDays, bool IsPaid, bool RequiresApproval, bool RequiresAttachment, bool IsActive);

namespace HR.Application.DTOs.LeaveTypes;
public record LeaveTypeDto(long Id, string Code, string NameEn, string NameAr, decimal DefaultDays, bool IsPaid, bool RequiresApproval, bool RequiresAttachment, bool IsActive);

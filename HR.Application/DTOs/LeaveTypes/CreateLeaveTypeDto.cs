namespace HR.Application.DTOs.LeaveTypes;
public record CreateLeaveTypeDto(string Code, string NameEn, string NameAr, decimal DefaultDays, bool IsPaid, bool RequiresApproval, bool RequiresAttachment);

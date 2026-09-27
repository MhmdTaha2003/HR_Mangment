using HR.Domain.Common;

namespace HR.Domain.Entity;

public class LeaveType : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public decimal DefaultDays { get; set; }
    public bool IsPaid { get; set; }
    public bool RequiresApproval { get; set; } = true;
    public bool RequiresAttachment { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; } = new List<EmployeeLeaveBalance>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}

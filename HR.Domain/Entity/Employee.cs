using HR.Domain.Common;
using HR.Domain.Enums;

namespace HR.Domain.Entity;

public class Employee : BaseAuditableEntity
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstNameEn { get; set; } = string.Empty;
    public string? MiddleNameEn { get; set; }
    public string LastNameEn { get; set; } = string.Empty;
    public string FirstNameAr { get; set; } = string.Empty;
    public string? MiddleNameAr { get; set; }
    public string LastNameAr { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime HireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public Gender Gender { get; set; }
    public EmploymentStatus EmploymentStatus { get; set; }
    public ICollection<EmployeeAssignment> Assignments { get; set; } = new List<EmployeeAssignment>();
    public ICollection<EmployeeAssignment> ManagedAssignments { get; set; } = new List<EmployeeAssignment>();
    public ICollection<EmployeeContract> Contracts { get; set; } = new List<EmployeeContract>();
    public ICollection<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();
    public ICollection<EmployeeLeaveBalance> LeaveBalances { get; set; } = new List<EmployeeLeaveBalance>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<LeaveRequestApproval> ApprovalsGiven { get; set; } = new List<LeaveRequestApproval>();
}
